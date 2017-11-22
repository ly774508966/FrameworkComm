using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public class FlowNode
    {
        public virtual string NodeName
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

        public FlowNodeType type = FlowNodeType.None;
        public int id = 0;
        public Vector2 position = Vector2.zero;
        public float[] color = new float[4];
        public List<int> linkList = new List<int>();

        public static FlowNode Create(FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = null;

            switch (type)
            {
                case FlowNodeType.Start:
                    {
                        node = new StartNode();
                    }
                    break;
                case FlowNodeType.Normal:
                    {
                        node = new NormalNode();
                    }
                    break;
                case FlowNodeType.End:
                    {
                        node = new EndNode();
                    }
                    break;
            }

            node.type = type;
            node.id = id;
            node.position = position;
            node.color = new float[] { 1f, 1f, 1f, 1f };

            return node;
        }

        public static FlowNode CreateInGraph(FlowGraph graph, FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = Create(type, id, position);
            node.SetRectInGraph(graph, node.position);
            graph.AddNode(node);
            return node;
        }

        public Rect GetRectInGraph(FlowGraph graph)
        {
            return new Rect(position.x + graph.graphOffset.x, position.y + graph.graphOffset.y, NodeWidth, NodeHeight);
        }

        public void SetRectInGraph(FlowGraph graph, Vector2 position)
        {
            this.position = new Vector2(position.x - graph.graphOffset.x, position.y - graph.graphOffset.y);
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

        public Color GetColor()
        {
            return new Color(color[0], color[1], color[2], color[3]);
        }

        void SetColor(Color color)
        {
            this.color[0] = color.r;
            this.color[1] = color.g;
            this.color[2] = color.b;
            this.color[3] = color.a;
        }

        public virtual void OnDrawProperty()
        {
            GUILayout.Label(NodeName, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            SetColor(EditorGUILayout.ColorField("Node Color", GetColor()));
        }
    }
}