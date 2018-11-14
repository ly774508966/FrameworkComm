using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UICutImage : Image
    {
        private Vector2[] uvs;

        private bool changed;

        private float fWidth;
        private float fHeight;

        private float pivotx;
        private float pivoty;

        protected override void Start()
        {
            base.Start();

            if (uvs != null && uvs.Length > 0)
            {
                SetUV(uvs);
            }
        }

        public void SetUV(Vector2[] uvs)
        {
            this.changed = true;
            this.uvs = uvs;

            SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (changed)
            {
                fWidth = rectTransform.rect.width;
                fHeight = rectTransform.rect.height;
                pivotx = rectTransform.pivot.x;
                pivoty = rectTransform.pivot.y;

                Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
                float uvCenterX = (uv.x + uv.z) * 0.5f;
                float uvCenterY = (uv.y + uv.w) * 0.5f;
                float uvScaleX = (uv.z - uv.x) / fWidth;
                float uvScaleY = (uv.w - uv.y) / fHeight;

                Color32 color32 = color;

                toFill.Clear();

                int length = uvs.Length;

                for (int i = 0; i < length; i++)
                {
                    Vector3 pos = new Vector3(fWidth * uvs[i].x - pivotx * fWidth, fHeight * uvs[i].y - pivoty * fHeight, 0.0f);
                    Vector2 tmpuv = new Vector2((pos.x - (pivotx - 0.5f) * fWidth) * uvScaleX + uvCenterX, (pos.y - (pivoty - 0.5f) * fHeight) * uvScaleY + uvCenterY);
                    toFill.AddVert(pos, color32, tmpuv);
                }

                int start = 1;

                for (int i = 0; i < length - 2; i++)
                {
                    toFill.AddTriangle(0, start, start + 1);
                    start++;
                }

                changed = false;
            }
            else
            {
                base.OnPopulateMesh(toFill);
            }
        }
    }
}