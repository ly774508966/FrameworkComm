using System;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class iTweenUtils
    {
        /// <summary>
        /// 超时回调，生命周期依赖于挂载的GameObject
        /// </summary>
        public static void CreateTimeout(GameObject go, Action callback, float time, string name = "")
        {
            iTween.ValueTo(go, iTween.Hash(
                "name", "_iTweenTimeout_" + name,
                "time", time,
                "from", 0,
                "to", 1,
                "onupdate", (Action<object>)((x) => { }),
                "oncomplete", (Action<object>)((x) =>
                {
                    callback.Call();
                })
            ));
        }

        public static void ClearTimeout(GameObject go, string name = "")
        {
            iTween.StopByName(go, "_iTweenTimeout_" + name);
        }
    }
}
