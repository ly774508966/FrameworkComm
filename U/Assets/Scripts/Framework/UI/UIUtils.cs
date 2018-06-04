﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI工具类
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public static class UIUtils
    {
        public static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static GameObject AddChild(GameObject parent, GameObject prefab = null, bool resetTransform = true)
        {
            GameObject go = null;

            if (prefab != null)
            {
                go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            }
            else
            {
                go = new GameObject();
            }

            if (go != null && parent != null)
            {
                SetParent(parent, go);
            }

            return go;
        }

        public static void LerpPosition(GameObject target, float x, float y, float z, float delta)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, new Vector3(x, y, z), delta);
        }

        public static void SetOffsetPosition(GameObject target, float x, float y, float z)
        {
            target.transform.position += new Vector3(x, y, z);
        }

        public static void SetPosition(GameObject target, float x, float y, float z)
        {
            target.transform.position = new Vector3(x, y, z);
        }

        public static void SetRotation(GameObject target, float x, float y, float z)
        {
            target.transform.rotation = Quaternion.Euler(x, y, z);
        }

        public static void SetLocalPosition(GameObject target, float x, float y, float z)
        {
            target.transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetLocalScale(GameObject target, float x, float y, float z)
        {
            target.transform.localScale = new Vector3(x, y, z);
        }

        public static void SetLocalRotation(GameObject target, float x, float y, float z)
        {
            target.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public static void SetRectTransformSizeDelta(RectTransform target, float width, float height)
        {
            target.sizeDelta = new Vector2(width, height);
        }

        public static void SetRectTransformWidthDelta(RectTransform target, float width)
        {
            target.sizeDelta = new Vector2(width, target.sizeDelta.y);
        }

        public static void SetRectTransformHeightDelta(RectTransform target, float height)
        {
            target.sizeDelta = new Vector2(target.sizeDelta.x, height);
        }

        public static void SetParent(GameObject parent, GameObject go, bool resetTransform = true)
        {
            if (go != null && parent != null)
            {
                Transform transform = go.transform;
                transform.SetParent(parent.transform, false);
                if (resetTransform) ResetTransform(transform);
                SetLayer(go, parent.layer);
            }
        }

        public static void SetChild(GameObject parent, GameObject child)
        {
            if (child != null && parent != null)
            {
                child.transform.SetParent(parent.transform, true);
                ResetTransform(child.transform);
            }
        }

        public static GameObject GetLastChild(GameObject go)
        {
            if (go != null && go.transform.childCount > 0)
            {
                return go.transform.GetChild(go.transform.childCount - 1).gameObject;
            }
            return null;
        }

        public static void SetLayer(GameObject go, int layer)
        {
            if (go == null || layer < 0)
            {
                return;
            }

            if (!go.layer.Equals(layer))
            {
                go.layer = layer;
            }

            Transform transform = go.transform;

            for (int i = 0, max = transform.childCount; i < max; i++)
            {
                Transform child = transform.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        public static void DestroyGameObject(GameObject go, bool immediate = false)
        {
            if (go != null)
            {
                Transform t = go.transform;
                t.SetParent(null);
                if (immediate)
                {
                    Object.DestroyImmediate(go);
                }
                else
                {
                    Object.Destroy(go);
                }
            }
        }

        public static void DestroyGameObjects(List<GameObject> goList, bool immediate = false)
        {
            foreach (GameObject go in goList)
            {
                go.transform.SetParent(null);
                if (immediate)
                {
                    Object.DestroyImmediate(go);
                }
                else
                {
                    Object.Destroy(go);
                }
            }
        }

        public static void RemoveAllChildren(GameObject go)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                children.Add(child.gameObject);
            }
            DestroyGameObjects(children);
        }

        public static void RemoveAllChildrenFrom(GameObject go, bool immediate, int from, int to = -1)
        {
            List<GameObject> children = new List<GameObject>();

            if (to == -1)
            {
                to = go.transform.childCount;
            }

            for (int i = from; i <= to && i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                children.Add(child.gameObject);
            }

            DestroyGameObjects(children, immediate);
        }

        public static void RemoveAllComponents<T>(GameObject go)
        {
            Component[] components = go.GetComponents(typeof(T));
            foreach (Component component in components.CheckNull())
            {
                Object.Destroy(component);
            }
        }

        public static List<Transform> GetAllChildrenTransforms(GameObject go)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in go.transform)
            {
                children.Add(child);
            }
            return children;
        }

        public static void CopyLocalTransform(Transform src, Transform dst)
        {
            dst.localPosition = src.localPosition;
            dst.localEulerAngles = src.localEulerAngles;
            dst.localScale = src.localScale;
        }

        public static void SetScaleToZero(GameObject go, bool visible)
        {
            if (visible)
            {
                go.transform.localScale = Vector3.one;
            }
            else
            {
                go.transform.localScale = Vector3.zero;
            }
        }

        public static T FindInParent<T>(GameObject go) where T : Component
        {
            if (go == null)
                return null;

            T component = go.GetComponent<T>();
            if (component == null)
            {
                Transform transform = go.transform.parent;

                while (transform != null && component == null)
                {
                    component = transform.gameObject.GetComponent<T>();
                    transform = transform.parent;
                }
            }

            return component;
        }

        public static List<GameObject> FindGameObjectsInChildren(GameObject root, string name)
        {
            List<GameObject> childList = new List<GameObject>();

            foreach (Transform child in root.transform)
            {
                if (child.gameObject.name.StartsWith(name))
                {
                    childList.Add(child.gameObject);
                }
            }

            return childList;
        }
    }
}
