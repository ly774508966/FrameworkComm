using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TikiAL
{
    public class MainScene : BaseSceneFadeInOut
    {
        protected override void InitUI()
        {
            base.InitUI();
            //GuestModel.instance.SetLotteryByGuestIndex(new int[] { 0, 1, 2 }, new int[] { 3, 3, 0 });
            //GuestModel.instance.SaveGuestLotteryResult();
        }
    }
}