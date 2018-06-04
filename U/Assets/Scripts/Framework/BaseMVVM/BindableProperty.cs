using System;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public sealed class BindableProperty<T>
    {
        public Action<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged.Call(_value);
                }
            }
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : "";
        }
    }
}