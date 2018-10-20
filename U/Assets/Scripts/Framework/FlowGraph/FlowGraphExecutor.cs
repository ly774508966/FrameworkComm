using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class FlowGraphExecutor : MonoBehaviour
    {
        private const string FlowRootName = "FlowRoot";
        private const string FlowGraphName = "FlowGraph";

        private static GameObject FlowRoot = null;

        public static FlowGraphExecutor Execute(Object flowGraphAsset, Action finishCallback = null)
        {
            return Execute(FlowGraph.LoadFromAsset(flowGraphAsset), finishCallback);
        }

        public static FlowGraphExecutor Execute(string flowGraphPath, Action finishCallback = null)
        {
            return Execute(FlowGraph.Load(flowGraphPath), finishCallback);
        }

        public static FlowGraphExecutor Execute(FlowGraph flowGraph, Action finishCallback = null)
        {
            if (flowGraph == null || !flowGraph.Initialize())
            {
                Log.Error("[FlowGraphExecutor.Execute] flow graph is null or invalid");
                return null;
            }

            FlowGraphExecutor executor = GetOrCreateFlowGraphExecutor();
            executor._flowGraph = flowGraph;
            executor._finishDelegate = finishCallback;

            return executor.Execute();
        }

        private static FlowGraphExecutor GetOrCreateFlowGraphExecutor()
        {
            if (FlowRoot == null)
            {
                FlowRoot = new GameObject(FlowRootName);
            }

            GameObject flowGraphObject = new GameObject(FlowGraphName);
            UnityUtils.SetChild(FlowRoot, flowGraphObject);

            return flowGraphObject.AddComponent<FlowGraphExecutor>();
        }

        private FlowGraph _flowGraph;
        private Action _finishDelegate;

        private HashSet<FlowNode> _curNodeHs;
        private HashSet<FlowNode> _nextNodeHs;
        private HashSet<FlowNode> _finishedNodeHs;

        public FlowGraphExecutor Execute()
        {
            _curNodeHs = _flowGraph.GetStartNodes();

            if (_curNodeHs != null)
            {
                StartCoroutine(ProcessGraph());
            }

            return this;
        }

        private IEnumerator ProcessGraph()
        {
            _nextNodeHs = new HashSet<FlowNode>();
            _finishedNodeHs = new HashSet<FlowNode>();

            bool finished = false;

            while (_curNodeHs.Count > 0)
            {
                _nextNodeHs.Clear();
                _finishedNodeHs.Clear();

                foreach (FlowNode curNode in _curNodeHs)
                {
                    FlowNode.State curState = curNode.GetCurState();

                    if (curState == FlowNode.State.Wait)
                    {
                        if (curNode.CheckExecutable() && curNode.StartExecute())
                        {
                            StartCoroutine(curNode.OnExecute());
                        }
                    }
                    else if (curState == FlowNode.State.Finish)
                    {
                        _finishedNodeHs.Add(curNode);

                        if (curNode.type == FlowNodeType.End)
                        {
                            finished = true;
                        }
                        else
                        {
                            int linkCount = curNode.linkList != null ? curNode.linkList.Count : 0;
                            for (int i = 0; i < linkCount; i++)
                            {
                                FlowNode linkNode = _flowGraph.GetNode(curNode.linkList[i]);
                                if (linkNode.GetCurState() == FlowNode.State.Wait)
                                {
                                    linkNode.NotifyPreFinish();
                                    _nextNodeHs.Add(linkNode);
                                }
                            }
                        }
                    }
                }

                if (finished)
                {
                    break;
                }

                foreach (FlowNode finishedNode in _finishedNodeHs)
                {
                    _curNodeHs.Remove(finishedNode);
                }

                foreach (FlowNode nextNode in _nextNodeHs)
                {
                    _curNodeHs.Add(nextNode);
                }

                yield return null;
            }

            _finishDelegate.Call();

            Destroy(gameObject);
        }
    }
}