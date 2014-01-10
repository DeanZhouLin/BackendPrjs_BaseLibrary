using System;
using System.Data;

namespace Com.BaseLibrary.Entity
{
    public class DataMappingAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public DbType DbType { get; set; }

        public DataMappingAttribute()
        {

        }

        public DataMappingAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

        public DataMappingAttribute(string columnName, DbType dbType)
        {
            ColumnName = columnName;
            DbType = dbType;
        }
    }
}
