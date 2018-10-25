#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public sealed class RaycastTargetDrawer : MonoBehaviour
    {
        public Color lineColor = Color.red;

        private static Vector3[] fourCorners = new Vector3[4];

        private void OnDrawGizmos()
        {
            foreach (MaskableGraphic graphic in FindObjectsOfType<MaskableGraphic>())
            {
                if (graphic.raycastTarget)
                {
                    RectTransform rectTransform = graphic.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);

                    Gizmos.color = lineColor;

                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                    }
                }
            }
        }
    }
}
#endif