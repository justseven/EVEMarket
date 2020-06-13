using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web4BDC.Tools;

namespace EVEMarket.data
{
    public class DataLoad
    {
        static readonly string ModelFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}eve_data.xls";

        public static DataTable GetGoods()
        {
            DataTable dt = DataTableRenderToExcel.ExcelToDataTable(ModelFilePath, "物品列表",true);
            return dt;
        }

        public static DataTable GetXYLB()
        {
            DataTable dt = DataTableRenderToExcel.RenderDataTableFromExcel(ModelFilePath, "星域列表");
            return dt;
        }

        public static DataTable GetXZLB()
        {
            DataTable dt = DataTableRenderToExcel.RenderDataTableFromExcel(ModelFilePath, "星座列表");
            return dt;
        }
        public static DataTable GetXXLB()
        {
            DataTable dt = DataTableRenderToExcel.RenderDataTableFromExcel(ModelFilePath, "星系列表");
            return dt;
        }
        public static DataTable GetKJZ()
        {
            DataTable dt = DataTableRenderToExcel.RenderDataTableFromExcel(ModelFilePath, "NPC空間站");
            return dt;
        }

    }
}
