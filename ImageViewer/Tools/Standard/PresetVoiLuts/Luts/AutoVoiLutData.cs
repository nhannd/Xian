using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;
using DataLut=ClearCanvas.ImageViewer.Imaging.DataLut;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	internal interface IAutoVoiLut : IComposableLut
	{
		bool IsLast { get; }
		void ApplyNext();
	}

	[Cloneable(true)]
	internal class AdjustableAutoVoiDataLut : AdjustableDataLut, IAutoVoiLut
	{
		public AdjustableAutoVoiDataLut(AutoVoiLutData lut) : base(lut) {}
		private AdjustableAutoVoiDataLut() : base() {}

		public bool IsLast
		{
			get { return ((AutoVoiLutData) base.DataLut).IsLast; }
		}

		public void ApplyNext()
		{
			((AutoVoiLutData) base.DataLut).ApplyNext();
			this.Reset();
		}
	}

	[Cloneable(true)]
	internal abstract class AutoVoiLutData : DataLut, IAutoVoiLut
	{
		#region Memento

		private class AutoVoiLutDataMemento : IEquatable<AutoVoiLutDataMemento>
		{
			public readonly int Index;
			public readonly string Name;

			public AutoVoiLutDataMemento(string name, int index)
			{
				this.Name = name;
				this.Index = index;
			}

			public override int GetHashCode()
			{
				return this.Index.GetHashCode() ^ this.Name.GetHashCode() ^ 0x589bf89d;
			}

			public override bool Equals(object obj)
			{
				if (obj is AutoVoiLutDataMemento)
					return this.Equals((AutoVoiLutDataMemento) obj);
				return false;
			}

			public bool Equals(AutoVoiLutDataMemento other)
			{
				return other != null && this.Name == other.Name && this.Index == other.Index;
			}
		}

		#endregion

		#region Private Fields

		[CloneCopyReference]
		private readonly IList<VoiDataLut> _dataLuts;

		private readonly string _keyPrefix;
		private int _index;

		#endregion

		#region Constructors

		protected AutoVoiLutData(IList<VoiDataLut> dataLuts, string keyPrefix)
		{
			Platform.CheckForNullReference(dataLuts, "dataLuts");
			Platform.CheckPositive(dataLuts.Count, "dataLuts.Count");
			Platform.CheckForEmptyString(keyPrefix, "keyPrefix");

			_keyPrefix = keyPrefix;
			_dataLuts = dataLuts;
			_index = -1;

			ApplyNext();
		}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		protected AutoVoiLutData() {}

		#endregion

		#region Protected Properties

		protected int Index
		{
			get { return _index; }
			set
			{
				int lastIndex = _index;
				_index = value;
				if (_index >= _dataLuts.Count)
					_index = 0;

				if (lastIndex != _index)
				{
					VoiDataLut lut = _dataLuts[_index];
					base.MinInputValue = lut.FirstMappedPixelValue;
					base.MaxInputValue = lut.LastMappedPixelValue;
					base.MinOutputValue = lut.MinOutputValue;
					base.MaxOutputValue = lut.MaxOutputValue;

					base.OnLutChanged();
				}
			}
		}

		#endregion

		#region Public Properties/Methods

		public abstract string Name { get; }

		public bool IsLast
		{
			get { return _index >= _dataLuts.Count - 1; }
		}

		public override int FirstMappedPixelValue
		{
			get { return _dataLuts[_index].FirstMappedPixelValue; }
		}

		public override int LastMappedPixelValue
		{
			get { return _dataLuts[_index].LastMappedPixelValue; }
		}

		public override sealed int[] Data
		{
			get { return _dataLuts[_index].Data; }
		}

		public void ApplyNext()
		{
			this.Index = _index + 1;
		}

		public override string GetKey()
		{
			return String.Format("{0}:VOIDATA:{1}", _keyPrefix, _index);
		}

		public override string GetDescription()
		{
			string name = _dataLuts[_index].Explanation;
			if (String.IsNullOrEmpty(name))
				name = "LUT" + _index;

			return String.Format(SR.FormatAutoVoiLutDataDescription, _dataLuts[_index].Explanation);
		}

		//TODO: override min/max input.

		public override sealed object CreateMemento()
		{
			return new AutoVoiLutDataMemento(this.Name, this.Index);
		}

		public override sealed void SetMemento(object memento)
		{
			AutoVoiLutDataMemento lutMemento = (AutoVoiLutDataMemento) memento;
			Platform.CheckTrue(this.Name == lutMemento.Name, "Memento has a different creator.");
			this.Index = lutMemento.Index;
		}

		#endregion
	}

	[Cloneable(true)]
	internal sealed class AutoImageVoiLutData : AutoVoiLutData
	{
		private readonly string _name = "AutoImageVoiLutData";
		private AutoImageVoiLutData(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoImageVoiLutData() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.ImageVoiDataLuts.Count > 0;
		}

		public static AutoImageVoiLutData CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.ImageVoiDataLuts.Count > 0)
				dataLuts = luts.ImageVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			return new AutoImageVoiLutData(dataLuts, string.Format("{0}:{1}", provider.DicomVoiLuts.ImageSopInstanceUid, provider.DicomVoiLuts.ImageSopFrameNumber));
		}
	}

	[Cloneable(true)]
	internal sealed class AutoPresentationVoiLutData : AutoVoiLutData
	{
		private readonly string _name = "AutoPresentationVoiLutData";
		private AutoPresentationVoiLutData(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoPresentationVoiLutData() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.PresentationVoiDataLuts.Count > 0;
		}

		public static AutoPresentationVoiLutData CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.PresentationVoiDataLuts.Count > 0)
				dataLuts = luts.PresentationVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			return new AutoPresentationVoiLutData(dataLuts, provider.DicomVoiLuts.PresentationStateSopInstanceUid);
		}
	}
}