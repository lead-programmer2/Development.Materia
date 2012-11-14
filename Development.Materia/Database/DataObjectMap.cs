#region "methods"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

#endregion

namespace Development.Materia.Database
{
    /// <summary>
    /// ORM-approached class for database table contents updating.
    /// </summary>
    public class DataObjectMap : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataObjectMap.
        /// </summary>
        /// <param name="dbconnection"></param>
        /// <param name="tablename"></param>
        public DataObjectMap(IDbConnection dbconnection, string tablename) : this(dbconnection, tablename, "")
        { }

        /// <summary>
        /// Creates a new instance of DataObjectMap.
        /// </summary>
        /// <param name="dbconnection"></param>
        /// <param name="tablename"></param>
        /// <param name="condition"></param>
        public DataObjectMap(IDbConnection dbconnection, string tablename, string condition)
        {
            _connection = dbconnection; _databasetable = tablename;
            _filtercondition = condition; Refresh();
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDbConnection _connection = null;

        /// <summary>
        /// Gets the instantiated DataObjectMap's database connection.
        /// </summary>
        public IDbConnection Connection
        {
            get { return _connection; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _databasetable = "";

        /// <summary>
        /// Gets the instantiated DataObjectMap's database table.
        /// </summary>
        public string DatabaseTable
        {
            get { return _databasetable; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _filtercondition = "";

        /// <summary>
        /// Gets the instantiated DataObjectMap's database filter condition.
        /// </summary>
        public string FilterCondition
        {
            get { return _filtercondition; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataTable _table = null;

        /// <summary>
        /// Gets the current contents of the instantiated DataObjectMap.
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
        }

        #endregion

        #region "methods"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Hashtable _delegatetable = new Hashtable();

        /// <summary>
        /// Adds a new row into the DataObjectMap's table
        /// </summary>
        /// <returns></returns>
        public DataRow AddRow()
        {
            if (_table != null) return _table.Rows.Add();
            else return null;
        }

        /// <summary>
        /// Adds a new row into the DataObjectMap's table
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public DataRow AddRow(object[] values)
        {
            if (_table != null) return _table.Rows.Add(values);
            else return null;
        }

        /// <summary>
        /// Applies table updates from the specified DataTable object into the current DataObjectMap's table.
        /// </summary>
        /// <param name="table"></param>
        public void ApplyUpdatesFromTable(DataTable table)
        {
            if (_table != null &&
                table != null)
            {
                int _rowdiff = _table.Rows.Count - table.Rows.Count;
                if (_rowdiff > 0) DeleteRows(_rowdiff);

                string _pk = "";

                foreach (DataColumn _column in _table.Columns)
                {
                    if (_column.Unique)
                    {
                        _pk = _column.ColumnName; break;
                    }
                }

                if (String.IsNullOrEmpty(_pk.RLTrim()))
                {
                    foreach (DataColumn _column in table.Columns)
                    {
                        if (_column.Unique &&
                            _table.Columns.Contains(_column.ColumnName))
                        {
                            _pk = _column.ColumnName; break;
                        }
                    }

                    if (String.IsNullOrEmpty(_pk.RLTrim())) _pk = _table.Columns[0].ColumnName;
                }

                DataColumn _pkcol = _table.Columns[_pk];
                int _currentrow = 0;

                foreach (DataRow _row in table.Rows)
                {
                    object _pkvalue = null;

                    switch (_row.RowState)
                    {
                        case DataRowState.Detached:
                        case DataRowState.Deleted:
                            try { _pkvalue = _row[_pk, DataRowVersion.Original]; }
                            catch { _pkvalue = null; }

                            if (!Materia.IsNullOrNothing(_pkvalue))
                            {
                                string _filter = "[" + _pk + "] = ";
                                if (_pkcol.DataType.Name == typeof(String).Name ||
                                    _pkcol.DataType.Name == typeof(string).Name) _filter += "'" + _pkvalue.ToString().ToSqlValidString(true) + "'";
                                else if (_pkcol.DataType.Name == typeof(DateTime).Name)
                                {
                                    if (VisualBasic.IsDate(_pkvalue)) _filter += "#" + VisualBasic.Format(VisualBasic.CDate(_pkvalue), "MM/dd/yyyy hh:mm:ss tt") + "#";
                                    else _filter = "";
                                }
                                else if (_pkcol.DataType.Name == typeof(bool).Name ||
                                        _pkcol.DataType.Name == typeof(Boolean).Name) 
                                {
                                    bool _value = VisualBasic.CBool(_pkvalue);
                                    _filter += _value.ToString();
                                }
                                else if (_pkcol.DataType.Name == typeof(byte).Name ||
                                         _pkcol.DataType.Name == typeof(Byte).Name ||
                                         _pkcol.DataType.Name == typeof(decimal).Name ||
                                         _pkcol.DataType.Name == typeof(Decimal).Name ||
                                         _pkcol.DataType.Name == typeof(double).Name ||
                                         _pkcol.DataType.Name == typeof(Double).Name ||
                                         _pkcol.DataType.Name == typeof(int).Name ||
                                         _pkcol.DataType.Name == typeof(Int16).Name ||
                                         _pkcol.DataType.Name == typeof(Int32).Name ||
                                         _pkcol.DataType.Name == typeof(Int64).Name ||
                                         _pkcol.DataType.Name == typeof(long).Name ||
                                         _pkcol.DataType.Name == typeof(short).Name ||
                                         _pkcol.DataType.Name == typeof(Single).Name ||
                                         _pkcol.DataType.Name == typeof(sbyte).Name ||
                                         _pkcol.DataType.Name == typeof(SByte).Name)
                                {
                                    if (VisualBasic.IsNumeric(_pkvalue)) _filter += _pkvalue.ToString();
                                    else _filter = "";
                                }
                                else _filter = "";

                                if (!String.IsNullOrEmpty(_filter.RLTrim())) DeleteRow(_filter);
                            }
                            break;
                        case DataRowState.Modified:
                        case DataRowState.Added:
                            if (_row.RowState == DataRowState.Modified)
                            {
                                 try { _pkvalue = _row[_pk, DataRowVersion.Original]; }
                                catch { _pkvalue = null; }
                            }
                            else _pkvalue = _row[_pk];
                          
                             if (!Materia.IsNullOrNothing(_pkvalue))
                             {
                                string _filter = "[" + _pk + "] = ";
                                if (_pkcol.DataType.Name == typeof(String).Name ||
                                    _pkcol.DataType.Name == typeof(string).Name) _filter += "'" + _pkvalue.ToString().ToSqlValidString(true) + "'";
                                else if (_pkcol.DataType.Name == typeof(DateTime).Name) 
                                {
                                     if (VisualBasic.IsDate(_pkvalue)) _filter += "#" + VisualBasic.Format(VisualBasic.CDate(_pkvalue), "MM/dd/yyyy hh:mm:ss tt") + "#";
                                     else _filter = "";
                                }
                                else if (_pkcol.DataType.Name == typeof(bool).Name ||
                                         _pkcol.DataType.Name == typeof(Boolean).Name) 
                                {
                                     bool _value = VisualBasic.CBool(_pkvalue);
                                     _filter += _value.ToString();
                                }
                                else if (_pkcol.DataType.Name == typeof(byte).Name ||
                                         _pkcol.DataType.Name == typeof(Byte).Name ||
                                         _pkcol.DataType.Name == typeof(decimal).Name ||
                                         _pkcol.DataType.Name == typeof(Decimal).Name ||
                                         _pkcol.DataType.Name == typeof(double).Name ||
                                         _pkcol.DataType.Name == typeof(Double).Name ||
                                         _pkcol.DataType.Name == typeof(int).Name ||
                                         _pkcol.DataType.Name == typeof(Int16).Name ||
                                         _pkcol.DataType.Name == typeof(Int32).Name ||
                                         _pkcol.DataType.Name == typeof(Int64).Name ||
                                         _pkcol.DataType.Name == typeof(long).Name ||
                                         _pkcol.DataType.Name == typeof(short).Name ||
                                         _pkcol.DataType.Name == typeof(Single).Name ||
                                         _pkcol.DataType.Name == typeof(sbyte).Name ||
                                         _pkcol.DataType.Name == typeof(SByte).Name)
                                {
                                    if (VisualBasic.IsNumeric(_pkvalue)) _filter += _pkvalue.ToString();
                                    else _filter = "";
                                }
                                else _filter = "";

                                if (!String.IsNullOrEmpty(_filter.RLTrim())) 
                                {
                                    DataRow[] _rows = Select(_filter);
                                    if (_rows.Length > 0) 
                                    {
                                        DataRow _actualrow = _rows[0];
                                        if (_actualrow.RowState != DataRowState.Deleted ||
                                            _actualrow.RowState != DataRowState.Detached)
                                        {
                                            foreach (DataColumn _col in _table.Columns)
                                            {
                                                if (!_col.AutoIncrement)
                                                {
                                                    if (table.Columns.Contains(_col.ColumnName)) _actualrow[_col.ColumnName] = _row[_col.ColumnName];
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DataRow _actualrow = null;
                                        if (_currentrow < _table.Rows.Count) _actualrow = _table.Rows[_currentrow];

                                        while (_actualrow.RowState == DataRowState.Deleted ||
                                               _actualrow.RowState == DataRowState.Detached) 
                                         {
                                            _currentrow += 1;
                                            if (_currentrow >= _table.Rows.Count) break;
                                            else _actualrow = _table.Rows[_currentrow];
                                         }

                                        if (_actualrow != null)
                                        {
                                            if (_actualrow.RowState == DataRowState.Deleted ||
                                                _actualrow.RowState == DataRowState.Detached) _actualrow = null;
                                        }
                                        else
                                        {
                                           if (_currentrow >= _table.Rows.Count) _actualrow = null;
                                        }
                                      
                                        if (_actualrow != null)
                                        {
                                             foreach (DataColumn _col in _table.Columns)
                                             {
                                                 if (!_col.AutoIncrement)
                                                 {
                                                     if (table.Columns.Contains(_col.ColumnName)) _actualrow[_col.ColumnName] = _row[_col.ColumnName];
                                                 }
                                             }
                                        }
                                        else
                                        {
                                            object[] _values = new object[_table.Columns.Count];
                                            foreach (DataColumn _col in _table.Columns)
                                            {
                                                if (!_col.AutoIncrement)
                                                {
                                                    if (table.Columns.Contains(_col.ColumnName)) _values[_col.Ordinal] = _row[_col.ColumnName];
                                                }
                                            }
                                            AddRow(_values);
                                        }
                                     }
                                   }
                                }

                            break;
                        default : break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously saves the current DataObjectMap's table updates into the connected database. Call the EndSave method after the synchornization is finish to get the actual sql execution result.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult BeginSave()
        {
            Func<QueResult> _delegate = new Func<QueResult>(Save);
            IAsyncResult _result = _delegate.BeginInvoke(null, _delegate);
            _delegatetable.Add(_result, _delegate);
            return _result;
        }

        /// <summary>
        /// Deletes all table rows from the DataObjectMap.
        /// </summary>
        public void DeleteAllRows()
        {
            if (_table != null)
            {
                DataRow[] _rows = _table.Select("");
                if (_rows != null)
                {
                    if (_rows.Length > 0)
                    {
                        IEnumerator _enumerator = _rows.GetEnumerator();
                        while (_enumerator.MoveNext())
                        {
                            DataRow _row = (DataRow)_enumerator.Current;
                            if (_row.RowState != DataRowState.Deleted &&
                                _row.RowState != DataRowState.Detached) _row.Delete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the table row at the specified index of the DataObjectMap's table.
        /// </summary>
        /// <param name="index"></param>
        public void DeleteRow(int index)
        {
            if (_table != null)
            {
                if (index < _table.Rows.Count &&
                    index >= 0)
                {
                    DataRow _row = _table.Rows[index];
                    if (_row.RowState != DataRowState.Detached &&
                        _row.RowState != DataRowState.Deleted) _row.Delete();
                }
            }
        }

        /// <summary>
        /// Deletes table rows that pass the specified condition from the DataObjectMap.
        /// </summary>
        /// <param name="condition"></param>
        public void DeleteRow(string condition)
        {
            if (_table != null && !String.IsNullOrEmpty(condition.RLTrim()))
            {
                DataRow[] _rows = _table.Select(condition);
                if (_rows != null)
                {
                    if (_rows.Length > 0)
                    {
                        IEnumerator _enumerator = _rows.GetEnumerator();
                        while (_enumerator.MoveNext())
                        {
                            DataRow _row = (DataRow)_enumerator.Current;
                            if (_row.RowState != DataRowState.Deleted &&
                                _row.RowState != DataRowState.Detached) _row.Delete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes specific number of rows to the current table of the DataObjectMap.
        /// </summary>
        /// <param name="rows"></param>
        public void DeleteRows(int rows)
        {
            if (_table != null)
            {
                int _rows = rows;
                if (_rows > _table.Rows.Count) _rows = _table.Rows.Count;
                if (_rows > 0)
                {
                    int _rowcount = _table.Rows.Count;
                    int _currentrow = _rowcount - 1;
                    for (int i = 1; i <= _rows; i++)
                    {
                        DeleteRow(_currentrow);
                        _currentrow = _rowcount - (i + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the actual sql execution result from an initialized BeginSave method call.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public QueResult EndSave(IAsyncResult result)
        {
            QueResult _result = null;

            if (result != null)
            {
                if (_delegatetable.ContainsKey(result))
                {
                    Func<QueResult> _delegate = (Func<QueResult>)_delegatetable[result];
                    _result = _delegate.EndInvoke(result);
                    _delegatetable.Remove(result);
                }
            }

            return _result;
        }

        /// <summary>
        /// Generates sql statement based on the current DataObjectMap's table value updates.
        /// </summary>
        /// <returns></returns>
        public string GenerateSqlStatement()
        { return GenerateSqlStatement(new string[]{ "" }); }

        /// <summary>
        /// Generates sql statement based on the current DataObjectMap's table value updates.
        /// </summary>
        /// <param name="excludedfields">List of table fields to be excluded from the sql statement generation.</param>
        /// <returns></returns>
        public string GenerateSqlStatement(string[] excludedfields)
        {
            string _sql = "";

            if (_table != null)
            {
                QueryGenerator _generator = new QueryGenerator(_table);
                if (excludedfields != null)
                {
                    foreach (string _exfield in excludedfields)
                    {
                        if (!String.IsNullOrEmpty(_exfield.RLTrim()))
                        {
                            if (_table.Columns.Contains(_exfield)) _generator.ExcludedFields.Add(_exfield);
                        }
                    }
                }
                _sql = _generator.ToString();
            }

            return _sql;
        }

        /// <summary>
        /// Retrieves data from the current connected database based on the current specified database table and filter condition.
        /// </summary>
        public void Refresh()
        {
            if (_table != null)
            {
                _table.Dispose(); _table = null;
                Materia.RefreshAndManageCurrentProcess();
            }

            if (_connection != null)
            {
                string _query = "SELECT *\n" +
                                "FROM\n" +
                                "`" + _databasetable + "`";

                if (!String.IsNullOrEmpty(_filtercondition.RLTrim())) _query += "\nWHERE\n" + _filtercondition;

                _table = _table.LoadData(_connection, _query);
            }
        }

        /// <summary>
        /// Asynchronously retrieves data from the current connected database based on the current specified database table and filter condition.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult RefreshAsync()
        {
            Action _delegate = new Action(Refresh);
            return _delegate.BeginInvoke(null, _delegate);
        }

        /// <summary>
        /// Saves the current DataObjectMap's table updates into the connected database.
        /// </summary>
        /// <returns></returns>
        public QueResult Save()
        {
            QueResult _result = null;

            if (_connection != null &&
                _table != null)
            {
                string _query = GenerateSqlStatement();
                if (!String.IsNullOrEmpty(_query.RLTrim())) _result = Que.Execute(_connection, _query);
            }

            return _result;
        }

        /// <summary>
        /// Gets the row at the specified index of the DataObjectMap's table
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataRow Select(int index)
        {
            if (_table != null) return _table.Rows[index];
            else return null;
        }

        /// <summary>
        /// Gets rows that pass the specified filter condition from the DataObjectMap's table.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataRow[] Select(string condition)
        {
            if (_table != null)
                if (String.IsNullOrEmpty(condition.RLTrim()))
                {
                    List<DataRow> _rows = new List<DataRow>();
                    return _rows.ToArray();
                }
                else return _table.Select(condition);
            else
            {
                List<DataRow> _rows = new List<DataRow>();
                return _rows.ToArray();
            }
        }

        /// <summary>
        /// Gets the current DataObjectMap's database table name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _databasetable;
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
                    if (_table != null)
                    {
                        _table.Dispose(); _table = null;
                    }

                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }
}
