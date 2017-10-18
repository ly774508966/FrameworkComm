using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class SerializeUtils
    {
        #region private
        private static bool CreateDirectoryIfNotExist(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetPath(string name)
        {
            string path = GetRoot() + "/ServerName/Uin/";
            CreateDirectoryIfNotExist(path);
            return path + name + ".lf";
        }

        private static string GetRoot()
        {
            string rootPath = "";
#if UNITY_EDITOR
            rootPath = Application.dataPath + "/../LocalFiles/";
#elif UNITY_ANDROID
            if (string.IsNullOrEmpty(Application.persistentDataPath))
                rootPath = "/sdcard/Android/data/com.zhenhaiwang.framework/files/";
            else
                rootPath = Application.persistentDataPath + "/";
#elif UNITY_IPHONE
            if (bReleaseBuild)
                rootPath = Application.temporaryCachePath + "/";
            else
                rootPath = Application.persistentDataPath + "/";
#endif
            CreateDirectoryIfNotExist(rootPath);
            return rootPath;
        }
        #endregion

        public static bool Serialize(object data, string key)
        {
            try
            {
                if (data != null && !string.IsNullOrEmpty(key))
                {
                    File.WriteAllText(GetPath(key), JsonConvert.SerializeObject(data));
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Serialize(int data, string key)
        {
            return Serialize(data.ToString(), key);
        }

        public static bool Serialize(byte[] data, string key)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(GetPath(key), FileMode.Create)))
                {
                    writer.Write(data);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T Deserialize<T>(string key) where T : class
        {
            try
            {
                string path = GetPath(key);
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(json))
                    {
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool Deserialize(string key, out int result)
        {
            return int.TryParse(Deserialize<string>(key), out result);
        }

        public static byte[] Deserialize(string key)
        {
            try
            {
                string path = GetPath(key);
                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public abstract class Serializable<T> where T : Serializable<T>, new()
    {
        public string sKey { get; set; }

        public static T Load(string key)
        {
            T local = SerializeUtils.Deserialize<T>(key);

            if (local == null)
            {
                local = new T();
            }

            local.sKey = key;

            return local;
        }

        public virtual bool Save(string key = null)
        {
            return string.IsNullOrEmpty(key) ?
                SerializeUtils.Serialize(this, sKey) :
                SerializeUtils.Serialize(this, key);
        }
    }
}