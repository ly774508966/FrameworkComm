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
        void Start()
        {
            Log.Debug("MainScene Start");

            FlowGraphExecutor.Execute("Assets/Resources/Flow/Flow Graph.asset");
        }
    }
}
