using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EVEMarket.until
{
    public class XML_JSON
    {
        /// <summary>
        /// 返回指定节点下信息的JSON格式字符串
        /// </summary>
        /// <param name="str">xml字符串</param>
        /// <param name="nodename">节点名称，应从根节点开始</param>
        /// <returns></returns>
        public string XML2Json(string str, string nodename)
        {
            try
            {
                string result = null;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(str);
                XmlNode node = xmldoc.SelectSingleNode(nodename);
                result = Newtonsoft.Json.JsonConvert.SerializeXmlNode(node);
                return result;
            }
            catch
            {
                return "";
            }
        }

        public string Json2XML(string str)
        {
            string result = null;
            XmlDocument xml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(str);
            result = xml.OuterXml;
            return result;
        }
    }
}
