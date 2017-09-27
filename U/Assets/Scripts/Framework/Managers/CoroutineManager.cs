using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 自定义协程管理器，主要用于异步网络请求处理
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public interface ICoroutiner
    {
        bool IsDone();
    }

    public class QCoroutiner : ICoroutiner
    {
        public bool isDone { get; set; }
        public bool IsDone() { return isDone; }
    }

    class CoroutineManager : MonoSingleton<CoroutineManager>
    {
        private List<IEnumerator> _enumerators = new List<IEnumerator>();
        private List<IEnumerator> _enumeratorsBuffer = new List<IEnumerator>();

        void LateUpdate()
        {
            for (int i = 0; i < _enumerators.Count; ++i)
            {
                if (_enumerators[i].Current is ICoroutiner)
                {
                    ICoroutiner yieldInstruction = _enumerators[i].Current as ICoroutiner;
                    if (!yieldInstruction.IsDone())
                    {
                        continue;
                    }
                }

                if (!_enumerators[i].MoveNext())
                {
                    _enumeratorsBuffer.Add(_enumerators[i]);
                    continue;
                }
            }

            for (int i = 0; i < _enumeratorsBuffer.Count; ++i)
            {
                _enumerators.Remove(_enumeratorsBuffer[i]);
            }

            _enumeratorsBuffer.Clear();
        }

        public IEnumerator StartQCoroutine(IEnumerator enumerator)
        {
            _enumerators.Add(enumerator);
            return enumerator;
        }

        public void StopQCoroutine(IEnumerator enumerator)
        {
            if (_enumerators.Contains(enumerator))
                _enumerators.Remove(enumerator);
        }

        public void StopAllQCoroutines()
        {
            _enumerators.Clear();
        }
    }
}
