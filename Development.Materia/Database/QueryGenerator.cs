#region "imports"

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
    /// Sql statement generaing class.
    /// </summary>
    public class QueryGenerator
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of QueryGenerator.
        /// </summary>
        /// <param name="table">System.Data.DataTable to eveluate</param>
        public QueryGenerator(DataTable table)
        {
            _excludedfields = new ExcludedDataSourceFieldCollection(this);
            _foreignkey = new ForeignKeyInfo(this);
            _primarykey = new PrimaryKeyInfo(this);
            _table = table;
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable (DebuggerBrowsableState.Never)]
        private ExcludedDataSourceFieldCollection _excludedfields = null;

        /// <summary>
        /// Gets the list of the current evaluated table's field that will be excluded from the sql statement generation.
        /// </summary>
        public ExcludedDataSourceFieldCollection ExcludedFields
        {
            get { return _excludedfields; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ForeignKeyInfo _foreignkey = null;

        /// <summary>
        /// Gets the foreign key information affixed to the current evaluated table.
        /// </summary>
        public ForeignKeyInfo ForeignKey
        {
            get { return _foreignkey; }    
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PrimaryKeyInfo _primarykey = null;

        /// <summary>
        /// Gets the primary key information affixed to the current evaluated table.
        /// </summary>
        public PrimaryKeyInfo PrimaryKey
        {
            get { return _primarykey; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataTable _table = null;

        /// <summary>
        /// Gets the current evaluated table to where the sql statements will be produced.
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
        }

        #endregion

        #region "methods"

        private string Generate()
        {
            StringBuilder _sql = new StringBuilder();

            if (_table != null)
            {
                if (_table.Columns.Count > 0)
                {
                    string _pk = _primarykey.Field;

                    if (String.IsNullOrEmpty(_pk.RLTrim()))
                    {
                        foreach (DataColumn _col in _table.Columns)
                        {
                            if (_col.Unique)
                            {
                                _pk = _col.ColumnName; break;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(_pk.RLTrim()))
                    {
                        string _tablename = _table.TableName;
                        if (String.IsNullOrEmpty(_tablename.RLTrim())) _tablename = "table";

                        string _insert = ""; string _update = "";
                        string _pkvalue = "{" + _table.Columns[_pk].Ordinal.ToString() + "}";
                        if (!String.IsNullOrEmpty(_primarykey.Value.RLTrim())) _pkvalue = _primarykey.Value;
                        string _delete = "DELETE FROM `" + _tablename + "` WHERE (`" + _pk + "` = " + _pkvalue + ");";

                        string _insertfields = ""; string _insertparameters = ""; string _updatefield = "";

                        foreach (DataColumn _column in _table.Columns)
                        {
                            if (!_column.AutoIncrement &&
                                !_excludedfields.Contains(_column.ColumnName))
                            {
                                _insertfields += (String.IsNullOrEmpty(_insertfields.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "`";

                                if (_column.ColumnName != _foreignkey.Field)
                                {
                                    _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") +  "{" + _column.Ordinal.ToString() + "}";
                                    _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = {" + _column.Ordinal.ToString() + "}";
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(_foreignkey.Value.RLTrim()))
                                    {
                                        if (_foreignkey.HeaderTable != null)
                                        {
                                            _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                            _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                        }
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(_foreignkey.HeaderPrimaryKey.RLTrim()))
                                            {
                                                if (_foreignkey.HeaderTable.Columns.Contains(_foreignkey.HeaderPrimaryKey))
                                                {
                                                    DataColumn _headercolumn = _foreignkey.HeaderTable.Columns[_foreignkey.HeaderPrimaryKey];
                                                    if (_headercolumn.AutoIncrement)
                                                    {
                                                        if (_foreignkey.HeaderTable.Rows.Count > 0)
                                                        {
                                                            DataRow rw = null;

                                                            foreach (DataRow row in _foreignkey.HeaderTable.Rows)
                                                            {
                                                                if (row.RowState != DataRowState.Deleted &&
                                                                    row.RowState != DataRowState.Detached)
                                                                {
                                                                    rw = row; break;
                                                                }
                                                            }

                                                            if (rw != null)
                                                            {
                                                                if (rw.RowState == DataRowState.Added)
                                                                {
                                                                    _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                                    _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                                                }
                                                                else
                                                                {
                                                                    if (!Materia.IsNullOrNothing(rw[_foreignkey.HeaderPrimaryKey]))
                                                                    {
                                                                        _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + "'" + rw[_foreignkey.HeaderPrimaryKey].ToString().ToSqlValidString() + "'";
                                                                        _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + "'" + rw[_foreignkey.HeaderPrimaryKey].ToString().ToSqlValidString() + "'";
                                                                    }
                                                                    else
                                                                    {
                                                                        _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                                        _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                                _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                            _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                    _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                                }
                                            }
                                            else
                                            {
                                                _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + _foreignkey.Value;
                                                _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = " + _foreignkey.Value;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _insertparameters += (String.IsNullOrEmpty(_insertparameters.RLTrim()) ? "" : ", ") + "{" + _column.Ordinal.ToString() + "}";
                                        _updatefield += (String.IsNullOrEmpty(_updatefield.RLTrim()) ? "" : ", ") + "`" + _column.ColumnName + "` = {" + _column.Ordinal.ToString() + "}";
                                    }
                                }
                            }
                        }

                        _insert = "INSERT INTO `" + _tablename + "`\n" +
                                  "(" + _insertfields + ")\n" +
                                  "VALUES\n" +
                                  "(" + _insertparameters + ");";

                        string _query = "";

                        foreach (DataRow row in _table.Rows)
                        {
                            if (row.RowState != DataRowState.Unchanged)
                            {
                                _query = ""; _update = "UPDATE `" + _tablename + "` SET " + _updatefield + " WHERE (`" + _pk + "` = " + _pkvalue + ");";

                                string[] _values = new string[_table.Columns.Count];

                                foreach (DataColumn _column in _table.Columns)
                                {
                                    string _value = "NULL";
                                    object _currentvalue = null;

                                    if (row.RowState == DataRowState.Deleted ||
                                        row.RowState == DataRowState.Detached)
                                    {
                                        try { _currentvalue = row[_column.ColumnName, DataRowVersion.Original]; }
                                        catch { }
                                    }
                                    else _currentvalue = row[_column.ColumnName];

                                    if (row.RowState == DataRowState.Modified)
                                    {
                                        if (_column.ColumnName == _pk)
                                        {
                                            object _originalvalue = null;

                                            try
                                            {
                                                _originalvalue = row[_column.ColumnName, DataRowVersion.Original];
                                                if (_originalvalue != _currentvalue)
                                                {
                                                    string _originalpk = "NULL";

                                                    if (_column.DataType.Name == typeof(string).Name ||
                                                        _column.DataType.Name == typeof(String).Name) _originalpk = "'" + _originalvalue.ToString().ToSqlValidString() + "'";
                                                    else if (_column.DataType.Name == typeof(DateTime).Name)
                                                    {
                                                        if (VisualBasic.IsDate(_originalvalue)) _originalpk = "'" + VisualBasic.CDate(_originalvalue).ToSqlValidString(true) + "'";
                                                    }
                                                    else if (_column.DataType.Name == typeof(byte).Name ||
                                                             _column.DataType.Name == typeof(Byte).Name ||
                                                             _column.DataType.Name == typeof(decimal).Name ||
                                                             _column.DataType.Name == typeof(Decimal).Name ||
                                                             _column.DataType.Name == typeof(double).Name ||
                                                             _column.DataType.Name == typeof(Double).Name ||
                                                             _column.DataType.Name == typeof(float).Name ||
                                                             _column.DataType.Name == typeof(int).Name ||
                                                             _column.DataType.Name == typeof(Int16).Name ||
                                                             _column.DataType.Name == typeof(Int32).Name ||
                                                             _column.DataType.Name == typeof(Int64).Name ||
                                                             _column.DataType.Name == typeof(long).Name ||
                                                             _column.DataType.Name == typeof(sbyte).Name ||
                                                             _column.DataType.Name == typeof(SByte).Name ||
                                                             _column.DataType.Name == typeof(short).Name ||
                                                             _column.DataType.Name == typeof(Single).Name)
                                                    {
                                                        if (VisualBasic.IsNumeric(_originalvalue)) _originalpk = _originalvalue.ToString();
                                                    }
                                                    else if (_column.DataType.Name == typeof(bool).Name ||
                                                             _column.DataType.Name == typeof(Boolean).Name)
                                                    {
                                                        try
                                                        {
                                                            if (VisualBasic.CBool(_originalvalue)) _originalpk = "1";
                                                            else _originalpk = "0";
                                                        }
                                                        catch { }
                                                    }
                                                    else
                                                    {
                                                        if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                                                            _column.DataType.Name.ToLower().Contains("byte()") ||
                                                            _column.DataType.Name.ToLower().Contains("bytes[]") ||
                                                            _column.DataType.Name.ToLower().Contains("bytes()"))
                                                        {
                                                            try
                                                            { _originalpk = "x'" + ((byte[])_originalvalue).ToHexadecimalString().ToSqlValidString() + "'"; }
                                                            catch { _originalpk = "NULL"; }
                                                        }
                                                    }

                                                    _update = "UPDATE `" + _tablename + "` SET " + _updatefield + " WHERE (`" + _pk + "` = " + _originalpk + ");";
                                                }
                                            }
                                            catch { }
                                        }
                                    }

                                    if (!Materia.IsNullOrNothing(_currentvalue))
                                    {
                                        if (_column.DataType.Name == typeof(string).Name ||
                                            _column.DataType.Name == typeof(String).Name) _value = "'" + _currentvalue.ToString().ToSqlValidString() + "'";
                                        else if (_column.DataType.Name == typeof(DateTime).Name)
                                        {
                                            if (VisualBasic.IsDate(_currentvalue)) _value = "'" + VisualBasic.CDate(_currentvalue).ToSqlValidString(true) + "'";
                                        }
                                        else if (_column.DataType.Name == typeof(byte).Name ||
                                                 _column.DataType.Name == typeof(Byte).Name ||
                                                 _column.DataType.Name == typeof(decimal).Name ||
                                                 _column.DataType.Name == typeof(Decimal).Name ||
                                                 _column.DataType.Name == typeof(double).Name ||
                                                 _column.DataType.Name == typeof(Double).Name ||
                                                 _column.DataType.Name == typeof(float).Name ||
                                                 _column.DataType.Name == typeof(int).Name ||
                                                 _column.DataType.Name == typeof(Int16).Name ||
                                                 _column.DataType.Name == typeof(Int32).Name ||
                                                 _column.DataType.Name == typeof(Int64).Name ||
                                                 _column.DataType.Name == typeof(long).Name ||
                                                 _column.DataType.Name == typeof(sbyte).Name ||
                                                 _column.DataType.Name == typeof(SByte).Name ||
                                                 _column.DataType.Name == typeof(short).Name ||
                                                 _column.DataType.Name == typeof(Single).Name)
                                        {
                                            if (VisualBasic.IsNumeric(_currentvalue)) _value = _currentvalue.ToString();
                                        }
                                        else if (_column.DataType.Name == typeof(bool).Name ||
                                                 _column.DataType.Name == typeof(Boolean).Name)
                                        {
                                            try
                                            {
                                                if (VisualBasic.CBool(_currentvalue)) _value = "1";
                                                else _value = "0";
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                                                _column.DataType.Name.ToLower().Contains("byte()") ||
                                                _column.DataType.Name.ToLower().Contains("bytes[]") ||
                                                _column.DataType.Name.ToLower().Contains("bytes()"))
                                            {
                                                try
                                                { _value = "x'" + ((byte[]) _currentvalue).ToHexadecimalString().ToSqlValidString() +"'"; }
                                                catch { _value = "NULL"; }
                                            }
                                        }
                                    }

                                    _values[_column.Ordinal] = _value;  
                                }

                                switch (row.RowState)
                                {
                                    case DataRowState.Added:
                                        _query = _insert; break;
                                    case DataRowState.Modified:
                                        _query = _update; break;
                                    case DataRowState.Deleted:
                                        _query = _delete; break;
                                    case DataRowState.Detached:
                                    default: break;
                                }

                                _query = String.Format(_query, _values);
                                string _currentsql = _sql.ToString();
                                if (!String.IsNullOrEmpty(_currentsql.RLTrim())) _sql.Append("\n");
                                _sql.Append(_query);
                            }
                        }
                    }
                }
            }

            return _sql.ToString();
        }

        /// <summary>
        /// Returns the generated sql statement based on the current hosted DataTable object's row values and states.
        /// </summary>
        /// <returns>Sql statement generated from th specified table's modifications.</returns>
        public override string ToString()
        {
            string _sql = ""; string _query = Generate();

            if (!String.IsNullOrEmpty(_query.RLTrim()))
            {
                bool _blobexists = false;

                if (_table != null)
                {
                    foreach (DataColumn _column in _table.Columns)
                    {
                        if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                            _column.DataType.Name.ToLower().Contains("byte()") ||
                            _column.DataType.Name.ToLower().Contains("bytes[]") ||
                            _column.DataType.Name.ToLower().Contains("bytes()"))
                        {
                            _blobexists = true; break;
                        }
                    }
                }

                if (_blobexists) _sql = "SET GLOBAL max_allowed_packet = (1024 * 1024) * " + MySql.MaxAllowedPacket.ToString() + ";";
                _sql += (String.IsNullOrEmpty(_sql.RLTrim()) ? "" : "\n") + _query;
            }

            return  _query;
        }

        #endregion

    }

    /// <summary>
    /// List of QueryGenerator's excluded fields.
    /// </summary>
    public class ExcludedDataSourceFieldCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of ExcludedDataSourceFieldCollection.
        /// </summary>
        /// <param name="generator">Development.Materia.Database.QueryGenerator that owne the class.</param>
        public ExcludedDataSourceFieldCollection(QueryGenerator generator)
        { _generator = generator; }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets the current excluded field name at the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get { return List[index].ToString(); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private QueryGenerator _generator = null;

        /// <summary>
        /// Gets the current hosted sql statement generator.
        /// </summary>
        public QueryGenerator Generator
        {
            get { return _generator; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a execluded data source field name into the collection.
        /// </summary>
        /// <param name="field">Table's field name</param>
        /// <returns>Index of the newly added field within the collection.</returns>
        public int Add(string field)
        {
            Remove(field); return List.Add(field);
        }

        /// <summary>
        /// Adds a range of excluded data source field names into the collection.
        /// </summary>
        /// <param name="fields">Table's field names.</param>
        /// <returns>List of the indexes of each newly added field within the collection.</returns>
        public List<int> Add(params string[] fields)
        {
            List<int> _list = new List<int>();

            foreach (string field in fields) _list.Add(Add(field));

            return _list;
        }

        /// <summary>
        /// Returns whether the specified field name already exists in the collection.
        /// </summary>
        /// <param name="field">Table field name to evaluate.</param>
        /// <returns>True if the field name already exists within the collection otherwise false.</returns>
        public bool Contains(string field)
        { return List.Contains(field); }

        /// <summary>
        /// Removes the specified field name from the collection.
        /// </summary>
        /// <param name="field">Table field name to remove.</param>
        public void Remove(string field)
        {
            if (List.Contains(field)) List.Remove(field);
        }

        #endregion

    }

    /// <summary>
    /// Data source foreign key information.
    /// </summary>
    public class ForeignKeyInfo : PrimaryKeyInfo
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of ForeignKeyInfo.
        /// </summary>
        /// <param name="generator">Development.Materia.Database.QueryGenerator that owns the current class.</param>
        public ForeignKeyInfo(QueryGenerator generator)  : base(generator)
        { }

        /// <summary>
        /// Creates a new instance of ForeignKeyInfo.
        /// </summary>
        /// <param name="generator">Development.Materia.Database.QueryGenerator that owns the current class.</param>
        /// <param name="field">Table's field name.</param>
        /// <param name="value">Field's assigned value.</param>
        public ForeignKeyInfo(QueryGenerator generator, string field, string value) : base(generator, field, value)
        { }

        #endregion

        #region "additional properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _headerprimarykey = "";

        /// <summary>
        /// Gets or sets the currented header table's reference primary key field name.
        /// </summary>
        public string HeaderPrimaryKey
        {
            get { return _headerprimarykey; }
            set { _headerprimarykey = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataTable _headertable = null;

        /// <summary>
        /// Gets or sets the current parented header table for the header-detail key referencing.
        /// </summary>
        public DataTable HeaderTable
        {
            get { return _headertable; }
            set { _headertable = value; }
        }

        #endregion

    }

    /// <summary>
    /// Data source primary key information.
    /// </summary>
    public class PrimaryKeyInfo
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of PrimaryKeyInfo.
        /// </summary>
        /// <param name="generator">Development.Materia.Database.QueryGenerator that owns the current class.</param>
        public PrimaryKeyInfo(QueryGenerator generator) : this(generator, "", "")
        { }

        /// <summary>
        /// Creates a new instance of PrimaryKeyInfo.
        /// </summary>
        /// <param name="generator">Development.Materia.Database.QueryGenerator that owns the current class.</param>
        /// <param name="field">Table's field name.</param>
        /// <param name="value">Field's assigned value.</param>
        public PrimaryKeyInfo(QueryGenerator generator, string field, string value)
        { _generator = generator; _field = field; _value = value; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _field = "";

        /// <summary>
        /// Gets or sets the key field name.
        /// </summary>
        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private QueryGenerator _generator = null;

        /// <summary>
        /// Gets the current hosted query generator.
        /// </summary>
        public QueryGenerator Generator
        {
            get { return _generator; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _value = "";

        /// <summary>
        /// Gets or sets the affixed value for the current generator's key field. Assigned value should already be in sql-qualified format.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

    }

}
