using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace TikiAL
{
    [XmlRoot("GiftRoot", Namespace = "TikiAL", IsNullable = true)]
    public class GiftInfo
    {
        public GiftInfo() { }

        [XmlArray("GiftList")]
        public Gift[] gifts { set; get; }
    }

    public class Gift
    {
        public Gift() { }
        public Gift(int index, string level, string name, string picture, int count, int price)
        {
            this.index = index;
            this.level = level;
            this.name = name;
            this.picture = picture;
            this.count = count;
            this.price = price;
        }

        public int index { set; get; }
        public string level { set; get; }
        public string name { set; get; }
        public string picture { set; get; }
        public int count { set; get; }
        public int price { set; get; }
    }
}