using UnityEngine;

/// <summary>
/// 全局Manager管理器，游戏入口
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    class GameSystem : MonoBehaviour
    {
        public static GameSystem instance = null;

        private static bool isQuit = true;

        public static bool isApplicationQuit() { return isQuit; }

        void Awake()
        {
            instance = this;
            isQuit = false;

            Log.logFileName = "Framework.log";

            DontDestroyOnLoad(gameObject);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            Log.Debug("Init GameSystem ok, os: " + Application.platform.ToString());
        }

        public void OnApplicationQuit()
        {
            isQuit = true;
            Log.Debug("GameSystem OnApplicationQuit");
        }

        void OnApplicationFocus(bool isFocus)
        {
            Log.Debug("GameSystem OnApplicationFocus isFocus = " + isFocus);
        }

        void OnDestroy()
        {
            instance = null;
            isQuit = true;
        }

        public T GetManager<T>() where T : class
        {
            string managerName = typeof(T).Name;
            Transform t = transform.Find(managerName);
            if (t == null)
            {
                t = new GameObject(managerName).transform;
                t.parent = transform;
            }

            Component c = t.GetComponent(typeof(T));
            if (c == null)
            {
                return t.gameObject.AddComponent(typeof(T)) as T;
            }
            return c as T;
        }

        public T GetCallbackBridge<T>() where T : class
        {
            string managerName = typeof(T).Name;
            GameObject go = GameObject.Find("/" + managerName);
            if (go == null)
            {
                go = new GameObject(managerName);
                DontDestroyOnLoad(go);
            }

            Component c = go.GetComponent(typeof(T));
            if (c == null)
            {
                return go.AddComponent(typeof(T)) as T;
            }
            return c as T;
        }
    }
}
