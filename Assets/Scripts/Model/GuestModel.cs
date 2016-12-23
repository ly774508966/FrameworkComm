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
            LoadGuestLotteryDicFromFile();
            base.InitData();
        }

        public string GetGuestNameByIndex(int index)
        {
            if (index < 0 || index >= guestNum)
                return null;
            return _guestName[index];
        }

        /// <summary>
        /// 查询某个嘉宾是否中奖，以及奖项level，-1表示未中奖
        /// </summary>
        public int GetLotteryLevelByName(string name)
        {
            if (_guestLotteryDic != null && _guestLotteryDic.ContainsKey(name))
                return _guestLotteryDic[name];
            return -1;
        }

        /// <summary>
        /// 根据嘉宾索引，设置该嘉宾的中奖信息
        /// </summary>
        public void SetLotteryLevelByIndex(int index, int level = -1)
        {
            string name = GetGuestNameByIndex(index);
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

        private void LoadGuestLotteryDicFromFile()
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

        public void SaveGuestLotteryDicToFile()
        {
            if (_guestLotteryDic != null)
            {
                Util.SaveDictToFile(_guestLotteryDic, _guestLotteryFileName);
            }
        }
    }
}