using Newtonsoft.Json;
using SufeiUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarket.until
{
    public class EVEApi
    {
        public static string QuickLook(Param param)
        {
            string url= ConfigurationManager.AppSettings["quicklook"];
            url += param.ToString();
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL这里都是测试     必需项
                Method = "get",//URL     可选项 默认为Get
                Allowautoredirect = true,//是否根据301跳转     可选项   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",////用户的浏览器类型，版本，操作系统     可选项有默认值  
                ContentType = "text/html,application/xhtml+xml,application/xml",
                ResultType = ResultType.String,
                Encoding = System.Text.Encoding.GetEncoding("UTF-8")
            };
            item.Header.Add("Accept-Language", "zh-CN");
            item.Header.Add("Accept-Encoding", "gzip, deflate");
            //得到HTML代码
            HttpResult result = http.GetHtml(item);
            return result.Html;
        }

        public static string getRoute(string origin, string destination)
        {
            string url = ConfigurationManager.AppSettings["route"];
            url = string.Format(url, origin, destination);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL这里都是测试     必需项
                Method = "get",//URL     可选项 默认为Get
                Allowautoredirect = true,//是否根据301跳转     可选项   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",////用户的浏览器类型，版本，操作系统     可选项有默认值  
                ContentType = "text/html,application/json",
                ResultType = ResultType.String,
                Encoding = System.Text.Encoding.GetEncoding("UTF-8")
            };
            item.Header.Add("Accept-Language", "zh-CN");
            item.Header.Add("Accept-Encoding", "gzip, deflate");
            //得到HTML代码
            HttpResult result = http.GetHtml(item);
            return result.Html;
        }
    }

    public class Param
    {
        public string sethours { get; set; } //查询 X 小时内的订单, 默认值为 8760	否 否
        public string typeid { get; set; } //要查询的物品ID, 如 三钛合金 为 34	是 否
        public string setminQ { get; set; } //最小订单可交易量.对于卖单, 指订单剩余数量, 对于买单, 指最小可交易量.默认值为 0	否 否
        public string regionlimit { get; set; } //限定星域ID. 可以多次赋值用于限定多个星域, 默认值是所有星域 否   是
        public string usesystem { get; set; } //限定星系.如果要和 regionlimit 共用, 请保证该星系位于该星域下.默认值是不限定 否   否

        public override string ToString()
        {
            string res = "?";
            if (!string.IsNullOrEmpty(sethours))
                res += $"sethours={sethours}";
            if (!string.IsNullOrEmpty(typeid))
                res += $"typeid={typeid}";
            if (!string.IsNullOrEmpty(setminQ))
                res += $"setminQ={setminQ}";
            if (!string.IsNullOrEmpty(regionlimit))
                res += $"regionlimit={regionlimit}";
            if (!string.IsNullOrEmpty(usesystem))
                res += $"usesystem={usesystem}";
            return res;

        }
    }
}
