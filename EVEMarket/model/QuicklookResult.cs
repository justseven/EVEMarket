using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarket.model
{
  
    public class Regions
    {
        /// <summary>
        /// 伏尔戈
        /// </summary>
        public string region { get; set; }
    }

    public class OrderItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string @id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string station { get; set; }
        /// <summary>
        /// 吉他 IV - 卫星 4 - 加达里海军 组装车间
        /// </summary>
        public string station_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string security { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string range { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vol_remain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string min_volume { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expries { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reported_time { get; set; }
    }

    public class Sell_orders
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OrderItem> order { get; set; }
    }

    public class Buy_orders
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OrderItem> order { get; set; }
    }

    public class Quicklook
    {
        /// <summary>
        /// 
        /// </summary>
        public string item { get; set; }
        /// <summary>
        /// 三钛合金
        /// </summary>
        public string quicklook { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Regions regions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hours { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string minqty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Sell_orders sell_orders { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Buy_orders buy_orders { get; set; }
    }

    public class QuicklookResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Quicklook quicklook { get; set; }
    }
}
