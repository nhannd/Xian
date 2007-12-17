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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="ImageSop"/>.
	/// </summary>
	public class LocalImageSop : ImageSop
	{
		private delegate void GetTagDelegate<T>(DicomAttribute attribute, uint position, out T value);
 
		private DicomFile _dicomFile;
		private byte[] _pixelData;
		private bool _loaded;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalImageSop"/> with
		/// a specified filename.
		/// </summary>
		/// <param name="filename"></param>
		public LocalImageSop(string filename)
		{
			_dicomFile = new DicomFile(filename);
			_loaded = false;
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected override void Dispose(bool disposing)
		{
			_dicomFile = null;
			_pixelData = null;
			_loaded = false;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the DICOM toolkit representation of this <see cref="LocalImageSop"/>.
		/// </summary>
        public override DicomMessageBase NativeDicomObject
        {
            get { return _dicomFile; }
        }
		
		/// <summary>
		/// This method overrides <see cref="ImageSop.GetNormalizedPixelData"/>.
		/// </summary>
		/// <returns></returns>
		public override byte[] GetNormalizedPixelData()
		{
			Load();
			
			if (_pixelData == null)
			{
				byte[] pixelData = (byte[])_dicomFile.DataSet[DicomTags.PixelData].Values;
				_pixelData = NormalizePixelData(pixelData);
			}

			return _pixelData;
		}

		/// <summary>
		/// This method overides <see cref="Sop.GetTag(uint, out ushort, out bool)"/>.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out ushort value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out ushort, uint, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out ushort value, uint position, out bool tagExists)
		{
			GetTag<ushort>(tag, out value, position, out tagExists, GetUint16FromAttribute);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out int, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out int value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out int, uint, out bool)"/>.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out int value, uint position, out bool tagExists)
		{
			GetTag<int>(tag, out value, position, out tagExists, GetInt32FromAttribute);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out double, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out double value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out double, uint, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out double value, uint position, out bool tagExists)
		{
			GetTag<double>(tag, out value, position, out tagExists, GetFloat64FromAttribute);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out string, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out string value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTag(uint, out string, uint, out bool)"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public override void GetTag(uint tag, out string value, uint position, out bool tagExists)
		{
			GetTag<string>(tag, out value, position, out tagExists, GetStringFromAttribute);
			value = value ?? "";
		}

		/// <summary>
		/// This method overrides <see cref="Sop.GetTagAsDicomStringArray"/>
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		public override void GetTagAsDicomStringArray(uint tag, out string value, out bool tagExists)
		{
			GetTag<string>(tag, out value, 0, out tagExists, GetStringArrayFromAttribute);
			value = value ?? "";
		}

		private void GetUint16FromAttribute(DicomAttribute attribute, uint position, out ushort value)
		{
			attribute.TryGetUInt16((int)position, out value);
		}

		private void GetInt32FromAttribute(DicomAttribute attribute, uint position, out int value)
		{
			attribute.TryGetInt32((int)position, out value);
		}

		private void GetFloat64FromAttribute(DicomAttribute attribute, uint position, out double value)
		{
			attribute.TryGetFloat64((int)position, out value);
		}

		private void GetStringFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			attribute.TryGetString((int)position, out value);
		}

		private void GetStringArrayFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			value = attribute.ToString();
		}

		private void GetTag<T>(uint tag, out T value, uint position, out bool tagExists, GetTagDelegate<T> getter)
		{
			Load();
			value = default(T);
			tagExists = false;

			DicomAttribute dicomAttribute;
			if(_dicomFile.DataSet.Contains(tag))
			{
				dicomAttribute = _dicomFile.DataSet[tag];
				tagExists = !dicomAttribute.IsEmpty && dicomAttribute.Count > position;
				if (tagExists)
				{
					getter(dicomAttribute, position, out value);
					return;
				}
			}

			if (_dicomFile.MetaInfo.Contains(tag))
			{
				dicomAttribute = _dicomFile.MetaInfo[tag];
				tagExists = !dicomAttribute.IsEmpty && dicomAttribute.Count > position;
				if (tagExists)
					getter(dicomAttribute, position, out value);
			}
		}

		private void Load()
		{
			if (_loaded)
				return;

			_loaded = true;
			_dicomFile.Load(DicomReadOptions.Default);
		}
	}
}
