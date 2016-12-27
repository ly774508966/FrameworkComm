using UnityEngine;
using System.Collections;

namespace Framework
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private bool isAppQuit = false;

        void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit()");
            isAppQuit = true;
        }

        public bool IsAppQuit() { return isAppQuit; }
    }
}