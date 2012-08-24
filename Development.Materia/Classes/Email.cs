#region "imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

#endregion

namespace Development.Materia.Net
{

    #region "enumerations"

    /// <summary>
    /// Mail servers
    /// </summary>
    public enum MailHostEnum
    {
        /// <summary>
        /// Custom assigned mail server address
        /// </summary>
        CustomMail = 3,
        /// <summary>
        /// Mail server : smtp.gmail.com
        /// </summary>
        Gmail = 1,
        /// <summary>
        /// Mail server : smtp.mail.yahoo.com
        /// </summary>
        YahooMail = 2,
        /// <summary>
        /// Mail server : mail.fms.com.ph
        /// </summary>
        FMS = 0,
        /// <summary>
        /// Nothing
        /// </summary>
        None = -1
    }

    #endregion

    /// <summary>
    /// Class for sending emails (supports gmail and yahoo mail).
    /// </summary>
    public class Email : IDisposable
    {
        
        #region "constant variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string gmail = "smtp.gmail.com";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string yahoomail = "smtp.mail.yahoo.com";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string fmsmail = "mail.fms.com.ph";

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailAttachmentCollection _attachments = null;

        /// <summary>
        /// Gets the email's file attachment information.
        /// </summary>
        public MailAttachmentCollection Attachments
        {
            get
            {
                if (_attachments == null) _attachments = new MailAttachmentCollection(this);
                return _attachments;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailAddressCollection _bcc = new MailAddressCollection();

        /// <summary>
        /// Gets email's blind carbon copy recipient(s) information.
        /// </summary>
        public MailAddressCollection BCC
        {
            get { return _bcc; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _body = "";

        /// <summary>
        /// Gets or sets email's message contents.
        /// </summary>
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailAddressCollection _cc = new MailAddressCollection();

        /// <summary>
        /// Gets email's carbon copy recipient(s) information.
        /// </summary>
        public MailAddressCollection CC
        {
            get { return _cc; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ICredentialsByHost _credentials = null;

        /// <summary>
        /// Gets or sets mail account login credentials.
        /// </summary>
        public ICredentialsByHost Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _enablessl = false;

        /// <summary>
        /// Gets or sets whether mail server's SSL will be enabled or not.
        /// </summary>
        public bool EnableSSL
        {
            get { return _enablessl; }
            set { _enablessl = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _errormessage = "";

        /// <summary>
        /// Gets the error message of the last failed email sending attempt.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errormessage; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailAddress _from = null;

        /// <summary>
        /// Gets or sets email sender's information.
        /// </summary>
        public MailAddress From
        {
            get { return _from; }
            set { _from = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _host = "";

        /// <summary>
        /// Gets or sets mail server hostname.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set
            {
                if (_mailhost == MailHostEnum.CustomMail) _host = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _innerexception = "";

        /// <summary>
        /// Gets the inner exception details of the last failed email sending attempt. 
        /// </summary>
        public string InnerException
        {
            get { return _innerexception; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isbodyhtml = true;

        /// <summary>
        /// Gets or sets whether email message body will be threated as an html or not.
        /// </summary>
        public bool IsBodyHtml
        {
            get { return _isbodyhtml; }
            set { _isbodyhtml = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _port = 0;

        /// <summary>
        /// Gets or sets mail server port number.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _sent = false;

        /// <summary>
        /// Gets whether last email sending attempt is successfully sent or not.
        /// </summary>
        public bool Sent
        {
            get { return _sent; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _subject = "";

        /// <summary>
        /// 
        /// </summary>
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailAddressCollection _to = new MailAddressCollection();

        /// <summary>
        /// Gets email recipient(s) information.
        /// </summary>
        public MailAddressCollection To
        {
            get { return _to; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _usedefaultcredentials =  true;

        /// <summary>
        /// Gets or sets whether default network credentials shall be use to log on to the mail server or not.
        /// </summary>
        public bool UseDefaultCredentials
        {
            get { return _usedefaultcredentials; }
            set 
            { 
                _usedefaultcredentials = value;
                if (_smtp != null) _smtp.UseDefaultCredentials = value;
            }
        }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Hashtable _delegatetable = new Hashtable();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MailHostEnum _mailhost = MailHostEnum.CustomMail;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _pwd = "";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SmtpClient _smtp = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _uid = "";

        #endregion

        #region "constructors"

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="host">Mail server hostname</param>
        public Email(string host)
        {
            _host = host; _smtp = new SmtpClient(host);
        }

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="host">Mail server hostname</param>
        /// <param name="uid">Mail account user id</param>
        /// <param name="pwd">Mail account password</param>
        public Email(string host, string uid, string pwd) : this(host, 0, uid, pwd)
        { }

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="host">Mail server hostname</param>
        /// <param name="port">Mail server port number</param>
        public Email(string host, int port) : this(host, port, "", "")
        { }

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="host">Mail server hostname</param>
        /// <param name="port">Mail server port number</param>
        /// <param name="uid">Mail account user id</param>
        /// <param name="pwd">Mail account password</param>
        public Email(string host, int port, string uid, string pwd)
        {
            _uid = uid; _pwd = pwd; _host = host; _port = port;

            if (_port > 0) _smtp = new SmtpClient(_host, _port);
            else _smtp = new SmtpClient(_host);

            if (!String.IsNullOrEmpty(uid.RLTrim()) &&
                !String.IsNullOrEmpty(pwd.RLTrim())) _credentials = new NetworkCredential(uid, pwd);
        }

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="mailhost">Predefined mail server. Value should not be a CustomMail</param>
        public Email(MailHostEnum mailhost) : this(mailhost,"","")
        { }

        /// <summary>
        /// Creates a new instance of Email.
        /// </summary>
        /// <param name="mailhost">Predefined mail server. Value should not be a CustomMail</param>
        /// <param name="uid">Mail account user id</param>
        /// <param name="pwd">Mail account password</param>
        public Email(MailHostEnum mailhost, string uid, string pwd)
        {
            _mailhost = mailhost; _uid = uid; _pwd = pwd;

            switch (mailhost)
            {
                case MailHostEnum.FMS:
                    _host = fmsmail; break;
                case MailHostEnum.Gmail:
                    _host = gmail; _port = 465; break;
                case MailHostEnum.YahooMail:
                    _host = yahoomail; _port = 587; break;
                case MailHostEnum.CustomMail:
                case MailHostEnum.None:
                default: throw new Exception("Mail should not be other than FMS, Gmail or Yahoo mail host.");
            }

            if (_mailhost == MailHostEnum.FMS) _smtp = new SmtpClient(_host);
            else
            {
                if (_port > 0) _smtp = new SmtpClient(_host, _port);
                else _smtp = new SmtpClient(_host);

                _smtp.EnableSsl = true; _smtp.UseDefaultCredentials = false;
            }

            if (!String.IsNullOrEmpty(uid.RLTrim()) &&
                !String.IsNullOrEmpty(pwd.RLTrim())) _credentials = new NetworkCredential(uid, pwd);
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Calls the Send method and run it asynchronously. Must call the EndSend method once IAsynResult is finish.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult BeginSend()
        {
            Action _delegate = new Action(Send);
            IAsyncResult _result = _delegate.BeginInvoke(null, _delegate);
            if (!_delegatetable.ContainsKey(_result)) _delegatetable.Add(_result, _delegate);
            return _result;
        }

        /// <summary>
        /// Finalized the BeginSend call using its produced IAsynResult interface.
        /// </summary>
        /// <param name="result"></param>
        public void EndSend(IAsyncResult result)
        {
            if (_delegatetable.ContainsKey(result))
            {
                Action _delegate = (Action)_delegatetable[result];
                if (_delegate != null) _delegate.EndInvoke(result);

                try { _delegate = null; }
                catch { }

                Materia.RefreshAndManageCurrentProcess(); _delegatetable.Remove(result);
            }
        }

        /// <summary>
        /// Attempts to send the email.
        /// </summary>
        public void Send()
        {
            if (_smtp == null)
            {
                if (_port > 0) _smtp = new SmtpClient(_host, _port);
                else _smtp = new SmtpClient(_host);
            }

            _smtp.UseDefaultCredentials = _usedefaultcredentials;
            _smtp.EnableSsl = _enablessl; _smtp.Credentials = _credentials;

            MailMessage _mail = new MailMessage();
            if (_from != null) _mail.From = _from;
            else
            {
                if (_uid.IsEmail()) _mail.From = new MailAddress(_uid);
            }

            foreach (MailAddress _email in _to) _mail.To.Add(_email);

            foreach (MailAddress _email in _cc) _mail.CC.Add(_email);

            foreach (MailAddress _email in _bcc) _mail.Bcc.Add(_email);

            foreach (Attachment _attachment in Attachments) _mail.Attachments.Add(_attachment);

            _mail.Subject = _subject; _mail.SubjectEncoding = Encoding.UTF8;
            _mail.IsBodyHtml = _isbodyhtml; _mail.Body = _body; _mail.BodyEncoding = Encoding.UTF8;

            _sent = false; _errormessage = ""; _innerexception = "";

            try
            {
                _smtp.Send(_mail); _sent = true;
            }
            catch (Exception ex)
            {
                _errormessage = ex.Message;
                if (ex.InnerException != null) _innerexception = ex.InnerException.Message;
            }
            finally
            {
                _mail.Dispose(); Materia.RefreshAndManageCurrentProcess();
            }

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
                    if (_attachments != null) _attachments.Dispose();
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// Derived class for email file attachments.
    /// </summary>
    public class MailAttachmentCollection : CollectionBase, IDisposable
    {

        /// <summary>
        /// Gets or sets file attachment in the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Attachment this[int index]
        {
            get { return (Attachment) List[index]; }
            set { List[index] = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Email _email = null;

        /// <summary>
        /// Gets the parent email for the current mail attachments.
        /// </summary>
        public Email Email
        {
            get { return _email; }
        }

        /// <summary>
        /// Creates a new instance of MailAttachmentCollection.
        /// </summary>
        /// <param name="email">Parent email object</param>
        public MailAttachmentCollection(Email email)
        { _email = email; }

        /// <summary>
        /// Adds a file attachment in the collection.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public int Add(Attachment attachment)
        { return List.Add(attachment); }

        /// <summary>
        /// Adds a file attachment in the collection.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Attachment Add(string filename)
        {
            Attachment _attachment = new Attachment(filename);
            int _index = Add(_attachment); return (Attachment)List[_index];
        }

        /// <summary>
        ///  Validates whether the specified file is currently existing within the collection.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public bool Contains(Attachment attachment)
        { return List.Contains(attachment); }

        /// <summary>
        /// Removes the specified email attachment within the collection.
        /// </summary>
        /// <param name="attachment"></param>
        public void Remove(Attachment attachment)
        { List.Remove(attachment); }

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
                    foreach (Attachment _attachment in List) _attachment.Dispose();
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }

}
