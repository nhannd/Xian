#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class DecoratedTableView : TableView
    {
        private const int CUSTOM_CONTENT_HEIGHT = 20;

        private int _rowHeight = 0;


        public DecoratedTableView()
            : base()
        {
            _rowHeight = this.DataGridView.RowTemplate.Height;

            this.DataGridView.RowPrePaint += SetCustomBackground;
            this.DataGridView.RowPostPaint += DisplayCellSubRows;
            this.DataGridView.RowPostPaint += OutlineCell;
        }

        public ITable Table
        {
            get { return base.Table; }
            set
            {
                base.Table = value;

                IDecoratedTable summaryTable = base.Table as IDecoratedTable;
                if (summaryTable != null)
                {
                    // Set a cell padding to provide space for the top of the focus 
                    // rectangle and for the content that spans multiple columns. 
                    Padding newPadding = new Padding(0, 1, 0,
                        CUSTOM_CONTENT_HEIGHT * ((int)summaryTable.CellRowCount - 1));
                    this.DataGridView.RowTemplate.DefaultCellStyle.Padding = newPadding;

                    // Set the row height to accommodate the content that 
                    // spans multiple columns.
                    this.DataGridView.RowTemplate.Height = _rowHeight + CUSTOM_CONTENT_HEIGHT * ((int)summaryTable.CellRowCount - 1);
                }
            }
        }

        // Paints the custom background specified in IDecoratedTable
        private void SetCustomBackground(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if ((e.State & DataGridViewElementStates.Selected) ==
                        DataGridViewElementStates.Selected)
            {
                // do nothing?
                return;
            }

            IDecoratedTable summaryTable = base.Table as IDecoratedTable;
            if (summaryTable != null && summaryTable.BackgroundColorSelector != null)
            {
                Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

                // Color.FromName("Empty") does not return Color.Empty, so need to manually check for Empty
                string colorName = summaryTable.BackgroundColorSelector(base.Table.Items[e.RowIndex]);
                Color backgroundColor = colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);

                if (backgroundColor.Equals(Color.Empty))
                {
                    backgroundColor = e.InheritedRowStyle.BackColor;
                }

                // Paint the custom selection background.
                using (Brush backbrush =
                    new System.Drawing.SolidBrush(backgroundColor))
                {
                    e.PaintParts &= ~DataGridViewPaintParts.Background;
                    e.Graphics.FillRectangle(backbrush, rowBounds);
                }
            }
        }

        // Paints the custom outline specified in IDecoratedTable
        private void OutlineCell(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

            IDecoratedTable summaryTable = base.Table as IDecoratedTable;
            if (summaryTable != null && summaryTable.OutlineColorSelector != null)
            {
                int penWidth = 2;
                Rectangle outline = new Rectangle(
                    rowBounds.Left + penWidth / 2,
                    rowBounds.Top + penWidth / 2 + 1,
                    rowBounds.Width - penWidth,
                    rowBounds.Height - penWidth - 2);

                Color outlineColor = Color.FromName(summaryTable.OutlineColorSelector(base.Table.Items[e.RowIndex]));

                using (Pen outlinePen =
                    new System.Drawing.Pen(outlineColor, penWidth))
                {
                    e.Graphics.DrawRectangle(outlinePen, outline);
                }
            }
        }

        // Paints the content that spans multiple columns and the focus rectangle.
        private void DisplayCellSubRows(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

            SolidBrush forebrush = null;
            try
            {
                // Determine the foreground color.
                if ((e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected)
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.SelectionForeColor);
                }
                else
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.ForeColor);
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < base.Table.Columns.Count; i++)
                {
                    IDecoratedTableColumn col = base.Table.Columns[i] as IDecoratedTableColumn;
                    if (col != null && col.CellRow > 0)
                    {
                        DataGridViewRow row = this.DataGridView.Rows[e.RowIndex];
                        object recipe = row.Index != -1 ? row.Cells[i].Value : null;

                        if (recipe != null)
                        {
                            sb.Append(recipe.ToString() + " ");
                        }

                    }
                }

                string text = sb.ToString().Trim();

                if (string.IsNullOrEmpty(text) == false)
                {
                    // Calculate the bounds for the content that spans multiple 
                    // columns, adjusting for the horizontal scrolling position 
                    // and the current row height, and displaying only whole
                    // lines of text.
                    Rectangle textArea = rowBounds;
                    textArea.X -= this.DataGridView.HorizontalScrollingOffset;
                    textArea.Width += this.DataGridView.HorizontalScrollingOffset;
                    textArea.Y += rowBounds.Height - e.InheritedRowStyle.Padding.Bottom;
                    textArea.Height -= rowBounds.Height - e.InheritedRowStyle.Padding.Bottom;
                    textArea.Height = (textArea.Height / e.InheritedRowStyle.Font.Height) * e.InheritedRowStyle.Font.Height;

                    // Calculate the portion of the text area that needs painting.
                    RectangleF clip = textArea;
                    int startX = this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0;
                    clip.Width -= startX + 1 - clip.X;
                    clip.X = startX + 1;
                    RectangleF oldClip = e.Graphics.ClipBounds;
                    e.Graphics.SetClip(clip);

                    // Draw the content that spans multiple columns.
                    e.Graphics.DrawString(text, e.InheritedRowStyle.Font, forebrush, textArea);

                    e.Graphics.SetClip(oldClip);
                }
            }
            finally
            {
                forebrush.Dispose();
            }
        }

        private Rectangle GetAdjustedRowBounds(Rectangle rowBounds)
        {
            return new Rectangle(
                    (this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0) + rowBounds.Left,
                    rowBounds.Top,
                    this.DataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.DataGridView.HorizontalScrollingOffset,
                    rowBounds.Height);
        }
    }
}
