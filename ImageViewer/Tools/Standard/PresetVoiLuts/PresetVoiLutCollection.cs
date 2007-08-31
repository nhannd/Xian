using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class PresetVoiLutCollection : ICollection<PresetVoiLut>
	{
		private readonly List<PresetVoiLut> _presets;

		public PresetVoiLutCollection()
		{
			_presets = new List<PresetVoiLut>();
		}

		#region ICollection<PresetVoiLut> Members

		public void Add(PresetVoiLut item)
		{
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
			throw new Exception("The method or operation is not implemented.");
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
	}
}
