using UnityEngine;
using System.Collections;
using Framework;

namespace TikiAL
{
    public class BaseScene : BaseController, KeyEventInterface
    {
        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnKeyBackRelease();
        }

        protected virtual void GotoScene(string sceneName)
        {
            SceneManager.instance.GotoScene(sceneName);
        }

        protected virtual void BackToLastScene()
        {
            SceneManager.instance.Back();
        }

        public virtual void OnKeyBackRelease()
        {
            Log.Debug("OnKeyBackRelease() back to last scene.");
            BackToLastScene();
        }
    }
}