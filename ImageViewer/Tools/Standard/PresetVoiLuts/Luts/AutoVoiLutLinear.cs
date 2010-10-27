#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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
		private readonly IList<VoiWindow> _windows;

		private int _index;

		#endregion

		#region Constructors

		protected AutoVoiLutLinear(IList<VoiWindow> windows)
		{
			_windows = windows;
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

		public string Explanation
		{
			get { return _windows[_index].Explanation; }	
		}

		public bool IsLast
		{
			get { return _index >= _windows.Count - 1; }
		}

		public void ApplyNext()
		{
			this.Index = _index + 1;
		}

		public override string GetDescription()
		{
			if (string.IsNullOrEmpty(Explanation))
				return String.Format(SR.FormatDescriptionAutoLinearLutNoExplanation, WindowWidth, WindowCenter);
			else
				return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, Explanation);
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
		private AutoImageVoiLutLinear(IList<VoiWindow> windows) : base(windows) {}

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
				return new AutoImageVoiLutLinear(luts.ImageVoiLinearLuts);
			return null;
		}
	}

	[Cloneable(true)]
	internal sealed class AutoPresentationVoiLutLinear : AutoVoiLutLinear
	{
		private readonly string _name = "AutoPresentationVoiLutLinear";

		private AutoPresentationVoiLutLinear(IList<VoiWindow> windows) : base(windows) {}

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

		public override sealed string GetDescription()
		{
			if (string.IsNullOrEmpty(Explanation))
				return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, SR.LabelPresentationStateVoiLinearLut);
			else
				return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, Explanation);
		}

		public static AutoPresentationVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			if (luts.PresentationVoiLinearLuts.Count > 0)
				return new AutoPresentationVoiLutLinear(luts.PresentationVoiLinearLuts);
			return null;
		}
	}
}