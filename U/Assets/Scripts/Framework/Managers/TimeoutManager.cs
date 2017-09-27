using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 倒计时管理器(全局)
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class TimeoutManager : MonoSingleton<TimeoutManager>
    {
        Dictionary<long, TimeoutObject> _timeoutDic = new Dictionary<long, TimeoutObject>();
        HashSet<TimeoutObject> _tempHs = new HashSet<TimeoutObject>();

        bool _bWaitCheck = false;

        public long CreateTimeout(Action<long, object> callback, float time, object parameter = null)
        {
            TimeoutObject timeout = new TimeoutObject(time, callback, parameter);
            _timeoutDic[timeout.ID] = timeout;
            _bWaitCheck = true;
            return timeout.ID;
        }

        public void ClearTimeout(long id, bool doCallback = false)
        {
            if (_timeoutDic.ContainsKey(id))
            {
                if (doCallback)
                {
                    _timeoutDic[id].DoTimeoutCallback();
                }

                _timeoutDic.Remove(id);
                _bWaitCheck = true;
            }
        }

        public void ClearAllTimeout()
        {
            if (_timeoutDic.Count > 0)
            {
                _timeoutDic.Clear();
                _bWaitCheck = true;
            }
        }

        void FixedUpdate()
        {
            if (_bWaitCheck)
            {
                CheckTimeout();
            }
        }

        void CheckTimeout()
        {
            CancelInvoke("SetTimeout");

            _tempHs.Clear();
            _bWaitCheck = false;

            DateTime nowTime = DateTime.Now;
            TimeoutObject minObject = null;
            float minTime = 0f;

            foreach (long timeoutId in _timeoutDic.Keys)
            {
                TimeoutObject timeoutObject = _timeoutDic[timeoutId];
                float deltaTime = (float)(nowTime - timeoutObject.dateTime).TotalMilliseconds / 1000f;
                if (deltaTime >= timeoutObject.timeout)
                {
                    _tempHs.Add(timeoutObject);
                }
                else
                {
                    deltaTime = timeoutObject.timeout - deltaTime;
                    if (minObject == null || deltaTime < minTime)
                    {
                        minObject = timeoutObject;
                        minTime = deltaTime;
                    }
                }
            }

            foreach (TimeoutObject timeoutObject in _tempHs)
            {
                _timeoutDic.Remove(timeoutObject.ID);
                timeoutObject.DoTimeoutCallback();
            }

            if (minObject != null)
            {
                Invoke("SetTimeout", minTime);
            }
        }

        void SetTimeout()
        {
            _bWaitCheck = true;
        }

        #region private class
        sealed class TimeoutObject
        {
            static long _sequence = 0;

            public long ID { get; private set; }
            public float timeout { get; private set; }
            public DateTime dateTime { get; private set; }
            Action<long, object> timeoutDelegate { get; set; }
            object timeoutParameter { get; set; }

            public TimeoutObject(float time, Action<long, object> callback, object parameter = null)
            {
                ID = ++_sequence;
                timeout = time;
                dateTime = DateTime.Now;
                timeoutDelegate = callback;
                timeoutParameter = parameter;
            }

            public void DoTimeoutCallback()
            {
                timeoutDelegate.Call(ID, timeoutParameter);
                timeoutDelegate = null;
            }
        }
        #endregion
    }
}
