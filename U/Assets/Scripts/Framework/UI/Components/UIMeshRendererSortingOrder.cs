using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [ExecuteInEditMode]
    public class UIMeshRendererSortingOrder : MonoBehaviour
    {
        enum Type
        {
            Absolute = 0,
            Relative,
        }

        [SerializeField]
        private Type setType = Type.Relative;
        public int sortingOrder = 0;

        void OnEnable()
        {
#if UNITY_EDITOR
            MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < meshRenderer.Length(); i++)
            {
                if (setType == Type.Absolute)
                {
                    meshRenderer[i].sortingOrder = sortingOrder;
                }
                else if (setType == Type.Relative)
                {
                    meshRenderer[i].sortingOrder += sortingOrder;
                }
            }
#endif
        }
    }
}