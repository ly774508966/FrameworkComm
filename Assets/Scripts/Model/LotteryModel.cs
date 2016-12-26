using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Framework;

namespace TikiAL
{
    /// <summary>
    /// 抽奖数据
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
        /// 根据嘉宾name查询中奖信息，返回礼品id，-1表示未中奖
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
        /// 获取当前能够参与抽奖的嘉宾列表，返回列表长度
        /// </summary>
        public int GetCanLotteryGuestList(out List<string> result)
        {
            result = null;

            List<string> allGuestList = GuestModel.instance.allGuestList;
            if (allGuestList != null)
            {
                int length = allGuestList.Count;
                if (length > 0)
                {
                    if (_lotteryDic == null || _lotteryDic.Count == 0)
                    {//没有嘉宾中奖
                        result = allGuestList;
                    }
                    else
                    {//嘉宾中奖，不能再次参与抽奖
                        result = new List<string>();

                        for (int i = 0; i < length; i++)
                        {
                            string name = allGuestList[i];
                            if (_lotteryDic.ContainsKey(name) && _lotteryDic[name] >= 0)
                                continue;
                            else
                                result.Add(name);
                        }
                    }
                }
            }

            return result != null ? result.Count : 0;
        }

        /// <summary>
        /// 根据嘉宾name，设置嘉宾的中奖信息(礼品id)
        /// </summary>
        public void Lottery(string name, int giftId)
        {
            SetLotteryDictionary(name, giftId);
            SaveLotteryDicToFile();
        }

        /// <summary>
        /// 根据嘉宾name列表，批量设置嘉宾的中奖信息(礼品id)
        /// </summary>
        public void Lottery(List<string> name, int giftId)
        {
            int length = name != null ? name.Count : 0;
            for (int i = 0; i < length; i++)
            {
                SetLotteryDictionary(name[i], giftId);
            }
            SaveLotteryDicToFile();
        }

        /// <summary>
        /// 根据嘉宾抽奖的本地数据，设置嘉宾的中奖信息
        /// </summary>
        public void SetLotteryFromHistory()
        {
            if (_lotteryDic != null && _lotteryDic.Count > 0)
            {
                Dictionary<int, int> giftDic = new Dictionary<int, int>();
                foreach (int giftId in _lotteryDic.Values)
                {
                    if (!giftDic.ContainsKey(giftId))
                        giftDic.Add(giftId, 1);
                    else
                        giftDic[giftId]++;
                }

                Log.Debug("SetLotteryFromHistory() Dic length = " + giftDic.Count.ToString());

                GiftModel.instance.SendGift(giftDic);
            }
        }

        /// <summary>
        /// 清除嘉宾中奖信息
        /// </summary>
        public void ResetLottery()
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
                Util.SaveDictionaryToText(_lotteryDic, _txtFileName, "————嘉宾抽奖结果————");
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

            if (Util.LoadDictFromFile(_lotteryDic, _lotteryFileName))
                Log.Debug("LoadLotteryDicFromFile() Dic length = " + _lotteryDic.Count.ToString());
            else
                Log.Debug("LoadLotteryDicFromFile() failed.");
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