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
        // Serialized
        [HideInInspector]
        public List<string> nodeJsonList;
        [HideInInspector]
        public List<GameObject> targetList;
        [HideInInspector]
        public Vector2 graphOffset;

        // Non Serialized
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

            if (graph != null && graph.nodeJsonList != null)
            {
                graph._nodeNextID = 0;
                graph._nodeList.Clear();

                int index = 0;

                foreach (string json in graph.nodeJsonList)
                {
                    FlowNode node = FlowNode.CreateFromJson(json);
                    node.SetTargetGameObject(graph.targetList[index++]);
                    graph._nodeList.Add(node);
                }
            }

            return graph;
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