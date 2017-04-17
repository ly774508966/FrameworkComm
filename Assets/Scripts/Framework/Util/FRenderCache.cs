using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 简单的列表缓存器，管理列表数据重复刷新
    /// 暂时支持GameObject和UIToggle两种泛型
    /// </summary>
    public class FRenderCache<T> where T : class
    {
        private GameObject parentGo;
        private GameObject childGo;
        private List<T> buffer;

        private int reserve = 0;

        public int size
        {
            get
            {
                if (buffer == null)
                    return 0;
                return buffer.Count;
            }
        }

        public FRenderCache(GameObject parent, GameObject prefab)
        {
            parentGo = parent;
            childGo = prefab;
            buffer = new List<T>();
        }

        public FRenderCache(UITable parent, GameObject prefab)
        {
            parentGo = parent.gameObject;
            childGo = prefab;
            buffer = new List<T>();
        }

        public FRenderCache(UIGrid parent, GameObject prefab)
        {
            parentGo = parent.gameObject;
            childGo = prefab;
            buffer = new List<T>();
        }

        /// <summary>
        /// AddChild(parent, prefab)
        /// 如果节点已存在，则复用；否则创建
        /// </summary>
        public T PushRender(int index, GameObject parent = null, GameObject prefab = null)
        {
            if (index < 0)
            {
                FLog.Debug("[ObjectPool]Input index can't be smaller than zero.");
                return null;
            }

            GameObject actualParent;
            if (parent == null) actualParent = parentGo;
            else actualParent = parent;

            GameObject actualChild;
            if (prefab == null) actualChild = childGo;
            else actualChild = prefab;

            if (index < size)
            {
                reserve = index + 1;
                return buffer[index];
            }
            else
            {
                GameObject go = GameObject.Instantiate(actualChild) as GameObject;
#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
                if (go != null && actualParent != null)
                {
                    Transform t = go.transform;
                    t.parent = actualParent.transform;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    go.layer = actualParent.layer;
                    go.name = index.ToString() + "_" + go.name;
                }
                else
                {
                    FLog.Debug("[ObjectPool]Can't find parent or prefab GameObject.");
                    return null;
                }

                Type type = typeof(T);
                if (type == typeof(GameObject))
                {
                    T obj = go as T;
                    buffer.Add(obj);
                    reserve = size;
                    return obj;
                }
                else
                {
                    T component = go.GetComponent<T>();
                    if (component == null)
                    {
                        FLog.Debug("[ObjectPool]Can't find type: " + typeof(T).ToString());
                        UnityEngine.Object.Destroy(go);
                        return null;
                    }
                    else
                    {
                        buffer.Add(component);
                        reserve = size;
                        return component;
                    }
                }
            }
        }

        /// <summary>
        /// 通过索引获取Render引用
        /// </summary>
        public T Render(int index)
        {
            if (buffer == null || index >= buffer.Count)
                return null;
            return buffer[index];
        }

        /// <summary>
        /// 释放无用的资源
        /// </summary>
        public void Release()
        {
            Type type = typeof(T);

            if (reserve == 0)
            {
                int length = buffer.Count;

                for (int i = 0; i < length; i++)
                {
                    if (buffer[i] == null)
                        continue;

                    if (type == typeof(GameObject))
                        UnityEngine.Object.Destroy(buffer[i] as GameObject);
                    else
                        UnityEngine.Object.Destroy((buffer[i] as Component).gameObject);
                }

                buffer.Clear();
            }
            else
            {
                while (buffer.Count > reserve)
                {
                    if (buffer[buffer.Count - 1] != null)
                    {
                        if (type == typeof(GameObject))
                            UnityEngine.Object.Destroy(buffer[buffer.Count - 1] as GameObject);
                        else
                            UnityEngine.Object.Destroy((buffer[buffer.Count - 1] as Component).gameObject);
                    }

                    buffer.RemoveAt(buffer.Count - 1);
                }
            }
        }

        /// <summary>
        /// 清空缓存器
        /// </summary>
        public void Clear()
        {
            reserve = 0;
            Release();
        }
    }
}