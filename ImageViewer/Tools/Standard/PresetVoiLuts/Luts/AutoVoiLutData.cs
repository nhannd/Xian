using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using DataLut=ClearCanvas.ImageViewer.Imaging.DataLut;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	#region DataLut cache

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DataLutCacheTool : ImageViewerTool
	{
		private bool _disposed = false;

		private static int _referenceCount;
		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, WeakReference> _lutCache = new Dictionary<string, WeakReference>();

		public DataLutCacheTool()
		{
		}

		public override void Initialize()
		{
			lock(_syncLock)
			{
				++_referenceCount;
			}
			
			base.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			lock (_syncLock)
			{
				if (_disposed)
					return;

				_disposed = true;

				--_referenceCount;
				if (_referenceCount == 0)
					_lutCache.Clear();
			}

			base.Dispose(disposing);
		}

		internal static List<VoiDataLut> GetDataLuts(ImageSop sop)
		{
			lock(_syncLock)
			{
				WeakReference reference = null;
				List<VoiDataLut> dataLuts = null;

				if (_lutCache.ContainsKey(sop.SopInstanceUID))
				{
					reference = _lutCache[sop.SopInstanceUID];
					if (reference.IsAlive)
					{
						dataLuts = reference.Target as List<VoiDataLut>;
						if (dataLuts != null)
							return dataLuts;
					}
				}
				else
				{
					reference = new WeakReference(null);
					_lutCache[sop.SopInstanceUID] = reference;
				}

				dataLuts = VoiDataLut.Create(delegate(uint tag) { return sop[tag]; });
				_lutCache[sop.SopInstanceUID].Target = dataLuts;
				return dataLuts;
			}
		}
	}

	#endregion

	[Cloneable(true)]
	internal class AutoVoiLutData : DataLut
	{
		#region Memento

		private class AutoVoiLutDataMemento : IEquatable<AutoVoiLutDataMemento>
		{
			public readonly int Index;

			public AutoVoiLutDataMemento(int index)
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

				if (obj is AutoVoiLutDataMemento)
					return this.Equals((AutoVoiLutDataMemento)obj);

				return false;
			}

			#region IEquatable<AutoVoiLutDataMemento> Members

			public bool Equals(AutoVoiLutDataMemento other)
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
		private readonly List<VoiDataLut> _dataLuts;
		private readonly string _keyPrefix;
		private int _index;

		#endregion

		#region Constructor

		private AutoVoiLutData(List<VoiDataLut> dataLuts, string keyPrefix)
		{
			Platform.CheckForNullReference(dataLuts, "dataLuts");
			Platform.CheckPositive(dataLuts.Count, "dataLuts.Count");
			Platform.CheckForEmptyString(keyPrefix, "keyPrefix");

			_keyPrefix = keyPrefix;
			_dataLuts = dataLuts;
			_index = -1;

			ApplyNext();
		}
		
		private AutoVoiLutData()
		{
		}

		#endregion

		#region Private Methods

		private void SetIndex(int newIndex)
		{
			int lastIndex = _index;
			_index = newIndex;
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

		#endregion

		#region Public Properties

		public bool IsLast
		{
			get { return _index >= _dataLuts.Count - 1; }
		}

		public override int[] Data
		{
			get { return _dataLuts[_index].Data; }
		}

		#endregion

		#region Public Methods

		#region Statics

		public static bool CanCreateFrom(Frame frame)
		{
			if (frame.ParentImageSop.NumberOfFrames > 1)
			{
				//data luts don't apply to multi-frame images of any kind.
				return false;
			}

			DicomAttributeSQ voiLutSequence = (DicomAttributeSQ)frame.ParentImageSop[DicomTags.VoiLutSequence];
			return !voiLutSequence.IsEmpty && !voiLutSequence.IsNull;
		}

		public static AutoVoiLutData CreateFrom(Frame frame)
		{
			if (frame.ParentImageSop.NumberOfFrames > 1)
			{
				//data luts don't apply to multi-frame images of any kind.
				return null;
			}

			List<VoiDataLut> luts = DataLutCacheTool.GetDataLuts(frame.ParentImageSop);
			if (luts == null || luts.Count == 0)
				return null;

			foreach (VoiDataLut lut in luts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			string keyPrefix = String.Format("{0}:{1}", frame.ParentImageSop.SopInstanceUID, frame.FrameNumber);
			return new AutoVoiLutData(luts, keyPrefix);
		}

		#endregion

		public void ApplyNext()
		{
			SetIndex(_index + 1);
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

		public override object CreateMemento()
		{
			return new AutoVoiLutDataMemento(_index);
		}

		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			AutoVoiLutDataMemento lutMemento = memento as AutoVoiLutDataMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(AutoVoiLutDataMemento).FullName);

			SetIndex(lutMemento.Index);
		}

		#endregion
	}
}
