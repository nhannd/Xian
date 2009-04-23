using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client
{
	public abstract class WorklistDetailEditorComponentBase : ApplicationComponent
	{
		private readonly List<WorklistClassSummary> _worklistClasses;
		private string[] _categoryChoices;
		private string _selectedCategory;
		private string _procedureTypeGroupClass;
		private event EventHandler _procedureTypeGroupClassChanged;

		public WorklistDetailEditorComponentBase(List<WorklistClassSummary> worklistClasses)
        {
            _worklistClasses = worklistClasses;
        }

		public override void Start()
		{
			_categoryChoices = CollectionUtils.Unique(
				CollectionUtils.Map<WorklistClassSummary, string>(_worklistClasses,
					delegate(WorklistClassSummary wc) { return wc.CategoryName; })).ToArray();

			if (_categoryChoices.Length > 0)
			{
				// set the selected category if there is no existing one, or the existing selection is invalid.
				if (string.IsNullOrEmpty(_selectedCategory) ||
					!CollectionUtils.Contains(_categoryChoices, delegate(string category) { return Equals(category, _selectedCategory); }))
				{
					_selectedCategory = _categoryChoices[0];
					UpdateWorklistClassChoices();
				}
			}

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
				}
			}
		}

		public IList WorklistClassChoices
		{
			get { return this.WorklistClassChoicesCore; }
		}

		#endregion 

		/// <summary>
		/// Gets the choices for the currently selected category.
		/// </summary>
		protected List<WorklistClassSummary> WorklistClassChoicesCore
		{
			get
			{
				return CollectionUtils.Select(_worklistClasses,
					delegate(WorklistClassSummary wc) { return wc.CategoryName == _selectedCategory; });
			}
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
			NotifyPropertyChanged("WorklistClassChoices");

			// all classes in subset should have the same procedureTypeGroupClass, so just grab the first one
			this.ProcedureTypeGroupClass = CollectionUtils.FirstElement(this.WorklistClassChoicesCore).ProcedureTypeGroupClassName;
		}
	}
}