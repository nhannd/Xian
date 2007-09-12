using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutCollectionSortByKeyStrokeSortByName : IComparer<PresetVoiLut>
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
				int nameCompare = x.Applicator.Name.CompareTo(y.Applicator.Name);
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
				throw new InvalidOperationException("An equivalent preset already exists.");

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
					throw new InvalidOperationException("An equivalent preset already exists.");

				_presets[index] = value;
			}
		}

		#endregion

		#region ICollection<PresetVoiLut> Members

		public void Add(PresetVoiLut item)
		{
			if (_presets.Contains(item))
				throw new InvalidOperationException("An equivalent preset already exists.");

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
