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
        public static void CreateTimeout(GameObject timeObject, Action callback, float time, string name = "")
        {
            iTween.ValueTo(timeObject, iTween.Hash(
                "name", "_ITweenTimeout_" + name,
                "time", time,
                "from", 0,
                "to", 1,
                "oncomplete", (Action<object>)((x) =>
                {
                    callback.Call();
                })
            ));
        }

        public static void ClearTimeout(GameObject timeObject, string name = "")
        {
            iTween.StopByName(timeObject, "_ITweenTimeout_" + name);
        }
    }
}
