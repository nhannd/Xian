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
		/// Gets the lookup Handler for duplciate item.
		/// </summary>
		public abstract ILookupHandler DuplicateLookupHandler { get; }

		/// <summary>
		/// Gets the lookup Handler for original item.
		/// </summary>
		public abstract ILookupHandler OriginalLookupHandler { get; }

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
	}

	public abstract class MergeComponentBase<TSummary> : MergeComponentBase
		where TSummary : DataContractBase
	{
		private TSummary _selectedDuplicate;
		private TSummary _selectedOriginal;
		private string _mergeReport;

		public MergeComponentBase()
			: this(null, null)
		{
		}

		public MergeComponentBase(TSummary duplicate, TSummary original)
		{
			_selectedDuplicate = duplicate;
			_selectedOriginal = original;

			this.Validation.Add(new ValidationRule("SelectedOriginal",
				delegate
				{
					bool isIdentical = IsSameItem(_selectedDuplicate, _selectedOriginal);
					return new ValidationResult(!isIdentical, SR.MessageMergeIdenticalItems);
				}));
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

					if (string.IsNullOrEmpty(this.MergeReport))
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
