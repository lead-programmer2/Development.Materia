#region "imports"

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
    /// Database command execution class.
    /// </summary>
    public class Que : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of Que.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="query">Database command statement</param>
        public Que(string connectionstring, string query) : this(connectionstring,query, CommandExecution.ExecuteNonQuery)
        { }

        /// <summary>
        /// Creates a new instance of Que.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="query">Database command statement</param>
        /// <param name="execution">Database command execution mode</param>
        public Que(string connectionstring, string query, CommandExecution execution)
        {
            _connection = Database.CreateConnection(connectionstring); ConnectionString = _connection.ConnectionString;
            CommandText = query; CommandExecution = execution;
        }

        /// <summary>
        /// Creates a new instance of Que.
        /// </summary>
        /// <param name="connection">Database connection object</param>
        /// <param name="query">Database command statement</param>
        public Que(IDbConnection connection, string query) : this(connection,query, CommandExecution.ExecuteNonQuery)
        { }

        /// <summary>
        /// Creates a new instance of Que.
        /// </summary>
        /// <param name="connection">Database connection object</param>
        /// <param name="query">Database command statement</param>
        /// <param name="execution">Database command execution mode</param>
        public Que(IDbConnection connection, string query, CommandExecution execution)
        {
            _connection = connection; ConnectionString = connection.ConnectionString;
            CommandText = query; CommandExecution = execution;
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandExecution _commandexecution = CommandExecution.ExecuteNonQuery;

        /// <summary>
        /// Gets or sets the command execution mode to be performed by the class.
        /// </summary>
        public CommandExecution CommandExecution
        {
            get { return _commandexecution; }
            set { _commandexecution = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _commandtext = "";

        /// <summary>
        /// Gets or sets the database command statement to be executed by the class.
        /// </summary>
        public string CommandText
        {
            get { return _commandtext; }
            set { _commandtext = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDbConnection _connection = null;

        /// <summary>
        /// Gets the database connection used by the class.
        /// </summary>
        public IDbConnection Connection
        {
            get { return _connection; }
            set
            {
                if (_connection != null &&
                    value !=null)
                {
                    if (_connection.State != ConnectionState.Closed) _connection.Close();
                    _connection.Dispose(); _connection = null; Materia.RefreshAndManageCurrentProcess();
                }

                _connection = value;
                if (value != null) ConnectionString = _connection.ConnectionString;
                else ConnectionString = "";
            }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _connectionstring = "";

        /// <summary>
        /// Gets or sets the SQL command statement to be executed by the class.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionstring; }
            set
            {
                if (_connection == null) _connection = new OleDbConnection();

                if (_connection.GetType().Name == typeof(OleDbConnection).Name)
                {
                    string _provider = value.ConnectionStringValue(ConnectionStringSection.Provider);
                    if (!String.IsNullOrEmpty(_provider.RLTrim()))
                    {
                        _connectionstring = value;
                        if (_connection.ConnectionString != _connectionstring)
                        {
                            bool _isopen = VisualBasic.CBool(_connection.State == ConnectionState.Open);
                            if (!_isopen)
                            {
                                if (!WithAllowedVariablesBlock(_connectionstring))
                                {
                                    _connectionstring += (_connectionstring.RLTrim().EndsWith(";") ? "" : ";");
                                    _connectionstring += "ALLOW USER VARIABLES=TRUE;";
                                }
                                _connection.ConnectionString = _connectionstring;
                            }
                        }
                    }
                    else
                    {
                        OleDbConnection tempconnetion = Database.CreateConnection(value);
                        _connection.ConnectionString = tempconnetion.ConnectionString;
                        if (tempconnetion.State == ConnectionState.Open) tempconnetion.Close();
                        tempconnetion.Dispose(); Materia.RefreshAndManageCurrentProcess();
                    }
                }
                else
                {
                    bool _isopen = VisualBasic.CBool(_connection.State == ConnectionState.Open);
                    if (!_isopen)
                    {
                        _connectionstring = value;
                        if (!WithAllowedVariablesBlock(_connectionstring))
                        {
                            _connectionstring += (_connectionstring.RLTrim().EndsWith(";") ? "" : ";");
                            _connectionstring += "ALLOW USER VARIABLES=TRUE;";
                        }
                        _connection.ConnectionString = _connectionstring;
                    }
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _delimiter = ";";

        /// <summary>
        /// Gets or sets the delimiter string that serves as separator from one command statement from the other.
        /// </summary>
        public static string Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Hashtable _delegatetable = new Hashtable();

        #endregion

        /// <summary>
        /// Executes the current sql command information associated with the current class asynchronously. Result can be get thru EndExecute function when the synchronization is completed.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult BeginExecute()
        {
            Func<QueResult> _delegate = new Func<QueResult>(Execute);
            IAsyncResult _result = _delegate.BeginInvoke(null, _delegate);
            if (!_delegatetable.ContainsKey(_result)) _delegatetable.Add(_result, _delegate);
            return _result;
        }

        #region "BeginExecution"

        /// <summary>
        /// Executes and generates result information using the specified database and command information. Result can be get thru EndExecute function when the synchronization is completed.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statements</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecution(string connectionstring, string sql)
        { return BeginExecution(connectionstring, sql, CommandExecution.ExecuteNonQuery); }

        /// <summary>
        /// Executes and generates result information using the specified database and command information. Result can be get thru EndExecute function when the synchronization is completed.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statements</param>
        /// <param name="execution">Database command execution mode</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecution(string connectionstring, string sql, CommandExecution execution)
        {
            OleDbConnection connection = Database.CreateConnection(connectionstring);
            return BeginExecution(connection, sql, execution);
        }

        /// <summary>
        /// Executes and generates result information using the specified database and command information. Result can be get thru EndExecute function when the synchronization is completed.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statements</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecution(IDbConnection connection, string sql)
        {  return BeginExecution(connection, sql, CommandExecution.ExecuteNonQuery); }

        /// <summary>
        /// Executes and generates result information using the specified database and command information. Result can be get thru EndExecute function when the synchronization is completed.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statements</param>
        /// <param name="execution">Database command execution mode</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecution(IDbConnection connection, string sql, CommandExecution execution)
        {
            Func<IDbConnection, string, CommandExecution, QueResult> _delegate = new Func<IDbConnection, string, CommandExecution, QueResult>(Execute);
            IAsyncResult _result = _delegate.BeginInvoke(connection, sql, execution, null, _delegate);
            if (!_delegatetable.ContainsKey(_result)) _delegatetable.Add(_result, _delegate);
            return _result;
        }

        #endregion

        /// <summary>
        /// Returns the QueResult object generated by a completed BeginExecute call. 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public QueResult EndExecute(IAsyncResult result)
        {
            QueResult _result = null;

            if (_delegatetable.ContainsKey(result))
            {
                Func<QueResult> _delegate = (Func<QueResult>)_delegatetable[result];

                if (_delegate != null)
                {
                    _result = _delegate.EndInvoke(result);
                    try { _delegate = null; }
                    catch { }
                }

                _delegatetable.Remove(result); Materia.RefreshAndManageCurrentProcess();
            }

            return _result;
        }

        /// <summary>
        /// Returns the QueResult object generated by a completed BeginExecution call. 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static QueResult EndExecution(IAsyncResult result)
        {
            QueResult _result = null;

            if (_delegatetable.ContainsKey(result))
            {
                Func<IDbConnection, string, CommandExecution, QueResult> _delegate = (Func<IDbConnection, string, CommandExecution, QueResult>)_delegatetable[result];

                if (_delegate != null)
                {
                    _result = _delegate.EndInvoke(result);
                    try { _delegate = null; }
                    catch { }
                }

                _delegatetable.Remove(result); Materia.RefreshAndManageCurrentProcess();
            }

            return _result;
        }

        #region "Execute"

        /// <summary>
        /// Executes the current sql command information associated with the current class.
        /// </summary>
        /// <returns></returns>
        public QueResult Execute()
        {
            QueResult _result = new QueResult(this); _result.Execute(); return _result;
        }

        /// <summary>
        /// Executes and generates result information using the specified database and command information.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statements</param>
        /// <returns></returns>
        public static QueResult Execute(string connectionstring, string sql)
        { return Execute(connectionstring, sql, CommandExecution.ExecuteNonQuery);  }

        /// <summary>
        /// Executes and generates result information using the specified database and command information.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statements</param>
        /// <param name="execution">Database command execution mode</param>
        /// <returns></returns>
        public static QueResult Execute(string connectionstring, string sql, CommandExecution execution)
        {
            OleDbConnection connection = Database.CreateConnection(connectionstring);
            return Execute(connection, sql, execution);
        }

        /// <summary>
        /// Executes and generates result information using the specified database and command information.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statements</param>
        /// <returns></returns>
        public static QueResult Execute(IDbConnection connection, string sql)
        { return Execute(connection, sql, CommandExecution.ExecuteNonQuery);  }

        /// <summary>
        /// Executes and generates result information using the specified database and command information.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statements</param>
        /// <param name="execution">Database command execution mode</param>
        /// <returns></returns>
        public static QueResult Execute(IDbConnection connection, string sql, CommandExecution execution)
        {
            Que _que = new Que(connection, sql, execution); return _que.Execute();
        }

        private static bool WithAllowedVariablesBlock(string dbconnectionstring)
        {
            string _pattern = "(A|a)(L|l)(L|l)(O|o)(W|w)[\\n\\r\\t ]+(U|u)(S|s)(E|e)(R|r)[\\n\\r\\t ]+(V|v)(A|a)(R|r)(I|i)(A|a)(B|b)(L|l)(E|e)(S|s)[\\n\\r\\t =]+";
            return Regex.IsMatch(dbconnectionstring, _pattern);
        }

        #endregion

        #region "GetValue"

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Sql command statement</param>
        /// <returns></returns>
        public static T GetValue<T>(string connectionstring, string sql)
        { return GetValue<T>(connectionstring, sql, default(T)); }

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Sql command statement</param>
        /// <param name="defaultvalue">Default value in case record retrieval fails or no record has been found</param>
        /// <returns></returns>
        public static T GetValue<T>(string connectionstring, string sql, T defaultvalue)
        {
            OleDbConnection _connection = Database.CreateConnection(connectionstring);
            T _value = GetValue<T>(_connection, sql, defaultvalue);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); Materia.RefreshAndManageCurrentProcess();

            return _value;
        }

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Sql command statement</param>
        /// <returns></returns>
        public static T GetValue<T>(IDbConnection connection, string sql)
        {
            T _defaultvalue = default(T);
            return GetValue<T>(connection, sql, _defaultvalue);
        }

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Sql command statement</param>
        /// <param name="defaultvalue">Default value in case record retrieval fails or no record has been found</param>
        /// <returns></returns>
        public static T GetValue<T>(IDbConnection connection, string sql, T defaultvalue)
        {
            T _value = defaultvalue;

            QueResult _result = Execute(connection, sql, CommandExecution.ExecuteNonQuery);
            if (_result != null)
            {
                if (_result.ResultSet != null)
                {
                    if (_result.ResultSet.Tables.Count > 0)
                    {
                        DataTable _table = _result.ResultSet.Tables[0];
                        if (_table.Rows.Count > 0)
                        {
                            try { _value = (T)_table.Rows[0][0]; }
                            catch { _value = defaultvalue; }
                        }
                    }
                }

                _result.Dispose();
            }
            Materia.RefreshAndManageCurrentProcess();

            return _value;
        }

        #endregion

        #region "IDisposable support"

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// Dispose off any resources used by the class to free up memory space.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Track whether Dispose has been called.
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /// <summary>
        /// Dispose off any resources used by the class to free up memory space.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == ConnectionState.Open)
                        {
                            try { _connection.Close(); }
                            catch { }
                        }

                        try { _connection.Dispose(); }
                        catch { }
                        finally { _connection = null; }
                    }

                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }

    /// <summary>
    /// Que execution result information.
    /// </summary>
    public class QueResult : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of QueResult.
        /// </summary>
        /// <param name="que"></param>
        public QueResult(Que que)
        { _que = que; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _error = "";

        /// <summary>
        /// Gets the error message trapped during the current associated Que object's command execution.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Que _que = null;

        /// <summary>
        /// Gets the current associated Que object to where the result came from.
        /// </summary>
        public Que Que
        {
            get { return _que; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataSet _resultset = null;

        /// <summary>
        /// Gets the lists of result tables produce by the associated Que object.
        /// </summary>
        public DataSet ResultSet
        {
            get { return _resultset; }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _rowsaffected = -1;

        /// <summary>
        /// Gets the number of the database rows affected by the command specified in the associated Que object.
        /// </summary>
        public int RowsAffected
        {
            get { return _rowsaffected; }
        }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDbTransaction _transaction = null;

        #endregion

        #region "methods"

        private void Clear()
        {
            if (_transaction != null) _transaction = null;

            if (_resultset != null)
            {
                try
                {
                    foreach (DataTable table in _resultset.Tables)
                    {
                        table.Dispose();
                    }

                    _resultset.Tables.Clear();
                }
                catch { }
                finally { _resultset = null; }

                Materia.RefreshAndManageCurrentProcess();
            }

            _error = ""; _rowsaffected = -1;
        }

        /// <summary>
        /// Executes the associated Que object's command statements to get the results.
        /// </summary>
        public void Execute()
        {
            Clear();

            if (_que != null)
            {
                if (_que.Connection != null)
                {
                    IDbConnection _connection = _que.Connection;
                    CommandParser _parser = new CommandParser(_que);

                    if (_parser.CommandStatements.Count > 0)
                    {
                        try
                        {
                            if (_connection.State == ConnectionState.Open) _connection.Close();
                            if (_connection.State == ConnectionState.Closed) _connection.Open();

                            _rowsaffected = 0;
                            string _gmaxpacketpattern = "(S|s)(E|e)(T|t)[\\n\\r\\t ]+((G|g)(L|l)(O|o)(B|b)(A|a)(L|l)[\\n\\r\\t ]+)?(M|m)(A|a)(X|x)_(A|a)(L|l)(L|l)(O|o)(W|w)(E|e)(D|d)_(P|p)(A|a)(C|c)(K|k)(E|e)(T|t)[\\n\\r\\t =]+";

                            foreach (string _sql in _parser.CommandStatements)
                            {
                                if (!Regex.IsMatch(_sql, _gmaxpacketpattern))
                                {
                                    if (_transaction == null) _transaction = _connection.BeginTransaction();
                                }

                                try
                                {
                                    IDbCommand _command = _connection.CreateCommand();
                                    _command.CommandText = _sql; _command.CommandTimeout = 0;
                                    _command.CommandType = CommandType.Text; _command.Transaction = _transaction;

                                    if (IsSelectStatement(_sql))
                                    {
                                        DataTable dt = null;
                                        GenericDataAdapter _adapter = new GenericDataAdapter(_command);

                                        try
                                        {
                                            dt = new DataTable();
                                            try
                                            {
                                                _adapter.FillSchema(dt, SchemaType.Mapped);
                                                string _tablename = dt.TableName;
                                                dt.TableName = TableNameFromCommandText(_sql, _tablename);
                                            }
                                            catch { }

                                            _adapter.Fill(dt); _rowsaffected += dt.Rows.Count; InitializeTable(dt);

                                            if (_resultset == null) _resultset = new DataSet();
                                            _resultset.Tables.Add(dt);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (_transaction != null)
                                            {
                                                try
                                                { _transaction.Rollback(); }
                                                catch { }
                                                finally { _transaction = null; }
                                            }

                                            if (dt != null) dt.Dispose();
                                            Clear(); _error = ex.Message; break;
                                        }
                                        finally
                                        {
                                            try { _adapter.Dispose(); }
                                            catch { }
                                            finally { Materia.RefreshAndManageCurrentProcess(); }
                                        }
                                    }
                                    else
                                    {
                                        if (IsStoredProcExecution(_sql))
                                        {
                                            if (_que.CommandExecution == CommandExecution.ExecuteNonQuery)
                                            {
                                                try { _rowsaffected += _command.ExecuteNonQuery(); }
                                                catch (Exception ex)
                                                {
                                                    if (_transaction != null)
                                                    {
                                                        try
                                                        { _transaction.Rollback(); }
                                                        catch { }
                                                        finally { _transaction = null; }
                                                    }

                                                    Clear(); _error = ex.Message; break;
                                                }
                                                finally
                                                {
                                                    try { _command.Dispose(); }
                                                    catch {}
                                                    finally { Materia.RefreshAndManageCurrentProcess();}
                                                }
                                            }
                                            else
                                            {
                                                DataTable dt = null;
                                                GenericDataAdapter _adapter = new GenericDataAdapter(_command);

                                                try
                                                {
                                                    dt = new DataTable();
                                                    try
                                                    {
                                                        _adapter.FillSchema(dt, SchemaType.Mapped);
                                                        string _tablename = dt.TableName;
                                                        dt.TableName = TableNameFromCommandText(_sql, _tablename);
                                                    }
                                                    catch { }

                                                    _adapter.Fill(dt); _rowsaffected += dt.Rows.Count; InitializeTable(dt);

                                                    if (_resultset == null) _resultset = new DataSet();
                                                    _resultset.Tables.Add(dt);
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (_transaction != null)
                                                    {
                                                        try
                                                        { _transaction.Rollback(); }
                                                        catch { }
                                                        finally { _transaction = null; }
                                                    }

                                                    if (dt != null) dt.Dispose();
                                                    Clear(); _error = ex.Message; break;
                                                }
                                                finally
                                                {
                                                    try { _adapter.Dispose(); }
                                                    catch { }
                                                    finally { Materia.RefreshAndManageCurrentProcess(); }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try { _rowsaffected += _command.ExecuteNonQuery(); }
                                            catch (Exception ex)
                                            {
                                                if (_transaction != null)
                                                {
                                                    try
                                                    { _transaction.Rollback(); }
                                                    catch { }
                                                    finally { _transaction = null; }
                                                }

                                                Clear(); _error = ex.Message; break;
                                            }
                                            finally
                                            {
                                                try { _command.Dispose(); }
                                                catch { }
                                                finally { Materia.RefreshAndManageCurrentProcess(); }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (_transaction != null)
                                    {
                                        try
                                        { _transaction.Rollback(); }
                                        catch { }
                                        finally { _transaction = null; }
                                    }

                                    Clear(); _error = ex.Message; break;
                                }
                            }

                            if (String.IsNullOrEmpty(_error.RLTrim()))
                            {
                                if (_que.CommandExecution == CommandExecution.ExecuteNonQuery)
                                {
                                    if (_transaction != null)
                                    {
                                        try
                                        { _transaction.Commit(); }
                                        catch  (Exception ex)
                                        {
                                            Clear(); _error = ex.Message;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { _error = ex.Message; }
                    }
                    else _error = "No command statement can be executed.";
                }
                else _error = "Database connection wasn't initialized yet.";
            }
            else _error = "No accessible Que object to be executed.";
        }

        private void InitializeTable(DataTable table)
        {
            if (table != null)
            {
                foreach (DataColumn col in table.Columns)
                {
                    col.ReadOnly = false;

                    if (col.Unique)
                    {
                        if (!col.AutoIncrement)
                        {
                            if (col.DataType.Name == typeof(string).Name ||
                                col.DataType.Name == typeof(String).Name) col.DefaultValue = "";
                            else if (col.DataType.Name == typeof(DateTime).Name) col.DefaultValue = VisualBasic.CDate("01/01/1900");
                            else if (col.DataType.Name == typeof(byte).Name ||
                                     col.DataType.Name == typeof(Byte).Name ||
                                     col.DataType.Name == typeof(decimal).Name ||
                                     col.DataType.Name == typeof(Decimal).Name ||
                                     col.DataType.Name == typeof(double).Name ||
                                     col.DataType.Name == typeof(Double).Name ||
                                     col.DataType.Name == typeof(float).Name ||
                                     col.DataType.Name == typeof(int).Name ||
                                     col.DataType.Name == typeof(Int16).Name ||
                                     col.DataType.Name == typeof(Int32).Name ||
                                     col.DataType.Name == typeof(Int64).Name ||
                                     col.DataType.Name == typeof(long).Name ||
                                     col.DataType.Name == typeof(sbyte).Name ||
                                     col.DataType.Name == typeof(SByte).Name ||
                                     col.DataType.Name == typeof(short).Name ||
                                     col.DataType.Name == typeof(Single).Name) col.DefaultValue = 0;
                            else if (col.DataType.Name == typeof(bool).Name ||
                                     col.DataType.Name == typeof(Boolean).Name) col.DefaultValue = false;
                        }
                    }
                    else
                    {
                        col.AllowDBNull = true;

                        if (col.DataType.Name == typeof(string).Name ||
                                col.DataType.Name == typeof(String).Name) col.DefaultValue = "";
                            else if (col.DataType.Name == typeof(DateTime).Name) col.DefaultValue = VisualBasic.CDate("01/01/1900");
                            else if (col.DataType.Name == typeof(byte).Name ||
                                     col.DataType.Name == typeof(Byte).Name ||
                                     col.DataType.Name == typeof(decimal).Name ||
                                     col.DataType.Name == typeof(Decimal).Name ||
                                     col.DataType.Name == typeof(double).Name ||
                                     col.DataType.Name == typeof(Double).Name ||
                                     col.DataType.Name == typeof(float).Name ||
                                     col.DataType.Name == typeof(int).Name ||
                                     col.DataType.Name == typeof(Int16).Name ||
                                     col.DataType.Name == typeof(Int32).Name ||
                                     col.DataType.Name == typeof(Int64).Name ||
                                     col.DataType.Name == typeof(long).Name ||
                                     col.DataType.Name == typeof(sbyte).Name ||
                                     col.DataType.Name == typeof(SByte).Name ||
                                     col.DataType.Name == typeof(short).Name ||
                                     col.DataType.Name == typeof(Single).Name) col.DefaultValue = 0;
                            else if (col.DataType.Name == typeof(bool).Name ||
                                     col.DataType.Name == typeof(Boolean).Name) col.DefaultValue = false;
                    }
                }
            }
        }

        private bool IsSelectStatement(string sql)
        {
            bool _isselect = false;
            List<string> _patterns = new List<string>();

            // SELECT (DISTINCT)
            _patterns.Add("^(S|s)(E|e)(L|l)(E|e)(C|c)(T|t)[\\n\\r\\t ]+((D|d)(I|i)(S|s)(T|t)(I|i)(N|n)(C|c)(T|t))?");
            
            // ANALYZE (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(A|a)(N|n)(A|a)(L|l)(Y|y)(Z|z)(E|e)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // CHECK TABLE
            _patterns.Add("^(C|c)(H|h)(E|e)(C|c)(K|k)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)");

            // OPTIMIZE (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(O|o)(P|p)(T|t)(I|i)(M|m)(I|i)(Z|z)(E|e)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // REPAIR (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(R|r)(E|e)(P|p)(A|a)(I|i)(R|r)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // DESCRIBE [tablename]
            _patterns.Add("^(D|d)(E|e)(S|s)(C|c)(R|r)(I|i)(B|b)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW (FULL) COLUMNS FROM [name]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((F|f)(U|u)(L|l)(L|l)[\\n\\r\\t ]+)?(C|c)(O|o)(L|l)(U|u)(M|n)(S|s)[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW CREATES (DATABASE) (FUNCTION) (PROCEDURE) (TABLE) [name]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(C|c)(R|r)(E|e)(A|a)(T|t)(E|e)[\\n\\r\\t ]+((D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e)[\\n\\r\\t ]+)?((F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+)?" +
                           "((P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+)?((T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+)?[a-zA-Z0-9`\\[\\]_\\.\\\\]+");

            // SHOW DATABASES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e)(S|s)[\\n\\r\\t ]+");

            // SHOW ENGINE [name]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(E|e)(N|n)(G|g)(I|i)(N|n)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.-]+");

            // SHOW (STORAGE) ENGINES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((S|s)(T|t)(O|o)(R|r)(A|a)(G|g)(E|e)[\\n\\r\\t ]+)?(E|e)(N|n)(G|g)(I|i)(N|n)(E|e)(S|s)");

            // SHOW ERRORS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(E|e)(R|r)(R|r)(O|o)(R|r)(S|s)");

            // SHOW FUNCTION CODE [functionname]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+(C|c)(O|o)(D|d)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW FUNCTION STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW INDEX FROM [name]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(I|i)(N|n)(D|d)(E|e)(X|x)[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW INNODB STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(I|i)(N|n)(N|n)(O|o)(D|d)(B|b)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW PROCEDURE CODE [procedurename]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+(C|c)(O|o)(D|d)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW PROCEDURE STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW OPEN TABLES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(O|o)(P|p)(E|e)(N|n)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)(S|s)");

            // SHOW PRIVILEGES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(I|i)(V|v)(I|i)(L|l)(E|e)(G|g)(E|e)(S|s)");

            // SHOW PROCESSLIST
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((F|f)(U|u)(L|l)(L|l)[\\n\\r\\t ]+)?(P|p)(R|r)(O|o)(C|c)(E|e)(S|s)(S|s)(L|l)(I|i)(S|s)(T|t)");

            // SHOW (PROFILE) (PROFILES)
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((P|p)(R|r)(O|o)(F|f)(I|i)(L|l)(E|e)(S|s)|(P|p)(R|r)(O|o)(F|f)(I|i)(L|l)(E|e))");

            // SHOW (GLOBAL) (SESSION) (STATUS) (VARIABLES) 
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((G|g)(L|l)(O|o)(B|b)(A|a)(L|l)[\\n\\r\\t ]+)?((S|s)(E|e)(S|s)(S|s)(I|i)(O|o)(N|n)[\\n\\r\\t ]+)?((S|s)(T|t)(A|a)(T|t)(U|u)(S|s)|(V|v)(A|a)(R|r)(I|i)(A|a)(B|b)(L|l)(E|e)(S|s))");

            // SHOW TABLE STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW (TABLES) (TRIGGERS) (WARNINGS)
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(((T|t)(A|a)(B|b)(L|l)(E|e)(S|s)|(T|t)(R|r)(I|i)(G|g)(G|g)(E|e)(R|r)(S|s))|(W|w)(A|a)(R|r)(N|n)(I|i)(N|n)(G|g)(S|s))");

            foreach (string pattern in _patterns)
            {
                _isselect = _isselect || Regex.IsMatch(sql, pattern);
                if (_isselect) break;
            }

            _patterns = null; Materia.RefreshAndManageCurrentProcess();

            return _isselect;
            
        }

        private bool IsStoredProcExecution(string sql)
        {
            return Regex.IsMatch(sql, "^(C|c)(A|a)(L|l)(L|l)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.\\(\\)]+");
        }

        private string TableNameFromCommandText(string sql, string currenttablename)
        {
            string _tablename = currenttablename;
            string _selectsqlpattern = "^(S|s)(E|e)(L|l)(E|e)(C|c)(T|t)[\\n\\r\\t ]+((D|d)(I|i)(S|s)(T|t)(I|i)(N|n)(C|c)(T|t))?";

            if (Regex.IsMatch(sql, _selectsqlpattern))
            {
                string _fromclausewoaliaspattern = "[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\._\\[\\]]+";
                string _fromclausewaliaspattern1 = "[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\._\\[\\]]+[\\n\\r\\t ]+[a-zA-Z0-9`]+";
                string _fromclausewaliaspattern2 = "[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\._\\[\\]]+[\\n\\r\\t ]+(A|a)(S|s)[\\n\\r\\t ]+[a-zA-Z0-9`]+";
                string _whereclausepattern = "[\\n\\r\\t ]+(W|w)(H|h)(E|e)(R|r)(E|e)";

                string _fromclause = "";

                MatchCollection _froms = Regex.Matches(sql, _fromclausewaliaspattern2);
                if (_froms.Count > 0) _fromclause = _froms[0].Value;
                else
                {
                    _froms = null; _froms = Regex.Matches(sql, _fromclausewaliaspattern1);
                    if (_froms.Count > 0)
                    {
                        string _rawfrom = _froms[0].Value;
                        MatchCollection _wheres = Regex.Matches(_rawfrom, _whereclausepattern);
                        if (_wheres.Count > 0)
                        {
                            foreach (Match _where in _wheres)
                            {
                                if (_rawfrom.RLTrim().EndsWith(_where.Value))
                                {
                                    _fromclause = Regex.Replace(_rawfrom, _whereclausepattern, ""); break;
                                }
                            }

                            _rawfrom = _fromclause;

                            MatchCollection _fromwoalias = Regex.Matches(_rawfrom, _fromclausewoaliaspattern);

                            foreach (Match _from in _fromwoalias)
                            {
                                if (_rawfrom.RLTrim() == _from.Value.RLTrim())
                                {
                                    _fromclause = ""; break;
                                }
                            }
                        }
                        else _fromclause = _rawfrom;
                    }
                }

                if (!String.IsNullOrEmpty(_fromclause.RLTrim()))
                {
                    string _rawfrom = "";
                    _froms = null; _froms = Regex.Matches(_fromclause, _fromclausewoaliaspattern);
                    if (_froms.Count > 0) _rawfrom = _froms[0].Value;

                    if (!String.IsNullOrEmpty(_rawfrom.RLTrim()))
                    {
                        string _frompattern = "[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+";
                        _tablename = Regex.Replace(_rawfrom, _frompattern, "").RLTrim();
                    }
                }
            }

            Materia.RefreshAndManageCurrentProcess();
            return _tablename.Replace("`","").Replace("[","").Replace("]", "");
        }

        #endregion

        #region "IDisposable support"

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// Dispose off any resources used by the class to free up memory space.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose off any resources used by the class to free up memory space.
        /// </summary>
        /// <param name="disposition">Disposition type</param>
        public void Dispose(QueResultDisposition disposition)
        {
            switch (disposition)
            {
                case QueResultDisposition.WithAssociatedQue:
                    if (_que != null) _que.Dispose();
                    break;
                default: break;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Track whether Dispose has been called.
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /// <summary>
        /// Dispose off any resources used by the class to free up memory space.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    Clear();
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        #endregion

    }

}
