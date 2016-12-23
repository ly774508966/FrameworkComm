using UnityEngine;
using System.Collections;
using Framework;

namespace TikiAL
{
    public class BaseSceneEaseInOut : BaseScene
    {
        protected override void Start()
        {
            base.Start();
            SceneEaseIn(delegate ()
            {
                OnSceneEaseInFinish();
            });
        }

        protected virtual void SceneEaseIn(Callback.FunVoid fCallback)
        {
            fCallback();
        }

        protected virtual void OnSceneEaseInFinish() { }

        protected virtual void SceneEaseOut(Callback.FunVoid fCallback)
        {
            fCallback();
        }

        protected override void GotoScene(string sceneName)
        {
            SceneEaseOut(delegate ()
            {
                SceneManager.instance.GotoScene(sceneName);
            });
        }

        protected override void BackToLastScene()
        {
            SceneEaseOut(delegate ()
            {
                SceneManager.instance.Back();
            });
        }
    }
}