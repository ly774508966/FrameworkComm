using UnityEngine;
using System.Collections;

namespace Framework
{
    public class DestroyEventTrigger : MonoBehaviour
    {
        public delegate void DestroyHandler(GameObject target);
        public event DestroyHandler onDestroy;
        public Transform parent;
        public bool canBeBatchRemoved = true;   //能否被批量移除

        void Awake()
        {
            parent = transform.parent;
        }

        void OnDestroy()
        {
            if (!GameManager.instance.IsAppQuit())
            {
                Log.Debug("DestroyEventTrigger -> OnDestroy()");
                if (onDestroy != null)
                    onDestroy(gameObject);
            }

            parent = null;
        }
    }
}

