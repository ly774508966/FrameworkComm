using System;

namespace Framework
{
    public class GameManager : FMonoSingleton<GameManager>
    {
        private bool isAppQuit = false;

        void OnApplicationQuit()
        {
            FLog.Debug("OnApplicationQuit()");
            isAppQuit = true;
        }

        public bool IsAppQuit() { return isAppQuit; }
    }
}