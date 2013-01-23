#region "imports"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Development.Materia.Cryptography
{
    /// <summary>
    /// Cryptographic service providing class.
    /// </summary>
    public abstract class Cryptographer
    {

        #region "private methods"

        private static string AdvancedDecrypt(string value, string key)
        {
            string _decrypted = "";
            MD5Hash _md5 = new MD5Hash(key); TripleDESCryptoServiceProvider _des = new TripleDESCryptoServiceProvider();

            try
            {
                _des.Key = _md5.Hash; _des.Mode = CipherMode.ECB;
                byte[] _bytes = Convert.FromBase64String(value);
                _decrypted = ASCIIEncoding.ASCII.GetString(_des.CreateDecryptor().TransformFinalBlock(_bytes, 0, _bytes.Length));
            }
            catch { }
            finally
            {
                _md5 = null; _des.Clear(); _des = null;
                Materia.RefreshAndManageCurrentProcess();
            }

            return _decrypted;
        }

        private static string AdvancedEncrypt(string value, string key)
        {
            string _encrypted = value;
            MD5Hash _md5 = new MD5Hash(key);
            TripleDESCryptoServiceProvider _des = new TripleDESCryptoServiceProvider();

            try
            {
                _des.Key = _md5.Hash; _des.Mode = CipherMode.ECB;
                byte[] _bytes = ASCIIEncoding.ASCII.GetBytes(value);
                _encrypted = Convert.ToBase64String(_des.CreateEncryptor().TransformFinalBlock(_bytes, 0, _bytes.Length));
            }
            catch { }
            finally
            {
                _md5 = null; _des.Clear(); _des = null;
                Materia.RefreshAndManageCurrentProcess();
            }

            return _encrypted;
        }

        private static string SimpleDecrypt(string value, string key)
        {
            string _decrypted = ""; int _keycount = key.Trim().Length;
            char[] _chars = value.ToCharArray();

            if (_keycount <= 7) _keycount = 7;

            foreach (char _char in _chars)
            {
                _decrypted += VisualBasic.Chr(VisualBasic.Asc(_char) - _keycount).ToString();
            }

            return _decrypted;
        }

        private static string SimpleEncrypt(string value, string key)
        {
            string _encrypted = ""; int _keycount = key.Trim().Length;
            char[] _chars = value.ToCharArray();

            if (_keycount <= 7) _keycount = 7;

            foreach (char _char in _chars)
            {
                _encrypted += VisualBasic.Chr(VisualBasic.Asc(_char) + _keycount).ToString();
            }

            return _encrypted;
        }

        #endregion

        #region "static methods"

        /// <summary>
        /// Gets the decrypted string value deciphered using the supplied encryption key pattern.
        /// </summary>
        /// <param name="value">Value to decrypt</param>
        /// <param name="key">Encryption key pattern string</param>
        /// <returns>Returns a more readable string value of the specified encrypted text using the specified key pattern.</returns>
        public static string Decrypt(string value, string key)
        {
            return Decrypt(value, key, false);
        }

        /// <summary>
        /// Returns decrypted string value deciphered using the supplied encryption key pattern.
        /// </summary>
        /// <param name="value">Value to decrypt</param>
        /// <param name="key">Encryption key pattern string</param>
        /// <param name="usesimpledecryption">Use simple decryption or not</param>
        /// <returns>Returns a more readable string value of the specified encrypted text using the specified key pattern.</returns>
        public static string Decrypt(string value, string key, bool usesimpledecryption)
        {
            if (usesimpledecryption) return SimpleDecrypt(value, key);
            else return AdvancedDecrypt(value, key);
        }

        /// <summary>
        /// Returns encrypted string value of the specified string using the supplied encryption key pattern.
        /// </summary>
        /// <param name="value">Value to encrypt</param>
        /// <param name="key">Encryption key pattern string</param>
        /// <returns>Returns a non-human-readable text derived from the specified string value and key pattern.</returns>
        public static string Encrypt(string value, string key)
        {
            return Encrypt(value, key, false);
        }

        /// <summary>
        /// Returns encrypted string value of the specified string using the supplied encryption key pattern.
        /// </summary>
        /// <param name="value">Value to encrypt</param>
        /// <param name="key">Encryption key pattern string</param>
        /// <param name="usesimpleencryption">Use simple encryption or not</param>
        /// <returns>Returns a non-human-readable text derived from the specified string value and key pattern.</returns>
        public static string Encrypt(string value, string key, bool usesimpleencryption)
        {
            if (usesimpleencryption) return SimpleEncrypt(value, key);
            else return AdvancedEncrypt(value, key);
        }

        #endregion

    }
}
