using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	public sealed class PatientItemCollection : BindingList<PatientItem>
	{
		// mappings between nodes and items
		private readonly Dictionary<PatientNode, PatientItem> _map = new Dictionary<PatientNode, PatientItem>();

		// reference to the underlying collection
		private readonly PatientNodeCollection _collection;

		internal PatientItemCollection(PatientNodeCollection collection)
		{
			_collection = collection;
			foreach (PatientNode node in collection)
			{
				base.Add(new PatientItem(node));
			}
		}

		internal PatientItem GetById(DicomAttributeCollection dataSet)
		{
			PatientNode node = _collection.GetPatientById(dataSet);
			if (!_map.ContainsKey(node))
				base.Add(new PatientItem(node));
			return _map[node];
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			int id = base.IndexOf(sender as PatientItem);
			if (id >= 0)
				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, id));
		}

		protected override object AddNewCore()
		{
			return new PatientItem(new PatientNode());
		}

		protected override void ClearItems()
		{
			foreach (PatientItem item in _map.Values)
			{
				item.PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
			_map.Clear();
			_collection.Clear();
		}

		protected override void InsertItem(int index, PatientItem item)
		{
			PatientNode node = item.Node;
			_map.Add(node, item);
			if (!_collection.Contains(node)) // this method is also called when initializing the list from the collection, so we need to check this to avoid re-adding
				_collection.Add(node);

			base.InsertItem(index, item);

			item.PropertyChanged += Item_PropertyChanged;
		}

		protected override void RemoveItem(int index)
		{
			PatientNode node = base[index].Node;

			_map[node].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(node);
			_collection.Remove(node);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, PatientItem item)
		{
			PatientNode oldNode = base[index].Node;
			PatientNode newNode = item.Node;
			_map.Add(newNode, item);
			_map[oldNode].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(oldNode);
			_collection.Remove(oldNode);
			_collection.Add(newNode);
			item.PropertyChanged += Item_PropertyChanged;

			base.SetItem(index, item);
		}
	}
}