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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	public abstract class WorklistDetailEditorComponentBase : ApplicationComponent
	{
		private readonly List<WorklistClassSummary> _worklistClasses;
		private readonly WorklistClassSummary _initialClass;
		private string[] _categoryChoices;
		private string _selectedCategory;
		private string _procedureTypeGroupClass;
		private event EventHandler _procedureTypeGroupClassChanged;
		private event EventHandler _worklistClassChanged;
		private event EventHandler _worklistCategoryChanged;

		public WorklistDetailEditorComponentBase(List<WorklistClassSummary> worklistClasses, WorklistClassSummary initialClass)
        {
            _worklistClasses = worklistClasses;
			_initialClass = initialClass ?? CollectionUtils.FirstElement(_worklistClasses);
		}

		public override void Start()
		{
			// get unique category choices, sorted alphabetically
			_categoryChoices = CollectionUtils.Sort(
									CollectionUtils.Unique(
										CollectionUtils.Map<WorklistClassSummary, string>(_worklistClasses,
										delegate(WorklistClassSummary wc) { return wc.CategoryName; }))).ToArray();

			// determine initial selections
			if (_initialClass != null)
			{
				_selectedCategory = _initialClass.CategoryName;
			}

			UpdateWorklistClassChoices();

			base.Start();
		}

		#region Presentation Model

		public string ProcedureTypeGroupClass
		{
			get { return _procedureTypeGroupClass; }
			private set
			{
				if(_procedureTypeGroupClass != value)
				{
					_procedureTypeGroupClass = value;
					EventsHelper.Fire(_procedureTypeGroupClassChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ProcedureTypeGroupClassChanged
		{
			add { _procedureTypeGroupClassChanged += value; }
			remove { _procedureTypeGroupClassChanged -= value; }
		}

		public event EventHandler WorklistClassChanged
		{
			add { _worklistClassChanged += value; }
			remove { _worklistClassChanged -= value; }
		}

		public string[] CategoryChoices
		{
			get { return _categoryChoices; }
		}

		public string SelectedCategory
		{
			get { return _selectedCategory; }
			set
			{
				if(value != _selectedCategory)
				{
					_selectedCategory = value;
					this.Modified = true;
					UpdateWorklistClassChoices();
					NotifyPropertyChanged("SelectedCategory");
					NotifyWorklistCategoryChanged();
				}
			}
		}

		public event EventHandler WorklistCategoryChanged
		{
			add { _worklistCategoryChanged += value; }
			remove { _worklistCategoryChanged -= value; }
		}

		public IList WorklistClassChoices
		{
			get { return this.GetWorklistClassChoicesForCategory(_selectedCategory); }
		}

		#endregion

		/// <summary>
		/// Gets the choices for the currently selected category.
		/// </summary>
		protected List<WorklistClassSummary> GetWorklistClassChoicesForCategory(string category)
		{
			return CollectionUtils.Select(_worklistClasses,
					  delegate(WorklistClassSummary wc) { return wc.CategoryName == category; });
		}

		/// <summary>
		/// Gets entire list of worklist classes.
		/// </summary>
		protected List<WorklistClassSummary> WorklistClasses
		{
			get { return _worklistClasses; }
		}

		protected virtual void UpdateWorklistClassChoices()
		{
			// update stored setting, but don't save yet
			WorklistEditorComponentSettings.Default.DefaultWorklistCategory = _selectedCategory;

			NotifyPropertyChanged("WorklistClassChoices");
			NotifyWorklistClassChanged();

			// all classes in subset should have the same procedureTypeGroupClass, so just grab the first one
			// use property rather than member var, so that the event is fired!
			this.ProcedureTypeGroupClass = CollectionUtils.FirstElement(this.GetWorklistClassChoicesForCategory(_selectedCategory)).ProcedureTypeGroupClassName;
		}

		protected void NotifyWorklistClassChanged()
		{
			EventsHelper.Fire(_worklistClassChanged, this, EventArgs.Empty);
		}

		protected void NotifyWorklistCategoryChanged()
		{
			EventsHelper.Fire(_worklistCategoryChanged, this, EventArgs.Empty);
		}
	}
}