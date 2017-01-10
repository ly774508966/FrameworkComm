using System.Xml.Serialization;

namespace Project
{
    [XmlRoot("InfoRoot", Namespace = "Project", IsNullable = true)]
    public class Info
    {
        public Info() { }

        [XmlArray("GiftList")]
        public Item[] items { set; get; }
    }

    public class Item
    {
        public Item() { }
        public Item(int id, string name, string picture)
        {
            this.id = id;
            this.name = name;
            this.picture = picture;
        }

        public int id { set; get; }
        public string name { set; get; }
        public string picture { set; get; }
    }
}