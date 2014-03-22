

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
    /// Excel ��ժҪ˵����
    /// 
    public class ExcelUtil
    {
        public ExcelUtil()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
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


        /*�õ���ͷ��*/
        /*��ͷ����TABLE��ɣ�ż�������У�TABLE��HTML�м� RUNAT=SERVER*/
        public string tableHeader(HtmlTable Tab)
        {
            int iCols = Tab.Rows[0].Cells.Count;
            int iRows = Tab.Rows.Count;
            string str = "";

            for (int row = 0; row < Tab.Rows.Count; row++)
            {
                for (int col = 0; col < Tab.Rows[row].Cells.Count; col++)
                {
                    if (col % 2 == 1)//ȡż����Ŀؼ����ݣ�Ŀǰֻ��TextBox��DropDownList��û�а���LABEL)
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
        /// ��Excel�������ݣ����ض�ȫ��Sheet������
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllDataFromFile(string fileName, bool isGetOne = false)
        {
            DataSet myDS = new DataSet();
            string provider = fileName.EndsWith(".xlsx") ? oledb2007 : oledb2003;
            //���ݿ������ַ���
            string myConn = string.Format(provider, fileName);
            //��ѯ�ַ��� 
            //�������ݿ���� 
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
                        //ִ��SQL������ 
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
        /// ��Excel�������ݣ�������gridview��Clolumn�ж����ӳ���ϵ����һ��List<T>
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
        /// ����excel 
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
            XlsDocument xls = new XlsDocument();//�½�һ��xls�ĵ�
            xls.FileName = excelName + ".xls";//�趨�ļ���
            foreach (DataTable table in ds.Tables)
            {
                AddSheet(xls, table);
            }
            xls.Send();
        }
        public static void SaveToExcel(DataSet ds, string excelName)
        {
            XlsDocument xls = new XlsDocument();//�½�һ��xls�ĵ�
            xls.FileName = excelName + ".xls";//�趨�ļ���
            foreach (DataTable table in ds.Tables)
            {
                AddSheet(xls, table);
            }
            xls.Send();
        }

        private static void AddSheet(XlsDocument xls, DataTable table)
        {
            string sheetName = table.TableName;
            Worksheet sheet = xls.Workbook.Worksheets.Add(sheetName);//�����Ϊ"chc ʵ��"��sheetҳ
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
            XlsDocument xls = new XlsDocument();//�½�һ��xls�ĵ�
            xls.FileName = excelName + ".xls";//�趨�ļ���
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
    /// �߶ȿ���չ��excel���� By Dean 20140320
    /// Ӧ�ó�����1 ��Ҫ֧��ʵ�������Ժ�excel���е�һ�Զ��ϵ ��excel�е�������JHB���ۺñҡ�PVʱ�򣬾���Ϊӳ�䵽ʵ���е�JHB�ֶ�
    ///           2 ��Ҫ֧��ʵ�������Ժ�excel���еĶ��һ��ϵ ��excel��ֻ���������ơ��У���ͬʱ���ʵ�����Code & Name����ʱ�����Խ�Code����Ҳ��Ӧ�������ơ��У�Ȼ��дһ��ת��������ͨ�������ơ���DB��ȡ��Code��ӳ��
    /// ��    �ܣ�1 �����Զ��޳�ȫ��Ϊ�յ���
    ///           2 �ܹ��Զ�������ý�excel�е���ӳ�䵽ʵ�����ϵ��ĸ��ֶΣ��Լ�ӳ��ķ���
    ///           3 �ܹ���excel�е����ݵ���Ч�Խ����Զ���ļ��
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class ImportUtil<Entity> where Entity : ErrorInfoBase, new()
    {

        public readonly List<Entity> ErrorList = new List<Entity>();

        //���е��У�key��ʵ�����ж�Ӧ�У�value��excel�ж�Ӧ�� ���԰������ ʹ��|�ָ
        /// <summary>
        /// ���е��У�key��ʵ�����ж�Ӧ�У�value��excel�ж�Ӧ�� ���԰������ ʹ��|�ָ
        /// </summary>
        private Dictionary<string, string> AllColumns { get; set; }

        //ʵ�ʵ����excel�У�ʵ������к�excel���еĶ�Ӧ��ϵ
        /// <summary>
        /// ʵ�ʵ����excel�У�ʵ������к�excel���еĶ�Ӧ��ϵ
        /// </summary>
        private readonly Dictionary<string, string> CurrentHasAllColumns = new Dictionary<string, string>();

        //������Ҫʹ��Ԥ����ת��������excel���е�ֵ��������ת������
        /// <summary>
        /// ������Ҫʹ��Ԥ����ת��������excel���е�ֵ��������ת������
        /// ��Ϊʵ�����е�����
        /// </summary>
        private Dictionary<KnownDataType, List<string>> ConverterFields { get; set; }

        //������Ҫ��������ת������
        /// <summary>
        /// ������Ҫ��������ת������
        /// key Ϊʵ�����е����� value Ϊ�Զ���ת������������excel�ж�Ӧ�е�ֵ�Ͳ�����ķ���ֵ��Func��
        /// </summary>
        private readonly Dictionary<string, Func<object, object>> AllNeedConvertedFields = new Dictionary<string, Func<object, object>>();


        public ImportUtil(Dictionary<string, string> allColumns)
        {
            AllColumns = allColumns;
        }


        // ����excel�ļ�����ȡʵ�����б� ��Ψһ���ⲿ�����Ľӿڡ�
        /// <summary>
        /// ����excel�ļ�����ȡʵ�����б� ��Ψһ���ⲿ�����Ľӿڡ�
        /// </summary>
        /// <param name="fileName">excel�ļ���</param>
        /// <param name="errorInfo">��������е��κδ�����Ϣ����ŵ�������</param>
        /// <param name="converterFields">��Ҫ����Ԥ��������ת������</param>
        /// <param name="customConverts">��Ҫ�����Զ�������ת����key��ʵ�����Ӧ���У�value�ǰ���excel�ж�Ӧ�е�ֵ�Ͳ�����ķ���ֵ��Func��</param>
        /// <param name="dataValidateChecks">�ڽ���excel��ӳ�䵽ʵ����֮ǰ����excel���е�ֵ���е�Ԥ���</param>
        /// <returns>ʵ�����б�</returns>
        public List<Entity> GetEntityList(string fileName, ErrorInfoBase errorInfo,
            Dictionary<KnownDataType, List<string>> converterFields = null,
            Dictionary<string, Func<object, object>> customConverts = null,
            Dictionary<string, Func<object, string>> dataValidateChecks = null)
        {

            ErrorList.Clear();
            CurrentHasAllColumns.Clear();
            AllNeedConvertedFields.Clear();

            //1 ����������Ҫ��������ת������ ��ע�⣺�Զ����ת�������Ḳ��ϵͳĬ���ṩ��ת��������
            SetAllNeedConvertedFields(converterFields, customConverts);

            //2 ��ȡexcel�����DataTable
            var dt = ExcelUtil.GetAllDataFromFile(fileName, true).Tables[0];

            //3 �����Ҫ��ȡ�����Ƿ��������DataTable��
            CheckDataTableColumns(dt, errorInfo);

            //4 ��鲻ͨ��
            if (errorInfo.HasError) return new List<Entity>();

            //5 �Ƴ�DataTable�еĿ��У�Ĭ���Ƴ�ȫ��Ϊ�յ���
            RemoveEmptyRows(dt);

            //6 ����DataTable����ʵ�����б�
            return GetEntityList(dt, dataValidateChecks, errorInfo);

        }

        //����������Ҫ��������ת������ ��ע�⣺�Զ����ת�������Ḳ��ϵͳĬ���ṩ��ת��������
        /// <summary>
        /// ����������Ҫ��������ת������ ��ע�⣺�Զ����ת�������Ḳ��ϵͳĬ���ṩ��ת��������
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

        //�����Ҫ��ȡ�����Ƿ��������DataTable��
        /// <summary>
        /// �����Ҫ��ȡ�����Ƿ��������DataTable��
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
                    result.AddError("�����excel�в������У�" + cs);
                }
            }
        }

        //�Ƴ�DataTable�еĿ��У�Ĭ���Ƴ�ȫ��Ϊ�յ���
        /// <summary>
        /// �Ƴ�DataTable�еĿ��У�Ĭ���Ƴ�ȫ��Ϊ�յ���
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

        //����DataTable����ʵ�����б�
        /// <summary>
        /// ����DataTable����ʵ�����б�
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

            //ʵ��������ʵ��
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

        //�Զ����ת������( Ŀǰ֧��Int��Double��DateTime��String���͵�ת��)
        /// <summary>
        /// �Զ����ת������( Ŀǰ֧��Int��Double��DateTime��String ���͵�ת��)
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

