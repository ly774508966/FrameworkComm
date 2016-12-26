using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Framework
{
    public class Util
    {
        public static GameObject GetRoot()
        {
            return GameObject.Find("/UI Root");
        }

        public static UIRoot GetUIRoot()
        {
            GameObject root = GetRoot();
            if (root != null)
                return root.GetComponent<UIRoot>();
            return null;
        }

        public static Vector2 GetCurrentScreenSize()
        {
            Vector2 vec2 = new Vector2(0.0f, 0.0f);

            UIRoot root = GetUIRoot();
            if (root != null)
            {
                float s = (float)root.activeHeight / Screen.height;
                vec2.x = Mathf.CeilToInt(Screen.width * s);
                vec2.y = Mathf.CeilToInt(Screen.height * s);
            }

            return vec2;
        }

        public static void SetSortingOrder(GameObject go, int sortingOrder)
        {
            if (go == null) return;

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sortingOrder = sortingOrder;
        }

        public static bool SaveToXml<T>(string filePath, T source) where T : class
        {
            if (source == null)
            {
                Log.Debug("SaveToXml() failed, source is null.");
                return false;
            }

            if (!File.Exists(filePath))
            {
                Log.Debug("SaveToXml() filePath is not existed, create file: " + filePath.ToString());
                File.Create(filePath);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(writer, source);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveToXml() " + ex.ToString());
                return false;
            }
            return true;
        }

        public static bool LoadFromXml<T>(string filePath, out T result) where T : class
        {
            result = null;

            if (!File.Exists(filePath))
            {
                Log.Debug("LoadFromXml() failed, filePath is not existed.");
                return false;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    result = xmlSerializer.Deserialize(reader) as T;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadFromXml() " + ex.ToString());
                return false;
            }
            return true;
        }

        public static bool LoadListFromFile(List<string> list, string filePath)
        {
            if (!File.Exists(filePath) || list == null)
                return false;

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    Stream stream = reader.BaseStream;
                    long length = stream.Length;
                    while (stream.Position < length)
                    {
                        string msg = reader.ReadString();
                        if (msg != null)
                            list.Add(msg);
                        else break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadListFromFile() " + ex.Message);
                return false;
            }

            return true;
        }

        public static void SaveListToFile(List<string> list, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || list == null)
                return;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    foreach (string key in list)
                    {
                        writer.Write(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveListToFile() " + ex.Message);
            }
        }

        public static bool LoadDictFromFile(Dictionary<string, string> dict, string filePath)
        {
            if (!File.Exists(filePath) || dict == null)
                return false;

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    Stream stream = reader.BaseStream;
                    long length = stream.Length;
                    while (stream.Position < length)
                    {
                        string dictKey = reader.ReadString();
                        string dictValue = reader.ReadString();

                        if (dictKey != null && dictValue != null)
                            dict[dictKey] = dictValue;
                        else break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadDictFromFile() " + ex.Message);
                return false;
            }

            return true;
        }

        public static bool LoadDictFromFile(Dictionary<int, int> dict, string filePath)
        {
            if (!File.Exists(filePath) || dict == null)
                return false;

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    Stream stream = reader.BaseStream;
                    long length = stream.Length;
                    while (stream.Position < length)
                    {
                        int dictKey = reader.ReadInt32();
                        int dictValue = reader.ReadInt32();

                        dict[dictKey] = dictValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadDictFromFile() " + ex.Message);
                return false;
            }

            return true;
        }

        public static bool LoadDictFromFile(Dictionary<string, int> dict, string filePath)
        {
            if (!File.Exists(filePath) || dict == null)
                return false;

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    Stream stream = reader.BaseStream;
                    long length = stream.Length;
                    while (stream.Position < length)
                    {
                        string dictKey = reader.ReadString();
                        int dictValue = reader.ReadInt32();

                        if (dictKey != null)
                            dict[dictKey] = dictValue;
                        else break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("LoadDictFromFile() " + ex.Message);
                return false;
            }

            return true;
        }

        public static void SaveDictToFile(Dictionary<string, string> dict, string filePath)
        {
            if (dict == null)
                return;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    foreach (string key in dict.Keys)
                    {
                        writer.Write(key);
                        writer.Write(dict[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveDictToFile() " + ex.Message);
            }
        }

        public static void SaveDictToFile(Dictionary<int, int> dict, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || dict == null)
                return;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {

                    foreach (int key in dict.Keys)
                    {
                        writer.Write(key);
                        writer.Write(dict[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveDictToFile() " + ex.Message);
            }
        }

        public static void SaveDictToFile(Dictionary<string, int> dict, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || dict == null)
                return;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {

                    foreach (string key in dict.Keys)
                    {
                        writer.Write(key);
                        writer.Write(dict[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveDictToFile() " + ex.Message);
            }
        }

        public static void SaveBytesToFile(byte[] allBytes, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || allBytes == null)
                return;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    writer.Write(allBytes);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveBytesToFile() " + ex.Message);
            }
        }

        public static byte[] LoadBytesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                Log.Debug("LoadBytesFromFile() " + ex.Message);
                return null;
            }
        }

        public static void SaveDictionaryToText(Dictionary<string, int> glDic, string filePath, string title = "")
        {
            if (string.IsNullOrEmpty(filePath) || glDic == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(title + "\n");
            foreach (KeyValuePair<string, int> kvp in glDic)
            {
                sb.AppendLine(kvp.Value.ToString() + ", " + kvp.Key);
            }
            sb.AppendLine("\nLength = " + glDic.Count.ToString());
            sb.AppendLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Create)))
                {
                    writer.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SaveDictionaryToTxt() " + ex.Message);
            }
        }

        /// <summary>
        /// 随机从 0 ~ max-1 中取出不重复的 num 个整数
        /// </summary>
        public static int[] GetRandom(int num, int max)
        {
            if (num < 0 || max < 0 || num > max)
                return null;

            System.Random random = new System.Random((int)(DateTime.Now.Ticks & 0x0000FFFF));

            int[] result = new int[num];
            int[] seed = new int[max];

            for (int i = 0; i < max; i++)
                seed[i] = i;

            for (int r = 0; r < num; r++)
            {
                int index = random.Next(0, num - r);
                result[r] = seed[index];
                seed[index] = seed[num - r - 1];
            }

            return result;
        }
    }
}