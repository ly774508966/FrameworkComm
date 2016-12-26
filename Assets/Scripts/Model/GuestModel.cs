﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Framework;

namespace TikiAL
{
    /// <summary>
    /// 嘉宾数据
    /// </summary>
    public class GuestModel : BaseModel<GuestModel>
    {
        private string _folderName = PathConfig.Guest;

        //嘉宾列表
        private List<string> _guestName;

        public List<string> allGuestList
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
            base.InitData();
        }

        public bool IsValidGuestName(string name)
        {
            return !string.IsNullOrEmpty(name) && _guestName.Contains(name);
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

                Log.Debug("LoadGuestNameFromFolder() Guest num = " + length.ToString());
            }
            else
            {
                Log.Debug("LoadGuestNameFromFolder() failed, folder name is not exist.");
            }

            return length;
        }
    }
}