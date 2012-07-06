#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class OrderNoteboxFolder : WorkflowFolder<OrderNoteboxItemSummary>
	{
		private readonly string _noteboxClassName;

		public OrderNoteboxFolder(OrderNoteboxFolderSystem folderSystem, string noteboxClassName)
			: base(new OrderNoteboxTable())
		{
			_noteboxClassName = noteboxClassName;

			this.AutoInvalidateInterval = new TimeSpan(0, 0, 0, 0, OrderNoteboxFolderSystemSettings.Default.RefreshTime);
		}

		public override int PageSize
		{
			get { return OrderNoteboxFolderSystemSettings.Default.ItemsPerPage; }
		}

		protected override QueryItemsResult QueryItems(int firstRow, int maxRows)
		{
			QueryItemsResult result = null;
			Platform.GetService(
				delegate(IOrderNoteService service)
				{
					var request = new QueryNoteboxRequest(_noteboxClassName, true, true) {Page = new SearchResultPage(firstRow, maxRows)};
					PrepareQueryRequest(request);
					var response = service.QueryNotebox(request);
					result = new QueryItemsResult(response.NoteboxItems, response.ItemCount);
				});

			return result;
		}

		protected override int QueryCount()
		{
			int count = -1;
			Platform.GetService(
				delegate(IOrderNoteService service)
				{
					var request = new QueryNoteboxRequest(_noteboxClassName, true, false);
					PrepareQueryRequest(request);
					var response = service.QueryNotebox(request);
					count = response.ItemCount;
				});

			return count;
		}

		protected virtual void PrepareQueryRequest(QueryNoteboxRequest request)
		{
			// nothing to do
		}
	}
}
