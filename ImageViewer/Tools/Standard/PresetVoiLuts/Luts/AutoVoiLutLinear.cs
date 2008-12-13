#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	[Cloneable(true)]
	internal sealed class AutoVoiLutLinear : CalculatedVoiLutLinear
	{
		#region Memento

		private class AutoVoiLutLinearMemento : IEquatable<AutoVoiLutLinearMemento>
		{
			public readonly int Index;

			public AutoVoiLutLinearMemento(int index)
			{
				this.Index = index;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is AutoVoiLutLinearMemento)
					return this.Equals((AutoVoiLutLinearMemento) obj);

				return false;	
			}

			#region IEquatable<AutoVoiLutLinearMemento> Members

			public bool Equals(AutoVoiLutLinearMemento other)
			{
				if (other == null)
					return false;

				return this.Index == other.Index;
			}

			#endregion
		}

		#endregion

		#region Private Fields

		[CloneCopyReference]
		private readonly IList<Window> _windows;
		[CloneCopyReference]
		private readonly IList<string> _explanations;
		private int _index;

		#endregion

		#region Constructor

		private AutoVoiLutLinear(IList<Window> windows, IList<string> explanations)
		{
			_windows = windows;
			_explanations = explanations;
			_index = 0;
		}

		private AutoVoiLutLinear()
		{
		}

		#endregion

		#region Private Methods

		private void SetIndex(int newIndex)
		{
			int lastIndex = _index;
			_index = newIndex;
			if (_index >= _windows.Count)
				_index = 0;

			if (lastIndex != _index)
				base.OnLutChanged();
		}

		private static Window[] GetWindowCenterAndWidth(Frame frame)
		{
			Window[] windowCenterAndWidth = frame.WindowCenterAndWidth;
			if (windowCenterAndWidth == null || windowCenterAndWidth.Length == 0)
				return null;

			return windowCenterAndWidth;
		}

		#endregion

		#region Public Properties/Methods

		public override double WindowWidth
		{
			get { return _windows[_index].Width; }
		}

		public override double WindowCenter
		{
			get { return _windows[_index].Center; }
		}

		public bool IsLast
		{
			get { return _index >= _windows.Count - 1; }
		}

		#region Statics
		
		public static bool CanCreateFrom(Frame frame)
		{
			return GetWindowCenterAndWidth(frame) != null;
		}

		public static AutoVoiLutLinear CreateFrom(Frame frame)
		{
			Window[] windowCenterAndWidth = GetWindowCenterAndWidth(frame);
			if (windowCenterAndWidth == null)
				return null;

			string[] explanations = frame.WindowCenterAndWidthExplanation;
			if (explanations == null || explanations.Length != windowCenterAndWidth.Length)
				return new AutoVoiLutLinear(new List<Window>(windowCenterAndWidth), null);
			else
				return new AutoVoiLutLinear(new List<Window>(windowCenterAndWidth), explanations);
		}

		#endregion
		public void ApplyNext()
		{
			SetIndex(_index + 1);
		}
		
		public override string GetDescription()
		{
			if (_explanations == null)
				return String.Format(SR.FormatDescriptionAutoLinearLutNoExplanation, WindowWidth, WindowCenter);
			else 
				return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, _explanations[_index]);
		}

		public override object CreateMemento()
		{
			return new AutoVoiLutLinearMemento(_index);
		}

		public override void SetMemento(object memento)
		{
			AutoVoiLutLinearMemento autoMemento = memento as AutoVoiLutLinearMemento;
			Platform.CheckForInvalidCast(autoMemento, "memento", typeof(AutoVoiLutLinearMemento).Name);
			this.SetIndex(autoMemento.Index);
		}

		#endregion
	}
}
