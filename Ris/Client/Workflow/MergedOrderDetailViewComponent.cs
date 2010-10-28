#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class MergedOrderDetailViewComponent : OrderDetailViewComponent
	{
		public MergedOrderDetailViewComponent()
		{
		}

		public MergedOrderDetailViewComponent(OrderDetail detail)
		{
			_context = detail;
		}

		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.MergedOrderDetailPageUrl; }
		}
	}
}
