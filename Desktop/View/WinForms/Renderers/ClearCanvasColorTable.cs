using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Crownwood.DotNetMagic.Common;

namespace ClearCanvas.Desktop.View.WinForms.Renderers
{
	public class ClearCanvasColorTable : Office2007ColorTable
	{
		public ClearCanvasColorTable() : base(Office2007Theme.Black)
		{

		}

		public override Color ToolStripGradientBegin
		{
			get { return Color.FromArgb(255, 27, 29, 31); }
		}

		public override Color ToolStripGradientMiddle
		{
			get { return Color.FromArgb(255, 91, 92, 93); }
		}

		public override Color ToolStripGradientEnd
		{
			get { return Color.FromArgb(255, 176, 177, 177); }
		}

		public override Color ImageMarginGradientBegin
		{
			get { return this.ToolStripGradientMiddle; }
		}

		public override Color ImageMarginGradientMiddle
		{
			get { return base.ToolStripGradientMiddle; }
		}

		public override Color ImageMarginGradientEnd
		{
			get { return base.ToolStripGradientEnd; }
		}

		public override Color CheckSelectedBackground
		{
			get { return this.MenuItemSelectedGradientEnd; }
		}

		public override Color MenuItemSelected
		{
			get { return this.MenuItemSelectedGradientBegin; }
		}

		public override Color MenuItemBorder
		{
			get { return this.MenuItemSelectedGradientBegin; }
		}

		public virtual Color MenuItemText
		{
			get { return Color.WhiteSmoke; }
		}

		public override Color ButtonCheckedGradientBegin
		{
			get { return this.MenuItemSelectedGradientBegin; }
		}

		public override Color ButtonCheckedGradientMiddle
		{
			get { return this.MenuItemSelectedGradientBegin; }
		}

		public override Color ButtonCheckedGradientEnd
		{
			get { return this.MenuItemSelectedGradientEnd; }
		}

		public override Color ButtonSelectedBorder
		{
			get { return this.MenuItemSelectedGradientBegin; }
		}
	}
}
