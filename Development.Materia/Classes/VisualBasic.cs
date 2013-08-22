#region "imports"

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion

namespace Development.Materia
{

    #region "enumerations"

    /// <summary>
    /// Date and time interval enumerations.
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// Day of week.
        /// </summary>
        Day=0,
        /// <summary>
        /// Day of year.
        /// </summary>
        DayOfYear=1,
        /// <summary>
        /// Hours
        /// </summary>
        Hour = 2,
        /// <summary>
        /// Minutes
        /// </summary>
        Minute = 3,
        /// <summary>
        /// Months
        /// </summary>
        Month = 4,
        /// <summary>
        /// Quarter of year.
        /// </summary>
        Quarter = 5,
        /// <summary>
        /// Seconds
        /// </summary>
        Second = 6,
        /// <summary>
        /// Weeks
        /// </summary>
        Week = 7,
        /// <summary>
        /// Week of year.
        /// </summary>
        WeekOfYear = 8,
        /// <summary>
        /// Years
        /// </summary>
        Year
    }

    /// <summary>
    /// First day of week enumerations.
    /// </summary>
    public enum FirstDayOfWeek
    {
        /// <summary>
        /// Sunday
        /// </summary>
        Sunday=0,
        /// <summary>
        /// Monday
        /// </summary>
        Monday =1,
        /// <summary>
        /// Tuesday
        /// </summary>
        Tuesday=2,
        /// <summary>
        /// Wednesday
        /// </summary>
        Wednesday=3,
        /// <summary>
        /// Thursday
        /// </summary>
        Thursday=4,
        /// <summary>
        /// Friday
        /// </summary>
        Friday =5,
        /// <summary>
        /// Saturday
        /// </summary>
        Saturday=6
    }

    #endregion

    /// <summary>
    /// Visual Basic mimics (for C# convenience).
    /// </summary>
    public static class VisualBasic
    {

        #region "Asc"

        /// <summary>
        /// Works like Visual Basic Asc method.
        /// </summary>
        /// <param name="character">Character to interpret</param>
        /// <returns>ASCII representation of the specified character.</returns>
        public static int Asc(char character)
        { return Asc(character.ToString()); }

        /// <summary>
        /// Works like Visual Basic Asc method.
        /// </summary>
        /// <param name="value">String to interpret</param>
        /// <returns>ASCII representation of the specified string.</returns>
        public static int Asc(string value)
        {
            int _asc = 0;
            
            if (!String.IsNullOrEmpty(value.Trim()))
            {
                char[] _chars = value.Trim().ToCharArray();
                if (_chars.Length > 0)
                {
                    char _chr = _chars[0];
                    _asc = (int)_chr;
                }
            }

            return _asc;
        }

        #endregion

        /// <summary>
        /// Works like Visual Basic CBool method.
        /// </summary>
        /// <param name="value">Expression to convert.</param>
        /// <returns>System.Boolean representation of the specified expression.</returns>
        public static bool CBool(object value)
        {
            if (IsNumeric(value))
            {
                if (CLng(value) == 0) return false;
                else return true;
            }
            else
            {
                bool _result = false;

                try { _result = (bool)value; }
                catch { _result = bool.Parse(value.ToString()); }

                return _result;
            }
        }

        /// <summary>
        /// Works like Visual Basic CByte method
        /// </summary>
        /// <param name="value">Expression to convert</param>
        /// <returns>System.Byte representation of the specified expression.</returns>
        public static byte CByte(object value)
        {
            byte _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (byte)value; }
                catch { _result = byte.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToByte(value); }
                catch { _result = byte.Parse(value.ToString()); }
            }
           
            return _result;
        }

        /// <summary>
        /// Works like Visual Basic CDate method.
        /// </summary>
        /// <param name="value">Expression to convert.</param>
        /// <returns>System.Date representation of the specified expression.</returns>
        public static DateTime CDate(object value)
        {
            DateTime _result = DateTime.Parse("1/1/1900");

            try 
            { _result = Convert.ToDateTime(value); }
            catch { _result = DateTime.Parse(value.ToString()); }

            return _result; 
        }

        /// <summary>
        /// Works like Visual Basic CDec method.
        /// </summary>
        /// <param name="value">Expression to convert.</param>
        /// <returns>System.Decimal representation of the specified expression.</returns>
        public static decimal CDec(object value)
        {
            decimal _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (decimal)value; }
                catch { _result = decimal.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToDecimal(value); }
                catch { _result = decimal.Parse(value.ToString()); }
            }
           
            return _result; 
        }

        /// <summary>
        /// Works like Visual Basic CDbl method.
        /// </summary>
        /// <param name="value">Expression to convert.</param>
        /// <returns>System.Double representation of the specified expression.</returns>
        public static double CDbl(object value)
        {
            double _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (double)value; }
                catch { _result = double.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToDouble(value); }
                catch { _result = double.Parse(value.ToString()); }
            }
           
            return _result; 
        }

        /// <summary>
        /// Works like Visual Basic CInt method.
        /// </summary>
        /// <param name="value">Expression to convert</param>
        /// <returns>System.Integer representation of the specified expression.</returns>
        public static int CInt(object value)
        {
            int _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (int)value; }
                catch { _result = int.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToInt32(value); }
                catch { _result = int.Parse(value.ToString()); }
            }
           
            return _result; 
        }

        /// <summary>
        /// Works like Visual Basic CSByte method.
        /// </summary>
        /// <param name="value">Expression to convert</param>
        /// <returns>System.SByte repsentation of the specified expression.</returns>
        public static sbyte CSByte(object value)
        {
            sbyte _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (sbyte)value; }
                catch { _result = sbyte.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToSByte(value); }
                catch { _result = sbyte.Parse(value.ToString()); }
            }
           
            return _result;
        }

        /// <summary>
        /// Works like Visual basic CShort method.
        /// </summary>
        /// <param name="value">Expression to convert</param>
        /// <returns>System.Short representation of the specified expression.</returns>
        public static short CShort(object value)
        {
            short _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (short)value; }
                catch { _result = short.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToInt16(value); }
                catch { _result = short.Parse(value.ToString()); }
            }
           
            return _result;
        }

        /// <summary>
        /// Works like Visual Basic CSng method.
        /// </summary>
        /// <param name="value">Expression to convert</param>
        /// <returns>System.Single representation of the specified expression.</returns>
        public static float CSng(object value)
        {
            float _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (float)value; }
                catch { _result = float.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToSingle(value); }
                catch { _result = float.Parse(value.ToString()); }
            }
            
            return _result;
        }

        /// <summary>
        /// Works like Visual Basic Chr method.
        /// </summary>
        /// <param name="ascii">ASCII code to convert</param>
        /// <returns>System.Char representation of the specified ASCII code.</returns>
        public static char Chr(int ascii)
        { return Convert.ToChar(ascii); }

        /// <summary>
        /// Works like Visual Basic CLng method.
        /// </summary>
        /// <param name="value">Expression to convert.</param>
        /// <returns>System.Long representation of the specified expression.</returns>
        public static long CLng(object value)
        {
            long _result = 0;

            if (value.GetType().IsEnum)
            {
                try { _result = (long)value; }
                catch { _result = long.Parse(value.ToString()); }
            }
            else
            {
                try { _result = Convert.ToInt64(value); }
                catch { _result = long.Parse(value.ToString()); }
            }
           
            return _result; 
        }

        /// <summary>
        /// Works like Visual Basic DateDiff method.
        /// </summary>
        /// <param name="interval">Date interval</param>
        /// <param name="date1">1st Date value</param>
        /// <param name="date2">2nd Date value</param>
        /// <returns>Duration of the two dates from each other in the specified date interval.</returns>
        public static double DateDiff(DateInterval interval, DateTime date1, DateTime date2)
        {
            double _value = 0;
            TimeSpan _span = date2 - date1;

            switch (interval)
            {
                case DateInterval.Day:
                case DateInterval.DayOfYear:
                    _value = _span.TotalDays; break;
                case DateInterval.Hour:
                    _value = _span.TotalHours; break;
                case DateInterval.Minute:
                     _value = _span.TotalMinutes; break;
                case DateInterval.Month:
                    _value = _span.TotalDays / 30; break;
                case DateInterval.Quarter:
                    _value = ((_span.TotalDays / 30) / 4); break;
                case DateInterval.Second:
                    _value = _span.TotalSeconds;  break;
                case DateInterval.Week:
                case DateInterval.WeekOfYear:
                    _value = _span.TotalDays / 7; break;
                case DateInterval.Year:
                    _value = _span.TotalDays / 365; break;
                default: break;
            }

            return _value;
        }

        /// <summary>
        /// Works like built-in Visual Basic DeleteSetting method.
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="section">Registry section.</param>
        /// <param name="key">Registry key</param>
        public static void DeleteSetting(string application, string section, string key)
        {
            RegistryKey _machinekey = Registry.LocalMachine;
            string _subkey = "SOFTWARE\\" + application + "\\" + section;

            try
            {
                RegistryKey _registrysubkey = _machinekey.CreateSubKey(_subkey);
                if (_subkey != null) _registrysubkey.DeleteValue(key);
            }
            catch { }
        }

        /// <summary>
        /// Works like built-in Visual Basic Format function.
        /// </summary>
        /// <param name="value">Value to be formatted.</param>
        /// <param name="format">Format pattern.</param>
        /// <returns>String formatted based on the specified value and pattern.</returns>
        public static string Format(object value, string format)
        {
            string _formatted = "";

            if (value != null &&
                value != DBNull.Value)
            {
                if (value.GetType().Name == typeof(DateTime).Name)
                {
                    List<string> _patterns = new List<string>();
                    _patterns.Clear();
                    _patterns.Add("^(S|s)(H|h)(O|o)(R|r)(T|t)");
                    _patterns.Add("^(S|s)(H|h)(O|o)(R|r)(T|t)(D|d)(A|a)(T|t)(E|e)");
                    _patterns.Add("^(S|s)(H|h)(O|o)(R|r)(T|t)[\n\r\t ]+(D|d)(A|a)(T|t)(E|e)");
                    _patterns.Add("^(L|l)(O|o)(N|n)(G|g)");
                    _patterns.Add("^(L|l)(O|o)(N|n)(G|g)(D|d)(A|a)(T|t)(E|e)");
                    _patterns.Add("^(L|l)(O|o)(N|n)(G|g)[\n\r\t ]+(D|d)(A|a)(T|t)(E|e)");

                    string _currentpattern = "";

                    foreach (string _pattern in _patterns)
                    {
                        if (Regex.IsMatch(format, _pattern))
                        {
                            _currentpattern = _pattern; break;
                        }
                    }

                    if (!String.IsNullOrEmpty(_currentpattern.Trim()))
                    {
                        string _shortpattern = "^(S|s)(H|h)(O|o)(R|r)(T|t)";

                        if (Regex.IsMatch(format, _shortpattern)) _formatted = String.Format("{0:MM/dd/yyyy}", value);
                        else _formatted = String.Format("{0:MMMM dd, yyyy}", value);
                    }
                    else _formatted = String.Format("{0:" + format + "}", value);

                    _patterns.Clear(); _patterns = null; GC.Collect();
                }
                else if (value.GetType().Name == typeof(byte).Name ||
                         value.GetType().Name == typeof(Byte).Name ||
                         value.GetType().Name == typeof(decimal).Name ||
                         value.GetType().Name == typeof(Decimal).Name ||
                         value.GetType().Name == typeof(double).Name ||
                         value.GetType().Name == typeof(Double).Name ||
                         value.GetType().Name == typeof(float).Name ||
                         value.GetType().Name == typeof(int).Name ||
                         value.GetType().Name == typeof(Int16).Name ||
                         value.GetType().Name == typeof(Int32).Name ||
                         value.GetType().Name == typeof(Int64).Name ||
                         value.GetType().Name == typeof(long).Name ||
                         value.GetType().Name == typeof(short).Name ||
                         value.GetType().Name == typeof(Single).Name)
                {
                    List<string> _patterns = new List<string>();
                    _patterns.Clear();
                    _patterns.Add("^(N|n)[0-9]+");
                    _patterns.Add("^(F|f)[0-9]+");

                    string _currentpattern = "";

                    foreach (string _pattern in _patterns)
                    {
                        if (Regex.IsMatch(format, _pattern))
                        {
                            _currentpattern = _pattern; break;
                        }
                    }

                    if (!String.IsNullOrEmpty(_currentpattern.Trim()))
                    {
                        string _decimalpatterns = "[0-9]+";
                        int _decimals = int.Parse(Regex.Match(format, _decimalpatterns).Value);
                        string _decimalplaces = "";

                        for (int i = 0; i < (_decimals - 1); i++) _decimalplaces += "#";

                        if (Regex.IsMatch(format, "^(N|n)")) _formatted = String.Format("{0:###,###,###,##0." + _decimalplaces + "0}", value);
                        else _formatted = String.Format("{0:###########0." + _decimalplaces + "0}", value);
                    }
                    else _formatted = String.Format("{0:" + format + "}", value);
                }
            }

            return _formatted;
        }

        #region "GetSetting"

        /// <summary>
        /// Works like Visual Basic GetSetting method.
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="section">Registry section.</param>
        /// <param name="key">Registry key.</param>
        /// <returns>Value of the specified registry key.</returns>
        public static object GetSetting(string application, string section, string key)
        { return GetSetting(application, section, key, null); }

        /// <summary>
        /// Works like Visual Basic GetSetting method.
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="section">Registry section.</param>
        /// <param name="key">Registry key.</param>
        /// <param name="defaultvalue">Default value to return if registry key is not existing.</param>
        /// <returns>Value of the specified registry key.</returns>
        public static object GetSetting(string application, string section, string key, object defaultvalue)
        {
            object _value = defaultvalue;

            RegistryKey _machinekey = Registry.LocalMachine;
            string _subkey = "SOFTWARE\\" + application + "\\" + section;

            try
            {
                RegistryKey _registrysubkey = _machinekey.CreateSubKey(_subkey);
                if (_registrysubkey != null) _value = _registrysubkey.GetValue(key);
            }
            catch { }

            if (Materia.IsNullOrNothing(_value)) _value = defaultvalue;

            return _value;
        }

        #endregion

        #region "InputBox"

        /// <summary>
        /// Mimics Visual Basic InputBox method.
        /// </summary>
        /// <param name="prompt">Dialog notification message.</param>
        /// <returns>User-input value.</returns>
        public static string InputBox(string prompt)
        { return InputBox(prompt, Application.ProductName); }

        /// <summary>
        /// Mimics Visual Basic InputBox method.
        /// </summary>
        /// <param name="prompt">Dialog notification message.</param>
        /// <param name="title">Dialog title.</param>
        /// <returns>User-input value.</returns>
        public static string InputBox(string prompt, string title)
        { return InputBox(prompt, title, ""); }

        /// <summary>
        /// Mimics Visual Basic InputBox method.
        /// </summary>
        /// <param name="prompt">Dialog notification message.</param>
        /// <param name="title">Dialog title.</param>
        /// <param name="defaultresponse">Dialog's default return value if user does not enter any.</param>
        /// <returns>User-input value.</returns>
        public static string InputBox(string prompt, string title, string defaultresponse)
        { return InputBox(prompt, title, defaultresponse, 0); }

        /// <summary>
        /// Mimics Visual Basic InputBox method.
        /// </summary>
        /// <param name="prompt">Dialog notification message.</param>
        /// <param name="title">Dialog title.</param>
        /// <param name="defaultresponse">Dialog's default return value if user does not enter any.</param>
        /// <param name="x">Position of the dialog horizontally.</param>
        /// <returns>User-input value.</returns>
        public static string InputBox(string prompt, string title, string defaultresponse, int x)
        { return InputBox(prompt, title, defaultresponse, x, 0); }

        /// <summary>
        /// Mimics Visual Basic InputBox method.
        /// </summary>
        /// <param name="prompt">Dialog notification message.</param>
        /// <param name="title">Dialog title.</param>
        /// <param name="defaultresponse">Dialog's default return value if user does not enter any.</param>
        /// <param name="x">Position of the dialog horizontally.</param>
        /// <param name="y">Position of the dialog vertically.</param>
        /// <returns>User-input value.</returns>
        public static string InputBox(string prompt, string title, string defaultresponse, int x, int y)
        {
            string _value = "";

            Form _form = new Form();
            Label _label = new Label();
            TextBox _textBox = new TextBox();
            Button _buttonOk = new Button();
            Button _buttonCancel = new Button();

            _form.Text = title;
            _label.Text = prompt;
            _textBox.Text = defaultresponse;

            _buttonOk.Text = "&OK";
            _buttonCancel.Text = "&Cancel";
            _buttonOk.DialogResult = DialogResult.OK;
            _buttonCancel.DialogResult = DialogResult.Cancel;

            _label.SetBounds(9, 20, 372, 13);
            _textBox.SetBounds(12, 36, 372, 20);
            _buttonOk.SetBounds(228, 72, 75, 23);
            _buttonCancel.SetBounds(309, 72, 75, 23);

            _label.AutoSize = true;
            _textBox.Anchor = _textBox.Anchor | AnchorStyles.Right;
            _buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            _form.ClientSize = new Size(396, 107);
            _form.Controls.AddRange(new Control[] { _label, _textBox, _buttonOk, _buttonCancel });
            _form.ClientSize = new Size(Math.Max(300, _label.Right + 10), _form.ClientSize.Height);
            _form.FormBorderStyle = FormBorderStyle.FixedDialog;

            if (x > 0 || y > 0)
            {
                _form.StartPosition = FormStartPosition.Manual;
                _form.Location = new Point(x, y);
            }
            else _form.StartPosition = FormStartPosition.CenterScreen;
            
            _form.MinimizeBox = false;
            _form.MaximizeBox = false;
            _form.AcceptButton = _buttonOk;
            _form.CancelButton = _buttonCancel;

            if (_form.ShowDialog() == DialogResult.OK) _value = _textBox.Text;
            else _value = "";

            _form.Dispose(); Materia.RefreshAndManageCurrentProcess();

            return _value;
        }

        #endregion

        /// <summary>
        /// Works like Visual Basic IsDate method.
        /// </summary>
        /// <param name="value">Value to evaluate.</param>
        /// <returns>True if value is a valid date otherwise false.</returns>
        public static bool IsDate(object value)
        {
            bool _isdate = false;

            if (!Materia.IsNullOrNothing(value))
            {
                try
                {
                    DateTime _date = DateTime.Now;
                    _isdate = DateTime.TryParse(value.ToString(), out _date);
                }
                catch { _isdate = false; }
            }
           
            return _isdate;
        }

        /// <summary>
        /// Works like Visual Basic IsArray method.
        /// </summary>
        /// <param name="value">Value to evaluate.</param>
        /// <returns>True if value is an array otherwise false.</returns>
        public static bool IsArray(object value)
        {
            bool _isarray = false;

            if (!Materia.IsNullOrNothing(value))
            {
                try
                {  _isarray = value.GetType().IsArray; }
                catch { _isarray = false; }
            }
            
            return _isarray;
        }

        /// <summary>
        /// Works like Visual Basic IsDBNull method.
        /// </summary>
        /// <param name="value">Value to evaluate.</param>
        /// <returns>True if value is equivalent to DBNull.Value otherwise false.</returns>
        public static bool IsDBNull(object value)
        {
            if (value != null) return VisualBasic.CBool(value == DBNull.Value);
            else return false;
        }

        /// <summary>
        /// Works like Visual Basic IsNumeric method.
        /// </summary>
        /// <param name="value">Value to evaluate.</param>
        /// <returns>True if value is a number otherwise false.</returns>
        public static bool IsNumeric(object value)
        {
            bool _isnumeric = false;

            if (!Materia.IsNullOrNothing(value))
            {
                string[] _numerictypes = new string[] { typeof(byte).Name, typeof(Byte).Name,
                                                        typeof(int).Name, typeof(long).Name,
                                                        typeof(Int16).Name, typeof(Int32).Name, typeof(Int64).Name,
                                                        typeof(sbyte).Name, typeof(SByte).Name,
                                                        typeof(decimal).Name, typeof(Decimal).Name,
                                                        typeof(double).Name, typeof(Double).Name,
                                                        typeof(float).Name, typeof(Single).Name };
                
                if (_numerictypes.Contains(value.GetType().Name)) _isnumeric = true;
                else
                {
                    if (value.GetType() == typeof(string) ||
                        value.GetType() == typeof(String))
                    {
                        String _pattern = "^[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?$";
                        Match _match = Regex.Match(value.ToString(), _pattern, RegexOptions.IgnoreCase);
                        if (_match != null)
                        {
                            if (_match.Value.Trim() == value.ToString().Trim()) _isnumeric = true;
                            else
                            {
                                try
                                {
                                    double _number = 0;
                                    _isnumeric = double.TryParse(value.ToString(), out _number);
                                }
                                catch { _isnumeric = false; }
                            }
                        }
                    }
                }
            }
            
            return _isnumeric;
        }

        #region "MonthName"

        /// <summary>
        /// Works like Visual Basic MonthName method.
        /// </summary>
        /// <param name="month">Month number in the calendar</param>
        /// <returns>Month name</returns>
        public static string MonthName(int month)
        { return MonthName(month, false); }

        /// <summary>
        /// Works like Visual Basic MonthName method.
        /// </summary>
        /// <param name="month">Month number in the calendar</param>
        /// <param name="abbreviate">A value that determines whether to abbreviate the output or not.</param>
        /// <returns>Month name</returns>
        public static string MonthName(int month, bool abbreviate)
        {
            string[] _months = { "January", "February", "March", "April", "May",
                                 "June", "July", "August", "September", "October",
                                 "November", "December"};

            if (month <= _months.Length &&
                month > 0)
            {
                string _month = _months[month - 1];
                if (abbreviate) return _month.Substring(0, 3);
                else return _month;
            }
            else throw new ArgumentException("Month should be between 1 to 12 only.");
        }

        #endregion

        /// <summary>
        /// Works like Visual Basic SaveSetting method.
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="section">Registry section.</param>
        /// <param name="key">Registry key.</param>
        /// <param name="value">Value to store in the registry.</param>
        public static void SaveSetting(string application, string section, string key, object value)
        {
            RegistryKey _machinekey = Registry.LocalMachine;
            string _subkey = "SOFTWARE\\" + application + "\\" + section;

            try
            {
                RegistryKey _registrysubkey = _machinekey.CreateSubKey(_subkey);
                if (Materia.IsNullOrNothing(value)) _registrysubkey.SetValue(key, "");
                else _registrysubkey.SetValue(key, value);
            }
            catch { }
        }

        #region "WeekDayName"

        /// <summary>
        ///  Works like Visual Basic WeekDayname method.
        /// </summary>
        /// <param name="day">Day of the week.</param>
        /// <returns>Week day name</returns>
        public static string WeekDayName(int day)
        { return WeekDayName(day, false); }

        /// <summary>
        /// Works like Visual Basic WeekDayname method.
        /// </summary>
        /// <param name="day">Day of the week.</param>
        /// <param name="abbreviate">A value that determines whether to abbreviate the output or not.</param>
        /// <returns>Week day name</returns>
        public static string WeekDayName(int day, bool abbreviate)
        { return WeekDayName(day, abbreviate, FirstDayOfWeek.Sunday); }

        /// <summary>
        /// Works like Visual Basic WeekDayname method.
        /// </summary>
        /// <param name="day">Day of the week.</param>
        /// <param name="abbreviate">A value that determines whether to abbreviate the output or not.</param>
        /// <param name="firstdayofweek">A value to determines to what day the week should start.</param>
        /// <returns>Week day name</returns>
        public static string WeekDayName(int day, bool abbreviate, FirstDayOfWeek firstdayofweek)
        {
            string[] _days = { "Sunday", "Monday", "Tuesday", "Wednesday",
                               "Thursday", "Friday", "Saturday"};

            if (day <= _days.Length &&
                day > 0)
            {
                int _selectedday = day + (int)firstdayofweek;
                if (_selectedday >= (_days.Length))
                {
                    int _over = _selectedday - _days.Length;
                    _selectedday = _over;
                }
                string _day = _days[_selectedday];

                if (abbreviate) return _day.Substring(0, 3);
                else return _day;
            }
            else throw new ArgumentException("Day should be between 1 to 7 only.");
        }

        #endregion

    }
}
