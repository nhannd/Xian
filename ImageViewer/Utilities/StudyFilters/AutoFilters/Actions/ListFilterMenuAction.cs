using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions
{
	[ExtensionPoint]
	public class ListFilterMenuActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	public interface IListFilterDataSource
	{
		IEnumerable<object> Values { get; }
		bool GetSelectedState(object value);
		void SetSelectedState(object value, bool selected);
		void SetAllSelectedState(bool selected);
	}

	[AssociateView(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuAction : Action
	{
		private readonly IListFilterDataSource _dataSource;

		public ListFilterMenuAction(string actionID, ActionPath actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
			: base(actionID, actionPath, resourceResolver)
		{
			Platform.CheckForNullReference(dataSource, "dataSource");
			_dataSource = dataSource;
		}

		public IListFilterDataSource DataSource
		{
			get { return _dataSource; }
		}

		public static ListFilterMenuAction CreateAction(Type callingType, string actionID, string actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
		{
			ListFilterMenuAction action = new ListFilterMenuAction(
				string.Format("{0}:{1}", callingType.FullName, actionID),
				new ActionPath(actionPath, resourceResolver),
				dataSource, resourceResolver);
			action.Label = action.Path.LastSegment.LocalizedText;
			action.Persistent = true;
			return action;
		}
	}
}