using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM SOP.
	/// </summary>
	public abstract class Sop : ICacheableSop
	{
		private int _referenceCount;
		private Series _parentSeries;

		/// <summary>
		/// Gets the parent <see cref="Series"/>.
		/// </summary>
		public Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		/// <summary>
		/// Gets or sets the SOP Instance UID.
		/// </summary>
		public abstract string SopInstanceUID { get; set; }

		/// <summary>
		/// Gets or sets the Transfer Syntax UID.
		/// </summary>
		public abstract string TransferSyntaxUID { get; set; }

		public override string ToString()
		{
			return this.SopInstanceUID;
		}

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion

		#region ICacheableSop Members

		string ICacheableSop.SopInstanceUID
		{
			get { return this.SopInstanceUID; }
		}

		bool ICacheableSop.IsReferenceCountZero
		{
			get { return (_referenceCount == 0); }
		}

		void ICacheableSop.Load()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void ICacheableSop.Unload()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void ICacheableSop.IncrementReferenceCount()
		{
			_referenceCount++;
		}

		void ICacheableSop.DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

#if UNIT_TESTS
		int ICacheableSop.ReferenceCount
		{
			get { return _referenceCount; }
		}
#endif

		#endregion
	}
}
