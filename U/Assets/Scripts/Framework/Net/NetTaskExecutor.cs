using System;
using System.Collections.Generic;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    class NetTaskExecutor
    {
        private List<Action> _actions = new List<Action>();
        private List<Action> _currentActions = new List<Action>();

        public void Update()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }

                for (int i = 0; i < _currentActions.Count; i++)
                {
                    _currentActions[i].Call();
                }
            }
        }

        public void Add(Action action)
        {
            lock (_actions)
            {
                _actions.Add(action);
            }
        }

        public void Clear()
        {
            lock (_actions)
            {
                _actions.Clear();
            }
        }
    }
}
