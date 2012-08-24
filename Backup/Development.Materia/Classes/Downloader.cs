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
    /// Download event invokation enumerations.
    /// </summary>
    public enum DownloadInvokations
    {
        /// <summary>
        /// Actual download events.
        /// </summary>
        EventRaiser,
        /// <summary>
        /// Download failure events.
        /// </summary>
        FileDownloadFailedRaiser,
        /// <summary>
        /// Download calculation events.
        /// </summary>
        CalculatingFileNrRaiser
    }

    /// <summary>
    /// Download transition enumerations.
    /// </summary>
    public enum DownloadTransitions
    {
        /// <summary>
        /// While trying to calculate total size of download file.
        /// </summary>
        CalculationFileSizesStarted,
        /// <summary>
        /// After download file size has been determined.
        /// </summary>
        FileSizesCalculationComplete,
        /// <summary>
        /// Download file deletion after successful download process.
        /// </summary>
        DeletingFileAfterDownload,
        /// <summary>
        /// Download file deletion when process has been cancelled.
        /// </summary>
        DeletingFilesAfterCancel,
        /// <summary>
        /// Attempting to communicate with the download server.
        /// </summary>
        FileDownloadAttempting,
        /// <summary>
        ///  File download has been started.
        /// </summary>
        FileDownloadStarted,
        /// <summary>
        /// File download has been terminated.
        /// </summary>
        FileDownloadStopped,
        /// <summary>
        /// File download finished successfully.
        /// </summary>
        FileDownloadSucceeded,
        /// <summary>
        /// File download is progressing.
        /// </summary>
        ProgressChanged
    }

    #endregion

    /// <summary>
    /// HTTP and FTP downloader with progress report feature.
    /// </summary>
    public class Downloader :IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of Downloader.
        /// </summary>
        public Downloader() : this(false)
        { }

        /// <summary>
        /// Creates a new instance of Downloader.
        /// </summary>
        /// <param name="supportsprogressreport">Determines whether downloader supports progress report or not</param>
        public Downloader(bool supportsprogressreport)
        {
            _downloaderworker.WorkerReportsProgress = true;
            _downloaderworker.WorkerSupportsCancellation = true;

            _downloads = new DownloadFileInfoCollection(this);
            SetPacketSize(4096); _stopwatchcycle = 5;
            _deletecompletedfilesaftercancel = false;
            _supportsprogress = supportsprogressreport;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificationValidate);

            _downloaderworker.DoWork += new DoWorkEventHandler(_downloaderworker_DoWork);
            _downloaderworker.ProgressChanged += new ProgressChangedEventHandler(_downloaderworker_ProgressChanged);
            _downloaderworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_downloaderworker_RunWorkerCompleted);
        }

        #endregion

        #region "events"

        /// <summary>
        /// Event handler that is invoked when download routine fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        public delegate void FileDownloadFailedEventHandler(object sender, Exception ex);

        /// <summary>
        /// Event handler that is invoked upon download file size calculation routines.
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
        /// <param name="e"></param>
        protected virtual void OnCalculationFileSizesStarted(EventArgs e)
        {
            if (CalculationFileSizesStarted != null) CalculationFileSizesStarted(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has been canceled by the user
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Calls the Cancelled event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCancelled(EventArgs e)
        {
            if (Cancelled != null) Cancelled(this, e);
        }

        /// <summary>
        /// Occurs when the user has requested to cancel the downloads
        /// </summary>
        public event EventHandler CancelRequested;

        /// <summary>
        /// Calls the CancelRequested event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCancelRequested(EventArgs e)
        {
            if (CancelRequested != null) CancelRequested(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has been completed (without canceling it)
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Calls the Completed event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null) Completed(this, e);
        }

        /// <summary>
        /// Occurs when the user has requested to cancel the downloads and the cleanup of the downloaded files has started
        /// </summary>
        public event EventHandler DeleteFilesAfterCancel;

        /// <summary>
        /// Calls the DeleteFilesAfterCancel event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDeleteFilesAfterCancel(EventArgs e)
        {
            if (DeleteFilesAfterCancel != null) DeleteFilesAfterCancel(this, e);
        }

        /// <summary>
        /// Occurs when a certain download file is about to be removed in the server after successfult file download process.
        /// </summary>
        public event EventHandler DeletingFileAfterDownload;

        /// <summary>
        /// Calls the DeletingFileAfterDownload event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDeletingFileAfterDownload(EventArgs e)
        {
            if (DeletingFileAfterDownload != null) DeletingFileAfterDownload(this, e);
        }

        /// <summary>
        /// Occurs when the calculation of the file sizes has started
        /// </summary>
        public event FileSizeCalculationEventHandler CalculatingFileSize;

        /// <summary>
        /// Calls the CalculatingFileSize event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCalculatingFileSize(Int32 e)
        {
            if (CalculatingFileSize != null) CalculatingFileSize(this, e);
        }

        /// <summary>
        /// Occurs when the FileDownloader attempts to get a web response to download the file.
        /// </summary>
        public event EventHandler FileDownloadAttempting;

        /// <summary>
        /// Calls the FileDownloadAttempting event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileDownloadAttempting(EventArgs e)
        {
            if (FileDownloadAttempting != null) FileDownloadAttempting(this, e);
        }

        /// <summary>
        /// Occurs when a file download has been completed unsuccessfully
        /// </summary>
        public event FileDownloadFailedEventHandler FileDownloadFailed;

        /// <summary>
        /// Calls the FileDownloadFailed event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileDownloadFailed(Exception e)
        {
            if (FileDownloadFailed != null) FileDownloadFailed(this, e);
        }

        /// <summary>
        /// Occurs when a file download has started
        /// </summary>
        public event EventHandler FileDownloadStarted;

        /// <summary>
        /// Calls the FileDownloadStarted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileDownloadStarted(EventArgs e)
        {
            if (FileDownloadStarted != null) FileDownloadStarted(this, e);
        }

        /// <summary>
        /// Occurs when a file download has stopped
        /// </summary>
        public event EventHandler FileDownloadStopped;

        /// <summary>
        /// Calls the FileDownloadStopped  event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileDownloadStopped(EventArgs e)
        {
            if (FileDownloadStopped != null) FileDownloadStopped(this, e);
        }

        /// <summary>
        /// Occurs when a file download has been completed successfully
        /// </summary>
        public event EventHandler FileDownloadSucceeded;

        /// <summary>
        /// Calls the FileDownloadSucceeded event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileDownloadSucceeded(EventArgs e)
        {
            if (FileDownloadSucceeded != null) FileDownloadSucceeded(this, e);
        }

        /// <summary>
        /// Occurs when the FileDownloader attempts to get a web response to download the file
        /// </summary>
        public event EventHandler FileSizesCalculationComplete;

        /// <summary>
        /// Calls the FileSizesCalculationComplete event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileSizesCalculationComplete(EventArgs e)
        {
            if (FileSizesCalculationComplete != null) FileSizesCalculationComplete(this, e);
        }

        /// <summary>
        /// Occurs when the busy state of the FileDownloader has changed
        /// </summary>
        public event EventHandler IsBusyChanged;

        /// <summary>
        /// Calls the IsBusyChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsBusyChanged(EventArgs e)
        {
            if (IsBusyChanged != null) IsBusyChanged(this, e);
        }

        /// <summary>
        /// Occurs when the pause state of the FileDownloader has changed
        /// </summary>
        public event EventHandler IsPausedChanged;

        /// <summary>
        /// Calls the IsPausedChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsPausedChanged(EventArgs e)
        {
            if (IsPausedChanged != null) IsPausedChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has been paused.
        /// </summary>
        public event EventHandler Paused;

        /// <summary>
        /// Calls the Paused event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPaused(EventArgs e)
        {
            if (Paused != null) Paused(this, e);
        }

        /// <summary>
        /// Occurs every time a block of data has been downloaded
        /// </summary>
        public event EventHandler ProgressChanged;

        /// <summary>
        /// Calls the ProgressChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null) ProgressChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has been resumed
        /// </summary>
        public event EventHandler Resumed;

        /// <summary>
        /// Calls the Resumed event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnResumed(EventArgs e)
        {
            if (Resumed != null) Resumed(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Calls the Started event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStarted(EventArgs e)
        {
            if (Started != null) Started(this, e);
        }

        /// <summary>
        /// Occurs when the either the busy or pause state of the FileDownloader have changed
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Calls the StateChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null) StateChanged(this, e);
        }

        /// <summary>
        /// Occurs when the file downloading has been stopped by either cancellation or completion
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Calls the Stopped event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStopped(EventArgs e)
        {
            if (Stopped != null) Stopped(this, e);
        }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets whether the downloader can resume the download process or not.
        /// </summary>
        public bool CanResume
        {
            get { return IsBusy && (!IsPaused) && (!_downloaderworker.CancellationPending); }
        }

        /// <summary>
        /// Gets whether the downloader can start a new process or not.
        /// </summary>
        public bool CanStart
        {
            get { return (!IsBusy); }
        }

        /// <summary>
        /// Gets whether the downloader can be cancelled or not.
        /// </summary>
        public bool CanStop
        {
            get { return IsBusy && (!_downloaderworker.CancellationPending); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _currentlydownloaded = 0;

        /// <summary>
        /// Gets the total bytes downloaded for the current downloading file. To get the overall download size refer to TotalDownloaded value.
        /// </summary>
        public double CurrentlyDownloaded
        {
            get { return _currentlydownloaded; }
        }

        /// <summary>
        /// Gets the current downloading file progress percentage.
        /// </summary>
        public double CurrentDownloadPercentage
        {
            get { return GetCurrentDownloadPercentage(0); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _currentdownloadsize = 0;

        /// <summary>
        /// Gets the size of the current downloading file size in bytes.
        /// </summary>
        public double CurrentDownloadSize
        {
            get { return _currentdownloadsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _currentfile = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _deletecompletedfilesaftercancel = false;

        /// <summary>
        /// Gets or sets whether the download process will delete currently downloaded files when cancelled or not. Default value is set to False. 
        /// </summary>
        public bool DeleteCompletedFilesAfterCancel
        {
            get { return _deletecompletedfilesaftercancel; }
            set { _deletecompletedfilesaftercancel = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BackgroundWorker _downloaderworker = new BackgroundWorker();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _downloadspeed = 0;

        /// <summary>
        /// Gets the current download speed in bytes.
        /// </summary>
        public int DownloadSpeed
        {
            get { return _downloadspeed; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DownloadFileInfoCollection _downloads = null;

        /// <summary>
        /// Gets the lists of download files.
        /// </summary>
        public DownloadFileInfoCollection Downloads
        {
            get { return _downloads; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isbusy = false;

        /// <summary>
        /// Gets whether the downloader is currently busy or not.
        /// </summary>
        public bool IsBusy
        {
            get { return _isbusy; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _iscancelled = false;

        /// <summary>
        /// Gets whether the download process has ben cancelled or not.
        /// </summary>
        public bool IsCancelled
        {
            get { return _iscancelled; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _ispaused = false;

        /// <summary>
        /// Gets whether the downloader is currently at paused state or not.
        /// </summary>
        public bool IsPaused
        {
            get { return _ispaused; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _localdirectory = "";

        /// <summary>
        /// Gets or sets the local directory in which files will be stored.
        /// </summary>
        public string LocalDirectory
        {
            get { return _localdirectory; }
            set { _localdirectory = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _packetsize = 4096;

        /// <summary>
        /// Gets the size of the blocks that will be downloaded.
        /// </summary>
        public int PacketSize
        {
            get { return _packetsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _stopwatchcycle = 0;

        /// <summary>
        /// Gets or sets the amount of blocks that need to be downloaded before the progress speed is re-calculated. Note: setting this to a low value might decrease the accuracy of the calculation. Default value is 5.
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
        /// Gets or sets whether the downloader supports progress reports or not. When enabled, downloader will have to make computations of the downloading files before proceeding with the download process. Default value is set to False.
        /// </summary>
        public bool SupportsProgress
        {
            get { return _supportsprogress; }
            set
            {
                if (IsBusy) throw new Exception("Can't set the progress report support while the downloader is running.");
                else _supportsprogress = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _totaldownloaded = 0;

        /// <summary>
        /// Gets the total amount of bytes being downloaded.
        /// </summary>
        public double TotalDownloaded
        {
            get { return _totaldownloaded; }
        }

        /// <summary>
        /// Gets the total download percentage.
        /// </summary>
        public double TotalDownloadPercentage
        {
            get { return GetTotalDownloadPercentage(0); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _totaldownloadsize = 0;

        /// <summary>
        /// Gets the total size of all files together. Correct value will be returned if SupportsProgress is set otherwise returns 0.
        /// </summary>
        public double TotalDownloadSize
        {
            get { return _totaldownloadsize; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ManualResetEvent _trigger = new ManualResetEvent(true);

        #endregion

        #region "methods"

        private void CalculateFileSizes()
        {
            FireEventFromWorker(DownloadTransitions.CalculationFileSizesStarted);
            _totaldownloadsize = 0;

            for (int i = 0; i <= Downloads.Count - 1; i++)
            {
                _downloaderworker.ReportProgress((int) DownloadInvokations.CalculatingFileNrRaiser, i + 1);

                try
                {
                    DownloadFileInfo _download = Downloads[i];

                    if (IsFtpRequest(_download.Path))
                    {
                        FtpWebRequest _request = (FtpWebRequest)WebRequest.Create(_download.Path);
                        if (_download.Credentials != null) _request.Credentials = _download.Credentials;
                        _request.KeepAlive = true; _request.UseBinary = true;
                        _request.Method = WebRequestMethods.Ftp.DownloadFile;
                        FtpWebResponse _response = (FtpWebResponse) _request.GetResponse();
                        string _status = _response.StatusDescription;
                        MatchCollection _matches = Regex.Matches(_status.ToLower(), "[0-9]+ bytes");
                        if (_matches.Count > 0) _totaldownloadsize += VisualBasic.CDbl(_matches[0].Value.ToLower().Replace("bytes", "").RLTrim());
                        else _totaldownloadsize += _response.ContentLength;
                        _response.Close();
                    }
                    else
                    {
                        HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(_download.Path);
                        if (_download.Credentials != null)
                        {
                            _request.Credentials = _download.Credentials; _request.PreAuthenticate = true;
                        }
                        HttpWebResponse _response = (HttpWebResponse) _request.GetResponse();
                        _totaldownloadsize += _response.ContentLength;
                        _response.Close();
                    }
                }
                catch { }
            }

            FireEventFromWorker(DownloadTransitions.FileSizesCalculationComplete);
        }

        private void CleanUpFiles()
        { CleanUpFiles(0); }

        private void CleanUpFiles(int start)
        { CleanUpFiles(start, -1); }

        private void CleanUpFiles(int start, int length)
        {
            int _last = (length < 0? Downloads.Count -1 : start + length - 1);

            for (int i = start; i <= _last; i++)
            {
                string _fullname = LocalDirectory + "\\" + Downloads[i].Name;
                if (File.Exists(_fullname))
                {
                    try { File.Delete(_fullname); }
                    catch { }
                }
            }

            Materia.RefreshAndManageCurrentProcess();
        }

        private void DeleteFileFromServer(int index)
        {
            DownloadFileInfo _info = Downloads[index];

            if (IsFtpRequest(_info.Path) &&
                _info.DeleteFileAfterDownload)
            {
                FtpWebRequest _request = null; FtpWebResponse _response = null;

                try
                {
                    _request = (FtpWebRequest) WebRequest.Create(_info.Path);
                    if (_info.Credentials != null) _request.Credentials = _info.Credentials;
                    _request.KeepAlive = true; _request.UseBinary = true;
                    _request.Method = WebRequestMethods.Ftp.DeleteFile;
                    _response = (FtpWebResponse)_request.GetResponse(); _response.Close();
                }
                catch { }

                Materia.RefreshAndManageCurrentProcess();
            }
        }

        private void DownloadFile(int index)
        {
            _currentdownloadsize = 0; FireEventFromWorker(DownloadTransitions.FileDownloadAttempting);

            DownloadFileInfo _file = Downloads[index]; double _size = 0;
            byte[] _readbytes = new byte[PacketSize - 1]; int _currentpackagesize = 0;
            FileStream _writer = new FileStream(LocalDirectory + "\\" + _file.Name, FileMode.Create);
            Stopwatch _speedtimer = new Stopwatch(); Stream _stream = null;
            Int32 _readings = 0; Exception _ex = null; int _packet = PacketSize;

            if (IsFtpRequest(_file.Path))
            {
                FtpWebRequest _request = null; FtpWebResponse _response = null;
                try
                {
                    _request = (FtpWebRequest) WebRequest.Create(_file.Path);
                    if (_file.Credentials != null) _request.Credentials = _file.Credentials;
                    _request.KeepAlive = true; _request.UseBinary = true;
                    _request.Method = WebRequestMethods.Ftp.DownloadFile;
                    _response = (FtpWebResponse)_request.GetResponse();

                    string _status = _response.StatusDescription;
                    MatchCollection _matches = Regex.Matches(_status.ToLower(), "[0-9]+ bytes");
                    if (_matches.Count > 0) _size = VisualBasic.CDbl(_matches[0].Value.ToLower().Replace("bytes", "").RLTrim());
                    else _size = _response.ContentLength;
                }
                catch (Exception ex) { _ex = ex; }

                _currentdownloadsize = _size; FireEventFromWorker(DownloadTransitions.FileDownloadStarted);

                if (_ex != null)
                {
                    try { _writer.Close(); }
                    catch { }
                    _downloaderworker.ReportProgress((int) DownloadInvokations.FileDownloadFailedRaiser, _ex);
                }
                else
                {
                    _currentlydownloaded = 0; _currentpackagesize = 0;

                    do
                    {
                        if (_downloaderworker.CancellationPending)
                        {
                            _speedtimer.Stop(); _writer.Close(); _response.Close(); return;
                        }
                        try { _trigger.WaitOne(); }
                        catch { }

                        _speedtimer.Start();

                        if (_stream==null) _stream = _response.GetResponseStream();

                        if (_readbytes.Length < PacketSize) _packet = _readbytes.Length;
                        else _packet = PacketSize;

                        _currentpackagesize = _stream.Read(_readbytes, 0, _packet);
                        _currentlydownloaded += _currentpackagesize; _totaldownloaded += _packet;
                        FireEventFromWorker(DownloadTransitions.ProgressChanged);

                        _writer.Write(_readbytes, 0, _currentpackagesize); _readings += 1;

                        if (_readings >= StopWatchCycle)
                        {
                             _downloadspeed = VisualBasic.CInt(_packet * StopWatchCycle * 1000 / (_speedtimer.ElapsedMilliseconds + 1));
                             _speedtimer.Reset(); _readings = 0;
                        }
                    }
                    while (_currentpackagesize != 0);

                    _writer.Close() ; _speedtimer.Stop() ; _response.Close();
                    FireEventFromWorker(DownloadTransitions.FileDownloadSucceeded);

                    if (_file.DeleteFileAfterDownload)
                    {
                        FireEventFromWorker(DownloadTransitions.DeletingFileAfterDownload);
                        DeleteFileFromServer(index);
                    }
                }
            }
            else
            {
                HttpWebRequest _request = null; HttpWebResponse _response = null;

                try
                {
                    _request = (HttpWebRequest)WebRequest.Create(_file.Path);
                    if (_file.Credentials != null) _request.Credentials = _file.Credentials;
                    _response = (HttpWebResponse)_request.GetResponse();
                    _size = _response.ContentLength;
                }
                catch (Exception ex) { _ex = ex; }

                _currentdownloadsize = _size; FireEventFromWorker(DownloadTransitions.FileDownloadStarted);

                if (_ex != null)
                {
                    try { _writer.Close(); }
                    catch { }
                    _downloaderworker.ReportProgress((int) DownloadInvokations.FileDownloadFailedRaiser, _ex);
                }
                else
                {
                    _currentlydownloaded = 0; _currentpackagesize = 0;

                    do
                    {
                        if (_downloaderworker.CancellationPending)
                        {
                            _speedtimer.Stop(); _writer.Close(); _response.Close(); return;
                        }

                        try { _trigger.WaitOne(); }
                        catch { }
                        
                        _speedtimer.Start();

                        if (_stream == null) _stream = _response.GetResponseStream();

                        if (_readbytes.Length < PacketSize) _packet = _readbytes.Length;
                        else _packet = PacketSize;

                        _currentpackagesize = _stream.Read(_readbytes, 0, _packet);
                        _currentlydownloaded += _currentpackagesize; _totaldownloaded += _packet;
                        FireEventFromWorker(DownloadTransitions.ProgressChanged);

                        _writer.Write(_readbytes, 0, _currentpackagesize); _readings += 1;

                        if (_readings >= StopWatchCycle)
                        {
                            _downloadspeed = VisualBasic.CInt(PacketSize * StopWatchCycle * 1000 / (_speedtimer.ElapsedMilliseconds + 1));
                            _speedtimer.Reset(); _readings = 0;
                        }
                    }
                    while (_currentpackagesize != 0);

                    _writer.Close(); _speedtimer.Stop(); _response.Close();
                    FireEventFromWorker(DownloadTransitions.FileDownloadSucceeded);

                    if (_file.DeleteFileAfterDownload)
                    {
                        FireEventFromWorker(DownloadTransitions.DeletingFileAfterDownload);
                        DeleteFileFromServer(index);
                    }
                }
            }
        }

        private void FireEventFromWorker(DownloadTransitions triggeredevent)
        { _downloaderworker.ReportProgress((int) DownloadInvokations.EventRaiser, triggeredevent); }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with binary notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <returns></returns>
        public static string FormatSizeBinary(long size)
        { return FormatSizeBinary(size, 0); }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with binary notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <param name="decimals">The amount of decimals for the notation</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static string FormatSizeDecimal(long size)
        { return FormatSizeDecimal(size, 0); }

        /// <summary>
        /// Formats the amount of bytes to a more readible notation with decimal notation symbols
        /// </summary>
        /// <param name="size">The raw amount of bytes</param>
        /// <param name="decimals">The amount of decimals for the notation</param>
        /// <returns></returns>
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

        private bool IsFtpRequest(string url)
        { return Regex.IsMatch(url, "ftp://"); }

        /// <summary>
        ///  Returns the current downloading file progress percentage.
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public double GetCurrentDownloadPercentage(int decimals)
        {
            if (!SupportsProgress) return 0;
            else
            {
                if (CurrentDownloadSize > 0) return Math.Round((CurrentlyDownloaded / CurrentDownloadSize) * 100, decimals);
                else return 0;
            }
        }

        /// <summary>
        /// Gets the total download percentage.
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public double GetTotalDownloadPercentage(int decimals)
        {
            if (!SupportsProgress) return 0;
            else
            {
                if (TotalDownloadSize > 0) return Math.Round((TotalDownloaded / TotalDownloadSize) * 100, decimals);
                else return 0;
            }
        }

        private static bool RemoteCertificationValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { return true; }

        /// <summary>
        /// Sets the downloader's busy state.
        /// </summary>
        /// <param name="busy"></param>
        protected virtual void SetBusy(bool busy)
        {
            if (IsBusy != busy)
            {
                _isbusy = busy;
                if (IsBusy)
                {
                    _totaldownloaded = 0; _downloaderworker.RunWorkerAsync();
                    OnStarted(new EventArgs());
                    OnIsBusyChanged(new EventArgs());
                    OnStateChanged(new EventArgs());
                }
                else
                {
                    _ispaused = false; _downloaderworker.CancelAsync();
                    OnCancelRequested(new EventArgs());
                    OnStateChanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Sets the downloader's allocated packet size block.
        /// </summary>
        /// <param name="packets"></param>
        public virtual void SetPacketSize(int packets)
        {
            if (packets > 0) _packetsize = packets;
            else throw new Exception("Packet size cannot be less than or equal to zero.");
        }

        /// <summary>
        /// Sets the current downloader's paused state.
        /// </summary>
        /// <param name="paused"></param>
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
        /// Starts the download process.
        /// </summary>
        public void Start()
        { SetBusy(true); }

        /// <summary>
        /// Pause the download process.
        /// </summary>
        public void Pause()
        { SetPaused(true); }

        /// <summary>
        /// Resumes the download process.
        /// </summary>
        public void Resume()
        { SetPaused(false); }

        /// <summary>
        /// Cancels the currently running download process.
        /// </summary>
        public void Stop()
        { Stop(false); }

        /// <summary>
        /// Cancels the currently running download process.
        /// </summary>
        /// <param name="deletecompletedfiles">Indicates whether the completed downloaded files should be deleted or not.</param>
        public void Stop(bool deletecompletedfiles)
        {
            if (deletecompletedfiles) DeleteCompletedFilesAfterCancel = deletecompletedfiles;
            _iscancelled = true; SetBusy(false);
        }

        #endregion

        #region "backgroundworker events"

        private void _downloaderworker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SupportsProgress) CalculateFileSizes();
            if (!Directory.Exists(LocalDirectory))
            {
                try
                { Directory.CreateDirectory(LocalDirectory); }
                catch { }
            }

            int i = 0;

            while (i < Downloads.Count && (!_downloaderworker.CancellationPending))
            {
                _currentfile = i; DownloadFile(i);
                if (_downloaderworker.CancellationPending)
                {
                    FireEventFromWorker(DownloadTransitions.DeletingFilesAfterCancel);
                    CleanUpFiles((DeleteCompletedFilesAfterCancel ? 0 : _currentfile), (DeleteCompletedFilesAfterCancel ? _currentfile + 1 : 1));
                }
                else i += 1;
            }
        }

        private void _downloaderworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadInvokations _invokation = (DownloadInvokations)e.ProgressPercentage;

            switch (_invokation)
            {
                case DownloadInvokations.EventRaiser:
                    DownloadTransitions _transition = (DownloadTransitions)e.UserState;
                    switch (_transition)
                    {
                        case DownloadTransitions.CalculationFileSizesStarted:
                            OnCalculationFileSizesStarted(new EventArgs()); break;
                        case DownloadTransitions.FileSizesCalculationComplete:
                            OnFileSizesCalculationComplete(new EventArgs()); break;
                        case DownloadTransitions.DeletingFileAfterDownload:
                            OnDeletingFileAfterDownload(new EventArgs()); break;
                        case DownloadTransitions.DeletingFilesAfterCancel:
                            OnDeleteFilesAfterCancel(new EventArgs()); break;
                        case DownloadTransitions.FileDownloadAttempting:
                            OnFileDownloadAttempting(new EventArgs()); break;
                        case DownloadTransitions.FileDownloadStarted:
                            OnFileDownloadStarted(new EventArgs()); break;
                        case DownloadTransitions.FileDownloadStopped:
                            OnFileDownloadStopped(new EventArgs()); break;
                        case DownloadTransitions.FileDownloadSucceeded:
                            OnFileDownloadSucceeded(new EventArgs()); break;
                        case DownloadTransitions.ProgressChanged:
                            OnProgressChanged(new EventArgs()); break;
                        default: break;
                    }
                    break;
                case DownloadInvokations.FileDownloadFailedRaiser:
                    OnFileDownloadFailed((Exception)e.UserState); break;
                case DownloadInvokations.CalculatingFileNrRaiser:
                    OnCalculatingFileSize((int)e.UserState); break;
                default: break;
            }
        }

        private void _downloaderworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    if (_downloaderworker != null)
                    {
                        try { _downloaderworker.Dispose(); }
                        catch { }
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
    /// Download file information class.
    /// </summary>
    public class DownloadFileInfo
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DownloadFileInfo.
        /// </summary>
        /// <param name="downloadpath">The http or ftp path of the downloading file</param>
        /// <param name="uid">Server authentication User Id</param>
        /// <param name="pwd">Server authentication Password</param>
        public DownloadFileInfo(string downloadpath, string uid, string pwd) : this(downloadpath,uid,pwd, false)
        { }

        /// <summary>
        /// Creates a new instance of DownloadFileInfo.
        /// </summary>
        /// <param name="downloadpath">The http or ftp path of the downloading file</param>
        /// <param name="uid">Server authentication User Id</param>
        /// <param name="pwd">Server authentication Password</param>
        /// <param name="deletefileafterdownload">Determines whether to delete file from the server after successfult download</param>
        public DownloadFileInfo(string downloadpath, string uid, string pwd, bool deletefileafterdownload) : this(downloadpath, new NetworkCredential(uid, pwd), deletefileafterdownload)
        { }

        /// <summary>
        /// Creates a new instance of DownloadFileInfo.
        /// </summary>
        /// <param name="downloadpath">The http or ftp path of the downloading file</param>
        /// <param name="credential">Network authentication information for the download server</param>
        public DownloadFileInfo(string downloadpath, NetworkCredential credential) : this(downloadpath, credential, false)
        { }

        /// <summary>
        /// Creates a new instance of DownloadFileInfo.
        /// </summary>
        /// <param name="downloadpath">The http or ftp path of the downloading file</param>
        /// <param name="credential">Network authentication information for the download server</param>
        /// <param name="deletefileafterdownload">Determines whether to delete file from the server after successfult download</param>
        public DownloadFileInfo(string downloadpath, NetworkCredential credential, bool deletefileafterdownload)
        {
            _path = downloadpath; _credentials = credential; _deletfileafterdownload = deletefileafterdownload;
            string[] _sections = downloadpath.Split(new char[] { '/' });
            _name = _sections[_sections.Length - 1];
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _deletfileafterdownload = false;

        /// <summary>
        /// Gets or sets whether to delete the specified download file from the server after successful download. This is applicable only for ftp file downloads. Default value is False.
        /// </summary>
        public bool DeleteFileAfterDownload
        {
            get { return _deletfileafterdownload; }
            set { _deletfileafterdownload = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkCredential _credentials = null;

        /// <summary>
        /// Gets or sets the downloading file's http or ftp network credentials. Not specifying this value indicates there is no authentications needed in the download server.
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _name = "";

        /// <summary>
        /// Gets or sets the downloading file's associated name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _path = "";

        /// <summary>
        /// Gets the currently initialized downnloading file's path.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Returns the name of the downloaded file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        { return _name; }

        #endregion

    }

    /// <summary>
    /// Collection of download file information.
    /// </summary>
    public class DownloadFileInfoCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DownloadFileInfoCollection
        /// </summary>
        /// <param name="downloader"></param>
        public DownloadFileInfoCollection(Downloader downloader)
        { _downloader = downloader; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Downloader _downloader = null;

        /// <summary>
        /// Gets the current hosted Downloader.
        /// </summary>
        public Downloader Downloader
        {
            get { return _downloader; }
        }

        /// <summary>
        /// Gets the downloading file information in the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DownloadFileInfo this[int index]
        {
            get { return (DownloadFileInfo)List[index]; }
        }

        /// <summary>
        /// Gets a downloading file information with the specified name within the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DownloadFileInfo this[string name]
        {
            get { return GetDownloadInfoByName(name); }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new download file information into the collection.
        /// </summary>
        /// <param name="path">Http or Ftp download path</param>
        /// <returns></returns>
        public DownloadFileInfo Add(string path)
        { return Add(path, "", ""); }

        /// <summary>
        /// Adds a new download file information into the collection.
        /// </summary>
        /// <param name="path">Http or Ftp download path</param>
        /// <param name="uid">Server authentication User Id</param>
        /// <param name="pwd">Server authentication Password</param>
        /// <returns></returns>
        public DownloadFileInfo Add(string path, string uid, string pwd)
        { return Add(path, uid, pwd, false); }

        /// <summary>
        /// Adds a new download file information into the collection.
        /// </summary>
        /// <param name="path">Http or Ftp download path</param>
        /// <param name="uid">Server authentication User Id</param>
        /// <param name="pwd">Server authentication Password</param>
        /// <param name="deletfileafterdownload">Determines whether to delete download file from the server after successful download process</param>
        /// <returns></returns>
        public DownloadFileInfo Add(string path, string uid, string pwd, bool deletfileafterdownload)
        {
            NetworkCredential _credential = null;
            if (!String.IsNullOrEmpty(uid.RLTrim()) ||
                !String.IsNullOrEmpty(pwd.RLTrim())) _credential = new NetworkCredential(uid, pwd);
            return Add(path, _credential, deletfileafterdownload); 
        }

        /// <summary>
        /// Adds a new download file information into the collection.
        /// </summary>
        /// <param name="path">Http or Ftp download path</param>
        /// <param name="credential">Server authentication credentials</param>
        /// <returns></returns>
        public DownloadFileInfo Add(string path, NetworkCredential credential)
        { return Add(path, credential, false); }

        /// <summary>
        /// Adds a new download file information into the collection.
        /// </summary>
        /// <param name="path">Http or Ftp download path</param>
        /// <param name="credential">Server authentication credentials</param>
        /// <param name="deletfileafterdownload">Determines whether to delete download file from the server after successful download process</param>
        /// <returns></returns>
        public DownloadFileInfo Add(string path, NetworkCredential credential, bool deletfileafterdownload)
        {
            DownloadFileInfo _info = new DownloadFileInfo(path, credential, deletfileafterdownload);
            int _index = List.Add(_info); return (DownloadFileInfo)List[_index];
        }

        /// <summary>
        /// Returns whether an specific download file information with the specified name exists within the collection or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        { return VisualBasic.CBool(GetDownloadInfoByName(name) != null); }

        /// <summary>
        /// Returns whether the specified download information already exists within the collection or not.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Contains(DownloadFileInfo info)
        { return List.Contains(info); }


        private DownloadFileInfo GetDownloadInfoByName(string name)
        {
            DownloadFileInfo _info = null;

            foreach (DownloadFileInfo info in List)
            {
                if (info.Name.ToLower() == name.ToLower())
                {
                    _info = info; break;
                }
            }

            return _info;
        }

        /// <summary>
        /// Removes a certain download file information with the specified name from the collection.
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            DownloadFileInfo _info = GetDownloadInfoByName(name);
            if (_info != null) Remove(_info);
        }

        /// <summary>
        /// Removes the specified download file information from the collection.
        /// </summary>
        /// <param name="info"></param>
        public void Remove(DownloadFileInfo info)
        {
            if (Contains(info)) List.Remove(info);
        }

        #endregion

    }
}
