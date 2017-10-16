using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UICircleImage : Image
    {
        List<Vector3> innerVertices;
        List<Vector3> outerVertices;

        [Tooltip("Fill percent")]
        [Range(0, 1)]
        [SerializeField]
        float _fillPercent = 1f;

        [Tooltip("Circle or ring")]
        [SerializeField]
        bool _fill = true;

        [Tooltip("Ring thickness")]
        [SerializeField]
        float _thickness = 5;

        [Tooltip("Smooth")]
        [Range(3, 100)]
        [SerializeField]
        int _segements = 20;

        protected override void Awake()
        {
            base.Awake();
            innerVertices = new List<Vector3>();
            outerVertices = new List<Vector3>();
        }

        void Update()
        {
            _thickness = Mathf.Clamp(_thickness, 0, rectTransform.rect.width / 2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            innerVertices.Clear();
            outerVertices.Clear();

            float degreeDelta = 2 * Mathf.PI / _segements;
            int curSegements = (int)(_segements * _fillPercent);

            float tw = rectTransform.rect.width;
            float th = rectTransform.rect.height;
            float outerRadius = rectTransform.pivot.x * tw;
            float innerRadius = rectTransform.pivot.x * tw - _thickness;
            
            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            float uvCenterX = (uv.x + uv.z) * 0.5f;
            float uvCenterY = (uv.y + uv.w) * 0.5f;
            float uvScaleX = (uv.z - uv.x) / tw;
            float uvScaleY = (uv.w - uv.y) / th;

            float curDegree = 0;
            UIVertex uiVertex;
            int verticeCount;
            int triangleCount;
            Vector2 curVertice;

            if (_fill)
            {// Circle
                curVertice = Vector2.zero;
                verticeCount = curSegements + 1;
                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                vh.AddVert(uiVertex);

                for (int i = 1; i < verticeCount; i++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                    curDegree += degreeDelta;

                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);

                    outerVertices.Add(curVertice);
                }

                triangleCount = curSegements * 3;
                for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
                {
                    vh.AddTriangle(vIdx, 0, vIdx + 1);
                }
                if (_fillPercent == 1)
                {// Connect head and tail
                    vh.AddTriangle(verticeCount - 1, 0, 1);
                }
            }
            else
            {// Ring
                verticeCount = curSegements * 2;
                for (int i = 0; i < verticeCount; i += 2)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curDegree += degreeDelta;

                    curVertice = new Vector3(cosA * innerRadius, sinA * innerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);
                    innerVertices.Add(curVertice);

                    curVertice = new Vector3(cosA * outerRadius, sinA * outerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);
                    outerVertices.Add(curVertice);
                }

                triangleCount = curSegements * 3 * 2;
                for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
                {
                    vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                    vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
                }
                if (_fillPercent == 1)
                {// Connect head and tail
                    vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                    vh.AddTriangle(verticeCount - 2, 0, 1);
                }
            }
        }
    }
}
