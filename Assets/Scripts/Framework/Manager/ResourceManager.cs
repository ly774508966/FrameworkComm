using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class ResourceManager : FMonoSingleton<ResourceManager>
    {
        private Dictionary<string, Sprite> resourceDic;

        public void LoadResourcesAsyn(string folder, FCallback.FunVoid fCallback = null)
        {
            StartCoroutine(LoadResources(folder, fCallback));
        }

        private IEnumerator LoadResources(string folder, FCallback.FunVoid fCallback)
        {
            yield return null;

            DirectoryInfo dir = new DirectoryInfo(folder);
            if (dir.Exists)
            {
                FLog.Debug("LoadResources() start from folder: " + folder);

                if (resourceDic == null)
                    resourceDic = new Dictionary<string, Sprite>();

                int length = 0;

                FileInfo[] fInfo = dir.GetFiles();
                if (fInfo != null)
                {
                    length = fInfo.Length;
                    for (int i = 0; i < length; i++)
                    {
                        string name = fInfo[i].Name;
                        string fullname = fInfo[i].FullName;

                        WWW www = new WWW("file://" + fullname);
                        yield return www;

                        Texture2D texture = www.texture;
                        //texture = FTextureScaler.scaled(texture, 220, 220, FilterMode.Bilinear);
                        //FUtil.SaveToJpg(texture, "./User/" + name);

                        if (texture != null)
                        {
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            if (sprite != null)
                            {
                                if (!resourceDic.ContainsKey(name))
                                    resourceDic.Add(name, sprite);

                                FLog.Debug("Load file: " + name + ", succeed.");
                            }
                            else
                            {
                                FLog.Debug("Load file: " + name + ", sprite create failed.");
                            }
                        }
                        else
                        {
                            FLog.Debug("Load file: " + name + ", www.texture is null.");
                        }
                    }
                }

                FLog.Debug("LoadResources() end, length = " + length.ToString());
            }
            else
            {
                FLog.Debug("LoadResources() error, directory is not exist.");
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