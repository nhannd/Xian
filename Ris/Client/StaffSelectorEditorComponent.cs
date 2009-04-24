using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class StaffSelectorEditorComponent : WorklistSelectorEditorComponent<StaffSummary, StaffTable>
	{
		public class DummyItem : StaffSummary
		{
			public DummyItem()
			{
				this.Name = new PersonNameDetail();
				this.Name.FamilyName = SR.DummyItemUser;
				this.StaffId = "";
				this.StaffType = new EnumValueInfo("", "");
				this.StaffRef = new EntityRef(typeof(DummyItem), new object(), 0);
			}
		}

		private static readonly StaffSummary _currentUserItem = new DummyItem();

		private static IEnumerable<StaffSummary> CollectionAndCurrentUser(IEnumerable<StaffSummary> items)
		{
			List<StaffSummary> a = new List<StaffSummary>();
			a.Add(_currentUserItem);
			a.AddRange(items);
			return a;
		}

		public StaffSelectorEditorComponent(IEnumerable<StaffSummary> allItems, IEnumerable<StaffSummary> selectedItems, bool includeCurrentUser)
			: base(
				CollectionAndCurrentUser(allItems), 
				includeCurrentUser ? CollectionAndCurrentUser(selectedItems) : selectedItems, 
				delegate(StaffSummary s) { return s.StaffRef; })
		{
		}

		public bool IncludeCurrentUser
		{
			get { return base.SelectedItems.Contains(_currentUserItem); }
		}

		public override IList<StaffSummary> SelectedItems
		{
			get
			{
				return CollectionUtils.Select(base.SelectedItems, delegate(StaffSummary staff) { return staff != _currentUserItem; });
			}
		}
	}
}