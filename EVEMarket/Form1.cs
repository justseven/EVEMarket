using EVEMarket.model;
using EVEMarket.until;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVEMarket
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Way> bestWay = new List<Way>();

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = data.DataLoad.GetGoods();
            this.progressBar1.Maximum = dt.Rows.Count;
            this.progressBar1.Step = 0;
            ThreadPool.QueueUserWorkItem((object obj) =>
            {
                foreach (DataRow row in dt.Rows)
                {
                    string typeId = row["typeID"].ToString();
                    QuicklookResult result = GetOrders(typeId);
                    if (null != result)
                    {
                         Way way=GetWay(result);
                        if (null != way && way.priceValue > 0)
                        {
                            AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue}");
                            AddItemToListBox(this.listBox1, "===================================================");
                        }
                    }
                    this.Invoke(new Action(() => {
                        ++this.progressBar1.Step;
                        this.label3.Text = $"{this.progressBar1.Maximum - this.progressBar1.Step}";
                    }));
                }
                bestWay.Sort((a,b) => { var sortIndex=Convert.ToInt32(b.priceValue - a.priceValue);return sortIndex; });
                this.Invoke(new Action(() => {
                    this.listBox1.Items.Clear();
                }));
                foreach (var way in bestWay)
                {
                    if (way.priceValue > 0)
                    {
                        AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue}");
                        AddItemToListBox(this.listBox1, "===================================================");
                    }

                }
                MessageBox.Show("OK");
            });
        }

        private void AddItemToListBox(ListBox box, string content)
        {
            this.Invoke(new Action(() => {
                box.Items.Add(content);
                box.SelectedIndex = box.Items.Count - 1;
            }));
        }


        private QuicklookResult GetOrders(string typeId)
        {
            XML_JSON xML_JSON = new XML_JSON();
            Param param = new Param();
            param.typeid = typeId;
           // param.usesystem = "30000142";
            
           
            try
            {
                string xml = EVEApi.QuickLook(param);
                string json = xML_JSON.XML2Json(xml, "evec_api/quicklook");
                QuicklookResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<QuicklookResult>(json);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private Way GetWay(QuicklookResult result)
        {
            Way way = new Way();
            if (null != result.quicklook.sell_orders && result.quicklook.sell_orders.order.Count > 0 &&
                null != result.quicklook.buy_orders && result.quicklook.buy_orders.order.Count > 0
                )
            {
                //1.查找卖价低于买价 差价最高的
                List<OrderItem> sell_orders = result.quicklook.sell_orders.order;
                List<OrderItem> buy_orders = result.quicklook.buy_orders.order;
                decimal pricevalue = 0;

                foreach (var sell in sell_orders)
                {

                    decimal sellPrice = Convert.ToDecimal(sell.price);

                    foreach (var buy in buy_orders)
                    {

                        decimal buyPrice = Convert.ToDecimal(buy.price);
                        decimal value = buyPrice - sellPrice;
                        if (value > 0 && value > pricevalue && Convert.ToDecimal(sell.vol_remain) > 30 && Convert.ToDecimal(buy.vol_remain) > 30)
                        {
                            way.name = result.quicklook.quicklook;
                            way.buy = buy;
                            way.sell = sell;
                            way.priceValue = value;
                            pricevalue = value;
                        }

                    }

                }
            }
            if(null!=way && way.priceValue>0)
                bestWay.Add(way);
            return way;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Way> result = new List<Way>();
            bestWay.ForEach(i => result.Add(i));
            if (!string.IsNullOrEmpty(txtStart.Text))
            {
                result = bestWay.FindAll(a => a.sell.station_name.Contains(txtStart.Text));
            }
            if(!string.IsNullOrEmpty(txtEnd.Text))
            {
                result = result.FindAll(a => a.buy.station_name.Contains(txtEnd.Text));
            }
            if(!string.IsNullOrEmpty(txtBuyNum.Text))
            {
                result = result.FindAll(a => Convert.ToDecimal(a.buy.vol_remain) > Convert.ToDecimal(txtBuyNum.Text));
            }
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                result = result.FindAll(a => a.name.Contains(txtName.Text));
            }
            if (!string.IsNullOrEmpty(txtSellPrice.Text))
            {
                result = result.FindAll(a => Convert.ToDecimal(a.sell.price) < Convert.ToDecimal(txtSellPrice.Text));
            }
            if (!string.IsNullOrEmpty(txtSellNum.Text))
            {
                result = result.FindAll(a => Convert.ToDecimal(a.sell.vol_remain) > Convert.ToDecimal(txtSellNum.Text));
            }
            this.listBox1.Items.Clear();
            foreach (var way in result)
            {
                if (way.priceValue > 0)
                {
                    AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue}");
                    AddItemToListBox(this.listBox1, "===================================================");
                }

            }
            MessageBox.Show("OK");
        }
    }
    public class Way
    {
        public string name { get; set; }
        public OrderItem sell { get; set; }
        public OrderItem buy { get; set; }
        public decimal priceValue { get; set; }

        private decimal totalPrice;
        public decimal TotalPrice { get
            {
                if (Convert.ToDecimal(sell.vol_remain) < Convert.ToDecimal(buy.vol_remain))
                    return priceValue * Convert.ToDecimal(sell.vol_remain);
                else
                    return priceValue * Convert.ToDecimal(buy.vol_remain);
            }
            set { totalPrice = value; }
        }
    }
}
