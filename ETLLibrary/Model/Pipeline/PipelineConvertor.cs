using System;
using System.Collections.Generic;
using System.Linq;
using ETLLibrary.Database.Utils;
using ETLLibrary.Enums;
using ETLLibrary.Model.Pipeline.Nodes.Destinations;
using ETLLibrary.Model.Pipeline.Nodes.Destinations.Csv;
using ETLLibrary.Model.Pipeline.Nodes.Destinations.Sql;
using ETLLibrary.Model.Pipeline.Nodes.Sources;
using ETLLibrary.Model.Pipeline.Nodes.Sources.Csv;
using ETLLibrary.Model.Pipeline.Nodes.Sources.Sql;
using ETLLibrary.Model.Pipeline.Nodes.Transformations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Aggregations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Conditions;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Enums;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Joins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Type = ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Enums.Type;

namespace ETLLibrary.Model.Pipeline
{
    public class PipelineConvertor
    {
        private string _username;
        private string _jsonString;
        private Dictionary<string, string> _joinNodeToSecondDataSet = new();
        public Pipeline Pipeline { get; }

        public PipelineConvertor(string username, string jsonString)
        {
            _username = username;
            _jsonString = jsonString;
            Pipeline = new Pipeline(1, "");
            JObject jObject = JsonConvert.DeserializeObject<JObject>(_jsonString);
            JArray nodes = (JArray) jObject["nodes"];
            CreatePipelineNodes(nodes);
            JArray edges = (JArray) jObject["edges"];
            CreateEdges(edges);
        }

        private void CreatePipelineNodes(JArray nodes)
        {
            int nodesSize = nodes.Count;
            for (int i = 0; i < nodesSize; ++i)
            {
                JToken node = nodes[i];
                AddNode(node);
            }
        }

        private void AddNode(JToken node)
        {
            string id = node["id"].ToString();
            string type = node["data"]["type"]?.ToString();
            if (type == "filter")
            {
                CreateFilterNode(node);
            }

            if (type == "join")
            {
                CreateJoinNode(node);
            }

            if (type == "aggregate")
            {
                CreateAggregationNode(node);
            }

            if (type == "common" && id == "source")
            {
                CreateSourceNode(node);
            }

            if (type == "common" && id == "destination")
            {
                CreateDestinationNode(node);
            }
        }

        private void CreateFilterNode(JToken node)
        {
            string id = node["id"].ToString();
            Condition condition = GetCondition(node["data"]["filterTree"]);
            FilterNode filterNode = new FilterNode(id, "", condition);
            Pipeline.AddNode(filterNode);
        }

        private Condition GetCondition(JToken tree)
        {
            JArray nodes = (JArray) tree["nodes"];
            int nodesSize = nodes.Count;

            if (nodesSize == 1)
                return GetSingleCondition(nodes[0]);
            JArray edges = (JArray) tree["edges"];
            int edgesSize = edges.Count;
            Queue<string> queue = new Queue<string>();
            Dictionary<string, Condition> conditionMapper = new();
            Dictionary<string, List<Condition>> childMapper = new();
            Dictionary<string, List<string>> adjacentIds = new();
            Dictionary<string, LogicOperator> conditionOperator = new();
            for (int i = 0; i < nodesSize; ++i)
            {
                JToken node = nodes[i];
                adjacentIds[node["id"]?.ToString() ?? string.Empty] = new();
                if (node["data"]?["type"]?.ToString() != "condition")
                {
                    if (node["data"]?["type"]?.ToString() == "or")
                        conditionOperator[node["id"]?.ToString() ?? string.Empty] = LogicOperator.Or;
                    else if (node["data"]?["type"]?.ToString() == "and")
                        conditionOperator[node["id"]?.ToString() ?? string.Empty] = LogicOperator.And;
                    else
                        throw new NotImplementedException("logic operator not supported");
                    continue;
                }

                conditionMapper[node["id"]?.ToString() ?? string.Empty] = GetSingleCondition(node);
                queue.Enqueue(node["id"]?.ToString());
            }

            for (int i = 0; i < edgesSize; ++i)
            {
                JToken edge = edges[i];
                string source = edge["source"]?.ToString();
                string target = edge["target"]?.ToString();
                adjacentIds[source ?? string.Empty].Add(target);
                adjacentIds[target ?? string.Empty].Add(source);
            }


            while (queue.Count > 0)
            {
                string id = queue.Dequeue();
                int count = adjacentIds[id].Count(x => !conditionMapper.ContainsKey(x));
                if (count == 0)
                    return conditionMapper[id];
                string parent = adjacentIds[id].First(x => !conditionMapper.ContainsKey(x));
                if (!childMapper.ContainsKey(parent))
                    childMapper[parent] = new();
                childMapper[parent].Add(conditionMapper[id]);
                if (childMapper[parent].Count == 2)
                {
                    conditionMapper[parent] = new MultipleCondition(childMapper[parent][0], childMapper[parent][1],
                        conditionOperator[parent]);
                    queue.Enqueue(parent);
                }
            }

            throw new Exception("impossible to reach");
        }

