using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class FlowGraph : ScriptableObject
    {
        public List<string> nodeJson;
        public Vector2 graphOffset;

        private List<FlowNode> _nodeList = new List<FlowNode>();
        private int _nodeNextID = 0;

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

        public void Open()
        {
            Debug.Log("111111111111111");
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
            }

            _nodeList.Remove(node);
        }

        public static FlowGraph LoadFromAsset(Object graphAsset)
        {
            FlowGraph graph = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(graphAsset), typeof(FlowGraph)) as FlowGraph;

            if (graph != null && graph.nodeJson != null)
            {
                graph._nodeNextID = 0;
                graph._nodeList.Clear();

                foreach (string json in graph.nodeJson)
                {
                    FlowNode node = JsonConvert.DeserializeObject<FlowNode>(json) as FlowNode;

                    switch (node.type)
                    {
                        case FlowNodeType.Start:
                            {
                                node = JsonConvert.DeserializeObject<StartNode>(json) as StartNode;
                            }
                            break;
                        case FlowNodeType.Normal:
                            {
                                node = JsonConvert.DeserializeObject<NormalNode>(json) as NormalNode;
                            }
                            break;
                        case FlowNodeType.End:
                            {
                                node = JsonConvert.DeserializeObject<EndNode>(json) as EndNode;
                            }
                            break;
                    }

                    graph._nodeList.Add(node);
                }
            }

            return graph;
        }

        public Object Save(string path, bool create)
        {
            nodeJson = new List<string>();

            foreach (FlowNode node in _nodeList)
            {
                nodeJson.Add(JsonConvert.SerializeObject(node, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
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