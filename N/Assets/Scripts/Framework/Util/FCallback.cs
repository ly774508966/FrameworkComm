using System;

namespace Framework
{
    public class FCallback
    {
        public delegate void FunVoid();
        public delegate void FunObject(object o);
        public delegate void FunFloat(float f);
        public delegate void FunInt(int i);
        public delegate void FunLong(long l);
        public delegate void FunString(string s);
        public delegate void FunBool(bool b);

        public static Action CreateVoidAction(FunVoid fCallback)
        {
            return (Action)(delegate ()
            {
                if (fCallback != null)
                    fCallback();
            });
        }

        public static Action<object> CreateAction(FunObject fCallback)
        {
            return (Action<object>)((x) =>
            {
                if (fCallback != null)
                {
                    fCallback(x);
                }
            });
        }

        public static Action<object> CreateAction(FunVoid fCallback)
        {
            return (Action<object>)((x) =>
            {
                if (fCallback != null)
                {
                    fCallback();
                }
            });
        }

        public static FunVoid CreateClosures(object o, FunObject fCallback)
        {
            return delegate ()
            {
                fCallback(o);
            };
        }

        public static FunVoid CreateClosures(int i, FunInt fCallback)
        {
            return delegate ()
            {
                fCallback(i);
            };
        }

        public static FunVoid CreateClosures(float f, FunFloat fCallback)
        {
            return delegate ()
            {
                fCallback(f);
            };
        }

        public static FunVoid CreateClosures(string s, FunString fCallback)
        {
            return delegate ()
            {
                fCallback(s);
            };
        }

        public static EventDelegate.Callback CreateClosuresForEventDelegate(string s, FunString fCallback)
        {
            return delegate ()
            {
                fCallback(s);
            };
        }

        public static EventDelegate.Callback CreateClosuresForEventDelegate(int i, FunInt fCallback)
        {
            return delegate ()
            {
                fCallback(i);
            };
        }

        public static EventDelegate.Callback CreateClosuresForEventDelegate(object o, FunObject fCallback)
        {
            return delegate ()
            {
                fCallback(o);
            };
        }

        public static void Call(FunVoid fCallback)
        {
            if (fCallback != null)
            {
                fCallback();
            }
        }
    }
}
