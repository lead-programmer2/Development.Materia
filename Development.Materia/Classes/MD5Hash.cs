#region "imports"
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Linq;
using System.Text;
#endregion

namespace Development.Materia.Cryptography
{
    /// <summary>
    /// Hash byte generator using MD5.
    /// </summary>
    public class MD5Hash
    {
        #region "variables"
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _value = "";

        #endregion

        #region "constructors"

        /// <summary>
        /// Creates a new instance of MD5Hash.
        /// </summary>
        public MD5Hash() : this("")
        { }

        /// <summary>
        /// Creates a new instance of MD5Hash.
        /// </summary>
        /// <param name="value">Text to be evaluated for the hash creation</param>
        public MD5Hash(string value)
        { _value = value;  }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets the hash byte value generated for the given key value.
        /// </summary>
        public byte[] Hash
        {
            get { return GetHash(); }
        }

        #endregion

        #region "methods"

        private byte[] GetHash()
        { return _md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(_value));  }

        #endregion

    }
}
