using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Framework;

namespace TikiAL
{
    public class GuestModel : BaseModel<GuestModel>
    {
        private string _folderName = PathConfig.Guest;
        private string _guestLotteryFileName = PathConfig.Debug + "guestlottery.tikial";
        private string _resultFileName = PathConfig.Debug + "result.txt";

        //嘉宾抽奖结果 <key=嘉宾姓名, value=所中奖项level>
        private Dictionary<string, int> _guestLotteryDic;
        //嘉宾列表
        private List<string> _guestName;

        public List<string> guestName
        {
            get { return _guestName; }
        }

        public int guestNum
        {
            get { return _guestName != null ? _guestName.Count : 0; }
        }

        protected override void InitData()
        {
            LoadGuestNameFromFolder();
            LoadLotteryDicFromFile();
            base.InitData();
        }

        /// <summary>
        /// 根据嘉宾索引，获取嘉宾姓名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetNameByGuestIndex(int index)
        {
            if (!IsValidGuestIndex(index))
                return null;
            return _guestName[index];
        }

        /// <summary>
        /// 根据嘉宾姓名查询中奖信息，返回奖项level，-1表示未中奖
        /// </summary>
        public int GetLotteryByGuestName(string name)
        {
            if (!IsValidGuestName(name))
                return -1;
            if (_guestLotteryDic != null && _guestLotteryDic.ContainsKey(name))
                return _guestLotteryDic[name];
            return -1;
        }

        public int GetLotteryByGuestIndex(int index)
        {
            if (!IsValidGuestIndex(index))
                return -1;
            return GetLotteryByGuestName(GetNameByGuestIndex(index));
        }

        /// <summary>
        /// 根据嘉宾索引，设置该嘉宾的中奖信息
        /// </summary>
        public void SetLotteryByGuestIndex(int index, int level = -1)
        {
            SetLotteryDictionary(GetNameByGuestIndex(index), level);
            SaveLotteryDicToFile();
        }

        /// <summary>
        /// 根据嘉宾索引数组，批量设置该嘉宾的中奖信息
        /// </summary>
        public void SetLotteryByGuestIndex(int[] index, int[] level)
        {
            int iLength = index != null ? index.Length : 0;
            int lLength = level != null ? level.Length : 0;
            if (iLength == lLength)
            {
                for (int i = 0; i < iLength; i++)
                {
                    SetLotteryDictionary(GetNameByGuestIndex(index[i]), level[i]);
                }
                SaveLotteryDicToFile();
            }
        }

        public void SaveGuestLotteryResult()
        {
            if (_guestLotteryDic != null || _guestLotteryDic.Count > 0)
            {
                Util.SaveDictionaryToTxt(_guestLotteryDic, _resultFileName, "————嘉宾抽奖结果————");
                Log.Debug("SaveGuestLotteryResult() success, length = " + _guestLotteryDic.Count.ToString());
            }
        }

        private bool IsValidGuestIndex(int index)
        {
            return index >= 0 && index < guestNum;
        }

        private bool IsValidGuestName(string name)
        {
            return !string.IsNullOrEmpty(name) && _guestName.Contains(name);
        }

        private void SetLotteryDictionary(string name, int level)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_guestLotteryDic == null)
                _guestLotteryDic = new Dictionary<string, int>();

            if (level >= 0)
            {//中奖
                if (!_guestLotteryDic.ContainsKey(name))
                    _guestLotteryDic.Add(name, level);
                else
                    _guestLotteryDic[name] = level;
            }
            else
            {//未中奖
                if (_guestLotteryDic.ContainsKey(name))
                    _guestLotteryDic.Remove(name);
            }

            Log.Debug("SetLotteryDictionary() key = " + name + ", value = " + level.ToString());
        }

        private int LoadGuestNameFromFolder()
        {
            int length = 0;

            DirectoryInfo dir = new DirectoryInfo(_folderName);
            if (dir.Exists)
            {
                if (_guestName == null)
                    _guestName = new List<string>();

                FileInfo[] fInfo = dir.GetFiles();
                if (fInfo != null)
                {
                    length = fInfo.Length;
                    for (int i = 0; i < length; i++)
                    {
                        string name = fInfo[i].Name.Trim(fInfo[i].Extension.ToCharArray());
                        _guestName.Add(name);
                    }
                }

                Log.Debug("LoadGuestNameFromFolder() guest num = " + length.ToString());
            }
            else
            {
                Log.Debug("LoadGuestNameFromFolder() failed, folder name is not exist.");
            }

            return length;
        }

        private void LoadLotteryDicFromFile()
        {
            if (_guestLotteryDic == null)
                _guestLotteryDic = new Dictionary<string, int>();

            bool isOk = Util.LoadDictFromFile(_guestLotteryDic, _guestLotteryFileName);
            if (!isOk)
            {
                Log.Debug("LoadGuestLotteryDicFromFile() dic length = " + _guestLotteryDic.Count.ToString());
            }
            else
            {
                Log.Debug("LoadGuestLotteryDicFromFile() failed.");
            }
        }

        private void SaveLotteryDicToFile()
        {
            if (_guestLotteryDic != null)
            {
                Util.SaveDictToFile(_guestLotteryDic, _guestLotteryFileName);
            }
        }
    }
}