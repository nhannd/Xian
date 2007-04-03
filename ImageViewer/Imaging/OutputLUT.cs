using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class OutputLUT : IReferenceCountable
	{
		private int _referenceCount = 0;
		private int[] _outputLUT;
		private string _key;

		public OutputLUT(string key, int numEntries)
		{
			_key = key;
			_outputLUT = new int[numEntries];
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public int[] LUT
		{
			get { return _outputLUT; }
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
