using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Framework
{
    public class FUtil
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

        public static void BringForwardInParent(GameObject parent, GameObject gameObject)
        {
            UIPanel[] list = parent.GetComponentsInChildren<UIPanel>();
            int size = list.Length;
            int iMaxDepth = int.MinValue;

            if (size > 0 && gameObject != null)
            {
                for (int i = 0; i < size; ++i)
                {
                    UIPanel panel = list[i];
                    //新添加的gameObject，不纳入计算
                    if (panel.depth > iMaxDepth && !NGUITools.IsChild(gameObject.transform, panel.transform))
                        iMaxDepth = panel.depth;
                }

                if (iMaxDepth > int.MinValue)
                {
                    UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>(true);
                    int iCurPanelMinDepth = int.MaxValue;

                    //找出新添加的gameObject下最小的UIPanel.depth
                    for (int i = 0; i < panels.Length; i++)
                    {
                        if (panels[i].depth < iCurPanelMinDepth)
                            iCurPanelMinDepth = panels[i].depth;
                    }

                    //如果新添加的gameObject下最小的UIPanel.depth，比原有弹窗的最大depth还小的时候(被遮挡)，调整depth使其显示在最顶部
                    if (iCurPanelMinDepth <= iMaxDepth)
                    {
                        for (int i = 0; i < panels.Length; ++i)
                        {
                            UIPanel p = panels[i];
                            p.depth += iMaxDepth - iCurPanelMinDepth + 1;
                        }
                    }
                }
            }
        }

        public static void DestroyGameObjects(List<GameObject> listObj, bool bImmediate = false)
        {
            foreach (GameObject obj in listObj)
            {
                obj.transform.parent = null;    // parent = null，对当前帧判断孩子的个数很重要
                if (bImmediate)
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
                else
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }


        public static void RemoveAllChildren(GameObject gameObj)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in gameObj.transform)
            {
                children.Add(child.gameObject);
            }
            DestroyGameObjects(children);
        }

        public static void RemoveAllChildren(GameObject gameObj, bool bDestoryImmediate, int fromIndex, int toIndex = -1)
        {
            List<GameObject> children = new List<GameObject>();

            if (toIndex == -1)
            {
                toIndex = gameObj.transform.childCount;
            }

            for (int i = fromIndex; i <= toIndex && i < gameObj.transform.childCount; i++)
            {
                Transform child = gameObj.transform.GetChild(i);
                children.Add(child.gameObject);
            }

            DestroyGameObjects(children, bDestoryImmediate);
        }

        public static void SaveToJpg(Texture2D texture, string filePath, bool destroyTexture = false)
        {
            if (texture == null)
            {
                FLog.Debug("SaveToJpg() failed, texture is null.");
                return;
            }

            byte[] png = texture.EncodeToJPG();
            SaveBytesToFile(png, filePath);
            if (destroyTexture)
                Texture2D.DestroyImmediate(texture);
            png = null;

            FLog.Debug("SaveToJpg() succeed.");
        }

        public static bool SaveToXml<T>(string filePath, T source) where T : class
        {
            if (source == null)
            {
                FLog.Debug("SaveToXml() failed, source is null.");
                return false;
            }

            if (!File.Exists(filePath))
            {
                FLog.Debug("SaveToXml() filePath is not existed, create file: " + filePath.ToString());
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
                FLog.Debug("SaveToXml() " + ex.ToString());
                return false;
            }
            return true;
        }

        public static bool LoadFromXml<T>(string filePath, out T result) where T : class
        {
            result = null;

            if (!File.Exists(filePath))
            {
                FLog.Debug("LoadFromXml() failed, filePath is not existed.");
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
                FLog.Debug("LoadFromXml() " + ex.ToString());
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
                FLog.Debug("LoadListFromFile() " + ex.Message);
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
                FLog.Debug("SaveListToFile() " + ex.Message);
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
                FLog.Debug("LoadDictFromFile() " + ex.Message);
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
                FLog.Debug("LoadDictFromFile() " + ex.Message);
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
                FLog.Debug("LoadDictFromFile() " + ex.Message);
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
                FLog.Debug("SaveDictToFile() " + ex.Message);
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
                FLog.Debug("SaveDictToFile() " + ex.Message);
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
                FLog.Debug("SaveDictToFile() " + ex.Message);
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
                FLog.Debug("SaveBytesToFile() " + ex.Message);
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
                FLog.Debug("LoadBytesFromFile() " + ex.Message);
                return null;
            }
        }

        public static void SaveDictToTxt(Dictionary<string, int> dic, string filePath, string title = "")
        {
            if (string.IsNullOrEmpty(filePath) || dic == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(title + "\n");
            foreach (KeyValuePair<string, int> kvp in dic)
            {
                sb.AppendLine(kvp.Value.ToString() + ", " + kvp.Key.Substring(0, kvp.Key.LastIndexOf('.')));
            }
            sb.AppendLine("\nLength = " + dic.Count.ToString());
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
                FLog.Debug("SaveDictionaryToTxt() " + ex.Message);
            }
        }

        public static void SetTimeout(GameObject timeObject, FCallback.FunVoid fCallback, float time, string name = "")
        {
            iTween.ValueTo(timeObject, iTween.Hash(
                "name", "SetTimeout_" + name,
                "time", time, "from", 0, "to", 1,
                "onupdate", FCallback.CreateAction(delegate () { }),
                "oncomplete", FCallback.CreateAction(delegate ()
                {
                    if (fCallback != null) fCallback();
                })
            ));
        }

        public static void ClearTimeout(GameObject timeObject, string name = "")
        {
            iTween.StopByName(timeObject, "SetTimeout_" + name);
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
                int index = random.Next(0, max - r);
                result[r] = seed[index];
                seed[index] = seed[num - r - 1];
            }

            return result;
        }
    }
}