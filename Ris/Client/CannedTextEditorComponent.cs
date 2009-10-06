#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;

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
        private readonly List<string> _categoryChoices;

        private CannedTextDetail _cannedTextDetail;
        private CannedTextSummary _cannedTextSummary;

        private List<StaffGroupSummary> _staffGroupChoices;

        private readonly bool _isDuplicate;
        private readonly bool _isNew;
        private readonly bool _canChangeType;
        private bool _isEditingPersonal;

        /// <summary>
        /// Constructor for creating a new canned text.
        /// </summary>
        public CannedTextEditorComponent(List<string> categoryChoices)
        {
            _isNew = true;
            _canChangeType = HasPersonalAdminAuthority && HasGroupAdminAuthority;
            _categoryChoices = categoryChoices;
        }

        /// <summary>
        /// Constructor for editing an existing canned text.
        /// </summary>
        public CannedTextEditorComponent(List<string> categoryChoices, EntityRef cannedTextRef)
        {
            _isNew = false;
            _cannedTextRef = cannedTextRef;
            _categoryChoices = categoryChoices;

            _canChangeType = false;
        }

        /// <summary>
        /// Constructor for duplicating an existing canned text.
        /// </summary>
        public CannedTextEditorComponent(List<string> categoryChoices, EntityRef cannedTextRef, bool duplicate)
            : this(categoryChoices)
        {
            _cannedTextRef = cannedTextRef;
            _isDuplicate = duplicate;
        }

        public override void Start()
        {
            // Insert a blank choice as the first element
            _categoryChoices.Insert(0, "");

            Platform.GetService<ICannedTextService>(
            	service =>
            	{
            		var formDataResponse =
            			service.GetCannedTextEditFormData(new GetCannedTextEditFormDataRequest());
            		_staffGroupChoices = formDataResponse.StaffGroups;


            		if (_isNew && _isDuplicate == false)
            		{
            			_cannedTextDetail = new CannedTextDetail();
            			_isEditingPersonal = HasPersonalAdminAuthority;
            		}
            		else
            		{
            			var response =
            				service.LoadCannedTextForEdit(new LoadCannedTextForEditRequest(_cannedTextRef));
            			_cannedTextDetail = response.CannedTextDetail;

            			_isEditingPersonal = _cannedTextDetail.IsPersonal;

            			if (_isDuplicate)
            				this.Name = "";
            		}
            	});

            // The selected staff groups should only contain entries in the selected group choices
            if (_cannedTextDetail.StaffGroup == null)
            {
                _cannedTextDetail.StaffGroup = CollectionUtils.FirstElement(_staffGroupChoices);
            }
            else
            {
                _cannedTextDetail.StaffGroup = CollectionUtils.SelectFirst(_staffGroupChoices,
                                                                           choice =>
                                                                           _cannedTextDetail.StaffGroup.StaffGroupRef.
                                                                           	Equals(choice.StaffGroupRef, true));
            }

            // add validation rule to ensure the group must be populated when editing group
            this.Validation.Add(new ValidationRule("StaffGroup",
                delegate
                {
                    var ok = this.IsEditingPersonal || this.IsEditingGroup && this.StaffGroup != null;
                    return new ValidationResult(ok, Desktop.SR.MessageValueRequired);
                }));

            if (CannedTextSettings.Default.RestrictCannedTextNameToAlphaChars)
            {
                // add validation rule to ensure the name does not contain invalid characters
                this.Validation.Add(new ValidationRule("Name",
                    delegate
                    {
                        // only allow alphabets and space
                        var ok = Regex.IsMatch(this.Name, @"^[A-Za-z ]+$");
                        return new ValidationResult(ok, SR.MessageNameCanOnlyContainAlphaChars);
                    }));
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

        public bool CanChangeType
        {
            get { return _canChangeType; }
        }

        public bool IsEditingPersonal
        {
            get{ return _isEditingPersonal; }
            set{ _isEditingPersonal = value; }
        }

        public bool IsEditingGroup
        {
            get { return !this.IsEditingPersonal; }
            set { this.IsEditingPersonal = !value; }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.IsEditingPersonal && !HasPersonalAdminAuthority ||
                        this.IsEditingGroup && !HasGroupAdminAuthority;
            }
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
            get { return _cannedTextDetail.Name; }
            set
            {
                _cannedTextDetail.Name = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Category
        {
            get { return _cannedTextDetail.Category; }
            set
            {
                _cannedTextDetail.Category = value;
                this.Modified = true;
            }
        }

        public bool HasGroupAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Group); }
        }

        public bool HasPersonalAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Personal); }
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

        // Dynamic Null Validation when IsEditingGroup
        public StaffGroupSummary StaffGroup
        {
            get { return _cannedTextDetail.StaffGroup; }
            set
            {
                _cannedTextDetail.StaffGroup = value;
                this.Modified = true;
            }
        }

        public IList StaffGroupChoices
        {
            get { return _staffGroupChoices; }
        }

		public string FormatStaffGroup(object item)
		{
			if (!(item is StaffGroupSummary))
				return item.ToString(); // place-holder items

			var staffGroup = (StaffGroupSummary)item;
			return staffGroup.Name;
		}


        public IList CategoryChoices
        {
            get { return _categoryChoices; }
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
                // Depends on the editing mode, remove the unnecessary information
                if (this.IsEditingPersonal)
                    _cannedTextDetail.StaffGroup = null;

                // Find a list of canned texts that matches the name
//                if(!ValidateStuff())
//					return;

            	// Commit the changes
                Platform.GetService<ICannedTextService>(
                	service =>
                	{
                		if (_isNew)
                		{
                			var response = service.AddCannedText(new AddCannedTextRequest(_cannedTextDetail));
                			_cannedTextSummary = response.CannedTextSummary;
                		}
                		else
                		{
                			var response =
                				service.UpdateCannedText(new UpdateCannedTextRequest(_cannedTextRef, _cannedTextDetail));
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

		private bool ValidateStuff()
		{
			List<CannedTextSummary> cannedTexts = null;
			Platform.GetService<ICannedTextService>(
				service =>
				{
					var response = service.ListCannedTextForUser(new ListCannedTextForUserRequest { Name = this.Name });
					cannedTexts = response.CannedTexts;
				});

			// If updating an existing canned text, remove the one being edited from the list
			if (!_isNew)
			{
				CollectionUtils.Remove(cannedTexts, c => c.CannedTextRef.Equals(_cannedTextRef, true));
			}

			// Warn user if there are other cannedtext (for which the user has access to) with the same name
			if (cannedTexts.Count > 0)
			{
				var messageBuilder = new StringBuilder();

				messageBuilder.AppendLine(SR.MessageWarningDuplicateCannedTextName);
				messageBuilder.AppendLine();
				CollectionUtils.ForEach(cannedTexts,
										c =>
										messageBuilder.AppendLine(c.IsPersonal
																	? SR.ColumnPersonal
																	: c.StaffGroup.Name));

				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.YesNo))
					return false;
			}
			return true;
		}
	}
}
