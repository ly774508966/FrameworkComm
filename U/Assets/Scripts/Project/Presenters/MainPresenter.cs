using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public class MainPresenter : MonoBehaviour
    {
        public FlowGraph graph;

        void Start()
        {
            Log.Debug("MainScene Start");

            FlowGraphExecutor.Execute(graph, () =>
            {
                Log.Debug("FlowGraph execute finish");
            });
        }
    }
}
