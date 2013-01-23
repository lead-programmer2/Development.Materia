#region "imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Development.Materia.Controls
{
    /// <summary>
    /// Control extender for marking input controls with a tiny indicator.
    /// </summary>
    [ProvideProperty("Required", typeof(Control)), 
     ProvideProperty("RequiredIndicatorColor", typeof(Control)),
     ProvideProperty("RequiredIndicatorToolTip", typeof(Control)),
     Description("Control extender for attaching input controls with a required field indicator."),
     ToolboxBitmap(typeof(RequiredFieldMarker), "RequiredFieldMarker.bmp") ]
    public class RequiredFieldMarker :Component, IExtenderProvider
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of RequiredFieldMarker.
        /// </summary>
        public RequiredFieldMarker()
        { }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static List<string> _additionalsupportedcontrols = new List<string>();

        /// <summary>
        /// Gets the global lists of additional supported control names for the RequiredFieldMarker.
        /// </summary>
        public static List<string> AdditionalSupportedControls
        {
            get { return _additionalsupportedcontrols; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Hashtable _contoltable = new Hashtable();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IndicatorPositionEnum _indicatorposition = IndicatorPositionEnum.LeftTop;

        /// <summary>
        /// Gets or sets required field indicator's position within the control's bounds.
        /// </summary>
        [DefaultValue(typeof(IndicatorPositionEnum), "LeftTop"),
         Description("Gets or sets required field indicator's position within the control's bounds.")]
        public IndicatorPositionEnum IndicatorPosition
        {
            get { return _indicatorposition; }
            set
            {
                _indicatorposition = value; UpdateIndicators();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static RequiredFieldMarker _globalmarker = new RequiredFieldMarker();

        #endregion

        #region "methods"

        /// <summary>
        /// Validates if evaluated control is supported by RequiredFieldMarker.
        /// </summary>
        /// <param name="extendee">Object to evaluate.</param>
        /// <returns>True if the specified object is supported otherwise false.</returns>
        public bool CanExtend(object extendee) 
        {
            bool _canextend = false;

            if (extendee != null)
            {
                string[] _supported = new string[] { "TextBoxX", "ComboBoxEx", "DateTimeInput",
                                                     "DoubleInput", "IntegerInput", "ComboTree",
                                                     "TextBox", "ComboBox", "DateTimePicker",
                                                     "C1Combo", "NumericUpDown", "RichTextBox",
                                                     "MaskedTextBox", "C1DropDownControl", 
                                                     "FilterComboControl", "SearchCtrl" };

                foreach (string _support in _supported)
                {
                    if (extendee.GetType().Name.ToLower().Contains(_support.ToLower()) ||
                        extendee.GetType().Name.ToLower().Contains(_support.ToLower()))
                    {
                        _canextend = true; break;
                    }
                    else
                    {
                        if (_additionalsupportedcontrols.Contains(extendee.GetType().FullName))
                        {
                            _canextend = true; break;
                        }
                    }
                }
            }

            return _canextend;
        }

        /// <summary>
        /// Returns whether the specified control is marked as required using the RequiredFieldMarker.
        /// </summary>
        /// <param name="control">Control to evaluate.</param>
        /// <returns>True if the control is already marked as required field otherwise false.</returns>
        public static bool ControlIsRequired(Control control)
        { return _globalmarker.IsRequired(control); }

        /// <summary>
        /// Gets required field indicator's presence for this control.
        /// </summary>
        /// <param name="control">Control to evaluate.</param>
        /// <returns>True if the specified control is already marked as a required field otherwise false.</returns>
        [Description("Indicates required field indicator's presence for this control.")]
        public bool GetRequired(Control control)
        {
            if (control != null)
            {
                if (CanExtend(control)) return _contoltable.Contains(control);
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Gets required field indicator's fill color.
        /// </summary>
        /// <param name="control">Control to evaluate.</param>
        /// <returns>Control's attached required field indicator's color.</returns>
        [Description("Indicates required field indicator's fill color."),
         DefaultValue(typeof(Color), "OrangeRed")]
        public Color GetRequiredIndicatorColor(Control control)
        {
            if (control != null)
            {
                if (CanExtend(control))
                {
                    if (_contoltable.Contains(control)) return (Color)_contoltable[control];
                    else return Color.Empty;
                }
                else return Color.Empty;
            }
            else return Color.Empty;
        }

        private void Indicator_Pin(object sender, EventArgs e)
        {
            if (sender != null)
            {
                if (CanExtend(sender))
                {
                    if (IsRequired((Control)sender)) PinIndicator((Control)sender);
                    else UnpinIndicator((Control)sender);
                }
            }
        }

        /// <summary>
        /// Gets whether control was marked as required or not.
        /// </summary>
        /// <param name="control">Control to evalulate.</param>
        /// <returns>True if control is already marked as a required field otherwise false.</returns>
        public bool IsRequired(Control control)
        {
            bool _isrequired = false;

            if (control != null)
            {
                _isrequired = _contoltable.Contains(control);

                if (!_isrequired)
                {
                    if (control.Controls.Count > 0)
                    {
                        foreach (Control _control in control.Controls)
                        {
                            if (_control.GetType() == typeof(Label))
                            {
                                _isrequired = (_control.Name == "lbl_" + control.Name);
                                if (_isrequired) break;
                            }
                        }
                    }
                }
            }

            return _isrequired;
        }

        private void ParentForm_Load(object sender, EventArgs e)
        { UpdateIndicators(); }

        private void PinIndicator(Control control)
        {
            if (control != null)
            {
                UnpinIndicator(control);
                if (control.Visible)
                {
                    Color _color = Color.OrangeRed;

                    if (_contoltable.Contains(control))
                    {
                        if (_contoltable[control] != null) _color = (Color)_contoltable[control];
                    }

                    Label _label = new Label(); string _indicatorname = "lbl_" + control.Name;
                    _label.Name = _indicatorname;
                    _label.Size = new Size(5, 5); _label.BackColor = _color;

                    try
                    {
                        Form _form = control.FindForm();
                        if (_form != null)
                        {
                            RemoveControlFrom(_form.Controls, _indicatorname);
                            foreach (Control _control in _form.Controls)
                            {
                                if (_control.Controls.Count > 0) RemoveControlFrom(_control.Controls, _indicatorname);
                            }
                        }

                        if (control.Parent != null)
                        {
                            RemoveControlFrom(control.Parent.Controls, _indicatorname);
                            control.Parent.Controls.Add(_label);
                        }
                        else
                        {
                            if (_form != null) _form.Controls.Add(_label);
                            else
                            {
                                if (control.Controls.ContainsKey(_indicatorname)) control.Controls.RemoveByKey(_indicatorname);
                            }
                        }

                        switch (_indicatorposition)
                        {
                            case IndicatorPositionEnum.RigthTop:
                                int _x = control.Location.X + control.Size.Width;

                                if (control.GetType().FullName == typeof(TextBox).FullName ||
                                    control.GetType().Name .ToLower().Contains("TextBoxX") ||
                                    control.GetType().BaseType.FullName == typeof(TextBox).FullName ||
                                    control.GetType().BaseType.Name.ToLower().Contains("TextBoxX"))
                                {
                                    Point _location = new Point(_x - 5, control.Top);

                                    if (Materia.PropertyExists(control, "ButtonCustom") &&
                                        Materia.PropertyExists(control, "ButtonCustom2"))
                                    {
                                        object _buttoncustom1 = Materia.GetPropertyValue<object>(control, "ButtonCustom");
                                        object _buttoncustom2 = Materia.GetPropertyValue<object>(control, "ButtonCustom2");

                                        if (_buttoncustom1 != null &&
                                            _buttoncustom2 != null)
                                        {
                                            bool _btn1visible = false; bool _btn2visible = false;

                                            if (Materia.PropertyExists(_buttoncustom1, "Visible")) _btn1visible =  Materia.GetPropertyValue<bool>(_buttoncustom1, "Visible");
                                            if (Materia.PropertyExists(_buttoncustom2, "Visible")) _btn2visible =  Materia.GetPropertyValue<bool>(_buttoncustom2, "Visible");

                                            if (_btn1visible || _btn2visible)
                                            {
                                                if (_btn1visible && _btn2visible) _location = new Point(_x - 38, control.Top + 2);
                                                else _location = new Point(_x - 21, control.Top);
                                            }
                                            else
                                            {
                                                bool _multiline = false;
                                                ScrollBars _scrollbars = ScrollBars.None;

                                                if (Materia.PropertyExists(control, "Multiline")) _multiline = Materia.GetPropertyValue<bool>(control, "Multiline");
                                                if (Materia.PropertyExists(control, "ScrollBars")) _scrollbars = Materia.GetPropertyValue<ScrollBars>(control, "ScrollBars", null, ScrollBars.None);

                                                if (_multiline &&
                                                    (_scrollbars == ScrollBars.Vertical ||
                                                     _scrollbars == ScrollBars.Both)) _location = new Point(_x - 21, control.Top);
                                                else _location = new Point(_x - 5, control.Top);
                                            }
                                        }
                                    }

                                    if (_location == null) _location = new Point(_x - 5, control.Top);
                                    _label.Location = _location;
                                }
                                else
                                {
                                    if (control.GetType().Name.ToLower().Contains("DateTimeInput".ToLower()) ||
                                        control.GetType().Name.ToLower().Contains("DateTimePicker".ToLower()) ||
                                        control.GetType().Name.ToLower().Contains("DateTime".ToLower()) ||
                                        control.GetType().BaseType.Name.ToLower().Contains("DateTimeInput".ToLower()) ||
                                        control.GetType().BaseType.Name.ToLower().Contains("DateTimePicker".ToLower()) ||
                                        control.GetType().BaseType.Name.ToLower().Contains("DateTime".ToLower()))
                                    {
                                        Point _location = new Point(_x - 5, control.Top);

                                        if (Materia.PropertyExists(control, "ButtonDropDown"))
                                        {
                                            object _buttondropdown = Materia.GetPropertyValue<object>(control, "ButtonDropDown");
                                            if (_buttondropdown != null)
                                            {
                                                bool _bdvisible = false;
                                                if (Materia.PropertyExists(_buttondropdown, "Visible")) _bdvisible = Materia.GetPropertyValue<bool>(_buttondropdown, "Visible");
                                                if (_bdvisible) _location = new Point(_x - 21, control.Top + 2);
                                                else _location = new Point(_x - 5, control.Top);
                                            }
                                        }

                                        if (_location == null) _location = new Point(_x - 5, control.Top);
                                        _label.Location = _location;
                                    }
                                    else
                                    {
                                        if (control.GetType().Name.ToLower().Contains("DoubleInput".ToLower()) ||
                                            control.GetType().Name.ToLower().Contains("IntegerInput".ToLower()) ||
                                            control.GetType().Name.ToLower().Contains("NumericUpDown".ToLower()) ||
                                            control.GetType().BaseType.Name.ToLower().Contains("DoubleInput".ToLower()) ||
                                            control.GetType().BaseType.Name.ToLower().Contains("IntegerInput".ToLower()) ||
                                            control.GetType().BaseType.Name.ToLower().Contains("NumericUpDown".ToLower()))
                                        {
                                            bool _showupdown = false; Point _location = new Point(_x - 5, control.Top);
                                            if (Materia.PropertyExists(control, "ShowUpDown")) _showupdown = Materia.GetPropertyValue<bool>(control, "ShowUpDown");

                                            if (_showupdown) _location = new Point(_x - 21, control.Top + 2);
                                            else _location = new Point(_x - 5, control.Top);

                                            _label.Location = _location;
                                        }
                                        else
                                        {
                                            if (control.GetType().Name.ToLower().Contains("ComboBox".ToLower()) ||
                                                control.GetType().Name.ToLower().Contains("ComboBoxEx".ToLower()) ||
                                                control.GetType().Name.ToLower().Contains("ComboTree".ToLower()) ||
                                                control.GetType().BaseType.Name.ToLower().Contains("ComboBox".ToLower()) ||
                                                control.GetType().BaseType.Name.ToLower().Contains("ComboBoxEx".ToLower()) ||
                                                control.GetType().BaseType.Name.ToLower().Contains("ComboTree".ToLower())) _label.Location = new Point(_x - 21, control.Top + 2);
                                            else
                                            {
                                                if (control.GetType().Name.ToLower().Contains("C1Combo".ToLower())) _label.Location = new Point(_x - 24, control.Top);
                                                else _label.Location = new Point(_x - 5, control.Top);
                                            }
                                        }
                                    }
                                }
                                break;
                            case IndicatorPositionEnum.LeftTop:
                                _label.Location = control.Location; break;
                            default: break;
                        }
                    }
                    catch { }

                    _label.Visible = true; _label.Show(); _label.BringToFront();
                }
            }
        }

        private void PinIndicator(Control.ControlCollection controls)
        {
            if (controls != null)
            {
                foreach (Control control in controls)
                {
                    if (CanExtend(control))
                    {
                        if (IsRequired(control)) PinIndicator(control);
                        else UnpinIndicator(control);
                    }
                    else UnpinIndicator(control);

                    if (control.Controls.Count > 0) PinIndicator(control.Controls);
                }
            }
        }

        private void RemoveControlFrom(Control.ControlCollection controls, string controlname)
        {
            foreach (Control control in controls)
            {
                if (control.Name.ToLower() == controlname.ToLower())
                {
                    controls.Remove(control); controls.Owner.Update();
                }
            }
        }

        #region "SetAsRequired"

        /// <summary>
        /// Sets aor unsets each of the specified controls with a rquired field indicator.
        /// </summary>
        /// <param name="controls">List of controls to set  / unset.</param>
        public static void SetAsRequired(List<Control> controls)
        { SetAsRequired(controls, true); }

        /// <summary>
        /// Sets aor unsets each of the specified controls with a rquired field indicator.
        /// </summary>
        /// <param name="controls">List of controls to set  / unset.</param>
        /// <param name="required">Determines whether to place a mark or not</param>
        public static void SetAsRequired(List<Control> controls, bool required)
        {
            if (controls != null)
            {
                foreach (Control control in controls) SetAsRequired(control, required);
            }
        }

        /// <summary>
        /// Sets and attaches each of the specified controls with a rquired field indicator.
        /// </summary>
        /// <param name="controls">Controls to mark as required fields.</param>
        public static void SetAsRequired(params Control[] controls)
        {
            if (controls != null)
            {
                foreach (Control control in controls) SetAsRequired(control);
            }
        }

        /// <summary>
        /// Sets a control with a required field indicator.
        /// </summary>
        /// <param name="control">Control to be mark with</param>
        public static void SetAsRequired(Control control)
        { SetAsRequired(control, true); }

        /// <summary>
        ///  Sets or unsets a control with a required field indicator.
        /// </summary>
        /// <param name="control">Control to be mark / unmark with</param>
        /// <param name="required">Determines whether to place a mark or not</param>
        public static void SetAsRequired(Control control, bool required)
        { 
           if (control!=null) _globalmarker.SetRequired(control, required); 
        }

        /// <summary>
        /// Sets a control with a required field indicator in the specified position within the control's bounds.
        /// </summary>
        /// <param name="control">Control to be mark with</param>
        /// <param name="color">Required field indicator's fill color</param>
        public static void SetAsRequired(Control control, Color color)
        {
            if (control != null)
            {
                _globalmarker.SetRequiredIndicatorColor(control, color);
                _globalmarker.SetRequired(control, true);
            }
        }

        #endregion

        /// <summary>
        /// Sets required field indicator's presence for this control.
        /// </summary>
        /// <param name="control">Control to set or unset as a required field.</param>
        /// <param name="required">A value that determines whether the control is a required field or not.</param>
        [Description("Indicates required field indicator's presence for this control."),
         DefaultValue(typeof(bool), "False")]
        public void SetRequired(Control control, bool required)
        {
            if (control != null)
            {
                if (CanExtend(control))
                {
                    if (required)
                    {
                        if (!_contoltable.Contains(control)) _contoltable.Add(control, Color.OrangeRed);
                        else _contoltable[control] = Color.OrangeRed;

                        UnpinIndicator(control);
                        control.Resize += new EventHandler(Indicator_Pin);
                        control.LocationChanged += new EventHandler(Indicator_Pin);
                        control.ParentChanged += new EventHandler(Indicator_Pin);
                        control.VisibleChanged += new EventHandler(Indicator_Pin);
                        PinIndicator(control);
                    }
                    else
                    {
                        if (_contoltable.Contains(control)) _contoltable.Remove(control);
                        control.Resize -= new EventHandler(Indicator_Pin);
                        control.LocationChanged -= new EventHandler(Indicator_Pin);
                        control.ParentChanged -= new EventHandler(Indicator_Pin);
                        control.VisibleChanged -= new EventHandler(Indicator_Pin);
                        UnpinIndicator(control);
                    }
                }
            }
        }

        /// <summary>
        /// Sets required field indicator's fill color.
        /// </summary>
        /// <param name="control">Control to evaluate.</param>
        /// <param name="color">Required field indicator marker's color.</param>
        [Description("Indicates required field indicator's fill color."),
         DefaultValue(typeof(Color), "OrangeRed")]
        public void SetRequiredIndicatorColor(Control control, Color color)
        {
            if (control != null)
            {
                if (CanExtend(control))
                {
                    if (IsRequired(control))
                    {
                        if (!_contoltable.Contains(control)) _contoltable.Add(control, color);
                        else _contoltable[control] = color;

                        UnpinIndicator(control);
                        control.Resize += new EventHandler(Indicator_Pin);
                        control.LocationChanged += new EventHandler(Indicator_Pin);
                        control.ParentChanged += new EventHandler(Indicator_Pin);
                        control.VisibleChanged += new EventHandler(Indicator_Pin);
                        PinIndicator(control);
                    }
                    else
                    {
                        if (_contoltable.Contains(control)) _contoltable.Remove(control);
                        control.Resize -= new EventHandler(Indicator_Pin);
                        control.LocationChanged -= new EventHandler(Indicator_Pin);
                        control.ParentChanged -= new EventHandler(Indicator_Pin);
                        control.VisibleChanged -= new EventHandler(Indicator_Pin);
                        UnpinIndicator(control);
                    }
                }
            }
        }

        private void UnpinIndicator(Control control)
        {
            if (control != null)
            {
                string _indicatorname = "lbl_" + control.Name;

                try
                {
                    Form _form = control.FindForm();
                    if (_form != null)
                    {
                        RemoveControlFrom(_form.Controls, _indicatorname);
                        foreach (Control _control in _form.Controls)
                        {
                            if (_control.Controls.Count > 0) RemoveControlFrom(_control.Controls, _indicatorname);
                        }
                    }

                    if (control.Parent != null) RemoveControlFrom(control.Parent.Controls, _indicatorname);
                }
                catch { }
                finally { Materia.RefreshAndManageCurrentProcess(); }
            }
        }

        #region "UpdateIndicators"

        /// <summary>
        /// Updates 'required' marked control just in case there is a repainting of the marked control and
        /// the indicator attached to it needs to be repositioned and redrawn also.
        /// </summary>
        /// <param name="control">Control to be updated and redrawn.</param>
        public void UpdateIndicators(Control control)
        {
            if (control != null)
            {
                if (CanExtend(control))
                {
                    if (IsRequired(control)) PinIndicator(control);
                }
            }
        }

        /// <summary>
        /// Updates 'required' marked controls just in case there is a repainting of each marked control(s) and
        /// the indicator(s) attached to it needs to be repositioned and redrawn also.
        /// </summary>
        public void UpdateIndicators()
        {
            foreach (Control control in _contoltable)
            {
                if (CanExtend(control)) PinIndicator(control);
            }
        }

        #endregion

        #endregion

    }
}
