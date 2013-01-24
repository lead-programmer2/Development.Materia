#region "imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace Development.Materia.Net
{

    #region "enumerations"

    /// <summary>
    /// Upload event invokation enumerations.
    /// </summary>
    public enum UploadInvokations
    {
        /// <summary>
        /// Actual upload events.
        /// </summary>
        EventRaiser,
        /// <summary>
        /// Upload failure events.
        /// </summary>
        FileUploadFailedRaiser,
        /// <summary>
        /// Upload calculation events.
        /// </summary>
        CalculatingFileNrRaiser
    }

    /// <summary>
    /// Upload transition enumerations.
    /// </summary>
    public enum UploadTransitions
    {
        /// <summary>
        /// While trying to calculate total size of upload file.
        /// </summary>
        CalculationFileSizesStarted,
        /// <summary>
        /// After upload file size has been determined.
        /// </summary>
        FileSizesCalculationComplete,
        /// <summary>
        /// Upload file deletion after successful upload process.
        /// </summary>
        DeletingFileAfterUpload,
        /// <summary>
        /// Upload file deletion when process has been cancelled.
        /// </summary>
        DeletingFilesAfterCancel,
        /// <summary>
        /// Attempting to communicate with the upload server.
        /// </summary>
        FileUploadAttempting,
        /// <summary>
        ///  File upload has been started.
        /// </summary>
        FileUploadStarted,
        /// <summary>
        /// File upload has been terminated.
        /// </summary>
        FileUploadStopped,
        /// <summary>
        /// File upload finished successfully.
        /// </summary>
        FileUploadSucceeded,
        /// <summary>
        /// File upload is progressing.
        /// </summary>
        ProgressChanged
    }

    #endregion

    /// <summary>
    /// FTP uploder class with progress reporting features.
    /// </summary>
    public class Uploader : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of Uploader.
        /// </summary>
        /// <param name="address">Uploading FTP address</param>
        public Uploader(string address) : this(address, null)
        { }

        /// <summary>
        /// Creates a new instance of Uploader.
        /// </summary>
        /// <param name="address">Uploading FTP address</param>
        /// <param name="uid">FTP server logon user id</param>
        /// <param name="pwd">FTP server logon password</param>
        public Uploader(string address, string uid, string pwd) : this(address, null)
        {
            if (!String.IsNullOrEmpty(uid.RLTrim()) ||
                !String.IsNullOrEmpty(pwd.RLTrim())) _credential = new NetworkCredential(uid, pwd);
        }

        /// <summary>
        /// Creates a new instance of Uploader.
        /// </summary>
        /// <param name="address">Uploading FTP address</param>
        /// <param name="credential">FTP server logon credential</param>
        public Uploader(string address, NetworkCredential credential)
        {
            _address = address; _credential = credential;
            _uploaderworker.WorkerReportsProgress = true;
            _uploaderworker.WorkerSupportsCancellation = true;
            SetPacketSize(4096); _stopwatchcycle = 5;

            _uploads = new UploadFileInfoCollection(this);

            _uploaderworker.DoWork += new DoWorkEventHandler(_uploaderworker_DoWork);
            _uploaderworker.ProgressChanged += new ProgressChangedEventHandler(_uploaderworker_ProgressChanged);
            _uploaderworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_uploaderworker_RunWorkerCompleted);
        }

        #endregion

        #region "events"

        /// <summary>
        /// Event handler that is invoked when upload routine fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        public delegate void FileUploadFailedEventHandler(object sender, Exception ex);

        /// <summary>
        /// Event handler that is invoked upon upload file size calculation routines.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filenumber"></param>
        public delegate void FileSizeCalculationEventHandler(object sender, Int32 filenumber);

        /// <summary>
        /// Occurs when the calculation of the file sizes has started
        /// </summary>
        public event EventHandler CalculationFileSizesStarted;

        /// <summary>
        /// Calls the CalculationFileSizesStarted event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnCalculationFileSizesStarted(EventArgs e)
        {
            if (CalculationFileSizesStarted != null) CalculationFileSizesStarted(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has been canceled by the user
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Calls the Cancelled event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnCancelled(EventArgs e)
        {
            if (Cancelled != null) Cancelled(this, e);
        }

        /// <summary>
        /// Occurs when the user has requested to cancel the Uploads
        /// </summary>
        public event EventHandler CancelRequested;

        /// <summary>
        /// Calls the CancelRequested event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnCancelRequested(EventArgs e)
        {
            if (CancelRequested != null) CancelRequested(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has been completed (without canceling it)
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Calls the Completed event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null) Completed(this, e);
        }

        /// <summary>
        /// Occurs when the calculation of the file sizes has started
        /// </summary>
        public event FileSizeCalculationEventHandler CalculatingFileSize;

        /// <summary>
        /// Calls the CalculatingFileSize event.
        /// </summary>
        /// <param name="e">Uploading file size.</param>
        protected virtual void OnCalculatingFileSize(Int32 e)
        {
            if (CalculatingFileSize != null) CalculatingFileSize(this, e);
        }

        /// <summary>
        /// Occurs when the FileUploader attempts to get a web response to upload the file.
        /// </summary>
        public event EventHandler FileUploadAttempting;

        /// <summary>
        /// Calls the FileUploadAttempting event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnFileUploadAttempting(EventArgs e)
        {
            if (FileUploadAttempting != null) FileUploadAttempting(this, e);
        }

        /// <summary>
        /// Occurs when a file upload has been completed unsuccessfully
        /// </summary>
        public event FileUploadFailedEventHandler FileUploadFailed;

        /// <summary>
        /// Calls the FileUploadFailed event.
        /// </summary>
        /// <param name="e">Exception encountered</param>
        protected virtual void OnFileUploadFailed(Exception e)
        {
            if (FileUploadFailed != null) FileUploadFailed(this, e);
        }

        /// <summary>
        /// Occurs when a file upload has started
        /// </summary>
        public event EventHandler FileUploadStarted;

        /// <summary>
        /// Calls the FileUploadStarted event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnFileUploadStarted(EventArgs e)
        {
            if (FileUploadStarted != null) FileUploadStarted(this, e);
        }

        /// <summary>
        /// Occurs when a file upload has stopped
        /// </summary>
        public event EventHandler FileUploadStopped;

        /// <summary>
        /// Calls the FileUploadStopped  event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnFileUploadStopped(EventArgs e)
        {
            if (FileUploadStopped != null) FileUploadStopped(this, e);
        }

        /// <summary>
        /// Occurs when a file upload has been completed successfully
        /// </summary>
        public event EventHandler FileUploadSucceeded;

        /// <summary>
        /// Calls the FileUploadSucceeded event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnFileUploadSucceeded(EventArgs e)
        {
            if (FileUploadSucceeded != null) FileUploadSucceeded(this, e);
        }

        /// <summary>
        /// Occurs when the FileUploader attempts to get a web response to upload the file
        /// </summary>
        public event EventHandler FileSizesCalculationComplete;

        /// <summary>
        /// Calls the FileSizesCalculationComplete event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnFileSizesCalculationComplete(EventArgs e)
        {
            if (FileSizesCalculationComplete != null) FileSizesCalculationComplete(this, e);
        }

        /// <summary>
        /// Occurs when the busy state of the FileUploader has changed
        /// </summary>
        public event EventHandler IsBusyChanged;

        /// <summary>
        /// Calls the IsBusyChanged event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnIsBusyChanged(EventArgs e)
        {
            if (IsBusyChanged != null) IsBusyChanged(this, e);
        }

        /// <summary>
        /// Occurs when the pause state of the FileUploader has changed
        /// </summary>
        public event EventHandler IsPausedChanged;

        /// <summary>
        /// Calls the IsPausedChanged event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnIsPausedChanged(EventArgs e)
        {
            if (IsPausedChanged != null) IsPausedChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has been paused.
        /// </summary>
        public event EventHandler Paused;

        /// <summary>
        /// Calls the Paused event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnPaused(EventArgs e)
        {
            if (Paused != null) Paused(this, e);
        }

        /// <summary>
        /// Occurs every time a block of data has been Uploaded
        /// </summary>
        public event EventHandler ProgressChanged;

        /// <summary>
        /// Calls the ProgressChanged event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null) ProgressChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has been resumed
        /// </summary>
        public event EventHandler Resumed;

        /// <summary>
        /// Calls the Resumed event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnResumed(EventArgs e)
        {
            if (Resumed != null) Resumed(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Calls the Started event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnStarted(EventArgs e)
        {
            if (Started != null) Started(this, e);
        }

        /// <summary>
        /// Occurs when the either the busy or pause state of the FileUploader have changed
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Calls the StateChanged event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null) StateChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file Uploading has been stopped by either cancellation or completion
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Calls the Stopped event.
        /// </summary>
        /// <param name="e">Base data that contains the event's information.</param>
        protected virtual void OnStopped(EventArgs e)
        {
            if (Stopped != null) Stopped(this, e);
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _address = "";

        /// <summary>
        /// Gets or sets the FTP uploading address.
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Gets whether the uploader can resume the upload process or not.
        /// </summary>
        public bool CanResume
        {
            get { return IsBusy && (!IsPaused) && (!_uploaderworker.CancellationPending); }
        }

        /// <summary>
        /// Gets whether the uploader can start a new process or not.
        /// </summary>
        public bool CanStart
        {
            get { return (!IsBusy); }
        }

        /// <summary>
        /// Gets whether the uploader can be cancelled or not.
        /// </summary>
        public bool CanStop
        {
            get { return IsBusy && (!_uploaderworker.CancellationPending); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkCredential _credential = null;

        /// <summary>
        /// Gets or sets the FTP server logon credential information.
        /// </summary>
        public NetworkCredential Credenital
        {
            get { return _credential; }
            set { _credential = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _currentlyuploaded = 0;

        /// <summary>
        /// Gets the total bytes uploaded for the current uploading file. To get the overall upload size refer to TotalUploaded value.
        /// </summary>
        public double CurrentlyUploaded
        {
            get { return _currentlyuploaded; }
        }

        /// <summary>
        /// Gets the current uploading file progress percentage.
        /// </summary>
        public double CurrentUploadPercentage
        {
            get { return GetCurrentUploadPercentage(0); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _currentuploadsize = 0;

        /// <summary>
        /// Gets the size of the current uploading file size in bytes.
        /// </summary>
        public double CurrentUploadSize
        {
            get { return _currentuploadsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _currentfile = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isbusy = false;

        /// <summary>
        /// Gets whether the uploader is currently busy or not.
        /// </summary>
        public bool IsBusy
        {
            get { return _isbusy; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _iscancelled = false;

        /// <summary>
        /// Gets whether the upload process has ben cancelled or not.
        /// </summary>
        public bool IsCancelled
        {
            get { return _iscancelled; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _ispaused = false;

        /// <summary>
        /// Gets whether the uploader is currently at paused state or not.
        /// </summary>
        public bool IsPaused
        {
            get { return _ispaused; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _packetsize = 4096;

        /// <summary>
        /// Gets the size of the blocks that will be uploaded.
        /// </summary>
        public int PacketSize
        {
            get { return _packetsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _stopwatchcycle = 0;

        /// <summary>
        /// Gets or sets the amount of blocks that need to be uploaded before the progress speed is re-calculated. Note: setting this to a low value might decrease the accuracy of the calculation. Default value is 5.
        /// </summary>
        public int StopWatchCycle
        {
            get { return _stopwatchcycle; }
            set
            {
                if (value > 0) _stopwatchcycle = value;
                else throw new Exception("Stop watch cycle cannot be less than or equal to zero.");
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _supportsprogress = false;

        /// <summary>
        /// Gets or sets whether the uploader supports progress reports or not. When enabled, uploader will have to make computations of the uploading files before proceeding with the upload process. Default value is set to False.
        /// </summary>
        public bool SupportsProgress
        {
            get { return _supportsprogress; }
            set
            {
                if (IsBusy) throw new Exception("Can't set the progress report support while the uploader is running.");
                else _supportsprogress = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _totaluploaded = 0;

        /// <summary>
        /// Gets the total amount of bytes being uploaded.
        /// </summary>
        public double TotalUploaded
        {
            get { return _totaluploaded; }
        }

        /// <summary>
        /// Gets the total upload percentage.
        /// </summary>
        public double TotalUploadPercentage
        {
            get { return GetTotalUploadPercentage(0); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _totaluploadsize = 0;

        /// <summary>
        /// Gets the total size of all files together. Correct value will be returned if SupportsProgress is set otherwise returns 0.
        /// </summary>
        public double TotalUploadSize
        {
            get { return _totaluploadsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ManualResetEvent _trigger = new ManualResetEvent(true);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BackgroundWorker _uploaderworker = new BackgroundWorker();


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UploadFileInfoCollection _uploads = null;

        /// <summary>
        /// Gets the lists of upload files.
        /// </summary>
        public UploadFileInfoCollection Uploads
        {
            get { return _uploads; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _uploadspeed = 0;

        /// <summary>
        /// Gets the current upload speed in bytes.
        /// </summary>
        public int UploadSpeed
        {
            get { return _uploadspeed; }
        }

        #endregion

        #region "methods"

        private void CalculateFileSizes()
        {
            FireEventFromWorker(UploadTransitions.CalculationFileSizesStarted);
            _totaluploadsize = 0;

            for (int i = 0; i <= (Uploads.Count - 1); i++)
            {
                _uploaderworker.ReportProgress((int) UploadInvokations.CalculatingFileNrRaiser, i + 1);
                try
                {
                    UploadFileInfo _upload = Uploads[i];

                    if (File.Exists(_upload.Path))
                    {
                        FileInfo _file = new FileInfo(_upload.Path);
                        _totaluploadsize += _file.Length;
                    }
                }
                catch { }
            }

            FireEventFromWorker(UploadTransitions.FileSizesCalculationComplete);
        }

        private void FireEventFromWorker(UploadTransitions triggeredevent)
        { _uploaderworker.ReportProgress((int) UploadInvokations.EventRaiser, triggeredevent); }


        /// <summary>
        /// Formats the amount of bytes to a more readible notation with binary notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <returns>Readable notation for the specied value.</returns>
        public static string FormatSizeBinary(long size)
        { return FormatSizeBinary(size, 0); }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with binary notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <param name="decimals">The amount of decimals for the notation</param>
        /// <returns>Readable notation for the specied value.</returns>
        public static string FormatSizeBinary(long size, int decimals)
        {
            string[] _sizes = new string[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };
            double _formattedsize = size; int _sizeindex = 0;
            while (_formattedsize >= 1024 && _sizeindex < _sizes.Length)
            {
                _formattedsize /= 1024; _sizeindex += 1;
            }
            return Math.Round(_formattedsize, decimals).ToString() + _sizes[_sizeindex];
        }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with decimal notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <returns>Readable notation for the specied value.</returns>
        public static string FormatSizeDecimal(long size)
        { return FormatSizeDecimal(size, 0); }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with decimal notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <param name="decimals">The amount of decimals for the notation</param>
        /// <returns>Readable notation for the specied value.</returns>
        public static string FormatSizeDecimal(long size, int decimals)
        {
            string[] _sizes = new string[] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double _formattedsize = size; int _sizeindex = 0;
            while (_formattedsize >= 1000 && _sizeindex < _sizes.Length)
            {
                _formattedsize /= 1000; _sizeindex += 1;
            }
            return Math.Round(_formattedsize, decimals).ToString() + _sizes[_sizeindex];
        }

        /// <summary>
        ///  Gets the current uploading file progress percentage.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>Uploading file progress percentage</returns>
        public double GetCurrentUploadPercentage(int decimals)
        {
            if (!SupportsProgress) return 0;
            else
            {
                if (CurrentUploadSize > 0) return Math.Round((CurrentlyUploaded / CurrentUploadSize) * 100, decimals);
                else return 0;
            }
        }

        /// <summary>
        /// Gets the total upload percentage.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>Total upload progress percentage.</returns>
        public double GetTotalUploadPercentage(int decimals)
        {
            if (!SupportsProgress) return 0;
            else
            {
                if (TotalUploadSize > 0) return Math.Round((TotalUploaded / TotalUploadSize) * 100, decimals);
                else return 0;
            }
        }

        private static bool RemoteCertificationValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { return true; }

        /// <summary>
        /// Sets the uploader's busy state.
        /// </summary>
        /// <param name="busy">A value that determines whether the Uploader is currently busy or not.</param>
        protected virtual void SetBusy(bool busy)
        {
            if (IsBusy != busy)
            {
                _isbusy = busy;
                if (IsBusy)
                {
                    _totaluploaded = 0; _uploaderworker.RunWorkerAsync();
                    OnStarted(new EventArgs());
                    OnIsBusyChanged(new EventArgs());
                    OnStateChanged(new EventArgs());
                }
                else
                {
                    _ispaused = false; _uploaderworker.CancelAsync();
                    OnCancelRequested(new EventArgs());
                    OnStateChanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Sets the uploader's allocated packet size block.
        /// </summary>
        /// <param name="packets">Maximum upload-speed limit.</param>
        public virtual void SetPacketSize(int packets)
        {
            if (packets > 0) _packetsize = packets;
            else throw new Exception("Packet size cannot be less than or equal to zero.");
        }

        /// <summary>
        /// Sets the current uploader's paused state.
        /// </summary>
        /// <param name="paused">A value that determines whether the Uploader is marked as paused or not.</param>
        protected virtual void SetPaused(bool paused)
        {
            if (IsBusy)
            {
                if (IsPaused != paused)
                {
                    _ispaused = paused;
                    if (IsPaused)
                    {
                        _trigger.Reset(); OnPaused(new EventArgs());
                    }
                    else
                    {
                        _trigger.Set(); OnResumed(new EventArgs());
                    }
                    OnIsPausedChanged(new EventArgs());
                    OnStateChanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Starts the upload process.
        /// </summary>
        public void Start()
        { SetBusy(true); }

        /// <summary>
        /// Pause the upload process.
        /// </summary>
        public void Pause()
        { SetPaused(true); }

        /// <summary>
        /// Resumes the upload process.
        /// </summary>
        public void Resume()
        { SetPaused(false); }

        /// <summary>
        /// Cancels the currently running upload process.
        /// </summary>
        public void Stop()
        { _iscancelled = true; SetBusy(false); }

        private void UploadFile(int index)
        {
            _currentuploadsize = 0; FireEventFromWorker(UploadTransitions.FileUploadAttempting);

            UploadFileInfo _file = Uploads[index]; double _size = 0;
            byte[] _readbytes = new byte[PacketSize]; int _currentpackagesize = 0;
            FileInfo _fileinfo = new FileInfo(_file.Path); FileStream _filestream = null;
            Stopwatch _speedtimer = new Stopwatch(); Stream _stream = null;
            Int32 _readings = 0; Exception _ex = null; int _packet = PacketSize;

            try { _size = _fileinfo.Length; }
            catch { }

            FtpWebRequest _request = null; FtpWebResponse _response = null;

            try
            {
                string _ftpaddress = Path.GetDirectoryName(Address).ToLower().Replace("ftp:\\", "ftp://").Replace("\\", "/") + "/" + Path.GetFileName(_file.Path);
                _request = (FtpWebRequest) WebRequest.Create(_ftpaddress);
                if (_credential != null) _request.Credentials = _credential;
                _request.KeepAlive = true; _request.UseBinary = true;
                _request.Method = WebRequestMethods.Ftp.UploadFile;
                _filestream = new FileStream(_file.Path, FileMode.Open);
                _request.ContentLength = VisualBasic.CLng(_size); _stream = _request.GetRequestStream();
            }
            catch (Exception ex) { _ex = ex; }

            _currentuploadsize = _size; FireEventFromWorker(UploadTransitions.FileUploadStarted);

            if (_ex != null) _uploaderworker.ReportProgress((int) UploadInvokations.FileUploadFailedRaiser, _ex);
            else
            {
                _currentlyuploaded = 0; _currentpackagesize = 0;

                do
                {
                    if (_uploaderworker.CancellationPending)
                    {
                        _filestream.Close(); _stream.Close(); _speedtimer.Stop();
                        if (_response != null) _response.Close(); 
                        return;
                    }

                    _trigger.WaitOne(); _speedtimer.Start();
                    if (_stream == null &&
                        _response != null) _stream = _response.GetResponseStream();


                    if (_readbytes.Length < PacketSize) _packet = _readbytes.Length;
                    else _packet = PacketSize;

                    _currentpackagesize = _filestream.Read(_readbytes, 0, _packet);
                    _currentlyuploaded += _currentpackagesize; _totaluploaded += _currentpackagesize;
                    FireEventFromWorker(UploadTransitions.ProgressChanged);

                    _stream.Write(_readbytes, 0, _currentpackagesize); _readings += 1;

                    if (_readings >= StopWatchCycle)
                    {
                        _uploadspeed = VisualBasic.CInt(_packet * StopWatchCycle * 1000 / (_speedtimer.ElapsedMilliseconds + 1));
                        _speedtimer.Reset(); _readings = 0;
                    }

                } while (_currentpackagesize != 0);

                _stream.Close(); _filestream.Close(); _speedtimer.Stop();
                FireEventFromWorker(UploadTransitions.FileUploadSucceeded);
            }

            FireEventFromWorker(UploadTransitions.FileUploadStopped);
        }

        #endregion

        #region "backgroundworker events"

        private void _uploaderworker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SupportsProgress) CalculateFileSizes();

            int i = 0;

            while (i < Uploads.Count &&
                   !_uploaderworker.CancellationPending)
            {
                _currentfile = i; UploadFile(i);
                if (!_uploaderworker.CancellationPending) i += 1;
            }
        }

        private void _uploaderworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UploadInvokations _invokation = (UploadInvokations)e.ProgressPercentage;
            switch (_invokation)
            {
                case UploadInvokations.EventRaiser:
                    UploadTransitions _transition = (UploadTransitions)e.UserState;
                    switch (_transition)
                    {
                        case UploadTransitions.CalculationFileSizesStarted:
                            OnCalculationFileSizesStarted(new EventArgs()); break;
                        case UploadTransitions.FileSizesCalculationComplete:
                            OnFileSizesCalculationComplete(new EventArgs()); break;
                        case UploadTransitions.FileUploadAttempting:
                            OnFileUploadAttempting(new EventArgs()); break;
                        case UploadTransitions.FileUploadStarted:
                            OnFileUploadStarted(new EventArgs()); break;
                        case UploadTransitions.FileUploadStopped:
                            OnFileUploadStopped(new EventArgs()); break;
                        case UploadTransitions.FileUploadSucceeded:
                            OnFileUploadSucceeded(new EventArgs()); break;
                        case UploadTransitions.ProgressChanged:
                            OnProgressChanged(new EventArgs()); break;
                        default: break;
                    }
                    break;
                case UploadInvokations.FileUploadFailedRaiser:
                    OnFileUploadFailed((Exception)e.UserState); break;
                case UploadInvokations.CalculatingFileNrRaiser:
                    OnCalculatingFileSize((int)e.UserState); break;
                default: break;
            }
        }

        private void _uploaderworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetPaused(false); SetBusy(false);

            if (IsCancelled) OnCancelled(new EventArgs());
            else OnCompleted(new EventArgs());

            OnStopped(new EventArgs());
            OnIsBusyChanged(new EventArgs());
            OnStateChanged(new EventArgs());
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

    /// <summary>
    /// Uploaded file information class.
    /// </summary>
    public class UploadFileInfo
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of UploadFileInfo.
        /// </summary>
        /// <param name="path">The local path of the file to be uploaded.</param>
        public UploadFileInfo(string path)
        {
            _path = path; string[] _sections = path.Split(new char[] { '/' });
            _name = _sections[_sections.Length - 1];
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _name = "";

        /// <summary>
        /// Gets or sets the Uploading file's associated name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _path = "";

        /// <summary>
        /// Gets the currently initialized downnloading file's path.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion

        #region "overrides"

        /// <summary>
        /// Gets the name of the uploaded file.
        /// </summary>
        /// <returns>Upload file's name.</returns>
        public override string ToString()
        {
            return _name;
        }

        #endregion

    }

    /// <summary>
    /// ollection of uploaded file information.
    /// </summary>
    public class UploadFileInfoCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of UploadFileInfoCollection.
        /// </summary>
        /// <param name="uploader">Development.Materia.Net.Uploader object that owns the current collection.</param>
        public UploadFileInfoCollection(Uploader uploader)
        { _uploader = uploader; }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets the uloading file information in the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UploadFileInfo this[int index]
        {
            get { return (UploadFileInfo)List[index]; }
        }

        /// <summary>
        /// Gets a uploading file information with the specified name within the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UploadFileInfo this[string name]
        {
            get { return GetUploadInfoByName(name); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Uploader _uploader = null;

        /// <summary>
        /// Gets the current hosted Uploader.
        /// </summary>
        public Uploader Uploader
        {
            get { return _uploader; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new upload file information into the collection.
        /// </summary>
        /// <param name="path">Local file path</param>
        /// <returns>Newly added Development.Materia.Net.UploadFileInfo.</returns>
        public UploadFileInfo Add(string path)
        {
            UploadFileInfo _upload = new UploadFileInfo(path);
            int _index = Add(_upload); return (UploadFileInfo)List[_index];
        }

        /// <summary>
        /// Adds a new upload file information into the collection.
        /// </summary>
        /// <param name="upload">Upload file information</param>
        /// <returns>Index of the added Development.Materia.Net.UploadFileInfo in the collection.</returns>
        public int Add(UploadFileInfo upload)
        { return List.Add(upload); }

        /// <summary>
        /// Validates whether an specific upload file information with the specified name exists within the collection or not.
        /// </summary>
        /// <param name="name">Upload file name.</param>
        /// <returns>True if a Development.Materia.Net.UploadFileInfo with the specified name already exists within the collecton otherwise false.</returns>
        public bool Contains(string name)
        { return VisualBasic.CBool(GetUploadInfoByName(name) != null); }

        /// <summary>
        /// Returns whether the specified upload information already exists within the collection or not.
        /// </summary>
        /// <param name="upload">Development.Materia.Net.UploadFileInfo to evaluate.</param>
        /// <returns>True if the specified Development.Materia.Net.UploadFileInfo aready exists within the collection otherwise false.</returns>
        public bool Contains(UploadFileInfo upload)
        { return List.Contains(upload); }

        private UploadFileInfo GetUploadInfoByName(string name)
        {
            UploadFileInfo _info = null;

            foreach (UploadFileInfo upload in List)
            {
                if (upload.Name == name)
                {
                    _info = upload; break;
                }
            }

            return _info;
        }

        /// <summary>
        /// Removes a certain upload file information with the specified name from the collection.
        /// </summary>
        /// <param name="name">Upload file name.</param>
        public void Remove(string name)
        {
            UploadFileInfo _upload = GetUploadInfoByName(name);
            if (_upload != null) Remove(_upload);
        }

        /// <summary>
        ///  Removes the specified upload file information from the collection.
        /// </summary>
        /// <param name="upload">Development.Materia.Net.UploadFileInfo to remove.</param>
        public void Remove(UploadFileInfo upload)
        {
            if (Contains(upload)) List.Remove(upload);
        }

        #endregion

    }

}
