#region "imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Development.Materia.Database
{
    
    /// <summary>
    /// Data-control binding component.
    /// </summary>
    [ProvideProperty("FieldName", typeof(Control)),
     ToolboxBitmap(typeof(DataBinder), "DataBinder.bmp"),
     DefaultProperty("AfterSave"),
     Description("Data-control binding component.")]
    public class DataBinder : Component, IExtenderProvider
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBinder.
        /// </summary>
        public DataBinder()
        {
            _bindedcontrols = new BindedControlCollection(this);
            _databinding = new DataBinding(this);
        }

        #endregion

        #region "custom event handlers"

        /// <summary>
        /// Handler for DataBinder data-loading events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataBinderLoadingEventHandler(object sender, DataBinderLoadingEventArgs e);

        /// <summary>
        /// Handler for DataBinder data-saving events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataBinderSavingEventHandler(object sender, DataBinderSavingEventArgs e);

        /// <summary>
        /// Handler for DataBinder data-validation events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataBinderValidationEventHandler(object sender, DataBinderValidationEventArgs e);

        #endregion

        #region "events"

        /// <summary>
        /// Occurs when data loading routines has been performed in a certain database binding information.
        /// </summary>
        [Description("Occurs when data loading routines has been performed in a certain database binding information.")]
        public event DataBinderLoadingEventHandler AfterBindingDataLoad;

        /// <summary>
        /// Raises the AfterBindingDataLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterBindingDataLoad(DataBinderLoadingEventArgs e)
        {
            if (AfterBindingDataLoad != null) AfterBindingDataLoad(this, e);
        }

        /// <summary>
        /// Occurs after validation routines has been performed to a binded control.
        /// </summary>
        [Description("Occurs after validation routines has been performed to a binded control.")]
        public event DataBinderValidationEventHandler AfterControlValidation;

        /// <summary>
        /// Raises the AfterControlValidation event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterControlValidation(DataBinderValidationEventArgs e)
        {
            if (AfterControlValidation != null) AfterControlValidation(this, e);
        }

        /// <summary>
        /// Occurs after all data loading routines in all database binding information has been performed.
        /// </summary>
        [Description("Occurs after all data loading routines in all database binding information has been performed.")]
        public event EventHandler AfterDataLoad;

        /// <summary>
        /// Raises the AfterDataLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterDataLoad(EventArgs e)
        {
            if (AfterDataLoad != null) AfterDataLoad(this, e);
        }

        /// <summary>
        /// Occurs after data saving routines of the binder has been performed.
        /// </summary>
        [Description("Occurs after data saving routines of the binder has been performed.")]
        public event DataBinderSavingEventHandler AfterDataSave;

        /// <summary>
        /// Raises the AfterDataSave event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterDataSave(DataBinderSavingEventArgs e)
        {
            if (AfterDataSave != null) AfterDataSave(this, e);
        }

        /// <summary>
        /// Occurs after all the binded controls already passed the validation routines.
        /// </summary>
        [Description("Occurs after all the binded controls already passed the validation routines.")]
        public event DataBinderValidationEventHandler AfterValidation;

        /// <summary>
        /// Raises the AfterValidation event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterValidation(DataBinderValidationEventArgs e)
        {
            if (AfterValidation != null) AfterValidation(this, e);
        }

        /// <summary>
        /// Occurs before data loading routines for a certain database biding information executes.
        /// </summary>
        [Description("Occurs before data loading routines for a certain database biding information executes.")]
        public event DataBinderLoadingEventHandler BeforeBindingDataLoad;

        /// <summary>
        /// Raises the BeforeBindingDataLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeBindingDataLoad(DataBinderLoadingEventArgs e)
        {
            if (BeforeBindingDataLoad != null) BeforeBindingDataLoad(this, e);
        }

        /// <summary>
        /// Occurs before a binded control is evaluated for entry validation.
        /// </summary>
        [Description("Occurs before a binded control is evaluated for entry validation.")]
        public event DataBinderValidationEventHandler BeforeControlValidation;

        /// <summary>
        /// Raises the BeforeControlValidation event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeControlValidation(DataBinderValidationEventArgs e)
        {
            if (BeforeControlValidation != null) BeforeControlValidation(this, e);
        }

        /// <summary>
        /// Occurs when data loading method has been invoked.
        /// </summary>
        [Description("Occurs when data loading method has been invoked.")]
        public event EventHandler BeforeDataLoad;

        /// <summary>
        /// Raises the BeforeDataLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeDataLoad(EventArgs e)
        {
            if (BeforeDataLoad != null) BeforeDataLoad(this, e);
        }

        /// <summary>
        /// Occurs before data saving routines of the binder executes.
        /// </summary>
        [Description("Occurs before data saving routines of the binder executes.")]
        public event DataBinderSavingEventHandler BeforeDataSave;

        /// <summary>
        /// Raises the BeforeDataSave event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeDataSave(DataBinderSavingEventArgs e)
        {
            if (BeforeDataSave != null) BeforeDataSave(this, e);
        }

        /// <summary>
        /// Occurs before entry validation routines to all binded controls takes place.
        /// </summary>
        [Description("Occurs before entry validation routines to all binded controls takes place.")]
        public event DataBinderValidationEventHandler BeforeValidation;

        /// <summary>
        /// Raises the BeforeValidation event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeValidation(DataBinderValidationEventArgs e)
        {
            if (BeforeValidation != null) BeforeValidation(this, e);
        }

        /// <summary>
        /// Occurs upon data gathering routines of the binder before the actual data saving events.
        /// </summary>
        [Description("Occurs upon data gathering routines of the binder before the actual data saving events.")]
        public event EventHandler DataGathering;

        /// <summary>
        /// Raises the DataGathering event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataGathering(EventArgs e)
        {
            if (DataGathering != null) DataGathering(this, e);
        }

        /// <summary>
        /// Occurs upon data loading routines of the binder.
        /// </summary>
        [Description("Occurs upon data loading routines of the binder.")]
        public event EventHandler DataLoading;

        /// <summary>
        /// Raises the DataLoading event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataLoading(EventArgs e)
        {
            if (DataLoading != null) DataLoading(this, e);
        }

        /// <summary>
        /// Occurs before the actual data saving execution.
        /// </summary>
        [Description("Occurs before the actual data saving execution.")]
        public event DataBinderSavingEventHandler DataSaveExecuting;

        /// <summary>
        /// Raises the DataSaveExecuting event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataSaveExecuting(DataBinderSavingEventArgs e)
        {
            if (DataSaveExecuting != null) DataSaveExecuting(this, e);
        }

        /// <summary>
        /// Occurs upon data savingroutines of the binder.
        /// </summary>
        [Description("Occurs upon data savingroutines of the binder.")]
        public event EventHandler DataSaving;

        /// <summary>
        /// Raises the DataSaving event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataSaving(EventArgs e)
        {
            if (DataSaving != null) DataSaving(this, e);
        }

        /// <summary>
        /// Occurs upon data validation routines before the actual data saving events occur.
        /// </summary>
        [Description("Occurs upon data validation routines before the actual data saving events occur.")]
        public event EventHandler DataValidating;

        /// <summary>
        /// Raises the DataValidating event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataValidating(EventArgs e)
        {
            if (DataValidating != null) DataValidating(this, e);
        }

        /// <summary>
        /// Occurs when the component's hosted form invokes its Load event.
        /// </summary>
        [Description("Occurs when the component's hosted form invokes its Load event.")]
        public event EventHandler ParentFormLoad;

        /// <summary>
        /// Raises the ParentFormLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParentFormLoad(EventArgs e)
        {
            if (ParentFormLoad != null) ParentFormLoad(this, e);
        }

        /// <summary>
        /// Occurs when the component's hosted form already performs all of its data binding routines after it is shown.
        /// </summary>
        [Description("Occurs when the component's hosted form already performs all of its data binding routines after it is shown.")]
        public event EventHandler ParentFormShown;

        /// <summary>
        /// Raises the ParentFormShown event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParentFormShown(EventArgs e)
        {
            if (ParentFormShown != null) ParentFormShown(this, e);
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BindedControlCollection _bindedcontrols = null;

        /// <summary>
        /// Gets the list of binded controls.
        /// </summary>
        [Browsable(false)]
        public BindedControlCollection BindedControls
        {
            get { return _bindedcontrols; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinding _databinding = null;

        /// <summary>
        /// Gets the database binding information of the component.
        /// </summary>
        [Browsable(false)]
        public DataBinding Binding
        {
            get { return _databinding; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _cancelrunningprocess = false;

        /// <summary>
        /// Gets or sets whether the currently running loading and / or data saving processes will be cancelled or not.
        /// </summary>
        [Browsable(false)]
        public bool CancelRunningProcess
        {
            get { return _cancelrunningprocess; }
            set { _cancelrunningprocess = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDbConnection _connection = null;

        /// <summary>
        /// Gets or sets the database connection used to retrieve and save data from the binded controls and grids.
        /// </summary>
        [Browsable(false)]
        public IDbConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContainerControl _containercontrol = null;

        /// <summary>
        /// Gets or sets the container object to where the component is currently hosted into.
        /// </summary>
        [Browsable(false)]
        public ContainerControl ContainerControl
        {
            get
            { return _containercontrol; }
            set
            {
                if (value != null)
                {
                    Form _form = null;

                    try { _form = (Form)value; }
                    catch { _form = null; }

                    if (_form != null)
                    {
                        if (_containercontrol == null)
                        {
                            _form.Load += new EventHandler(ParentForm_Load);
                            _form.Shown += new EventHandler(ParentForm_Shown);
                        }
                        else
                        {
                            if (!_containercontrol.Equals(_form))
                            {
                                _form.Load += new EventHandler(ParentForm_Load);
                                _form.Shown += new EventHandler(ParentForm_Shown);
                            }
                        }

                        _containercontrol = _form;
                    }
                    else _containercontrol = value;
                }
                else _containercontrol = value;
                
                if (_errorprovider == null) InitializeErrorProvider();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ErrorProvider _errorprovider = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _haveupdates = false;

        /// <summary>
        /// Gets whether there was an update that occured to the currently binded data sources.
        /// </summary>
        [Browsable(false)]
        public bool HaveUpdates
        {
            get { return _haveupdates; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static List<Type> _othersupportedcontrols = new List<Type>();

        /// <summary>
        /// Gets the list of other types of control the DataBinder is bound to support other than the default ones.
        /// </summary>
        public static List<Type> OtherSupportedControls
        {
            get { return _othersupportedcontrols; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _parentformisshown = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _savebutton = null;

        /// <summary>
        /// Gets or sets the accosiated button to perform the record updating routines.
        /// </summary>
        [Browsable(false)]
        public object SaveButton
        {
            get { return _savebutton; }
            set
            {
                if (value != null)
                {
                    if (_savebutton == null)
                    {
                        if (Materia.EventExists(value, "Click"))
                        {
                            try { Materia.AttachHandler(value, "Click", new EventHandler(SaveButton_Click)); }
                            catch { }
                        }
                    }
                    else
                    {
                        if (!_savebutton.Equals(value))
                        {
                            if (Materia.EventExists(value, "Click"))
                            {
                                try { Materia.AttachHandler(value, "Click", new EventHandler(SaveButton_Click)); }
                                catch { }
                            }
                        }
                    }
                }
                _savebutton = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _saving = false;

        /// <summary>
        /// Gets whether the binder is currently executing its data saving routines or not.
        /// </summary>
        [Browsable(false)]
        public bool Saving
        {
            get { return _saving; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _sessionstarted = false;

        /// <summary>
        ///  Gets or sets the component's site.
        /// </summary>
        [Description("Gets or sets the component's site.")]
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                if (value != null)
                {
                    IDesignerHost _host = (IDesignerHost) value.GetService(typeof(IDesignerHost));
                    if (_host != null)
                    {
                        IComponent _componenthost = _host.RootComponent;
                        ContainerControl _container = null;
                        
                        try { _container = (ContainerControl)_componenthost; }
                        catch { _container = null; }

                        if (_container !=null) ContainerControl = _container;
                    }
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Icon _valdiatoricon = Properties.Resources.validator;

        /// <summary>
        /// Gets or sets the icon that will be used by the binder's internal error validator.
        /// </summary>
        [Description("Gets or sets the icon that will be used by the binder's internal error validator.")]
        public Icon ValidatorIcon
        {
            get { return _valdiatoricon; }
            set 
            { 
                _valdiatoricon = value;
                if (!Materia.IsNullOrNothing(value))
                {
                    if (_errorprovider != null)
                    {
                        _errorprovider.Dispose(); _errorprovider = null;
                        Materia.RefreshAndManageCurrentProcess();
                    }
                    InitializeErrorProvider();
                }
            }
        }

        #endregion

        #region "methods"

        private void AcceptAllChanges()
        { AcceptAllChanges(Binding); }

        private void AcceptAllChanges(DataBinding binding)
        {
            if (binding != null)
            {
                binding.AcceptChanges();
                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding db in binding.Details) AcceptAllChanges(db);
                }
            }
        }

        private void AcceptUpdates()
        { AcceptUpdates(Binding); }

        private void AcceptUpdates(DataBinding binding)
        {
            if (binding != null)
            {
                if (binding.Table != null)
                {
                    try { binding.Table.AcceptChanges(); }
                    catch { }

                    if (binding.Details.Count > 0)
                    {
                        foreach (DataBinding db in binding.Details) AcceptUpdates(db);
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether the specified object is supported by the DataBinder or not.
        /// </summary>
        /// <param name="extendee"></param>
        /// <returns></returns>
        public bool CanExtend(object extendee)
        {
            bool _extended = false;

            if (extendee != null)
            {
                if (extendee.GetType().Name.ToLower().Contains("TextBox".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("ComboBox".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("ComboTree".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("DateTimePicker".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("DateTimeInput".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("NumericUpDown".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("DoubleInput".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("IntegerInput".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("Label".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("PictureBox".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("CheckBox".ToLower()) ||
                    extendee.GetType().Name.ToLower().Contains("C1Combo".ToLower()) ||
                    _othersupportedcontrols.Contains(extendee.GetType())) _extended = true;
                else
                {
                    if (extendee.GetType().BaseType != null)
                    {
                        if (extendee.GetType().BaseType.Name.ToLower().Contains("TextBox".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("ComboBox".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("ComboTree".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("DateTimePicker".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("DateTimeInput".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("NumericUpDown".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("DoubleInput".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("IntegerInput".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("Label".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("PictureBox".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("CheckBox".ToLower()) ||
                            extendee.GetType().BaseType.Name.ToLower().Contains("C1Combo".ToLower()) ||
                            _othersupportedcontrols.Contains(extendee.GetType().BaseType)) _extended = true;
                    }
                }
            }

            return _extended;
        }

        private void DataBinding_DataLoading(object sender, EventArgs e)
        { OnDataLoading(e); }

        private void EnableBindedFields()
        { EnableBindedFields(true); }

        private void EnableBindedFields(bool enabled)
        {
            foreach (BindedControl bc in BindedControls)
            {
                object _control = bc.Control;
                if (_control != null)
                {
                    if (Materia.PropertyExists(_control, "Enabled"))
                    {
                        try { Materia.SetPropertyValue(_control, "Enabled", enabled); }
                        catch { }
                    }
                }
            }
        }

        private void Editor_ValueChanged(object sender, EventArgs e)
        {
            if (sender == null) return;
            if (!_parentformisshown) return;
            if (Materia.PropertyExists(sender, "Enabled"))
            {
                object _enabled = Materia.GetPropertyValue(sender, "Enabled");
                if (!VisualBasic.CBool(_enabled)) return;
            }
            MarkAsUpdated();
        }

        /// <summary>
        /// Determines the binded database field name into the specified control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        [Description("Determines the binded database field name into the specified control.")]
        public string GetFieldName(Control control)
        {
            string _field = "";
            if (BindedControls.Contains(control)) _field = BindedControls[control].FieldName;
            return _field;
        }

        private void Grid_Edit(object sender, object e)
        {
            if (!_parentformisshown) return;
            object _grid = sender;
            if (_grid == null) return;
            bool _enabled = Materia.GetPropertyValue<bool>(_grid, "Enabled", true);
            if (!_enabled) return;
            Form _currentform = null;

            try { _currentform = (Form)ContainerControl; }
            catch { _currentform = null; }

            if (_currentform == null) return;
            _currentform.MarkAsEdited(); _haveupdates = true;
        }

        private void GridFinishEdit()
        { GridFinishEdit(Binding); }

        private void GridFinishEdit(DataBinding binding)
        {
            if (binding != null)
            {
                if (binding.Grid != null)
                {
                    bool _allowediting = Materia.GetPropertyValue<bool>(binding.Grid, "AllowEditing", true);
                    object _datasource = Materia.GetPropertyValue(binding.Grid, "DataSource");

                    if (_allowediting && _datasource != null)
                    {
                        try { Materia.InvokeMethod(binding.Grid, "FinishEditing"); }
                        catch { }

                        bool _addnew = Materia.GetPropertyValue<bool>(binding.Grid, "AllowAddNew", true);
                        try { Materia.SetPropertyValue(binding.Grid, "AllowAddNew", false); }
                        catch { }

                        object _rows = Materia.GetPropertyValue(binding.Grid, "Rows");
                        if (_rows != null)
                        {
                            int _rowcount = Materia.GetPropertyValue<int>(_rows, "Count", 0);
                            int _rowfixed = Materia.GetPropertyValue<int>(_rows, "Fixed", 1);

                            if (_rowcount > _rowfixed)
                            {
                                try { Materia.SetPropertyValue(binding.Grid, "Row", _rowfixed - 1); }
                                catch { }
                            }
                        }

                        try { Materia.SetPropertyValue(binding.Grid, "AllowAddNew", _addnew); }
                        catch { }
                    }
                }

                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding db in binding.Details) GridFinishEdit(db);
                }
            }
        }

        private void InitializeEditableControls(object parent)
        {
            if (parent != null)
            {
                object _controls = null;

                try { _controls =  Materia.GetPropertyValue(parent, "Controls"); }
                catch { _controls = null; }

                if (_controls != null)
                {
                    int _controlcount = Materia.GetPropertyValue<int>(_controls, "Count", 0);
                    if (_controlcount > 0)
                    {
                        if (CanExtend(parent))
                        {
                            if (Materia.EventExists(parent, "CheckChanged"))
                            {
                                try { Materia.AttachHandler(parent, "CheckChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "SelectedIndexChanged"))
                            {
                                try
                                { Materia.AttachHandler(parent, "SelectedIndexChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "SelectedValueChanged"))
                            {
                                try { Materia.AttachHandler(parent, "SelectedValueChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "ValueChanged"))
                            {
                                try { Materia.AttachHandler(parent, "ValueChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            object _datasource = null;

                            if (Materia.PropertyExists(parent, "DataSource")) _datasource = Materia.GetPropertyValue(parent, "DataSource");

                            if (_datasource == null)
                            {
                                if (Materia.EventExists(parent, "TextChanged"))
                                {
                                    try { Materia.AttachHandler(parent, "TextChanged", new EventHandler(Editor_ValueChanged)); }
                                    catch { }
                                }
                            }
                        }
                        else
                        {

                            for (int i = 0; i <= (((Control.ControlCollection)_controls).Count - 1); i++)
                            {
                                Control _control = ((Control.ControlCollection)_controls)[i];
                                InitializeEditableControls(_control);
                            }
                        }
                    }
                    else
                    {
                        if (CanExtend(parent))
                        {
                            if (Materia.EventExists(parent, "CheckChanged"))
                            {
                                try { Materia.AttachHandler(parent, "CheckChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "SelectedIndexChanged"))
                            {
                                try
                                { Materia.AttachHandler(parent, "SelectedIndexChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "SelectedValueChanged"))
                            {
                                try { Materia.AttachHandler(parent, "SelectedValueChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            if (Materia.EventExists(parent, "ValueChanged"))
                            {
                                try { Materia.AttachHandler(parent, "ValueChanged", new EventHandler(Editor_ValueChanged)); }
                                catch { }
                            }

                            object _datasource = null;

                            if (Materia.PropertyExists(parent, "DataSource")) _datasource = Materia.GetPropertyValue(parent, "DataSource");

                            if (_datasource == null)
                            {
                                if (Materia.EventExists(parent, "TextChanged"))
                                {
                                    try { Materia.AttachHandler(parent, "TextChanged", new EventHandler(Editor_ValueChanged)); }
                                    catch { }
                                }
                            }
                        }
                        else
                        {
                            if (parent.GetType().Name.ToLower().Contains("C1FlexGrid".ToLower()) ||
                                parent.GetType().BaseType.Name.ToLower().Contains("C1FlexGrid".ToLower()))
                            {
                              
                            }
                        }
                    }
                }
            }
        }

        private void InitializeErrorProvider()
        {
            if (_errorprovider == null)
            {
                if (ContainerControl != null)
                {
                    _errorprovider = new ErrorProvider(ContainerControl);
                    _errorprovider.Icon = ValidatorIcon;
                    _errorprovider.BlinkRate = 250;
                    _errorprovider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
                }
            }
        }

        private void InitializeRequiredFields()
        {
            InitializeRequiredFields(Binding);
            if (Binding.Details.Count > 0)
            {
                foreach (DataBinding _binding in Binding.Details) InitializeRequiredFields(_binding);
            }
        }

        private void InitializeRequiredFields(DataBinding binding)
        {
            for (int i = 0; i <= (BindedControls.Count - 1); i++)
            {
                BindedControl bc = BindedControls[i];
                InitializeRequiredFields(binding, (Control)bc.Control);
            }
        }

        private void InitializeRequiredFields(DataBinding binding, Control control)
        {
            if (control == null) return;
            if (binding == null) return;
            if (BindedControls.Contains(control))
            {
                string _field = BindedControls[control].FieldName;
                if (!String.IsNullOrEmpty(_field.RLTrim()))
                {
                    if (binding.RequiredFields.Contains(_field) &&
                        binding.Grid == null) ProduceMarkerLabel(control);
                }
            }
        }

        private bool IsValid(Control control, bool condition, string notification)
        {
            InitializeErrorProvider();

            if (_errorprovider != null)
            {

                if (condition) _errorprovider.SetError(control, "");
                else
                {
                    _errorprovider.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                    _errorprovider.SetIconPadding(control, 5);
                    _errorprovider.SetError(control, notification);
                }

                return condition;
            }
            else return true;
        }

        /// <summary>
        /// Sets the binder to mark itself and the current hosted form as modified.
        /// </summary>
        public void MarkAsUpdated()
        {
            if (_parentformisshown)
            {
                if (ContainerControl == null) return;
                Form _currentform = null;

                try { _currentform = (Form)ContainerControl; }
                catch { _currentform = null; }

                if (_currentform != null)
                {
                    if (_sessionstarted)
                    {
                        for (int i = 0; i <= (BindedControls.Count - 1); i++)
                        {
                            Control _control = (Control)BindedControls[i].Control;
                            _errorprovider.SetError(_control, "");
                        }

                        _currentform.MarkAsEdited(); _haveupdates = true;
                    }
                }
            }
        }

        private void ParentForm_Load(object sender, EventArgs e)
        {
            Form _form = null;

            try { _form = (Form)sender; }
            catch { _form = null; }

            if (_form != null)
            {
                if (!DesignMode)
                {
                    _parentformisshown = false; 
                    Control.CheckForIllegalCrossThreadCalls = false;
                    _form.ManageOnDispose(); InitializeRequiredFields();
                    try { InitializeEditableControls(_form); }
                    catch { }
                    OnParentFormLoad(e);
                }
            }
        }

        private void ParentForm_Shown(object sender, EventArgs e)
        {
            Form _form = null;

            try { _form = (Form)sender; }
            catch { _form = null; }

            if (_form != null)
            {
                _parentformisshown = true; OnParentFormShown(e);
            }
        }

        private Label ProduceMarkerLabel(Control control)
        {
            Label _label = new Label();

            _label.Size = new Size(5, 5); _label.BackColor = Color.OrangeRed;
            _label.Location = control.Location; _label.Name = "lblNew_" + control.Name;
            _label.Visible = true; _label.Show(); _label.BringToFront();

            if (control.Parent != null)
            {
                control.Parent.Controls.Add(_label);
                _label.Show(); _label.BringToFront();
            }

            return _label;
        }

        /// <summary>
        /// Raises the event handler attached into the currently assigned object in the SaveButton property.
        /// </summary>
        public void RaiseSaveButtonClick()
        { SaveButton_Click(_savebutton, new EventArgs()); }

        private void RedrawGrids()
        { RedrawGrids(true); }

        private void RedrawGrids(bool redraw)
        { RedrawGrids(Binding, redraw); }

        private void RedrawGrids(DataBinding binding)
        { RedrawGrids(binding, true); }

        private void RedrawGrids(DataBinding binding, bool redraw)
        {
            if (binding != null)
            {
                if (binding.Grid != null)
                {
                    try
                    { Materia.SetPropertyValue(binding.Grid, "KeyActionEnter", 3); }
                    catch { }

                    try
                    { Materia.SetPropertyValue(binding.Grid, "SelectionMode", 3); }
                    catch { }

                    try { Materia.Redraw((Control)binding.Grid, redraw); }
                    catch { }
                }

                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding _binding in binding.Details) RedrawGrids(_binding, redraw);
                }
            }
        }

        /// <summary>
        /// Loads all of then specified component's binding information.
        /// </summary>
        public void Reload()
        {
            OnBeforeDataLoad(new EventArgs());
            Form _form = null; _sessionstarted = false;

            if (ContainerControl != null)
            {
                try { _form = (Form)ContainerControl; }
                catch { _form = null; }
            }

            if (_form != null)
            {
                if (_form.Text.RLTrim().EndsWith("*")) _form.Text = _form.Text.Replace("*", "");
            }

            foreach (BindedControl bc in BindedControls)
            {
                object _control = bc.Control;
                if (_control != null)
                {
                    if (Materia.PropertyExists(_control, "DataSource"))
                    {
                        object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                        if (_datasource != null)
                        {
                            if (Materia.PropertyExists(_control, "SelectedValue"))
                            {
                                try { Materia.SetPropertyValue(_control, "SelectedValue", null); }
                                catch
                                {
                                    if (Materia.PropertyExists(_control, "SelectedIndex"))
                                    {
                                        try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                        catch { }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Checked"))
                                        {
                                            try { Materia.SetPropertyValue(_control, "Checked", false); }
                                            catch { }
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Value"))
                                            {
                                                try { Materia.SetPropertyValue(_control, "Value", null); }
                                                catch { }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Image"))
                                                {
                                                    try { Materia.SetPropertyValue(_control, "Image", null); }
                                                    catch { }
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", "");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Materia.PropertyExists(_control, "SelectedIndex"))
                                {
                                    try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                    catch { }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(_control, "Checked"))
                                    {
                                        try { Materia.SetPropertyValue(_control, "Checked", false); }
                                        catch { }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Value"))
                                        {
                                            try { Materia.SetPropertyValue(_control, "Value", null); }
                                            catch { }
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Image"))
                                            {
                                                try { Materia.SetPropertyValue(_control, "Image", null); }
                                                catch { }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", "");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Materia.PropertyExists(_control, "SelectedIndex"))
                            {
                                try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                catch { }
                            }
                            else
                            {
                                if (Materia.PropertyExists(_control, "Checked"))
                                {
                                    try { Materia.SetPropertyValue(_control, "Checked", false); }
                                    catch { }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(_control, "Value"))
                                    {
                                        try { Materia.SetPropertyValue(_control, "Value", null); }
                                        catch { }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Image"))
                                        {
                                            try { Materia.SetPropertyValue(_control, "Image", null); }
                                            catch { }
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Materia.PropertyExists(_control, "SelectedIndex"))
                        {
                            try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                            catch { }
                        }
                        else
                        {
                            if (Materia.PropertyExists(_control, "Checked"))
                            {
                                try { Materia.SetPropertyValue(_control, "Checked", false); }
                                catch { }
                            }
                            else
                            {
                                if (Materia.PropertyExists(_control, "Value"))
                                {
                                    try { Materia.SetPropertyValue(_control, "Value", null); }
                                    catch { }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(_control, "Image"))
                                    {
                                        try { Materia.SetPropertyValue(_control, "Image", null); }
                                        catch { }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", "");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CancelRunningProcess = false; EnableBindedFields(false); RedrawGrids(false);
            DataBinderLoadingEventArgs _args = ReloadBinding(Binding);
            if (!_args.Cancel)
            {
                RedrawGrids(); EnableBindedFields(); OnAfterDataLoad(new EventArgs());
            }

            _sessionstarted = true;
        }

        private DataBinderLoadingEventArgs ReloadBinding(DataBinding binding)
        {
            IAsyncResult _result = binding.BeginLoad();
            while (!_result.IsCompleted &&
                   !CancelRunningProcess)
            {
                OnDataLoading(new EventArgs());
                Thread.Sleep(1); Application.DoEvents();
            }

            if (CancelRunningProcess)
            {
                if (!_result.IsCompleted)
                {
                    try { _result = null; }
                    catch { }
                }

                return null;
            }

            DataBinderLoadingEventArgs _args = binding.EndLoad(_result);
            if (!_args.Cancel)
            {
                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding db in binding.Details)
                    {
                       IAsyncResult _resultdetails = db.BeginLoad();

                        while (!_resultdetails.IsCompleted &&
                               !CancelRunningProcess)
                        {
                            OnDataLoading(new EventArgs());
                            Thread.Sleep(1); Application.DoEvents();
                        }

                        if (CancelRunningProcess)
                        {
                            if (!_resultdetails.IsCompleted)
                            {
                                try { _resultdetails = null; }
                                catch { }
                            }

                            return null;
                        }

                        _args = db.EndLoad(_resultdetails);
                        if (_args.Cancel) break;
                    }

                    return _args;
                }
                else return _args;
            }
            else return _args;
        }

        /// <summary>
        /// Saves the changes made to the binded data sources.
        /// </summary>
        public void Save()
        {
            Button _button = new Button(); SaveButton_Click(_button, new EventArgs());
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            object _button = sender;
            if (_button == null) return;
            bool _enabled = Materia.GetPropertyValue<bool>(_button, "Enabled", true);
            if (!_enabled) return;

            _saving = false; GridFinishEdit(); DataBinderValidationEventArgs _args = Validate();

            if (_args != null)
            {
                if (!_args.Valid)
                {
                    if (_args.Control != null)
                    {
                        if (_args.Control.GetType().Name.ToLower().Contains("C1FlexGrid".ToLower()) ||
                            _args.Control.GetType().BaseType.Name.ToLower().Contains("C1FlexGrid".ToLower()))
                        {
                            if (!String.IsNullOrEmpty(_args.Notification.RLTrim()))
                            {
                                SetControlFocus((Control) _args.Control); MessageBox.Show(_args.Notification, "Entry Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                bool _allowediting = true;

                                try { _allowediting = Materia.GetPropertyValue<bool>(_args.Control, "AllowEditing", true); }
                                catch { }

                                if (_allowediting)
                                {
                                    int _row = -1; int _col = -1;

                                    try { _row = Materia.GetPropertyValue<int>(_args.Control, "Row", -1); }
                                    catch { }

                                    try { _col = Materia.GetPropertyValue<int>(_args.Control, "Col", -1); }
                                    catch { }

                                    if (_row > -1 &&
                                        _col > -1)
                                    {
                                        try { Materia.InvokeMethod(_args.Control, "StartEditing", new object[] { _row, _col }); }
                                        catch { }
                                    }
                                }
                            }

                            return;
                        }
                        else return;
                    }
                    else return;
                }
                else
                {
                    if (_args.Cancel) return;
                }


                _enabled = Materia.GetPropertyValue<bool>(_button, "Enabled", true);
                try { Materia.SetPropertyValue(_button, "Enabled", false); }
                catch { }
                CancelRunningProcess = false; _saving = true;

                Action _valuesetdelegate = new Action(SetFieldValues);
                IAsyncResult _valuesetresult = _valuesetdelegate.BeginInvoke(null, _valuesetdelegate);

                while (!_valuesetresult.IsCompleted &&
                       !CancelRunningProcess)
                {
                    OnDataGathering(new EventArgs());
                    Thread.Sleep(1); Application.DoEvents();
                }

                if (CancelRunningProcess)
                {
                    if (!_valuesetresult.IsCompleted)
                    {
                        try { _valuesetresult = null; }
                        catch { }
                    }

                    try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                    catch { }

                    Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                }

                _valuesetdelegate.EndInvoke(_valuesetresult);

                Action _fksetdelegate = new Action(SetForeignKeys);
                IAsyncResult _fksetresult = _fksetdelegate.BeginInvoke(null, _fksetdelegate);

                while (!_fksetresult.IsCompleted &&
                       !CancelRunningProcess)
                {
                    OnDataGathering(new EventArgs());
                    Thread.Sleep(1); Application.DoEvents();
                }

                if (CancelRunningProcess)
                {
                    if (!_fksetresult.IsCompleted)
                    {
                        try { _fksetresult = null; }
                        catch { }
                    }

                    try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                    catch { }

                    Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                }

                _fksetdelegate.EndInvoke(_fksetresult);

                Func<string> _sqlgetdelegate = new Func<string>(Binding.GetUpdateStatements);
                IAsyncResult _sqlgetresult = _sqlgetdelegate.BeginInvoke(null, _sqlgetdelegate);

                while (!_sqlgetresult.IsCompleted &&
                       !CancelRunningProcess)
                {
                    OnDataGathering(new EventArgs());
                    Thread.Sleep(1); Application.DoEvents();
                }

                if (CancelRunningProcess)
                {
                    if (!_sqlgetresult.IsCompleted)
                    {
                        try { _sqlgetresult = null; }
                        catch { }
                    }

                    try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                    catch { }

                    Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                }

                string _sql = _sqlgetdelegate.EndInvoke(_sqlgetresult);
                try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                catch { }

                DataBinderSavingEventArgs _saveargs = new DataBinderSavingEventArgs(_sql);
                OnBeforeDataSave(_saveargs); AcceptUpdates();

                if (!_saveargs.Cancel)
                {
                    try { Materia.SetPropertyValue(_button, "Enabled", false); }
                    catch { }

                    _sql = "";

                    Func<string> _sqlgetfinaldelegate = new Func<string>(Binding.GetUpdateStatements);
                    IAsyncResult _sqlgetfinalresult = _sqlgetfinaldelegate.BeginInvoke(null, _sqlgetfinaldelegate);

                    while (!_sqlgetfinalresult.IsCompleted &&
                           !CancelRunningProcess)
                    {
                        OnDataGathering(new EventArgs());
                        Thread.Sleep(1); Application.DoEvents();
                    }

                    if (CancelRunningProcess)
                    {
                        if (_sqlgetfinalresult != null)
                        {
                            try { _sqlgetfinalresult = null; }
                            catch { }
                        }

                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }

                        Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                    }

                    string _query = _sqlgetfinaldelegate.EndInvoke(_sqlgetfinalresult);

                    if (WithBlobDataSource())
                    {
                        if (!String.IsNullOrEmpty(_query.RLTrim()))
                        {
                            if (!_query.RLTrim().ToLower().StartsWith("set global max_allowed_packet")) _sql = "SET GLOBAL max_allowed_packet = (1024 * 1204) * " + MySql.MaxAllowedPacket.ToString() + ";";
                        }
                    }

                    _sql += ((!String.IsNullOrEmpty(_sql.RLTrim())) ? "\n" : "") + _query;

                    if (String.IsNullOrEmpty(_sql.RLTrim()))
                    {
                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }

                        Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                    }

                    _saveargs.CommandText = _sql;
                    OnDataSaveExecuting(_saveargs);

                    if (_saveargs.Cancel)
                    {
                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }

                        Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                    }

                    _sql = _saveargs.CommandText;
                    IAsyncResult _saveresult = Que.BeginExecution(Connection, _sql, CommandExecution.ExecuteNonQuery);

                    while (!_saveresult.IsCompleted &&
                           !CancelRunningProcess)
                    {
                        OnDataSaving(new EventArgs());
                        Thread.Sleep(1); Application.DoEvents();
                    }

                    if (CancelRunningProcess)
                    {
                        if (!_saveresult.IsCompleted)
                        {
                            try { _saveresult = null; }
                            catch { }
                        }

                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }

                        Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                    }

                    QueResult _result = Que.EndExecution(_saveresult); _saveargs = null;

                    if (_result == null)
                    {
                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }

                        Materia.RefreshAndManageCurrentProcess(); _saving = false; return;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(_result.Error.RLTrim())) _saveargs = new DataBinderSavingEventArgs(_sql, _result.Error);
                        else _saveargs = new DataBinderSavingEventArgs(_sql, _result.RowsAffected);

                        if (!String.IsNullOrEmpty(_result.Error.RLTrim())) _saveargs.ErrorNotification = "An error occured while trying to save the changes and updates to the current record(s). Please try again and\n/ or report this issue to the system administrator.";
                        else
                        {
                            Action _updatedelegate = new Action(Update);
                            IAsyncResult _updateresult = _updatedelegate.BeginInvoke(null, _updatedelegate);

                            while (!_updateresult.IsCompleted)
                            {
                                OnDataLoading(new EventArgs());
                                Thread.Sleep(1); Application.DoEvents(); 
                            }

                            _updatedelegate.EndInvoke(_updateresult);
                        }

                        try { Materia.SetPropertyValue(_button, "Enabled", _enabled); }
                        catch { }
                        Materia.RefreshAndManageCurrentProcess(); _saving = false;
                        AcceptUpdates(); OnAfterDataSave(_saveargs);

                        if (!String.IsNullOrEmpty(_saveargs.Error.RLTrim()) &&
                            !String.IsNullOrEmpty(_saveargs.ErrorNotification.RLTrim())) MessageBox.Show(_saveargs.ErrorNotification, "Record Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                        if (!_saveargs.Cancel)
                        {
                            AcceptAllChanges(); _haveupdates = false;

                            Form _form = null;

                            try { _form = (Form)ContainerControl; }
                            catch { _form = null; }

                            if (_form != null)
                            {
                                if (_form.Text.RLTrim().EndsWith("*")) _form.Text = _form.Text.Replace("*", "");
                            }
                        }
                    }
                }
            }


        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _seltab = null;

        private void SetControlFocus(Control control)
        {
            if (control == null) return;

            if (control.Parent != null)
            {
                if (control.Parent.GetType().FullName == "System.Windows.Forms.TabControl" ||
                    control.Parent.GetType().FullName == "DevComponents.DotNetBar.SuperTabControl" ||
                    control.Parent.GetType().FullName == "DevComponents.DotNetBar.TabControl")
                {
                    if (_seltab != null)
                    {
                        try { Materia.SetPropertyValue(control.Parent, "SelectedTab", _seltab); }
                        catch { }
                    }
                    _seltab = null;
                    if (control.Parent.Parent != null) SetControlFocus(control.Parent);
                }
                else if (control.Parent.GetType().FullName == "DevComponents.DotNetBar.ExpandablePanel")
                {
                    try { Materia.SetPropertyValue(control.Parent, "Expanded", true); }
                    catch { }
                    if (control.Parent.Parent != null) SetControlFocus(control.Parent);
                }
                else if (control.Parent.GetType().FullName == "DevComponents.DotNetBar.TabControlPanel" ||
                         control.Parent.GetType().FullName == "DevComponents.DotNetBar.SuperTabControlPanel")
                {
                    _seltab = null;
                    try { _seltab = Materia.GetPropertyValue(control.Parent, "TabItem"); }
                    catch { _seltab = null; }

                    SetControlFocus(control.Parent);
                }
                else if (control.Parent.GetType().FullName == "System.Windows.Forms.TabPage")
                {
                    _seltab = null;
                    try { _seltab = Materia.GetPropertyValue(control.Parent, "Parent"); }
                    catch { _seltab = null; }

                    if (_seltab != null)
                    {
                        if (_seltab.GetType().FullName != "System.Windows.Forms.TabControl") _seltab = null;
                    }

                    SetControlFocus(control.Parent);
                }
                else SetControlFocus(control.Parent);
            }

            control.Focus();
        }

        /// <summary>
        /// Determines the binded database field name into the specified control.
        /// </summary>
        /// <param name="control">Binded control</param>
        /// <param name="fieldname">Database field name to bind to control into</param>
        [Description("Determines the binded database field name into the specified control.")]
        public void SetFieldName(Control control, string fieldname)
        { _bindedcontrols.Add(control, fieldname); }

        private void SetFieldValues()
        { SetFieldValues(Binding); }

        private void SetFieldValues(DataBinding binding)
        {
            if (binding != null)
            {
                binding.SetFieldValues();

                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding _binding in binding.Details) SetFieldValues(_binding);
                }
            }
        }

        private void SetForeignKeys()
        { SetForeignKeys(Binding); }

        private void SetForeignKeys(DataBinding binding)
        {
            SetForeignKeyValues(binding);

            if (binding.Details.Count > 0)
            {
                foreach (DataBinding _binding in binding.Details) SetForeignKeyValues(_binding);
            }
        }

        private void SetForeignKeyValues(DataBinding binding)
        {
            if (binding.Header != null)
            {
                DataTable _htable = binding.Header.Table;
                if (_htable != null)
                {
                    if (_htable.Columns.Count > 0 &&
                        _htable.Rows.Count > 0)
                    {
                        string _pk = binding.Header.PrimaryKey;

                        if (String.IsNullOrEmpty(_pk.RLTrim()))
                        {
                            foreach (DataColumn _column in _htable.Columns)
                            {
                                if (_column.Unique)
                                { _pk = _column.ColumnName; break; }
                            }
                        }
                        else
                        {
                            if (!_htable.Columns.Contains(_pk))
                            {
                                foreach (DataColumn _column in _htable.Columns)
                                {
                                    if (_column.Unique)
                                    { _pk = _column.ColumnName; break; }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(_pk.RLTrim()))
                        {
                            DataTable _table = binding.Table;

                            if (_table != null)
                            {
                                if (_table.Columns.Count > 0 &&
                                    _table.Rows.Count > 0)
                                {
                                    string _fk = binding.ForeignKey;

                                    if (!String.IsNullOrEmpty(_fk.RLTrim()))
                                    {
                                        if (!_table.Columns.Contains(_fk))
                                        {
                                            if (_table.Columns.Contains(_pk)) _fk = _pk;
                                            else _fk = "";
                                        }
                                    }
                                    else
                                    {
                                        if (_table.Columns.Contains(_pk)) _fk = _pk;
                                        else _fk = "";
                                    }

                                    if (!String.IsNullOrEmpty(_fk.RLTrim()))
                                    {
                                        object _fkvalue = _htable.Rows[0][_pk];

                                        foreach (DataRow _row in _table.Rows)
                                        {
                                            if (_row.RowState != DataRowState.Deleted &&
                                                _row.RowState != DataRowState.Detached) _row[_fk] = _fkvalue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Update()
        { Update(Binding); }

        private void Update(DataBinding binding)
        {
            if (binding != null)
            {
                binding.Update();

                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding _binding in binding.Details) Update(_binding);
                }
            }
        }

        private DataBinderValidationEventArgs Validate()
        {
            DataBinderValidationEventArgs _args = new DataBinderValidationEventArgs();
            OnBeforeValidation(_args);

            if (!_args.Cancel)
            {
                Form _form = null;

                try { _form = (Form)ContainerControl; }
                catch { }

                foreach (BindedControl bc in BindedControls)
                {
                    object _control = bc.Control;
                    if (_control != null)
                    {
                        _args = null; Materia.RefreshAndManageCurrentProcess(); 
                        _args = new DataBinderValidationEventArgs(_control);
                        OnBeforeControlValidation(_args);
                        _args = null; Materia.RefreshAndManageCurrentProcess();
                        _args = ValidateControls(_control);
                        OnDataValidating(new EventArgs());
                        if (_args != null)
                        {
                            OnAfterControlValidation(_args);

                            if (!_args.Valid)
                            {
                                SetControlFocus((Control)_control);
                                bool _valid = IsValid((Control)_control, _args.Valid, _args.Notification);
                                break;
                            }
                        }
                    }
                }

                if (_args.Valid)
                {
                    _args = null; Materia.RefreshAndManageCurrentProcess();
                    _args = Validate(Binding, _args);
                }
            }

            if (_args == null) _args = new DataBinderValidationEventArgs();
            OnAfterValidation(_args);

            return _args;
        }

        private DataBinderValidationEventArgs Validate(DataBinding binding, DataBinderValidationEventArgs args)
        {
            DataBinderValidationEventArgs _args = null;

            if (binding != null)
            {
                if (binding.Grid != null)
                {
                    _args = new DataBinderValidationEventArgs(binding.Grid);
                    OnBeforeControlValidation(_args);
                    _args = ValidateGrid(binding.Grid);
                    OnAfterControlValidation(_args);
                }

                if (_args == null)
                {
                    if (binding.Details.Count > 0)
                    {
                        foreach (DataBinding db in binding.Details)
                        {
                            _args = Validate(db, _args);
                            if (_args != null)
                            {
                                if (_args.Cancel) break;
                            }
                        }
                    }
                }
                else
                {
                    if (!_args.Cancel)
                    {
                        if (binding.Details.Count > 0)
                        {
                            foreach (DataBinding db in binding.Details)
                            {
                                _args = Validate(db, _args);
                                if (_args != null)
                                {
                                    if (_args.Cancel) break;
                                }
                            }
                        }
                    }
                }
            }

            return _args;
        }

        private DataBinderValidationEventArgs ValidateControls(object control)
        {
            if (control != null)
            {
                DataBinderValidationEventArgs _args = new DataBinderValidationEventArgs((Control) control);

                if (control.GetType().Name.ToLower().Contains("C1FlexGrid".ToLower()) ||
                    control.GetType().BaseType.Name.ToLower().Contains("C1FlexGrid".ToLower())) _args = ValidateGrid(control);
                else
                {
                    if (BindedControls.Contains(control))
                    {
                        string _validation = ""; string _field = BindedControls[control].FieldName;
                        DataTable _table = ValidationDataSource(_field);

                        if (_table != null)
                        {
                            DataColumn _column = _table.Columns[_field];

                            if (_column.DataType.Name == typeof(string).Name ||
                                _column.DataType.Name == typeof(String).Name)
                            {
                                if (Materia.PropertyExists(control, "DataSource"))
                                {
                                    object _datasource = Materia.GetPropertyValue(control, "DataSource");
                                    if (_datasource != null)
                                    {
                                        if (Materia.PropertyExists(control, "SelectedValue"))
                                        {
                                            object _selectedvalue = Materia.GetPropertyValue(control, "SelectedValue");
                                            if (Materia.IsNullOrNothing(_selectedvalue)) _validation = "Please select a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "SelectedIndex"))
                                            {
                                                int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                                if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(control, "Text"))
                                                {
                                                    string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                    if (String.IsNullOrEmpty(_text.RLTrim())) _validation = "Please specify a proper value for this field.";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "SelectedIndex"))
                                        {
                                            int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                            if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "Text"))
                                            {
                                                string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                if (String.IsNullOrEmpty(_text.RLTrim())) _validation = "Please specify a proper value for this field.";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(control, "SelectedIndex"))
                                    {
                                        int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                        if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "Text"))
                                        {
                                            string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                            if (String.IsNullOrEmpty(_text.RLTrim())) _validation = "Please specify a proper value for this field.";
                                        }
                                    }
                                }
                            }
                            else if (_column.DataType.Name == typeof(DateTime).Name)
                            {
                                if (Materia.PropertyExists(control, "DataSource"))
                                {
                                    object _datasource = Materia.GetPropertyValue(control, "DataSource");
                                    if (_datasource != null)
                                    {
                                        if (Materia.PropertyExists(control, "SelectedValue"))
                                        {
                                            object _selectedvalue = Materia.GetPropertyValue(control, "SelectedValue");
                                            if (!VisualBasic.IsDate(_selectedvalue)) _validation = "Please select a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "Value"))
                                            {
                                                object _value = Materia.GetPropertyValue(control, "Value");
                                                if (!VisualBasic.IsDate(_value)) _validation = "Please specify a proper value for this field.";
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(control, "Text"))
                                                {
                                                    string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                    if (!VisualBasic.IsDate(_text)) _validation = "Please specify a proper value for this field.";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "Value"))
                                        {
                                            object _value = Materia.GetPropertyValue(control, "Value");
                                            if (!VisualBasic.IsDate(_value)) _validation = "Please specify a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "Text"))
                                            {
                                                string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                if (!VisualBasic.IsDate(_text)) _validation = "Please specify a proper value for this field.";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(control, "Value"))
                                    {
                                        object _value = Materia.GetPropertyValue(control, "Value");
                                        if (!VisualBasic.IsDate(_value)) _validation = "Please specify a proper value for this field.";
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "Text"))
                                        {
                                            string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                            if (!VisualBasic.IsDate(_text)) _validation = "Please specify a proper value for this field.";
                                        }
                                    }
                                }
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
                                if (Materia.PropertyExists(control, "DataSource"))
                                {
                                    object _datasource = Materia.GetPropertyValue(control, "DataSource");
                                    if (_datasource != null)
                                    {
                                        if (Materia.PropertyExists(control, "SelectedValue"))
                                        {
                                            object _selectedvalue = Materia.GetPropertyValue(control, "SelectedValue");
                                            if (!VisualBasic.IsNumeric(_selectedvalue)) _validation = "Please select a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "Value"))
                                            {
                                                object _selectedvalue = Materia.GetPropertyValue(control, "Value");
                                                if (!VisualBasic.IsNumeric(_selectedvalue)) _validation = "Please specify a proper value for this field.";
                                                else
                                                {
                                                    if (VisualBasic.CDbl(_selectedvalue) <= 0) _validation = "Please specify a proper value for this field.";
                                                }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(control, "SelectedIndex"))
                                                {
                                                    int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                                    if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(control, "Text"))
                                                    {
                                                        string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                        if (!VisualBasic.IsNumeric(_text)) _validation = "Please specify a proper value for this field.";
                                                        else
                                                        {
                                                            if (VisualBasic.CDbl(_text) <= 0) _validation = "Please specify a proper value for this field.";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "Value"))
                                        {
                                            object _selectedvalue = Materia.GetPropertyValue(control, "Value");
                                            if (!VisualBasic.IsNumeric(_selectedvalue)) _validation = "Please specify a proper value for this field.";
                                            else
                                            {
                                                if (VisualBasic.CDbl(_selectedvalue) <= 0) _validation = "Please specify a proper value for this field.";
                                            }
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "SelectedIndex"))
                                            {
                                                int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                                if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(control, "Text"))
                                                {
                                                    string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                    if (!VisualBasic.IsNumeric(_text)) _validation = "Please specify a proper value for this field.";
                                                    else
                                                    {
                                                        if (VisualBasic.CDbl(_text) <= 0) _validation = "Please specify a proper value for this field.";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Materia.PropertyExists(control, "Value"))
                                    {
                                        object _selectedvalue = Materia.GetPropertyValue(control, "Value");
                                        if (!VisualBasic.IsNumeric(_selectedvalue)) _validation = "Please specify a proper value for this field.";
                                        else
                                        {
                                            if (VisualBasic.CDbl(_selectedvalue) <= 0) _validation = "Please specify a proper value for this field.";
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(control, "SelectedIndex"))
                                        {
                                            int _selectedindex = Materia.GetPropertyValue<int>(control, "SelectedIndex", -1);
                                            if (_selectedindex < 0) _validation = "Please select a proper value for this field.";
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(control, "Text"))
                                            {
                                                string _text = Materia.GetPropertyValue<string>(control, "Text", "");
                                                if (!VisualBasic.IsNumeric(_text)) _validation = "Please specify a proper value for this field.";
                                                else
                                                {
                                                    if (VisualBasic.CDbl(_text) <= 0) _validation = "Please specify a proper value for this field.";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                                    _column.DataType.Name.ToLower().Contains("byte()") ||
                                    _column.DataType.Name.ToLower().Contains("bytes[]") ||
                                    _column.DataType.Name.ToLower().Contains("bytes()"))
                                {
                                    if (Materia.PropertyExists(control, "Image"))
                                    {
                                        object _image = Materia.GetPropertyValue(control, "Image");
                                        if (Materia.IsNullOrNothing(_image)) _validation = "Please specify a value for this field.";
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(_validation.RLTrim())) _args = new DataBinderValidationEventArgs((Control) control, false);
                        _args.Notification = _validation;
                    }
                }

                return _args;
            }
            else return null;
        }

        private DataBinderValidationEventArgs ValidateGrid(object grid)
        {
            if (grid != null)
            {
                DataBinderValidationEventArgs _args = new DataBinderValidationEventArgs((Control) grid);
                string _notification = ""; object _datasource = null;
                
                if (Materia.PropertyExists(grid, "DataSource"))
                {
                    try { _datasource = Materia.GetPropertyValue(grid, "DataSource"); }
                    catch { _datasource = null; }
                }

                if (_datasource != null)
                {
                    DataBinding _binding = ValidationGridBinding(grid);
                    if (_binding != null)
                    {
                        bool _addnew = true;
                        if (Materia.PropertyExists(grid, "AllowAddNew"))
                        {
                            _addnew = Materia.GetPropertyValue<bool>(grid, "AllowAddNew", false);
                            Materia.SetPropertyValue(grid, "AllowAddNew", false);
                        }

                        if (Materia.PropertyExists(grid, "Rows") &&
                            Materia.PropertyExists(grid, "Cols"))
                        {
                            object _rows = Materia.GetPropertyValue(grid, "Rows");
                            int _rowcount = Materia.GetPropertyValue<int>(_rows, "Count");
                            int _rowfixed = Materia.GetPropertyValue<int>(_rows, "Fixed");

                            if (_rowcount == _rowfixed) _notification = "Please specify at least an entry detail.";

                            if (String.IsNullOrEmpty(_notification.RLTrim()))
                            {
                                DataTable _table = null;

                                try { _table = (DataTable)_datasource; }
                                catch { _table = null; }

                                if (_table != null)
                                {
                                    object _cols = null;

                                    try { _cols = Materia.GetPropertyValue(grid, "Cols"); }
                                    catch { _cols = null; }

                                    if (_cols != null)
                                    {
                                        foreach (DataColumn _column in _table.Columns)
                                        {
                                            if (_binding.RequiredFields.Contains(_column.ColumnName))
                                            {
                                                object _col = null;

                                                try { _col = Materia.GetPropertyValue<object>(_cols, "Item", new object[] { _column.ColumnName}, null); }
                                                catch { _col = null; }

                                                if (_col != null)
                                                {
                                                    object _coleditor = Materia.GetPropertyValue(_col, "Editor");
                                                    string _colcaption = Materia.GetPropertyValue<string>(_col, "Caption", "");

                                                    if (_column.DataType.Name == typeof(string).Name ||
                                                        _column.DataType.Name == typeof(String).Name)
                                                    {
                                                        if (_coleditor != null)
                                                        {
                                                            object _editordatasource = null;

                                                            try { _editordatasource = Materia.GetPropertyValue(_coleditor, "DataSource"); }
                                                            catch { _editordatasource = null; }

                                                            if (_editordatasource != null)
                                                            {
                                                                for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                {
                                                                    object _row = null;

                                                                    try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null ); }
                                                                    catch { _row = null; }

                                                                    if (_row != null)
                                                                    {
                                                                        object _value = null;

                                                                        try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                        catch { _value = null; }

                                                                        if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                        else
                                                                        {
                                                                            if (String.IsNullOrEmpty(_value.ToString().RLTrim())) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                string _displaymember = "";

                                                                                try { _displaymember = Materia.GetPropertyValue<string>(_coleditor, "DisplayMember", ""); }
                                                                                catch { _displaymember = ""; }

                                                                                if (!String.IsNullOrEmpty(_displaymember.RLTrim()))
                                                                                {
                                                                                    DataTable _coleditortable = null;
                                                                                    try { _coleditortable = (DataTable)_editordatasource; }
                                                                                    catch { _coleditortable = null; }

                                                                                    if (_coleditortable != null)
                                                                                    {
                                                                                        object _selvalue = _coleditortable.GetValue<object>("(CONVERT([" + _displaymember + "], System.String) LIKE '" + _value.ToString().ToSqlValidString(true) + "')", _displaymember);
                                                                                        if (Materia.IsNullOrNothing(_selvalue)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                    {
                                                                        int _subindex = _rowfixed - 1;
                                                                        if (_subindex <= 0) _subindex = 0;

                                                                        try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                        catch { }

                                                                        int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                        try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                        catch { }

                                                                        _notification = String.Format(_notification, i - _subindex); break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.MethodExists(_coleditor, "FindStringExact"))
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (String.IsNullOrEmpty(_value.ToString().RLTrim())) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                else
                                                                                {
                                                                                    int _index = Materia.GetMethodValue<int>(_coleditor, "FindStringExact", new object[] { _value.ToString() }, -1);
                                                                                    if (_index < 0) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                }
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i } ,null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (String.IsNullOrEmpty(_value.ToString().RLTrim())) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                            {
                                                                object _row = null;

                                                                try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                catch { _row = null; }

                                                                if (_row != null)
                                                                {
                                                                    object _value = null;

                                                                    try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName}, null); }
                                                                    catch { _value = null; }

                                                                    if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                    else
                                                                    {
                                                                        if (String.IsNullOrEmpty(_value.ToString().RLTrim())) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                    }
                                                                }

                                                                if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                {
                                                                    int _subindex = _rowfixed - 1;
                                                                    if (_subindex <= 0) _subindex = 0;

                                                                    try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                    catch { }

                                                                    int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                    try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                    catch { }

                                                                    _notification = String.Format(_notification, i - _subindex); break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (_column.DataType.Name == typeof(DateTime).Name)
                                                    {
                                                        if (_coleditor != null)
                                                        {
                                                            object _editordatasource = null;

                                                            try { _editordatasource = Materia.GetPropertyValue(_coleditor, "DataSource"); }
                                                            catch { _editordatasource = null; }

                                                            if (_editordatasource != null)
                                                            {
                                                                for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                {
                                                                    object _row = null;

                                                                    try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                    catch { _row = null; }

                                                                    if (_row != null)
                                                                    {
                                                                        object _value = null;

                                                                        try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                        catch { _value = null; }

                                                                        if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                        else
                                                                        {
                                                                            if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                string _displaymember = "";

                                                                                try { _displaymember = Materia.GetPropertyValue<string>(_coleditor, "DisplayMember", ""); }
                                                                                catch { _displaymember = ""; }

                                                                                if (!String.IsNullOrEmpty(_displaymember.RLTrim()))
                                                                                {
                                                                                    DataTable _coleditortable = null;
                                                                                    try { _coleditortable = (DataTable)_editordatasource; }
                                                                                    catch { _coleditortable = null; }

                                                                                    if (_coleditortable != null)
                                                                                    {
                                                                                        object _selvalue = _coleditortable.GetValue<object>("(CONVERT([" + _displaymember + "], System.String) LIKE '" + _value.ToString().ToSqlValidString(true) + "')", _displaymember);
                                                                                        if (Materia.IsNullOrNothing(_selvalue)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                    {
                                                                        int _subindex = _rowfixed - 1;
                                                                        if (_subindex <= 0) _subindex = 0;

                                                                        try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                        catch { }

                                                                        int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                        try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                        catch { }

                                                                        _notification = String.Format(_notification, i - _subindex); break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.MethodExists(_coleditor, "FindStringExact"))
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                else
                                                                                {
                                                                                    int _index = Materia.GetMethodValue<int>(_coleditor, "FindStringExact", new object[] { _value.ToString() }, -1);
                                                                                    if (_index < 0) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                }
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] {_column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                            {
                                                                object _row = null;

                                                                try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                catch { _row = null; }

                                                                if (_row != null)
                                                                {
                                                                    object _value = null;

                                                                    try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName } ,null); }
                                                                    catch { _value = null; }

                                                                    if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                    else
                                                                    {
                                                                        if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                    }
                                                                }

                                                                if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                {
                                                                    int _subindex = _rowfixed - 1;
                                                                    if (_subindex <= 0) _subindex = 0;

                                                                    try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                    catch { }

                                                                    int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                    try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                    catch { }

                                                                    _notification = String.Format(_notification, i - _subindex); break;
                                                                }
                                                            }
                                                        }
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
                                                        if (_coleditor != null)
                                                        {
                                                            object _editordatasource = null;

                                                            try { _editordatasource = Materia.GetPropertyValue(_coleditor, "DataSource"); }
                                                            catch { _editordatasource = null; }

                                                            if (_editordatasource != null)
                                                            {
                                                                for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                {
                                                                    object _row = null;

                                                                    try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                    catch { _row = null; }

                                                                    if (_row != null)
                                                                    {
                                                                        object _value = null;

                                                                        try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName } , null); }
                                                                        catch { _value = null; }

                                                                        if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                        else
                                                                        {
                                                                            if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                string _displaymember = "";

                                                                                try { _displaymember = Materia.GetPropertyValue<string>(_coleditor, "DisplayMember", ""); }
                                                                                catch { _displaymember = ""; }

                                                                                if (!String.IsNullOrEmpty(_displaymember.RLTrim()))
                                                                                {
                                                                                    DataTable _coleditortable = null;
                                                                                    try { _coleditortable = (DataTable)_editordatasource; }
                                                                                    catch { _coleditortable = null; }

                                                                                    if (_coleditortable != null)
                                                                                    {
                                                                                        object _selvalue = _coleditortable.GetValue<object>("(CONVERT([" + _displaymember + "], System.String) LIKE '" + _value.ToString().ToSqlValidString(true) + "')", _displaymember);
                                                                                        if (Materia.IsNullOrNothing(_selvalue)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                    {
                                                                        int _subindex = _rowfixed - 1;
                                                                        if (_subindex <= 0) _subindex = 0;

                                                                        try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                        catch { }

                                                                        int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                        try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                        catch { }

                                                                        _notification = String.Format(_notification, i - _subindex); break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.MethodExists(_coleditor, "FindStringExact"))
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (!VisualBasic.IsDate(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                else
                                                                                {
                                                                                    int _index = Materia.GetMethodValue<int>(_coleditor, "FindStringExact", new object[] { _value.ToString() }, -1);
                                                                                    if (_index < 0) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                }
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                                    {
                                                                        object _row = null;

                                                                        try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                        catch { _row = null; }

                                                                        if (_row != null)
                                                                        {
                                                                            object _value = null;

                                                                            try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                            catch { _value = null; }

                                                                            if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                            else
                                                                            {
                                                                                if (!VisualBasic.IsNumeric(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                                else
                                                                                {
                                                                                    if (VisualBasic.CDbl(_value) == 0) _notification = "Please specify a proper value for " + _colcaption + " at row {0}.";
                                                                                }
                                                                            }
                                                                        }

                                                                        if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                        {
                                                                            int _subindex = _rowfixed - 1;
                                                                            if (_subindex <= 0) _subindex = 0;

                                                                            try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                            catch { }

                                                                            int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                            try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                            catch { }

                                                                            _notification = String.Format(_notification, i - _subindex); break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = _rowfixed; i <= (_rowcount - 1); i++)
                                                            {
                                                                object _row = null;

                                                                try { _row = Materia.GetPropertyValue<object>(_rows, "Item", new object[] { i }, null); }
                                                                catch { _row = null; }

                                                                if (_row != null)
                                                                {
                                                                    object _value = null;

                                                                    try { _value = Materia.GetPropertyValue<object>(_row, "Item", new object[] { _column.ColumnName }, null); }
                                                                    catch { _value = null; }

                                                                    if (Materia.IsNullOrNothing(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                    else
                                                                    {
                                                                        if (!VisualBasic.IsNumeric(_value)) _notification = "Please specify a value for " + _colcaption + " at row {0}.";
                                                                        else
                                                                        {
                                                                            if (VisualBasic.CDbl(_value) == 0) _notification = "Please specify a proper value for " + _colcaption + " at row {0}.";
                                                                        }
                                                                    }
                                                                }

                                                                if (!String.IsNullOrEmpty(_notification.RLTrim()))
                                                                {
                                                                    int _subindex = _rowfixed - 1;
                                                                    if (_subindex <= 0) _subindex = 0;

                                                                    try { Materia.SetPropertyValue(grid, "Row", i); }
                                                                    catch { }

                                                                    int _colfixed = Materia.GetPropertyValue<int>(_cols, "Fixed", 1);

                                                                    try { Materia.SetPropertyValue(grid, "Col", _column.Ordinal + _colfixed); }
                                                                    catch { }

                                                                    _notification = String.Format(_notification, i - _subindex); break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (!String.IsNullOrEmpty(_notification.RLTrim())) break;
                                        }
                                    }
                                }
                            }
                        }

                        if (Materia.PropertyExists(grid, "AllowAddNew"))
                        {
                            try { Materia.SetPropertyValue(grid, "AllowAddNew", _addnew); }
                            catch { }
                        }

                        if (!String.IsNullOrEmpty(_notification.RLTrim())) _args = new DataBinderValidationEventArgs(grid, false);
                        else
                        {
                            if (_args == null) _args = new DataBinderValidationEventArgs(grid);
                        }

                        _args.Notification = _notification;

                        return _args;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        private DataTable ValidationDataSource(string field)
        { return ValidationDataSource(field, Binding); }

        private DataTable ValidationDataSource(string field, DataBinding binding)
        {
            DataTable _table = null;

            if (binding != null)
            {
                if (!String.IsNullOrEmpty(field.RLTrim()))
                {
                    if (binding.Grid == null)
                    {
                        if (binding.Table != null)
                        {
                            if (binding.Table.Columns.Contains(field) &&
                                binding.RequiredFields.Contains(field)) _table = binding.Table;
                        }
                    }

                    if (_table == null)
                    {
                        if (binding.Details.Count > 0)
                        {
                            foreach (DataBinding db in binding.Details)
                            {
                                _table = ValidationDataSource(field, db);
                                if (_table != null) break;
                            }
                        }
                    }
                }
            }

            return _table;
        }

        private DataBinding ValidationGridBinding(object grid)
        { return ValidationGridBinding(grid, Binding); }

        private DataBinding ValidationGridBinding(object grid, DataBinding binding)
        {
            DataBinding _binding = null;

            if (grid != null &&
                binding != null)
            {
                if (binding.Grid != null)
                {
                    if (binding.Grid == grid) _binding = binding;
                }

                if (_binding == null)
                {
                    if (binding.Details.Count > 0)
                    {
                        foreach (DataBinding db in binding.Details)
                        {
                            _binding = ValidationGridBinding(grid, db);
                            if (_binding != null) break;
                        }
                    }
                }
            }

            return _binding;
        }

        private bool WithBlobDataSource()
        { return WithBlobDataSource(Binding); }

        private bool WithBlobDataSource(DataBinding binding)
        {
            if (binding != null)
            {
                DataTable _table = binding.Table;

                if (_table != null)
                {
                    foreach (DataColumn _column in _table.Columns)
                    {
                        if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                            _column.DataType.Name.ToLower().Contains("byte()") ||
                            _column.DataType.Name.ToLower().Contains("bytes[]") ||
                            _column.DataType.Name.ToLower().Contains("bytes()")) return true;
                    }
                }

                if (binding.Details.Count > 0)
                {
                    foreach (DataBinding _binding in binding.Details)
                    {
                        if (WithBlobDataSource(_binding)) return true;
                    }
                }
            }

            return false;
        }

        #endregion 

    }

    /// <summary>
    /// DataBinder binded control information class.
    /// </summary>
    public class BindedControl
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of BindedControl.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="control"></param>
        /// <param name="field"></param>
        public BindedControl(BindedControlCollection parent, object control, string field)
        {
            _parent = parent; _control = control; _fieldname = field;
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _control = null;

        /// <summary>
        /// Gets the currently binded control.
        /// </summary>
        public object Control
        {
            get { return _control; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _fieldname = "";

        /// <summary>
        /// Gets the currently binded database field name into the current binded control.
        /// </summary>
        public string FieldName
        {
            get { return _fieldname; }

        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BindedControlCollection _parent = null;

        /// <summary>
        /// Gets the collection to where the binded control is parented into.
        /// </summary>
        public BindedControlCollection Parent
        {
            get { return _parent; }
        }

        #endregion

    }

    /// <summary>
    ///  Collection of binded control information.
    /// </summary>
    public class BindedControlCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of BindedControlCollection.
        /// </summary>
        /// <param name="binder"></param>
        public BindedControlCollection(DataBinder binder)
        { _binder = binder; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinder _binder = null;

        /// <summary>
        /// Gets the binder associated with the current binded control information collection.
        /// </summary>
        public DataBinder Binder
        {
            get { return _binder; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Hashtable _controltable = new Hashtable();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Hashtable _fieldtable = new Hashtable();

        /// <summary>
        /// Gets the binded control information at the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BindedControl this[int index]
        {
            get { return (BindedControl)List[index]; }
        }

        /// <summary>
        /// Gets the binded control information thru the specified control object within the collection. 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public BindedControl this[object control]
        {
            get { return GetInfoByControl(control); }
        }

        /// <summary>
        /// Gets the binded control information thru the specified database field name within the collection.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public BindedControl this[string field]
        {
            get { return GetInfoByName(field); }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new binded control information into the collection.
        /// </summary>
        /// <param name="control">Binded control object</param>
        /// <param name="field">Database field name to bind into the control</param>
        /// <returns></returns>
        public BindedControl Add(object control, string field)
        {
            if (!String.IsNullOrEmpty(field.RLTrim()))
            {
                BindedControl _control = new BindedControl(this, control, field);
                if (Contains(control)) Remove(control);
                int _index = List.Add(_control);
                if (!_controltable.ContainsKey(control)) _controltable.Add(control, (BindedControl)List[_index]);
                if (!_fieldtable.ContainsKey(field)) _fieldtable.Add(field, (BindedControl)List[_index]);
                return (BindedControl)List[_index];
            }
            else return null;
        }

        /// <summary>
        /// Returns whether the specified binded control information already exists within the collection or not.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Contains(BindedControl control)
        { return List.Contains(control); }

        /// <summary>
        /// Returns whether a binded control information with the specified control object exists within the collection or not.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Contains(object control)
        { return (bool)(GetInfoByControl(control) != null); }

        /// <summary>
        /// Returns whether a binded control information with the specified database field name exists within the collection or not.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool Contains(string field)
        { return (bool)(GetInfoByName(field) != null); }

        private BindedControl GetInfoByControl(object control)
        {
            BindedControl _control = null;

            if (_controltable.ContainsKey(control)) _control = (BindedControl)_controltable[control]; 

            return _control;
        }

        private BindedControl GetInfoByName(string name)
        {
            BindedControl _control = null;

            if (_fieldtable.ContainsKey(name)) _control = (BindedControl)_fieldtable[name];

            return _control;
        }

        /// <summary>
        /// Removes the specified binded control information from the collection.
        /// </summary>
        /// <param name="control"></param>
        public void Remove(BindedControl control)
        {
            if (Contains(control))
            {
                if (_controltable.ContainsKey(control.Control)) _controltable.Remove(control.Control);
                if (_fieldtable.ContainsKey(control.FieldName)) _fieldtable.Remove(control.FieldName);
                List.Remove(control);
            }
        }

        /// <summary>
        /// Removes a binded control information with the specified control object from the collection.
        /// </summary>
        /// <param name="control"></param>
        public void Remove(object control)
        {
            BindedControl _control = GetInfoByControl(control);
            if (_control != null)
            {
                if (_controltable.ContainsKey(control)) _controltable.Remove(control);
                if (_fieldtable.ContainsKey(_control.FieldName)) _fieldtable.Remove(_control.FieldName);
                List.Remove(_control);
            }
        }

        /// <summary>
        /// Removes a binded control information with the specified database field name from the collection.
        /// </summary>
        /// <param name="field"></param>
        public void Remove(string field)
        {
            BindedControl _control = GetInfoByName(field);
            if (_control != null)
            {
                if (_fieldtable.ContainsKey(field)) _fieldtable.Remove(field);
                if (_controltable.ContainsKey(_control.Control)) _controltable.Remove(_control.Control);
                List.Remove(_control);
            }
        }

        #endregion

    }

    /// <summary>
    /// DataBinder data loading event arguments.
    /// </summary>
    public class DataBinderLoadingEventArgs : EventArgs
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBinderLoadingEventArgs.
        /// </summary>
        /// <param name="binding">Currently evealuated DataBinding object</param>
        public DataBinderLoadingEventArgs(DataBinding binding) : this(binding, "")
        { }

        /// <summary>
        /// Creates a new instance of DataBinderLoadingEventArgs.
        /// </summary>
        /// <param name="binding">Currently evealuated DataBinding object</param>
        /// <param name="error">Encountered exception's message</param>
        public DataBinderLoadingEventArgs(DataBinding binding, string error)
        { _binding = binding; _error = error; _cancel = !String.IsNullOrEmpty(error.RLTrim()); }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinding _binding = null;

        /// <summary>
        /// Gets the current RecordBinding object that is to-be / currently loaded.
        /// </summary>
        public DataBinding Binding
        {
            get { return _binding; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _cancel = false;

        /// <summary>
        /// Gets or sets whether to cancel preceeding actions after the event this argument was invoked.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _error = "";

        /// <summary>
        /// Gets the exception message the invoking event have encountered.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        #endregion

    }

    /// <summary>
    /// DataBinder data saving event arguments.
    /// </summary>
    public class DataBinderSavingEventArgs : EventArgs
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBinderSavingEventArgs.
        /// </summary>
        /// <param name="sql"></param>
        public DataBinderSavingEventArgs(string sql) : this(sql, "")
        { }

        /// <summary>
        /// Creates a new instance of DataBinderSavingEventArgs.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rowsaffected"></param>
        public DataBinderSavingEventArgs(string sql, int rowsaffected) : this(sql, "")
        { _rowsaffected = rowsaffected; _saved = true; }

        /// <summary>
        /// Creates a new instance of DataBinderSavingEventArgs.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="error"></param>
        public DataBinderSavingEventArgs(string sql, string error)
        {
            _cancel = !String.IsNullOrEmpty(error.RLTrim()); _commandtext = sql;
            _error = error; _saved = false;
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _cancel = false;

        /// <summary>
        /// Gets or sets whether to cancel preceeding events after the current argument was invoked by an event.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _commandtext = "";

        /// <summary>
        /// Gets or sets the current sql statement to be or was executed.
        /// </summary>
        public string CommandText
        {
            get { return _commandtext; }
            set { _commandtext = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _error = "";

        /// <summary>
        /// Gets the current encountered exception message.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        [DebuggerBrowsable (DebuggerBrowsableState.Never)]
        private string _errornotification = "";

        /// <summary>
        /// Gets or sets whether the notification message when error has been encountered after data saving execution.
        /// </summary>
        public string ErrorNotification
        {
            get { return _errornotification; }
            set { _errornotification = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _rowsaffected = -1;

        /// <summary>
        /// Gets the number oif database records affected by the executed updates.
        /// </summary>
        public int RowsAffected
        {
            get { return _rowsaffected; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _saved = false;

        /// <summary>
        /// Gets whether the data saving is executed properly and without errors.
        /// </summary>
        public bool Saved
        {
            get { return _saved; }
        }

        #endregion

    }

    /// <summary>
    /// DataBinder validation event arguments.
    /// </summary>
    public class DataBinderValidationEventArgs : EventArgs
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBinderValidationEventArgs.
        /// </summary>
        public DataBinderValidationEventArgs() : this(null)
        { }

        /// <summary>
        /// Creates a new instance of DataBinderValidationEventArgs.
        /// </summary>
        /// <param name="control"></param>
        public DataBinderValidationEventArgs(object control) : this(control, true)
        { }

        /// <summary>
        /// Creates a new instance of DataBinderValidationEventArgs.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="valid"></param>
        public DataBinderValidationEventArgs(object control, bool valid)
        { _control = control; _valid = valid; _cancel = (!valid); }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _cancel = false;

        /// <summary>
        /// Gets or sets whether to cancel preceeding events after this argument was invoked.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _control = null;

        /// <summary>
        /// Gets the control that is currently eveluated.
        /// </summary>
        public object Control
        {
            get { return _control; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _notification = "";

        /// <summary>
        /// Gets or sets the validation notification that should prompt the user if in case control did not pass the evaluation.
        /// </summary>
        public string Notification
        {
            get { return _notification; }
            set { _notification = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _valid = true;

        /// <summary>
        /// Gets whether the current validating control is evaluated as valid or not.
        /// </summary>
        public bool Valid
        {
            get { return _valid; }
        }

        #endregion

    }

    /// <summary>
    /// Data binding information class.
    /// </summary>
    public class DataBinding : IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        public DataBinding(DataBinder binder) : this(binder, "", "",  null)
        { }

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        /// <param name="header">Header database binding information</param>
        public DataBinding(DataBinder binder, DataBinding header) : this(binder, "", "",  null)
        { }

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        /// <param name="sql">Database command text used to deploy automated database record updates</param>
        public DataBinding(DataBinder binder, string sql) : this(binder, sql, "", null)
        { }

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        /// <param name="sql">Database command text used to deploy automated database record updates</param>
        /// <param name="header">Header database binding information</param>
        public DataBinding(DataBinder binder, string sql, DataBinding header) : this(binder, sql, "", header)
        { }

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        /// <param name="basecommandtext">Database command text used to deploy automated database record updates</param>
        /// <param name="viewcommandtext">Database command text used to provide customized data binded views specially if records are binded into a grid</param>
        public DataBinding(DataBinder binder, string basecommandtext, string viewcommandtext) : this(binder, basecommandtext, viewcommandtext, null)
        { }

        /// <summary>
        /// Creates a new instance of DataBinding.
        /// </summary>
        /// <param name="binder">Parent DataBinder component</param>
        /// <param name="basecommandtext">Database command text used to deploy automated database record updates</param>
        /// <param name="viewcommandtext">Database command text used to provide customized data binded views specially if records are binded into a grid</param>
        /// <param name="header">Header database binding information</param>
        public DataBinding(DataBinder binder, string basecommandtext, string viewcommandtext, DataBinding header)
        { 
            _binder = binder; _basecommandtext = basecommandtext; 
            _viewcommandtext = viewcommandtext; _header = header;
            _details = new DataBindingCollection(this);
            _requiredfields = new RequiredFieldCollection(this);
        }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _allownovaluesingrid = true;

        /// <summary>
        /// Gets or sets whether to allow no entries at all in the current binding information's grid. Default value is True.
        /// </summary>
        public bool AllowNoValuesInGrid
        {
            get { return _allownovaluesingrid; }
            set { _allownovaluesingrid = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _basecommandtext = "";

        /// <summary>
        /// Gets or sets the raw database command text used to deploy automated database record updates.
        /// </summary>
        public string BaseCommandText
        {
            get { return _basecommandtext; }
            set { _basecommandtext = value; }
        }

        private DataTable _basetable = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinder _binder = null;

        /// <summary>
        /// Gets the parenting DataBinder component.
        /// </summary>
        public DataBinder Binder
        {
            get { return _binder; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Hashtable _delegatetable = new Hashtable();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBindingCollection _details = null;

        /// <summary>
        /// Gets the collection of dependent detail binding information.
        /// </summary>
        public DataBindingCollection Details
        {
            get { return _details; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _foreignkey = "";

        /// <summary>
        /// Gets the foreign key field name. This determines header-detail relation linking.
        /// </summary>
        public string ForeignKey
        {
            get { return _foreignkey; }
            set { _foreignkey = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _grid = null;

        /// <summary>
        /// Gets or sets the grid object to where the retrieved data will be binded. Not setting this value will have the retrieved database fields bind into the parented DataBinder's matching binded controls.
        /// </summary>
        public object Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinding _header = null;

        /// <summary>
        /// Gets the header database binding information of the current detail binding information.
        /// </summary>
        public DataBinding Header
        {
            get { return _header; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _name = "";

        /// <summary>
        /// Gets or sets the current database binding's name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string _pkfieldname = "BasePKFieldName";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _primarykey = "";

        /// <summary>
        /// Gets or sets the primary key field name. This will determine constraints and header-detail relationship linking.
        /// </summary>
        public string PrimaryKey
        {
            get { return _primarykey; }
            set { _primarykey = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RequiredFieldCollection _requiredfields = null;

        /// <summary>
        ///  Gets the list of database required fields.
        /// </summary>
        public RequiredFieldCollection RequiredFields
        {
            get { return _requiredfields; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataTable _table = null;

        /// <summary>
        ///  Gets the generated bindable DataTable object.
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _viewcommandtext = "";

        /// <summary>
        /// Gets or sets the command text used to provide customized data binded views specially if records are binded into a grid.
        /// </summary>
        public string ViewCommandText
        {
            get { return _viewcommandtext; }
            set { _viewcommandtext = value; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Accepts all relevant changes and updates into the binded values ans have it finalized.
        /// </summary>
        public void AcceptChanges()
        {
            if (_basetable != null) _basetable.AcceptChanges();
            if (_table != null) _table.AcceptChanges();
        }

        /// <summary>
        /// Attaches binding handlers into the current bindable table.
        /// </summary>
        public void AttachTableHandlers()
        {
            if (_table != null)
            {
                _table.RowChanged += new DataRowChangeEventHandler(ViewTable_RowChanged);
                _table.RowDeleting += new DataRowChangeEventHandler(ViewTable_RowDeleting);
            }
        }

        /// <summary>
        ///  Asynchronously loads the data binding information and returns an argument relative to its data loading execution. Call the EndLoad(IAsyncResult) to get synchronization result argument after synchronization is complete.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult BeginLoad()
        {
            if (_binder == null) return null;

            DataBinderLoadingEventArgs _args = new DataBinderLoadingEventArgs(this);
            Materia.RaiseEvent(_binder, "BeforeBindingDataLoad", _args);
         
            if (!_args.Cancel)
            {
                Func<DataBinderLoadingEventArgs> _delegate = new Func<DataBinderLoadingEventArgs>(LoadBinding);
                IAsyncResult _result = _delegate.BeginInvoke(null, _delegate);
                if (!_delegatetable.ContainsKey(_result)) _delegatetable.Add(_result, _delegate);
                return _result;
            }
            else return null;
        }

        private void Bind()
        {
            if (_table != null)
            {
                if (_grid != null)
                {
                    if (Materia.PropertyExists(_grid, "DataSource"))
                    {
                        Materia.SetPropertyValue(_grid, "DataSource", null);
                        Materia.SetPropertyValue(_grid, "DataSource", _table);
                        FormatGrid();
                    }
                }
                else
                {
                    BindMaxLengths();

                    if (_table.Rows.Count > 0)
                    {
                        foreach (DataColumn _column in _table.Columns)
                        {
                            if (_binder.BindedControls.Contains(_column.ColumnName))
                            {
                                Control _control = (Control) _binder.BindedControls[_column.ColumnName].Control;
                                object _value = _table.Rows[0][_column.ColumnName];

                                if (_column.DataType.Name == typeof(string).Name ||
                                    _column.DataType.Name == typeof(String).Name)
                                {
                                    if (Materia.PropertyExists(_control, "DataSource"))
                                    {
                                        if (Materia.PropertyExists(_control, "SelectedValue"))
                                        {
                                            object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                            if (_datasource != null)
                                            {
                                                if (Materia.IsNullOrNothing(_value))
                                                {
                                                    if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                    {
                                                        try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                                        catch { }
                                                    }
                                                    else
                                                    {
                                                        try { Materia.SetPropertyValue(_control, "SelectedValue", null); }
                                                        catch { }
                                                    }
                                                }
                                                else Materia.SetPropertyValue(_control, "SelectedValue", _value);
                                            }
                                            else
                                            {
                                                if (Materia.IsNullOrNothing(_value)) _value = "";
                                                if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _value);
                                            }
                                        }
                                        else
                                        {
                                            if (Materia.IsNullOrNothing(_value)) _value = "";
                                            if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _value);
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.IsNullOrNothing(_value)) _value = "";
                                        if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _value);
                                    }
                                }
                                else if (_column.DataType.Name == typeof(DateTime).Name)
                                {
                                    if (Materia.PropertyExists(_control, "DataSource"))
                                    {
                                        object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                        if (_datasource != null)
                                        {
                                            if (Materia.IsNullOrNothing(_value))
                                            {
                                                if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                {
                                                    try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                                    catch { }
                                                }
                                                else
                                                {
                                                    try { Materia.SetPropertyValue(_control, "SelectedValue", null); }
                                                    catch { }
                                                }
                                            }
                                            else Materia.SetPropertyValue(_control, "SelectedValue", _value);
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Value"))
                                            {
                                                if (VisualBasic.IsDate(_value)) Materia.SetPropertyValue(_control, "Value", _value);
                                            }
                                            else
                                            {
                                                string _datevalue = "";
                                                if (VisualBasic.IsDate(_value)) _datevalue = VisualBasic.Format(VisualBasic.CDate(_value), "dd-MMM-yyyy");
                                                if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _datevalue);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Value"))
                                        {
                                            if (VisualBasic.IsDate(_value)) Materia.SetPropertyValue(_control, "Value", _value);
                                        }
                                        else
                                        {
                                            string _datevalue = "";
                                            if (VisualBasic.IsDate(_value)) _datevalue = VisualBasic.Format(VisualBasic.CDate(_value), "dd-MMM-yyyy");
                                            if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _datevalue);
                                        }
                                    }
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
                                    if (Materia.PropertyExists(_control, "DataSource"))
                                    {
                                        object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                        if (_datasource != null)
                                        {
                                            if (Materia.IsNullOrNothing(_value))
                                            {
                                                if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                {
                                                    try { Materia.SetPropertyValue(_control, "SelectedIndex", -1); }
                                                    catch { }
                                                }
                                                else
                                                {
                                                    try { Materia.SetPropertyValue(_control, "SelectedValue", null); }
                                                    catch { }
                                                }
                                            }
                                            else Materia.SetPropertyValue(_control, "SelectedValue", _value);
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Value"))
                                            {
                                                Materia.SetPropertyValue(_control, "Value", _value);
                                                object _currentvalue = Materia.GetPropertyValue(_control, "Value");
                                                if (!_value.Equals(_currentvalue))
                                                {
                                                    if (Materia.PropertyExists(_control, "Text"))
                                                    {
                                                        string _text = "";
                                                        if (VisualBasic.IsNumeric(_value)) _text = _value.ToString();
                                                        Materia.SetPropertyValue(_control, "Text", _text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Checked"))
                                                {
                                                    bool _checked = true;
                                                    if (!VisualBasic.IsNumeric(_value)) _checked = false;
                                                    Materia.SetPropertyValue(_control, "Checked", _checked);
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Text"))
                                                    {
                                                        string _text = "";
                                                        if (VisualBasic.IsNumeric(_value)) _text = _value.ToString();
                                                        Materia.SetPropertyValue(_control, "Text", _text);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Materia.PropertyExists(_control, "Value"))
                                        {
                                            Materia.SetPropertyValue(_control, "Value", _value);
                                            object _currentvalue = Materia.GetPropertyValue(_control, "Value");
                                            if (!_value.Equals(_currentvalue))
                                            {
                                                if (Materia.PropertyExists(_control, "Text"))
                                                {
                                                    string _text = "";
                                                    if (VisualBasic.IsNumeric(_value)) _text = _value.ToString();
                                                    Materia.SetPropertyValue(_control, "Text", _text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Materia.PropertyExists(_control, "Checked"))
                                            {
                                                bool _checked = true;
                                                if (!VisualBasic.IsNumeric(_value)) _checked = false;
                                                else _checked = VisualBasic.CBool(_value);

                                                Materia.SetPropertyValue(_control, "Checked", _checked);
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Text"))
                                                {
                                                    string _text = "";
                                                    if (VisualBasic.IsNumeric(_value)) _text = _value.ToString();
                                                    Materia.SetPropertyValue(_control, "Text", _text);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (_column.DataType.Name == typeof(bool).Name ||
                                         _column.DataType.Name == typeof(Boolean).Name)
                                {
                                    if (!Materia.IsNullOrNothing(_value))
                                    {
                                        if (Materia.PropertyExists(_control, "Checked")) Materia.SetPropertyValue(_control, "Checked", VisualBasic.CBool(_value));
                                    }
                                }
                                else 
                                {
                                    if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                                        _column.DataType.Name.ToLower().Contains("byte()") ||
                                        _column.DataType.Name.ToLower().Contains("bytes[]") ||
                                        _column.DataType.Name.ToLower().Contains("bytes()"))
                                    {
                                        if (Materia.PropertyExists(_control, "Image"))
                                        {
                                            if (!Materia.IsNullOrNothing(_value))
                                            {
                                                try
                                                {
                                                    Image _image = ((byte[])_value).ToImage();
                                                    Materia.SetPropertyValue(_control, "Image", _image);
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BindMaxLengths()
        {
            if (_table != null)
            {
                if (_binder != null)
                {
                    foreach (DataColumn _column in _table.Columns)
                    {
                        if (_column.DataType != null)
                        {
                            if (_column.DataType.Name == typeof(string).Name)
                            {
                                if (_column.MaxLength > 0)
                                {
                                    if (_binder.BindedControls.Contains(_column.ColumnName))
                                    {
                                        try
                                        { Materia.SetPropertyValue(_binder.BindedControls[_column.ColumnName].Control, "MaxLength", _column.MaxLength); }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DisposeTables()
        {
            if (_basetable != null)
            {
                try { _basetable.Dispose(); }
                catch { }
                _basetable = null;
            }

            if (_table != null)
            {
                try { _table.Dispose(); }
                catch { }
                _table = null;
            }
        }

        /// <summary>
        /// Finalizes data loading and returns the result argument based on the specified synchronization called by a BeginLoad function.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public DataBinderLoadingEventArgs EndLoad(IAsyncResult result)
        {
            DataBinderLoadingEventArgs _args = null;

            if (_delegatetable.ContainsKey(result))
            {
                Func<DataBinderLoadingEventArgs> _delegate = (Func<DataBinderLoadingEventArgs>)_delegatetable[result];
                _args = _delegate.EndInvoke(result);  _delegatetable.Remove(result);

                if (!_args.Cancel)
                {
                    Control.CheckForIllegalCrossThreadCalls = false;
                    Action _binddelegate = new Action(Bind);
                    IAsyncResult _result = _binddelegate.BeginInvoke(null, _binddelegate);

                    while (!_result.IsCompleted &&
                           !_binder.CancelRunningProcess)
                    {
                        Materia.RaiseEvent(_binder, "DataLoading", new EventArgs());
                        Thread.Sleep(1); Application.DoEvents();
                    }

                    if (_binder.CancelRunningProcess)
                    {
                        if (_result.IsCompleted)
                        {
                            try { _result = null; }
                            catch { }
                        }

                        return null;
                    }

                    _binddelegate.EndInvoke(_result);
                    Materia.RaiseEvent(_binder, "AfterBindingDataLoad", _args);
                }
                else return null;
            }

            return _args;
        }

        private void FormatGrid()
        {
            if (_grid != null)
            {
                if (Materia.PropertyExists(_grid, "Cols"))
                {
                    object _cols = Materia.GetPropertyValue(_grid, "Cols");
                    if (_cols != null)
                    {
                        if (Materia.PropertyExists(_cols, "Count"))
                        {
                            object _colcounts = Materia.GetPropertyValue(_cols, "Count");
                            if (VisualBasic.IsNumeric(_colcounts))
                            {
                                int _count = VisualBasic.CInt(_colcounts);
                                if (_count > 0)
                                {
                                    for (int i = 0; i <= (_count - 1); i++)
                                    {
                                        object _col = Materia.GetPropertyValue(_cols, "Item", new object[] { i });
                                        if (_col != null)
                                        {
                                            if (Materia.PropertyExists(_col, "DataType") &&
                                                Materia.PropertyExists(_col, "Format"))
                                            {
                                                Type _type = (Type) Materia.GetPropertyValue(_col, "DataType");

                                                if (_type != null)
                                                {
                                                    if (_type.Name == typeof(DateTime).Name) Materia.SetPropertyValue(_col, "Format", "dd-MMM-yyyy");
                                                    else if (_type.Name == typeof(decimal).Name ||
                                                         _type.Name == typeof(Decimal).Name ||
                                                         _type.Name == typeof(double).Name ||
                                                         _type.Name == typeof(Double).Name ||
                                                         _type.Name == typeof(float).Name ||
                                                         _type.Name == typeof(Single).Name) Materia.SetPropertyValue(_col, "Format", "N2");
                                                    else { }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current base table of the current data binding information.
        /// </summary>
        /// <returns></returns>
        public DataTable GetBaseTable()
        { return _basetable; }

        /// <summary>
        /// Returns the sql statement associated with the updates applied into the current DataBinding.
        /// </summary>
        /// <returns></returns>
        public string GetSqlStatement()
        {
            QueryGenerator _sqlgenerator = new QueryGenerator(_basetable);
            StringBuilder _sql = new StringBuilder();
            _sqlgenerator.ExcludedFields.Add(_pkfieldname);

            if (_basetable != null)
            {
                if (_basetable.Columns.Contains(_pkfieldname))
                {
                    if (_basetable.Rows.Count > 0) _sqlgenerator.PrimaryKey.Value = "{" + _basetable.Columns[_pkfieldname].Ordinal + "}";
                }
            }

            if (_header == null) _sql.Append(_sqlgenerator.ToString());
            else
            {
                string _pk = _header.PrimaryKey;

                if (_header.Table != null)
                {
                    if (String.IsNullOrEmpty(_pk.RLTrim()))
                    {
                        foreach (DataColumn _column in _header.Table.Columns)
                        {
                            if (_column.Unique)
                            {
                                _pk = _column.ColumnName; break;
                            }
                        }
                    }
                    else
                    {
                        if (!_header.Table.Columns.Contains(_pk))
                        {
                            foreach (DataColumn _column in _header.Table.Columns)
                            {
                                if (_column.Unique)
                                {
                                    _pk = _column.ColumnName; break;
                                }
                            }
                        }
                    }
                }

                _sqlgenerator.ForeignKey.HeaderPrimaryKey = _pk;
                _sqlgenerator.ForeignKey.HeaderTable = _header.Table;


                if (!String.IsNullOrEmpty(_pk.RLTrim()))
                {
                    string _fk = ForeignKey; string _fkvalue = "";

                    if (String.IsNullOrEmpty(_fk.RLTrim()))
                    {
                        if (_basetable.Columns.Contains(_pk)) _fk = _pk;
                    }
                    else
                    {
                        if (!_basetable.Columns.Contains(_fk))
                        {
                            if (_basetable.Columns.Contains(_pk)) _fk = _pk;
                            else _fk = "";
                        }
                    }

                    if (!String.IsNullOrEmpty(_fk.RLTrim()))
                    {
                        DataTable _htable = _header.GetBaseTable();
                        if (_htable != null)
                        {
                            if (_htable.Columns.Contains(_pk))
                            {
                                if (_htable.Rows.Count > 0)
                                {
                                    if (_htable.Columns[_pk].AutoIncrement)
                                    {
                                        DataRow rw = _htable.Rows[0];
                                        if (rw.RowState == DataRowState.Added) _fkvalue = Database.LastInsertIdCall;
                                        else
                                        {
                                            _fkvalue = "NULL";

                                            if (!Materia.IsNullOrNothing(rw[_pk]))
                                            {
                                                DataColumn _fkcolumn = _htable.Columns[_pk];

                                                if (_fkcolumn.DataType.Name == typeof(string).Name ||
                                                    _fkcolumn.DataType.Name == typeof(String).Name) _fkvalue = "'" + rw[_pk].ToString().ToSqlValidString() + "'";
                                                else if (_fkcolumn.DataType.Name == typeof(DateTime).Name)
                                                {
                                                    if (VisualBasic.IsDate(rw[_pk])) _fkvalue = "'" + VisualBasic.CDate(rw[_pk]).ToSqlValidString(true) + "'";
                                                }
                                                else if (_fkcolumn.DataType.Name == typeof(byte).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Byte).Name ||
                                                         _fkcolumn.DataType.Name == typeof(decimal).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Decimal).Name ||
                                                         _fkcolumn.DataType.Name == typeof(double).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Double).Name ||
                                                         _fkcolumn.DataType.Name == typeof(float).Name ||
                                                         _fkcolumn.DataType.Name == typeof(int).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Int16).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Int32).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Int64).Name ||
                                                         _fkcolumn.DataType.Name == typeof(long).Name ||
                                                         _fkcolumn.DataType.Name == typeof(sbyte).Name ||
                                                         _fkcolumn.DataType.Name == typeof(SByte).Name ||
                                                         _fkcolumn.DataType.Name == typeof(short).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Single).Name)
                                                {
                                                    if (VisualBasic.IsNumeric(rw[_pk])) _fkvalue = rw[_pk].ToString();
                                                }
                                                else if (_fkcolumn.DataType.Name == typeof(bool).Name ||
                                                         _fkcolumn.DataType.Name == typeof(Boolean).Name)
                                                {
                                                    if (VisualBasic.CBool(rw[_pk])) _fkvalue = "1";
                                                    else _fkvalue = "0";
                                                }
                                                else
                                                { }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DataRow rw = _htable.Rows[0]; _fkvalue = "NULL";

                                        if (!Materia.IsNullOrNothing(rw[_pk]))
                                        {
                                            DataColumn _fkcolumn = _htable.Columns[_pk];

                                            if (_fkcolumn.DataType.Name == typeof(string).Name ||
                                                _fkcolumn.DataType.Name == typeof(String).Name) _fkvalue = "'" + rw[_pk].ToString().ToSqlValidString() + "'";
                                            else if (_fkcolumn.DataType.Name == typeof(DateTime).Name)
                                            {
                                                if (VisualBasic.IsDate(rw[_pk])) _fkvalue = "'" + VisualBasic.CDate(rw[_pk]).ToSqlValidString(true) + "'";
                                            }
                                            else if (_fkcolumn.DataType.Name == typeof(byte).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Byte).Name ||
                                                     _fkcolumn.DataType.Name == typeof(decimal).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Decimal).Name ||
                                                     _fkcolumn.DataType.Name == typeof(double).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Double).Name ||
                                                     _fkcolumn.DataType.Name == typeof(float).Name ||
                                                     _fkcolumn.DataType.Name == typeof(int).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Int16).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Int32).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Int64).Name ||
                                                     _fkcolumn.DataType.Name == typeof(long).Name ||
                                                     _fkcolumn.DataType.Name == typeof(sbyte).Name ||
                                                     _fkcolumn.DataType.Name == typeof(SByte).Name ||
                                                     _fkcolumn.DataType.Name == typeof(short).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Single).Name)
                                            {
                                                if (VisualBasic.IsNumeric(rw[_pk])) _fkvalue = rw[_pk].ToString();
                                            }
                                            else if (_fkcolumn.DataType.Name == typeof(bool).Name ||
                                                     _fkcolumn.DataType.Name == typeof(Boolean).Name)
                                            {
                                                if (VisualBasic.CBool(rw[_pk])) _fkvalue = "1";
                                                else _fkvalue = "0";
                                            }
                                            else
                                            { }
                                        }
                                    }
                                }
                                else
                                {
                                    if (_htable.Columns[_pk].AutoIncrement) _fkvalue = Database.LastInsertIdCall; 
                                }
                            }
                        }

                        _sqlgenerator.ForeignKey.Field = _fk; _sqlgenerator.ForeignKey.Value = _fkvalue;
                    }
                }

                _sql.Append(_sqlgenerator.ToString());
            }

            return _sql.ToString();
        }

        /// <summary>
        /// Returns the sql command statements generated from the current binding table's changes.
        /// </summary>
        /// <returns></returns>
        public string GetUpdateStatements()
        {
            StringBuilder _sql = new StringBuilder();
            _sql.Append(GetSqlStatement());

            if (Details.Count > 0)
            {
                foreach (DataBinding db in Details)
                {
                    string _query = db.GetSqlStatement();
                    if (!String.IsNullOrEmpty(_query.RLTrim()))
                    {
                        string _cursql = _sql.ToString();
                        if (!String.IsNullOrEmpty(_cursql.RLTrim())) _sql.Append("\n");
                        _sql.Append(_query);
                    }
                }
            }

            return _sql.ToString();
        }

        private DataTable InitializedTable(DataTable table)
        { return InitializedTable(table, false); }

        private DataTable InitializedTable(DataTable table, bool createcolkey)
        {
            if (table != null)
            {
                DataTable dt = null; string _uniquekey = ""; string _pk = "";

                foreach (DataColumn _column in table.Columns)
                {
                    if (_column.Unique)
                    {
                        _uniquekey = _column.ColumnName; break;
                    }
                }

                if (!String.IsNullOrEmpty(_uniquekey.RLTrim()))
                {
                    if (!String.IsNullOrEmpty(_primarykey.RLTrim()))
                    {
                        if (table.Columns.Contains(_primarykey)) _pk = _primarykey;
                        else _pk = _uniquekey;
                    }
                    else _pk = _uniquekey;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_primarykey.RLTrim()))
                    {
                        if (table.Columns.Contains(_primarykey)) _pk = _primarykey;
                        else _pk = table.Columns[0].ColumnName;
                    }
                    else _pk = table.Columns[0].ColumnName;
                }

                table.Constraints.Clear(); bool _withuint = false;

                foreach (DataColumn _column in table.Columns)
                {
                    _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(uint).Name);
                    _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt16).Name);
                    _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt32).Name);
                    _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt64).Name);
                    _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(ulong).Name);
                    if (_withuint) break;
                }

                DataTable _temptable = null;

                if (_withuint)
                {
                    _temptable = table.Clone(); _temptable.TableName = table.TableName;

                    foreach (DataColumn _column in _temptable.Columns)
                    {
                        _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(uint).Name);
                        _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt16).Name);
                        _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt32).Name);
                        _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(UInt64).Name);
                        _withuint = _withuint || VisualBasic.CBool(_column.DataType.Name.ToLower() == typeof(ulong).Name);
                        if (_withuint)_temptable.Columns[_column.ColumnName].DataType = typeof(long);
                    }

                    foreach (DataRow rw in table.Rows)
                    {
                        if (rw.RowState != DataRowState.Deleted &&
                            rw.RowState != DataRowState.Detached) _temptable.Rows.Add(rw.ItemArray);
                    }

                    _temptable.AcceptChanges();
                }
                else _temptable = table.Replicate();

                if (createcolkey &&
                    !String.IsNullOrEmpty(_pk.RLTrim()))
                {
                    dt = new DataTable(); dt.TableName = _temptable.TableName;
                    dt.Columns.Add(_pkfieldname, _temptable.Columns[_pk].DataType);

                    foreach (DataColumn _column in _temptable.Columns) dt.Columns.Add(_column.ColumnName, _column.DataType);

                    foreach (DataRow rw in _temptable.Rows)
                    {
                        if (rw.RowState != DataRowState.Deleted &&
                            rw.RowState != DataRowState.Detached)
                        {
                            object[] _values = new object[dt.Columns.Count];
                            _values[0] = rw[_pk];

                            foreach (DataColumn _column in _temptable.Columns) _values[_column.Ordinal + 1] = rw[_column.ColumnName];
                            dt.Rows.Add(_values);
                        }
                    }
                }
                else dt = _temptable.Replicate();

                if (dt != null &&
                    !String.IsNullOrEmpty(_pk.RLTrim()))
                {
                    DataColumn _col = dt.Columns[_pk];

                    dt.Constraints.Add("PK", _col, true);

                    if (_col.DataType.Name == typeof(int).Name ||
                        _col.DataType.Name == typeof(Int16).Name ||
                        _col.DataType.Name == typeof(Int32).Name ||
                        _col.DataType.Name == typeof(Int64).Name ||
                        _col.DataType.Name == typeof(long).Name ||
                        _col.DataType.Name == typeof(byte).Name ||
                        _col.DataType.Name == typeof(Byte).Name ||
                        _col.DataType.Name == typeof(sbyte).Name ||
                        _col.DataType.Name == typeof(SByte).Name)
                    {
                        if (!_col.AutoIncrement)
                        {
                            if (_col.DefaultValue != null)
                            {
                                try { _col.DefaultValue = null; }
                                catch { }
                            }
                            _col.AutoIncrement = true;
                        }

                        _col.AutoIncrementStep = 1;
                        if (dt.Rows.Count > 0)
                        {
                            object _max = dt.Compute("MAX([" + _pk + "])", "");
                            if (VisualBasic.IsNumeric(_max)) _col.AutoIncrementSeed = VisualBasic.CLng(_max);
                            else _col.AutoIncrementSeed = 1;
                        }
                        else _col.AutoIncrementSeed = 1;
                    }

                    foreach (DataColumn _column in dt.Columns)
                    {
                        if (!_column.AutoIncrement &&
                            !_column.Unique)
                        {
                            if (_column.DataType.Name == typeof(string).Name ||
                                _column.DataType.Name == typeof(String).Name) _column.DefaultValue = "";
                            else if (_column.DataType.Name == typeof(DateTime).Name) _column.DefaultValue = DateTime.Now;
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
                                     _column.DataType.Name == typeof(Single).Name) _column.DefaultValue = 0;
                            else if (_column.DataType.Name == typeof(bool).Name ||
                                     _column.DataType.Name == typeof(Boolean).Name) _column.DefaultValue = false;
                            else { }
                        }
                    }
                }

                if (dt != null) dt.AcceptChanges();
                return dt;
            }
            else return null;
        }

        /// <summary>
        /// Loads the data binding information and returns an argument relative to its data loading execution.
        /// </summary>
        /// <returns></returns>
        public DataBinderLoadingEventArgs Load()
        {
            if (_binder == null) return null;
            else
            {
                DataBinderLoadingEventArgs _args = new DataBinderLoadingEventArgs(this);
                Materia.RaiseEvent(_binder, "BeforeBindingDataLoad", _args);

                if (!_args.Cancel)
                {
                    DataBinderLoadingEventArgs _newargs = LoadBinding();
                    if (!_args.Cancel)
                    {
                        Bind();  Materia.RaiseEvent(_binder, "AfterBindingDataLoad", _args);
                    }

                    return _newargs;
                }
                else return _args;
            }
        }

        private DataBinderLoadingEventArgs LoadBinding()
        {
            DisposeTables(); Materia.RefreshAndManageCurrentProcess();

            string _query = _basecommandtext + (_basecommandtext.RLTrim().EndsWith(";") ? "" : ";");
            if (!String.IsNullOrEmpty(_viewcommandtext.RLTrim())) _query += _viewcommandtext + (_viewcommandtext.RLTrim().EndsWith(";") ? "" : ";");

            string _error = "";

            if (!String.IsNullOrEmpty(_query.RLTrim()))
            {
                QueResult _result = Que.Execute(_binder.Connection, _query, CommandExecution.ExecuteReader);
                _error = _result.Error;

                if (String.IsNullOrEmpty(_error.RLTrim()))
                {
                    if (_result.ResultSet != null)
                    {
                        if (_result.ResultSet.Tables.Count > 0)
                        {
                            _basetable = _result.ResultSet.Tables[0].Replicate();

                            if (!String.IsNullOrEmpty(_basetable.TableName.RLTrim()) &&
                                String.IsNullOrEmpty(_name.RLTrim())) _name = _basetable.TableName;

                            if (_result.ResultSet.Tables.Count >= 2) _table = _result.ResultSet.Tables[1].Replicate();
                            else _table = _basetable.Replicate();

                            _basetable = InitializedTable(_basetable, true);
                            _table = InitializedTable(_table); AttachTableHandlers();
                        }
                        else _error = "No resultset has been returned by the executed command.";
                    }
                    else _error = "No resultset has been returned by the executed command.";
                }
            }
            else _error = "No database command statement is specified.";

            return new DataBinderLoadingEventArgs(this, _error);
        }

        /// <summary>
        /// Removes binding handler from the current bindable table.
        /// </summary>
        public void RemoveTableHandlers()
        {
            if (_table != null)
            {
                _table.RowChanged -= new DataRowChangeEventHandler(ViewTable_RowChanged);
                _table.RowDeleting -= new DataRowChangeEventHandler(ViewTable_RowDeleting);
            }
        }

        /// <summary>
        /// Sets all binded field values from each of the binded controls.
        /// </summary>
        public void SetFieldValues()
        {
            if (_grid == null)
            {
                if (_binder != null)
                {
                    if (_table != null)
                    {
                        DataRow rw = null;

                        if (_table.Rows.Count > 0)
                        {
                            foreach (DataRow _row in _table.Rows)
                            {
                                if (_row.RowState != DataRowState.Deleted &&
                                    _row.RowState != DataRowState.Detached)
                                {
                                    rw = _row; break;
                                }
                            }
                        }

                        object[] _values = new object[_table.Columns.Count];

                        foreach (DataColumn _column in _table.Columns)
                        {
                            if (!_column.AutoIncrement)
                            {
                                if (rw != null) _values[_column.Ordinal] = rw[_column.ColumnName];
                                if (_binder.BindedControls.Contains(_column.ColumnName))
                                {
                                    object _control = _binder.BindedControls[_column.ColumnName].Control;

                                    if (_control != null)
                                    {
                                        object _value = Materia.GetDefaultValueByType(_column.DataType);

                                        if (_column.DataType.Name == typeof(string).Name ||
                                            _column.DataType.Name == typeof(String).Name)
                                        {
                                            if (Materia.PropertyExists(_control, "DataSource"))
                                            {
                                                object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                                if (_datasource != null)
                                                {
                                                    if (Materia.PropertyExists(_control, "SelectedValue")) _value = Materia.GetPropertyValue(_control, "SelectedValue");
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Text")) _value = Materia.GetPropertyValue(_control, "Text");
                                                    }
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Text")) _value = Materia.GetPropertyValue(_control, "Text");
                                                }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Text")) _value = Materia.GetPropertyValue(_control, "Text");
                                            }
                                        }
                                        else if (_column.DataType.Name == typeof(DateTime).Name)
                                        {
                                            if (Materia.PropertyExists(_control, "DataSource"))
                                            {
                                                object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                                if (_datasource != null)
                                                {
                                                    if (Materia.PropertyExists(_control, "SelectedValue"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "SelectedValue");
                                                        if (!VisualBasic.IsDate(_value) &&
                                                            !Materia.IsNullOrNothing(_value)) _value = null;
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Value"))
                                                        {
                                                            _value = Materia.GetPropertyValue(_control, "Value");
                                                            if (!VisualBasic.IsDate(_value) &&
                                                                !Materia.IsNullOrNothing(_value)) _value = null;
                                                        }
                                                        else
                                                        {
                                                            if (Materia.PropertyExists(_control, "Text"))
                                                            {
                                                                _value = Materia.GetPropertyValue(_control, "Text");
                                                                if (!VisualBasic.IsDate(_value) &&
                                                                    !Materia.IsNullOrNothing(_value)) _value = null;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Value"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "Value");
                                                        if (!VisualBasic.IsDate(_value) &&
                                                            !Materia.IsNullOrNothing(_value)) _value = null;
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Text"))
                                                        {
                                                            _value = Materia.GetPropertyValue(_control, "Text");
                                                            if (!VisualBasic.IsDate(_value) &&
                                                                !Materia.IsNullOrNothing(_value)) _value = null;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Value"))
                                                {
                                                    _value = Materia.GetPropertyValue(_control, "Value");
                                                    if (!VisualBasic.IsDate(_value) &&
                                                        !Materia.IsNullOrNothing(_value)) _value = null;
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Text"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "Text");
                                                        if (!VisualBasic.IsDate(_value) &&
                                                            !Materia.IsNullOrNothing(_value)) _value = null;
                                                    }
                                                }
                                            }
                                        }
                                        else if (_column.DataType.Name == typeof(bool).Name ||
                                                 _column.DataType.Name == typeof(Boolean).Name ||
                                                 _column.DataType.Name == typeof(byte).Name ||
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
                                                 _column.DataType.Name == typeof(sbyte).Name ||
                                                 _column.DataType.Name == typeof(short).Name ||
                                                 _column.DataType.Name == typeof(Single).Name)
                                        {
                                            if (Materia.PropertyExists(_control, "DataSource"))
                                            {
                                                object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                                if (_datasource != null)
                                                {
                                                    if (Materia.PropertyExists(_control, "SelectedValue"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "SelectedValue");
                                                        if (!VisualBasic.IsNumeric(_value) &&
                                                            !Materia.IsNullOrNothing(_value)) _value = 0;
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Checked"))
                                                        {
                                                            _value = Materia.GetPropertyValue(_control, "Checked");
                                                            if (Materia.IsNullOrNothing(_value)) _value = 0;
                                                            else
                                                            {
                                                                if (VisualBasic.CBool(_value)) _value = 1;
                                                                else _value = 0;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                            {
                                                                _value = Materia.GetPropertyValue(_control, "SelectedIndex");
                                                                if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                            }
                                                            else
                                                            {
                                                                if (Materia.PropertyExists(_control, "Text"))
                                                                {
                                                                    _value = Materia.GetPropertyValue(_control, "Text");
                                                                    if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "Checked"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "Checked");
                                                        if (Materia.IsNullOrNothing(_value)) _value = 0;
                                                        else
                                                        {
                                                            if (VisualBasic.CBool(_value)) _value = 1;
                                                            else _value = 0;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                        {
                                                            _value = Materia.GetPropertyValue(_control, "SelectedIndex");
                                                            if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                        }
                                                        else
                                                        {
                                                            if (Materia.PropertyExists(_control, "Text"))
                                                            {
                                                                _value = Materia.GetPropertyValue(_control, "Text");
                                                                if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Materia.PropertyExists(_control, "Checked"))
                                                {
                                                    _value = Materia.GetPropertyValue(_control, "Checked");
                                                    if (Materia.IsNullOrNothing(_value)) _value = 0;
                                                    else
                                                    {
                                                        if (VisualBasic.CBool(_value)) _value = 1;
                                                        else _value = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                    {
                                                        _value = Materia.GetPropertyValue(_control, "SelectedIndex");
                                                        if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Text"))
                                                        {
                                                            _value = Materia.GetPropertyValue(_control, "Text");
                                                            if (!VisualBasic.IsNumeric(_value)) _value = 0;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_column.DataType.Name.ToLower().Contains("byte[]") ||
                                                _column.DataType.Name.ToLower().Contains("byte()") ||
                                                _column.DataType.Name.ToLower().Contains("bytes[]") ||
                                                _column.DataType.Name.ToLower().Contains("bytes()"))
                                            {
                                                if (Materia.PropertyExists(_control, "Image"))
                                                {
                                                    object _image = Materia.GetPropertyValue(_control, "Image");
                                                    if (!Materia.IsNullOrNothing(_image))
                                                    {
                                                        try { _value = ((Image)_image).ToByteArray(); }
                                                        catch { _value = null; }
                                                    }
                                                    else _value = null;
                                                }
                                            }
                                        }

                                        if (Materia.IsNullOrNothing(_value)) _value = Materia.GetDefaultValueByType(_column.DataType);
                                        _values[_column.Ordinal] = _value;
                                    }
                                }

                                if (rw != null)
                                {
                                    try { rw[_column.ColumnName] = _values[_column.Ordinal]; }
                                    catch { }
                                }
                            }
                        }

                        if (rw == null)
                        {
                            try
                            {
                                _table.Rows.Add(_values); _table.AcceptChanges();
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the reference fields of the current binding information.
        /// </summary>
        public void Update()
        {
            if (_basetable != null &&
                _binder != null &&
                _table != null)
            {
                if (_header == null)
                {
                    if (_basetable.Columns.Contains(_pkfieldname))
                    {
                        DataRow rw = null;

                        foreach (DataRow _row in _basetable.Rows)
                        {
                            if (_row.RowState != DataRowState.Deleted &&
                                _row.RowState != DataRowState.Detached)
                            {
                                rw = _row; break;
                            }
                        }

                        if (rw != null)
                        {
                            string _pk = "";

                            foreach (DataColumn _column in _basetable.Columns)
                            {
                                if (_column.Unique)
                                {
                                    _pk = _column.ColumnName; break;
                                }
                            }

                            if (!String.IsNullOrEmpty(_pk.RLTrim()))
                            {
                                if (_basetable.Columns[_pk].AutoIncrement)
                                {
                                    string _filter = "";

                                    foreach (DataColumn _column in _basetable.Columns)
                                    {
                                        if (_column.ColumnName != _pkfieldname &&
                                            _column.ColumnName != _pk)
                                        {
                                            string _value = "NULL";

                                            if (!Materia.IsNullOrNothing(rw[_column.ColumnName]))
                                            {
                                                if (_column.DataType.Name == typeof(string).Name ||
                                                    _column.DataType.Name == typeof(String).Name) _value = "'" + rw[_column.ColumnName].ToString().ToSqlValidString() + "'";
                                                else if (_column.DataType.Name == typeof(DateTime).Name)
                                                {
                                                    if (VisualBasic.IsDate(rw[_column.ColumnName])) _value = "'" + VisualBasic.CDate(rw[_column.ColumnName]).ToSqlValidString(true) + "'";
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
                                                    if (VisualBasic.IsNumeric(rw[_column.ColumnName])) _value = rw[_column.ColumnName].ToString();
                                                }
                                                else if (_column.DataType.Name == typeof(bool).Name ||
                                                         _column.DataType.Name == typeof(Boolean).Name)
                                                {
                                                    if (VisualBasic.CBool(rw[_column.ColumnName])) _value = "1";
                                                    else _value = "0";
                                                }
                                                else { }
                                            }

                                            if (_column.DataType.Name == typeof(DateTime).Name)
                                            {
                                                if (!Materia.IsNullOrNothing(rw[_column.ColumnName])) _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + " OR `" + _column.ColumnName + "` = '" + VisualBasic.CDate(rw[_column.ColumnName]).ToSqlValidString() + "')";
                                                else _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + ")";
                                            }
                                            else _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + ")";
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(_filter.RLTrim()))
                                    {
                                        string _query = "SELECT `" + _pk + "`\n" +
                                                        "FROM `" + _basetable.TableName + "`\n" +
                                                        "WHERE\n" +
                                                        _filter;

                                        object _pkupdated = Que.GetValue<object>(_binder.Connection, _query);
                                        if (VisualBasic.IsNumeric(_pkupdated))
                                        {
                                            if (_table.Columns.Contains(_pk))
                                            {
                                                DataRow vrw = null;

                                                foreach (DataRow _row in _table.Rows)
                                                {
                                                    if (_row.RowState != DataRowState.Deleted &&
                                                        _row.RowState != DataRowState.Detached)
                                                    {
                                                        vrw = _row; break;
                                                    }
                                                }

                                                if (vrw != null) vrw[_pk] = _pkupdated;
                                            }

                                            rw[_pk] = _pkupdated; rw[_pkfieldname] = _pkupdated;

                                            if (_binder.BindedControls.Contains(_pk))
                                            {
                                                object _control = _binder.BindedControls[_pk].Control;

                                                if (_control != null)
                                                {
                                                    object _pkint = 0;

                                                    try { _pkint = VisualBasic.CInt(_pkupdated); }
                                                    catch { _pkint = _pkupdated; }

                                                    if (Materia.PropertyExists(_control, "DataSource"))
                                                    {
                                                        object _datasource = Materia.GetPropertyValue(_control, "DataSource");
                                                        if (_datasource != null)
                                                        {
                                                            if (Materia.PropertyExists(_control, "SelectedValue"))
                                                            {
                                                                try { Materia.SetPropertyValue(_control, "SelectedValue", _pkint); }
                                                                catch { }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.PropertyExists(_control, "Value"))
                                                                {
                                                                    try { Materia.SetPropertyValue(_control, "Value", _pkint); }
                                                                    catch { }
                                                                }
                                                                else
                                                                {
                                                                    if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                                    {
                                                                        try { Materia.SetPropertyValue(_control, "SelectedIndex", _pkint); }
                                                                        catch { }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _pkint.ToString());
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Materia.PropertyExists(_control, "Value"))
                                                            {
                                                                try { Materia.SetPropertyValue(_control, "Value", _pkint); }
                                                                catch { }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                                {
                                                                    try { Materia.SetPropertyValue(_control, "SelectedIndex", _pkint); }
                                                                    catch { }
                                                                }
                                                                else
                                                                {
                                                                    if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _pkint.ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Materia.PropertyExists(_control, "Value"))
                                                        {
                                                            try { Materia.SetPropertyValue(_control, "Value", _pkint); }
                                                            catch { }
                                                        }
                                                        else
                                                        {
                                                            if (Materia.PropertyExists(_control, "SelectedIndex"))
                                                            {
                                                                try { Materia.SetPropertyValue(_control, "SelectedIndex", _pkint); }
                                                                catch { }
                                                            }
                                                            else
                                                            {
                                                                if (Materia.PropertyExists(_control, "Text")) Materia.SetPropertyValue(_control, "Text", _pkint.ToString());
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else rw[_pkfieldname] = rw[_pk];
                            }
                        }
                    }
                }
                else
                {
                    DataTable _htable = _header.Table;
                    if (_htable != null)
                    {
                        string _hpk = "";

                        foreach (DataColumn _column in _htable.Columns)
                        {
                            if (_column.Unique)
                            {
                                _hpk = _column.ColumnName; break;
                            }
                        }

                        if (!String.IsNullOrEmpty(_hpk.RLTrim()))
                        {
                            if (_htable.Columns[_hpk].AutoIncrement)
                            {
                                string _fk = ForeignKey;

                                if (String.IsNullOrEmpty(_fk.RLTrim()))
                                {
                                    if (_table.Columns.Contains(_hpk) &&
                                        _basetable.Columns.Contains(_hpk)) _fk = _hpk;
                                }
                                else
                                {
                                    if (!_basetable.Columns.Contains(_fk) ||
                                        !_table.Columns.Contains(_fk))
                                    {
                                        if (_table.Columns.Contains(_hpk) &&
                                         _basetable.Columns.Contains(_hpk)) _fk = _hpk; 
                                    }
                                }

                                if (!String.IsNullOrEmpty(_fk.RLTrim()))
                                {
                                    DataRow hrw = null;

                                    foreach (DataRow _row in _htable.Rows)
                                    {
                                        if (_row.RowState != DataRowState.Deleted &&
                                            _row.RowState != DataRowState.Detached)
                                        {
                                            hrw = _row; break;
                                        }
                                    }

                                    if (hrw != null)
                                    {
                                        object _hpkvalue = hrw[_hpk];

                                        if (!Materia.IsNullOrNothing(_hpkvalue))
                                        {
                                            foreach (DataRow _row in _table.Rows)
                                            {
                                                if (_row.RowState != DataRowState.Deleted &&
                                                    _row.RowState != DataRowState.Detached) _row[_fk] = _hpkvalue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (_table != null &&
                        _basetable != null)
                    {
                        string _pk = "";

                        foreach (DataColumn _column in _basetable.Columns)
                        {
                            if (_column.Unique)
                            {
                                _pk = _column.ColumnName; break;
                            }
                        }

                        if (!String.IsNullOrEmpty(_pk.RLTrim()))
                        {
                            if (_basetable.Columns.Contains(_pk) &&
                                _table.Columns.Contains(_pk))
                            {
                                if (_basetable.Columns[_pk].AutoIncrement)
                                {
                                    foreach (DataRow _row in _basetable.Rows)
                                    {
                                        if (_row.RowState != DataRowState.Deleted &&
                                            _row.RowState != DataRowState.Detached)
                                        {
                                            string _filter = "";

                                            foreach (DataColumn _column in _basetable.Columns)
                                            {
                                                if (_column.ColumnName != _pkfieldname &&
                                                    _column.ColumnName != _pk)
                                                {
                                                    string _value = "NULL";

                                                    if (!Materia.IsNullOrNothing(_row[_column.ColumnName]))
                                                    {
                                                        if (_column.DataType.Name == typeof(string).Name ||
                                                            _column.DataType.Name == typeof(String).Name) _value = "'" + _row[_column.ColumnName].ToString().ToSqlValidString() + "'";
                                                        else if (_column.DataType.Name == typeof(DateTime).Name)
                                                        {
                                                            if (VisualBasic.IsDate(_row[_column.ColumnName])) _value = "'" + VisualBasic.CDate(_row[_column.ColumnName]).ToSqlValidString(true) + "'";
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
                                                            if (VisualBasic.IsNumeric(_row[_column.ColumnName])) _value = _row[_column.ColumnName].ToString();
                                                        }
                                                        else if (_column.DataType.Name == typeof(bool).Name ||
                                                                 _column.DataType.Name == typeof(Boolean).Name)
                                                        {
                                                            if (VisualBasic.CBool(_row[_column.ColumnName])) _value = "1";
                                                            else _value = "0";
                                                        }
                                                        else { }
                                                    }

                                                    if (_column.DataType.Name == typeof(DateTime).Name)
                                                    {
                                                        if (!Materia.IsNullOrNothing(_row[_column.ColumnName])) _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + " OR `" + _column.ColumnName + "` = '" + VisualBasic.CDate(_row[_column.ColumnName]).ToSqlValidString() + "')";
                                                        else _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + ")";
                                                    }
                                                    else _filter += (!String.IsNullOrEmpty(_filter.RLTrim()) ? " AND\n" : "") + "(`" + _column.ColumnName + "` = " + _value + ")";
                                                }
                                            }

                                            if (!String.IsNullOrEmpty(_filter.RLTrim()))
                                            {
                                                string _query = "SELECT `" + _pk + "`\n" +
                                                                "FROM `" + _basetable.TableName + "`\n" +
                                                                "WHERE\n" +
                                                                _filter;

                                                object _pkupdated = Que.GetValue<object>(_binder.Connection, _query);
                                                if (VisualBasic.IsNumeric(_pkupdated))
                                                {
                                                    if (_basetable.Columns.Contains(_pkfieldname)) _row[_pkfieldname] = _pkupdated;
                                                    DataRow[] vrws = _table.Select("CONVERT([" + _pk + "], System.String) = '" + _row[_pk].ToString().ToSqlValidString(true) + "'");
                                                    if (vrws.Length > 0) vrws[0][_pk] = _pkupdated;
                                                    _row[_pk] = _pkupdated;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the DataBinding's given name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {  return _name; }

        private void ViewTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e != null)
            {
                if (e.Row != null)
                {
                    if (_basetable != null &&
                        _table != null)
                    {
                        DataRow rw = e.Row; bool _withpkfield = _basetable.Columns.Contains(_pkfieldname);
                        string _pkfield = ""; bool _pkisautoincrement = false;

                        foreach (DataColumn _column in _basetable.Columns)
                        {
                            if (_column.Unique)
                            {
                                _pkfield = _column.ColumnName;
                                _pkisautoincrement = _column.AutoIncrement; break;
                            }
                        }

                        switch (e.Action)
                        {
                            case DataRowAction.Add:
                                object[] _values = new object[_basetable.Columns.Count];

                                foreach (DataColumn _column in _basetable.Columns)
                                {
                                    if (_table.Columns.Contains(_column.ColumnName))
                                    {
                                        if (_column.ColumnName == _pkfield)
                                        {
                                            if (_withpkfield)
                                            {
                                                if (!_pkisautoincrement) _values[_basetable.Columns[_pkfieldname].Ordinal] = rw[_column.ColumnName];
                                            }
                                            
                                            _values[_column.Ordinal] = rw[_column.ColumnName];
                                        }
                                        else _values[_column.Ordinal] = rw[_column.ColumnName];
                                    }
                                }

                                _basetable.Rows.Add(_values);

                                break;
                            case DataRowAction.Change:
                            case DataRowAction.ChangeCurrentAndOriginal:
                            case DataRowAction.ChangeOriginal:
                                foreach (DataColumn _column in _table.Columns)
                                {
                                    if (_column.Unique)
                                    {
                                        _pkfield = _column.ColumnName; break;
                                    }
                                }

                                object _value = rw[_pkfield];
                                DataRow[] rws = null;

                                try
                                {
                                    _value = rw[_pkfield, DataRowVersion.Original];
                                    rws = _basetable.Select("CONVERT([" + _pkfield + "], System.String) = '" + _value.ToString().ToSqlValidString(true) + "'");
                                    if (rws.Length <= 0)
                                    {
                                        _value = rw[_pkfield];
                                        rws = _basetable.Select("CONVERT([" + _pkfield + "], System.String) = '" + _value.ToString().ToSqlValidString(true) + "'");
                                    }
                                }
                                catch
                                {
                                    _value = rw[_pkfield];
                                    rws = _basetable.Select("CONVERT([" + _pkfield + "], System.String) = '" + _value.ToString().ToSqlValidString(true) + "'");
                                }

                                if (rws != null)
                                {
                                    if (rws.Length > 0)
                                    {
                                        DataRow dr = rws[0];

                                        foreach (DataColumn _column in _basetable.Columns)
                                        {
                                            if (_table.Columns.Contains(_column.ColumnName)) dr[_column.ColumnName] = rw[_column.ColumnName];
                                        }

                                        if (dr.RowState == DataRowState.Added)
                                        {
                                            if (!Materia.IsNullOrNothing(rw[_pkfield]))
                                            {
                                                if (_basetable.Columns.Contains(_pkfield) &&
                                                    _basetable.Columns.Contains(_pkfieldname)) dr[_pkfieldname] = rw[_pkfield];
                                            }
                                        }
                                    }

                                    if (_binder == null) _table.AcceptChanges();
                                    else
                                    {
                                        if (!_binder.Saving) _table.AcceptChanges();
                                    }
                                }

                                break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void ViewTable_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e != null)
            {
                if (e.Row != null)
                {
                    if (_basetable != null &&
                        _table != null)
                    {
                        switch (e.Action)
                        {
                            case DataRowAction.Delete:
                                DataRow rw = e.Row; string _pkfield = "";

                                foreach (DataColumn _column in _table.Columns)
                                {
                                    if (_column.Unique)
                                    {
                                        _pkfield = _column.ColumnName; break;
                                    }
                                }

                                if (!String.IsNullOrEmpty(_pkfield.RLTrim()))
                                {
                                    DataRow[] rws = _basetable.Select("CONVERT([" + _pkfield + "], System.String) = '" + rw[_pkfield].ToString().ToSqlValidString(true) + "'");
                                    if (rws.Length > 0) rws[0].Delete();
                                }

                                break;
                            default: break;
                        }
                    }
                }
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
                    DisposeTables();
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

    }

    /// <summary>
    /// Collection of DataBinder binding information.
    /// </summary>
    public class DataBindingCollection : CollectionBase, IDisposable
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of DataBindingCollection.
        /// </summary>
        /// <param name="header"></param>
        public DataBindingCollection(DataBinding header)
        { _header = header; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinding _header = null;

        /// <summary>
        /// Gets the header binding information of the collection.
        /// </summary>
        public DataBinding Header
        {
            get { return _header; }
        }

        /// <summary>
        /// Gets the binding information in the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataBinding this[int index]
        {
            get { return (DataBinding)List[index]; }
        }

        /// <summary>
        /// Gets a binding information with the specified name within the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataBinding this[string name]
        {
            get { return GetBindingByName(name); }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new detail binding information into the collection.
        /// </summary>
        /// <returns></returns>
        public DataBinding Add()
        { return Add(""); }

        /// <summary>
        /// Adds a new detail binding information into the collection.
        /// </summary>
        /// <param name="sql">Raw database command text used to deploy automated database record updates</param>
        /// <returns></returns>
        public DataBinding Add(string sql)
        { return Add(sql, ""); }

        /// <summary>
        /// Adds a new detail binding information into the collection.
        /// </summary>
        /// <param name="basecommandtext">Raw database command text used to deploy automated database record updates</param>
        /// <param name="viewcommandtext">Command text used to provide customized data binded views specially if records are binded into a grid</param>
        /// <returns></returns>
        public DataBinding Add(string basecommandtext, string viewcommandtext)
        {
            DataBinding _binding = new DataBinding(_header.Binder, basecommandtext, viewcommandtext, _header);
            int _index = List.Add(_binding); return (DataBinding)List[_index];
        }

        /// <summary>
        ///  Returns whether the specified binding information already exists within the collection.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public bool Contains(DataBinding binding)
        { return List.Contains(binding); }

        /// <summary>
        /// Returns whether a certain binding information with the specified name exists within the collection or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        { return VisualBasic.CBool(GetBindingByName(name) != null); }

        private DataBinding GetBindingByName(string name)
        {
            DataBinding _binding = null;

            foreach (DataBinding db in List)
            {
                if (db.Name.ToLower() == name.ToLower())
                {
                    _binding = db; break;
                }
            }

            return _binding;
        }

        /// <summary>
        /// Removes the specified binding information from the collection.
        /// </summary>
        /// <param name="binding"></param>
        public void Remove(DataBinding binding)
        {
            if (Contains(binding)) List.Remove(binding);
        }

        /// <summary>
        /// Removes a certain binding information with the specified name from the collection.
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            DataBinding _binding = GetBindingByName(name);
            if (_binding != null) Remove(_binding);
        }

        /// <summary>
        /// Returns the underlying binding names inside the collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _tostring = "";

            foreach (DataBinding db in List)
            {
                if (db != null)
                {
                    if (!String.IsNullOrEmpty(db.ToString().RLTrim())) _tostring += (!String.IsNullOrEmpty(_tostring.RLTrim()) ? "\n" : "") + db.ToString();
                }
            }

            return _tostring;
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
                    foreach (DataBinding db in List) db.Dispose();
                    Materia.RefreshAndManageCurrentProcess();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// DataBinding class required field collection.
    /// </summary>
    public class RequiredFieldCollection : CollectionBase
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of RequiredFieldCollection.
        /// </summary>
        /// <param name="binding"></param>
        public RequiredFieldCollection(DataBinding binding)
        { _binding = binding; }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataBinding _binding = null;

        /// <summary>
        /// Gets the current parented binding information class.
        /// </summary>
        public DataBinding Binding
        {
            get { return _binding; }
        }

        /// <summary>
        /// Gets the assigned required field in the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get { return List[index].ToString(); }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new database required field into the collection.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public int Add(string field)
        { Remove(field); return List.Add(field); }

        /// <summary>
        /// Adds database required fields into the collection.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public List<int> Add(string[] fields)
        {
            List<int> _list = new List<int>();

            foreach (string field in fields)
            { _list.Add(Add(field)); }

            return _list;
        }

        /// <summary>
        /// Returns whether the specified field name already exists within the collection.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool Contains(string field)
        { return List.Contains(field); }

        /// <summary>
        /// Removes the specified field name from the collection.
        /// </summary>
        /// <param name="field"></param>
        public void Remove(string field)
        {
            if (Contains(field)) List.Remove(field);
        }

        #endregion

    }

}
