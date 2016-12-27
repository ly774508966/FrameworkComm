using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        private List<string> sceneList = new List<string>();
        private int MAX_SCENECOUNT = 3;

        public void GotoScene(string value, Callback.FunVoid fCallback = null)
        {
            StartCoroutine(GotoSceneAsync(value, fCallback));
        }

        public void Back()
        {
            if (sceneList.Count > 0)
            {
                string scene = sceneList[sceneList.Count - 1];
                sceneList.RemoveAt(sceneList.Count - 1);
                if (scene == SceneName.LogoScene)
                {
                    Log.Debug("Application.Quit()");
                    Application.Quit();
                }
                else
                {
                    Log.Debug("BacktoScene() -> " + scene);
                    Application.LoadLevel(scene);
                }
            }
            else
            {
                Log.Debug("Application.Quit()");
                Application.Quit();
            }
        }

        private IEnumerator GotoSceneAsync(string value, Callback.FunVoid fCallback)
        {
            yield return null;

            if (value != Application.loadedLevelName)
            {
                sceneList.Add(Application.loadedLevelName);
                if (sceneList.Count > MAX_SCENECOUNT)
                    sceneList.RemoveAt(0);

                Log.Debug("GotoSceneAsync() " + Application.loadedLevelName + " -> " + value);

                AsyncOperation asyncO = Application.LoadLevelAsync(value);
                yield return asyncO;

                if (fCallback != null) { fCallback(); }
            }
        }
    }
}