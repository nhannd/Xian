using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class ComposedLut : IReferenceCountable
	{
		private int _referenceCount = 0;
		private readonly int[] _data;

		public ComposedLut(int numEntries)
		{
			_data = new int[numEntries];
		}

		public int[] Data
		{
			get { return _data; }	
		}

		#region IReferenceCountable Members

		public void IncrementReferenceCount()
		{
			_referenceCount++;
		}

		public void DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

		public bool IsReferenceCountZero
		{
			get { return (_referenceCount == 0); }
		}

		public int ReferenceCount
		{
			get { return _referenceCount; }
		}

		#endregion
	}
}
