#region "imports"

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Development.Materia
{

        #region "enumerations"

        /// <summary>
        /// Supported archiving tools.
        /// </summary>
        public enum ArchivingToolEnum
        {
            /// <summary>
            ///  7Zip archiving application.
            /// </summary>
            SevenZip = 0,
            /// <summary>
            /// WinRar archiving application.
            /// </summary>
            WinRar = 1
        }

        /// <summary>
        /// Archiving methods.
        /// </summary>
        public enum ArchivingMethodEnum
        {
            /// <summary>
            /// Insert : puts the whole specified file / directory in the archive file.
            /// </summary>
            Insert = 0,
            /// <summary>
            /// Append : puts a copy of specified file / directory inside the archive file.
            /// </summary>
            Append = 1
        }

        /// <summary>
        /// Database command execution enumerations.
        /// </summary>
        public enum CommandExecution
        {
            /// <summary>
            /// Executes database updating command statements.
            /// </summary>
            ExecuteNonQuery = 0,
            /// <summary>
            /// Executes record-retrieving command statements.
            /// </summary>
            ExecuteReader = 1
        }

        /// <summary>
        /// Common database connection string parameter values sections.
        /// </summary>
        public enum ConnectionStringSection
        {
            /// <summary>
            /// DRIVER part of a connection string.
            /// </summary>
            Driver = 0,
            /// <summary>
            /// SERVER part of a connection string.
            /// </summary>
            Server = 1,
            /// <summary>
            /// DATABASE part of a connection string.
            /// </summary>
            Database = 2,
            /// <summary>
            /// UID (User Id) part of a connection string.
            /// </summary>
            UID = 3,
            /// <summary>
            /// PWD (Password) part of a connection string.
            /// </summary>
            PWD = 4,
            /// <summary>
            /// PORT (Port Number) part of a connection string.
            /// </summary>
            Port = 5,
            /// <summary>
            /// Provider part of a connection string.
            /// </summary>
            Provider = 6,
            /// <summary>
            /// Nothing
            /// </summary>
            None = -1
        }

        /// <summary>
        /// Required field marking's position within the controls bounds.
        /// </summary>
        public enum IndicatorPositionEnum
        {
            /// <summary>
            /// Upper left corner of the control.
            /// </summary>
            LeftTop = 0,
            /// <summary>
            /// Upper right corner of the control.
            /// </summary>
            RigthTop = 1
        }

        /// <summary>
        /// MySql dump parameter enumerations.
        /// </summary>
        public enum MySqlDumpParameters
        {
            /// <summary>
            /// --add-drop-database
            /// </summary>
            AddDropDatabase = 0,
            /// <summary>
            /// --add-drop-table
            /// </summary>
            AddDropTable = 1,
            /// <summary>
            /// --add-locks
            /// </summary>
            AddLocks = 2,
            /// <summary>
            ///  --all-databases
            /// </summary>
            AllDatabases = 3,
            /// <summary>
            /// --allow-keywords
            /// </summary>
            AllowKeywords = 4,
            /// <summary>
            /// --comments
            /// </summary>
            Comments = 5,
            /// <summary>
            /// --compact
            /// </summary>
            Compact = 6,
            /// <summary>
            /// --compress
            /// </summary>
            Compress = 7,
            /// <summary>
            /// --complete-insert
            /// </summary>
            CompleteInsert = 8,
            /// <summary>
            /// --create-options
            /// </summary>
            CreateOptions = 9,
            /// <summary>
            /// --delayed-insert
            /// </summary>
            DelayedInsert = 10,
            /// <summary>
            /// --delete-master-logs
            /// </summary>
            DeleteMasterLogs = 11,
            /// <summary>
            /// --disable-keys
            /// </summary>
            DisableKeys = 12,
            /// <summary>
            /// --dump-date
            /// </summary>
            DumpDate = 13,
            /// <summary>
            /// --extended-insert
            /// </summary>
            ExtendedInsert = 14,
            /// <summary>
            /// --flush-logs
            /// </summary>
            FlushLogs = 15,
            /// <summary>
            /// --flush-privileges
            /// </summary>
            FlushPrivileges = 16,
            /// <summary>
            /// --force
            /// </summary>
            Force = 17,
            /// <summary>
            /// --hex-blob
            /// </summary>
            HexBlob = 18,
            /// <summary>
            /// --insert-ignore
            /// </summary>
            InsertIgnore = 19,
            /// <summary>
            /// --lock-all-tables
            /// </summary>
            LockAllTables = 20,
            /// <summary>
            /// --lock-tables
            /// </summary>
            LockTables = 21,
            /// <summary>
            /// --no-auto-commit
            /// </summary>
            NoAutoCommit = 22,
            /// <summary>
            /// --no-create-db
            /// </summary>
            NoCreateDb = 23,
            /// <summary>
            /// --no-create-info
            /// </summary>
            NoCreateInfo = 24,
            /// <summary>
            /// --no-data
            /// </summary>
            NoData = 25,
            /// <summary>
            /// --no-set-names
            /// </summary>
            NoSetNames = 26,
            /// <summary>
            /// --opt
            /// </summary>
            Opt = 27,
            /// <summary>
            /// -order-by-primary
            /// </summary>
            OrderByPrimary = 28,
            /// <summary>
            /// --quick
            /// </summary>
            Quick = 29,
            /// <summary>
            /// --quote-names
            /// </summary>
            QuoteNames = 30,
            /// <summary>
            /// --routines
            /// </summary>
            Routines = 31,
            /// <summary>
            /// --set-charset
            /// </summary>
            SetCharset = 32,
            /// <summary>
            /// --single-transaction
            /// </summary>
            SingleTransaction = 33,
            /// <summary>
            /// --skip-add-drop-tables
            /// </summary>
            SkipAddDropTables = 34,
            /// <summary>
            /// --skip-add-locks
            /// </summary>
            SkipAddLocks = 35,
            /// <summary>
            /// --skip-comments
            /// </summary>
            SkipComments = 36,
            /// <summary>
            /// --skip-disable-keys
            /// </summary>
            SkipDisableKeys = 37,
            /// <summary>
            /// --skip-dump-date
            /// </summary>
            SkipDumpDate = 38,
            /// <summary>
            /// --skip-opt
            /// </summary>
            SkipOpt = 39,
            /// <summary>
            /// --skip-set-charset
            /// </summary>
            SkipSetCharset = 40,
            /// <summary>
            /// --skip-triggers
            /// </summary>
            SkipTriggers = 41,
            /// <summary>
            /// --skip-tz-utc
            /// </summary>
            SkipTzUtc = 42,
            /// <summary>
            /// --tables
            /// </summary>
            Tables = 43,
            /// <summary>
            /// --triggers
            /// </summary>
            Triggers = 44,
            /// <summary>
            /// --tz-utc
            /// </summary>
            TzUtc = 45,
            /// <summary>
            /// --verbose
            /// </summary>
            Verbose = 46,
            /// <summary>
            /// --version
            /// </summary>
            Version = 47
        }

        /// <summary>
        /// MySql application parameter enumerations.
        /// </summary>
        public enum MySqlParameters
        {
            /// <summary>
            /// --auto-rehash
            /// </summary>
            AutoRehash = 1,
            /// <summary>
            /// --batch
            /// </summary>
            Batch = 2,
            /// <summary>
            /// --column-names
            /// </summary>
            ColumnNames = 3,
            /// <summary>
            /// --comments
            /// </summary>
            Comments = 4,
            /// <summary>
            /// --compress
            /// </summary>
            Compress = 5,
            /// <summary>
            /// --debug-info
            /// </summary>
            DebugInfo = 6,
            /// <summary>
            /// --force
            /// </summary>
            Force = 7,
            /// <summary>
            /// --help
            /// </summary>
            Help = 8,
            /// <summary>
            /// --html
            /// </summary>
            Html = 9,
            /// <summary>
            /// --ignore-spaces
            /// </summary>
            IgnoreSpaces = 10,
            /// <summary>
            /// --line-numbers
            /// </summary>
            LineNumbers = 11,
            /// <summary>
            /// --named-commands
            /// </summary>
            NamedCommands = 12,
            /// <summary>
            /// --no-auto-rehash
            /// </summary>
            NoAutoRehash = 13,
            /// <summary>
            /// --no-beep
            /// </summary>
            NoBeep = 14,
            /// <summary>
            /// --no-named-commands
            /// </summary>
            NoNamedCommands = 15,
            /// <summary>
            /// --no-pager
            /// </summary>
            NoPager = 16,
            /// <summary>
            /// --no-tee
            /// </summary>
            NoTee = 17,
            /// <summary>
            /// --one-database
            /// </summary>
            OneDatabase = 18,
            /// <summary>
            /// --quick
            /// </summary>
            Quick = 19,
            /// <summary>
            /// --raw
            /// </summary>
            Raw = 20,
            /// <summary>
            /// --reconnect
            /// </summary>
            Reconnect = 21,
            /// <summary>
            /// --safe-updates
            /// </summary>
            SafeUpdates = 22,
            /// <summary>
            /// --secure-auth
            /// </summary>
            SecureAuth = 23,
            /// <summary>
            /// --show-warnings
            /// </summary>
            ShowWarnings = 24,
            /// <summary>
            /// -sigint-ignore
            /// </summary>
            SigintIgnore = 25,
            /// <summary>
            /// --silent
            /// </summary>
            Silent = 26,
            /// <summary>
            /// --skip-column-names
            /// </summary>
            SkipColumnNames = 27,
            /// <summary>
            /// --skip-line-numbers
            /// </summary>
            SkipLineNumbers = 28,
            /// <summary>
            /// --skip-pager
            /// </summary>
            SkipPager = 29,
            /// <summary>
            /// --table
            /// </summary>
            Table = 30,
            /// <summary>
            /// --unbuffered
            /// </summary>
            Unbuffered = 31,
            /// <summary>
            /// --verbose
            /// </summary>
            Verbose = 32,
            /// <summary>
            /// --version
            /// </summary>
            Version = 33,
            /// <summary>
            /// --vertical
            /// </summary>
            Vertical = 34,
            /// <summary>
            /// --wait
            /// </summary>
            Wait = 35
        }

        /// <summary>
        /// Que result disposition enumerations.
        /// </summary>
        public enum QueResultDisposition
        {
            /// <summary>
            /// Dispose the result only. 
            /// </summary>
            ResultOnly = 0,
            /// <summary>
            /// Dispose associated Que object also.
            /// </summary>
            WithAssociatedQue = 1
        }

        #endregion

    public static partial class Materia
    {

        #region "properties"

        private static Image _cleartextimage = Properties.Resources.ClearTextImage;

        /// <summary>
        /// Gets or sets the globally used clear search box image.
        /// </summary>
        public static Image ClearTextImage
        {
            get { return _cleartextimage; }
            set { _cleartextimage = value; }
        }

        /// <summary>
        /// Gets the list of countries and loads it in a System.Data.DataTable.
        /// </summary>
        public static DataTable CountryTable
        {
            get
            {
                DataTable _table = new DataTable();
                _table.Columns.Clear(); _table.Rows.Clear();
                DataColumn _countrycol =  _table.Columns.Add("Country", typeof(string));
                _countrycol.AllowDBNull = true;

                CultureInfo[] _cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

                foreach (CultureInfo _culture in _cultures)
                {
                    try
                    {
                        RegionInfo _region = new RegionInfo(_culture.Name);
                        if (_table.Select("[Country] LIKE '" + _region.EnglishName.ToSqlValidString(true) + "'").Length <= 0) _table.Rows.Add(_region.EnglishName);
                    }
                    catch { }
                }

                if (_table.Select("[Country] LIKE 'Afghanistan'").Length <= 0) _table.Rows.Add("Afghanistan");
                _table.DefaultView.Sort = "[Country]";

                return _table;
            }
        }

        /// <summary>
        /// Gets the conventional elipsis '.' character usually used as password display replacement.
        /// </summary>
        public static char PasswordCharacter
        {
            get { return Convert.ToChar((int)(8226 & 0xffff)); }
        }

        /// <summary>
        /// Gets the list of global time zonez and loads it in a System.Data.DataTable.
        /// </summary>
        /// <returns></returns>
        public static DataTable TimeZoneTable
        {
            get
            {
                DataTable _timezonetable = new DataTable();
                _timezonetable.Columns.Add("Time Zone", typeof(string));
                _timezonetable.Columns.Add("Std Zone", typeof(string));
                _timezonetable.Columns.Add("Offset", typeof(decimal));
                _timezonetable.TableName = "TimeZones";

                try
                {
                    RegistryKey _timeinfokey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones");
                    foreach (string timeinfo in _timeinfokey.GetSubKeyNames())
                    {
                        RegistryKey _timezonekey = _timeinfokey.OpenSubKey(timeinfo);
                        object _display = null;

                        try { _display = _timezonekey.GetValue("Display"); }
                        catch { _display = null; }

                        if (_display != null)
                        {
                            object[] _values = new object[_timezonetable.Columns.Count];
                            _values[0] = _display.ToString();
                            _values[1] = timeinfo;

                            MatchCollection _matches = Regex.Matches(_display.ToString(), "(U|u)(T|t)(C|c)[\\+\\-]+[0-9\\:]+");
                            if (_matches.Count > 0)
                            {
                                string _stdoffset = _matches[0].Value;
                                string _numericoffset = Regex.Replace(_stdoffset, "(U|u)(T|t)(C|c)", "");
                                string[] _offsets = _numericoffset.Split(new char[] { ':' });
                                if (_offsets.Length >= 2)
                                {
                                    decimal _offset = VisualBasic.CDec(_offsets[0]);
                                    decimal _decimal = VisualBasic.CDec(_offsets[1]) / 60;
                                    if (_decimal > 0)
                                    {
                                        if (_offset < 0) _offset += VisualBasic.CDec(_decimal * -1);
                                        else _offset += _decimal;
                                    }
                                    _values[2] = _offset;
                                }
                            }
                            else _values[2] = 0;

                            _timezonetable.Rows.Add(_values);
                        }
                    }
                }
                catch { }

                if (_timezonetable.Rows.Count > 0) _timezonetable.DefaultView.Sort = "[Offset], [Std Zone]";

                return _timezonetable;
            }
        }

        #endregion

        #region "Between"

        /// <summary>
        /// Returns whether the current character is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this char value, char from, char to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this byte value, byte from, byte to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current date is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this DateTime value, DateTime from, DateTime to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this decimal value, decimal from, decimal to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this double value, double from, double to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this float value, float from, float to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this int value, int from, int to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this long value, long from, long to)
        {
            return (bool)(value >= from && value <= to);
        }

        /// <summary>
        /// Returns whether the current numeric value is within the specified scope of ranges.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="from">Starting range</param>
        /// <param name="to">Ending range</param>
        /// <returns></returns>
        public static bool Between(this short value, short from, short to)
        {
            return (bool)(value >= from && value <= to);
        }

        #endregion

        /// <summary>
        /// Test whether the specified database connection can establish a database connection or not.
        /// </summary>
        /// <param name="connection">Database connection object</param>
        /// <returns></returns>
        public static bool CanConnect(this IDbConnection connection)
        {
            bool _canconnect = false;

            if (connection != null)
            {
                bool _isopen = (bool)(connection.State == ConnectionState.Open);

                try
                {
                    if (connection.State != ConnectionState.Open) connection.Open();
                    _canconnect = true;
                }
                catch {  _canconnect = false; }
                finally
                {
                    if (_isopen)
                    {
                        try
                        {
                            if (connection.State != ConnectionState.Open) connection.Open();
                        }
                        catch { }
                    }
                    else
                    {
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }

            return _canconnect;
        }

        #region "ConnectionStringValue"

        /// <summary>
        /// Returns the connection string assigned value from the specified connection string section.
        /// </summary>
        /// <param name="connectionstring">Database connection string.</param>
        /// <param name="section">Database connection string section.</param>
        /// <returns></returns>
        public static string ConnectionStringValue(this string connectionstring, ConnectionStringSection section)
        {
            string _section = Enum.GetName(typeof(ConnectionStringSection), section);
            return connectionstring.ConnectionStringValue(_section);
        }

        /// <summary>
        /// Returns the connection string assigned value from the specified connection string section.
        /// </summary>
        /// <param name="connectionstring">Database connection string.</param>
        /// <param name="section">Database connection string section.</param>
        /// <returns></returns>
        public static string ConnectionStringValue(this string connectionstring, string section)
        {
            string _value = "";
            string _pattern = "";
            char[] _chars = section.ToCharArray();

            foreach (char c in _chars)
            {
                if (Char.IsLetter(c)) _pattern += "(" + c.ToString().ToUpper() + "|" + c.ToString().ToLower() + ")";
                else _pattern += c.ToString();
            }

            _pattern += "[\\n\\r\\t =]+[a-zA-Z0-9-_!@\\. ]+(;)";

             MatchCollection _matches  = Regex.Matches(connectionstring, _pattern);
            if (_matches.Count > 0) 
            {
                string _section  = _matches[0].Value;
                _pattern = "";
                
                foreach (char c in _chars)
                {
                    if (Char.IsLetter(c)) _pattern += "(" + c.ToString().ToUpper() + "|" + c.ToString().ToLower() + ")";
                    else _pattern += c.ToString();
                }
                
                _pattern += "[\\n\\r\\t =]+";
                string _result = Regex.Replace(_section, _pattern, "");
                if (_result.RLTrim().EndsWith(";"))  _value = _result.Substring(0, _result.Length - 1);
                else  _value = _result;
            }
            
            return _value.Replace(";", "");
        }

        #endregion

        #region "Decrypt"

        /// <summary>
        /// Returns a decrypted string value of the specified string using the specified encryption key as its pattern.
        /// </summary>
        /// <param name="value">Value to decrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns></returns>
        public static string Decrypt(this string value, string key)
        {
            return value.Decrypt(key, false);
        }

        /// <summary>
        /// Returns a decrypted string value of the specified string using the specified encryption key as its pattern.
        /// </summary>
        /// <param name="value">Value to decrypt</param>
        /// <param name="key">Encryption key</param>
        /// <param name="usesimpledecryption">Use simple decryption or not</param>
        /// <returns></returns>
        public static string Decrypt(this string value, string key, bool usesimpledecryption)
        {
            return Cryptography.Cryptographer.Decrypt(value, key, usesimpledecryption);
        }

        /// <summary>
        /// Returns the decrypted contents of the file using the supplied encryption key.
        /// </summary>
        /// <param name="file">File to decrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns></returns>
        public static string Decrypt(this FileInfo file, string key)
        {
            return file.Decrypt(key, false);
        }

        /// <summary>
        /// Returns the decrypted contents of the file using the supplied encryption key.
        /// </summary>
        /// <param name="file">File to decrypt</param>
        /// <param name="key">Encryption key</param>
        /// <param name="usesimpledecryption">Use simple encryption or not</param>
        /// <returns></returns>
        public static string Decrypt(this FileInfo file, string key, bool usesimpledecryption)
        {
            StringBuilder _decrypted = new StringBuilder();

            if (file != null)
            {
                if (file.Exists)
                {
                    try
                    {
                        String _contents = ReadFile(file);
                        _decrypted.Append(_contents.Decrypt(key, usesimpledecryption));
                    }
                    catch { }
                }
            }

            return _decrypted.ToString();
        }

        #endregion

        #region "Encrypt"

        /// <summary>
        /// Returns whether the specified file's contents has been encrypted using the supplied encryption key pattern.
        /// </summary>
        /// <param name="file">File to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns></returns>
        public static bool Encrypt(this FileInfo file, string key)
        {
            return file.Encrypt(key, false);
        }

        /// <summary>
        /// Returns whether the specified file's contents has been encrypted using the supplied encryption key pattern.
        /// </summary>
        /// <param name="file">File to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <param name="usesimpleencryption">Use simple decryption or not</param>
        /// <returns></returns>
        public static bool Encrypt(this FileInfo file, string key, bool usesimpleencryption)
        {
            bool _encrypted = false;

            if (file != null)
            {
                if (file.Exists)
                {
                    try
                    {
                        string _contents = ReadFile(file);
                        if (!String.IsNullOrEmpty(_contents.RLTrim())) WriteToFile(file.FullName, _contents.Encrypt(key, usesimpleencryption), false);
                        _encrypted = true;
                    }
                    catch { _encrypted = false; }
                }
            }

            return _encrypted;
        }

        /// <summary>
        /// Returns a  encrypted string value of the specified string using the specified encryption key as its pattern.
        /// </summary>
        /// <param name="value">Value to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns></returns>
        public static string Encrypt(this string value, string key)
        {
            return value.Encrypt(key, false);
        }

        /// <summary>
        /// Returns a  encrypted string value of the specified string using the specified encryption key as its pattern.
        /// </summary>
        /// <param name="value">Value to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <param name="usesimpleencryption">Use simple decryption or not</param>
        /// <returns></returns>
        public static string Encrypt(this string value, string key, bool usesimpleencryption)
        {
            return Cryptography.Cryptographer.Encrypt(value, key, usesimpleencryption);
        }

        #endregion

        /// <summary>
        /// End the progressing state of a specified progressbar object asynchronously.
        /// </summary>
        /// <param name="progressbar">Progress bar control to synchronize</param>
        public static void EndProgress(this Control progressbar)
        { Synchronization.EndProgress(progressbar); }

        #region "In"

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this byte value, params byte[] values)
        {
            bool _exists = false;

            foreach (byte i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current character value is existing within the list of reference character values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this char value, params char[] values)
        {
            bool _exists = false;

            foreach (char i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this decimal value, params decimal[] values)
        {
            bool _exists = false;

            foreach (decimal i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this double value, params double[] values)
        {
            bool _exists = false;

            foreach (double i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this float value, params float[] values)
        {
            bool _exists = false;

            foreach (float i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this int value, params int[] values)
        {
            bool _exists = false;

            foreach (int i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this long value, params long[] values)
        {
            bool _exists = false;

            foreach (long i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current numeric value is existing within the list of reference numeric values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this short value, params short[] values)
        {
            bool _exists = false;

            foreach (short i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        /// <summary>
        /// Returns whether the current text value is existing within the list of reference text values or not.
        /// </summary>
        /// <param name="value">Value to be evaluated</param>
        /// <param name="values">List of searched values</param>
        /// <returns></returns>
        public static bool In(this string value, params string[] values)
        {
            bool _exists = false;

            foreach (string i in values)
            {
                _exists = _exists || VisualBasic.CBool(value == i);
                if (_exists) break;
            }

            return _exists;
        }

        #endregion

        #region "GetValue"

        /// <summary>
        /// Returns the first row of a certain DataTable object's specified field using the supplied DataTable row filter and sorting expressions.
        /// </summary>
        /// <typeparam name="T">Expected data type to return</typeparam>
        /// <param name="table">DataTable object to get the value from</param>
        /// <param name="filter">DataTable filter expression</param>
        /// <param name="field">Field name from the DataTable's columns</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataTable table, string filter, string field)
        { return table.GetValue<T>(filter, field, default(T)); }

        /// <summary>
        /// Returns the first row of a certain DataTable object's specified field using the supplied DataTable row filter and sorting expressions.
        /// </summary>
        /// <typeparam name="T">Expected data type to return</typeparam>
        /// <param name="table">DataTable object to get the value from</param>
        /// <param name="filter">DataTable filter expression</param>
        /// <param name="field">Field name from the DataTable's columns</param>
        /// <param name="defaultvalue">Default value to return in case of failure or the returning value is DBNull or Nothing.</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataTable table, string filter, string field, T defaultvalue)
        { return table.GetValue<T>(filter, "", field, defaultvalue); }

        /// <summary>
        /// Returns the first row of a certain DataTable object's specified field using the supplied DataTable row filter and sorting expressions.
        /// </summary>
        /// <typeparam name="T">Expected data type to return</typeparam>
        /// <param name="table">DataTable object to get the value from</param>
        /// <param name="filter">DataTable filter expression</param>
        /// <param name="sort">DataTable sort expression</param>
        /// <param name="field">Field name from the DataTable's columns</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataTable table, string filter, string sort, string field)
        { return table.GetValue<T>(filter, sort, filter, default(T)); }

        /// <summary>
        /// Returns the first row of a certain DataTable object's specified field using the supplied DataTable row filter and sorting expressions.
        /// </summary>
        /// <typeparam name="T">Expected data type to return</typeparam>
        /// <param name="table">DataTable object to get the value from</param>
        /// <param name="filter">DataTable filter expression</param>
        /// <param name="sort">DataTable sort expression</param>
        /// <param name="field">Field name from the DataTable's columns</param>
        /// <param name="defaultvalue">Default value to return in case of failure or the returning value is DBNull or Nothing.</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataTable table, string filter, string sort, string field, T defaultvalue)
        {
            T _value = defaultvalue;

            if (table != null)
            {
                try
                {
                    DataRow[] rws = table.Select(filter, sort);
                    if (rws.Length > 0)
                    {
                        if (!IsNullOrNothing(rws[0][field])) _value = (T)rws[0][field];
                    } 
                }
                catch { _value = defaultvalue; }
            }

            return _value;
        }

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Sql command statement</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDbConnection connection, string sql)
        { return connection.GetValue<T>(sql, default(T)); }

        /// <summary>
        /// Returns a value based on the supplied command statement. Gets the value at the first row of the first column of the result set ignoring other values.
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Sql command statement</param>
        /// <param name="defaultvalue">Default value in case record retrieval fails or no record has been found</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDbConnection connection, string sql, T defaultvalue)
        { return Database.Que.GetValue<T>(connection, sql, defaultvalue); }

        #endregion

        /// <summary>
        /// Returns whether the specified string is an email or not.
        /// </summary>
        /// <param name="value">String value to be evaluated</param>
        /// <returns></returns>
        public static bool IsEmail(this string value)
        {
            string _pattern="^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?" +
                            "[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?" +
                            "[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\\w-]+\\.)+[a-zA-Z]{2,4})$";

            return Regex.IsMatch(value, _pattern);
        }

        /// <summary>
        /// Returns whether the specified string is a IP address.
        /// </summary>
        /// <param name="value">String value to be evaluated</param>
        /// <returns></returns>
        public static bool IsIPAddress(this string value)
        {
            string _pattern = "\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b";
            return Regex.IsMatch(value, _pattern);
        }

        /// <summary>
        /// Returns whether the specified string is a valid URL or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsURL(this string value)
        {
            string _pattern = "((([A-Za-z]{3,9}:(?:\\/\\/)?)(?:[-;:&=\\+\\$,\\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\\+\\$,\\w]+@)[A-Za-z0-9.-]+)((?:\\/[\\+~%\\/.\\w-_]*)?\\??(?:[-\\+=&;%@.\\w_]*)#?(?:[.\\!\\/\\\\w]*))?)";
            return Regex.IsMatch(value, _pattern);
        }

        /// <summary>
        /// Loads list of countries into the specified control. 
        /// </summary>
        /// <param name="control">Control whose data source will be populated</param>
        public static void LoadCountries(this Control control)
        {
            if (control != null)
            {
                if (PropertyExists(control, "DataSource"))
                {
                    DataTable _table = CountryTable;

                    bool _enabled = true;

                    if (PropertyExists(control, "Enabled"))
                    {
                        _enabled = GetPropertyValue<bool>(control, "Enabled");
                        SetPropertyValue(control, "Enabled", false);
                    }

                    object _datasource = GetProperty(control, "DataSource", null);

                    if (_datasource != null)
                    {
                        try
                        {
                            if (MethodExists(_datasource, "Dispose")) InvokeMethod(_datasource, "Dispose");
                        }
                        catch { }
                        finally { _datasource = null; }
                    }

                    SetPropertyValue(control, "DataSource", _table);

                    if (PropertyExists(control, "DisplayMember")) SetPropertyValue(control, "DisplayMember", "Country");
                    if (PropertyExists(control, "ValueMember")) SetPropertyValue(control, "ValueMember", "Country");
                    if (PropertyExists(control, "AutoCompleteMode")) SetPropertyValue(control, "AutoCompleteMode", AutoCompleteMode.SuggestAppend);
                    if (PropertyExists(control, "AutoCompleteSource")) SetPropertyValue(control, "AutoCompleteSource", AutoCompleteSource.ListItems);
                    if (PropertyExists(control, "SelectedIndex")) SetPropertyValue(control, "SelectedIndex", -1);

                    if (PropertyExists(control, "Enabled")) SetPropertyValue(control, "Enabled", _enabled);
                }
            }
        }

        /// <summary>
        /// Gets number of rows from the specified table with specified row count limits.
        /// </summary>
        /// <param name="table">DataTable object to extract the data</param>
        /// <param name="starting">Starting record count</param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static DataTable LimitRows(this DataTable table, int starting, int rows)
        {
            DataTable _table = null;
            if (table != null)
            {
                try
                {
                    int _rows = rows;
                    if (table.Rows.Count < (starting + rows)) _rows = table.Rows.Count - (table.Rows.Count.WholePartDivision(rows) * rows);
                    _table = table.AsEnumerable().Skip(starting).Take(_rows).CopyToDataTable(); 
                }
                catch { _table = table.Clone(); }
            }
            return _table;
        }

        #region "LoadData (Control)"

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        public static void LoadData(this Control control, string connectionstring, string sql)
        { control.LoadData(connectionstring, sql, ""); }

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="displaymember">Display field name for the binding</param>
        public static void LoadData(this Control control, string connectionstring, string sql, string displaymember)
        { control.LoadData(connectionstring, sql, displaymember, ""); }

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="displaymember">Display field name for the binding</param>
        /// <param name="valuemember">Value field name for the binding</param>
        public static void LoadData(this Control control, string connectionstring, string sql, string displaymember, string valuemember)
        {
            IDbConnection _connection = Database.Database.CreateConnection(connectionstring);
            control.LoadData(_connection, sql, displaymember, valuemember);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); RefreshAndManageCurrentProcess();
        }

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statement</param>
        public static void LoadData(this Control control, IDbConnection connection, string sql)
        { control.LoadData(connection, sql, ""); }

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="displaymember">Display field name for the binding</param>
        public static void LoadData(this Control control, IDbConnection connection, string sql, string displaymember)
        { control.LoadData(connection, sql, displaymember, ""); }

        /// <summary>
        /// Loads data into the specified bindable control.
        /// </summary>
        /// <param name="control">Control to be filled by data</param>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="displaymember">Display field name for the binding</param>
        /// <param name="valuemember">Value field name for the binding</param>
        public static void LoadData(this Control control, IDbConnection connection, string sql, string displaymember, string valuemember)
        {
            if (control != null && connection!=null)
            {
                DataTable table = null; table = table.LoadData(connection, sql);

                if (table != null)
                {
                    if (table.Columns.Count > 0)
                    {
                        string dm = displaymember; string vm =valuemember;

                        if (String.IsNullOrEmpty(displaymember.RLTrim())) dm = table.Columns[0].ColumnName;
                        if (String.IsNullOrEmpty(valuemember.RLTrim())) vm = table.Columns[0].ColumnName;

                        if (PropertyExists(control, "DataSource"))
                        {
                            bool _enabled = true;

                            if (PropertyExists(control, "Enabled"))
                            {
                                _enabled = GetPropertyValue<bool>(control, "Enabled");
                                SetPropertyValue(control, "Enabled", false);
                            }

                            object _datasource = GetProperty(control, "DataSource", null);
                            if (_datasource != null)
                            {
                                if (MethodExists(_datasource, "Dispose"))
                                {
                                    try { InvokeMethod(_datasource, "Dispose"); }
                                    catch { }
                                }

                                _datasource = null;
                            }

                            SetPropertyValue(control, "DataSource", table);

                            if (PropertyExists(control, "DisplayMember")) SetPropertyValue(control, "DisplayMember", dm);
                            if (PropertyExists(control, "ValueMember")) SetPropertyValue(control, "ValueMember", vm);

                            if (PropertyExists(control, "AutoCompleteMode")) SetPropertyValue(control, "AutoCompleteMode", AutoCompleteMode.SuggestAppend);
                            if (PropertyExists(control, "AutoCompleteSource")) SetPropertyValue(control, "AutoCompleteSource", AutoCompleteSource.ListItems);
                            if (PropertyExists(control, "SelectedIndex")) SetPropertyValue(control, "SelectedIndex", -1);

                            if (PropertyExists(control, "Enabled")) SetPropertyValue(control, "Enabled", _enabled);
                        }
                    }
                }
            }
        }

        #endregion

        #region "LoadData (DataTable and DataSet)"

        /// <summary>
        /// Reloads the specified DataTable object with data using the supplied database connection and command information.
        /// </summary>
        /// <param name="table">DataTable object that will be filled by data</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        public static DataTable LoadData(this DataTable table, string connectionstring, string sql)
        {
            IDbConnection _connection = Database.Database.CreateConnection(connectionstring);
            DataTable _table =  table.LoadData(_connection, sql);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); RefreshAndManageCurrentProcess(); return _table;
        }

        /// <summary>
        /// Reloads the specified DataTable object with data using the supplied database connection and command information.
        /// </summary>
        /// <param name="table">DataTable object that will be filled by data</param>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statement</param>
        public static DataTable LoadData(this DataTable table, IDbConnection connection, string sql)
        {
            Database.QueResult _result = Database.Que.Execute(connection, sql, CommandExecution.ExecuteNonQuery);
            DataTable _table = null;

            if (_result != null)
            {
                if (_result.ResultSet != null)
                {
                    if (_result.ResultSet.Tables.Count > 0) _table = _result.ResultSet.Tables[0].Replicate();
                }

                _result.Dispose(); RefreshAndManageCurrentProcess();
            }

            return _table;
        }

        /// <summary>
        /// Reloads the specified DataSet object with data using the supplied database connection and command information.
        /// </summary>
        /// <param name="dataset">DataSet object that will be filled with data</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        public static DataSet LoadData(this DataSet dataset, string connectionstring, string sql)
        {
            IDbConnection _connection = Database.Database.CreateConnection(connectionstring);
            DataSet _dataset = dataset.LoadData(_connection, sql);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); RefreshAndManageCurrentProcess(); return _dataset;
        }

        /// <summary>
        /// Reloads the specified DataSet object with data using the supplied database connection and command information.
        /// </summary>
        /// <param name="dataset">DataSet object that will be filled with data</param>
        /// <param name="connection">Database connection</param>
        /// <param name="sql">Database command statement</param>
        public static DataSet LoadData(this DataSet dataset, IDbConnection connection, string sql)
        {
            Database.QueResult _result = Database.Que.Execute(connection, sql, CommandExecution.ExecuteNonQuery);
            DataSet _dataset = null;

            if (_result != null)
            {
                if (_result.ResultSet != null)
                {
                    if (_dataset == null) _dataset = new DataSet();
                    _dataset.DataSetName = _result.ResultSet.DataSetName;

                    if (_result.ResultSet.Tables.Count > 0)
                    {
                        foreach (DataTable table in _result.ResultSet.Tables)
                        {
                            DataTable _newtable = table.Replicate();
                            _dataset.Tables.Add(_newtable); 
                        }
                    }
                }

                _result.Dispose();
            }

            return _dataset;
        }

        #endregion

        #region "LoadExcel"

        /// <summary>
        ///  Loads an excel sheet from the specified file into the specified DataTable object.
        /// </summary>
        /// <param name="table">DataTable object to be filled by data</param>
        /// <param name="filename">Excel filename</param>
        /// <returns></returns>
        public static DataTable LoadExcel(this DataTable table, string filename)
        {
            return table.LoadExcel(filename, "");
        }

        /// <summary>
        ///  Loads an excel sheet from the specified file into the specified DataTable object.
        /// </summary>
        /// <param name="table">DataTable object to be filled by data</param>
        /// <param name="filename">Excel filename</param>
        /// <param name="sheetname">Excel spreadsheet</param>
        /// <returns></returns>
        public static DataTable LoadExcel(this DataTable table, string filename, string sheetname)
        {
            DataTable _table = null; DataSet _dataset = null;
            if (String.IsNullOrEmpty(sheetname.RLTrim())) _dataset = _dataset.LoadExcel(filename);
            else  _dataset = _dataset.LoadExcel(filename, new string[] { sheetname });

            if (_dataset != null)
            {
                if (_dataset.Tables.Count > 0)
                {
                    _table = _dataset.Tables[0].Replicate();

                    foreach (DataTable tbl in _dataset.Tables)
                    {
                        try { tbl.Dispose(); }
                        catch { }
                    }
                }
                _dataset.Dispose(); RefreshAndManageCurrentProcess();
            }

            return _table;
        }

        /// <summary>
        /// Loads excel sheets from the specified file into the specified DataSet object.
        /// </summary>
        /// <param name="dataset">DataSet object to be filled by data</param>
        /// <param name="filename">Excel filename</param>
        /// <returns></returns>
        public static DataSet LoadExcel(this DataSet dataset, string filename)
        {
            string _connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            OleDbConnection _connection = new OleDbConnection(_connectionstring);
            string[] _sheetnames = null;

            try
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();
                DataTable _table = _connection.GetSchema("Tables");

                if (_table.Rows.Count > 0)
                {
                    List<string> _sheets = new List<string>();
                
                    foreach (DataRow rw in _table.Rows)
                    { _sheets.Add(rw["TABLE_NAME"].ToString().Replace("$", "")); }

                    if (_sheets.Count > 0) _sheetnames = _sheets.ToArray();
                }
            }
            catch { }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    try { _connection.Close(); }
                    catch { }
                }

                _connection.Dispose(); RefreshAndManageCurrentProcess();
            }

            if (_sheetnames != null) return dataset.LoadExcel(filename, _sheetnames);
            else return null;
        }

        /// <summary>
        /// Loads excel sheets from the specified file into the specified DataSet object.
        /// </summary>
        /// <param name="dataset">DataSet object to be filled by data</param>
        /// <param name="filename">Excel filename</param>
        /// <param name="sheetnames">Excel spreadsheets</param>
        /// <returns></returns>
        public static DataSet LoadExcel(this DataSet dataset, string filename, string[] sheetnames)
        {
            string _connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            OleDbConnection _connection = new OleDbConnection(_connectionstring);
            string _query = "";

            foreach (string sheet in sheetnames)
            {
                _query += (String.IsNullOrEmpty(_query.RLTrim()) ? "" : "\n") + "SELECT * FROM [" + sheet + "$];";
            }

            DataSet _dataset = null;
            _dataset = _dataset.LoadData(_connection, _query);

            if (_connection.State == ConnectionState.Open)
            {
                try { _connection.Close(); }
                catch { }
            }

            _connection.Dispose(); RefreshAndManageCurrentProcess();

            return _dataset;
        }

        #endregion

        /// <summary>
        /// Releases all relative resources (including processes) of the whole application after the current form is disposed.
        /// </summary>
        /// <param name="form">Form to be associated with the disposal management routine</param>
        public static void ManageOnDispose(this Form form)
        {
            if (form != null) form.FormClosed += new FormClosedEventHandler(DisposingForm_Closed);
        }

        /// <summary>
        /// Places a '*' character at the rightmost side of the specified form's caption indicating a value inside a form has been changed.
        /// </summary>
        /// <param name="form"></param>
        public static void MarkAsEdited(this Form form)
        {
            if (form != null)
            {
                if (!form.Text.RLTrim().EndsWith("*")) form.Text += " *";
            }
        }

        #region "MySqlExec"

        /// <summary>
        /// Executes the specified file's sql statement contents into the specified MySQL database using the supplied database connection string.
        /// </summary>
        /// <param name="file">MySQL database dump file</param>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <returns></returns>
        public static Database.MySqlResult MySqlExec(this FileInfo file, string connectionstring)
        { return file.MySqlExec(connectionstring, null); }

        /// <summary>
        /// Executes the specified file's sql statement contents into the specified MySQL database using the supplied database connection string.
        /// </summary>
        /// <param name="file">MySQL database dump file</param>
        /// <param name="connectionstring">MySql database connection string</param>
        /// <param name="parameters">Additional MySql parameters</param>
        /// <returns></returns>
        public static Database.MySqlResult MySqlExec(this FileInfo file, string connectionstring, Database.MySqlParameterCollection parameters)
        { return Database.MySql.Execute(connectionstring, file, parameters); }

        #endregion

        /// <summary>
        /// Returns the specified file's contents.
        /// </summary>
        /// <param name="file">File to read to contents from</param>
        /// <returns></returns>
        public static string Read(this FileInfo file)
        { return ReadFile(file); }

        /// <summary>
        /// Creates an exact replica (schema and contents) of the specified DataTable object.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable Replicate(this DataTable table)
        {
            DataTable _table = null;

            if (table != null)
            {
                _table = table.Clone();
                _table.TableName = table.TableName;
                _table.Load(table.CreateDataReader());
                _table.AcceptChanges(); 
            }

            return _table;
        }

        #region "ResizeImage"

        /// <summary>
        /// Sets the display size of the specified image.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="width">Assigned width</param>
        /// <param name="height">Assigned height</param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, int width, int height)
        { return image.ResizeImage(new Size(width,height)); }

        /// <summary>
        /// Sets the display size of the specified image.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="size">Size information</param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, Size size)
        {
            Image _image = null;

            if (image != null)
            {
                Bitmap _bitmap = new Bitmap(image);
                int _width = size.Width; int _height = size.Height;
                if (_width <= 0) _width = 1;
                if (_height <= 0) _height = 1;
                Bitmap _resized = new Bitmap(_width, _height);
                Graphics _graphics = Graphics.FromImage(_resized);

                _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                _graphics.DrawImage(_bitmap, new Rectangle(0, 0, _width, _height), new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), GraphicsUnit.Pixel);
                _graphics.Dispose();

                _bitmap.Dispose(); RefreshAndManageCurrentProcess(); _image = _resized;

            }

            return _image;
        }

        #endregion

        /// <summary>
        /// Returns a complete space trimmed representation of the specified value (combination of RTrim and String.Trim() functions).
        /// </summary>
        /// <param name="value">String value to be truncated</param>
        /// <returns></returns>
        public static string RLTrim(this string value)
        { return value.TrimStart().TrimEnd().Trim(); }

        #region "SaveExcel"

        /// <summary>
        /// Exports the specified DataSet into an MS Excel file and return the file's information if it is successfully generated.
        /// </summary>
        /// <param name="table">DataTable object to be exported</param>
        /// <param name="filename">Export path</param>
        /// <returns></returns>
        public static FileInfo SaveExcel(this DataTable table, string filename)
        {
            FileInfo _file = null;

            if (table != null)
            {
                DataSourceExcelWriter _wtiter = new DataSourceExcelWriter(table, filename);
                try { _file = _wtiter.Write(); }
                catch { _file = null; }
                finally { _wtiter.Dispose(); }
            }

            return _file;
        }

        /// <summary>
        /// Exports the specified DataSet into an MS Excel file and return the file's information if it is successfully generated.
        /// </summary>
        /// <param name="dataset">DataSet object to be exported</param>
        /// <param name="filename">Export path</param>
        /// <returns></returns>
        public static FileInfo SaveExcel(this DataSet dataset, string filename)
        {
            FileInfo _file = null;

            if (dataset !=null)
            {
                if (dataset.Tables.Count > 0)
                {
                    DataSourceExcelWriter _wtiter = new DataSourceExcelWriter(dataset, filename);
                    try  { _file = _wtiter.Write(); }
                    catch { _file = null;}
                    finally { _wtiter.Dispose(); }
                }
            }

            return _file;
        }

        #endregion

        #region "SetAsRequired"

        /// <summary>
        /// Sets the specified control marked as a required field placing an indicator in a certain area of the control.
        /// </summary>
        /// <param name="control">Control to be marked / unmarked</param>
        public static void SetAsRequired(this Control control)
        { control.SetAsRequired(true); }

        /// <summary>
        /// Sets the specified control marked as a required field placing an indicator in a certain area of the control.
        /// </summary>
        /// <param name="control">Control to be marked / unmarked</param>
        /// <param name="required">Determines whether control will marked or unmarked as required field</param>
        public static void SetAsRequired(this Control control, bool required)
        { Controls.RequiredFieldMarker.SetAsRequired(control, required); }

        #endregion

        #region "ToByteArray"

        /// <summary>
        /// Converts the specified file into its corresponding byte array representation.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this FileInfo file)
        {
            return FileObjectToByteArray(file);
        }

        /// <summary>
        /// Converts the specified image into its corresponding byte array representation.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image image)
        {
            return ImageToByteArray(image);
        }

        /// <summary>
        /// Converts the specified hexadecimal string into uts corresponding byte array representation.
        /// </summary>
        /// <param name="hex">Hexadecimal string to convert</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hex)
        {
            return HexadecimalStringToByteArray(hex);
        }

        #endregion

        /// <summary>
        /// Converts the specified DataTable object into a serializable table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable ToExportableTable(this DataTable table)
        {
            DataTable _newtable = null;

            if (table != null)
            {
                _newtable = new DataTable(); _newtable.TableName = table.TableName;

                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Name.Trim().ToLower().Contains("byte[]") ||
                        col.DataType.Name.Trim().ToLower().Contains("byte()") ||
                        col.DataType.Name.Trim().ToLower().Contains("bytes[]") ||
                        col.DataType.Name.Trim().ToLower().Contains("bytes()") ||
                        col.DataType.Name.Trim().ToLower().Contains("image") ||
                        col.DataType.Name.Trim().ToLower().Contains("bitmap")) _newtable.Columns.Add(col.ColumnName, typeof(string));
                    else _newtable.Columns.Add(col.ColumnName, col.DataType);
                }

                foreach (DataRow rw in table.Rows)
                {
                    if (rw.RowState != DataRowState.Deleted &&
                        rw.RowState != DataRowState.Detached)
                    {
                        object[] _values = new object[_newtable.Columns.Count];
                        foreach (DataColumn col in table.Columns)
                        {
                            if (col.DataType.Name.Trim().ToLower().Contains("byte[]") ||
                                col.DataType.Name.Trim().ToLower().Contains("byte()") ||
                                col.DataType.Name.Trim().ToLower().Contains("bytes[]") ||
                                col.DataType.Name.Trim().ToLower().Contains("bytes()") ||
                                col.DataType.Name.Trim().ToLower().Contains("image") ||
                                col.DataType.Name.Trim().ToLower().Contains("bitmap"))
                            {
                                if (col.DataType.Name.Trim().ToLower().Contains("image") ||
                                    col.DataType.Name.Trim().ToLower().Contains("bitmap"))
                                {
                                    if (!IsNullOrNothing(rw[col.ColumnName]))
                                    {
                                        try
                                        { _values[col.Ordinal] = ((Image)rw[col.ColumnName]).ToHexadecimalString(); }
                                        catch { _values[col.Ordinal] = ""; }
                                    }
                                    else _values[col.Ordinal] = "";
                                }
                                else
                                {
                                    if (!IsNullOrNothing(rw[col.ColumnName]))
                                    {
                                        try
                                        { _values[col.Ordinal] = ((byte[])rw[col.ColumnName]).ToHexadecimalString(); }
                                        catch { _values[col.Ordinal] = ""; }
                                    }
                                    else _values[col.Ordinal] = "";
                                }
                            }
                            else _values[col.Ordinal] = rw[col.ColumnName];
                        }
                        _newtable.Rows.Add(_values);
                    }
                }
            }

            return _newtable;
        }

        #region "ToFileObject"

        /// <summary>
        /// Converts the specified byte array into a file object with the specified extension name.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <param name="extension">File extension</param>
        /// <returns></returns>
        public static FileInfo ToFileObject(this byte[] bytes, string extension)
        {  return bytes.ToFileObject(extension, Environment.CurrentDirectory); }

        /// <summary>
        /// Converts the specified byte array into a file object with the specified extension name.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <param name="extension">File extension</param>
        /// <param name="outputdirectory">Output directory for the exported file</param>
        /// <returns></returns>
        public static FileInfo ToFileObject(this byte[] bytes, string extension, string outputdirectory)
        { return ByteArrayToFileObject(bytes, extension, outputdirectory); }

        #endregion

        #region "ToHexadecimalString"

        /// <summary>
        /// Returns the hexadecimal string representation of the specified byte array.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns></returns>
        public static string ToHexadecimalString(this byte[] bytes)
        { return ByteArrayToHexaDecimalString(bytes);  }

        /// <summary>
        /// Returns the hexadecimal string representation of the specified file.
        /// </summary>
        /// <param name="file">File to convert</param>
        /// <returns></returns>
        public static string ToHexadecimalString(this FileInfo file)
        { return FileObjectToHexaDecimalString(file);  }

        /// <summary>
        /// Returns the hexadecimal string representation of the specified image.
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <returns></returns>
        public static string ToHexadecimalString(this Image image)
        { return ImageToHexaDecimalString(image); }

        #endregion

        /// <summary>
        /// Returns the image representation of the specified byte array.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns></returns>
        public static Image ToImage(this byte[] bytes)
        { return ByteArrayToImage(bytes); }

        /// <summary>
        /// Returns the proper case (first letter capitalized and small caps for the preceeding letters) representation of the specified string.
        /// </summary>
        /// <param name="value">String value to be transformed</param>
        /// <returns></returns>
        public static string ToProper(this string value)
        {
            string _value = "";

            if (!String.IsNullOrEmpty(value.RLTrim()))
            {
                string[] _sections = value.Split(new char[] { ' ' });
                if (_sections.Length >= 1)
                {
                    foreach (string _section in _sections)
                    {
                        string _currentvalue = "";

                        char[] _chars = _section.ToCharArray();

                        foreach (char _char in _chars)
                        {
                            if (Char.IsLetter(_char))
                            {
                                if (String.IsNullOrEmpty(_currentvalue.RLTrim())) _currentvalue += _char.ToString().ToUpper();
                                else _currentvalue += _char.ToString().ToLower();
                            }
                            else _currentvalue += _char.ToString();
                        }

                        _value += _currentvalue + " ";
                    }
                }
                else
                {
                    char[] _chars = value.ToCharArray();

                    foreach (char _char in _chars)
                    {
                        if (Char.IsLetter(_char))
                        {
                            if (String.IsNullOrEmpty(_value.RLTrim())) _value += _char.ToString().ToUpper();
                            else _value += _char.ToString().ToLower();
                        }
                        else _value += _char.ToString();
                    }
                }
            }

            return _value.Trim();
        }

        #region "ToSqlValidString"

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this byte value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this byte value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts date value to its SQL qualified date-string representation.
        /// </summary>
        /// <param name="value">Date value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this DateTime value)
        { return value.ToSqlValidString(false); }

        /// <summary>
        /// Converts date value to its SQL qualified date-string representation.
        /// </summary>
        /// <param name="value">Date value to convert.</param>
        /// <param name="withhours">Determines if output string shall represent the time together with the date.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this DateTime value, bool withhours)
        {
            string _format = "yyyy-MM-dd";
            if (withhours) _format += " HH:mm:ss";
            return VisualBasic.Format(value, _format);
        }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this decimal value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this decimal value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this double value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this float value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this float value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this int value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this int value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this double value, int decimalplaces)
        { return VisualBasic.Format(value, "F" + decimalplaces.ToString()); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this long value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this long value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this short value)
        { return value.ToSqlValidString(2); }

        /// <summary>
        /// Converts numeric value at floating point to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="decimalplaces">Number of decimal places.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this short value, int decimalplaces)
        { return double.Parse(value.ToString()).ToSqlValidString(decimalplaces); }

        /// <summary>
        /// Converts string value to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this string value)
        { return value.ToSqlValidString(false); }

        /// <summary>
        /// Converts string value to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <param name="datatableexpression">Determines if output will be used as a DataTable / DataColumn expression qualified string.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this string value, bool datatableexpression)
        { return value.ToSqlValidString(datatableexpression, !datatableexpression); }

        /// <summary>
        /// Converts string value to its SQL qualified string representation.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <param name="datatableexpression">Determines if output will be used as a DataTable / DataColumn expression qualified string.</param>
        /// <param name="trimmed">Determines whether to peform text carriage-return trimming before actual evaluation.</param>
        /// <returns></returns>
        public static string ToSqlValidString(this string value, bool datatableexpression, bool trimmed)
        {
            string _value = (trimmed ? value.Trim() : value);

            if (datatableexpression) _value = _value.Replace("'", "''").Replace("\\", "\\\\").Replace("[", "[[").Replace("]", "]]").Replace("[[", "[[]").Replace("]]", "[]]").Replace("*", "*]").Replace("*", "[*").Replace("%", "%]").Replace("%", "[%");
            else _value = _value.Replace("'", "''").Replace("\\", "\\\\");
            
            return _value;
        }

        #endregion

        #region "ToWords"

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this byte value)
        { return value.ToWords(""); }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this byte value, string currency)
        { return ((double)value).ToWords(currency); }

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this decimal value)
        {  return value.ToWords(""); }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this decimal value, string currency)
        { return ((double)value).ToWords(currency); }

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this double value)
        { return value.ToWords("");  }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this double value, string currency)
        { return AmountToWordsConverter.AmountToWords(value, currency);  }

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this float value)
        {  return value.ToWords("");  }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this float value, string currency)
        { return ((double)value).ToWords(currency);  }

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this int value)
        {  return value.ToWords(""); }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this int value, string currency)
        {  return ((double)value).ToWords(currency); }

        /// <summary>
        /// Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <returns></returns>
        public static string ToWords(this short value)
        {  return value.ToWords("");  }

        /// <summary>
        ///  Returns the english-word representation of the specified value.
        /// </summary>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="currency">Suffixing currency.</param>
        /// <returns></returns>
        public static string ToWords(this short value, string currency)
        {  return ((double)value).ToWords(currency);  }

        #endregion

        #region "WaitToFinish"

        /// <summary>
        /// Synchronizes the specified IAsyncResult object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="result">IAsyncResult to synchronize</param>
        public static void WaitToFinish(this IAsyncResult result)
        { result.WaitToFinish(null);  }

        /// <summary>
        /// Synchronizes the specified IAsyncResult object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="result">IAsyncResult to synchronize</param>
        /// <param name="progressbar">Synchronization progress bar</param>
        public static void WaitToFinish(this IAsyncResult result, object progressbar)
        { Synchronization.WaitToFinish(result, progressbar); }

        /// <summary>
        /// Synchronizes the specified Thread object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="thread">Thread to synchronize</param>
        public static void WaitToFinish(this Thread thread)
        { thread.WaitToFinish(null);  }

        /// <summary>
        /// Synchronizes the specified Thread object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="thread">Thread to synchronize</param>
        /// <param name="progressbar">Synchronization progress bar</param>
        public static void WaitToFinish(this Thread thread, object progressbar)
        { Synchronization.WaitToFinish(thread, progressbar);  }

        #endregion

        #region "WholePartDivision"

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this byte value, int divisor)
        { return WholePartDivision((double)value, divisor); }

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this float value, int divisor)
        { return WholePartDivision((double)value, divisor);  }

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this int value, int divisor)
        { return WholePartDivision((double)value, divisor); }

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this long value, int divisor)
        { return WholePartDivision((double)value, divisor); }

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this decimal value, int divisor)
        { return WholePartDivision((double)value, divisor);  }

        /// <summary>
        /// Gets the whole part of a division mathematical operation using the specified divisor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int WholePartDivision(this double value, int divisor)
        {
            double _remainder = value % divisor;
            int _value = (int) ((value - _remainder) / divisor);
            return _value;
        }

        #endregion

        #region "Write"

        /// <summary>
        /// Returns whether specified value was written into the current file or not.
        /// </summary>
        /// <param name="file">Path</param>
        /// <param name="contents">Value to be written</param>
        /// <returns></returns>
        public static bool Write(this FileInfo file, string contents)
        { return file.Write(contents, false); }

        /// <summary>
        /// Returns whether specified value was written into the current file or not.
        /// </summary>
        /// <param name="file">Path</param>
        /// <param name="contents">Value to be written</param>
        /// <param name="append">Determines whether to overwrite existing file contents or not</param>
        /// <returns></returns>
        public static bool Write(this FileInfo file, string contents, bool append)
        { return VisualBasic.CBool( ((FileInfo) WriteToFile(file.FullName, contents, append))!=null); }

        #endregion

    }
}
