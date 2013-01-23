#region "imports"

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

#endregion

namespace Development.Materia
{

    /// <summary>
    /// DataTable and DataSet standalone MS Excel file writer.
    /// </summary>
    public class DataSourceExcelWriter : IDisposable
    {

        #region "enumerations"

        /// <summary>
        /// Excel spreadsheet cell style enumerations.
        /// </summary>
        public enum CellStyles
        {
            /// <summary>
            /// Currency represented numeric values.
            /// </summary>
            Currency = 2,
            /// <summary>
            /// Date and time
            /// </summary>
            DateTime = 3,
            /// <summary>
            /// General types basically strings.
            /// </summary>
            General = 0,
            /// <summary>
            /// Numeric values.
            /// </summary>
            Number  = 1,
            /// <summary>
            /// Date
            /// </summary>
            ShortDate = 4
        }

        #endregion

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataSourceExcelWriter.
        /// </summary>
        /// <param name="datasource">Datasource table to export</param>
        public DataSourceExcelWriter(DataTable datasource) : this(datasource, "")
        { }

        /// <summary>
        /// Creates a new instance of DataSourceExcelWriter.
        /// </summary>
        /// <param name="datasource">Datasource table to export</param>
        /// <param name="filename">Exporting file path</param>
        public DataSourceExcelWriter(DataTable datasource, string filename)
        {
            _datasource = datasource; _filename = filename; InitializeWriter();
        }

        /// <summary>
        /// Creates a new instance of DataSourceExcelWriter.
        /// </summary>
        /// <param name="datasource">Datasource tables to export</param>
        public DataSourceExcelWriter(DataSet datasource) : this(datasource, "")
        { }

