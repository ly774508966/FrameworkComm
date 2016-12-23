using System;

namespace Framework
{
    public class Callback
    {
        public delegate void FunVoid();
        public delegate void FunObject(object obj);

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
    }
}
