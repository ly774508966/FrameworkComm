using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Framework;

namespace TikiAL
{
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        private Dictionary<string, Sprite> resourceDic;

        public void LoadResourcesAsyn(string folder, Callback.FunVoid fCallback = null)
        {
            StartCoroutine(LoadResources(folder, fCallback));
        }

        private IEnumerator LoadResources(string folder, Callback.FunVoid fCallback)
        {
            yield return null;

            DirectoryInfo dir = new DirectoryInfo(folder);
            if (dir.Exists)
            {
                Log.Debug("LoadResources() start from folder: " + folder);

                if (resourceDic == null)
                    resourceDic = new Dictionary<string, Sprite>();

                int length = 0;

                FileInfo[] fInfo = dir.GetFiles();
                if (fInfo != null)
                {
                    length = fInfo.Length;
                    for (int i = 0; i < length; i++)
                    {
                        string name = fInfo[i].Name.Trim(fInfo[i].Extension.ToCharArray());
                        string fullname = fInfo[i].FullName;

                        WWW www = new WWW("file://" + fullname);
                        yield return www;

                        Texture2D texture = www.texture;
                        if (texture != null)
                        {
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            if (sprite != null)
                            {
                                if (!resourceDic.ContainsKey(name))
                                    resourceDic.Add(name, sprite);

                                Log.Debug("Load file: " + name + ", succeed.");
                            }
                            else
                            {
                                Log.Debug("Load file: " + name + ", sprite create failed.");
                            }
                        }
                        else
                        {
                            Log.Debug("Load file: " + name + ", www.texture is null.");
                        }
                    }
                }

                Log.Debug("LoadResources() end, length = " + length.ToString());
            }
            else
            {
                Log.Debug("LoadResources() error, directory is not exist.");
            }

            if (fCallback != null)
            {
                fCallback();
            }
        }

        public Sprite GetSpriteByName(string name)
        {
            if (resourceDic != null && resourceDic.ContainsKey(name))
                return resourceDic[name];

            return null;
        }

        protected override void OnDestroy()
        {
            if (resourceDic != null)
            {
                resourceDic.Clear();
                resourceDic = null;
            }

            StopAllCoroutines();

            base.OnDestroy();
        }
    }
}