        /// <summary>
        /// Creates a new instance of DataSourceExcelWriter.
        /// </summary>
        /// <param name="datasource">Datasource tables to export</param>
        /// <param name="filename">Exporting file path</param>
        public DataSourceExcelWriter(DataSet datasource, string filename)
        {
            _datasource = datasource; _filename = filename; InitializeWriter();
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _datasource = null;

        /// <summary>
        /// Gets the initialized datasource object to export the contents from.
        /// </summary>
        public object DataSource
        {
            get { return _datasource; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _filename = "";

        /// <summary>
        /// Gets or sets the exporting file's path.
        /// </summary>
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string _urn = "urn:schemas-microsoft-com:office:spreadsheet";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XmlWriter _writer = null;

        #endregion

        #region "methods"

        private void Close()
        {
            if (_writer != null)
            {
                try { _writer.Close(); }
                catch { }
                finally 
                {
                    _writer = null; Materia.RefreshAndManageCurrentProcess();
                }
            }
        }

        private void InitializeWriter()
        {
            if (_writer == null)
            {
                XmlWriterSettings _settings = new XmlWriterSettings(); _settings.Indent = true;
                _writer = XmlWriter.Create(_filename, _settings);
            }
        }

        /// <summary>
        /// Writes the current associated data source object into the specified excel path and returns the file's information if data exportation is successful.
        /// </summary>
        /// <returns>System.IO.FileInfo object that associates the written excel file derived from the current source table. Returns nothing if error has been encountered during file-writing process.</returns>
        public FileInfo Write()
        {
            FileInfo _file = null;

            if (_datasource != null)
            {
                DataSet _dataset = null;

                if (_datasource.GetType().Name == typeof(DataSet).Name) _dataset =  (DataSet) _datasource;
                else if (_datasource.GetType().Name == typeof(DataTable).Name)
                {
                    _dataset = new DataSet(); _dataset.Tables.Add((DataTable)_datasource);
                }

                if (_dataset != null)
                {
                    if (WriteTable(_dataset))
                    {
                        if (File.Exists(_filename)) _file = new FileInfo(_filename);
                    }
                }
            }

            return _file;
        }

        private void WriteEndDocument()
        {
            if (_writer != null) _writer.WriteEndElement();
        }

        private void WriteEndRow()
        {
            if (_writer != null) _writer.WriteEndElement();
        }

        private void WriteEndWorkSheet()
        {
            if (_writer != null) _writer.WriteEndElement();
        }

        private void WriteExcelAutoStyledCell(object value)
        {
            if (_writer != null)
            {
                object _curvalue = value;

                if (Materia.IsNullOrNothing(value)) _curvalue = Materia.GetDefaultValueByType(value.GetType());
                if (Materia.IsNullOrNothing(_curvalue)) _curvalue = "";

                if (_curvalue.GetType().Name == typeof(string).Name ||
                    _curvalue.GetType().Name == typeof(String).Name) WriteExcelStyledCell(_curvalue, CellStyles.General);
                else if (_curvalue.GetType().Name == typeof(byte).Name ||
                         _curvalue.GetType().Name == typeof(Byte).Name ||
                         _curvalue.GetType().Name == typeof(int).Name ||
                         _curvalue.GetType().Name == typeof(Int16).Name ||
                         _curvalue.GetType().Name == typeof(Int32).Name ||
                         _curvalue.GetType().Name == typeof(Int64).Name ||
                         _curvalue.GetType().Name == typeof(sbyte).Name ||
                         _curvalue.GetType().Name == typeof(SByte).Name ||
                         _curvalue.GetType().Name == typeof(long).Name ||
                         _curvalue.GetType().Name == typeof(short).Name) WriteExcelStyledCell(_curvalue, CellStyles.Number);
                else if (_curvalue.GetType().Name == typeof (decimal).Name ||
                         _curvalue.GetType().Name == typeof (Decimal).Name ||
                         _curvalue.GetType().Name == typeof (double).Name ||
                         _curvalue.GetType().Name == typeof (Double).Name ||
                         _curvalue.GetType().Name == typeof (float).Name ||
                         _curvalue.GetType().Name == typeof(Single).Name) WriteExcelStyledCell(_curvalue, CellStyles.Currency);
                else if (_curvalue.GetType().Name == typeof(DateTime).Name) WriteExcelStyledCell(_curvalue, CellStyles.DateTime);
                else WriteExcelStyledCell(_curvalue, CellStyles.General);
            }
        }

        private void WriteExcelColumnDefinition(int columnwidth)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Column", _urn);
                _writer.WriteStartAttribute("Width", _urn);
                _writer.WriteValue(columnwidth);
                _writer.WriteEndAttribute();
                _writer.WriteEndElement();
            }
        }

        private void WriteExcelStyledCell(object value, CellStyles style)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Cell", _urn);
                _writer.WriteAttributeString("StyleID", _urn, Enum.GetName(typeof(CellStyles), style));
                _writer.WriteStartElement("Data", _urn);

                string _type = "";
                switch (style)
                {
                    case CellStyles.General :
                        _type = "String"; break;
                    case CellStyles.Currency:
                    case CellStyles.Number:
                        _type = "Number"; break;
                    case CellStyles.DateTime:
                    case CellStyles.ShortDate:
                        _type = "DateTime"; break;
                    default: break;
                }

                _writer.WriteAttributeString("Type", _urn, _type);
                _writer.WriteValue(value);
                _writer.WriteEndElement(); _writer.WriteEndElement();
            }
        }

