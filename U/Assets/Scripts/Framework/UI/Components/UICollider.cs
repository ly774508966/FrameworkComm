﻿using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class UICollider : Image
    {
        public Collider2D targetCollider;

        private UICollider()
        {
            useLegacyMeshGeneration = true;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return targetCollider.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UICollider))]
    public class CustomRaycastFilterInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            UICollider uiCollider = target as UICollider;
            uiCollider.targetCollider = EditorGUILayout.ObjectField("Collider 2D", uiCollider.targetCollider, typeof(Collider2D), true) as Collider2D;
        }
    }
#endif
}