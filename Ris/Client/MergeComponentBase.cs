#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="MergeComponentBase"/>
	/// </summary>
	[ExtensionPoint]
	public class MergeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// Abstract base class for merge components.
	/// </summary>
	[AssociateView(typeof(MergeComponentViewExtensionPoint))]
	public abstract class MergeComponentBase : ApplicationComponent
	{
		#region Presentation Model

		/// <summary>
		/// Gets a list of source items
		/// </summary>
		public abstract IList SourceItems { get; }

		/// <summary>
		/// Gets a list of target items
		/// </summary>
		public abstract IList TargetItems { get; }

		/// <summary>
		/// Gets and sets thecurrently selected duplicate item.
		/// </summary>
		public abstract object SelectedDuplicate { get; set;}

		/// <summary>
		/// Gets and sets the currently selected original item.
		/// </summary>
		public abstract object SelectedOriginal { get; set;}

		/// <summary>
		/// Gets the report if the merge is to take place.
		/// </summary>
		public abstract string MergeReport { get; set; }

		/// <summary>
		/// Handles the accept button.
		/// </summary>
		public abstract void Accept();

		/// <summary>
		/// Handles the cancel button.
		/// </summary>
		public abstract void Cancel();

		/// <summary>
		/// Switch duplicate and original.
		/// </summary>
		public abstract void Switch();

		#endregion

		/// <summary>
		/// Formats an item in either the <see mref="SourceItems"/> collection or <see mref="TargetItems"/> collection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public abstract object FormatItem(object p);
	}

	public abstract class MergeComponentBase<TSummary> : MergeComponentBase
		where TSummary : DataContractBase
	{
		private TSummary _selectedDuplicate;
		private TSummary _selectedOriginal;
		private string _mergeReport;
		private readonly IList<TSummary> _items;

		protected MergeComponentBase(IList<TSummary> items)
		{
			_items = items;

			this.Validation.Add(new ValidationRule("SelectedOriginal",
				delegate
				{
					bool isIdentical = IsSameItem(_selectedDuplicate, _selectedOriginal);
					return new ValidationResult(!isIdentical, SR.MessageMergeIdenticalItems);
				}));
		}

		public override IList SourceItems
		{
			get { return new List<TSummary>(_items); }
		}

		public override IList TargetItems
		{
			get { return new List<TSummary>(_items); }
		}

		public override void Start()
		{
			ShowReport();

			base.Start();
		}

		#region Abstract/overridable members

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract bool IsSameItem(TSummary x, TSummary y);

		/// <summary>
		/// Generate a report for the merge.
		/// </summary>
		/// <param name="duplicate"></param>
		/// <param name="original"></param>
		/// <returns></returns>
		protected abstract string GenerateReport(TSummary duplicate, TSummary original);

		#endregion

		#region Presentation Model

		public TSummary SelectedDuplicateSummary
		{
			get { return _selectedDuplicate; }
		}

		[ValidateNotNull]
		public override object SelectedDuplicate
		{
			get { return _selectedDuplicate; }
			set
			{
				if (_selectedDuplicate != value)
				{
					_selectedDuplicate = (TSummary) value;
					NotifyPropertyChanged("SelectedDuplicate");

					ShowReport();
				}
			}
		}

		public TSummary SelectedOriginalSummary
		{
			get { return _selectedOriginal; }
		}

		[ValidateNotNull]
		public override object SelectedOriginal
		{
			get { return _selectedOriginal; }
			set
			{
				if (_selectedOriginal != value)
				{
					_selectedOriginal = (TSummary) value;
					NotifyPropertyChanged("SelectedOriginal");

					ShowReport();
				}
			}
		}

		public override string MergeReport
		{
			get { return _mergeReport; }
			set
			{
				_mergeReport = value;
				NotifyPropertyChanged("MergeReport");
			}
		}

		public override void Accept()
		{
			this.ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		public override void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		public override void Switch()
		{
			object temp = this.SelectedOriginal;
			this.SelectedOriginal = this.SelectedDuplicate;
			this.SelectedDuplicate = temp;
		}

		#endregion

		private void ShowReport()
		{
			this.MergeReport = _selectedDuplicate == null || _selectedOriginal == null || IsSameItem(_selectedDuplicate, _selectedOriginal)
				? string.Empty
				: GenerateReport(_selectedDuplicate, _selectedOriginal);
		}
	}
}
