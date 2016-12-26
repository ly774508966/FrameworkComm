using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

namespace TikiAL
{
    /// <summary>
    /// 礼品数据
    /// </summary>
    public class GiftModel : BaseModel<GiftModel>
    {
        private string _configFileName = PathConfig.Config + "gift.xml";

        //礼品列表<key=礼品, value=实际剩余数量>
        private Dictionary<Gift, int> _giftDic;

        public List<Gift> giftList
        {
            get
            {
                List<Gift> gift = null;
                if (_giftDic != null)
                    gift = new List<Gift>(_giftDic.Keys);
                return gift;
            }
        }

        public int giftNum
        {
            get { return _giftDic != null ? _giftDic.Count : 0; }
        }

        protected override void InitData()
        {
            InitGiftDictionary(LoadGiftInfoFromXml());
            base.InitData();
        }

        /// <summary>
        /// 获取礼品实际剩余数量
        /// </summary>
        public int GetGiftRemain(Gift gift)
        {
            if (HasGift(gift))
                return _giftDic[gift];
            return 0;
        }

        private Gift GetGiftById(int id)
        {
            Gift result = null;

            if (_giftDic != null && _giftDic.Count > 0)
            {
                List<Gift> list = new List<Gift>(_giftDic.Keys);
                if (list != null && list.Count > 0)
                {
                    foreach (Gift gift in list)
                    {
                        if (gift.id == id)
                            result = gift;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 抽取指定礼品，-1表示全部抽取
        /// </summary>
        private bool SendGift(Gift gift, int count = -1)
        {
            if (!HasGift(gift))
                return true;

            bool isEmpty = false;

            if (count < 0)
                _giftDic[gift] = 0;         //全部抽取
            else
                _giftDic[gift] -= count;    //抽取一部分

            if (_giftDic[gift] <= 0)
            {
                _giftDic[gift] = 0;
                isEmpty = true;
            }

            Log.Debug("SendGift() id = " + gift.id.ToString() + ", name = " + gift.name + ", count = " + count.ToString() + ", remain = " + _giftDic[gift].ToString() + ", isEmpty = " + isEmpty.ToString());

            return isEmpty;
        }

        /// <summary>
        /// 批量抽取礼品
        /// key   = 礼品
        /// value = 抽取数量
        /// </summary>
        private void SendGift(Dictionary<Gift, int> sendDic)
        {
            if (sendDic == null)
                return;

            foreach (KeyValuePair<Gift, int> kvp in sendDic)
            {
                SendGift(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// 抽取指定礼品，-1表示全部抽取
        /// </summary>
        public bool SendGift(int giftId, int count = -1)
        {
            return SendGift(GetGiftById(giftId), count);
        }

        /// <summary>
        /// 批量抽取礼品
        /// key   = 礼品id
        /// value = 抽取数量
        /// </summary>
        public void SendGift(Dictionary<int, int> giftCountDic)
        {
            if (giftCountDic == null || giftCountDic.Count == 0)
                return;

            Dictionary<Gift, int> sendDic = new Dictionary<Gift, int>();
            foreach (KeyValuePair<int, int> kvp in giftCountDic)
            {
                Gift gift = GetGiftById(kvp.Key);
                if (gift != null)
                    sendDic.Add(gift, kvp.Value);
            }

            SendGift(sendDic);
        }

        /// <summary>
        /// 保存礼品配置到Xml文件
        /// </summary>
        private bool SaveGiftInfoToXml(GiftInfo giftinfo)
        {
            return Util.SaveToXml(_configFileName, giftinfo);
        }

        private bool HasGift(Gift gift)
        {
            return _giftDic != null && gift != null && _giftDic.ContainsKey(gift);
        }

        /// <summary>
        /// 从Xml文件读取礼品配置
        /// </summary>
        /// <returns>礼品数目</returns>
        private GiftInfo LoadGiftInfoFromXml()
        {
            GiftInfo info;

            if (Util.LoadFromXml(_configFileName, out info))
            {
                int length = (info != null && info.gifts != null) ? info.gifts.Length : 0;
                Log.Debug("LoadGiftInfoFromXml() Gift num = " + length.ToString());
            }
            else
            {
                Log.Debug("LoadGiftInfoFromXml() error, GiftInfo from xml is null.");
            }

            return info;
        }

        /// <summary>
        /// 根据配置信息，初始化礼品列表和剩余数量
        /// </summary>
        private int InitGiftDictionary(GiftInfo info)
        {
            if (info == null || info.gifts == null)
                return 0;

            int length = info.gifts.Length;
            if (length > 0)
            {
                if (_giftDic == null)
                    _giftDic = new Dictionary<Gift, int>();

                for (int i = 0; i < length; i++)
                {
                    Gift gift = info.gifts[i];
                    if (!_giftDic.ContainsKey(gift))
                        _giftDic.Add(gift, gift.count);
                }
            }

            return length;
        }
    }
}