#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffGroupDetailsEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffGroupDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// StaffGroupDetailsEditorComponent class.
	/// </summary>
	[AssociateView(typeof(StaffGroupDetailsEditorComponentViewExtensionPoint))]
	public class StaffGroupDetailsEditorComponent : ApplicationComponent
	{
		private StaffGroupDetail _staffGroupDetail;

		public StaffGroupDetail StaffGroupDetail
		{
			get { return _staffGroupDetail; }
			set { _staffGroupDetail = value; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _staffGroupDetail.Name; }
			set
			{
				if (_staffGroupDetail.Name != value)
				{
					_staffGroupDetail.Name = value;
					this.Modified = true;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public string Description
		{
			get { return _staffGroupDetail.Description; }
			set
			{
				if (_staffGroupDetail.Description != value)
				{
					_staffGroupDetail.Description = value;
					this.Modified = true;
					NotifyPropertyChanged("Description");
				}
			}
		}

		public bool IsElective
		{
			get { return _staffGroupDetail.IsElective; }
			set
			{
				if (_staffGroupDetail.IsElective != value)
				{
					_staffGroupDetail.IsElective = value;
					this.Modified = true;
					NotifyPropertyChanged("IsElective");
				}
			}
		}

		#endregion
	}
}
