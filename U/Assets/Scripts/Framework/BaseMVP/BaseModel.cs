using System.Collections.Generic;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    /// <summary>
    /// Model单例基类
    /// </summary>
    public abstract class ModelSingleton<T> : BaseModel where T : BaseModel, new()
    {
        protected static T _instance = null;
        private static readonly object _syslock = new object();

        public static T instance
        {
            get
            {
                lock (_syslock)
                {
                    if (_instance == null || _instance.bDestroyed)
                    {
                        _instance = new T();
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Model逻辑数据层基类
    /// </summary>
    public abstract class BaseModel : IEventListener
    {
        public static List<BaseModel> modelList = new List<BaseModel>();

        protected delegate bool EventHandler(object param1, object param2);

        public bool bDestroyed = false;

        protected float _fTimeout = 5f;

        private bool _bEnabled = true;

        private Dictionary<MSGID, EventHandler> _msgidDic = null;
        private Dictionary<MSGID, QCoroutiner> _msgidQCDic = null;

        private List<ModelTimeoutParameter> _timeoutList = null;

        public static void DestroyAllModels()
        {
            for (int i = 0; i < modelList.Count; i++)
                modelList[i].Destory();
            modelList.Clear();
        }

        public BaseModel()
        {
            bDestroyed = false;
            InitEvent(true);
            InitData();
            modelList.Add(this);
        }

        public virtual bool enabled
        {
            get { return _bEnabled; }
            set { _bEnabled = value; }
        }

        public virtual void Destory()
        {
            bDestroyed = true;
            InitEvent(false);
            RemoveAllTimeout();
        }

        protected void AddTimeout(MSGID id, ModelEvent modelEvent, object param1, object param2, float time = 5.0f)
        {
            AddTimeout((int)id, modelEvent, param1, param2, time);
        }

        protected void RemoveTimeout(MSGID id)
        {
            RemoveTimeout((int)id);
        }

        protected void RemoveAllTimeout()
        {
            for (int i = 0; i < _timeoutList.Count(); i++)
            {
                ModelTimeoutParameter param = _timeoutList[i];
                TimeoutManager.instance.ClearTimeout(param.lTimeId);
            }
            _timeoutList.Clear();
        }

        protected QCoroutiner GetQCoroutiner(MSGID id)
        {
            if (_msgidQCDic == null)
            {
                _msgidQCDic = new Dictionary<MSGID, QCoroutiner>();
            }
            if (!_msgidQCDic.ContainsKey(id))
            {
                _msgidQCDic[id] = new QCoroutiner();
            }
            return _msgidQCDic[id];
        }

        protected bool IsQCoroutinerRuning(MSGID id)
        {
            if (_msgidQCDic != null && _msgidQCDic.ContainsKey(id))
            {
                return _msgidQCDic[id].isDone == false;
            }
            return false;
        }

        protected QCoroutiner SetQCoroutinerIsDone(MSGID id, bool value, bool autoCreate = true)
        {
            if (autoCreate)
            {
                GetQCoroutiner(id);
            }
            if (_msgidQCDic != null && _msgidQCDic.ContainsKey(id))
            {
                _msgidQCDic[id].isDone = value;
                return _msgidQCDic[id];
            }
            return null;
        }

        protected QCoroutiner SendMsg(MSGID id, object msg, bool isQCoroutiner = true, bool bNeedEncrypt = true)
        {
            if (enabled == false)
                return null;
            //NetConnectionManager.instance.SendMsg(id, msg, bNeedEncrypt);
            if (isQCoroutiner)
                return SetQCoroutinerIsDone(id, false);
            return null;
        }

        protected virtual void OnDestory() { }

        /// <summary>
        /// 监听网络事件，以及事件处理函数
        /// </summary>
        protected virtual void InitEvent(Dictionary<MSGID, EventHandler> msgidDic) { }

        /// <summary>
        /// 数据加载和初始化
        /// </summary>
        protected virtual void InitData() { }

        public virtual bool OnFireEvent(uint dispatcherId, uint key, object param1, object param2)
        {
            EventHandler handlers = null;

            bool bRet = false;

            if (dispatcherId == (uint)EventDispatcherId.MSGID)
            {
                handlers = _msgidDic[(MSGID)key];
                RemoveTimeout((MSGID)key);
                if (enabled)
                    handlers(param1, param2);
                SetQCoroutinerIsDone((MSGID)key, true, false);
            }

            return bRet;
        }

        public virtual int GetListenerPriority(uint eventKey)
        {
            return 0;
        }

        void InitEvent(bool bIsAttach)
        {
            if (_msgidDic == null && bIsAttach)
            {
                _msgidDic = new Dictionary<MSGID, EventHandler>();
                InitEvent(_msgidDic);
            }

            foreach (MSGID key in _msgidDic.Keys.CheckNull())
            {
                EventDispatcherManager.instance.AttachListener(this, key, bIsAttach);
            }
        }

        void AddTimeout(int id, ModelEvent modelEvent, object param1, object param2, float timeLimit)
        {
            RemoveTimeout(id);

            if (_timeoutList == null)
            {
                _timeoutList = new List<ModelTimeoutParameter>();
            }

            ModelTimeoutParameter param = new ModelTimeoutParameter(id, modelEvent, param1, param2);
            param.lTimeId = TimeoutManager.instance.CreateTimeout(TimeoutCallBack, timeLimit, param);

            _timeoutList.Add(param);
        }

        void RemoveTimeout(int id)
        {
            for (int i = 0; i < _timeoutList.Count(); i++)
            {
                ModelTimeoutParameter param = _timeoutList[i];
                if (param.id == id)
                {
                    _timeoutList.RemoveAt(i);
                    TimeoutManager.instance.ClearTimeout(param.lTimeId);
                    break;
                }
            }
        }

        void TimeoutCallBack(long timeId, object param = null)
        {
            if (!_timeoutList.IsNullOrEmpty())
            {
                ModelTimeoutParameter timeoutParam = param as ModelTimeoutParameter;

                RemoveTimeout(timeoutParam.id);
                if (enabled)
                    EventDispatcherManager.instance.FireEvent(timeoutParam.modelEvent, timeoutParam.param1, timeoutParam.param2);
                SetQCoroutinerIsDone((MSGID)timeoutParam.id, true, false);
            }
        }
    }

    sealed class ModelTimeoutParameter
    {
        public int id { get; private set; }
        public long lTimeId { get; set; }
        public ModelEvent modelEvent { get; private set; }
        public object param1 { get; private set; }
        public object param2 { get; private set; }

        public ModelTimeoutParameter(int id, ModelEvent modelEvent, object param1 = null, object param2 = null)
        {
            this.id = id;
            this.modelEvent = modelEvent;
            this.param1 = param1;
            this.param2 = param2;

            lTimeId = 0;
        }
    }
}