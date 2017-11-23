using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class FlowGraphExecutor : MonoBehaviour
    {
        private const string FlowGraphRootName = "FlowGraphRoot";
        private const string FlowGraphObjectName = "FlowGraph";

        private static GameObject FlowGraphRoot = null;

        public static FlowGraphExecutor Execute(FlowGraph graph, Action finishCallback = null)
        {
            if (graph == null)
            {
                Log.Error("[FlowGraphExecutor.Execute] flow graph is null");
                return null;
            }

            FlowGraphExecutor executor = GetOrCreateFlowGraphExecutor();
            executor._graph = graph;
            executor._finishDelegate = finishCallback;
            executor.Execute();

            return executor;
        }

        public static FlowGraphExecutor Execute(string flowGraphPath, Action finishCallback = null)
        {
            FlowGraph graph = FlowGraph.Load(flowGraphPath);
            return Execute(graph, finishCallback);
        }

        private static FlowGraphExecutor GetOrCreateFlowGraphExecutor()
        {
            if (FlowGraphRoot == null)
            {
                FlowGraphRoot = new GameObject(FlowGraphRootName);
            }

            GameObject flowGraphObject = new GameObject(FlowGraphObjectName);

            return UI.UIUtils.AddChild(FlowGraphRoot, flowGraphObject).AddComponent<FlowGraphExecutor>();
        }

        private FlowGraph _graph;
        private Action _finishDelegate;

        public void Execute()
        {
            // to do
        }

        private IEnumerator ProcessGraph()
        {
            yield return null;

            // to do
        }
    }
}