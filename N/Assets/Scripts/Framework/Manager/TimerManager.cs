using System;
using System.Collections.Generic;

namespace Framework
{
    public class TimerManager : FMonoSingleton<TimerManager>
    {
        public delegate void TimeroutCallBack(long id, object param1, object param2);

        private Dictionary<long, TimeoutParam> _timerDic = new Dictionary<long, TimeoutParam>();
        private List<TimeoutParam> _timeoutList = new List<TimeoutParam>();    //time out
        private long _seq = 0;
        private bool _isChange = false;

        void FixedUpdate()
        {
            if (_isChange)
                CheckTimeout();
        }

        public long SetTimeout(TimeroutCallBack callBack, float t, object param1 = null, object param2 = null)
        {
            _seq++;
            TimeoutParam param = new TimeoutParam();
            param.callBack = callBack;
            param.time = t;
            param.dateTime = DateTime.Now;
            param.param1 = param1;
            param.param2 = param2;
            param.id = _seq;
            _timerDic[_seq] = param;

            _isChange = true;

            return _seq;
        }

        public void ClearTimeout(long id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _timerDic.Remove(id);
                _isChange = true;
            }
        }

        public void ClearAllTimeout()
        {
            if (_timerDic.Count > 0)
            {
                _timerDic.Clear();
                _isChange = true;
            }
        }

        private void CheckTimeout()
        {
            CancelInvoke("TimeoutCallBack");
            DateTime nowTime = DateTime.Now;
            TimeoutParam minParam = null;
            float minTime = 0;
            _timeoutList.Clear();
            _isChange = false;

            foreach (long id in _timerDic.Keys)
            {
                TimeoutParam param = _timerDic[id];
                float t = (float)(nowTime - param.dateTime).TotalMilliseconds / 1000.0f;
                if (t >= param.time)
                {
                    _timeoutList.Add(param);
                }
                else
                {
                    t = param.time - t;
                    if (minParam == null || t < minTime)
                    {
                        minParam = param;
                        minTime = t;
                    }
                }
            }

            for (int i = 0; i < _timeoutList.Count; i++)
            {
                TimeoutParam param = _timeoutList[i];
                _timerDic.Remove(param.id);
                param.callBack(param.id, param.param1, param.param2);
            }

            if (minParam != null)
            {
                Invoke("TimeoutCallBack", minTime);
            }

            _timeoutList.Clear();
        }

        private void TimeoutCallBack()
        {
            _isChange = true;
        }
    }

    class TimeoutParam
    {
        public long id;
        public float time;
        public DateTime dateTime;
        public TimerManager.TimeroutCallBack callBack;
        public object param1;
        public object param2;
    }
}
