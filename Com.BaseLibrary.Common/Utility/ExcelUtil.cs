

using System;
using System.Data;
using System.Linq.Dynamic;
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
using Com.BaseLibrary.Contract;
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

    public enum KnownDataType
    {
        Int, Double, String, DateTime
    }

    /// <summary>
    /// 高度可扩展的excel导入 By Dean 20140320
    /// 应用场景：1 需要支持实体类属性和excel中列的一对多关系 如excel中的列名是JHB、聚好币、PV时候，均认为映射到实体中的JHB字段
    ///           2 需要支持实体类属性和excel中列的多对一关系 如excel中只包含“名称”列，有同时填充实体类的Code & Name属性时，可以将Code属性也对应到“名称”列，然后写一个转换函数，通过“名称”从DB中取出Code再映射
    /// 功    能：1 可以自动剔除全部为空的行
    ///           2 能够自定义地设置将excel中的列映射到实体类上的哪个字段，以及映射的方法
    ///           3 能够对excel中的数据的有效性进行自定义的检查
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class ImportUtil<Entity> where Entity : ErrorInfoBase, new()
    {

        public readonly List<Entity> ErrorList = new List<Entity>();

        //所有的列（key是实体类中对应列，value是excel中对应列 可以包含多个 使用|分割）
        /// <summary>
        /// 所有的列（key是实体类中对应列，value是excel中对应列 可以包含多个 使用|分割）
        /// </summary>
        private Dictionary<string, string> AllColumns { get; set; }

        //实际导入的excel中，实体类的列和excel中列的对应关系
        /// <summary>
        /// 实际导入的excel中，实体类的列和excel中列的对应关系
        /// </summary>
        private readonly Dictionary<string, string> CurrentHasAllColumns = new Dictionary<string, string>();

        //所有需要使用预定义转换函数将excel中列的值进行类型转换的列
        /// <summary>
        /// 所有需要使用预定义转换函数将excel中列的值进行类型转换的列
        /// 列为实体类中的列名
        /// </summary>
        private Dictionary<KnownDataType, List<string>> ConverterFields { get; set; }

        //所有需要进行类型转换的列
        /// <summary>
        /// 所有需要进行类型转换的列
        /// key 为实体类中的列名 value 为自定义转换函数（包含excel中对应列的值和操作后的返回值的Func）
        /// </summary>
        private readonly Dictionary<string, Func<object, object>> AllNeedConvertedFields = new Dictionary<string, Func<object, object>>();


        public ImportUtil(Dictionary<string, string> allColumns)
        {
            AllColumns = allColumns;
        }


        // 根据excel文件名获取实体类列表 《唯一向外部公开的接口》
        /// <summary>
        /// 根据excel文件名获取实体类列表 《唯一向外部公开的接口》
        /// </summary>
        /// <param name="fileName">excel文件名</param>
        /// <param name="errorInfo">导入过程中的任何错误信息都会放到这里面</param>
        /// <param name="converterFields">需要进行预定义类型转换的列</param>
        /// <param name="customConverts">需要进行自定义类型转换（key是实体类对应的列，value是包含excel中对应列的值和操作后的返回值的Func）</param>
        /// <param name="dataValidateChecks">在进行excel列映射到实体类之前，对excel列中的值进行的预检查</param>
        /// <returns>实体类列表</returns>
        public List<Entity> GetEntityList(string fileName, ErrorInfoBase errorInfo,
            Dictionary<KnownDataType, List<string>> converterFields = null,
            Dictionary<string, Func<object, object>> customConverts = null,
            Dictionary<string, Func<object, string>> dataValidateChecks = null)
        {

            ErrorList.Clear();
            CurrentHasAllColumns.Clear();
            AllNeedConvertedFields.Clear();

            //1 设置所有需要进行类型转换的列 《注意：自定义的转换函数会覆盖系统默认提供的转换函数》
            SetAllNeedConvertedFields(converterFields, customConverts);

            //2 获取excel导入的DataTable
            var dt = ExcelUtil.GetAllDataFromFile(fileName, true).Tables[0];

            //3 检查需要获取的列是否均存在于DataTable中
            CheckDataTableColumns(dt, errorInfo);

            //4 检查不通过
            if (errorInfo.HasError) return new List<Entity>();

            //5 移除DataTable中的空行，默认移除全部为空的行
            RemoveEmptyRows(dt);

            //6 根据DataTable生成实体类列表
            return GetEntityList(dt, dataValidateChecks, errorInfo);

        }

        //设置所有需要进行类型转换的列 《注意：自定义的转换函数会覆盖系统默认提供的转换函数》
        /// <summary>
        /// 设置所有需要进行类型转换的列 《注意：自定义的转换函数会覆盖系统默认提供的转换函数》
        /// </summary>
        /// <param name="converterFields"></param>
        /// <param name="customConverts"></param>
        private void SetAllNeedConvertedFields(Dictionary<KnownDataType, List<string>> converterFields = null, Dictionary<string, Func<object, object>> customConverts = null)
        {
            if (converterFields != null)
            {
                foreach (var converterField in converterFields)
                {
                    foreach (var field in converterField.Value)
                    {
                        AllNeedConvertedFields.AddItem(field, Converters[converterField.Key]);
                    }
                }
            }
            if (customConverts == null) return;
            foreach (var customConvert in customConverts)
            {
                AllNeedConvertedFields.AddItem(customConvert.Key, customConvert.Value);
            }
        }

        //检查需要获取的列是否均存在于DataTable中
        /// <summary>
        /// 检查需要获取的列是否均存在于DataTable中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private void CheckDataTableColumns(DataTable dt, ErrorInfoBase result)
        {
            foreach (var column in AllColumns)
            {
                var cs = column.Value.Split('|');
                bool has = false;
                foreach (var s in cs)
                {
                    if (!dt.Columns.Contains(s)) continue;
                    has = true;
                    CurrentHasAllColumns.Add(column.Key, s);
                    break;
                }
                if (!has)
                {
                    result.AddError("导入的excel中不存在列：" + cs);
                }
            }
        }

        //移除DataTable中的空行，默认移除全部为空的行
        /// <summary>
        /// 移除DataTable中的空行，默认移除全部为空的行
        /// </summary>
        /// <param name="dt"></param>
        private static void RemoveEmptyRows(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                bool isEmpty = true;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!isEmpty)
                    {
                        break;
                    }
                    if (row[col.ColumnName] == null || string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                    {
                        continue;
                    }
                    isEmpty = false;
                }
                if (isEmpty)
                {
                    row.Delete();
                }
            }
            dt.AcceptChanges();
        }

        //根据DataTable生成实体类列表
        /// <summary>
        /// 根据DataTable生成实体类列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataValidateChecks"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        private List<Entity> GetEntityList(DataTable dt, Dictionary<string, Func<object, string>> dataValidateChecks, ErrorInfoBase errorInfo)
        {
            if (dataValidateChecks == null)
            {
                dataValidateChecks = new Dictionary<string, Func<object, string>>();
            }

            //实例化返回实体
            var res = new List<Entity>();
            if (dt == null)
            {
                return res;
            }

            foreach (DataRow dataRow in dt.Rows)
            {

                Entity e = new Entity();
                foreach (var currentHasAllColumn in CurrentHasAllColumns)
                {
                    string entityPropName = currentHasAllColumn.Key;
                    string excelColName = currentHasAllColumn.Value;

                    var excelCloValue = dataRow[excelColName];

                    if (dataValidateChecks.ContainsKey(entityPropName))
                    {
                        string chkRes = dataValidateChecks[entityPropName](excelCloValue);
                        if (!string.IsNullOrEmpty(chkRes))
                        {
                            errorInfo.AddError(chkRes);
                            e.AddError(chkRes);
                        }
                    }

                    e.SetValue(entityPropName, AllNeedConvertedFields.ContainsKey(entityPropName) ?
                                               AllNeedConvertedFields[entityPropName](dataRow[excelColName])
                                               : StringConverter(dataRow[excelColName]));
                }

                if (!e.HasError)
                {
                    res.Add(e);
                }
                else
                {
                    ErrorList.Add(e);
                }

            }
            return res;

        }

        //自定义的转换函数( 目前支持Int，Double，DateTime，String类型的转换)
        /// <summary>
        /// 自定义的转换函数( 目前支持Int，Double，DateTime，String 类型的转换)
        /// </summary>
        private readonly Dictionary<KnownDataType, Func<object, object>> Converters = new Dictionary<KnownDataType, Func<object, object>>
        {
            {KnownDataType.Int,IntConverter},
            {KnownDataType.Double,DoubleConverter},
            {KnownDataType.DateTime,DateTimeConverter},
            {KnownDataType.String,StringConverter}
        };

        private static readonly Func<object, object> IntConverter = o =>
        {
            if (o == null)
            {
                return 0;
            }
            int t;
            int.TryParse(o.ToString(), out t);
            return t;
        };

        private static readonly Func<object, object> DoubleConverter = o =>
        {
            if (o == null)
            {
                return 0;
            }
            double t;
            double.TryParse(o.ToString(), out t);
            return t;
        };

        private static readonly Func<object, object> DateTimeConverter = o =>
        {
            if (o == null)
            {
                return DateTime.Parse("2000-01-01");
            }
            DateTime t;
            return DateTime.TryParse(o.ToString(), out t) ? t : DateTime.Parse("2000-01-01");
        };

        private static readonly Func<object, object> StringConverter = o => o == null ? string.Empty : o.ToString().Trim();

    }

}

