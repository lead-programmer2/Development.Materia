#region "imports"
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
#endregion

namespace Development.Materia
{
    /// <summary>
    /// Commonly used shared methods.
    /// </summary>
    public static partial class Materia
    {
        #region "api"

        [DllImport("kernel32.dll")]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumworkingsetsize, int maximumworkingsetsize);
        
        #endregion

        /// <summary>
        /// Attaches event handler in a event with the specified name of the specified object. 
        /// </summary>
        /// <param name="owner">Object to attach the handler into</param>
        /// <param name="eventname">Object's event name</param>
        /// <param name="handler">Handler to attach</param>
        public static void AttachHandler(object owner, string eventname, Delegate handler)
        {
            if (owner != null)
            {
                EventInfo _event = GetEvent(owner, eventname);
                if (_event != null) _event.AddEventHandler(owner, handler);
            }
        }

        /// <summary>
        /// Converts the given byte array to its hexadecimal string representation.
        /// </summary>
        /// <param name="bytes">Array of byte to be interpreted.</param>
        /// <returns></returns>
        public static string ByteArrayToHexaDecimalString(byte[] bytes)
        {
            StringBuilder _hex = new StringBuilder();   
            string _result = "";

            for (int i = 0; i <=bytes.Length - 1; i++)
            { _hex.Append(bytes[i].ToString("X2")); }
            
            try
            { _result = BitConverter.ToString(BitConverter.GetBytes(VisualBasic.CLng(_hex.ToString()))).Replace("-", "");}
            catch { _result = _hex.ToString();}

            return _result;
        }

        #region "ByteArrayToFileObject"

        /// <summary>
        /// Converts the given byte array to its corresponding file with the specified file extension.
        /// </summary>
        /// <param name="bytes">Array of byte to be interpreted.</param>
        /// <param name="outputextension">Output file extension.</param>
        /// <returns></returns>
        public static FileInfo ByteArrayToFileObject(byte[] bytes, string outputextension)
        {
            return ByteArrayToFileObject(bytes, outputextension, Environment.CurrentDirectory);
        }

        /// <summary>
        /// Converts the given byte array to its corresponding file with the specified file extension.
        /// </summary>
        /// <param name="bytes">Array of byte to be interpreted.</param>
        /// <param name="outputextension">Output file extension.</param>
        /// <param name="outputdirectory">The output directory for the specified file.</param>
        /// <returns></returns>
        public static FileInfo ByteArrayToFileObject(byte[] bytes, string outputextension, string outputdirectory)
        {
            FileInfo _file = null;

            if (!Directory.Exists(outputdirectory))
            {
                try
                { Directory.CreateDirectory(outputdirectory); }
                catch { }
            }

            if (Directory.Exists(outputdirectory))
            {
                try
                {
                    string _filename = outputdirectory + (outputdirectory.RLTrim().EndsWith("\\") ? "" : "\\") + "file." + outputextension;
                    if (File.Exists(_filename)) File.Delete(_filename);
                    File.WriteAllBytes(_filename, bytes);
                    if (File.Exists(_filename)) _file = new FileInfo(_filename);
                }
                catch { _file = null; }
            }

            return _file;
        }

        #endregion

        /// <summary>
        /// Converts the given blob byte array to its image representation.
        /// </summary>
        /// <param name="bytes">Array of byte to be interpreted.</param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] bytes)
        {
            Image _image = null;
            MemoryStream _stream = new MemoryStream(bytes);

            try
            {  _image = Image.FromStream(_stream); }
            catch  { }
            finally
            {
                _stream.Close(); _stream.Dispose();
                RefreshAndManageCurrentProcess();
            }

            return _image;
        }

        private static void txtSearchButtonCustomClick(object sender, EventArgs e)
        {
            if (sender == null) return;
            if (PropertyExists(sender, "Enabled") &&
                PropertyExists(sender, "Text"))
            {
                if (GetPropertyValue<bool>(sender, "Enabled")) SetPropertyValue(sender, "Text", "");
            }
        }

