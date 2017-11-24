using System;
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

        public FlowNodeType type;
        public int id;
        public float x;
        public float y;
        public float[] color;
        public List<int> linkList;
        public List<int> preList;
        public float delay;
        public bool wait;

        [NonSerialized]
        private GameObject _target = null;
        [NonSerialized]
        private State _state = State.Wait;
        [NonSerialized]
        private int _preFinishCount = 0;

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
                node.preList = new List<int>();
                node.delay = 0f;
                node.wait = true;
            }

            return node;
        }

        private static FlowNode Create(FlowNodeType type, int id, Vector2 position)
        {
            FlowNode node = CreateOrLoadFromJson(type);

            node.id = id;
            node.x = position.x;
            node.y = position.y;

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
            node.SetRectInGraph(graph, node.x, node.y);
            graph.AddNode(node);
            return node;
        }
        #endregion

        #region Graph Process
        public virtual IEnumerator OnExecute()
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            yield break;
        }

        public virtual bool CheckExecutable()
        {
            return true;
        }

        public void NotifyPreFinish()
        {
            _preFinishCount++;
        }

        public State GetCurState()
        {
            return _state;
        }

        public bool StartExecute()
        {
            if (wait && preList != null)
            {
                if (_preFinishCount >= preList.Count)
                {
                    _state = State.Execute;
                }
            }
            else
            {
                _state = State.Execute;
            }

            return _state == State.Execute;
        }

        public void FinishExecute()
        {
            _state = State.Finish;
        }
        #endregion

        #region On Draw
        public virtual void OnDrawProperty()
        {
            GUILayout.Label(NodeName, EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space();

            if (type == FlowNodeType.Start)
            {
                delay = EditorGUILayout.FloatField("Delay", delay);
            }
            else if (type == FlowNodeType.End)
            {
                wait = EditorGUILayout.Toggle("Wait", wait);
            }
            else
            {
                SetColor(EditorGUILayout.ColorField("Color", GetColor()));
                _target = EditorGUILayout.ObjectField("Target", _target, typeof(GameObject), false) as GameObject;
                delay = EditorGUILayout.FloatField("Delay", delay);
                wait = EditorGUILayout.Toggle("Wait", wait);
            }

            EditorGUILayout.Space();
        }

        public virtual void OnDrawNode()
        {

        }
        #endregion

        public Rect GetRectInGraph(FlowGraph graph)
        {
            return new Rect(x + graph.graphOffset.x, y + graph.graphOffset.y, NodeWidth, NodeHeight);
        }

        public void SetRectInGraph(FlowGraph graph, Vector2 position)
        {
            SetRectInGraph(graph, position.x, position.y);
        }

        public void SetRectInGraph(FlowGraph graph, float xPos, float yPos)
        {
            x = xPos - graph.graphOffset.x;
            y = yPos - graph.graphOffset.y;
        }

        public void AddLinkNode(FlowNode linkNode)
        {
            if (linkNode != this && !linkList.Contains(linkNode.id))
            {
                linkList.Add(linkNode.id);
            }
        }

        public void RemoveLinkNode(FlowNode linkNode)
        {
            if (linkList.Contains(linkNode.id))
            {
                linkList.Remove(linkNode.id);
            }
        }

        public void AddPreNode(FlowNode preNode)
        {
            if (preNode != this && !preList.Contains(preNode.id))
            {
                preList.Add(preNode.id);
            }
        }

        public void RemovePreNode(FlowNode preNode)
        {
            if (preList.Contains(preNode.id))
            {
                preList.Remove(preNode.id);
            }
        }

        public Color GetColor()
        {
            return new Color(color[0], color[1], color[2], color[3]);
        }

        private void SetColor(Color color)
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
    }
}