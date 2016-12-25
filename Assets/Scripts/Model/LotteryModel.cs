using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Framework;

namespace TikiAL
{
    /// <summary>
    /// 中奖数据，耦合GuestModel和GiftModel
    /// </summary>
    public class LotteryModel : BaseModel<LotteryModel>
    {
        private string _lotteryFileName = PathConfig.Debug + "lottery.tikial";
        private string _txtFileName = PathConfig.Debug + "result.txt";

        //嘉宾抽奖结果 <key = guestName, value = giftId>
        private Dictionary<string, int> _lotteryDic;

        protected override void InitData()
        {
            LoadLotteryDicFromFile();
            base.InitData();
        }

        /// <summary>
        /// 根据嘉宾name查询中奖信息，返回奖项id，-1表示未中奖
        /// </summary>
        public int GetGiftIdByGuestName(string name)
        {
            if (!GuestModel.instance.IsValidGuestName(name))
                return -1;
            if (_lotteryDic != null && _lotteryDic.ContainsKey(name))
                return _lotteryDic[name];
            return -1;
        }

        /// <summary>
        /// 根据嘉宾index查询中奖信息，返回奖项id，-1表示未中奖
        /// </summary>
        public int GetGiftIdByGuestIndex(int index)
        {
            if (!GuestModel.instance.IsValidGuestIndex(index))
                return -1;
            return GetGiftIdByGuestName(GuestModel.instance.GetGuestNameByIndex(index));
        }

        /// <summary>
        /// 根据嘉宾index，设置该嘉宾的中奖信息(奖品id)
        /// </summary>
        public void SetLotteryByGuestIndex(int index, int giftId)
        {
            SetLotteryDictionary(GuestModel.instance.GetGuestNameByIndex(index), giftId);
            SaveLotteryDicToFile();
        }

        /// <summary>
        /// 根据嘉宾index数组，批量设置嘉宾的中奖信息(奖品id)
        /// </summary>
        public void SetLotteryByGuestIndex(int[] index, int giftId)
        {
            int length = index != null ? index.Length : 0;
            for (int i = 0; i < length; i++)
            {
                SetLotteryDictionary(GuestModel.instance.GetGuestNameByIndex(index[i]), giftId);
            }
            SaveLotteryDicToFile();
        }

        /// <summary>
        /// 继续抽奖，基于历史嘉宾抽奖数据
        /// </summary>
        public void SendGiftByLottery()
        {
            if (_lotteryDic != null && _lotteryDic.Count > 0)
            {
                Dictionary<int, int> idCountDic = new Dictionary<int, int>();
                foreach (int giftId in _lotteryDic.Values)
                {
                    if (!idCountDic.ContainsKey(giftId))
                        idCountDic.Add(giftId, 1);
                    else
                        idCountDic[giftId]++;
                }

                Log.Debug("SendGiftByLottery() continue lottery from history data.");

                GiftModel.instance.SendGift(idCountDic);
            }
        }

        /// <summary>
        /// 清理嘉宾中奖信息
        /// </summary>
        public void ClearLottery()
        {
            if (_lotteryDic != null)
            {
                _lotteryDic.Clear();
                _lotteryDic = null;
            }
        }

        /// <summary>
        /// 导出嘉宾中奖信息到文本文件
        /// </summary>
        public void ExportLotteryInfo()
        {
            if (_lotteryDic != null || _lotteryDic.Count > 0)
            {
                Util.SaveDictionaryToTxt(_lotteryDic, _txtFileName, "————嘉宾抽奖结果————");
                Log.Debug("ExportLotteryInfo() success, length = " + _lotteryDic.Count.ToString());
            }
        }

        private void SetLotteryDictionary(string name, int giftId)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_lotteryDic == null)
                _lotteryDic = new Dictionary<string, int>();

            if (giftId >= 0)
            {//中奖
                if (!_lotteryDic.ContainsKey(name))
                    _lotteryDic.Add(name, giftId);
                else
                    _lotteryDic[name] = giftId;
            }
            else
            {//未中奖
                if (_lotteryDic.ContainsKey(name))
                    _lotteryDic.Remove(name);
            }

            Log.Debug("SetLotteryDictionary() Name = " + name + ", GiftId = " + giftId.ToString());
        }

        private void LoadLotteryDicFromFile()
        {
            if (_lotteryDic == null)
                _lotteryDic = new Dictionary<string, int>();

            Util.LoadDictFromFile(_lotteryDic, _lotteryFileName);
            Log.Debug("LoadLotteryDicFromFile() dic length = " + _lotteryDic.Count.ToString());
        }

        private void SaveLotteryDicToFile()
        {
            if (_lotteryDic != null)
            {
                Util.SaveDictToFile(_lotteryDic, _lotteryFileName);
            }
        }
    }
}