        private static void Clear(object owner)
        {
            bool _enabled = true;

            if (PropertyExists(owner, "Enabled"))
            {
                _enabled = GetPropertyValue<bool>(owner, "Enabled");
                SetPropertyValue(owner, "Enabled", false);
            }

            if (PropertyExists(owner, "Name"))
            {
                string _name = GetPropertyValue<string>(owner, "Name");
                if (_name.ToLower().Contains("txtsearch"))
                {
                    if (PropertyExists(owner, "ButtonCustom"))
                    {
                        object _buttoncustom = GetProperty(owner, "ButtonCustom", null);
                        if (_buttoncustom != null)
                        {
                            if (PropertyExists(_buttoncustom, "Visible")) SetPropertyValue(_buttoncustom, "Visible", true);
                            if (PropertyExists(_buttoncustom, "Image")) SetPropertyValue(_buttoncustom, "Image", Properties.Resources.ClearTextImage);
                            if (EventExists(owner, "ButtonCustomClick")) AttachHandler(owner, "ButtonCustomClick", new EventHandler(txtSearchButtonCustomClick));
                        }
                    }
                }
            }

            if (owner.GetType().Name.ToLower().Contains("checkbox"))
            {
                if (PropertyExists(owner, "Checked")) SetPropertyValue(owner, "Checked", false);
            }

            if (PropertyExists(owner, "MinValue") &&
                !owner.GetType().Name.ToLower().Contains("date")) SetPropertyValue(owner, "MinValue", 0);

            if (PropertyExists(owner, "DataSource"))
            {
                object _datasource = GetPropertyValue<object>(owner, "DataSource");
                if (_datasource != null)
                {
                    if (_datasource.GetType().Name == typeof(DataTable).Name)
                    {
                        DataTable _table = TryChangeType<DataTable>(_datasource);
                        if (_table != null) _table.Dispose();
                        RefreshAndManageCurrentProcess();
                    }
                    else if (_datasource.GetType().Name == typeof(DataView).Name)
                    {
                        DataView _view = TryChangeType<DataView>(_datasource);
                        if (_view != null) _view.Dispose();
                        RefreshAndManageCurrentProcess();
                    }
                }

                SetPropertyValue(owner, "DataSource", null);
            }

            if (owner.GetType().Name.ToLower().Contains("c1combo") ||
                owner.GetType().BaseType.Name.ToLower().Contains("c1combo"))
            {
                if (PropertyExists(owner, "BorderStyle")) SetPropertyValue(owner, "BorderStyle", BorderStyle.FixedSingle);
                if (PropertyExists(owner, "AutoDropDown")) SetPropertyValue(owner, "AutoDropDown", true);
                if (PropertyExists(owner, "AutoCompletion")) SetPropertyValue(owner, "AutoCompletion", true);
                if (PropertyExists(owner, "RowTracking")) SetPropertyValue(owner, "RowTracking", true);
                if (PropertyExists(owner, "ScrollTrack")) SetPropertyValue(owner, "ScrollTrack", true);
                if (PropertyExists(owner, "ScrollTips")) SetPropertyValue(owner, "ScrollTips", true);
                if (PropertyExists(owner, "HeadingStyle"))
                {
                    object _headingstyle = GetProperty(owner, "HeadingStyle", null);
                    if (_headingstyle != null)
                    {
                        if (PropertyExists(_headingstyle, "Font")) SetPropertyValue(_headingstyle, "Font", new Font("Tahoma", 8, FontStyle.Regular));
                    }
                }

                if (PropertyExists(owner, "CaptionStyle"))
                {
                    object _captionstyle = GetProperty(owner, "CaptionStyle", null);
                    if (_captionstyle != null)
                    {
                        if (PropertyExists(_captionstyle, "Font")) SetPropertyValue(_captionstyle, "Font", new Font("Tahoma", 8, FontStyle.Regular));
                    }
                }

                if (PropertyExists(owner, "Style"))
                {
                    object _style = GetProperty(owner, "Style", null);
                    if (_style != null)
                    {
                        if (PropertyExists(_style, "Font")) SetPropertyValue(_style, "Font", new Font("Tahoma", 8, FontStyle.Regular));

                        if (PropertyExists(_style, "Borders"))
                        {
                            object _styleborders = GetProperty(_style, "Borders", null);

                            if (_styleborders != null)
                            {
                                if (PropertyExists(_styleborders, "Color")) SetPropertyValue(_styleborders, "Color", Color.Black);
                            }
                        }
                    }
                }

                if (PropertyExists(owner, "MaxDropDownItems")) SetPropertyValue(owner, "MaxDropDownItems", 20);
                if (PropertyExists(owner, "CellTips")) SetPropertyValue(owner, "CellTips", 2);
                if (PropertyExists(owner, "DeadAreaBackColor")) SetPropertyValue(owner, "DeadAreaBackColor", Color.White);

                if (PropertyExists(owner, "RowDivider"))
                {
                    object _rowdivider = GetProperty(owner, "RowDivider", null);
                    if (_rowdivider != null)
                    {
                        if (PropertyExists(_rowdivider, "Color")) SetPropertyValue(_rowdivider, "Color", Color.Gainsboro);
                        if (PropertyExists(_rowdivider, "Style")) SetPropertyValue(_rowdivider, "Style", 1);
                    }
                }

                if (PropertyExists(owner, "VisualStyle")) SetPropertyValue(owner, "VisualStyle", 2);
            }


            if (owner.GetType().Name.ToLower().Contains("C1FlexGrid".ToLower()) ||
                owner.GetType().BaseType.Name.ToLower().Contains("C1FlexGrid".ToLower()))
            {
                if (MethodExists(owner, "BeginUpdate")) InvokeMethod(owner, "BeginUpdate");
                if (PropertyExists(owner, "AutoSearch")) SetPropertyValue(owner, "AutoSearch", 0);
                if (PropertyExists(owner, "SelectionMode")) SetPropertyValue(owner, "SelectionMode", 3);
                if (PropertyExists(owner, "KeyActionEnter")) SetPropertyValue(owner, "KeyActionEnter", 3);
                if (PropertyExists(owner, "KeyActionTab")) SetPropertyValue(owner, "KeyActionTab", 3);

                if (PropertyExists(owner, "Styles"))
                {
                    object _styles = GetProperty(owner, "Styles", null);
                    if (_styles != null)
                    {
                        if (PropertyExists(_styles, "EmptyArea"))
                        {
                            object _emptyarea = GetProperty(_styles, "EmptyArea", null);
                            if (_emptyarea != null)
                            {
                                if (PropertyExists(_emptyarea, "BackColor")) SetPropertyValue(_emptyarea, "BackColor", Color.White);
                                if (PropertyExists(_emptyarea, "Border"))
                                {
                                    object _emptyareaborder = GetProperty(_emptyarea, "Border", null);
                                    if (_emptyareaborder != null)
                                    {
                                        if (PropertyExists(_emptyareaborder, "Color")) SetPropertyValue(_emptyareaborder, "Color", Color.White);
                                    }
                                }
                            }
                        }

                        if (PropertyExists(_styles, "Normal"))
                        {
                            object _normal = GetProperty(_styles, "Normal", null);
                            if (_normal!=null)
                            {
                                if (PropertyExists(_normal, "Border"))
                                {
                                    object _normalborder = GetProperty(_normal, "Border", null);
                                    if (_normalborder != null)
                                    {
                                        if (PropertyExists(_normalborder, "Color")) SetPropertyValue(_normalborder, "Color", Color.Gainsboro);
                                    }
                                }
                            }
                        }
                    }

                    if (PropertyExists(_styles, "Items"))
                    {
                        object _styleitems = GetProperty(_styles, "Items", null);

                        if (_styleitems != null)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (MethodExists(_styles, "Contains"))
                                {
                                    string _stylename = "SubTotal" + i.ToString();
                                    
                                    bool _stylecontains = GetMethodValue<bool>(_styles, "Contains", new object[] { _stylename });
                                    if (!_stylecontains)
                                    {
                                        if (MethodExists(_styles, "Add"))
                                        {
                                            object _newstyle = GetMethodValue<object>(_styles, "Add", new object[] { _stylename });
                                        }
                                    }

                                    object _currentstyle = GetProperty(_styles, "Items", new object[] { _stylename });
                                    if (_currentstyle != null)
                                    {
                                        if (PropertyExists(_currentstyle, "BackColor")) SetPropertyValue(_currentstyle, "BackColor", Color.Transparent);
                                        if (PropertyExists(_currentstyle, "Font")) SetPropertyValue(_currentstyle, "Font", new Font("Tahoma", 8, FontStyle.Bold));
                                        if (PropertyExists(_currentstyle, "ForeColor")) SetPropertyValue(_currentstyle, "ForeColor", Color.Black);
                                    }
                                }
                            }
                        }   
                    }
                }

                if (PropertyExists(owner, "ClipSeparators")) SetPropertyValue(owner, "ClipSeparators", "|;");
                if (PropertyExists(owner, "TabStop")) SetPropertyValue(owner, "TabStop", true);
                if (PropertyExists(owner, "VisualStyle")) SetPropertyValue(owner, "VisualStyle", 3);

