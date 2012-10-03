#region "imports"

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Development.Materia.Database
{

    /// <summary>
    /// MySQL query execution and backup functionalities.
    /// </summary>
    public abstract class MySql
    {

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static int _maxallowedpacket = 150;

        /// <summary>
        /// Gets or sets the globally assigned max allowed packet in (MB) for the connected MySQL database server. Default value is 150.
        /// </summary>
        public static int MaxAllowedPacket
        {
            get { return _maxallowedpacket; }
            set { _maxallowedpacket = value; }
        }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string _mysqldirectory = Application.StartupPath + "\\MySQL Apps";

        #endregion

        #region "methods"

        /// <summary>
        /// Creates a DSN connection using the specified database information.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <returns></returns>
        public static IDbConnection CreateDSN(string connectionstring)
        {
            string _server = connectionstring.ConnectionStringValue(ConnectionStringSection.Server);
            string _database = connectionstring.ConnectionStringValue(ConnectionStringSection.Database);
            string _uid=  connectionstring.ConnectionStringValue(ConnectionStringSection.UID);
            string _pwd =  connectionstring.ConnectionStringValue(ConnectionStringSection.PWD);
            string _port = connectionstring.ConnectionStringValue(ConnectionStringSection.Port);
            if (String.IsNullOrEmpty(_port)) _port = "3306";

            return CreateDSN(_server, _database, _uid, _pwd, VisualBasic.CInt(_port));
        }

        /// <summary>
        /// Creates a DSN connection using the specified database information.
        /// </summary>
        /// <param name="server">Server hostname or IP address</param>
        /// <param name="database">Database catalog name</param>
        /// <param name="uid">User Id</param>
        /// <param name="pwd">Password</param>
        /// <returns></returns>
        public static IDbConnection CreateDSN(string server, string database, string uid, string pwd)
        { return CreateDSN(server, database, uid, pwd, 3306);  }

        /// <summary>
        /// Creates a DSN connection using the specified database information.
        /// </summary>
        /// <param name="server">Server hostname or IP address</param>
        /// <param name="database">Database catalog name</param>
        /// <param name="uid">User Id</param>
        /// <param name="pwd">Password</param>
        /// <param name="port">Port</param>
        /// <returns></returns>
        public static IDbConnection CreateDSN(string server, string database, string uid, string pwd, int port)
        {
            IDbConnection _connection = null;

            RegistryKey _key = Registry.LocalMachine; 
            RegistryKey _software = _key.OpenSubKey("SOFTWARE", true); 
            RegistryKey _odbc = _software.OpenSubKey("ODBC", true); 
            RegistryKey _ini = _odbc.OpenSubKey("ODBC.INI", true); 
            RegistryKey _dsn = null;

            if (_ini.GetSubKeyNames().Contains(database)) _dsn = _ini.OpenSubKey(database, true);
            else _dsn = _ini.CreateSubKey(database);

            if (_dsn != null)
            {
                string[] _keys = _dsn.GetSubKeyNames();
                if (_keys.Contains("DARIVER")) _dsn.DeleteSubKey("DARIVER");
                _dsn.SetValue("SERVER", server);
                _dsn.SetValue("DATABASE", database);

                string _driverpath = "";

                if (!Materia.Is64BitApplication()) _driverpath = "C:\\Windows\\System32";
                else _driverpath = "C:\\Program Files\\MySQL\\Connector ODBC 3.51\\myodbc3.dll";

                _dsn.SetValue("Driver", _driverpath);
                _dsn.SetValue("DESCRIPTION", database.ToUpper() + " DSN");
                _dsn.SetValue("UID", uid);
                _dsn.SetValue("PWD", pwd);
                _dsn.SetValue("PORT", port.ToString());
            }
  
            RegistryKey _datasources = _ini.OpenSubKey("ODBC Data Sources", true);
            _datasources.SetValue(database, "MySQL ODBC 3.51 Driver");

            _connection = new OdbcConnection("DSN=" + database);
            
            return _connection;
        }

        /// <summary>
        /// Backup a MySql database using the specified database connection string into the specified file.
        /// </summary>
        /// <param name="connectionstring">MySql connection string</param>
        /// <param name="filename">Backup file path</param>
        /// <returns></returns>
        public static MySqlResult Dump(string connectionstring, string filename)
        { return Dump(connectionstring, filename, null); }

        /// <summary>
        /// Backup a MySql database using the specified database connection string into the specified file.
        /// </summary>
        /// <param name="connectionstring">MySql connection string</param>
        /// <param name="filename">Backup file path</param>
        /// <param name="parameters">MySql dump parameter</param>
        /// <returns></returns>
        public static MySqlResult Dump(string connectionstring, string filename, MySqlDumpParameterCollection parameters)
        {
            MySqlResult _result = null; ExtractResourceApplications();
            string _mysqldumppath = Application.StartupPath + "\\mysqldump.exe";

            if (File.Exists(_mysqldumppath))
            {
                string _batfilepath = Application.StartupPath + "\\dump.bat";
                string _server = connectionstring.ConnectionStringValue(ConnectionStringSection.Server);
                string _database = connectionstring.ConnectionStringValue(ConnectionStringSection.Database);
                string _uid = connectionstring.ConnectionStringValue(ConnectionStringSection.UID);
                string _pwd = connectionstring.ConnectionStringValue(ConnectionStringSection.PWD);
                string _parameters = "";

                if (parameters != null)
                {
                    foreach (string _parameter in parameters)
                    {
                        if (!String.IsNullOrEmpty(_parameter.RLTrim())) _parameters += (String.IsNullOrEmpty(_parameters.RLTrim()) ? "" : " ") + _parameter;
                    }
                }

                string _contents = "\"" + Application.StartupPath + "\\mysqldump\" --host=" + _server + " --user=" + _uid + " --password=" + _pwd + " " + _database + (String.IsNullOrEmpty(_parameters.RLTrim()) ? "" : " ") + _parameters + " --set-charset --default-character-set=utf8 > \"" + filename + "\"";
                FileInfo _batfile = Materia.WriteToFile(_batfilepath, _contents);

                if (_batfile != null)
                {
                    string _error = ""; Process _process = new Process();
                    _process.StartInfo.FileName = _batfilepath;
                    _process.StartInfo.CreateNoWindow = true; _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    _process.StartInfo.RedirectStandardError = true; _process.StartInfo.UseShellExecute = false;
                    _process.Start();

                    while (!_process.HasExited) Application.DoEvents();

                    if (_process.StandardError != null)
                    {
                        try { _error = _process.StandardError.ReadToEnd().Replace("The handle is invalid.\n", "").Replace("The handle is invalid.", "").RLTrim(); }
                        catch { _error = ""; }
                    }
                    else _error = "";

                    _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                    _result = new MySqlResult(filename, _error);

                    try { File.Delete(_batfilepath); }
                    catch { }
                }
                else _result = new MySqlResult("", "Can't create executable batch file into default directory : " + Application.StartupPath + ".");
            }
            else _result = new MySqlResult("", "Can't extract MySql application resources into default directory : " + Application.StartupPath + ".");
            
            RemoveResourceApplications();

            return _result;
        }

        /// <summary>
        /// Executes the specified sql commandtext using MySql application itself.
        /// </summary>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <param name="file">MySql dump file to where the sql statements resides</param>
        /// <returns></returns>
        public static MySqlResult Execute(string connectionstring, FileInfo file)
        { return Execute(connectionstring, file, null); }

        /// <summary>
        /// Executes the specified sql commandtext using MySql application itself.
        /// </summary>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <param name="file">MySql dump file to where the sql statements resides</param>
        /// <param name="parameters">Additional MySql parameters</param>
        /// <returns></returns>
        public static MySqlResult Execute(string connectionstring, FileInfo file, MySqlParameterCollection parameters)
        {
            MySqlResult _result = null;

            if (file != null)
            {
                string _sql = file.Read();

                if (!String.IsNullOrEmpty(_sql.RLTrim())) _result = Execute(connectionstring, _sql, parameters);
                else _result = new MySqlResult(file.FullName, "No sql statement has been red from the file.");
            }
            else _result = new MySqlResult("", "No file has been specified.");

            return _result;
        }

        /// <summary>
        /// Executes the specified sql commandtext using MySql application itself.
        /// </summary>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <param name="sql">Sql command statements</param>
        /// <returns></returns>
        public static MySqlResult Execute(string connectionstring, string sql)
        { return Execute(connectionstring, sql, null); }

        /// <summary>
        /// Executes the specified sql commandtext using MySql application itself.
        /// </summary>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <param name="sql">Sql command statements</param>
        /// <param name="parameters">Additional MySql parameters</param>
        /// <returns></returns>
        public static MySqlResult Execute(string connectionstring, string sql, MySqlParameterCollection parameters)
        {
            MySqlResult _result = null; ExtractResourceApplications();

            if (File.Exists(Application.StartupPath + "\\mysql.exe"))
            {
                string _filename = Application.StartupPath + "\\tempsql.sql";
                FileInfo _file = Materia.WriteToFile(_filename, sql);

                if (_file != null)
                {
                    string _batfilepath = Application.StartupPath + "\\execsql.bat";
                    string _server = connectionstring.ConnectionStringValue(ConnectionStringSection.Server);
                    string _database = connectionstring.ConnectionStringValue(ConnectionStringSection.Database);
                    string _uid = connectionstring.ConnectionStringValue(ConnectionStringSection.UID);
                    string _pwd = connectionstring.ConnectionStringValue(ConnectionStringSection.PWD);
                    string _parameters = "";

                    if (parameters != null)
                    {
                        foreach (string _parameter in parameters)
                        {
                            if (!String.IsNullOrEmpty(_parameter.RLTrim())) _parameters += (String.IsNullOrEmpty(_parameters.RLTrim()) ? "" : " ") + _parameter;
                        }
                    }

                    string _contents = "\"" + Application.StartupPath + "\\mysql\" -h " + _server + " -u " + _uid + " -p" + _pwd + " " + _database + (String.IsNullOrEmpty(_parameters.RLTrim()) ? "" : " ") + _parameters + " --max_allowed_packet=" + MaxAllowedPacket + "M --default-character-set=utf8 < \"" + _filename.RLTrim().Replace("\\", "/") + "\"";
                    FileInfo _batfile = Materia.WriteToFile(_batfilepath, _contents);

                    if (_batfile != null)
                    {
                        string _error = ""; Process _process = new Process();

                        IDbConnection _connection = Database.CreateConnection(connectionstring);
                        QueResult _qresult = Que.Execute(_connection, "SET GLOBAL max_allowed_packet=(1024 * 1024) * " + MaxAllowedPacket.ToString() + ";");
                        _qresult.Dispose(QueResultDisposition.WithAssociatedQue);

                        if (_connection != null)
                        {
                            if (_connection.State == ConnectionState.Open)
                            {
                                try { _connection.Close(); }
                                catch { }
                            }
                            _connection.Dispose(); _connection = null; Materia.RefreshAndManageCurrentProcess();
                        }

                        _process.StartInfo.FileName = _batfilepath;
                        _process.StartInfo.CreateNoWindow = true; _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        _process.StartInfo.RedirectStandardError = true; _process.StartInfo.UseShellExecute = false;
                        _process.Start();

                        while (!_process.HasExited) Application.DoEvents();

                        if (_process.StandardError != null)
                        {
                            try { _error = _process.StandardError.ReadToEnd().Replace("The handle is invalid.\n", "").Replace("The handle is invalid.", "").RLTrim(); }
                            catch { _error = ""; }
                        }
                        else _error = "";

                        _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                        _result = new MySqlResult(_filename, _error, sql);

                        try { File.Delete(_batfilepath); }
                        catch { }
                    }
                    else _result = new MySqlResult("", "Can't completely initialize database execution.");
                }
                else _result = new MySqlResult("", "Can't completely initialize sql statement.");
            }
            else _result = new MySqlResult("", "Can't extract MySql application resources into default directory : " + Application.StartupPath + ".");

            return _result;
        }

        private static void ExtractResourceApplications()
        {
            RemoveResourceApplications(); string _directory = _mysqldirectory;

            string[] _applications = new string[] { "mysql", "mysqldump" };

            foreach (string _application in _applications)
            {
                byte[] _app = null;

                switch (_application)
                {
                    case "mysql":
                        _app = Properties.Resources.MySql; break;
                    case "mysqldump":
                        _app = Properties.Resources.MySqlDump; break;
                    default: break;
                }

                if (_app != null)
                {
                    FileInfo _file = null;

                    try { _file = _app.ToFileObject("exe", _directory); }
                    catch { _file = null; }

                    if (_file != null)
                    {
                        try
                        { File.Copy(_file.FullName, Application.StartupPath + "\\"  + _application + ".exe"); }
                        catch { }
                    }
                }
            }

        }

        /// <summary>
        /// Returns the MySql dump parameter representation of the specified MySqlDumpParameters enumeration.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetParameterValue(MySqlDumpParameters parameter)
        {
            string _value = "";
            string _name = Enum.GetName(typeof(MySqlDumpParameters), parameter);
            char[] _chars = _name.ToCharArray();

            if (_chars.Length > 0)
            {
                _value += "-";
                foreach (char _char in _chars)
                {
                    if (Char.IsLetter(_char) &&
                        Char.IsUpper(_char)) _value += "-";
                    _value += _char.ToString().ToLower();
                }
            }

            return _value;
        }

        /// <summary>
        /// Returns the MySql dump parameter representation of the specified MySqlParameters enumeration.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetParameterValue(MySqlParameters parameter)
        {
            string _value = "";
            string _name = Enum.GetName(typeof(MySqlParameters), parameter);
            char[] _chars = _name.ToCharArray();

            if (_chars.Length > 0)
            {
                _value += "-";
                foreach (char _char in _chars)
                {
                    if (Char.IsLetter(_char) &&
                        Char.IsUpper(_char)) _value += "-";
                    _value += _char.ToString().ToLower();
                }
            }

            return _value;
        }

        /// <summary>
        /// Gets the list of tables in a certain database using the specified database connection string.
        /// </summary>
        /// <param name="connectionstring">Database connection string</param>
        /// <returns></returns>
        public static List<string> GetTables(string connectionstring)
        {
            IDbConnection _connection = Database.CreateConnection(connectionstring);
            List<string> _tables = GetTables(_connection);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); Materia.RefreshAndManageCurrentProcess();

            return _tables;
        }

        /// <summary>
        /// Gets the list of tables in a certain database using the specified database connection.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <returns></returns>
        public static List<string> GetTables(IDbConnection connection)
        {
            List<string> _tables = new List<string>();

            string _query = "SELECT\n" +
                            "`tables`.TABLE_NAME AS `Table`\n" +
                            "FROM\n" +
                            "information_schema.`TABLES` AS `tables`\n" +
                            "WHERE\n" +
                            "(`tables`.TABLE_SCHEMA = DATABASE()) AND\n" +
                            "(`tables`.TABLE_COMMENT NOT LIKE 'VIEW')\n" +
                            "ORDER BY\n" +
                            "`Table`";

            DataTable _table = null;
            _table = _table.LoadData(connection, _query);

            if (_table != null)
            {
                foreach (DataRow rw in _table.Rows)
                {
                    if (rw.RowState!= DataRowState.Deleted &&
                        rw.RowState != DataRowState.Detached) _tables.Add(rw["Table"].ToString());
                }

                _table.Dispose(); Materia.RefreshAndManageCurrentProcess();
            }

            return _tables;
        }

        private static void RemoveResourceApplications()
        {
            string _directory = _mysqldirectory;

            if (Directory.Exists(_directory))
            {
                try { Directory.Delete(_directory, true); }
                catch { }
            }

            string[] _applications = new string[] { "mysql.exe", "mysqldump.exe" };

            foreach (string _application in _applications)
            {
                string _path = Application.StartupPath + "\\" + _application;
                if (File.Exists(_path))
                {
                    try { File.Delete(_path); }
                    catch { }
                }
            }

            int _counter = 0;

            while (_counter < 30 &&
                  (Directory.Exists(_mysqldirectory))
            {
                try { Directory .Delete (_mysqldirectory, true); }
                catch {}
                Materia.RefreshAndManageCurrentProcess();
                System.Threading.Thread.Sleep (100); Application.DoEvents();
                _counter += 1;
            }

            Materia.RefreshAndManageCurrentProcess();
        }

        #endregion

    }

    /// <summary>
    /// Collection of MySql dump parameters.
    /// </summary>
    public class MySqlDumpParameterCollection : ParameterCollection
    {

        #region "methods"

        /// <summary>
        /// Adds a new parameter in the collection.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int Add(MySqlDumpParameters parameter)
        { return base.Add(MySql.GetParameterValue(parameter)); }

        /// <summary>
        /// Returns whether the specified parameter already exists in the collection or not.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Contains(MySqlDumpParameters parameter)
        { return base.Contains(MySql.GetParameterValue(parameter)); }

        /// <summary>
        /// Removes the specified parameter from the collection.
        /// </summary>
        /// <param name="parameter"></param>
        public void Remove(MySqlDumpParameters parameter)
        { base.Remove(MySql.GetParameterValue(parameter)); }

        #endregion

    }

    /// <summary>
    /// Collection of MySql application parameters.
    /// </summary>
    public class MySqlParameterCollection : ParameterCollection
    {

        #region "methods"

        /// <summary>
        /// Adds a new parameter in the collection.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int Add(MySqlParameters parameter)
        { return base.Add(MySql.GetParameterValue(parameter)); }

        /// <summary>
        /// Returns whether the specified parameter already exists in the collection or not.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Contains(MySqlParameters parameter)
        { return base.Contains(MySql.GetParameterValue(parameter)); }

        /// <summary>
        /// Removes the specified parameter from the collection.
        /// </summary>
        /// <param name="parameter"></param>
        public void Remove(MySqlParameters parameter)
        { base.Remove(MySql.GetParameterValue(parameter)); }

        #endregion

    }

    /// <summary>
    /// MySql database importation or exportation results.
    /// </summary>
    public class MySqlResult
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of MySqlResult.
        /// </summary>
        /// <param name="filename">Relative output or input file path</param>
        public MySqlResult(string filename) : this(filename, "")
        { }

        /// <summary>
        /// Creates a new instance of MySqlResult.
        /// </summary>
        /// <param name="filename">Relative output or input file path</param>
        /// <param name="error">Exception message encountered during exportation or importation</param>
        public MySqlResult(string filename, string error) : this(filename, error, "")
        { }

        /// <summary>
        /// Creates a new instance of MySqlResult.
        /// </summary>
        /// <param name="filename">Relative output or input file path</param>
        /// <param name="error">Exception message encountered during exportation or importation</param>
        /// <param name="sql">Executed sql statement</param>
        public MySqlResult(string filename, string error, string sql)
        {
            if (File.Exists(filename)) _relativefile = new FileInfo(filename);
            _error = error; _sqlstatement = sql; _succeeded = String.IsNullOrEmpty(error.RLTrim());
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _error = "";

        /// <summary>
        /// Gets the exception message encountered during data exportation or importation.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FileInfo _relativefile = null;

        /// <summary>
        /// Gets the exported or dump file's information.
        /// </summary>
        public FileInfo RelativeFile
        {
            get { return _relativefile; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _sqlstatement = "";

        /// <summary>
        /// Gets the executed sql statement.
        /// </summary>
        public string SqlStatement
        {
            get { return _sqlstatement; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _succeeded = false;

        /// <summary>
        /// Gets whether the data exportation or importation succeeds or not.
        /// </summary>
        public bool Succeeded
        {
            get { return _succeeded; }
        }

        #endregion

    }

    /// <summary>
    /// Collection of string parameters.
    /// </summary>
    public abstract class ParameterCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of ParameterCollection.
        /// </summary>
        public ParameterCollection()
        { }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets or sets the parameter string at the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get { return (string)List[index]; }
            set { List[index] = value; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new parameter in the collection.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int Add(string parameter)
        {
            if (List.Contains(parameter)) return GetParameterIndex(parameter);
            else return List.Add(parameter);
        }

        /// <summary>
        /// Returns whether the specified parameter string already exists in the collection or not.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Contains(string parameter)
        { return List.Contains(parameter); }

        private int GetParameterIndex(string parameter)
        {
            int _index = -1;

            for (int i = 0; i <= (List.Count - 1); i++)
            {
                string _parameter = (string)List[i];
                if (_parameter.RLTrim() == parameter.RLTrim())
                {
                    _index = i; break;
                }
            }

            return _index;
        }

        /// <summary>
        /// Removes the specified parameter string from the collection.
        /// </summary>
        /// <param name="parameter"></param>
        public void Remove(string parameter)
        {
            if (Contains(parameter)) List.Remove(parameter);
        }

        #endregion
    }
}
