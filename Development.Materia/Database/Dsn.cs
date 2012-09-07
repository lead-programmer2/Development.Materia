#region "imports"

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Development.Materia.Database
{
    /// <summary>
    /// Class to create a DSN data access programatically.
    /// </summary>
    public class Dsn
    {

        #region "API"

        [DllImport("odbccp32.dll")]
        private static extern int SQLConfigDataSource(int hwnd, int request, string driver, string attributes);
        
        #endregion

        #region "methods"

        /// <summary>
        /// Creates the Dsn with the specified database driver and database connection attributes.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="attributes"></param>
        /// <returns>True if DSN has been added, otherwise false.</returns>
        public static bool Create(string driver, string attributes)
        {
            int _result = SQLConfigDataSource(0, 4, driver, attributes);
            if (_result == 1) return true;
            else return false;
        }

       #endregion

    }
}
