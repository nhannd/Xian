using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class OutputLut : IReferenceCountable
	{
		private int _referenceCount = 0;
		private int[] _outputLut;
		private string _key;

		public OutputLut(string key, int numEntries)
		{
			_key = key;
			_outputLut = new int[numEntries];
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public int[] Lut
		{
			get { return _outputLut; }
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
