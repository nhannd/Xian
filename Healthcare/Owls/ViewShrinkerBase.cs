#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstarct base implementation of <see cref="IViewShrinker"/>.
	/// </summary>
	/// <typeparam name="TViewItem"></typeparam>
	/// <typeparam name="TViewItemSearchCriteria"></typeparam>
	/// <typeparam name="TViewItemBroker"></typeparam>
	public abstract class ViewShrinkerBase<TViewItem, TViewItemSearchCriteria, TViewItemBroker> : IViewShrinker
		where TViewItem : Entity
		where TViewItemSearchCriteria : EntitySearchCriteria, new()
		where TViewItemBroker : IEntityBroker<TViewItem, TViewItemSearchCriteria>
	{
		private readonly ISearchPredicate<TViewItem, TViewItemSearchCriteria> _exclustionPredicate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="exclustionPredicate"></param>
		protected ViewShrinkerBase(ISearchPredicate<TViewItem, TViewItemSearchCriteria> exclustionPredicate)
		{
			_exclustionPredicate = exclustionPredicate;
		}

		/// <summary>
		/// Deletes items from the view, returning the number of items actually deleted.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="maxItems">Maximum number of items to delete.</param>
		/// <returns>Number of items deleted.</returns>
		public int DeleteItems(IUpdateContext context, int maxItems)
		{
			var broker = PersistenceScope.CurrentContext.GetBroker<TViewItemBroker>();
			return FindAndDelete(broker, maxItems);
		}

		private int FindAndDelete(TViewItemBroker broker, int maxItems)
		{
			// because neither SQL or NH support the concept of "delete top N from...", we can't do this in a single statement
			// instead, we first do a "select top N..." to get a batch of items, and then
			// delete items where the OID is in the specified batch (e.g. "delete X x where x.OID in (...)" )

			var items = broker.Find(_exclustionPredicate.GetSearchCriteria(), new SearchResultPage(0, maxItems));
			Platform.Log(LogLevel.Info, "View shrinker found {0} instances of {1} that should be deleted.", items.Count, typeof(TViewItem).Name);

			if (items.Count == 0)
				return 0;

			// construct the delete statement
			var deleteWhere = new TViewItemSearchCriteria();
			deleteWhere.OID.In(CollectionUtils.Map(items, (TViewItem item) => item.OID));
			var count = broker.Delete(deleteWhere);

			Platform.Log(LogLevel.Info, "View shrinker deleted {0} instances of {1}", count, typeof(TViewItem).Name);

			return count;
		}
	}
}
