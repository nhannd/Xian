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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Desktop.Validation;
using System.Collections;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistDetailEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistDetailEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistDetailEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistDetailEditorComponentViewExtensionPoint))]
    public class WorklistDetailEditorComponent : WorklistDetailEditorComponentBase
    {
        private readonly WorklistAdminDetail _worklistDetail;
        private readonly bool _dialogMode;
    	private readonly bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
		public WorklistDetailEditorComponent(WorklistAdminDetail detail, List<WorklistClassSummary> worklistClasses, bool dialogMode, bool isNew)
			:base(worklistClasses)
        {
            _worklistDetail = detail;
            _dialogMode = dialogMode;
        	_isNew = isNew;
        }

		public override void Start()
		{
			WorklistClassSummary wc = _worklistDetail.WorklistClass;

			base.Start();

			if (wc != null)
			{
				this.SelectedCategory = wc.CategoryName;
			}
			_worklistDetail.WorklistClass = wc;
		}

    	#region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _worklistDetail.Name; }
            set
            {
                _worklistDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _worklistDetail.Description; }
            set
            {
                _worklistDetail.Description = value;
                this.Modified = true;
            }
        }

    	public bool IsWorklistClassReadOnly
    	{
			get { return !_isNew; }
    	}

		[ValidateNotNull]
		public WorklistClassSummary WorklistClass
    	{
			get { return _worklistDetail.WorklistClass; }
			set
			{
				if(!Equals(_worklistDetail.WorklistClass, value))
				{
					_worklistDetail.WorklistClass = value;
					this.Modified = true;
					NotifyPropertyChanged("WorklistClass");
				}
			}
    	}

		public string FormatWorklistClass(object item)
		{
			WorklistClassSummary summary = (WorklistClassSummary) item;
			return summary.DisplayName;
		}

        public string WorklistClassDescription
        {
            get
            {
                return _worklistDetail.WorklistClass.Description;
            }
        }

        public bool AcceptButtonVisible
        {
            get { return _dialogMode; }
        }

        public bool CancelButtonVisible
        {
            get { return _dialogMode; }
        }

        public void Accept()
        {
            Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            Exit(ApplicationComponentExitCode.None);
        }

        #endregion

		protected override void UpdateWorklistClassChoices()
		{
			// blank out the selected worklist class
			_worklistDetail.WorklistClass = null;

			base.UpdateWorklistClassChoices();
		}

	}
}
