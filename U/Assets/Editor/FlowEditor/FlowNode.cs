using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public enum FlowNodeType
    {
        None = 0,
        Start,
        Normal,
        End,
        Count,
    }

    public abstract class FlowNode : ScriptableObject
    {
        [SerializeField]
        protected FlowNodeType type { get; set; }

        public int id { get; set; }
        public Rect position { get; set; }
        public Color color { get; set; }
        public List<int> linkList { get; set; }

        public static FlowNode Create(FlowNodeType type, int id, Rect position)
        {
            FlowNode node = null;

            switch (type)
            {
                case FlowNodeType.Start:
                    {
                        node = CreateInstance<StartNode>();
                    }
                    break;
                case FlowNodeType.Normal:
                    {
                        node = CreateInstance<NormalNode>();
                    }
                    break;
                case FlowNodeType.End:
                    {
                        node = CreateInstance<EndNode>();
                    }
                    break;
            }

            node.type = type;
            node.id = id;
            node.position = position;
            node.color = Color.white;

            return node;
        }

        public static FlowNode CreateInGraph(FlowGraph graph, FlowNodeType type, int id, Rect position)
        {
            FlowNode node = Create(type, id, position);
            node.SetPositionInGraph(graph, position);
            graph.AddNode(node);
            return node;
        }

        public Rect GetPositionInGraph(FlowGraph graph)
        {
            Rect rect = position;
            rect.x += graph.offset.x;
            rect.y += graph.offset.y;
            return rect;
        }

        public void SetPositionInGraph(FlowGraph graph, Rect rect)
        {
            rect.x -= graph.offset.x;
            rect.y -= graph.offset.y;
            position = rect;
        }

        public void AddLinkNode(FlowNode node)
        {
            if (node != this && !linkList.Contains(node.id))
            {
                linkList.Add(node.id);
            }
        }

        public void RemoveLinkNode(FlowNode node)
        {
            if (linkList.Contains(node.id))
            {
                linkList.Remove(node.id);
            }
        }
    }

    public class StartNode : FlowNode
    {
        public string nodeName = "StartNode";
    }

    public class NormalNode : FlowNode
    {
        public string nodeName = "NormalNode";
    }

    public class EndNode : FlowNode
    {
        public string nodeName = "EndNode";
    }
}