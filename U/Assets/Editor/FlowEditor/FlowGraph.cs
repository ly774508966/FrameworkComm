using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class FlowGraph : ScriptableObject
    {
        public List<FlowNode> nodeList { get; set; }
        public Vector2 offset { get; set; }

        public int GetNodeCount()
        {
            return nodeList != null ? nodeList.Count : 0;
        }

        public void AddNode(FlowNode node)
        {
            if (node == null)
            {
                return;
            }

            if (nodeList == null)
            {
                nodeList = new List<FlowNode>();
            }

            nodeList.Add(node);
        }

        public FlowNode GetNode(int id)
        {
            if (nodeList != null)
            {
                return nodeList.Find(node => node.id == id);
            }
            return null;
        }

        public void RemoveNode(FlowNode node)
        {
            if (nodeList != null)
            {
                foreach (FlowNode flowNode in nodeList)
                {
                    flowNode.RemoveLinkNode(node);
                }

                nodeList.Remove(node);
            }
        }
    }
}