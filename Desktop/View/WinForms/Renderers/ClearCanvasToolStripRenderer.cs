using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Resources;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using Crownwood.DotNetMagic.Common;

namespace ClearCanvas.Desktop.View.WinForms.Renderers
{
	public class ClearCanvasToolStripRenderer : ToolStripProfessionalRenderer
	{
		private ClearCanvasColorTable _colorTable;

		public ClearCanvasToolStripRenderer()
			: base(new ClearCanvasColorTable())
		{
			//_colorTable = new ClearCanvasColorTable();
			_colorTable = this.ColorTable as ClearCanvasColorTable;
			this.RoundedEdges = false;
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if (e.ToolStrip is MenuStrip ||
				e.ToolStrip is ToolStripDropDown)
			{
				if (e.Item.Selected || e.Item.Pressed)
					e.TextColor = _colorTable.ToolStripGradientBegin;
				else
					e.TextColor = _colorTable.MenuItemText;
			}

			base.OnRenderItemText(e);
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			e.ArrowColor = _colorTable.MenuItemText;
			base.OnRenderArrow(e);
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
		    base.OnRenderImageMargin(e);

		    Graphics g = e.Graphics;

		    using (LinearGradientBrush b = new LinearGradientBrush(
		        e.AffectedBounds,
				_colorTable.ImageMarginGradientEnd,
				_colorTable.ImageMarginGradientBegin,
		        0.0))
		    {
		        g.FillRectangle(b, e.AffectedBounds);
		    }
		}
		
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			base.OnRenderToolStripBackground(e);
			
			Graphics g = e.Graphics;

			if (e.ToolStrip is MenuStrip || 
				e.ToolStrip is ToolStripDropDown)
			{
				DrawFlatRectangle(e.Graphics, e.AffectedBounds, _colorTable.ToolStripGradientBegin);
			}
			else if (e.ToolStrip is ToolStrip)
			{
				if (e.ToolStrip.Orientation == Orientation.Horizontal)
				{
					DrawHighlightedRectangle(
						e.Graphics,
						e.AffectedBounds,
						_colorTable.ToolStripGradientBegin,
						_colorTable.ToolStripGradientMiddle,
						_colorTable.ToolStripGradientEnd,
						0.33f);
				}
				else
				{
					DrawFlatRectangle(e.Graphics, e.AffectedBounds, _colorTable.ToolStripGradientBegin);
				}
			}
		}

		private void DrawFlatRectangle(Graphics g, Rectangle rect, Color color)
		{
			using (LinearGradientBrush b = new LinearGradientBrush(
				rect,
				color,
				color,
				0.0f))
			{
				g.FillRectangle(b, rect);
			}
		}

		private void DrawHighlightedRectangle(
			Graphics g, 
			Rectangle rect, 
			Color colorBegin,
			Color colorMiddle,
			Color colorEnd,
			float highlightStart)
		{
			Rectangle upperHalf = new Rectangle(
				rect.Left,
				rect.Top,
				rect.Width,
				(int) (rect.Height * highlightStart));

			Rectangle lowerHalf = new Rectangle(
				rect.Left,
				(int) (rect.Top + rect.Height * highlightStart),
				rect.Width,
				rect.Height);

			using (LinearGradientBrush b = new LinearGradientBrush(
				lowerHalf,
				colorBegin,
				colorBegin,
				90))
			{
				g.FillRectangle(b, lowerHalf);
			}

			using (LinearGradientBrush b = new LinearGradientBrush(
				upperHalf,
				colorEnd,
				colorMiddle,
				90))
			{
				g.FillRectangle(b, upperHalf);
			}

		}
	}
}

		//protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		//{
		//    base.OnRenderItemCheck(e);

		//    ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;

		//    if (menuItem == null)
		//        return;

		//    // Check if the item is selected.
		//    if (menuItem.Checked)
		//    {
		//        //ResourceResolver resolver = new ResourceResolver(Assembly.GetAssembly(this.GetType()));
		//        //e.Graphics.DrawImageUnscaled(
		//        //    IconFactory.CreateIcon("Renderers.YellowDotSmall.png", resolver), 
		//        //    e.ImageRectangle.Location);

		//        using (LinearGradientBrush brush = new LinearGradientBrush(
		//            e.ImageRectangle,
		//            RoyalNoirTheme.ButtonHover,
		//            RoyalNoirTheme.ButtonHover,
		//            90))
		//        {
		//            Rectangle rect = new Rectangle(
		//                e.ImageRectangle.Left,
		//                e.ImageRectangle.Top,
		//                e.ImageRectangle.Width - 1,
		//                e.ImageRectangle.Height - 1);

		//            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		//            e.Graphics.FillRectangle(brush, rect);
		//        }
		//    }

		//}


			//public static readonly Color BackgroundColor = Color.FromArgb(255, 27, 29, 31);
		//public static readonly Color BackgroundGlossColorBegin = Color.FromArgb(255, 91, 92, 93);
		//public static readonly Color BackgroundGlossColorEnd = Color.FromArgb(255, 176, 177, 177);
		//public static readonly Color ForegroundColor = Color.WhiteSmoke;
		//public static readonly Color ButtonHover = Color.FromArgb(255, 255, 234, 94);
		//public static readonly Color ButtonChecked = Color.FromArgb(255, 255, 234, 94);
		//public static readonly Color MenuItemHover = Color.FromArgb(170, 255, 234, 94);