                if (PropertyExists(owner, "Tree"))
                {
                    object _tree = GetProperty(owner, "Tree", null);
                    if (_tree != null)
                    {
                        if (PropertyExists(_tree, "LineStyle")) SetPropertyValue(_tree, "LineStyle", DashStyle.Dot);
                        if (PropertyExists(_tree, "LineColor")) SetPropertyValue(_tree, "LineColor", Color.DimGray);
                    }
                }

                if (MethodExists(owner, "Clear")) InvokeMethod(owner, "Clear", new object[] { 65535 });
                
                if (PropertyExists(owner, "Rows"))
                {
                    object _rows = GetProperty(owner, "Rows", null);
                    if (_rows != null)
                    {
                        if (PropertyExists(_rows, "Count")) SetPropertyValue(_rows, "Count", 1);
                    }
                }

                if (PropertyExists(owner, "Cols"))
                {
                    object _cols = GetProperty(owner, "Cols", null);
                    if (_cols != null)
                    {
                        if (PropertyExists(_cols, "Fixed")) SetPropertyValue(_cols, "Fixed", 1);
                        if (PropertyExists(_cols, "Count")) SetPropertyValue(_cols, "Count", 1);
                    }
                }

                if (PropertyExists(owner, "AllowEditing")) SetPropertyValue(owner, "AllowEditing", false);
                if (PropertyExists(owner, "AllowDelete")) SetPropertyValue(owner, "AllowDelete", false);
                if (PropertyExists(owner, "AllowAddNew")) SetPropertyValue(owner, "AllowAddNew", false);
                if (PropertyExists(owner, "ExtendLastCol")) SetPropertyValue(owner, "ExtendLastCol", true);

