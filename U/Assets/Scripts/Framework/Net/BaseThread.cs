using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class BaseThread
    {
        private Thread _thread;
        private bool _terminateFlag;
        private object _terminateFlagMutex;

        public void Run()
        {
            _thread.Start(this);
        }

        protected static void ThreadProcess(object obj)
        {
            BaseThread me = (BaseThread)obj;
            me.Main();
        }

        protected virtual void Main()
        {

        }

        public void WaitTermination()
        {
            _thread.Join();
        }

        public void SetTerminated()
        {
            lock (_terminateFlagMutex)
            {
                _terminateFlag = true;
            }
        }

        public bool CheckTerminated()
        {
            lock (_terminateFlagMutex)
            {
                return _terminateFlag;
            }
        }

        public void Interrupt()
        {
            _thread.Interrupt();
        }

        public BaseThread()
        {
            _thread = new Thread(ThreadProcess);
            _terminateFlag = false;
            _terminateFlagMutex = new object();
        }
    }
}
