#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Action model that allows a user to control a <see cref="IPagingController{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class PagingActionModel<TItem> : SimpleActionModel
	{
		private readonly IDesktopWindow _desktopWindow;
		private readonly IPagingController<TItem> _controller;

		///<summary>
		/// Constructor.
		///</summary>
		///<param name="controller"></param>
		///<param name="desktopWindow"></param>
		public PagingActionModel(IPagingController<TItem> controller, IDesktopWindow desktopWindow)
			: base(new ApplicationThemeResourceResolver(typeof(PagingActionModel<TItem>).Assembly))
		{
			_controller = controller;
			_desktopWindow = desktopWindow;

			AddAction("Previous", SR.TitlePrevious, "Icons.PreviousPageToolSmall.png");
			AddAction("Next", SR.TitleNext, "Icons.NextPageToolSmall.png");

			Next.SetClickHandler(OnNext);
			Previous.SetClickHandler(OnPrevious);

			Next.Enabled = _controller.HasNext;
			Previous.Enabled = _controller.HasPrevious;  // can't go back from first

			_controller.PageChanged += PageChangedEventHandler;
		}

		private void OnNext()
		{
			try
			{
				_controller.GetNext();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, _desktopWindow);
			}
		}

		private void OnPrevious()
		{
			try
			{
				_controller.GetPrevious();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, _desktopWindow);
			}
		}

		private void PageChangedEventHandler(object sender, PageChangedEventArgs<TItem> args)
		{
			this.Next.Enabled = _controller.HasNext;
			this.Previous.Enabled = _controller.HasPrevious;
		}

		private ClickAction Next
		{
			get { return (ClickAction)this["Next"]; }
		}

		private ClickAction Previous
		{
			get { return (ClickAction)this["Previous"]; }
		}
	}
}
