using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class FlowGraph : ScriptableObject
    {
        [HideInInspector]
        public List<string> nodeJsonList;
        [HideInInspector]
        public List<GameObject> targetList;
        [HideInInspector]
        public Vector2 graphOffset;

        [NonSerialized]
        private List<FlowNode> _nodeList = new List<FlowNode>();
        [NonSerialized]
        private int _nodeNextID = 0;
        [NonSerialized]
        private bool _valid = false;

        public List<FlowNode> NodeList
        {
            get { return _nodeList; }
        }

        public int NodeCount
        {
            get { return _nodeList.Count; }
        }

        public int NodeNextID
        {
            get
            {
                if (_nodeNextID == 0)
                {
                    _nodeNextID = NodeCount + 1;
                }

                return _nodeNextID++;
            }
        }

        public bool Valid
        {
            get { return _valid; }
        }

        public void AddNode(FlowNode node)
        {
            if (node == null)
            {
                return;
            }

            _nodeList.Add(node);
        }

        public FlowNode GetNode(int nodeID)
        {
            return _nodeList.Find(node => node.id == nodeID);
        }

        public void RemoveNode(FlowNode node)
        {
            foreach (FlowNode flowNode in _nodeList)
            {
                flowNode.RemoveLinkNode(node);
                node.RemovePreNode(flowNode);
            }

            _nodeList.Remove(node);
        }

        public HashSet<FlowNode> GetStartNodes()
        {
            HashSet<FlowNode> startNodeHs = null;

            foreach (FlowNode node in _nodeList)
            {
                if (node.type == FlowNodeType.Start)
                {
                    if (startNodeHs == null)
                    {
                        startNodeHs = new HashSet<FlowNode>();
                    }

                    startNodeHs.Add(node);
                }
            }

            return startNodeHs;
        }

        public bool Initialize()
        {
            if (!_valid && nodeJsonList != null && targetList != null)
            {
                _nodeNextID = 0;
                _nodeList.Clear();

                int index = 0;

                foreach (string json in nodeJsonList)
                {
                    FlowNode node = FlowNode.CreateFromJson(json);
                    node.SetTargetGameObject(targetList[index++]);
                    _nodeList.Add(node);
                }

                return _valid = true;
            }

            return false;
        }

        public static FlowGraph Load(string path)
        {
            FlowGraph graph = AssetDatabase.LoadAssetAtPath<FlowGraph>(path);

            if (graph != null)
            {
                graph.Initialize();
            }

            return graph;
        }

        public static FlowGraph LoadFromAsset(Object graphAsset)
        {
            return Load(AssetDatabase.GetAssetPath(graphAsset));
        }

        public Object Save(string path, bool create)
        {
            nodeJsonList = new List<string>();
            targetList = new List<GameObject>();

            foreach (FlowNode node in _nodeList)
            {
                nodeJsonList.Add(JsonConvert.SerializeObject(node, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                targetList.Add(node.GetTargetGameObject());
            }

            if (create)
            {
                AssetDatabase.CreateAsset(this, path);
            }
            else
            {
                EditorUtility.SetDirty(this);
            }

            return this;
        }
    }
}