using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class ModalityPicker : UserControl
	{
		private CheckedListBox _modalityPicker;

		private List<string> _availableModalities; 
		private SortedList<string, string> _checkedModalities;

		private bool _availableModalitiesSet;

		public ModalityPicker()
		{
			InitializeComponent();

			_modalityPicker = new CheckedListBox();
			_modalityPicker.BorderStyle = BorderStyle.None;
			_modalityPicker.CheckOnClick = true;
			_modalityPicker.SelectionMode = SelectionMode.One;
			
			// We don't sort the list automatically, because we want the "Clear" item at the top.
			_modalityPicker.Sorted = false;
			_modalityPicker.ItemCheck += new ItemCheckEventHandler(OnModalityPickerItemCheck);

			_contextMenuStrip.Items.Add(new ToolStripControlHost(_modalityPicker));

			//_showModalityListButton.Image = IconFactory.CreateIcon("ExpandRight.png", new ResourceResolver(this.GetType().Assembly));

			_availableModalitiesSet = false;
			_availableModalities = new List<string>();
			_checkedModalities = new SortedList<string, string>();
		}

		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)] 
		[Browsable(false)]
		public IList<string> CheckedModalities
		{
			get
			{
				return _checkedModalities.Keys;
			}
			set
			{
				SetCheckedModalities(value);
			}
		}

		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		public void SetAvailableModalities(ICollection<string> modalities)
		{
			if (_availableModalitiesSet || modalities == null || modalities.Count == 0)
				throw new InvalidOperationException(SR.ErrorCannotResetModalities);

			_availableModalitiesSet = true;

			SortedList<string, string> sorter = new SortedList<string, string>();
			if (modalities != null)
			{
				foreach (string modality in modalities)
				{
					if (!sorter.ContainsKey(modality))
						sorter.Add(modality, modality);
				}
			}

			_availableModalities.Clear();
			_availableModalities.Add(SR.ItemClear);
			_availableModalities.AddRange(sorter.Keys);

			_modalityPicker.Items.Clear();
			_modalityPicker.Items.AddRange(_availableModalities.ToArray());

			this.CheckedModalities = new List<string>();
		}

		private void SetCheckedModalities(IList<string> modalities)
		{
			if (!_availableModalitiesSet)
				return;

			List<string> checkedModalities = new List<string>();
			if (modalities != null)
				checkedModalities.AddRange(modalities);

			_checkedModalities.Clear();

			foreach (string checkedModality in checkedModalities)
			{
				if (!_checkedModalities.ContainsKey(checkedModality))
					_checkedModalities.Add(checkedModality, checkedModality);
			}

			//skip the 'Clear' selection.
			for (int index = 1; index < _modalityPicker.Items.Count; ++index)
			{
				string modality = (string)_modalityPicker.Items[index];
				_modalityPicker.SetItemChecked(index, _checkedModalities.ContainsKey(modality));
			}

			UpdateText();
		}

		private void OnModalityPickerItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.Index == 0) // "Clear" item
			{
				e.NewValue = CheckState.Unchecked;
				this.CheckedModalities = new List<string>();
				return;
			}

			string itemModality = (string)_modalityPicker.Items[e.Index];

			if (e.NewValue == CheckState.Unchecked)
			{
				if (_checkedModalities.ContainsKey(itemModality))
					_checkedModalities.Remove(itemModality);
			}
			else
			{
				if (!_checkedModalities.ContainsKey(itemModality))
					_checkedModalities.Add(itemModality, itemModality);
			}

			UpdateText();
		}

		private void UpdateText()
		{
			string newText = "";
			foreach (string checkedModality in _checkedModalities.Keys)
			{
				newText += checkedModality + @", ";
			}

			if (newText.Length > 0)
				newText = newText.Remove(newText.Length - 2);

			this._modalitiesString.Text = newText;
		}

		private void OnShowModalityListClick(object sender, EventArgs e)
		{
			_contextMenuStrip.Show(_showModalityListButton, new Point(0, _showModalityListButton.Height), ToolStripDropDownDirection.BelowRight);
		}
	}
}
