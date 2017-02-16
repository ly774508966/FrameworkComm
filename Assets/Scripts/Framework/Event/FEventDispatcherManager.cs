using System;

namespace Framework
{
    public enum EventDispatcherId { MODEL, VIEW };

    public class FEventDispatcherManager : FSingleton<FEventDispatcherManager>
    {
        private FEventDispatcher[] m_dispatchers = new FEventDispatcher[2];

        public FEventDispatcherManager()
        {
            m_dispatchers[0] = new FEventDispatcher();
            m_dispatchers[0].id = (uint)EventDispatcherId.MODEL;

            m_dispatchers[1] = new FEventDispatcher();
            m_dispatchers[1].id = (uint)EventDispatcherId.VIEW;
        }

        private FEventDispatcher GetByEventTypeName(string evtTypeName, ref int iEventType)
        {
            if (evtTypeName.IndexOf("ModelEvent") != -1)
            {
                iEventType = (int)EventDispatcherId.MODEL;
                return m_dispatchers[iEventType];
            }
            else if (evtTypeName.IndexOf("MSGID") != -1)
            {
                iEventType = (int)EventDispatcherId.VIEW;
                return m_dispatchers[iEventType];
            }
            return null;
        }

        private string GetEventNameById(uint iEventId, int iEventType)
        {
            if (iEventType == (int)EventDispatcherId.MODEL)
                return ((ModelEvent)iEventId).ToString();
            else if (iEventType == (int)EventDispatcherId.VIEW)
                return ((ViewEvent)iEventId).ToString();
            return null;
        }

        public void FireEvent<T>(T eventId, object param1 = null, object param2 = null)
        {
            int iEventType = 0;
            FEventDispatcher evtDispatcher = GetByEventTypeName(typeof(T).FullName, ref iEventType);
            if (evtDispatcher != null)
            {
                uint iEventId = Convert.ToUInt32(eventId);
                string eventName = GetEventNameById(iEventId, iEventType);
                if (!string.IsNullOrEmpty(eventName))
                {
                    FLog.Debug("FireSync -> " + eventName);
                }
                evtDispatcher.FireEvent(iEventId, param1, param2);
            }
            else
            {
                FLog.Error("Error event type: " + typeof(T).FullName);
            }
        }

        public void AttachListener<T>(IEventListener listener, T eventId, bool bIsAttach = true)
        {
            int idx = 0;
            FEventDispatcher evtDispatcher = GetByEventTypeName(typeof(T).FullName, ref idx);
            if (evtDispatcher != null)
                evtDispatcher.AttachListenerNow(listener, Convert.ToUInt32(eventId), bIsAttach);
            else
                FLog.Error("Error event type: " + typeof(T).FullName);
        }
    }
}
