#region "imports"

using System;
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
    /// Textbox editor with sizable grip border.
    /// </summary>
    [Description("Multiline textbox editor with sizable grip border."),
     ToolboxBitmap(typeof(SizableTextBox), "SizableTextBox.bmp")]
    public class SizableTextBox  : TextBox
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of SizableTextBox.
        /// </summary>
        public SizableTextBox()
        { InitializeComponent(); }

        #endregion

        #region "variables"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Container _components = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Label _griplabel = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Point _pointdown = new Point(0, 0);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Size _sizedown = new Size(1, 1);

        #endregion

        #region "methods"

        /// <summary>
        /// Releases all unmanaged resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null) _components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _griplabel = new Label();
            SuspendLayout();

            _griplabel.Anchor = (AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Right);
            _griplabel.BackColor = Color.Transparent;
            _griplabel.Cursor = Cursors.SizeNWSE;
            _griplabel.Image = Properties.Resources.Grip;
            _griplabel.Location = new Point(100, 0);
            _griplabel.Name = "lblGrip";
            _griplabel.Size = new Size(16, 16);
            _griplabel.TabIndex = 1;
            _griplabel.MouseDown += new MouseEventHandler(lblGrip_MouseDown);
            _griplabel.MouseMove += new MouseEventHandler(lblGrip_MouseMove);

            BackColor = Color.White;
            Controls.Add(_griplabel);
            MinimumSize = new Size(120, 20);
            Multiline = true;
            Size = new Size(120, 20);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region "control events"

        private void lblGrip_MouseDown(object sender, MouseEventArgs e)
        {
            _pointdown = Control.MousePosition; _sizedown = Size;
        }

        private void lblGrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point _point = Control.MousePosition;
                Size = new Size(_sizedown.Width + (_point.X - _pointdown.X),
                                _sizedown.Height + (_point.Y - _pointdown.Y));
            }
        }

        #endregion

    }
}
