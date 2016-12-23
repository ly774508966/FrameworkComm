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
            base.InitData();
        }

        public string GetGuestNameByIndex(int index)
        {
            if (index < 0 || index >= guestNum)
                return null;
            return _guestName[index];
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
                Log.Debug("LoadGuestNameFromFolder() error, folder name is not exist.");
            }

            return length;
        }
    }
}