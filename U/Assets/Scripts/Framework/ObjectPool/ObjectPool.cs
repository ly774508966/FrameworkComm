using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class ObjectPool<T> where T : new()
    {
        readonly Stack<T> _stack = new Stack<T>();
        readonly UnityAction<T> _actionOnGet;
        readonly UnityAction<T> _actionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return _stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;

            if (_stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = _stack.Pop();
            }

            _actionOnGet.Call(element);

            return element;
        }

        public void Release(T element)
        {
            if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
                Log.Error("Internal error. Trying to destroy object that is already released to pool.");

            _actionOnRelease.Call(element);
            _stack.Push(element);
        }
    }
}
