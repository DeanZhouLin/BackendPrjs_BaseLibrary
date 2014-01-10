using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.BaseLibrary.DataAccess
{
    public class CustomDataCommand : DataCommand
    {
        public CustomDataCommand(string database, string commandText)
            : base(database)
        {
            Configuration.Command commandSetup = new Configuration.Command();
            commandSetup.CommandText = commandText;
            commandSetup.Database = database;
            DbCommand = DbServer.CreateCommand(commandSetup);
            
            CommandText = commandText;
        }
        public CommandType CommandType
        {
            get { return DbCommand.CommandType; }
            set { DbCommand.CommandType = value; }
        }

        #region add parameter

        public void AddInputParameter(string name, object value)
        {
            AddInputParameter(name, DbType.String, value);
        }
        public void AddInputParameter(string name, DbType dbType, object value)
        {
            AddCommandParameter(name, dbType, ParameterDirection.Input, 0, value);
        }
        public void AddCommandParameter(
            string name,
            DbType dbType,
            ParameterDirection direction,
            int size,
            object value)
        {
            DbCommand.Parameters.Add(DbServer.CreateParameter(name, dbType, direction, size, value));
        }

        #endregion

        public string CommandText
        {
            get { return DbCommand.CommandText; }
            set { DbCommand.CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return DbCommand.CommandTimeout; }
            set { DbCommand.CommandTimeout = value; }
        }
    }
}
