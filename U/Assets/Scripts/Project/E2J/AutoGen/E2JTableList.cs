//////////////////////////////////////////////////////////////////////////
/// This is an auto-generated script, please do not modify it manually ///
//////////////////////////////////////////////////////////////////////////

using System.Text;
using System.Collections;
using System.Collections.Generic;
using Framework;

namespace Project
{
    public class E2JTableList : E2JLoader
    {
        public static readonly string E2JName = "E2JTableList";

        public string TableName { get; private set; }
        public int KeyType { get; private set; }

        public void Load(Hashtable ht)
        {
            TableName = E2JConvertHelper.O2STrim(ht["TableName"]);
            KeyType = E2JConvertHelper.O2I(ht["KeyType"]);
        }

        public static E2JTableList GetElement(string elementKey)
        {
            return E2JManager.instance.GetElementString(E2JName, elementKey) as E2JTableList;
        }

        public static Dictionary<string, E2JLoader> GetElementTable()
        {
            return E2JManager.instance.GetTableString(E2JName);
        }

        public static E2JTableList Clone(E2JTableList src)
        {
            E2JTableList clone = new E2JTableList();
            clone.TableName = src.TableName;
            clone.KeyType = src.KeyType;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(E2JName).Append("->");
            sb.AppendLine();
            sb.Append("TableName: ").Append(TableName);
            sb.AppendLine();
            sb.Append("KeyType: ").Append(KeyType);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}