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
            //LotteryModel.instance.SetLotteryByGuestIndex(new int[] { 0, 1, 2 }, 2);
            LotteryModel.instance.SendGiftByLottery();
        }
    }
}