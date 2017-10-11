using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Framework;

/// <summary>
/// Excel2Json Manager, require E2JTableList & E2JHashHelper
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public class E2JManager : Singleton<E2JManager>
    {
        enum E2JKeyType
        {
            Int = 0,
            String,
        }

        Dictionary<string, Dictionary<string, E2JLoader>> _tableStringDict = new Dictionary<string, Dictionary<string, E2JLoader>>();
        Dictionary<string, Dictionary<int, E2JLoader>> _tableIntDict = new Dictionary<string, Dictionary<int, E2JLoader>>();

        string _jsonPath = "Json/";

        protected override void OnInit()
        {
            Reset();
            ParseJson(E2JTableList.E2JName, E2JKeyType.String);
            LoadFromTableList();
        }

        public void Reload()
        {
#if UNITY_EDITOR
            Reset();
            ParseJson(E2JTableList.E2JName, E2JKeyType.String);
            LoadFromTableList();
#endif
        }

        public E2JLoader GetElementString(string tableKey, string elementKey)
        {
            if (_tableStringDict.ContainsKey(tableKey))
            {
                Dictionary<string, E2JLoader> e2jTable = _tableStringDict[tableKey];
                if (e2jTable.ContainsKey(elementKey))
                {
                    return e2jTable[elementKey];
                }
            }

            return null;
        }

        public E2JLoader GetElementInt(string tableKey, int elementKey)
        {
            if (_tableIntDict.ContainsKey(tableKey))
            {
                Dictionary<int, E2JLoader> e2jTable = _tableIntDict[tableKey];
                if (e2jTable.ContainsKey(elementKey))
                {
                    return e2jTable[elementKey];
                }
            }

            return null;
        }

        public Dictionary<string, E2JLoader> GetTableString(string tableKey)
        {
            if (_tableStringDict.ContainsKey(tableKey))
            {
                return _tableStringDict[tableKey];
            }

            return null;
        }

        public Dictionary<int, E2JLoader> GetTableInt(string tableKey)
        {
            if (_tableIntDict.ContainsKey(tableKey))
            {
                return _tableIntDict[tableKey];
            }

            return null;
        }

        void Reset()
        {
            _tableStringDict.Clear();
            _tableIntDict.Clear();
        }

        uint BKDRHash(string str)
        {
            uint seed = 131;
            uint hash = 0;
            char[] seq = str.ToCharArray();
            for (int i = 0; i < seq.Length(); i++)
            {
                hash = (hash * seed + seq[i]) & 0x7FFFFFFF;
            }
            return hash & 0x7FFFFFFF;
        }

        void LoadFromTableList()
        {
            Dictionary<string, E2JLoader> table = E2JTableList.GetElementTable();
            foreach (KeyValuePair<string, E2JLoader> kvp in table.CheckNull())
            {
                ParseJson(kvp.Value as E2JTableList);
            }
        }

        void ParseJson(E2JTableList tableList)
        {
            ParseJson(tableList.TableName, (E2JKeyType)tableList.KeyType);
        }

        void ParseJson(string tableKey, E2JKeyType keyType)
        {
            uint hash = BKDRHash(tableKey);

            try
            {
                TextAsset jsonText = Resources.Load(_jsonPath + tableKey) as TextAsset;
                Hashtable jsonHt = JsonConvert.DeserializeObject(jsonText.text) as Hashtable;

                switch (keyType)
                {
                    case E2JKeyType.Int:
                        {
                            _tableIntDict.Remove(tableKey);

                            Dictionary<int, E2JLoader> intDict = new Dictionary<int, E2JLoader>();

                            foreach (DictionaryEntry entry in jsonHt)
                            {
                                string key = entry.Key as string;
                                Hashtable value = entry.Value as Hashtable;

                                E2JLoader loader = E2JHashHelper.CreateLoaderByHash(hash);
                                loader.Load(value);

                                intDict.Add(int.Parse(key), loader);
                            }

                            _tableIntDict.Add(tableKey, intDict);
                        }
                        break;
                    case E2JKeyType.String:
                        {
                            _tableStringDict.Remove(tableKey);

                            Dictionary<string, E2JLoader> stringDict = new Dictionary<string, E2JLoader>();

                            foreach (DictionaryEntry entry in jsonHt)
                            {
                                string key = entry.Key as string;
                                Hashtable value = entry.Value as Hashtable;

                                E2JLoader loader = E2JHashHelper.CreateLoaderByHash(hash);
                                loader.Load(value);

                                stringDict.Add(key, loader);
                            }

                            _tableStringDict.Add(tableKey, stringDict);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JManager ParseJson exception: " + ex.ToString());
            }
        }
    }
}
