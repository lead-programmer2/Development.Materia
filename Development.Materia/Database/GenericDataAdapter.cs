#region "imports"

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;

#endregion

namespace Development.Materia.Database
{
    /// <summary>
    /// Generic DataAdapter class for flexible data connection object.
    /// </summary>
    [ToolboxItem(false)]
    public class GenericDataAdapter : DbDataAdapter, IDbDataAdapter
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of GenericDataAdapter.
        /// </summary>
        public GenericDataAdapter()
        { }

        /// <summary>
        /// Creates a new instance of GenericDataAdapter.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Sql command statement</param>
        public GenericDataAdapter(string connectionstring, string sql)
        {
            OleDbConnection connection = (OleDbConnection) Database.CreateConnection(connectionstring);
            IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = Database.CommandTimeout;
            command.CommandType = CommandType.Text;
            SelectCommand = (DbCommand) command;
        }

        /// <summary>
        /// Creates a new instance of GenericDataAdapter.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Sql command statement</param>
        public GenericDataAdapter(IDbConnection connection, string sql)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = Database.CommandTimeout;
            command.CommandType = CommandType.Text;
            SelectCommand = (DbCommand) command;
        }

        /// <summary>
        /// Creates a new instance of GenericDataAdapter.
        /// </summary>
        /// <param name="command">SqlCommand object</param>
        public GenericDataAdapter(IDbCommand command)
        { SelectCommand = (DbCommand) command; }

        #endregion
    }
}
