using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CannedTextEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextEditorComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextEditorComponentViewExtensionPoint))]
	public class CannedTextEditorComponent : ApplicationComponent
	{
		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);

		private readonly EntityRef _cannedTextRef;
		private CannedTextDetail _cannedTextDetail;
		private CannedTextSummary _cannedTextSummary;

		private StaffSummary _currentStaff;
		private List<StaffGroupSummary> _staffGroupChoices;

		private readonly bool _isDuplicate;
		private readonly bool _isNew;
		private readonly bool _canChangeType;
		private bool _isEditingPersonal;

		/// <summary>
		/// Constructor for creating a new canned text.
		/// </summary>
		public CannedTextEditorComponent()
		{
			_isNew = true;
			_canChangeType = HasGroupAdminAuthority;
		}

		/// <summary>
		/// Constructor for editing an existing canned text.
		/// </summary>
		public CannedTextEditorComponent(EntityRef cannedTextRef)
		{
			_isNew = false;
			_cannedTextRef = cannedTextRef;

			_canChangeType = false;
		}

		/// <summary>
		/// Constructor for duplicating an existing canned text.
		/// </summary>
		public CannedTextEditorComponent(EntityRef cannedTextRef, bool duplicate)
		{
			_isNew = true;
			_cannedTextRef = cannedTextRef;

			_canChangeType = HasGroupAdminAuthority;
			_isDuplicate = duplicate;
		}


		public override void Start()
		{
			Platform.GetService<ICannedTextService>(
				delegate(ICannedTextService service)
					{
						GetCannedTextEditFormDataResponse formDataResponse =
							service.GetCannedTextEditFormData(new GetCannedTextEditFormDataRequest());
						_staffGroupChoices = formDataResponse.StaffGroups;
						_currentStaff = formDataResponse.CurrentStaff;


						if (_isNew && _isDuplicate == false)
						{
							_cannedTextDetail = new CannedTextDetail();

							// new canned text
							_isEditingPersonal = true;
							_cannedTextDetail.Id.Staff = _currentStaff;
						}
						else
						{
							LoadCannedTextForEditResponse response = service.LoadCannedTextForEdit(new LoadCannedTextForEditRequest(_cannedTextRef));
							_cannedTextDetail = response.CannedTextDetail;

							_isEditingPersonal = _cannedTextDetail.IsPersonal;

							// Duplicating an item, so the new canned text starts with fields pre-populated.  Set modified to true
							if (_isDuplicate)
								this.Modified = true;
						}
					}); 

			// The selected staff groups should only contain entries in the selected group choices
			if (_cannedTextDetail.Id.StaffGroup == null)
			{
				_cannedTextDetail.Id.StaffGroup = CollectionUtils.FirstElement(_staffGroupChoices);
			}
			else
			{
				_cannedTextDetail.Id.StaffGroup = CollectionUtils.SelectFirst(_staffGroupChoices,
					delegate(StaffGroupSummary choice)
					{
						return _cannedTextDetail.Id.StaffGroup.StaffGroupRef.Equals(choice.StaffGroupRef, true);
					});
			}

			base.Start();
		}

		/// <summary>
		/// Returns the Canned Text summary for use by the caller of this component
		/// </summary>
		public CannedTextSummary UpdatedCannedTextSummary
		{
			get { return _cannedTextSummary; }
		}

		#region Presentation Model

		public bool IsNew
		{
			get { return _isNew; }
		}

		public bool IsDuplicate
		{
			get { return _isDuplicate; }
		}

		public bool CanChangeType
		{
			get { return _canChangeType &&  _staffGroupChoices.Count > 0; }
		}

		public bool IsEditingPersonal
		{
			get { return _isEditingPersonal; }
			set { _isEditingPersonal = value; }
		}

		public bool IsEditingGroup
		{
			get { return !_isEditingPersonal; }
			set { _isEditingPersonal = !value; }
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		public object NullFilterItem
		{
			get { return _nullFilterItem; }
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _cannedTextDetail.Id.Name; }
			set
			{
				_cannedTextDetail.Id.Name = value;
				this.Modified = true;
			}
		}

		public string Category
		{
			get { return _cannedTextDetail.Id.Category; }
			set
			{
				_cannedTextDetail.Id.Category = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Text
		{
			get { return _cannedTextDetail.Text; }
			set
			{
				_cannedTextDetail.Text = value;
				this.Modified = true;
			}
		}

		public StaffGroupSummary StaffGroup
		{
			get { return _cannedTextDetail.Id.StaffGroup; }
			set
			{
				_cannedTextDetail.Id.StaffGroup = value;
				this.Modified = true;
			}
		}

		public IList StaffGroupChoices
		{
			get { return _staffGroupChoices; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				if (this.IsEditingPersonal)
					_cannedTextDetail.Id.StaffGroup = null;
				else
					_cannedTextDetail.Id.Staff = null;

				Platform.GetService<ICannedTextService>(
					delegate(ICannedTextService service)
					{
						if (_isNew)
						{
							AddCannedTextResponse response = service.AddCannedText(new AddCannedTextRequest(_cannedTextDetail));
							_cannedTextSummary = response.CannedTextSummary;
						}
						else
						{
							UpdateCannedTextResponse response = service.UpdateCannedText(new UpdateCannedTextRequest(_cannedTextRef, _cannedTextDetail));
							_cannedTextSummary = response.CannedTextSummary;
						}
					});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e,
					SR.ExceptionSaveCannedText,
					this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		public string FormatStaffGroup(object item)
		{
			if (item is StaffGroupSummary)
			{
				StaffGroupSummary staffGroup = (StaffGroupSummary)item;
				return staffGroup.Name;
			}
			else
				return item.ToString(); // place-holder items
		}

		private static bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.CannedText.Group); }
		}
	}
}