        private SingleCondition GetSingleCondition(JToken node)
        {
            JToken data = node["data"];
            string operation = data["operation"]?.ToString();
            Operator op;
            if (operation == ">")
                op = Operator.GreaterThan;
            else if (operation == "<")
                op = Operator.LessThan;
            else if (operation == "=")
                op = Operator.Equals;
            else
                throw new NotImplementedException("operator not supported");
            SingleCondition singleCondition;
            if (op == Operator.Equals)
            {
                singleCondition =
                    new SingleCondition(data["column"]?.ToString(), op, data["value"]?.ToString(), Type.String);
            }
            else
            {
                singleCondition =
                    new SingleCondition(data["column"]?.ToString(), op, data["value"]?.ToString(), Type.Double);
            }

            return singleCondition;
        }

        private void CreateJoinNode(JToken node)
        {
            JToken data = node["data"];
            _joinNodeToSecondDataSet[node["id"].ToString()] = data["dataset"]?.ToString();
            JoinNode joinNode = new JoinNode(node["id"].ToString(), "", JoinType.InnerJoin,
                data["rightKey"]?.ToString(), data["leftKey"]?.ToString());
            Pipeline.AddNode(joinNode);
        }

        private void CreateAggregationNode(JToken node)
        {
            AggregationType aggregationType;
            if (node["data"]["operation"]?.ToString() == "sum")
                aggregationType = AggregationType.Sum;
            else if (node["data"]["operation"]?.ToString() == "count")
                aggregationType = AggregationType.Count;
            else if (node["data"]["operation"]?.ToString() == "min")
                aggregationType = AggregationType.Min;
            else if (node["data"]["operation"]?.ToString() == "max")
                aggregationType = AggregationType.Max;
            else if (node["data"]["operation"]?.ToString() == "average")
                aggregationType = AggregationType.Average;
            else
                throw new NotImplementedException();
            TransformationNode transformationNode = new AggregationNode(node["id"].ToString(), "", aggregationType,
                node["data"]["column"]?.ToString(), node["data"]["outputName"]?.ToString(),
                node["data"]["groupColumns"]?.ToObject<List<string>>());
            Pipeline.AddNode(transformationNode);
        }

        private void CreateSourceNode(JToken node)
        {
            string datasetName = node["data"]["name"]?.ToString();
            SourceNode sourceNode;
            if (Dataset.TypeOf(_username, datasetName) == DatasetType.Csv)
            {
                sourceNode = new CsvSource(node["id"].ToString(), "",
                    PipelineConfigurator.GetCsvPath(_username, datasetName) , PipelineConfigurator.GetCsvDelimiter(_username , datasetName));
            }
            else
            {
                ConnectionInfo connectionInfo = PipelineConfigurator.GetConnectionString(_username, datasetName);
                sourceNode = new SqlSource(node["id"].ToString(), "",
                    connectionInfo.ConnectionString, connectionInfo.TableName);
            }

            Pipeline.AddNode(sourceNode);
        }

        private void CreateDestinationNode(JToken node)
        {
            string datasetName = node["data"]["name"]?.ToString();
            DestinationNode destinationNode;

            if (Dataset.TypeOf(_username, datasetName) == DatasetType.Csv)
            {
                destinationNode = new CsvDestination(node["id"].ToString(), "",
                    PipelineConfigurator.GetCsvPath(_username, datasetName) , PipelineConfigurator.GetCsvDelimiter(_username , datasetName));
            }
            else
            {
                ConnectionInfo connectionInfo = PipelineConfigurator.GetConnectionString(_username, datasetName);
                destinationNode = new SqlDestination(node["id"].ToString(), "", connectionInfo.ConnectionString,
                    connectionInfo.TableName);
            }

            Pipeline.AddNode(destinationNode);
        }

        private void CreateEdges(JArray edges)
        {
            int edgesSize = edges.Count;
            Dictionary<string, string> nextMapper = new();
            for (int i = 0; i < edgesSize; ++i)
            {
                JToken edge = edges[i];
                nextMapper[edge["source"]?.ToString() ?? string.Empty] = edge["target"]?.ToString();
            }

            for (int i = 0; i < edgesSize; ++i)
            {
                JToken edge = edges[i];
                if (edge["source"]?.ToString()[..3] != "add")
                {
                    if (nextMapper[edge["target"]?.ToString() ?? string.Empty][..4] == "join")
                    {
                        string joinNodeName = nextMapper[edge["target"]?.ToString() ?? string.Empty];
                        string datasetName = _joinNodeToSecondDataSet[joinNodeName];
                        SourceNode sourceNode;
                        if (Dataset.TypeOf(_username, datasetName) == DatasetType.Csv)
                        {
                            sourceNode = new CsvSource("source" + joinNodeName, "",
                                PipelineConfigurator.GetCsvPath(_username, datasetName) , PipelineConfigurator.GetCsvDelimiter(_username , datasetName));
                        }
                        else
                        {
                            ConnectionInfo connectionInfo =
                                PipelineConfigurator.GetConnectionString(_username, datasetName);
                            sourceNode = new SqlSource("source" + joinNodeName, "", connectionInfo.ConnectionString,
                                connectionInfo.TableName);
                        }

                        Pipeline.AddNode(sourceNode);
                        Pipeline.LinkNodesForJoin(edge["source"].ToString(), "source" + joinNodeName, joinNodeName);
                    }
                    else
                    {
                        Pipeline.LinkNodes(edge["source"]?.ToString(),
                            nextMapper[edge["target"]?.ToString() ?? string.Empty]);
                    }
                }
            }
        }
    }
}