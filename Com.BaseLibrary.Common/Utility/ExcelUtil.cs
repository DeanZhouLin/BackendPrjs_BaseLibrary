

using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Com.BaseLibrary.Entity;
using System.Data.OleDb;
using System.IO;
using org.in2bits.MyXls;

namespace Com.BaseLibrary.Utility
{
    /// 
    /// Excel 的摘要说明。
    /// 
    public class ExcelUtil
    {
        public ExcelUtil()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public class ExcelColumnInfo
        {
            public ExcelColumnInfo()
            {
            }
            public ExcelColumnInfo(string headerText, string mappingPropertyName)
            {
                HeaderText = headerText;
                MappingPropertyName = mappingPropertyName;
            }


            public string HeaderText { get; set; }
            public string MappingPropertyName { get; set; }
            public DataType DataType { get; set; }
            public int Width { get; set; }
            public string Format { get; set; }
        }
        public enum DataType
        {
            String,
            Date,
            DateTime,
            Int,
            Decimal,
        }

        public static void SaveToExcel<T>(HttpResponseBase resp, List<T> tList, List<ExcelColumnInfo> columns, string myExcelHeader, string myFileName)
        {
            //HttpResponse resp;
            //resp = myPage.Response;
            resp.Clear();
            resp.ContentEncoding = Encoding.GetEncoding("GB18030");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + myFileName + ".xls");
            resp.ContentType = "application/ms-excel";
            StringBuilder sb = GetExcelString<T>(tList, columns, myExcelHeader);
            resp.Write(sb.ToString());
            resp.End();
        }


