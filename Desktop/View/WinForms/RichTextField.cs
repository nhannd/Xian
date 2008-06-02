using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class RichTextField : UserControl
	{
		public RichTextField()
		{
			InitializeComponent();
		}

		public string Value
		{
			get { return NullIfEmpty(_richTextBox.Text); }
			set { _richTextBox.Text = value; }
		}

		public event EventHandler ValueChanged
		{
			add { _richTextBox.TextChanged += value; }
			remove { _richTextBox.TextChanged -= value; }
		}

		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		[DefaultValue(false)]
		public bool ReadOnly
		{
			get { return _richTextBox.ReadOnly; }
			set { _richTextBox.ReadOnly = value; }
		}

		[DefaultValue(true)]
		public bool WordWrap
		{
			get { return _richTextBox.WordWrap; }
			set { _richTextBox.WordWrap = value; }
		}

		[DefaultValue(RichTextBoxScrollBars.None)]
		public RichTextBoxScrollBars ScrollBars
		{
			get { return _richTextBox.ScrollBars; }
			set { _richTextBox.ScrollBars = value; }
		}

		[DefaultValue(32767)]
		public int MaximumLength
		{
			get { return _richTextBox.MaxLength; }
			set { _richTextBox.MaxLength = value; }
		}

		private static string NullIfEmpty(string value)
		{
			return (value != null && value.Length == 0) ? null : value;
		}

		private void _richTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (_richTextBox.ReadOnly)
			{
				e.Effect = DragDropEffects.None;
				return;
			}

			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void _richTextBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				string dropString = (String)e.Data.GetData(typeof(String));

				// Insert string at the current keyboard cursor
				int currentIndex = _richTextBox.SelectionStart;
				_richTextBox.Text = string.Concat(
					_richTextBox.Text.Substring(0, currentIndex),
					dropString,
					_richTextBox.Text.Substring(currentIndex));
			}
		}
	}
}
