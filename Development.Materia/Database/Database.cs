﻿#region "imports"

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Development.Materia.Database
{
    /// <summary>
    /// Database-related class.
    /// </summary>
    public static class Database
    {

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _databasedriver = "{MySQL ODBC 3.51 Driver}";

        /// <summary>
        /// Gets or sets the globally used database driver for the Database class.
        /// </summary>
        public static string DatabaseDriver
        {
            get { return _databasedriver; }
            set { _databasedriver = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _databaseprovider = "MSDataShape.1";

        /// <summary>
        /// Gets or sets the globally used database provider for the Database class.
        /// </summary>
        public static string DatabaseProvider
        {
            get { return _databaseprovider; }
            set { _databaseprovider = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _defaultconnectionstring = "Provider={0};DRIVER={1};SERVER={2};DATABASE={3};UID={4};PWD={5};PORT={6};";

        /// <summary>
        /// Gets or sets the globally used default database connection string pattern.
        /// </summary>
        public static string DefaultConnectionString
        {
            get { return _defaultconnectionstring; }
            set { _defaultconnectionstring = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _lastinsertidcall = "CASE WHEN @lastid IS NULL THEN @lastid:=LAST_INSERT_ID() ELSE @lastid END";

        /// <summary>
        /// Gets or sets how the auto-incremented header primary key value is called using the current database.
        /// </summary>
        public static string LastInsertIdCall
        {
            get { return _lastinsertidcall; }
            set { _lastinsertidcall = value; }
        }

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Hashtable _delegatetable = new Hashtable();

        private struct InvokingDelegate
        {
            public bool InvokedThruConnectionString;
            public Delegate Invoker;
        }

        #region "BeginTryConnect"

        /// <summary>
        /// Returns a asynchronization of the CanConnect function. Result can then be returned by calling EndTryConnect after the asynchronization is completed.
        /// </summary>
        /// <param name="connectionstring">Database connection string.</param>
        /// <returns></returns>
        public static IAsyncResult BeginTryConnect(string connectionstring)
        {
            Func<string, bool> _delegate = new Func<string, bool>(CanConnect);
            InvokingDelegate _invoker = new InvokingDelegate();
            _invoker.InvokedThruConnectionString = true;
            _invoker.Invoker = _delegate;
            IAsyncResult _result = _delegate.BeginInvoke(connectionstring, null, _delegate);
            _delegatetable.Add(_result, _invoker);
            return _result;
        }

        /// <summary>
        /// Returns a asynchronization of the CanConnect function. Result can then be returned by calling EndTryConnect after the asynchronization is completed.
        /// </summary>
        /// <param name="connection">Database connection object.</param>
        /// <returns></returns>
        public static IAsyncResult BeginTryConnect(IDbConnection connection)
        {
            Func<IDbConnection, bool> _delegate = new Func<IDbConnection, bool>(CanConnect);
            InvokingDelegate _invoker = new InvokingDelegate();
            _invoker.InvokedThruConnectionString = false;
            _invoker.Invoker = _delegate;
            IAsyncResult _result = _delegate.BeginInvoke(connection, null, _delegate);
            _delegatetable.Add(_result, _invoker);
            return _result;
        }

        #endregion

        #region "CanConnect"

        /// <summary>
        /// Returns whether a database connection can be established using the specified database connection string
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <returns></returns>
        public static bool CanConnect(string connectionstring)
        {
            IDbConnection connection = CreateConnection(connectionstring);
            bool _connected = connection.CanConnect();
            if (connection.State != ConnectionState.Closed) connection.Close();
            connection.Dispose(); Materia.RefreshAndManageCurrentProcess();

            return _connected;
        }

        /// <summary>
        /// Returns whether a database connection can be established using the specified database connection object.
        /// </summary>
        /// <param name="connection">Database connection object.</param>
        /// <returns></returns>
        public static bool CanConnect(IDbConnection connection)
        {
            return connection.CanConnect();
        }

        #endregion

        /// <summary>
        /// Creates a generic database connection using the specified database connection string
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string connectionstring)
        {
            string _server = connectionstring.ConnectionStringValue(ConnectionStringSection.Server);
            string _database = connectionstring.ConnectionStringValue(ConnectionStringSection.Database);
            string _uid = connectionstring.ConnectionStringValue(ConnectionStringSection.UID);
            string _pwd = connectionstring.ConnectionStringValue(ConnectionStringSection.PWD);
            string _port = connectionstring.ConnectionStringValue(ConnectionStringSection.Port);
            if (String.IsNullOrEmpty(_port.RLTrim())) _port = "3306";

            string _connectionstring = String.Format(DefaultConnectionString, DatabaseProvider, DatabaseDriver, _server, _database, _uid, _pwd, _port);
            string _allowvariablepattern = "(A|a)(L|l)(L|l)(O|o)(W|w)[\\n\\r\\t ]+(U|u)(S|s)(E|e)(R|r)[\\n\\r\\t ]+(V|v)(A|a)(R|r)(I|i)(A|a)(B|b)(L|l)(E|e)(S|s)[\\n\\r\\t =]+";

            if (!Regex.IsMatch(_connectionstring, _allowvariablepattern))
            {
                if (!_connectionstring.RLTrim().EndsWith(";")) _connectionstring += ";";
                _connectionstring += "ALLOW USER VARIABLE=TRUE;";
            }

            if (!Materia.Is64BitApplication()) return new OleDbConnection(_connectionstring);
            else return MySql.CreateDSN(connectionstring);
        }

        /// <summary>
        /// Returns whether a database connection can be established using the specified database connection string thru the specified synchronization result.
        /// </summary>
        /// <param name="result">Synchronization result from invoked BeginTryConnect method</param>
        /// <returns></returns>
        public static bool EndTryConnect(IAsyncResult result)
        {
            bool _connected = false;
            
            if (_delegatetable.ContainsKey(result))
            {
                InvokingDelegate _invoker = (InvokingDelegate)_delegatetable[result];
                if (_invoker.InvokedThruConnectionString)
                {
                    Func<string, bool> _delegate = (Func<string, bool>)_invoker.Invoker;
                    _connected =  _delegate.EndInvoke(result);

                    try
                    { _delegate = null; }
                    catch { }
                }
                else
                {
                    Func<IDbConnection, bool> _delegate = (Func<IDbConnection, bool>)_invoker.Invoker;
                    _connected = _delegate.EndInvoke(result);

                    try
                    { _delegate = null; }
                    catch { }
                }

                _delegatetable.Remove(result);
            }

            return _connected;
        }

    }
}
