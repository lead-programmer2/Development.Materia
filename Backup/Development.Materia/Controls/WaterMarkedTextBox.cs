#region "imports"

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

#endregion

namespace Development.Materia.Controls
{
    /// <summary>
    /// Watermark supported textbox control.
    /// </summary>
    [Description("Watermark supported textbox control."),
     ToolboxBitmap(typeof(WaterMarkedTextBox), "WaterMarkedTextBox.bmp")]
    public class WaterMarkedTextBox :  TextBox
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of WaterMarkedTextBox.
        /// </summary>
        public WaterMarkedTextBox()
        { JoinEvents(true); }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Font _oldfont = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _watermarktextenabled = false;

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _watermarkforecolor = Color.Gray;

        /// <summary>
        /// Gets or sets the foreground color for the watermark text.
        /// </summary>
        [Description("Gets or sets the foreground color for the watermark text."),
         DefaultValue(typeof(Color), "Gray")]
        public Color WaterMarkForeColor
        {
            get { return _watermarkforecolor; }
            set { _watermarkforecolor = value; Invalidate(); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _watermarktext = "";

        /// <summary>
        /// Gets or sets the watermark text associated with the control.
        /// </summary>
        [Description("Gets or sets the watermark text associated with the control.")]
        public string WaterMarkText
        {
            get { return _watermarktext; }
            set { _watermarktext = value; Invalidate(); }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Raises the OnCreate method.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            WaterMark_Toggel(null, null);
        }

        /// <summary>
        /// Raises the Paint method.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Font _drawfont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
            SolidBrush _drawbrush = new SolidBrush(WaterMarkForeColor);
            
            e.Graphics.DrawString((_watermarktextenabled ? WaterMarkText : Text), _drawfont, _drawbrush, new PointF(0.0F, 0.0F));
            base.OnPaint(e);
        }

        private void JoinEvents(bool join)
        {
            if (join)
            {
                TextChanged += new System.EventHandler(this.WaterMark_Toggel);
                LostFocus += new System.EventHandler(this.WaterMark_Toggel);
                FontChanged += new System.EventHandler(this.WaterMark_FontChanged);
            }
        }

        private void WaterMark_Toggel(object sender, EventArgs e)
        {
            if (Text.Length <= 0) EnableWaterMark();
            else  DisbaleWaterMark();
        }

        private void EnableWaterMark()
        {
           _oldfont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);
           SetStyle(ControlStyles.UserPaint, true);
            _watermarktextenabled = true; Refresh();
        }

        private void DisbaleWaterMark()
        {
           _watermarktextenabled = false;
            SetStyle(ControlStyles.UserPaint, false);
            if (_oldfont != null)
                Font = new Font(_oldfont.FontFamily, _oldfont.Size, _oldfont.Style, _oldfont.Unit);
        }

        private void WaterMark_FontChanged(object sender, EventArgs e)
        {
            if (_watermarktextenabled)
            {
                _oldfont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);  Refresh();
            }
        }

        #endregion

    }
}
