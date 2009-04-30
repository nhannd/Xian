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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	[Cloneable(true)]
	internal abstract class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLut
	{
		#region Memento

		private class AutoVoiLutLinearMemento : IEquatable<AutoVoiLutLinearMemento>
		{
			public readonly int Index;
			public readonly string Name;

			public AutoVoiLutLinearMemento(string name, int index)
			{
				this.Name = name;
				this.Index = index;
			}

			public override int GetHashCode()
			{
				return this.Name.GetHashCode() ^ this.Index.GetHashCode() ^ 0x09bf0923;
			}

			public override bool Equals(object obj)
			{
				if (obj is AutoVoiLutLinearMemento)
					return this.Equals((AutoVoiLutLinearMemento) obj);
				return false;
			}

			public bool Equals(AutoVoiLutLinearMemento other)
			{
				return other != null && this.Name == other.Name && this.Index == other.Index;
			}
		}

		#endregion

		#region Private Fields

		[CloneCopyReference]
		private readonly IList<Window> _windows;

		[CloneCopyReference]
		private readonly IList<string> _explanations;

		private int _index;

		#endregion

		#region Constructors

		protected AutoVoiLutLinear(IList<Window> windows, IList<string> explanations)
		{
			_windows = windows;
			_explanations = explanations;
			_index = 0;
		}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		protected AutoVoiLutLinear() {}

		#endregion

		#region Protected Properties

		protected int Index
		{
			get { return _index; }
			set
			{
				int lastIndex = _index;
				_index = value;
				if (_index >= _windows.Count)
					_index = 0;

				if (lastIndex != _index)
					base.OnLutChanged();
			}
		}

		#endregion

		#region Public Properties/Methods

		public abstract string Name { get; }

		public override sealed double WindowWidth
		{
			get { return _windows[_index].Width; }
		}

		public override sealed double WindowCenter
		{
			get { return _windows[_index].Center; }
		}

		public bool IsLast
		{
			get { return _index >= _windows.Count - 1; }
		}

		public void ApplyNext()
		{
			this.Index = _index + 1;
		}

		public override sealed string GetDescription()
		{
			if (_explanations == null || _index >= _explanations.Count)
				return String.Format(SR.FormatDescriptionAutoLinearLutNoExplanation, WindowWidth, WindowCenter);
			else
				return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, _explanations[_index]);
		}

		public override sealed object CreateMemento()
		{
			return new AutoVoiLutLinearMemento(this.Name, this.Index);
		}

		public override sealed void SetMemento(object memento)
		{
			AutoVoiLutLinearMemento autoMemento = (AutoVoiLutLinearMemento) memento;
			Platform.CheckTrue(this.Name == autoMemento.Name, "Memento has a different creator.");
			this.Index = autoMemento.Index;
		}

		#endregion
	}

	[Cloneable(true)]
	internal sealed class AutoImageVoiLutLinear : AutoVoiLutLinear
	{
		private readonly string _name = "AutoImageVoiLutLinear";
		private AutoImageVoiLutLinear(IList<Window> windows, IList<string> explanations) : base(windows, explanations) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoImageVoiLutLinear() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.ImageVoiLinearLuts.Count > 0;
		}

		public static AutoImageVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			if (luts.ImageVoiLinearLuts.Count > 0)
				return new AutoImageVoiLutLinear(luts.ImageVoiLinearLuts, luts.ImageVoiLinearLutExplanations);
			return null;
		}
	}

	[Cloneable(true)]
	internal sealed class AutoPresentationVoiLutLinear : AutoVoiLutLinear
	{
		private readonly string _name = "AutoPresentationVoiLutLinear";

		private AutoPresentationVoiLutLinear(IList<Window> windows, IList<string> explanations) : base(windows, explanations) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoPresentationVoiLutLinear() : base() {}

		public override string Name
		{
			get { return _name; }
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.PresentationVoiLinearLuts.Count > 0;
		}

		public static AutoPresentationVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			if (luts.PresentationVoiLinearLuts.Count > 0)
				return new AutoPresentationVoiLutLinear(luts.PresentationVoiLinearLuts, luts.PresentationVoiLinearLutExplanations);
			return null;
		}
	}
}