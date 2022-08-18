using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ETLBox.DataFlow;
using ETLBox.DataFlow.Transformations;
using ETLLibrary.Model.Pipeline.Nodes;
using ETLLibrary.Model.Pipeline.Nodes.Destinations;
using ETLLibrary.Model.Pipeline.Nodes.Sources;
using ETLLibrary.Model.Pipeline.Nodes.Transformations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Joins;

namespace ETLLibrary.Model.Pipeline
{
    public class Pipeline
    {
        private int _id;
        private string _name;
        private List<Node> _nodes = new();
        private List<DataFlowSource<ExpandoObject>> _sourceNodes;
        public Pipeline(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public void LinkNodes(string sourceId, string destinationId)
        {
            Node sourceNode = _nodes.First(x => x.Id == sourceId);
            Node destinationNode = _nodes.First(x => x.Id == destinationId);
            TryLinking(sourceNode, destinationNode);
        }

        public void LinkNodesForJoin(string firstSourceId, string secondSourceId, string joinNodeId) // only used for join
        {
            Node firstSource = _nodes.First(x => x.Id == firstSourceId);
            Node secondSource = _nodes.First(x => x.Id == secondSourceId);
            JoinNode joinNode = (JoinNode) _nodes.First(x => x.Id == joinNodeId);
            LinkJoinTarget(firstSource , joinNode.leftSort);
            LinkJoinTarget(secondSource , joinNode.rightSort);
        }

        private void LinkJoinTarget(Node source, Sort<ExpandoObject> joinTarget)
        {
            try
            {
                if (((TransformationNode) source).DataFlow == null)
                {
                    ((JoinNode) source).MergeJoin.LinkTo(joinTarget);
                }
                else
                {
                    ((TransformationNode) source).DataFlow.LinkTo(joinTarget);
                }
            }
            catch (InvalidCastException)
            {
            }
            try
            {
                ((SourceNode) source).DataFlow.LinkTo(joinTarget);
            }
            catch (InvalidCastException)
            {
            }
        }

        private void TryLinking(Node sourceNode, Node destinationNode)
        {
            LinkSourceTransformation(sourceNode, destinationNode);
            LinkSourceDestination(sourceNode, destinationNode);
            LinkTransformationTransformation(sourceNode, destinationNode);
            LinkTransformationDestination(sourceNode, destinationNode);
        }

        private void LinkTransformationDestination(Node sourceNode, Node destinationNode)
        {
            try
            {
                if (((TransformationNode) sourceNode).DataFlow == null)
                {
                    
                    ((JoinNode) sourceNode).MergeJoin.LinkTo(((DestinationNode) destinationNode).DataFlow);
                    
                }
                else
                {
                    ((TransformationNode) sourceNode).DataFlow.LinkTo(((DestinationNode) destinationNode).DataFlow);
                }
            }
            catch (InvalidCastException)
            {
            }
        }

        private void LinkTransformationTransformation(Node sourceNode, Node destinationNode)
        {
            try
            {
                if (((TransformationNode) sourceNode).DataFlow == null)
                {
                    ((JoinNode) sourceNode).MergeJoin.LinkTo(((TransformationNode) destinationNode).DataFlow);
                }
                else
                {
                    ((TransformationNode) sourceNode).DataFlow.LinkTo(((TransformationNode) destinationNode).DataFlow);
                }
            }
            catch (InvalidCastException)
            {
            }
        }

        private void LinkSourceDestination(Node sourceNode, Node destinationNode)
        {
            try
            {
                ((SourceNode) sourceNode).DataFlow.LinkTo(((DestinationNode) destinationNode).DataFlow);
            }
            catch (InvalidCastException)
            {
            }
        }

        private void LinkSourceTransformation(Node sourceNode, Node destinationNode)
        {
            try
            {
                ((SourceNode) sourceNode).DataFlow.LinkTo(((TransformationNode) destinationNode).DataFlow);
            }
            catch (InvalidCastException)
            {
            }
        }

        public void Run()
        {
            CreateSourceNodes();
            // ReSharper disable once CoVariantArrayConversion
            Network.Execute(_sourceNodes.ToArray());
        }

        public void Cancel()
        {
            // ReSharper disable once CoVariantArrayConversion
            Network.Cancel(_sourceNodes.ToArray());
        }

        private void CreateSourceNodes()
        {
            _sourceNodes = new();
            foreach (var node in _nodes)
            {
                try
                {
                    _sourceNodes.Add(((SourceNode) node).DataFlow);
                }
                catch (InvalidCastException)
                {
                }
            }
        }
    }
}