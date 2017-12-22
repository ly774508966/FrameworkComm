using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class FlowComponent : MonoBehaviour
    {
        private enum TriggerType
        {
            None = 0,
            OnTriggerEnter,
            OnTriggerExit,
        }

        [SerializeField]
        private FlowGraph flowGraph;
        [SerializeField]
        private TriggerType triggerType = TriggerType.None;
        [SerializeField]
        private GameObject actor;

        private void Awake()
        {
            if (flowGraph == null)
            {
                Log.Error("[FlowComponent] flow graph is null.");
                return;
            }

            flowGraph.actor = actor;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Execute();
            }
        }

        private void Execute()
        {
            FlowGraphExecutor.Execute(flowGraph);
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (triggerType == TriggerType.OnTriggerEnter)
        //    {
        //        Execute();
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    if (triggerType == TriggerType.OnTriggerExit)
        //    {
        //        Execute();
        //    }
        //}
    }
}