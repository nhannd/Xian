#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{

	/// <summary>
	/// Defines an extension point for views onto the <see cref="StackedComponentContainer"/>.
	/// </summary>
	public sealed class StackedComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StackedComponentContainerViewExtensionPoint))]
	public class StackedComponentContainer : PagedComponentContainer<StackedComponentContainer.StackedComponentContainerPage>
	{
		public class StackedComponentContainerPage : ContainerPage
		{
			public StackedComponentContainerPage(IApplicationComponent component)
				: base(component)
			{
			}
		}

		public void Show(IApplicationComponent component)
		{
			var page = this.Pages.FirstOrDefault(p => p.Component == component);
			if(page == null)
			{
				page = new StackedComponentContainerPage(component);
				this.Pages.Add(page);
			}

			// work around an issue with the PagedComponentContainer, where setting CurrentPage 
			// when the component is not started causes problems
			if(this.IsStarted)
				this.CurrentPage = page;
		}
	}
}
