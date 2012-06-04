#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using System.Linq;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.WorkItem;
using NUnit.Framework;


namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{

	[TestFixture]
	public class ActivityMonitorComponentTests
	{
		private static readonly Predicate<ActivityMonitorComponent.WorkItem> NoFilter = (w => true);
		private static readonly Predicate<ActivityMonitorComponent.WorkItem> NormalPriorityFilter = (w => w.Priority == WorkItemPriorityEnum.Normal);


		public ActivityMonitorComponentTests()
		{
		}

	    [Test]
		public void Test_add_new_item()
	    {
	    	var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

	    	var data = new WorkItemData {Identifier = 1};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			Assert.AreEqual(1, items.Count);
			Assert.IsTrue(items.Any(item => item.Id == 1));
	    }

		[Test]
		public void Test_add_deleted_item()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

			var data = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Deleted};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_add_new_item_filtered()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			// item not added because of filter
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_update_item()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Normal }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Normal, items[0].Priority);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Stat, items[0].Priority);
		}

		[Test]
		public void Test_update_item_delete()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Complete }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemStatusEnum.Complete, items[0].Status);

			var data = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Deleted};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			// item removed from collection, because of deleted status
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_update_item_filtered()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Normal }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Normal, items[0].Priority);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(data)});

			// item removed from collection, because filtered by priority
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_clear_items()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});

			// add a failed item
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(failedItem)});

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			manager.Clear();

			// items collection is cleared
			Assert.AreEqual(0, items.Count);

			// failed item count is cleared
			Assert.AreEqual(0, manager.FailedItemCount);
		}

		[Test]
		public void Test_failed_item_tracking()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});

			// add a failed item
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(failedItem)});

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			// add another failed item
			failedItem = new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(failedItem)});
			
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(2, manager.FailedItemCount);

			// update one of the items to Pending (e.g. retry)
			var item = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Pending };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(item)});

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			// update other item to Pending (e.g. retry)
			item = new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Pending };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(item)});

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(0, manager.FailedItemCount);

			// update an item to failed
			item = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(item)});

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

		}

		[Test]
		public void Test_failed_item_tracking_ignores_filter()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });

			// add a failed item that does not meet the filter criteria
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed, Priority = WorkItemPriorityEnum.Stat};
			manager.Update(new[] {new ActivityMonitorComponent.WorkItem(failedItem)});

			// verify that the item is not added to the collection, but is tracked as a failed item
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(1, manager.FailedItemCount);
		}
	}
}

#endif