#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class ExternalPractitionerMergeAffectedOrderCustomControl : UserControl
	{
		private readonly ExternalPractitionerMergeAffectedOrderTableItem _item;

		public ExternalPractitionerMergeAffectedOrderCustomControl(ExternalPractitionerMergeAffectedOrderTableItem item)
		{
			InitializeComponent();
			_item = item;

			_orderLink.DataBindings.Add("Text", _item, "OrderInfo", true, DataSourceUpdateMode.OnPropertyChanged);
			_orderLink.LinkClicked += delegate { _item.ShowOrderPreview(); };

			_practitionerLink.DataBindings.Add("Text", _item, "PractitionerInfo", true, DataSourceUpdateMode.OnPropertyChanged);
			_practitionerLink.LinkClicked += delegate { _item.ShowPractitionerPreview(); };

			_practitionerRole.DataBindings.Add("Text", _item, "PractitionerRole", true, DataSourceUpdateMode.OnPropertyChanged);
			_oldContactPointInfo.DataBindings.Add("Text", _item, "OldContactPointInfo", true, DataSourceUpdateMode.OnPropertyChanged);
			_newContactPointInfo.DataBindings.Add("Text", _item, "NewContactPointInfo", true, DataSourceUpdateMode.OnPropertyChanged);

			_replacedWith.DataSource = _item.ContactPointChoices;
			_replacedWith.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _item.FormatItem(args.ListItem); };
			_replacedWith.DataBindings.Add("Value", _item, "SelectedContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}

	public class ExternalPractitionerMergeAffectedOrderDynamicTable : DynamicCustomControlTable<ExternalPractitionerMergeAffectedOrderTableItem>
	{
		public ExternalPractitionerMergeAffectedOrderDynamicTable()
		{
			this.ColumnCount = 1;
		}

		#region Override Methods

		protected override void ClearCustomData()
		{
			// Nothing to do.
		}

		protected override List<Control> GetRowControls(ExternalPractitionerMergeAffectedOrderTableItem item)
		{
			return new List<Control> { CreateCustomControl(item) };
		}

		#endregion

		private static string GetUniqueControlName(ExternalPractitionerMergeAffectedOrderTableItem item)
		{
			return string.Format("{0} - {1}", item.Order.AccessionNumber, item.Recipient.Practitioner.PractitionerRef);
		}

		private static ExternalPractitionerMergeAffectedOrderCustomControl CreateCustomControl(ExternalPractitionerMergeAffectedOrderTableItem item)
		{
			return new ExternalPractitionerMergeAffectedOrderCustomControl(item)
				{
					Name = GetUniqueControlName(item),
					Dock = DockStyle.Fill,
				};
		}
	}
}