        private void WriteExcelStyleElement(CellStyles style)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Style", _urn);
                _writer.WriteAttributeString("ID", _urn, Enum.GetName(typeof(CellStyles), style));
                _writer.WriteEndElement();
            }
        }

        private void WriteExcelStyleElement(CellStyles style, string numberformat)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Style", _urn);
                _writer.WriteAttributeString("ID", _urn, Enum.GetName(typeof(CellStyles), style));
                _writer.WriteStartElement("NumberFormat", _urn);
                _writer.WriteAttributeString("Format", _urn, numberformat);
                _writer.WriteEndElement(); _writer.WriteEndElement();
            }
        }

        private void WriteExcelStyles()
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Styles", _urn);
                WriteExcelStyleElement(CellStyles.General);
                WriteExcelStyleElement(CellStyles.Number, "General Number");
                WriteExcelStyleElement(CellStyles.DateTime, "General Date");
                WriteExcelStyleElement(CellStyles.Currency, "Currency");
                WriteExcelStyleElement(CellStyles.ShortDate, "Short Date");
                _writer.WriteEndElement();
            }
        }

        private void WriteExcelUnstyledCell(string value)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Cell", _urn);
                _writer.WriteStartElement("Data", _urn);
                _writer.WriteAttributeString("Type", _urn, "String");
                _writer.WriteValue(value);
                _writer.WriteEndElement(); _writer.WriteEndElement();
            }
        }

        private void WriteStartDocument()
        {
            if (_writer != null)
            {
                _writer.WriteProcessingInstruction("mso-application", "progid=\\\"Excel.Sheet\\\"");
                _writer.WriteStartElement("ss", "Workbook", _urn); WriteExcelStyles();
            }
        }

        private void WriteStartRow()
        {
            if (_writer != null) _writer.WriteStartElement("Row", _urn);
        }

        private void WriteStartWorksheet(string worksheet)
        {
            if (_writer != null)
            {
                _writer.WriteStartElement("Worksheet", _urn);
                _writer.WriteAttributeString("Name", _urn, worksheet);
                _writer.WriteStartElement("Table", _urn);
            }
        }

        private bool WriteTable(DataSet tables)
        {
            bool _written = false; InitializeWriter();

            if (_writer != null)
            {
                if (tables != null)
                {
                    if (tables.Tables.Count > 0)
                    {
                        try
                        {
                            int _unnamed = 0; WriteStartDocument();
                            for (int i = 0; i <= (tables.Tables.Count - 1); i++)
                            {
                                DataTable _table = tables.Tables[i];
                                if (_table != null)
                                {
                                    string _worksheet = _table.TableName;
                                    
                                    if (String.IsNullOrEmpty(_worksheet.RLTrim()))
                                    {
                                        _unnamed += 1; _worksheet = "Table" + _unnamed.ToString();
                                    }

                                    WriteStartWorksheet(_worksheet); WriteStartRow();
                                    List<string> _includedcolumns = new List<string>();

                                    for (int col = 0; col <= (_table.Columns.Count - 1); col++)
                                    {
                                        DataColumn _column = _table.Columns[col];
                                        if (_column.DataType != null)
                                        {
                                            if (_column.DataType.Name == typeof(string).Name ||
                                                _column.DataType.Name == typeof(String).Name ||
                                                _column.DataType.Name == typeof(byte).Name ||
                                                _column.DataType.Name == typeof(Byte).Name ||
                                                _column.DataType.Name == typeof(bool).Name ||
                                                _column.DataType.Name == typeof(Boolean).Name ||
                                                _column.DataType.Name == typeof(DateTime).Name ||
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
                                                _includedcolumns.Add(_column.ColumnName);
                                                WriteExcelUnstyledCell(_column.ColumnName);
                                            }
                                        }
                                    }
                                    WriteEndRow();

                                    foreach (DataRow rw in _table.Rows)
                                    {
                                        if (rw.RowState != DataRowState.Deleted &&
                                            rw.RowState != DataRowState.Detached)
                                        {
                                            WriteStartRow();
                                            foreach (string _includedcolumn in _includedcolumns) WriteExcelAutoStyledCell(rw[_includedcolumn]);
                                            WriteEndRow();
                                        }
                                    }

                                    WriteEndWorkSheet();
                                }
                            }

                            WriteEndDocument(); Close(); _written = true;
                        }
                        catch { _written = false; }
                    }
                }
            }

            return _written;
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
                    Close();
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }
}
