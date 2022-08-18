using System.Collections.Generic;
using ETLLibrary.Model.Pipeline;
using ETLLibrary.Model.Pipeline.Nodes.Destinations.Csv;
using ETLLibrary.Model.Pipeline.Nodes.Sources.Csv;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Aggregations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Joins;
using Xunit;

namespace TestETLLibrary
{
    public class PipelineTests
    {
        [Fact]
        public void TestAggregation() // just for my local testing
        {
            Pipeline pipeline = new Pipeline(1, "sample");
            CsvSource csvSource = new CsvSource("1", "source from sample csv", "demo.csv" , ",");
            AggregationNode aggregationNode =
                new AggregationNode("2", "simple sum", AggregationType.Sum, "Period", "sum",
                    new List<string>() {"Series_reference"});
            CsvDestination csvDestination = new CsvDestination("3", "sample destination", "modified.csv" , ",");
            pipeline.AddNode(csvSource);
            pipeline.AddNode(csvDestination);
            pipeline.AddNode(aggregationNode);
            pipeline.LinkNodes("1", "2");
            pipeline.LinkNodes("2", "3");
            pipeline.Run();
        }

        [Fact]
        public void TestJoin() // just for my local testing
        {
            Pipeline pipeline = new Pipeline(1, "sample");
            CsvSource csvSource1 = new CsvSource("1", "source from sample csv", "join_1.csv" , ",");
            CsvSource csvSource2 = new CsvSource("2", "source from sample csv", "join_2.csv" , ",");
            JoinNode joinNode = new JoinNode("3","simple join" , JoinType.FullJoin , "id" , "id");
            CsvDestination csvDestination = new CsvDestination("4" , "" , "join_final.csv" , ",");
            pipeline.AddNode(csvSource1);
            pipeline.AddNode(csvSource2);
            pipeline.AddNode(joinNode);
            pipeline.AddNode(csvDestination);
            pipeline.LinkNodes("3" , "4");
            pipeline.LinkNodesForJoin("1" , "2" , "3");
            pipeline.Run();
        }

        [Fact]
        public void TestPipelineConvertor()
        {
            
        }
    }
}