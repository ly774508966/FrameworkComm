﻿&&MenuName=FlowNode&&
&&Priority=2&&
using System.Collections;

/// <summary>
/// @##Author##
/// </summary>
namespace Framework
{
    public class ##ClassName## : FlowNode
    {
        public override string NodeName
        {
            get { return "##ClassName##"; }
        }

        public override float NodeWidth
        {
            get { return 150f; }
        }

        public override float NodeHeight
        {
            get { return 50f; }
        }

        public override void OnDrawProperty()
        {
            base.OnDrawProperty();
        }

        public override void OnDrawNode()
        {
            base.OnDrawNode();
        }

        public override IEnumerator OnExecute()
        {
            yield return base.OnExecute();
            FinishExecute();
        }

        public override bool CheckExecutable()
        {
            return base.CheckExecutable();
        }
    }
}