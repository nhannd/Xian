using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class PresetVoiLutGroupCollection : ICollection<PresetVoiLutGroup>
	{
		private readonly List<PresetVoiLutGroup> _groups;

		public PresetVoiLutGroupCollection()
		{
			_groups = new List<PresetVoiLutGroup>();
		}

		#region ICollection<PresetVoiLutGroup> Members

		public void Add(PresetVoiLutGroup item)
		{
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
			throw new Exception("The method or operation is not implemented.");
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
	}
}
