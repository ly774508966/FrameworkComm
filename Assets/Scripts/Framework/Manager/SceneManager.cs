using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class SceneManager : FMonoSingleton<SceneManager>
    {
        private List<string> sceneList = new List<string>();
        private int MAX_SCENECOUNT = 3;

        public void GotoScene(string value, FCallback.FunVoid fCallback = null)
        {
            StartCoroutine(GotoSceneAsync(value, fCallback));
        }

        public void Back()
        {
            if (sceneList.Count > 0)
            {
                string scene = sceneList[sceneList.Count - 1];
                sceneList.RemoveAt(sceneList.Count - 1);
                if (scene == SceneName.LoadScene)
                {
                    Quit();
                }
                else
                {
                    FLog.Debug("BacktoScene() -> " + scene);
                    Application.LoadLevel(scene);
                }
            }
            else
            {
                Quit();
            }
        }

        private IEnumerator GotoSceneAsync(string value, FCallback.FunVoid fCallback)
        {
            yield return null;

            if (value != Application.loadedLevelName)
            {
                sceneList.Add(Application.loadedLevelName);
                if (sceneList.Count > MAX_SCENECOUNT)
                    sceneList.RemoveAt(0);

                FLog.Debug("GotoSceneAsync() " + Application.loadedLevelName + " -> " + value);

                AsyncOperation asyncO = Application.LoadLevelAsync(value);
                yield return asyncO;

                if (fCallback != null) { fCallback(); }
            }
        }

        private void Quit()
        {
            FLog.Debug("Application.Quit()");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }
    }
}