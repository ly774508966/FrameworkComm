using System;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class iTweenUtils
    {
        public static void CreateTimeout(
            GameObject go,
            Action callback,
            float time,
            string name = "")
        {
            iTween.ValueTo(go, iTween.Hash(
                "name", "_ITween_Timeout_" + name,
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

        public static void ClearTimeout(
            GameObject timeObject,
            string name = "")
        {
            iTween.StopByName(timeObject, "_ITweenTimeout_" + name);
        }

        public static void TickValue(
            GameObject go,
            float time,
            float from,
            float to,
            Action update,
            Action finish)
        {
            TickValue(go, time, from, to, update, finish);
        }

        public static void TickValue(
            GameObject go,
            float time,
            float from,
            float to,
            Action update,
            Action finish,
            iTween.EaseType easeType = iTween.EaseType.linear,
            string name = "")
        {
            iTween.ValueTo(go, iTween.Hash(
                "name", "_ITween_ValueTo_" + name,
                "time", time,
                "from", from,
                "to", to,
                "easetype", easeType,
                "onupdate", (Action<object>)((x) =>
                {
                    update.Call();
                }),
                "oncomplete", (Action<object>)((x) =>
                {
                    finish.Call();
                })
            ));
        }

        public static void MoveTo(
            this GameObject go,
            Vector3 position,
            float time,
            Action callback)
        {
            MoveTo(go, position, time, callback);
        }

        public static void MoveTo(
            this GameObject go,
            Vector3 position,
            float time,
            Action callback,
            iTween.EaseType easeType,
            string name = "",
            bool local = false)
        {
            go.SetActive(true);
            iTween.MoveTo(go, iTween.Hash(
                "position", position,
                "time", time,
                "islocal", local,
                "name", "_ITween_MoveTo_" + name,
                "easetype", easeType,
                "oncomplete", (Action<object>)((x) =>
                {
                    callback.Call();
                })
             ));
        }
    }
}
