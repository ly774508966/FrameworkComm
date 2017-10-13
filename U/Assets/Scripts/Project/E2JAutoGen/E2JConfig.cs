//////////////////////////////////////////////////////////////////////////
/// This is an auto-generated script, please do not modify it manually ///
//////////////////////////////////////////////////////////////////////////

using System.Text;
using System.Collections;
using System.Collections.Generic;
using Framework;

namespace Project
{
    public sealed class E2JConfig : E2JLoader
    {
        public static readonly string E2JName = "E2JConfig";

        public string ConfigName { get; private set; }
        public string ConfigValue { get; private set; }

        public void Load(Hashtable ht)
        {
            ConfigName = E2JConvertHelper.O2STrim(ht["ConfigName"]);
            ConfigValue = E2JConvertHelper.O2STrim(ht["ConfigValue"]);
        }

        public static E2JConfig GetElement(string elementKey)
        {
            return E2JManager.instance.GetElementString(E2JName, elementKey) as E2JConfig;
        }

        public static Dictionary<string, E2JLoader> GetElementTable()
        {
            return E2JManager.instance.GetTableString(E2JName);
        }

        public E2JConfig Clone()
        {
            E2JConfig clone = new E2JConfig();
            clone.ConfigName = ConfigName;
            clone.ConfigValue = ConfigValue;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(E2JName).Append("->");
            sb.AppendLine();
            sb.Append("ConfigName: ").Append(ConfigName);
            sb.AppendLine();
            sb.Append("ConfigValue: ").Append(ConfigValue);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
