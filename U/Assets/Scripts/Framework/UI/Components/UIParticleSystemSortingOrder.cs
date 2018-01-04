using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [ExecuteInEditMode]
    public class UIParticleSystemSortingOrder : MonoBehaviour
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
            ParticleSystem[] particleSystem = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < particleSystem.Length(); i++)
            {
                if (setType == Type.Absolute)
                {
                    particleSystem[i].GetComponent<Renderer>().sortingOrder = sortingOrder;
                }
                else if (setType == Type.Relative)
                {
                    particleSystem[i].GetComponent<Renderer>().sortingOrder += sortingOrder;
                }
            }
#endif
        }
    }
}