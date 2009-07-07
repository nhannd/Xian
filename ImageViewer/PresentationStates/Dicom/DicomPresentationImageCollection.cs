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
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	internal class DicomPresentationImageCollection<T> : IEnumerable<T> where T : IDicomPresentationImage
	{
		private readonly List<T> _images;
		private string _studyUid = null;
		private Dictionary<string, List<T>> _dictionary = null;

		public DicomPresentationImageCollection()
		{
			_images = new List<T>();
		}

		public DicomPresentationImageCollection(IEnumerable<T> images)
		{
			_images = new List<T>(images);

			if (_images.Count > 0)
				_studyUid = _images[0].ImageSop.StudyInstanceUID;
		}

		private Dictionary<string, List<T>> Dictionary
		{
			get
			{
				if (_dictionary == null)
				{
					_dictionary = new Dictionary<string, List<T>>();
					foreach (T image in _images)
					{
						string seriesUid = image.ImageSop.SeriesInstanceUID;
						if (!_dictionary.ContainsKey(seriesUid))
							_dictionary.Add(seriesUid, new List<T>());
						_dictionary[seriesUid].Add(image);
					}
				}
				return _dictionary;
			}
		}

		public void Add(T image)
		{
			if (_dictionary != null)
				throw new InvalidOperationException();
			if (_studyUid != null && _studyUid != image.ImageSop.StudyInstanceUID)
				throw new ArgumentException();
			else if (_studyUid == null)
				_studyUid = image.ImageSop.StudyInstanceUID;

			_images.Add(image);
		}

		public int Count
		{
			get { return _images.Count; }
		}

		public T FirstImage
		{
			get
			{
				if (_images.Count == 0)
					return default(T);
				return _images[0];
			}
		}

		public IEnumerable<string> EnumerateSeries()
		{
			return this.Dictionary.Keys;
		}

		public IEnumerable<T> EnumerateImages()
		{
			return _images;
		}

		public IEnumerable<T> EnumerateImages(string seriesUid)
		{
			if (_dictionary.ContainsKey(seriesUid))
			{
				foreach (T image in _dictionary[seriesUid])
					yield return image;
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.EnumerateImages().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnumerateImages().GetEnumerator();
		}
	}
}