        public static void SaveToExcel<T>(Page myPage, List<T> tList, List<ExcelColumnInfo> columns, string myExcelHeader, string myFileName)
        {
            HttpResponse resp;
            resp = myPage.Response;
            resp.Clear();
            resp.ContentEncoding = Encoding.GetEncoding("GB18030");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + myFileName + ".xls");
            resp.ContentType = "application/ms-excel";
            StringBuilder sb = GetExcelString<T>(tList, columns, myExcelHeader);
            resp.Write(sb.ToString());
            resp.End();
        }

        public static void WriteToExcel<T>(List<T> tList, List<ExcelColumnInfo> columns, string myExcelHeader, string myFileName, string path)
        {
            StringBuilder sb = GetExcelString<T>(tList, columns, myExcelHeader);
            sb.Replace("\"", "\\\"");
            string filePath = Path.Combine(path, myFileName);
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.GetEncoding("GB18030"));
            fs.SetLength(0);
            sw.Write(sb.ToString());
            sw.Close();
        }


        private static StringBuilder GetExcelString<T>(List<T> tList, List<ExcelColumnInfo> columns, string myExcelHeader)
        {
            //string colHeaders = "\t\t\t\t" + myExcelHeader + "\n\n\n";

            ////colHeaders += tableHeader(Tab) + "\n";
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(myExcelHeader))
            {
                sb.Append("" + myExcelHeader + "\n");
            }

            Dictionary<ExcelColumnInfo, PropertyInfo> columnpropertyList = GetColumnPropertyMappingList(columns, EntityTypeManager.GetPropertyList<T>());
            foreach (ExcelColumnInfo column in columnpropertyList.Keys)
            {
                sb.Append(column.HeaderText + "\t");
            }
            sb.Append("\n");
            foreach (T t in tList)
            {
                if (t == null)
                {
                    continue;
                }
                foreach (ExcelColumnInfo column in columnpropertyList.Keys)
                {
                    sb.Append(GetPropertyValue(column, columnpropertyList[column], t));
                    sb.Append("\t");
                }
                sb.Append("\n");
            }
            return sb;
        }

        private static object GetPropertyValue(ExcelColumnInfo column, PropertyInfo property, object obj)
        {
            object val = property.GetValue(obj, null);
            //
            return val;
        }

        private static Dictionary<ExcelColumnInfo, PropertyInfo> GetColumnPropertyMappingList(
            List<ExcelColumnInfo> columns,
            List<PropertyInfo> propertyList)
        {
            Dictionary<ExcelColumnInfo, PropertyInfo> dictList = new Dictionary<ExcelColumnInfo, PropertyInfo>();
            if (columns == null)
            {
                foreach (PropertyInfo property in propertyList)
                {
                    ExcelColumnInfo column = new ExcelColumnInfo { HeaderText = property.Name, MappingPropertyName = property.Name };
                    dictList.Add(column, property);
                }
            }
            else
            {
                foreach (ExcelColumnInfo column in columns)
                {
                    PropertyInfo property = propertyList.Find(c => c.Name == column.MappingPropertyName);
                    if (property != null)
                    {
                        dictList.Add(column, property);
                    }
                }
            }
            return dictList;

        }


        /*得到表单头子*/
        /*表单头子有TABLE组成，偶次项排列，TABLE在HTML中加 RUNAT=SERVER*/
        public string tableHeader(HtmlTable Tab)
        {
            int iCols = Tab.Rows[0].Cells.Count;
            int iRows = Tab.Rows.Count;
            string str = "";

            for (int row = 0; row < Tab.Rows.Count; row++)
            {
                for (int col = 0; col < Tab.Rows[row].Cells.Count; col++)
                {
                    if (col % 2 == 1)//取偶次项的控件数据（目前只有TextBox和DropDownList，没有包含LABEL)
                    {
                        try
                        {
                            if (Tab.Rows[row].Cells[col].Controls[0].ToString() == "System.Web.UI.LiteralControl")
                            {
                                if (Tab.Rows[row].Cells[col].Controls[1].ToString() == "System.Web.UI.WebControls.TextBox")
                                {
                                    str += ((System.Web.UI.WebControls.TextBox)(Tab.Rows[row].Cells[col].Controls[1])).Text + "\t";
                                }
                                if (Tab.Rows[row].Cells[col].Controls[1].ToString() == "System.Web.UI.WebControls.DropDownList")
                                {
                                    str += ((System.Web.UI.WebControls.ListControl)(((System.Web.UI.WebControls.DropDownList)((Tab.Rows[row].Cells[col].Controls[1]))))).SelectedValue + "\t";
                                }
                            }
                            else
                            {
                                if (Tab.Rows[row].Cells[col].Controls[0].ToString() == "System.Web.UI.WebControls.TextBox")
                                {
                                    str += ((System.Web.UI.WebControls.TextBox)(Tab.Rows[row].Cells[col].Controls[0])).Text + "\t";
                                }
                                if (Tab.Rows[row].Cells[col].Controls[0].ToString() == "System.Web.UI.WebControls.DropDownList")
                                {
                                    str += ((System.Web.UI.WebControls.ListControl)(((System.Web.UI.WebControls.DropDownList)((Tab.Rows[row].Cells[col].Controls[0]))))).SelectedValue + "\t";
                                }
                            }
                        }
                        catch
                        {
                            str += Tab.Rows[row].Cells[col].InnerHtml + "\t";
                        }
                        if ((col + 1) % iCols == 0)
                        {
                            str += "\n";
                        }
                    }
                    else
                    {
                        str += "\t" + Tab.Rows[row].Cells[col].InnerHtml + "\t";
                    }
                }
            }
            return (str);
        }


        const string oledb2007 = "Provider = Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\"";
        const string oledb2003 = "Provider = Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";


        /// <summary>
        /// 从Excel导入数据，返回定全部Sheet的数据
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllDataFromFile(string fileName, bool isGetOne = false)
        {
            DataSet myDS = new DataSet();
            string provider = fileName.EndsWith(".xlsx") ? oledb2007 : oledb2003;
            //数据库连接字符串
            string myConn = string.Format(provider, fileName);
            //查询字符串 
            //连接数据库操作 
            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(myConn))
                {
                    myConnection.Open();
                    DataTable dt = myConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    foreach (DataRow dr in dt.Rows)
                    {
                        string tableName = dr["TABLE_NAME"] == null ? string.Empty : dr["TABLE_NAME"].ToString();
                        if (StringUtil.IsNullOrEmpty(tableName))
                        {
                            continue;
                        }
                        //if (!tableName.EndsWith("$"))
                        //{

                        //    continue;
                        //}
                        string mySQLstr = string.Format("SELECT * FROM [{0}]", tableName);
                        //执行SQL语句操作 
                        OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(mySQLstr, myConnection);

                        myDataAdapter.Fill(myDS, tableName);

                        if (isGetOne)
                        {
                            break;
                        }
                    }
                }
                return myDS;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }

        /// <summary>
        /// 从Excel导入数据，并根据gridview的Clolumn中定义的映射关系创建一个List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gridview"></param>
        /// <returns></returns>
        public static List<T> GetEntityListFromFile<T>(string fileName, bool isGetOne = false)
            where T : class, new()
        {
            DataSet ds = GetAllDataFromFile(fileName, isGetOne);
            DataTable dt = ds.Tables[0];
            if (dt == null)
            {
                return null;
            }
            return EntityBuilder.BuildEntityList1<T>(dt);
        }

        /// <summary>
        /// 导出excel 
        /// </summary>
        /// <param name="myPage"></param>
        /// <param name="dt">datatable</param>
        /// <param name="myExcelHeader"></param>
        /// <param name="myFileName"></param>
        //public static void SaveToExcel(Page myPage, DataTable dt, string myExcelHeader, string myFileName)
        //{
        //	HttpResponse resp;
        //	resp = myPage.Response;
        //	resp.Clear();
        //	resp.ContentEncoding = Encoding.GetEncoding("GB18030");
        //	resp.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(myFileName) + ".xls");
        //	resp.ContentType = "application/ms-excel";
        //	StringBuilder sb = GetExcelDataTable(dt, myExcelHeader);
        //	resp.Write(sb.ToString());
        //	resp.End();
        //}

        public static void SaveToExcel(Page myPage, DataSet ds, string myExcelHeader, string excelName)
        {
            XlsDocument xls = new XlsDocument();//新建一个xls文档
            xls.FileName = excelName + ".xls";//设定文件名
            foreach (DataTable table in ds.Tables)
            {
                AddSheet(xls, table);
            }
            xls.Send();
        }
        public static void SaveToExcel(DataSet ds, string excelName)
        {
            XlsDocument xls = new XlsDocument();//新建一个xls文档
            xls.FileName = excelName + ".xls";//设定文件名
            foreach (DataTable table in ds.Tables)
            {
                AddSheet(xls, table);
            }
            xls.Send();
        }

        private static void AddSheet(XlsDocument xls, DataTable table)
        {
            string sheetName = table.TableName;
            Worksheet sheet = xls.Workbook.Worksheets.Add(sheetName);//填加名为"chc 实例"的sheet页
            Cells cells = sheet.Cells;
            int columnCount = table.Columns.Count;
            int rowCount = table.Rows.Count;
            XF commonXF = xls.NewXF();
            commonXF.UseBorder = true;
            commonXF.TopLineStyle = commonXF.LeftLineStyle = commonXF.RightLineStyle = commonXF.BottomLineStyle = 1;
            for (int i = 0; i < columnCount; i++)
            {
                cells.Add(1, i + 1, table.Columns[i].ColumnName, commonXF);
            }
            for (int i = 0; i < rowCount; i++)
            {

                for (int j = 0; j < columnCount; j++)
                {
                    object value = table.Rows[i][j];
                    if (value == null || value == DBNull.Value)
                    {
                        cells.Add(i + 2, j + 1, "", commonXF);
                        continue;
                    }
                    Cell cell = cells.Add(i + 2, j + 1, value, commonXF);

                }
            }
        }
        public static void SaveToExcel(Page myPage, DataTable dt, string myExcelHeader, string excelName)
        {
            XlsDocument xls = new XlsDocument();//新建一个xls文档
            xls.FileName = excelName + ".xls";//设定文件名
            AddSheet(xls, dt);
            xls.Send();
        }
        private static StringBuilder GetExcelDataTable(DataTable dt, string myExcelHeader)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(myExcelHeader))
            {
                sb.Append("" + myExcelHeader + "\n");
            }

            List<ExcelColumnInfo> columnpropertyList = GetColumnPropertyDataTable(dt);
            foreach (ExcelColumnInfo column in columnpropertyList)
            {
                sb.Append(column.HeaderText + "\t");
            }
            sb.Append("\n");
            foreach (DataRow dr in dt.Rows)
            {
                if (dt == null)
                {
                    continue;
                }
                foreach (DataColumn column in dt.Columns)
                {
                    sb.Append(dr[column].ToString());
                    sb.Append("\t");
                }
                sb.Append("\n");
            }
            return sb;
        }

        private static List<ExcelColumnInfo> GetColumnPropertyDataTable(DataTable dt)
        {
            List<ExcelColumnInfo> dictList = new List<ExcelColumnInfo>();
            foreach (DataColumn dataColumn in dt.Columns)
            {
                ExcelColumnInfo column = new ExcelColumnInfo { HeaderText = dataColumn.ColumnName, MappingPropertyName = dataColumn.ColumnName };
                dictList.Add(column);
            }

            return dictList;

        }
    }
}

