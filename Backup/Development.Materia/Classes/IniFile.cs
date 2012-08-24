#region "imports"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Development.Materia
{
    /// <summary>
    /// Ini file reader and writer.
    /// </summary>
    public class IniFile : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of IniReader.
        /// </summary>
        /// <param name="filename"></param>
        public IniFile(string filename)
        { _filename = filename; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _filename = "";

        /// <summary>
        /// Gets or sets file to read the settings from.
        /// </summary>
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// Gets the assigned value of a key within the specified section in the ini file.
        /// </summary>
        /// <param name="section">Ini file section.</param>
        /// <param name="key">Ini file section key</param>
        /// <returns></returns>
        public string Sections(string section, string key)
        { return ReadSection(section, key); }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _filecontents = "";

        #endregion

        #region "methods"

        private string Contents()
        {
            string _contents = "";

            if (IsIniFile())
            {
                if (String.IsNullOrEmpty(_contents.RLTrim()))
                {
                    FileInfo _file = new FileInfo(_filename);
                    _contents = _file.Read();
                }
            }

            return _contents;
        }

        /// <summary>
        /// Calls the IniFile class' Section property to get an specific key value under the specified section within the specified ini file.
        /// </summary>
        /// <param name="filename">Ini file path</param>
        /// <param name="section">Section in the ini file</param>
        /// <param name="key">Key to get the value from</param>
        /// <returns></returns>
        public static string GetKeyValue(string filename, string section, string key)
        {
            string _value = "";

            IniFile _ini = new IniFile(filename);
            _value = _ini.Sections(section, key); _ini.Dispose();

            return _value;
        }

        private bool IsIniFile()
        {
            bool _isinifile = false;

            if (!String.IsNullOrEmpty(_filename.RLTrim()))
            {
                if (File.Exists(_filename)) _isinifile = VisualBasic.CBool(Path.GetExtension(_filename).RLTrim().ToLower().Replace(".", "") == "ini");
            }

            return _isinifile;
        }

        private string ReadSection(string section, string key)
        {
            string _value = "";

            if (String.IsNullOrEmpty(_filecontents.RLTrim())) _filecontents = Contents();

            if (!String.IsNullOrEmpty(_filecontents.RLTrim()))
            {
                string[] _lines = _filecontents.Split("\n".ToCharArray());
                int _startline = 0; int _endline = 0;
                bool _withstart = false; bool _withend = false;

                for (int i = 0; i <= (_lines.Length - 1); i++)
                {
                    string _line = _lines[i];
                    if (_line.RLTrim().ToUpper() == "[" + section.ToUpper() + "]")
                    {
                        _startline = i; _withstart = true; break;
                    }
                }

                if (_withstart)
                {
                    if ((_startline + 1) <= (_lines.Length - 1))
                    {
                        int _curline = 0;

                        for (int i = (_startline + 1); i <= (_lines.Length - 1); i++)
                        {
                            string _line = _lines[i];
                            if (_line.RLTrim().StartsWith("[") &&
                                _line.RLTrim().EndsWith("]"))
                            {
                                if (i != _startline)
                                {
                                    _endline = i - 1; _withend = true;
                                }
                                break;
                            }

                            _curline = i;
                        }

                        if (_curline <= (_lines.Length - 1))
                        {
                            _withend = true; _endline = _curline;
                        }
                    }
                }

                if (_withstart && _withend)
                {
                    for (int i = _startline; i <= _endline; i++)
                    {
                        string _line = _lines[i];
                        char[] _chars = _line.ToCharArray();
                        string _key = "";

                        if (_chars.Length > 0)
                        {
                            foreach (char _char in _chars)
                            {
                                if (_char.ToString() != " " &&
                                    !String.IsNullOrEmpty(_char.ToString().RLTrim())) _key += _char.ToString();

                                if (_key.RLTrim().StartsWith(key.RLTrim() +  "=") &&
                                    !(_key.RLTrim() == (key.RLTrim() + "="))) _value += _char.ToString();
                            }
                        }
                    }
                }
            }

            return _value;
        }

        /// <summary>
        /// Calls the IniFile class' SetValue function to assign a value in the specified key under the specified section of the specified ini file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetKeyValue(string filename, string section, string key, string value)
        {
            bool _set = false;

            IniFile _ini = new IniFile(filename);
            _set = _ini.SetValue(section, key, value);
            _ini.Dispose();

            return _set;
        }

        /// <summary>
        /// Sets a value in the specified key of the specified section within the ini file's contents.
        /// </summary>
        /// <param name="section">Ini file section</param>
        /// <param name="key">Ini file section key</param>
        /// <param name="value">Key value to assign</param>
        /// <returns></returns>
        public bool SetValue(string section, string key, string value)
        {
            bool _set = false;
            if (String.IsNullOrEmpty(_filecontents.RLTrim())) _filecontents = Contents();

            if (!String.IsNullOrEmpty(_filecontents.RLTrim()))
            {
                string[] _lines = _filecontents.Split("\n".ToCharArray());
                int _startline = 0; int _endline = 0;

                for (int i = 0; i <= (_lines.Length - 1); i++)
                {
                    string _line = _lines[i];
                    if (_line.RLTrim().ToUpper() == "[" + section.ToUpper() + "]")
                    {
                        _startline = i; break;
                    }
                }

                if ((_startline + 1) <= (_lines.Length - 1))
                {

                    for (int i = (_startline + 1); i <= (_lines.Length - 1); i++)
                    {
                        string _line = _lines[i];
                        if (_line.RLTrim().StartsWith("[") &&
                            _line.RLTrim().EndsWith("]")) break;
                        _endline = i;
                    }

                    for (int i = _startline; i <= _endline; i++)
                    {
                        string _line = _lines[i];
                        char[] _chars = _line.ToCharArray();
                        string _key = "";

                        foreach (char _char in _chars)
                        {
                            if (_char.ToString() != " ") _key += _char.ToString();
                            if (_key.RLTrim().ToUpper().Contains(key.ToUpper() + "="))
                            {
                                _lines[i] = key.ToUpper() + "=" + value;
                                StreamWriter _writer = new StreamWriter(_filename);
                                try
                                {
                                    for (int a = 0; a <= (_lines.Length - 1); a++)
                                    { _writer.WriteLine(_lines[a]); }
                                    _set = true;
                                }
                                catch { _set = false; }
                                finally
                                { _writer.Close(); _writer.Dispose(); Materia.RefreshAndManageCurrentProcess(); }

                                return _set;
                            }
                        }

                        StringBuilder _contents = new StringBuilder();
                        _contents.Append(_filecontents);

                        if (_endline < (_lines.Length - 1)) _filecontents.RLTrim().Replace(_lines[_endline], _lines[_endline] + "\n" + key + "=" + value);
                        else _filecontents += (_filecontents.RLTrim().EndsWith("\n") ? "" : "\n") + key + "=" + value;

                        FileInfo _file = new FileInfo(_filename);
                        _set = _file.Write(_filecontents);
                    }
                }
            }

            return _set;
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
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }
}
