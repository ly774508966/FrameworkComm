using UnityEngine;

namespace Framework
{
    public class FBaseScene : FBaseController, FKeyEventInterface
    {
        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnKeyBackRelease();
#if UNITY_EDITOR || UNITY_STANDALONE
            else if (Input.GetKeyDown(KeyCode.Space))
                OnSpaceRelease();
#endif
        }

        protected virtual void GotoScene(string sceneName)
        {
            SceneManager.instance.GotoScene(sceneName);
        }

        protected virtual void BackToLastScene()
        {
            SceneManager.instance.Back();
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        public virtual void OnSpaceRelease()
        {
            FLog.Debug("OnSpaceRelease()");
        }
#endif

        public virtual void OnKeyBackRelease()
        {
            FLog.Debug("OnKeyBackRelease() -> BackToLastScene()");
            BackToLastScene();
        }
    }
}