using EVEMarket.model;
using EVEMarket.until;
using KuaiDi;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        DataTable goodsDt = data.DataLoad.GetGoods();
        DataTable kjzDt = data.DataLoad.GetKJZ();
        DataTable xxlbDT = data.DataLoad.GetXXLB();
        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bestWay.Clear();
            int count = Convert.ToInt32(txtThreadNum.Text);
            
            this.progressBar1.Maximum = goodsDt.Rows.Count;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
            ThreadPool.QueueUserWorkItem((object obj) =>
            {
                List<Goods> list = (List<Goods>)List_DataTable_Helper.DataTableToListModel<Goods>.ConvertToModel(goodsDt);
                MultipleThread.RunTask<Goods>(list, d =>
                {
                    string typeId = d.typeID;
                    QuicklookResult result = GetOrders(typeId);
                    if (null != result)
                    {
                        Way way = GetWay(result);
                        /*if (null != way && way.priceValue > 0)
                        {
                            AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue}");
                            AddItemToListBox(this.listBox1, "===================================================");
                        }*/
                    }
                    this.Invoke(new Action(() =>
                    {
                        this.progressBar1.PerformStep();
                        
                    }));

                }, count);
                bestWay.Sort((a, b) => { var sortIndex = Convert.ToInt32(b.priceValue - a.priceValue); return sortIndex; });
                this.Invoke(new Action(() =>
                {
                    this.listBox1.Items.Clear();
                }));
                foreach (var way in bestWay)
                {
                    if (way.priceValue > 0)
                    {
                        AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue};跳跃:{way.routeConut}");
                        AddItemToListBox(this.listBox1, "===================================================");
                    }

                }
                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
               
                this.Invoke(new Action(() => {
                    this.label3.Text = ts2.TotalSeconds + "秒";
                }));
                MessageBox.Show("OK");
            });
            
           







            /*  ThreadPool.QueueUserWorkItem((object obj) =>
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
              });*/
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
                            way.routeConut = setRouteCount(sell.station_name, buy.station_name);
                        }

                    }

                }
            }
            if(null!=way && way.priceValue>0)
                bestWay.Add(way);
            return way;
        }

        private int setRouteCount(string sell,string buy)
        {
            string origin = getSystemID(sell);
            string destination = getSystemID(buy);
            if(!string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination))
                return getJumpCount(origin, destination);
            return -1;
        }

        private string getSystemID(string name)
        {
            string xxName = name.Split('-')[0].Split(' ')[0].Trim();
            DataRow rows = xxlbDT.Rows.Find(xxName);//  Select($"空間站ID='{kjzDt}'");
            
            if(null!=rows)
            {
                return rows["星系ID"].ToString();
            }
            return "";
        }

        private int getJumpCount(string origin, string destination)
        {
            try
            {
                string res = EVEApi.getRoute(origin, destination);
                JArray array = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(res);
                return array.Count;
            }
            catch
            {
                return -1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Way> result = new List<Way>();
            bestWay.ForEach(i => result.Add(i));

            if (this.checkBox1.Checked)
            {
                result = bestWay.FindAll(a => a.sell.station_name.Contains(txtStart.Text) && a.buy.station_name.Contains(txtStart.Text));
                result = bestWay.FindAll(a => a.sell.station_name.Contains(txtEnd.Text) && a.buy.station_name.Contains(txtEnd.Text));
            }
            else
            {

                if (!string.IsNullOrEmpty(txtStart.Text))
                {
                    result = bestWay.FindAll(a => a.sell.station_name.Contains(txtStart.Text));
                }
                if (!string.IsNullOrEmpty(txtEnd.Text))
                {
                    result = result.FindAll(a => a.buy.station_name.Contains(txtEnd.Text));
                }
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
            if(!string.IsNullOrEmpty(txtPriceValue.Text))
            {
                result = result.FindAll(a =>a.priceValue* (Convert.ToDecimal(a.sell.vol_remain) < Convert.ToDecimal(a.buy.vol_remain)? Convert.ToDecimal(a.sell.vol_remain): Convert.ToDecimal(a.buy.vol_remain)) > Convert.ToDecimal(txtPriceValue.Text));
            }
            this.listBox1.Items.Clear();
            foreach (var way in result)
            {
                if (way.priceValue > 0)
                {
                    AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue};跳跃:{way.routeConut}");
                    AddItemToListBox(this.listBox1, "===================================================");
                }

            }
            MessageBox.Show("OK");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            foreach (var way in bestWay)
            {
                if (way.priceValue > 0)
                {
                    AddItemToListBox(this.listBox1, $"物品名称：{way.name};起点:{way.sell.station_name};价格:{way.sell.price};数量:{way.sell.vol_remain};终点:{way.buy.station_name};价格:{way.buy.price};数量:{way.buy.vol_remain};差价{way.priceValue}");
                    AddItemToListBox(this.listBox1, "===================================================");
                }

            }
            MessageBox.Show("OK");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string tmp = this.txtEnd.Text;
            this.txtEnd.Text = this.txtStart.Text;
            this.txtStart.Text = tmp;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach(Control c in this.Controls)
            {
                if(c is TextBox && !c.Name.Equals("txtThreadNum"))
                {

                    ((TextBox)c).Text = "";
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            JArray array = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(EVEApi.getRoute("60013867", "30003830"));
            int i = array.Count;
        }
    }
    public class Way
    {
        public string name { get; set; }
        public OrderItem sell { get; set; }
        public OrderItem buy { get; set; }
        public decimal priceValue { get; set; }

        public int routeConut { get; set; }
       
    }
}
