#region "imports"

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Development.Materia
{
    /// <summary>
    /// Electronic data interchange file class.
    /// </summary>
    public class EdiFile : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        public EdiFile(string filename) : this(filename, "X-X!V''Q")
        { }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="encryptionkey">Encryption key</param>
        public EdiFile(string filename, string encryptionkey) : this(filename, "", encryptionkey)
        { }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="contents">Encrypted or decrypted file contents</param>
        /// <param name="encryptionkey">Encryption key</param>
        public EdiFile(string filename, string contents, string encryptionkey)
        {
            _filename = filename; _contents.Append(contents); _encryptionkey = encryptionkey;
        }


        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database retrieving command statement</param>
        /// <param name="encryptionkey">Encryption key</param>
        public EdiFile(string filename, string connectionstring, string sql, string encryptionkey) : this(filename, Database.Database.CreateConnection (connectionstring),sql,encryptionkey)
        { }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="connection">Database connection object</param>
        /// <param name="sql">Database retrieving command statement</param>
        public EdiFile(string filename, IDbConnection connection, string sql) : this(filename, connection,sql,"X-X!V''Q")
        { }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="connection">Database connection object</param>
        /// <param name="sql">Database retrieving command statement</param>
        /// <param name="encryptionkey">Encryption key</param>
        public EdiFile(string filename, IDbConnection connection, string sql, string encryptionkey)
        {
            _filename = filename; _encryptionkey = encryptionkey;
            DataTable _table = null; _table = _table.LoadData(connection, sql);
            if (_table != null) InitializedContentsFromDataSource(_table);
        }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="table">Data source object to be encrypted</param>
        public EdiFile(string filename, DataTable table) : this(filename, table, "X-X!V''Q")
        { }

        /// <summary>
        /// Creates a new instance of EdiFile.
        /// </summary>
        /// <param name="filename">File to be encrypted or decrypted</param>
        /// <param name="table">Data source object to be encrypted</param>
        /// <param name="encryptionkey">Encryption key</param>
        public EdiFile(string filename, DataTable table, string encryptionkey)
        {
            _filename = filename; _encryptionkey = encryptionkey; InitializedContentsFromDataSource(table);
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private StringBuilder _contents = new StringBuilder();

        /// <summary>
        /// Gets the decrypted file's contents.
        /// </summary>
        public string Contents
        {
            get { return _contents.ToString(); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _encryptionkey = "X-X!V''Q";

        /// <summary>
        /// Encryption key to used for encryption or decryption of the file.
        /// </summary>
        public string EncryptionKey
        {
            get { return _encryptionkey; }
            set { _encryptionkey = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _filename = "";

        /// <summary>
        /// Gets the file to be written or to read the contents from.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
        }

        #endregion

        #region "methods"

        private void ClearContents()
        {
            if (_contents != null)
            {
                if (_contents.Length > 0)
                {
                    try { _contents.Remove(0, _contents.Length - 1); }
                    catch { }
                    finally { Materia.RefreshAndManageCurrentProcess(); }
                }
            }

            _contents = null; _contents = new StringBuilder();
        }

        /// <summary>
        /// Gets the decrypted file's contents using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File to decrypt</param>
        /// <returns>Decrypted file's contents. Returns an empty string if decryption process fails.</returns>
        public static string DecryptFromFile(string filename)
        { return DecryptFromFile(filename, "X-X!V''Q"); }

        /// <summary>
        /// Gets the decrypted file's contents using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File to decrypt</param>
        /// <param name="encryptionkey">Encryption key</param>
        /// <returns>Decrypted file's contents. Returns an empty string if decryption process fails.</returns>
        public static string DecryptFromFile(string filename, string encryptionkey)
        {
            StringBuilder _decrypted = new StringBuilder();
            EdiFile _edi = new EdiFile(filename, encryptionkey);
            _edi.Read(); _decrypted.Append(_edi.Contents); _edi.Dispose();
            return _decrypted.ToString();
        }

        private void InitializedContentsFromDataSource(DataTable table)
        {
            if (table != null)
            {
                ClearContents();
                string _temppath = Application.StartupPath + "\\tempdatasource.xml";
                try { table.WriteXml(_temppath, XmlWriteMode.WriteSchema); }
                catch { }

                if (File.Exists(_temppath))
                {
                    FileInfo _file = new FileInfo(_temppath);
                    try { _contents.Append(_file.Read()); }
                    catch { }
                    finally
                    {
                        try { File.Delete(_temppath); }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts the specified contents into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="contents">File contents to encrypt</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, string contents)
        { return EncryptToFile(filename, contents, "X-X!V''Q"); }

        /// <summary>
        /// Encrypts the specified contents into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="contents">File contents to encrypt</param>
        /// <param name="encryptionkey">Encryption key</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, string contents, string encryptionkey)
        {
            FileInfo _file = null;

            EdiFile _edi = new EdiFile(filename, contents, encryptionkey);
            _file = _edi.Write(); _edi.Dispose();

            return _file;
        }

        /// <summary>
        /// Encrypts data retrieved using the specified database connection and command statement into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="connectionstring">Database connection string</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="encryptionkey">Encryption key</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, string connectionstring, string sql, string encryptionkey)
        { return EncryptToFile(filename, Database.Database.CreateConnection(connectionstring), sql, encryptionkey); }

        /// <summary>
        /// Encrypts data retrieved using the specified database connection and command statement into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="connection">Database connection object</param>
        /// <param name="sql">Database command statement</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, IDbConnection connection, string sql)
        { return EncryptToFile(filename, connection, sql, "X-X!V''Q"); }

        /// <summary>
        /// Encrypts data retrieved using the specified database connection and command statement into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="connection">Database connection object</param>
        /// <param name="sql">Database command statement</param>
        /// <param name="encryptionkey">Encryption key</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, IDbConnection connection, string sql, string encryptionkey)
        {
            FileInfo _file = null;

            EdiFile _edi = new EdiFile(filename, connection, sql, encryptionkey);
            _file = _edi.Write(); _edi.Dispose();

            return _file;
        }

        /// <summary>
        /// Encrypts the specified DataTable object's contents into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="table">DataTable object to encrypt</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, DataTable table)
        { return EncryptToFile(filename, table, "X-X!V''Q"); }

        /// <summary>
        /// Encrypts the specified DataTable object's contents into a file using the supplied encryption key.
        /// </summary>
        /// <param name="filename">File path to put in the encrypted contents</param>
        /// <param name="table">DataTable object to encrypt</param>
        /// <param name="encryptionkey">Encryption key</param>
        /// <returns>System.IO.FileInfo object that corresponds the encrypted file's information. Returns nothing if encryption process fails.</returns>
        public static FileInfo EncryptToFile(string filename, DataTable table, string encryptionkey)
        {
            FileInfo _file = null;

            EdiFile _edi = new EdiFile(filename, table, encryptionkey);
            _file = _edi.Write(); _edi.Dispose();

            return _file;
        }

        /// <summary>
        /// Loads the decrypted contents of the initialiazed encrypted file.
        /// </summary>
        public void Read()
        {
            ClearContents();

            if (!String.IsNullOrEmpty(_filename.RLTrim()))
            {
                StreamReader _reader = new StreamReader(_filename);
                try
                { _contents.Append(_reader.ReadToEnd().Decrypt(_encryptionkey)); }
                catch { }
                finally
                {
                    _reader.Close(); _reader.Dispose();
                    Materia.RefreshAndManageCurrentProcess();
                }
            }
        }

        /// <summary>
        /// Writes the encrypted file initialized contents into the specified file.
        /// </summary>
        /// <returns>System.IO.FileInfo object that corresponds the written file's information. Returns nothing if error has been encountered during file-writing process.</returns>
        public FileInfo Write()
        {
            FileInfo _file = null;

            if (!String.IsNullOrEmpty(_filename.RLTrim()))
            {
                StreamWriter _writer = new StreamWriter(_filename);
                StringBuilder _encrypted = new StringBuilder();
                bool _written = false;
                
                try
                {
                    _encrypted.Append(_contents.ToString().Encrypt(_encryptionkey));
                    _writer.Write(_encrypted.ToString()); _written = true;
                }
                catch
                { _file = null; _written = false; }
                finally
                {
                    if (_encrypted.Length > 0)
                    {
                        try { _encrypted.Remove(0, _encrypted.Length); }
                        catch { }
                    }
                    _encrypted = null;
                    _writer.Close(); _writer.Dispose();
                    Materia.RefreshAndManageCurrentProcess();
                }

                if (_written)
                {
                    if (File.Exists(_filename)) _file = new FileInfo(_filename);
                }
            }

            return _file;
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
                    ClearContents(); 
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
