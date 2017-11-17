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
        public virtual string Name
        {
            get { return "FlowNode"; }
        }

        public virtual float NodeWidth
        {
            get { return 150f; }
        }

        public virtual float NodeHeight
        {
            get { return 50f; }
        }

        public FlowNodeType type { get; set; }
        public int id { get; set; }
        public Rect rect { get; set; }
        public Color color { get; set; }
        public List<int> linkList { get; set; }

        public static FlowNode Create(FlowNodeType type, int id, Vector2 position)
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
            node.rect = new Rect(position.x, position.y, node.NodeWidth, node.NodeHeight);
            node.color = Color.white;

            return node;
        }

        public static FlowNode CreateInGraph(FlowGraph graph, FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = Create(type, id, position);
            node.SetRectInGraph(graph, node.rect);
            graph.AddNode(node);
            return node;
        }

        public Rect GetRectInGraph(FlowGraph graph)
        {
            Rect rectCopy = rect;
            rectCopy.x += graph.offset.x;
            rectCopy.y += graph.offset.y;
            return rectCopy;
        }

        public void SetRectInGraph(FlowGraph graph, Rect rect)
        {
            rect.x -= graph.offset.x;
            rect.y -= graph.offset.y;
            this.rect = rect;
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
}