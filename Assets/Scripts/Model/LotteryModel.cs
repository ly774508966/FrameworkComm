using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

namespace TikiAL
{
    public class LotteryModel : BaseModel<LotteryModel>
    {
        private string _fileName = PathConfig.Config + "gift.xml";

        private List<Gift> _giftList;
        private List<int> _giftLeftCount;

        public List<Gift> gift
        {
            get { return _giftList; }
        }

        public int giftNum
        {
            get { return _giftList != null ? _giftList.Count : 0; }
        }

        protected override void InitData()
        {
            InitGiftListAndCount(LoadGiftInfoFromXml());
            base.InitData();
        }

        /// <summary>
        /// 根据索引获取对应礼品剩余数量
        /// </summary>
        /// <param name="index">礼品索引</param>
        /// <returns>礼品剩余数量</returns>
        public int GetGiftLeftByIndex(int index)
        {
            if (index < 0 || index >= giftNum)
                return 0;
            return _giftLeftCount[index];
        }

        /// <summary>
        /// 根据索引抽取一定数量的礼品
        /// </summary>
        /// <param name="index">礼品索引</param>
        /// <param name="count">抽取数量，默认-1表示全部抽取</param>
        /// <returns>当前礼品是否全部抽取完毕</returns>
        public bool SendGiftByIndex(int index, int count = -1)
        {
            if (index < 0 || index >= giftNum)
                return true;

            bool isEmpty = false;

            if (count == -1)
                _giftLeftCount[index] = 0;       //全部抽取
            else
                _giftLeftCount[index] -= count;  //抽取一部分

            if (_giftLeftCount[index] <= 0)
            {
                _giftLeftCount[index] = 0;
                isEmpty = true;
            }

            return isEmpty;
        }

        /// <summary>
        /// 保存礼品配置到Xml文件
        /// </summary>
        public bool SaveGiftInfoToXml(GiftInfo giftinfo)
        {
            return Util.SaveToXml(_fileName, giftinfo);
        }

        /// <summary>
        /// 从Xml文件读取礼品配置
        /// </summary>
        /// <returns>礼品数目</returns>
        private GiftInfo LoadGiftInfoFromXml()
        {
            GiftInfo info;

            if (Util.LoadFromXml(_fileName, out info))
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
        private int InitGiftListAndCount(GiftInfo info)
        {
            if (info == null || info.gifts == null)
                return 0;

            int length = info.gifts.Length;
            if (length > 0)
            {
                if (_giftList == null)
                    _giftList = new List<Gift>();
                if (_giftLeftCount == null)
                    _giftLeftCount = new List<int>();

                for (int i = 0; i < length; i++)
                {
                    _giftList.Add(info.gifts[i]);
                    _giftLeftCount.Add(info.gifts[i].count);
                }
            }

            return length;
        }
    }
}