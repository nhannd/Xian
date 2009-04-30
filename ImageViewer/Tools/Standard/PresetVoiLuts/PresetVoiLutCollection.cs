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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutComparer : IComparer<PresetVoiLut>
	{
		#region IComparer<PresetVoiLutCollection> Members

		public int Compare(PresetVoiLut x, PresetVoiLut y)
		{
			if (x.KeyStroke == XKeys.None)
			{
				if (y.KeyStroke == XKeys.None)
					return 0;

				return 1;
			}
			else if (y.KeyStroke == XKeys.None)
			{
				return -1; 
			}

			if (x.KeyStroke < y.KeyStroke)
			{
				return -1;
			}
			else if (x.KeyStroke == y.KeyStroke)
			{
				int nameCompare = x.Operation.Name.CompareTo(y.Operation.Name);
				if (nameCompare < 0)
					return -1;
				else if (nameCompare == 0)
					return 0;
			}

			return 1; ;
		}

		#endregion
	}

	internal sealed class PresetVoiLutCollection : IList<PresetVoiLut>
	{
		private readonly List<PresetVoiLut> _presets;

		public PresetVoiLutCollection()
		{
			_presets = new List<PresetVoiLut>();
		}

		#region IList<PresetVoiLut> Members

		public int IndexOf(PresetVoiLut item)
		{
			return _presets.IndexOf(item);
		}

		public void Insert(int index, PresetVoiLut item)
		{
			if (_presets.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

			_presets.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_presets.RemoveAt(index);
		}

		public PresetVoiLut this[int index]
		{
			get
			{
				return _presets[index];
			}
			set
			{
				if (_presets.Contains(value))
					throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

				_presets[index] = value;
			}
		}

		#endregion

		#region ICollection<PresetVoiLut> Members

		public void Add(PresetVoiLut item)
		{
			if (_presets.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

			_presets.Add(item);
		}

		public void Clear()
		{
			_presets.Clear();
		}

		public bool Contains(PresetVoiLut item)
		{
			return _presets.Contains(item);
		}

		public void CopyTo(PresetVoiLut[] array, int arrayIndex)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public int Count
		{
			get { return _presets.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PresetVoiLut item)
		{
			return _presets.Remove(item);
		}

		#endregion

		#region IEnumerable<PresetVoiLut> Members

		public IEnumerator<PresetVoiLut> GetEnumerator()
		{
			return _presets.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _presets.GetEnumerator();
		}

		#endregion

		public void Sort(IComparer<PresetVoiLut> comparer)
		{
			_presets.Sort(comparer);
		}

		public PresetVoiLutCollection Clone()
		{
			PresetVoiLutCollection clone = new PresetVoiLutCollection();
			foreach (PresetVoiLut preset in _presets)
				clone.Add(preset.Clone());

			return clone;
		}
	}
}
