#region "imports"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Development.Materia
{
    /// <summary>
    /// Creates a new instance of Archiver.
    /// </summary>
    public class Archiver : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of Archiver.
        /// </summary>
        public Archiver() : this("")
        { }

        /// <summary>
        /// Creates a new instance of Archiver.
        /// </summary>
        /// <param name="path">Path of the file or directory to compress</param>
        public Archiver(string path) : this(path, ArchivingToolEnum.SevenZip)
        { }

        /// <summary>
        /// Creates a new instance of Archiver.
        /// </summary>
        /// <param name="path">Path of the file or directory to compress</param>
        /// <param name="archivingtool">Archiving tool use</param>
        public Archiver(string path, ArchivingToolEnum archivingtool) : this(path, archivingtool, ProcessWindowStyle.Hidden)
        { }

        /// <summary>
        /// Creates a new instance of Archiver.
        /// </summary>
        /// <param name="path">Path of the file or directory to compress</param>
        /// <param name="archivingtool">Archiving tool use</param>
        /// <param name="processwindowstyle">Command prompt window visibility upon compression</param>
        public Archiver(string path, ArchivingToolEnum archivingtool, ProcessWindowStyle processwindowstyle) : this(path,archivingtool,processwindowstyle, ArchivingMethodEnum.Insert)
        { }

        /// <summary>
        /// Creates a new instance of Archiver.
        /// </summary>
        /// <param name="path">Path of the file or directory to compress</param>
        /// <param name="archivingtool">Archiving tool use</param>
        /// <param name="processwindowstyle">Command prompt window visibility upon compression</param>
        /// <param name="archivingmethod">Archiving method to use</param>
        public Archiver(string path, ArchivingToolEnum archivingtool, ProcessWindowStyle processwindowstyle, ArchivingMethodEnum archivingmethod)
        {
            _path = path; _archivingtool = archivingtool; _processwindowstyle = processwindowstyle;
            _archivingmethod = archivingmethod; _archivedpath = "";
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _archivedpath = "";

        /// <summary>
        ///  Gets archive's path after successful compression.
        /// </summary>
        public string ArchivedPath
        {
            get { return _archivedpath; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ArchivingMethodEnum _archivingmethod = ArchivingMethodEnum.Insert;

        /// <summary>
        /// Gets or sets archiving method.
        /// </summary>
        public ArchivingMethodEnum ArchivingMethod
        {
            get { return _archivingmethod; }
            set { _archivingmethod = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ArchivingToolEnum _archivingtool = ArchivingToolEnum.SevenZip;

        /// <summary>
        /// Gets or sets compression tool to use.
        /// </summary>
        public ArchivingToolEnum ArchivingTool
        {
            get { return _archivingtool; }
            set { _archivingtool = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _path = "";

        /// <summary>
        /// Gets or sets file's / directory's path to compress.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProcessWindowStyle _processwindowstyle = ProcessWindowStyle.Hidden;

        /// <summary>
        /// Gets or sets command prompt's window status upon compression.
        /// </summary>
        public ProcessWindowStyle ProcessWindowStyle
        {
            get { return _processwindowstyle; }
            set { _processwindowstyle = value; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Perform file / directory compression.
        /// </summary>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="Archiver 01" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="Archiver 01" language="vbnet" />
        /// </example> 
        public FileInfo Archive()
        {
            FileInfo _file = null; bool _pathexists = false; _archivedpath = "";

            if (System.IO.Path.HasExtension(_path)) _pathexists = File.Exists(_path);
            else _pathexists = Directory.Exists(_path);

            if (_pathexists)
            {
                ExtractResourceApplications();

                switch (_archivingtool)
                {
                    case ArchivingToolEnum.SevenZip :
                        if (File.Exists(Application.StartupPath + "\\7z.exe"))
                        {
                            string _compressfilename = System.IO.Path.GetFileNameWithoutExtension(_path) + ".7z";
                            string _compressdir = System.IO.Path.GetDirectoryName(_path);
                            string _compressfile = _compressdir + (_compressdir.RLTrim().EndsWith("\\") ? "" : "\\") + _compressfilename;

                            if (File.Exists(_compressfile))
                            {
                                try { File.Delete(_compressfile); }
                                catch { }
                            }

                            string _batfilename = Application.StartupPath + "\\archiver.bat";
                            string _batfilecontents = "7z a \"" + _compressfile + "\" \"" + _path + "\"";
                            FileInfo _batfile = Materia.WriteToFile(_batfilename, _batfilecontents);
                            string _error = "";

                            if (_batfile != null)
                            {
                                Process _process = new Process();
                                _process.StartInfo.FileName = _batfilename;
                                _process.StartInfo.CreateNoWindow = VisualBasic.CBool(_processwindowstyle == ProcessWindowStyle.Hidden);
                                _process.StartInfo.WindowStyle = _processwindowstyle;
                                _process.StartInfo.UseShellExecute = false;
                                _process.StartInfo.RedirectStandardError = true;
                                _process.Start();

                                while (!_process.HasExited) Application.DoEvents();

                                if (_process.StandardError != null)
                                {
                                    try { _error = _process.StandardError.ReadToEnd(); }
                                    catch { _error = ""; }
                                }
                                else _error = "";

                                _process.Dispose(); Materia.RefreshAndManageCurrentProcess();

                                try { _batfile.Delete(); }
                                catch { }
                            }
                            else _error = "Can't create executable archive batch file.";

                            _error = _error.Replace("\nThe handle is invalid.", "").Replace("The handle is invalid.", "");
                            if (!String.IsNullOrEmpty(_error.RLTrim())) Materia.LogError(_error, "7Zip Archiving Error");

                            if (File.Exists(_compressfile))
                            {
                                _archivedpath = _compressfile;

                                if (_archivingmethod == ArchivingMethodEnum.Insert)
                                {
                                    try
                                    {
                                        for (int i = 0; i <= 30; i++)
                                        {
                                            Thread.Sleep(10); Application.DoEvents();
                                        }

                                        if (System.IO.Path.HasExtension(_path)) File.Delete(_path);
                                        else Directory.Delete(_path, true);
                                    }
                                    catch { }
                                }

                                RemoveResourceApplications(); _file = new FileInfo(_archivedpath);
                            }
                        }
                        break;
                    case ArchivingToolEnum.WinRar :
                        if (File.Exists(Application.StartupPath + "\\WinRar.exe"))
                        {
                            string _compressfilename = System.IO.Path.GetFileNameWithoutExtension(_path) + ".rar";
                            string _compressdir = System.IO.Path.GetDirectoryName(_path);
                            string _compressfile = _compressdir + (_compressdir.RLTrim().EndsWith("\\") ? "" : "\\") + _compressfilename;

                            if (File.Exists(_compressfile))
                            {
                                try { File.Delete(_compressfile); }
                                catch { }
                            }

                            string _batfilename = Application.StartupPath + "\\archiver.bat";
                            string _batfilecontents = "WinRar a -ep \"" + _compressfile + "\" \"" + _path + "\"";
                            FileInfo _batfile = Materia.WriteToFile(_batfilename, _batfilecontents);
                            string _error = "";

                            if (_batfile != null)
                            {
                                Process _process = new Process();
                                _process.StartInfo.FileName = _batfilename;
                                _process.StartInfo.CreateNoWindow = VisualBasic.CBool(_processwindowstyle == ProcessWindowStyle.Hidden);
                                _process.StartInfo.WindowStyle = _processwindowstyle;
                                _process.StartInfo.UseShellExecute = false;
                                _process.StartInfo.RedirectStandardError = true;
                                _process.Start();

                                while (!_process.HasExited) Application.DoEvents();

                                if (_process.StandardError != null)
                                {
                                    try { _error = _process.StandardError.ReadToEnd(); }
                                    catch { _error = ""; }
                                }
                                else _error = "";

                                _process.Dispose(); Materia.RefreshAndManageCurrentProcess();

                                try { _batfile.Delete(); }
                                catch { }
                            }
                            else _error = "Can't create executable archive batch file.";

                            _error = _error.Replace("\nThe handle is invalid.", "").Replace("The handle is invalid.", "");
                            if (!String.IsNullOrEmpty(_error.RLTrim())) Materia.LogError(_error, "WinRar Archiving Error");

                            if (File.Exists(_compressfile))
                            {
                                _archivedpath = _compressfile;

                                if (_archivingmethod == ArchivingMethodEnum.Insert)
                                {
                                    try
                                    {
                                        for (int i = 0; i <= 30; i++)
                                        {
                                            Thread.Sleep(10); Application.DoEvents();
                                        }

                                        if (System.IO.Path.HasExtension(_path)) File.Delete(_path);
                                        else Directory.Delete(_path, true);
                                    }
                                    catch { }
                                }

                                RemoveResourceApplications(); _file = new FileInfo(_archivedpath);
                            }
                        }
                        break;
                    default : break;
                }
            }

            return _file;
        }

        /// <summary>
        ///  Performs file compression using selected archiving tool, file(s) will just be copied into the archive file.
        /// </summary>
        /// <param name="path">File / directory path to archive</param>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="Archiver 02" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="Archiver 01" language="vbnet" />
        /// </example> 
        public static FileInfo CompressAdd(string path)
        { return CompressAdd(path, ArchivingToolEnum.SevenZip); }

        /// <summary>
        ///  Performs file compression using selected archiving tool, file(s) will just be copied into the archive file.
        /// </summary>
        /// <param name="path">File / directory path to archive</param>
        /// <param name="archivingtool">Archiving tool to use</param>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="Archiver 03" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="Archiver 03" language="vbnet" />
        /// </example> 
        public static FileInfo CompressAdd(string path, ArchivingToolEnum archivingtool)
        {
            FileInfo _file = null;

            Archiver _archiver = new Archiver(path, archivingtool);
            _archiver.ArchivingMethod = ArchivingMethodEnum.Append;
            _file = _archiver.Archive(); _archiver.Dispose();

            return _file;
        }

        /// <summary>
        ///  Performs file compression using selected archiving tool, file(s) will be inserted directly to the archive file.
        /// </summary>
        /// <param name="path">File / directory path to archive</param>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="Archiver 04" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="Archiver 04" language="vbnet" />
        /// </example> 
        public static FileInfo CompressInsert(string path)
        { return CompressInsert(path, ArchivingToolEnum.SevenZip); }

        /// <summary>
        ///  Performs file compression using selected archiving tool, file(s) will be inserted directly to the archive file.
        /// </summary>
        /// <param name="path">File / directory path to archive</param>
        /// <param name="archivingtool">Archiving tool to use</param>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <summary>
        ///  Performs file compression using selected archiving tool, file(s) will be inserted directly to the archive file.
        /// </summary>
        /// <param name="path">File / directory path to archive</param>
        /// <returns>A System.IO.FileInfo object that contains the archived output file information. Returns nothing if error has been encountered during archive routines.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="Archiver 05" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="Archiver 05" language="vbnet" />
        /// </example> 
        public static FileInfo CompressInsert(string path, ArchivingToolEnum archivingtool)
        {
            FileInfo _file = null;

            Archiver _archiver = new Archiver(path, archivingtool);
            _archiver.ArchivingMethod = ArchivingMethodEnum.Insert;
            _file = _archiver.Archive(); _archiver.Dispose();

            return _file;
        }

        /// <summary>
        ///  Performs file extraction from a compressed file into the specified destination folder using the selected archiving tool.
        /// </summary>
        /// <param name="filename">Compressed file's filename</param>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public static bool Decompress(string filename)
        { return Decompress(filename, ArchivingToolEnum.SevenZip); }

        /// <summary>
        ///  Performs file extraction from a compressed file into the specified destination folder using the selected archiving tool.
        /// </summary>
        /// <param name="filename">Compressed file's filename</param>
        /// <param name="destination">Destination path for the extracted file(s)</param>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public static bool Decompress(string filename, string destination)
        { return Decompress(filename, destination, ArchivingToolEnum.SevenZip); }

        /// <summary>
        /// Performs file extraction from a compressed file into the specified destination folder using the selected archiving tool.
        /// </summary>
        /// <param name="filename">Compressed file's filename</param>
        /// <param name="archivingtool">Archiving tool to use</param>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public static bool Decompress(string filename, ArchivingToolEnum archivingtool)
        {
            string _directory = System.IO.Path.GetDirectoryName(filename);
            _directory += (_directory.RLTrim().EndsWith("\\") ? "" : "\\") + System.IO.Path.GetFileNameWithoutExtension(filename);
            return Decompress(filename, _directory, archivingtool);
        }

        /// <summary>
        ///  Performs file extraction from a compressed file into the specified destination folder using the selected archiving tool.
        /// </summary>
        /// <param name="filename">Compressed file's filename</param>
        /// <param name="destination">Destination path for the extracted file(s)</param>
        /// <param name="archivingtool">Archiving tool to use</param>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public static bool Decompress(string filename, string destination, ArchivingToolEnum archivingtool)
        {
            bool _decompressed = false;

            Archiver _archiver = new Archiver(filename, archivingtool);
            _decompressed = _archiver.Extract(destination);
            _archiver.Dispose();

            return _decompressed;
        }

        /// <summary>
        /// Performs archive extraction using the chosen archiving tool.
        /// </summary>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public bool Extract()
        {
            string _directory = System.IO.Path.GetDirectoryName(_path);
            _directory += (_directory.RLTrim().EndsWith("\\") ? "" : "\\") + System.IO.Path.GetFileNameWithoutExtension(_path);
            return Extract(_directory);
        }

        /// <summary>
        /// Performs archive extraction using the chosen archiving tool.
        /// </summary>
        /// <param name="destination">Destination path for the extracted file(s)</param>
        /// <returns>A boolean value that determines whether file extraction was successful or not.</returns>
        public bool Extract(string destination)
        {
            bool _extracted = false; string _error = ""; ExtractResourceApplications();
            string _archiverfile = "";

            switch (_archivingtool)
            {
                case ArchivingToolEnum.SevenZip:
                    _archiverfile = Application.StartupPath + "\\7z.exe";

                    if (File.Exists(_archiverfile))
                    {
                        Process _process = new Process();
                        _process.StartInfo.Arguments = "e \"" + _path + "\" -o\"" + destination + "\" *.* -r";
                        _process.StartInfo.FileName = _archiverfile;
                        _process.StartInfo.CreateNoWindow = VisualBasic.CBool(_processwindowstyle == ProcessWindowStyle.Hidden);
                        _process.StartInfo.WindowStyle = _processwindowstyle;
                        _process.StartInfo.RedirectStandardError = true;
                        _process.StartInfo.UseShellExecute = false;
                        _process.Start();

                        while (!_process.HasExited) Application.DoEvents();

                        if (_process.StandardError != null)
                        {
                            try { _error = _process.StandardError.ReadToEnd(); }
                            catch { _error = ""; }
                        }
                        else _error = "";
                        _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                    }
                    break;
                case ArchivingToolEnum.WinRar:
                    _archiverfile = Application.StartupPath + "\\WinRar.exe";

                    if (File.Exists(_archiverfile))
                    {
                        Process _process = new Process();
                        _process.StartInfo.Arguments = "e \"" + _path + "\" *.* \"" + destination + "\"";
                        _process.StartInfo.FileName = _archiverfile;
                        _process.StartInfo.CreateNoWindow = VisualBasic.CBool(_processwindowstyle == ProcessWindowStyle.Hidden);
                        _process.StartInfo.WindowStyle = _processwindowstyle;
                        _process.StartInfo.RedirectStandardError = true;
                        _process.StartInfo.UseShellExecute = false;
                        _process.Start();

                        while (!_process.HasExited) Application.DoEvents();

                        if (_process.StandardError != null)
                        {
                            try { _error = _process.StandardError.ReadToEnd(); }
                            catch { _error = ""; }
                        }
                        else _error = "";
                        _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                    }
                    break;
                default: break;
            }

            _extracted = String.IsNullOrEmpty(_error.Replace("\nThe handle is invalid.", "").Replace("The handle is invalid.", "").RLTrim());
            if (!_extracted) Materia.LogError(_error.Replace("\nThe handle is invalid.", "").Replace("The handle is invalid.", "").RLTrim(), Enum.GetName(typeof(ArchivingToolEnum), _archivingtool) + " File Extraction Error");
            RemoveResourceApplications();

            return _extracted;
        }

        private static void ExtractResourceApplications()
        {
            RemoveResourceApplications(); string _curdir = Application.StartupPath + "\\Archiving Tools";

            string[] _applications = new string[] { "7z.exe", "7z.dll", "7zG.exe", "WinRar.exe" };

            foreach (string _application in _applications)
            {
                string[] _names = _application.Split(new char[]{ '.'});

                if (_names.Length >= 2)
                {
                    string _name = _names[0]; string _extension = _names[1];
                    FileInfo _file = null;

                    try
                    {
                        switch (_name)
                        {
                            case "7z":
                                if (_extension.RLTrim() == "dll") _file = Properties.Resources.SevenZDll.ToFileObject(_extension, _curdir);
                                else if (_extension.RLTrim() == "exe") _file = Properties.Resources.SevenZ.ToFileObject(_extension, _curdir);
                                break;
                            case "7zG":
                                _file = Properties.Resources.SevenZG.ToFileObject(_extension, _curdir); break;
                            case "WinRar":
                                _file = Properties.Resources.WinRAR.ToFileObject(_extension, _curdir); break;
                            default: break;
                        }
                    }
                    catch { _file = null; }

                    if (_file!= null)
                    {
                        try
                        { File.Copy(_file.FullName, _application); }
                        catch { }

                        if (Directory.Exists(_curdir))
                        {
                            if (File.Exists(_curdir + "\\" + _application) &&
                                !File.Exists(Application.StartupPath + "\\" + _application))
                            {
                                try { File.Copy(_curdir + "\\" + _application, Application.StartupPath + "\\" + _application); }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveResourceApplications()
        {
            string _curdir = Application.StartupPath + "\\Archiving Tools";

            if (Directory.Exists(_curdir))
            {
                try { Directory.Delete(_curdir, true); }
                catch { }
            }

            string[] _applications = new string[] { "7z.exe", "7z.dll", "7zG.exe", "WinRar.exe" };

            foreach (string _application in _applications)
            {
                string _path = Application.StartupPath + "\\" + _application;
                if (File.Exists(_path))
                {
                    try { File.Delete(_path); }
                    catch { }
                }
            }

            Materia.RefreshAndManageCurrentProcess();
        }

        /// <summary>
        /// Runs the integrated 7z archiver with the specified command line arguments.
        /// </summary>
        /// <param name="commandlineargs">Command line arguments</param>
        /// <returns>Returns true if operation is successful otherwise false.</returns>
        public static bool SevenZ(string commandlineargs)
        {
            bool _succeeded = false; ExtractResourceApplications();

            string _directory = Application.StartupPath + "\\Archiving Tools";
            string[] _files = new string[] { "7z.exe", "7z.dll", "7zG.exe" };

            bool _runnable = true;

            foreach (string _file in _files)
            {
                string _path = _directory + "\\" + _file;
                _runnable = _runnable && (File.Exists(_path));
                if (!_runnable) break;
            }

            if (_runnable)
            {
                string _error = "";

                Process _process = new Process();
                _process.StartInfo.Arguments = commandlineargs;
                _process.StartInfo.FileName = _directory + "\\7z.exe";
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.UseShellExecute = false;
                _process.Start();

                while (!_process.HasExited) Application.DoEvents();

                if (_process.StandardError != null)
                {
                    try { _error = _process.StandardError.ReadToEnd(); }
                    catch { _error = ""; }
                }
                else _error = "";

                _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                _succeeded = String.IsNullOrEmpty(_error.RLTrim());
            }

            RemoveResourceApplications();

            return _succeeded;
        }

        /// <summary>
        /// Runs the integrated WinRar archiver with the specified command line arguments.
        /// </summary>
        /// <param name="commandlineargs">Command line arguments</param>
        /// <returns>Returns true if operation is successful otherwise false.</returns>
        public static bool WinRar(string commandlineargs)
        {
            bool _succeeded = false; ExtractResourceApplications();

            string _path = Application.StartupPath + "\\Archiving Tools\\WinRar.exe";
            if (File.Exists(_path))
            {
                string _error = "";

                Process _process = new Process();
                _process.StartInfo.Arguments = commandlineargs;
                _process.StartInfo.FileName = _path;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.UseShellExecute = false;
                _process.Start();

                while (!_process.HasExited) Application.DoEvents();

                if (_process.StandardError != null)
                {
                    try { _error = _process.StandardError.ReadToEnd(); }
                    catch { _error = ""; }
                }
                else _error = "";

                _process.Dispose(); Materia.RefreshAndManageCurrentProcess();
                _succeeded = String.IsNullOrEmpty(_error.RLTrim());
            }

            RemoveResourceApplications();

            return _succeeded;
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
                    RemoveResourceApplications();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }
}
