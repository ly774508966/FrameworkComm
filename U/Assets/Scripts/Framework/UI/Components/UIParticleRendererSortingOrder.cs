using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [ExecuteInEditMode]
    public class UIParticleRendererSortingOrder : MonoBehaviour
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
            ParticleRenderer[] renderer = GetComponentsInChildren<ParticleRenderer>(true);
            for (int i = 0; i < renderer.Length(); i++)
            {
                if (setType == Type.Absolute)
                {
                    renderer[i].sortingOrder = sortingOrder;
                }
                else if (setType == Type.Relative)
                {
                    renderer[i].sortingOrder += sortingOrder;
                }
            }
#endif
        }
    }
}