                if (MethodExists(owner, "EndUpdate")) InvokeMethod(owner, "EndUpdate");
            }

            if (PropertyExists(owner, "SelectedIndex"))
            {
                try
                { SetPropertyValue(owner, "SelectedIndex", -1);  }
                catch  { }
            }

            if (PropertyExists(owner, "Items"))
            {
                if (owner.GetType().Name == typeof(TextBox).Name ||
                    owner.GetType().Name == typeof(RichTextBox).Name ||
                    owner.GetType().Name == typeof(ComboBox).Name)
                {
                    try
                    {
                        ComboBox.ObjectCollection _items = (ComboBox.ObjectCollection)GetProperty(owner, "Items", null);
                        _items.Clear();
                    }
                    catch { }
                }
                else
                {
                    object _items = GetPropertyValue(owner, "Items");
                    if (_items != null)
                    {
                        if (MethodExists(_items, "Clear")) InvokeMethod(_items, "Clear");
                    }
                }
            }

            if (PropertyExists(owner, "Value"))
            {
                if (PropertyExists(owner, "CustomFormat") &&
                    PropertyExists(owner, "Format"))
                {
                    SetPropertyValue(owner, "CustomFormat", "dd-MMM-yyyy");
                    if (PropertyExists(owner, "LockUpdateChecked")) SetPropertyValue(owner, "Format", 0);
                    else SetPropertyValue(owner, "Format", DateTimePickerFormat.Custom);
                }

                if (owner.GetType().Name == typeof(TextBox).Name ||
                    owner.GetType().Name == typeof(RichTextBox).Name ||
                    owner.GetType().Name == typeof(ComboBox).Name) SetPropertyValue(owner, "Value", "");
                else if (owner.GetType().Name == typeof(DateTimePicker).Name) SetPropertyValue(owner, "Value", DateTime.Now);
                else if (owner.GetType().Name == typeof(NumericUpDown).Name) SetPropertyValue(owner, "Value", 0);
                else
                {
                    if (owner.GetType().BaseType.Name == typeof(TextBox).Name ||
                        owner.GetType().BaseType.Name == typeof(RichTextBox).Name ||
                        owner.GetType().BaseType.Name == typeof(ComboBox).Name) SetPropertyValue(owner, "Value", "");
                    else if (owner.GetType().BaseType.Name == typeof(DateTimePicker).Name) SetPropertyValue(owner, "Value", DateTime.Now);
                    else if (owner.GetType().BaseType.Name == typeof(NumericUpDown).Name) SetPropertyValue(owner, "Value", 0);
                    else
                    {
                        if (owner.GetType().Name.ToLower().Contains("date")) SetPropertyValue(owner, "Value", DateTime.Now);
                        else SetPropertyValue(owner, "Value", 0);
                    }
                }
            }

            if (PropertyExists(owner, "Text"))
            {
                if (owner.GetType().Name == typeof(TextBox).Name ||
                    owner.GetType().Name == typeof(RichTextBox).Name ||
                    owner.GetType().Name == typeof(ComboBox).Name) SetPropertyValue(owner, "Text", "");
                else if (owner.GetType().Name == typeof(DateTimePicker).Name) SetPropertyValue(owner, "Text", DateTime.Now);
                else if (owner.GetType().Name == typeof(NumericUpDown).Name) SetPropertyValue(owner, "Text", 0);
                else
                {
                    if (owner.GetType().BaseType.Name == typeof(TextBox).Name ||
                        owner.GetType().BaseType.Name == typeof(RichTextBox).Name ||
                        owner.GetType().BaseType.Name == typeof(ComboBox).Name) SetPropertyValue(owner, "Text", "");
                    else if (owner.GetType().BaseType.Name == typeof(DateTimePicker).Name) SetPropertyValue(owner, "Text", DateTime.Now);
                    else if (owner.GetType().BaseType.Name == typeof(NumericUpDown).Name) SetPropertyValue(owner, "Text", 0);
                }
            }

            if (PropertyExists(owner, "Enabled")) SetPropertyValue(owner, "Enabled", _enabled);
        }

        /// <summary>
        /// Clears each of the controls in the specified form or control (or specified control itself) text, items, and data sources.
        /// </summary>
        /// <param name="owner">Control, form or container to be iterated</param>
        public static void ClearContents(object owner)
        {
            Control.ControlCollection _controls =  (Control.ControlCollection) GetProperty(owner, "Controls", null);
            if (_controls != null)
            {
                if (_controls.Count > 0)
                {
                    foreach (Control control in _controls) ClearContents(control);
                }
                else Clear(owner);
            }
            else Clear(owner);
        }

        private static void DisposingForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (sender != null)
            {
                GC.Collect(); GC.SuppressFinalize(sender);
                RefreshAndManageCurrentProcess();
            }
        }

        #region "EnableFields"

        private static void Enable(object owner, bool enabled)
        {
            if (PropertyExists(owner, "Enabled")) SetPropertyValue(owner, "Enabled", enabled);
        }

        /// <summary>
        /// Enables all input controls and button within the specified form, control or container.
        /// </summary>
        /// <param name="owner">Control, form or container to be iterated</param>
        public static void EnableFields(object owner)
        {
            EnableFields(owner, true);
        }

        /// <summary>
        /// Enables or disables all input controls and button within the specified form, control or container.
        /// </summary>
        /// <param name="owner">Control, form or container to be iterated</param>
        /// <param name="enabled">Determines whether the controls will be disable or not</param>
        public static void EnableFields(object owner, bool enabled)
        {
            Control.ControlCollection _controls = (Control.ControlCollection) GetProperty(owner, "Controls", null);
            if (_controls != null)
            {
                if (_controls.Count > 0)
                {
                    foreach (Control control in _controls) EnableFields(control, enabled);
                }
                else Enable(owner, enabled);
            }
            else Enable(owner, enabled);
        }

        #endregion

        /// <summary>
        /// Returns whether a certain event exists within an object's members or not.
        /// </summary>
        /// <param name="owner">Object to evaluate</param>
        /// <param name="eventname">Event name</param>
        /// <returns></returns>
        public static bool EventExists(object owner, string eventname)
        {
            EventInfo _event = null;

            try
            {
                EventInfo[] _events = owner.GetType().GetEvents();
                if (_events.Length > 0)
                {
                    foreach (EventInfo e in _events)
                    {
                        if (e.Name == eventname)
                        {
                            _event = e; break;
                        }
                    }
                }
            }
            catch { _event = null; }

            return VisualBasic.CBool(_event != null);
        }

        #region "FileObjectToByteArray"

        /// <summary>
        /// Converts the file (in the given path) to its byte array representation.
        /// </summary>
        /// <param name="filename">File's full path.</param>
        /// <returns></returns>
        public static byte[] FileObjectToByteArray(string filename)
        {
            return FileObjectToByteArray(new FileInfo(filename));
        }

        /// <summary>
        /// Converts the file to its byte array representation.
        /// </summary>
        /// <param name="file">File to convert</param>
        /// <returns></returns>
        public static byte[] FileObjectToByteArray(FileInfo file)
        {
            byte[] _bytes = null;

            if (file != null)
            {
                if (file.Exists)
                {
                    StreamReader sr = new StreamReader(file.FullName);
                    BinaryReader br = new BinaryReader(sr.BaseStream);

                    try
                    { _bytes = br.ReadBytes(VisualBasic.CInt(br.BaseStream.Length)); }
                    catch  { _bytes = null; }
                    finally
                    {
                        br.Close(); br = null;
                        sr.Close(); sr.Dispose();
                        RefreshAndManageCurrentProcess();
                    }
                }
            }

            return _bytes;
        }

        #endregion

        #region "FileObjectToHexaDecimalString"

        /// <summary>
        /// Converts the file (in the given path) to its hexadecimal string representation.
        /// </summary>
        /// <param name="filename">File's full path.</param>
        /// <returns></returns>
        public static string FileObjectToHexaDecimalString(string filename)
        {
            return FileObjectToHexaDecimalString(new FileInfo(filename));
        }

        /// <summary>
        /// Converts the file to its hexadecimal string representation.
        /// </summary>
        /// <param name="file">File to convert</param>
        /// <returns></returns>
        public static string FileObjectToHexaDecimalString(FileInfo file)
        {
            StringBuilder _hex = new StringBuilder();

            if (file != null)
            {
                if (file.Exists)
                {
                    StreamReader sr = new StreamReader(file.FullName);
                    BinaryReader br = new BinaryReader(sr.BaseStream);

                    try
                    {
                        _hex.Append(BitConverter.ToString(br.ReadBytes(VisualBasic.CInt(br.BaseStream.Length))).Replace("-", ""));
                    }
                    catch  { }
                    finally
                    {
                        br.Close(); br = null;
                        sr.Close(); sr.Dispose();
                        RefreshAndManageCurrentProcess();
                    }
                }
            }

            return _hex.ToString();
        }

        #endregion

        #region "Filter"

        /// <summary>
        /// Filters the DataTable object's view finding relevant records matching the supplied searched value to all of the DataTable's columns except for the ones specified as excluded. 
        /// </summary>
        /// <param name="table">DataTable object to filter</param>
        /// <param name="value">Value to be searched</param>
        public static void Filter(this DataTable table, object value)
        { table.Filter(value, new string[] { "" }); }

        /// <summary>
        /// Filters the DataTable object's view finding relevant records matching the supplied searched value to all of the DataTable's columns except for the ones specified as excluded. 
        /// </summary>
        /// <param name="table">DataTable object to filter</param>
        /// <param name="value">Value to be searched</param>
        /// <param name="excludedfields">Excluded fields to apply to search into</param>
        public static void Filter(this DataTable table, object value, string[] excludedfields)
        {
            if (table != null)
            {
                string _filter = ""; string _value = "";
                if (!IsNullOrNothing(value)) _value = value.ToString();

                foreach (DataColumn _column in table.Columns)
                {
                    if (!excludedfields.Contains(_column.ColumnName) &&
                        !_column.DataType.Name.ToLower().Contains("byte[]") &&
                        !_column.DataType.Name.ToLower().Contains("byte()") &&
                        !_column.DataType.Name.ToLower().Contains("bytes[]") &&
                        !_column.DataType.Name.ToLower().Contains("bytes()")) _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " OR " : "") + "(ISNULL(CONVERT([" + _column.ColumnName + "], System.String), '') LIKE '%" + _value.ToSqlValidString(true).Replace(" ", "%') AND (ISNULL(CONVERT([" + _column.ColumnName + "], System.String), '') LIKE '%") + "%')";
                }

                try { table.DefaultView.RowFilter = _filter; }
                catch { }
            }
        }

        #endregion

        /// <summary>
        /// Returns the current workstation's IP address.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentIPAddress()
        {
            string _ipaddress = "";
            string _host = Dns.GetHostName();
            IPAddress[] _ips = Dns.GetHostEntry(_host).AddressList;

            if (_ips.Length > 0)
            {
                foreach (IPAddress ip in _ips)
                {
                    if (ip.ToString().IsIPAddress())
                    {
                        _ipaddress = ip.ToString(); break;
                    }
                }
            }

            return _ipaddress;
        }

        #region "GetDefaultValueByType"

        /// <summary>
        /// Gets a default value based on the specified data type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDefaultValueByType<T>()
        {
            Type _type = typeof(T);
            return (T) GetDefaultValueByType(_type);
        }

        /// <summary>
        /// Gets a default value based on the specified data type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValueByType(Type type)
        {
            object _value = null;

            if (type != null)
            {
                if (type.Name == typeof(string).Name ||
                    type.Name == typeof(String).Name) _value = "";
                else if (type.Name == typeof(int).Name ||
                         type.Name == typeof(Int16).Name ||
                         type.Name == typeof(Int32).Name ||
                         type.Name == typeof(Int64).Name ||
                         type.Name == typeof(byte).Name ||
                         type.Name == typeof(Byte).Name ||
                         type.Name == typeof(float).Name ||
                         type.Name == typeof(decimal).Name ||
                         type.Name == typeof(Decimal).Name ||
                         type.Name == typeof(double).Name ||
                         type.Name == typeof(Double).Name ||
                         type.Name == typeof(long).Name ||
                         type.Name == typeof(short).Name ||
                         type.Name == typeof(Single).Name) _value = 0;
                else if (type.Name == typeof(bool).Name ||
                         type.Name == typeof(Boolean).Name) _value = false;
                else if (type.Name == typeof(DateTime).Name) _value = VisualBasic.CDate("01/01/1900");
                else _value = null;
            }

            return _value;
        }

        #endregion

        /// <summary>
        /// Gets the event with the specified name from the specified owning object.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="eventname"></param>
        /// <returns></returns>
        public static EventInfo GetEvent(object owner, string eventname)
        {
            EventInfo _event = null;

            if (owner != null) _event = owner.GetType().GetEvent(eventname, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return _event;
        }

        #region "GetMethodValue"

        /// <summary>
        ///  Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <returns></returns>
        public static object GetMethodValue(object owner, string methodname)
        { return GetMethodValue<object>(owner, methodname); }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <param name="parameter">Method / function parameters</param>
        /// <returns></returns>
        public static object GetMethodValue(object owner, string methodname, object parameter)
        { return GetMethodValue<object>(owner, methodname, parameter); }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <param name="parameters">Method / function parameters</param>
        /// <returns></returns>
        public static object GetMethodValue(object owner, string methodname, object[] parameters)
        { return GetMethodValue<object>(owner, methodname, parameters); }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <returns></returns>
        public static T GetMethodValue<T>(object owner, string methodname)
        {
            return GetMethodValue<T>(owner, methodname, null);
        }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <param name="parameter">Method / function parameters</param>
        /// <returns></returns>
        public static T GetMethodValue<T>(object owner, string methodname, object parameter)
        {
            return GetMethodValue<T>(owner, methodname, new object[] { parameter });
        }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <param name="parameters">Method / function parameters</param>
        /// <returns></returns>
        public static T GetMethodValue<T>(object owner, string methodname, object[] parameters)
        {
            T _defaultvalue = GetDefaultValueByType<T>();
            return GetMethodValue<T>(owner, methodname, parameters, _defaultvalue);
        }

        /// <summary>
        /// Returns the returning value of the specified object's function / method based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method / function name</param>
        /// <param name="parameters">Method / function parameters</param>
        /// <param name="defaultvalue">Default value in case it returns a DBNull or Nothing.</param>
        /// <returns></returns>
        public static T GetMethodValue<T>(object owner, string methodname, object[] parameters, T defaultvalue)
        {
            T _value = defaultvalue;

            if (owner != null)
            {
                bool _allarenull = true;

                if (parameters != null)
                {
                    foreach (object parameter in parameters)
                    {
                        _allarenull = _allarenull && VisualBasic.CBool(parameter == null);
                        if (!_allarenull) break;
                    }
                }

                MethodInfo _method = null;

                if (_allarenull) _method = owner.GetType().GetMethod(methodname);
                else
                {
                    Type[] _types = new Type[parameters.Length];
                    for (int i = 0; i <= parameters.Length - 1; i++)
                    {
                        object p = parameters[i];
                        if (p == null) _types[i] = typeof(object);
                        else _types[i] = p.GetType();
                    }
                    _method = owner.GetType().GetMethod(methodname, BindingFlags.Public & BindingFlags.Instance, null, _types, null);
                }

                if (_method != null)
                {
                    if (_allarenull) _value = (T) _method.Invoke(owner, null);
                    else _value = (T) _method.Invoke(owner, parameters);

                    if (IsNullOrNothing(_value)) _value = defaultvalue;
                }
            }

            return _value;
        }

        #endregion

        #region "GetPropertyValue"

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <returns></returns>
        public static object GetPropertyValue(object owner, string propertyname)
        { return GetPropertyValue<object>(owner, propertyname); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameter">Property accessibility parameter</param>
        /// <returns></returns>
        public static object GetPropertyValue(object owner, string propertyname, object parameter)
        { return GetPropertyValue<object>(owner, propertyname, new object[] { parameter }, null); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameters">Property accessibility parameter</param>
        /// <returns></returns>
        public static object GetPropertyValue(object owner, string propertyname, object[] parameters)
        { return GetPropertyValue<object>(owner, propertyname, parameters); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string propertyname)
        {  return GetPropertyValue<T>(owner, propertyname, null); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="defaultvalue">Default value to return when function evaluates no value</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string propertyname, T defaultvalue)
        { return GetPropertyValue<T>(owner, propertyname, null, defaultvalue); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameters">Property accessibility parameters</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string propertyname, object[] parameters)
        { return GetPropertyValue<T>(owner, propertyname, parameters, GetDefaultValueByType<T>()); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameters">Property accessibility parameters</param>
        /// <param name="defaultvalue">Default value to return when function evaluates no value</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string propertyname, object[] parameters, T defaultvalue)
        {
            T _value = defaultvalue;

            if (owner != null)
            {
                string[] _properties = propertyname.Split(new char[] { '.' });
                if (_properties.Length >= 1) _value = GetPropertyValue<T>(owner, _properties, parameters, defaultvalue);
                else _value = (T)GetProperty(owner, propertyname, parameters);
            }

            return _value;
        }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertynames">Property names</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string[] propertynames)
        { return GetPropertyValue<T>(owner, propertynames, null); }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertynames">Property names</param>
        /// <param name="parameters">Property accessibility parameters</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string[] propertynames, object[] parameters)
        {
            T _defaultvalue = GetDefaultValueByType<T>();
            return GetPropertyValue<T>(owner, propertynames, parameters, _defaultvalue);
        }

        /// <summary>
        /// Returns the value of a certain object's property based on the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">Property owner</param>
        /// <param name="propertynames">Property names</param>
        /// <param name="parameters">Property accessibility parameters</param>
        /// <param name="defaultvalue">Default value to return when function evaluates no value</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object owner, string[] propertynames, object[] parameters, T defaultvalue)
        {
            T _value = defaultvalue;

            if (owner != null)
            {
                object _currentowner = owner;

                for (int i = 0; i <= (propertynames.Length - 1); i++)
                {
                    if (i == (propertynames.Length - 1)) _value = (T) GetProperty(_currentowner, propertynames[i], parameters);
                    else
                    {
                        _currentowner = GetProperty(_currentowner, propertynames[i], null);
                        if (_currentowner == null) break;
                    }
                }
            }

            return _value;
        }

        private static object GetProperty(object owner, string propertyname, object[] parameters)
        {
            if (owner != null)
            {
                Type[] _types = null;

                bool _allisnull = true;

                if (parameters != null)
                {
                    foreach (object parameter in parameters)
                    {
                        _allisnull = _allisnull && VisualBasic.CBool(parameter == null);
                        if (!_allisnull) break;
                    }

                    if (!_allisnull)
                    {
                        _types = new Type[parameters.Length];
                        for (int i = 0; i <= (parameters.Length - 1); i++) _types[i] = parameters[i].GetType();
                    }
                }

                PropertyInfo _property = null;

                if (_types == null) _property = owner.GetType().GetProperty(propertyname);
                else _property = owner.GetType().GetProperty(propertyname, _types);

                if (_property != null)
                {
                    if (_property.CanRead)
                    {
                        if (parameters == null) return _property.GetValue(owner, null);
                        else
                        {
                            if (_allisnull) return _property.GetValue(owner, null);
                            else return _property.GetValue(owner, parameters);
                        }
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        #endregion

        /// <summary>
        /// Converts the specified hexadecimal string into its byte array representation.
        /// </summary>
        /// <param name="hex">Hexadecimal string to be interpreted</param>
        /// <returns></returns>
        public static byte[] HexadecimalStringToByteArray(string hex)
        {
            int _len = hex.Length;
            int _upper = _len.WholePartDivision(2);

            if (_len % 2 == 0) _upper = -1;
            byte[] _bytes = null;

            if (_upper >= 0)
            {
                _bytes = new byte[_upper];
                for (int i = 0; i <= _upper; i++)
                {
                    _bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
            }

            return _bytes;
        }

        /// <summary>
        /// Converts the specified image to its bytes array representation.
        /// </summary>
        /// <param name="image">Image object to be interpreted</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image image)
        {
            byte[] _bytes = null;
            MemoryStream _stream = new MemoryStream();

            try
            {
                Image _cloned =  (Image) image.Clone();
                _cloned.Save(_stream, ImageFormat.Png);
                _cloned.Dispose(); RefreshAndManageCurrentProcess();
                _bytes = _stream.ToArray();
            }
            catch  { _bytes = null; }
            finally
            {
                _stream.Close(); _stream.Dispose();
                RefreshAndManageCurrentProcess();
            }

            return _bytes;
        }

        /// <summary>
        /// Converts the given image to its hexadecimal string representation
        /// </summary>
        /// <param name="image">Image object to be interpreted</param>
        /// <returns></returns>
        public static string ImageToHexaDecimalString(Image image)
        {
            StringBuilder _hex = new StringBuilder();
            MemoryStream _stream = new MemoryStream();

            try
            {
                Image _cloned = (Image) image.Clone();
                _cloned.Save(_stream, ImageFormat.Png);
                _cloned.Dispose(); RefreshAndManageCurrentProcess();

                _hex.Append(BitConverter.ToString(_stream.ToArray()).Replace("-", ""));
            }
            catch  { }
            finally
            {
                _stream.Close(); _stream.Dispose();
                RefreshAndManageCurrentProcess();
            }

            return _hex.ToString();
        }

        #region "InvokeMethod"

        /// <summary>
        /// Invokes the specified method of the object thru its method name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method name</param>
        public static void InvokeMethod(object owner, string methodname)
        { InvokeMethod(owner, methodname, null); }

        /// <summary>
        /// Invokes the specified method of the object thru its method name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method name</param>
        /// <param name="parameter">Method accessing parameter</param>
        public static void InvokeMethod(object owner, string methodname, object parameter)
        { InvokeMethod(owner, methodname, Tuple<object>(parameter)); }

        /// <summary>
        /// Invokes the specified method of the object thru its method name.
        /// </summary>
        /// <param name="owner">Method owner</param>
        /// <param name="methodname">Method name</param>
        /// <param name="parameters">Method accessing parameters</param>
        public static void InvokeMethod(object owner, string methodname, object[] parameters)
        {
            if (owner != null)
            {
                MethodInfo _method = null;

                bool _allarenull = true;

                if (parameters != null)
                {
                    foreach (object parameter in parameters)
                    {
                        _allarenull = _allarenull && VisualBasic.CBool(parameter == null);
                        if (!_allarenull) break;
                    }
                }

                if (_allarenull) _method = owner.GetType().GetMethod(methodname);
                else
                {
                    Type[] _types = new Type[parameters.Length];
                    for (int i = 0; i <= (parameters.Length - 1); i++) _types[i] = parameters[i].GetType();
                    _method = owner.GetType().GetMethod(methodname, _types);
                }
                
                if (_method != null)
                {
                    if (_allarenull)
                    {
                        try
                        {
                            _method.Invoke(owner, null);
                        }
                        catch  { }
                    }
                    else
                    {
                        try
                        {
                            _method.Invoke(owner, parameters);
                        }
                        catch  { }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns whether the specified value is equivalent to NULL or DBNull.Value.
        /// </summary>
        /// <param name="value">Value to evaluate</param>
        /// <returns></returns>
        public static bool IsNullOrNothing(object value)
        { return (value == null) || (value == DBNull.Value); }

        #region "LogError"

        /// <summary>
        /// Log an error message into an error log file ([Current Directory]\error.log).
        /// </summary>
        /// <param name="error">Error encountered</param>
        public static void LogError(Exception error)
        {
            LogError(error, "");
        }

        /// <summary>
        /// Log an error message into an error log file ([Current Directory]\error.log).
        /// </summary>
        /// <param name="error">Error encountered</param>
        /// <param name="remarks">Remarks</param>
        public static void LogError(Exception error, string remarks)
        {
            LogError(error.Message, remarks);
        }

        /// <summary>
        /// Log an error message into an error log file ([Current Directory]\error.log).
        /// </summary>
        /// <param name="error">Error message</param>
        public static void LogError(string error)
        {
            LogError(error, "");
        }

        /// <summary>
        /// Log an error message into an error log file ([Current Directory]\error.log).
        /// </summary>
        /// <param name="error">Error message</param>
        /// <param name="remarks">Remarks</param>
        public static void LogError(string error, string remarks)
        {
            string _filename = Environment.CurrentDirectory + "\\errorlogs.txt";
            StreamWriter _writer = new StreamWriter(_filename, true);

            try
            {
               _writer.WriteLine(VisualBasic.Format(DateTime.Now,"dd-MMM-yyyy hh:mm:ss tt") + (String.IsNullOrEmpty(remarks.RLTrim())? "" : "\t") + remarks + "\t" + error);
            }
            catch  { }
            finally
            {
                _writer.Close(); _writer.Dispose();
                RefreshAndManageCurrentProcess();
            }
        }

        #endregion

        /// <summary>
        /// Returns whether a certain method exists within an object's members or not.
        /// </summary>
        /// <param name="owner">Object to evaluate</param>
        /// <param name="methodname">Method name to find</param>
        /// <returns></returns>
        public static bool MethodExists(object owner, string methodname)
        {
            MethodInfo _method = null;

            try
            {
                MemberInfo[] _methods = owner.GetType().GetMethods();
                if (_methods.Length > 0)
                {
                    foreach (MethodInfo m in _methods)
                    {
                        if (m.Name == methodname)
                        {
                            _method = m; break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("ambigous")) Debug.Print("Ambigous method : " + owner.GetType().Name + " : " + methodname);
                _method = null; 
            }

            return  VisualBasic.CBool(_method != null);
        }

        /// <summary>
        /// Returns whether a certain property exists within an object's members or not.
        /// </summary>
        /// <param name="owner">Object to evaluate</param>
        /// <param name="propertyname">Property name to find.</param>
        /// <returns></returns>
        public static bool PropertyExists(object owner, string propertyname)
        {
            PropertyInfo _property = null;

            try
            {
                PropertyInfo[] _properties = owner.GetType().GetProperties();
                if (_properties.Length > 0)
                {
                    foreach (PropertyInfo p in _properties)
                    {
                        if (p.Name == propertyname)
                        {
                            _property = p; break;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                if (ex.Message.ToLower().Contains("ambigous")) Debug.Print("Ambigous property : " + owner.GetType().Name + " : " + propertyname);
                _property = null; 
            }

            return  VisualBasic.CBool(_property != null);
        }

        #region "ReadFile"

        /// <summary>
        /// Reads file contents from the specified file.
        /// </summary>
        /// <param name="filename">Path of the file to read the contents from</param>
        /// <returns></returns>
        public static string ReadFile(string filename)
        {
            return ReadFile(new FileInfo(filename));
        }

        /// <summary>
        /// Reads file contents from the specified file.
        /// </summary>
        /// <param name="file">File to read the contents from</param>
        /// <returns></returns>
        public static string ReadFile(FileInfo file)
        {
            StringBuilder _contents = new StringBuilder();

            if (file != null)
            {
                StreamReader _reader = new StreamReader(file.FullName);
                try
                {
                    _contents.Append(_reader.ReadToEnd());
                }
                catch  { }
                finally
                {
                    _reader.Close(); _reader.Dispose();
                    RefreshAndManageCurrentProcess();
                }
            }

            return _contents.ToString();
        }

        #endregion

        #region "Redraw"

        /// <summary>
        /// Repaints; basically invokes the EndUpdate method of the specified object.
        /// </summary>
        /// <param name="control"></param>
        public static void Redraw(Control control)
        { Redraw(control, true); }

        /// <summary>
        /// Repaints or unpaints an object; basically invokes the BeginUpdate or EndUpdate method of the specified object.
        /// </summary>
        /// <param name="control">Control to repaint or unpaint</param>
        /// <param name="repaint">Determines whether to repaint or not</param>
        public static void Redraw(Control control, bool repaint)
        {
            if (control != null)
            {
                if (MethodExists(control, "BeginUpdate") &&
                    MethodExists(control, "EndUpdate"))
                {
                    for (int i = 1; i <= 3; i++) InvokeMethod(control, "EndUpdate");
                    if (!repaint) InvokeMethod(control, "BeginUpdate");
                }
            }
        }

        #endregion

        /// <summary>
        /// Refreshes and force the release of the entire unmanaged resources of the current application's process.
        /// </summary>
        public static void RefreshAndManageCurrentProcess()
        {
            GC.Collect();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Process _process = Process.GetCurrentProcess();
                SetProcessWorkingSetSize(_process.Handle, -1, -1);
                _process.Close(); _process.Refresh(); _process.Dispose();
            }
        }

        #region "SetPropertyValue"

        /// <summary>
        /// Sets the property value of an specified object by specifying the property name
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        public static void SetPropertyValue(object owner, string propertyname)
        {
            if (owner != null)
            {
                PropertyInfo _property = null;

                foreach (PropertyInfo property in owner.GetType().GetProperties())
                {
                    if (property.Name == propertyname)
                    {
                        _property = property; break;
                    }
                }

                if (_property != null)
                {
                    object _value = GetDefaultValueByType(_property.PropertyType);
                    SetPropertyValue(owner, propertyname, _value);
                }
            }
        }

        /// <summary>
        /// Sets the property value of an specified object by specifying the property name
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="value">Value to assign</param>
        public static void SetPropertyValue(object owner, string propertyname, object value)
        { SetPropertyValue(owner, propertyname, null, value); }

        /// <summary>
        /// Sets the property value of an specified object by specifying the property name
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameter">Property accessing parameters</param>
        /// <param name="value">Value to assign</param>
        public static void SetPropertyValue(object owner, string propertyname, object parameter, object value)
        {  SetPropertyValue(owner, propertyname, Tuple<object>(parameter), value);  }

        /// <summary>
        /// Sets the property value of an specified object by specifying the property name
        /// </summary>
        /// <param name="owner">Property owner</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="parameters">Property accessing parameters</param>
        /// <param name="value">Value to assign</param>
        public static void SetPropertyValue(object owner, string propertyname, object[] parameters, object value)
        {
            if (owner != null)
            {
                bool _allarenull = true;

                if (parameters != null)
                {
                    foreach (object parameter in parameters)
                    {
                        _allarenull = _allarenull && (parameters == null);
                        if (!_allarenull) break;
                    }
                }

                PropertyInfo _property = null;

                if (_allarenull) _property =  owner.GetType().GetProperty(propertyname);
                else
                {
                    Type[] _types = new Type[parameters.Length];
                    for (int i = 0; i <= (parameters.Length - 1); i++) _types[i] = parameters[i].GetType();
                    _property = owner.GetType().GetProperty(propertyname, _types);
                }

                if (_property != null)
                {
                    if (_property.CanWrite)
                    {
                        object _curvalue = value;

                        if (_property.PropertyType != null && !IsNullOrNothing(_curvalue))
                        {
                            if (_property.PropertyType.Name == typeof(string).Name ||
                                _property.PropertyType.Name == typeof(String).Name) _curvalue = value.ToString();
                            else if (_property.PropertyType.Name == typeof(byte).Name ||
                                     _property.PropertyType.Name == typeof(Byte).Name ||
                                     _property.PropertyType.Name == typeof(Int16).Name)
                            {
                                try
                                { _curvalue = (byte)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(bool).Name ||
                                     _property.PropertyType.Name == typeof(Boolean).Name)
                            {
                                try
                                { _curvalue = (bool)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(DateTime).Name)
                            {
                                try
                                { _curvalue = (DateTime)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(decimal).Name ||
                                     _property.PropertyType.Name == typeof(Decimal).Name)
                            {
                                try
                                { _curvalue = (decimal)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(double).Name ||
                                     _property.PropertyType.Name == typeof(Double).Name)
                            {
                                try
                                { _curvalue = (double)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(float).Name ||
                                     _property.PropertyType.Name == typeof(Single).Name)
                            {
                                try
                                { _curvalue = (float)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(int).Name ||
                                     _property.PropertyType.Name == typeof(Int32).Name)
                            {
                                try
                                { _curvalue = (int)value; }
                                catch  { }
                            }
                            else if (_property.PropertyType.Name == typeof(long).Name ||
                                     _property.PropertyType.Name == typeof(Int64).Name)
                            {
                                try
                                { _curvalue = (long)value; }
                                catch { }
                            }
                            else if (_property.PropertyType.Name == typeof(short).Name)
                            {
                                try
                                { _curvalue = (short)value; }
                                catch  { }
                            }
                        }

                        if (_allarenull)
                        {
                            try
                            { _property.SetValue(owner, _curvalue, null); }
                            catch  { }
                        }
                        else
                        {
                            try
                            { _property.SetValue(owner, _curvalue, parameters); }
                            catch  { }
                        }
                    }
                }
            }
        }

        #endregion

        #region "ToSafeValue"

        /// <summary>
        /// Parses the specified value to return its corresponding type-safe representation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to evaluate</param>
        /// <returns></returns>
        public static T ToSafeValue<T>(T value)
        { return ToSafeValue<T>(value, GetDefaultValueByType<T>()); }

        /// <summary>
        /// Parses the specified value to return its corresponding type-safe representation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to evaluate</param>
        /// <param name="defaultvalue">Default value to return just in case it is unsafe</param>
        /// <returns></returns>
        public static T ToSafeValue<T>(T value, T defaultvalue)
        {
            T _value = defaultvalue;
            if (!IsNullOrNothing(value)) _value = value;
            return _value;
        }

        #endregion

        /// <summary>
        /// Works like TryCast function but this time supports assigned value types (ea. Integer, Decimal, Date and etc.).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">Expression to convert</param>
        /// <returns></returns>
        public static T TryChangeType<T>(object expression)
        {
            T _value = default(T);

            try
            { _value = (T) expression; }
            catch { _value = default(T); }

            return _value;
        }

        #region "Tuple"

        /// <summary>
        /// Encapsulates an array of object with the data type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T[] Tuple<T>(T arg)
        { return new T[] { arg }; }

        /// <summary>
        /// Encapsulates an array of object with two data types.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static object[] Tuple<T1, T2>(T1 arg1, T2 arg2)
        { return new object[] { arg1, arg2 }; }

        /// <summary>
        /// Encapsulates an array of object with three data types.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static object[] Tuple<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        { return new object[] { arg1, arg2, arg3 }; }

        /// <summary>
        /// Encapsulates an array of object with three data types.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static object[] Tuple<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        { return new object[] { arg1, arg2, arg3, arg4 }; }

        /// <summary>
        /// Encapsulates an array of object with three data types.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static object[] Tuple<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        { return new object[] { arg1, arg2, arg3, arg4, arg5 }; }

        /// <summary>
        /// Encapsulates an array of object with three data types.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static object[] Tuple<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        { return new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }; }

        #endregion

        /// <summary>
        /// Validates whether condition was satisfied otherwise control specified will be highligthed (validator should be a DevComponents.DotNetBar.Validator.SuperValidator).
        /// </summary>
        /// <param name="validator">DevComponents.DotNetBar.Validator.SuperValidator to use as the control notifying object</param>
        /// <param name="control">Control to place a notifier with</param>
        /// <param name="condition">The true part of the satisfying condition to evaluate</param>
        /// <param name="notification">Notification message to be shown</param>
        /// <returns></returns>
        public static bool Valid(object validator, Control control, bool condition, string notification)
        {
           if (validator != null &&
                control != null)
            {
                if (validator.GetType().Name.ToLower().Contains("SuperValidator".ToLower()) ||
                    validator.GetType().BaseType.Name.ToLower().Contains("SuperValidator".ToLower()))
                {
                    if (PropertyExists(validator, "ErrorProvider") &&
                        PropertyExists(validator, "Highlighter"))
                    {
                        object _errorprovider = GetProperty(validator, "ErrorProvider", null);
                        object _highlighter = GetProperty(validator, "Highlighter", null);

                        if (_errorprovider != null &&
                            _highlighter != null)
                        {
                            if (MethodExists(_errorprovider, "Clear")) InvokeMethod(_errorprovider, "Clear");
                            if (MethodExists(_highlighter, "SetHighlightColor")) InvokeMethod(_highlighter, "SetHighlightColor", new object[] { control, 0 });

                            if (!condition)
                            {
                                if (MethodExists(_highlighter, "SetHighlightColor")) InvokeMethod(_highlighter, "SetHighlightColor", new object[] { control, 1 });
                                if (MethodExists(_errorprovider, "SetError")) InvokeMethod(_errorprovider, "SetError", new object[] { control, notification });
                                if (MethodExists(control, "Focus")) InvokeMethod(control, "Focus");
                            }
                        }
                    }
                }
            }

           return condition;
        }

        #region "WriteToFile"

        /// <summary>
        /// Writes the specified value into an specific file.
        /// </summary>
        /// <param name="filename">File path</param>
        /// <param name="contents">Value to be written</param>
        /// <returns></returns>
        public static FileInfo WriteToFile(string filename, string contents)
        { return WriteToFile(filename, contents, false);  }

        /// <summary>
        /// Writes the specified value into an specific file.
        /// </summary>
        /// <param name="filename">File path</param>
        /// <param name="contents">Value to be written</param>
        /// <param name="append">Determines whether to overwrite the existing file's contents of not</param>
        /// <returns></returns>
        public static FileInfo WriteToFile(string filename, string contents, bool append)
        {
            FileInfo _file = null;
            StreamWriter _writer = new StreamWriter(filename, append);
            bool _written = false;

            try
            {
                _writer.Write(contents); _written = true;
            }
            catch { _file = null; _written = false; }
            finally
            {
                _writer.Close(); _writer.Dispose();
                RefreshAndManageCurrentProcess();
            }

            if (_written)
            {
                if (File.Exists(filename)) _file = new FileInfo(filename);
            }

            return _file;
        }

        #endregion

    }
}
