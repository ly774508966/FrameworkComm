using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class FlowNode
    {
        public enum State
        {
            Wait = 0,
            Execute,
            Finish,
        }

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

        // Serialized
        public FlowNodeType type;
        public int id;
        public Vector2 position;
        public float[] color;
        public List<int> linkList;
        public string description;

        // Non Serialized
        private GameObject _target = null;
        private State _state = State.Wait;

        #region Create Node
        private static FlowNode CreateOrLoadFromJson(FlowNodeType type, string json = null)
        {
            FlowNode node = null;

            bool fromJson = !string.IsNullOrEmpty(json);

            switch (type)
            {
                case FlowNodeType.Start:
                    {
                        if (fromJson)
                        {
                            node = JsonConvert.DeserializeObject<StartNode>(json) as StartNode;
                        }
                        else
                        {
                            node = new StartNode();
                        }
                    }
                    break;
                case FlowNodeType.Normal:
                    {
                        if (fromJson)
                        {
                            node = JsonConvert.DeserializeObject<NormalNode>(json) as NormalNode;
                        }
                        else
                        {
                            node = new NormalNode();
                        }
                    }
                    break;
                case FlowNodeType.End:
                    {
                        if (fromJson)
                        {
                            node = JsonConvert.DeserializeObject<EndNode>(json) as EndNode;
                        }
                        else
                        {
                            node = new EndNode();
                        }
                    }
                    break;
            }

            if (!fromJson)
            {
                node.type = type;
                node.color = new float[] { 1f, 1f, 1f, 1f };
                node.linkList = new List<int>();
                node.description = "";
            }

            return node;
        }

        private static FlowNode Create(FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = CreateOrLoadFromJson(type);

            node.id = id;
            node.position = position;

            return node;
        }

        public static FlowNode CreateFromJson(string json)
        {
            FlowNode node = JsonConvert.DeserializeObject<FlowNode>(json) as FlowNode;
            return CreateOrLoadFromJson(node.type, json);
        }

        public static FlowNode CreateFromGraph(FlowGraph graph, FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = Create(type, id, position);
            node.SetRectInGraph(graph, node.position);
            graph.AddNode(node);
            return node;
        }
        #endregion

        #region Graph Process
        public virtual IEnumerator Execute()
        {
            _state = State.Execute;
            yield return null;
        }

        public void Finish()
        {
            _state = State.Finish;
        }

        public State GetCurState()
        {
            return _state;
        }

        public virtual bool CheckExecutable()
        {
            return true;
        }
        #endregion

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

        public GameObject GetTargetGameObject()
        {
            return _target;
        }

        public void SetTargetGameObject(GameObject target)
        {
            if (type > FlowNodeType.Start && type < FlowNodeType.End)
            {
                _target = target;
            }
            else
            {
                _target = null;
            }
        }

        public virtual void OnDrawProperty(FlowGraph graph)
        {
            GUILayout.Label(NodeName, EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space();

            if (type > FlowNodeType.Start && type < FlowNodeType.End)
            {
                SetColor(EditorGUILayout.ColorField("Color", GetColor()));
                _target = EditorGUILayout.ObjectField("Target", _target, typeof(GameObject), false) as GameObject;
                description = EditorGUILayout.TextField("Description", description);
                EditorGUILayout.Space();
            }
        }
    }
}