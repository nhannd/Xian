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

	[Cloneable]
	internal class AdjustableAutoVoiDataLut : AdjustableDataLut, IAutoVoiLut
	{
		public AdjustableAutoVoiDataLut(AutoVoiDataLut lut) : base(lut) {}
		protected AdjustableAutoVoiDataLut(AdjustableAutoVoiDataLut source, ICloningContext context) 
			: base(source, context)
		{
		}

		public bool IsLast
		{
			get { return ((AutoVoiDataLut) base.DataLut).IsLast; }
		}

		public void ApplyNext()
		{
			((AutoVoiDataLut) base.DataLut).ApplyNext();
			this.Reset();
		}
	}

	[Cloneable(true)]
	internal abstract class AutoVoiDataLut : DataLut, IAutoVoiLut
	{
		#region Memento

		private class AutoVoiDataLutMemento : IEquatable<AutoVoiDataLutMemento>
		{
			public readonly int Index;
			public readonly string Name;

			public AutoVoiDataLutMemento(string name, int index)
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
				if (obj is AutoVoiDataLutMemento)
					return this.Equals((AutoVoiDataLutMemento) obj);
				return false;
			}

			public bool Equals(AutoVoiDataLutMemento other)
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

		protected AutoVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix)
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
		protected AutoVoiDataLut() {}

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

		public string Explanation
		{
			get { return _dataLuts[_index].Explanation; }
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
			string name = Explanation;
			if (String.IsNullOrEmpty(name))
				name = String.Format("{0}{1}", SR.PrefixDefaultVoiDataLutExplanation, _index + 1);

			return String.Format(SR.FormatAutoVoiDataLutDescription, name);
		}

		public override sealed object CreateMemento()
		{
			return new AutoVoiDataLutMemento(this.Name, this.Index);
		}

		public override sealed void SetMemento(object memento)
		{
			AutoVoiDataLutMemento lutMemento = (AutoVoiDataLutMemento) memento;
			Platform.CheckTrue(this.Name == lutMemento.Name, "Memento has a different creator.");
			this.Index = lutMemento.Index;
		}

		#endregion
	}

	[Cloneable(true)]
	internal sealed class AutoImageVoiDataLut : AutoVoiDataLut
	{
		private readonly string _name = "AutoImageVoiDataLut";
		private AutoImageVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoImageVoiDataLut() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.ImageVoiDataLuts.Count > 0;
		}

		public static AutoImageVoiDataLut CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.ImageVoiDataLuts.Count > 0)
				dataLuts = luts.ImageVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			return new AutoImageVoiDataLut(dataLuts, string.Format("{0}:{1}", provider.DicomVoiLuts.ImageSopInstanceUid, provider.DicomVoiLuts.ImageSopFrameNumber));
		}
	}

	[Cloneable(true)]
	internal sealed class AutoPresentationVoiDataLut : AutoVoiDataLut
	{
		private readonly string _name = "AutoPresentationVoiDataLut";
		private AutoPresentationVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoPresentationVoiDataLut() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.PresentationVoiDataLuts.Count > 0;
		}

		public override string GetDescription()
		{
			string name = base.Explanation;
			if (String.IsNullOrEmpty(name))
				name = SR.LabelPresentationStateVoiDataLut;

			return String.Format(SR.FormatAutoVoiDataLutDescription, name);
		}

		public static AutoPresentationVoiDataLut CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.PresentationVoiDataLuts.Count > 0)
				dataLuts = luts.PresentationVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			return new AutoPresentationVoiDataLut(dataLuts, provider.DicomVoiLuts.PresentationStateSopInstanceUid);
		}
	}
}