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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutGroupCollection : IList<PresetVoiLutGroup>
	{
		private readonly List<PresetVoiLutGroup> _groups;

		public PresetVoiLutGroupCollection()
		{
			_groups = new List<PresetVoiLutGroup>();
		}

		#region IList<PresetVoiLutGroup> Members

		public int IndexOf(PresetVoiLutGroup item)
		{
			return _groups.IndexOf(item);
		}

		public void Insert(int index, PresetVoiLutGroup item)
		{
			if (_groups.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

			_groups.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_groups.RemoveAt(index);
		}

		public PresetVoiLutGroup this[int index]
		{
			get
			{
				return _groups[index];
			}
			set
			{
				if (_groups.Contains(value))
					throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

				_groups[index] = value;
			}
		}

		#endregion

		#region ICollection<PresetVoiLutGroup> Members

		public void Add(PresetVoiLutGroup item)
		{
			if (_groups.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

			_groups.Add(item);
		}

		public void Clear()
		{
			_groups.Clear();
		}

		public bool Contains(PresetVoiLutGroup item)
		{
			return _groups.Contains(item);
		}

		public void CopyTo(PresetVoiLutGroup[] array, int arrayIndex)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public int Count
		{
			get { return _groups.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PresetVoiLutGroup item)
		{
			return _groups.Remove(item);
		}

		#endregion

		#region IEnumerable<PresetVoiLutGroup> Members

		public IEnumerator<PresetVoiLutGroup> GetEnumerator()
		{
			return _groups.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _groups.GetEnumerator();
		}

		#endregion

		public void Sort()
		{
			_groups.Sort();
		}

		public PresetVoiLutGroupCollection Clone()
		{
			PresetVoiLutGroupCollection clone = new PresetVoiLutGroupCollection();
			foreach (PresetVoiLutGroup group in _groups)
				clone.Add(group.Clone());

			return clone;
		}
	}
}
