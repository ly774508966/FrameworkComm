using UnityEngine;
using Framework;

namespace Project
{
    public class BaseScene : FBaseController, FKeyEventInterface
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
            FLog.Debug("OnKeyBackRelease() -> BackToLastScene()");
            BackToLastScene();
        }
    }
}