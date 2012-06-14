#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public enum ActivityMonitorQuickLink
	{
		SystemConfiguration,
		LocalStorageConfiguration,
		StudyManagementRules
	}

	public static class ActivityMonitorQuickLinkExtensions
	{
		public static bool CanInvoke(this ActivityMonitorQuickLink link)
		{
			var handlers = (new ActivityMonitorQuickLinkHandlerExtensionPoint()).CreateExtensions().Cast<IActivityMonitorQuickLinkHandler>();
			return handlers.Any(h => h.CanHandle(link));
		}

		public static void Invoke(this ActivityMonitorQuickLink link, IDesktopWindow desktopWindow)
		{
			var handlers = (new ActivityMonitorQuickLinkHandlerExtensionPoint()).CreateExtensions().Cast<IActivityMonitorQuickLinkHandler>();
			var handler = handlers.FirstOrDefault(h => h.CanHandle(link));
			if (handler != null)
			{
				handler.Handle(link, desktopWindow);
			}
		}
	}


	[ExtensionPoint]
	public sealed class ActivityMonitorQuickLinkHandlerExtensionPoint : ExtensionPoint<IActivityMonitorQuickLinkHandler> { }

	public interface IActivityMonitorQuickLinkHandler
	{
		/// <summary>
		/// Asks handler if it can handle the specified link.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		bool CanHandle(ActivityMonitorQuickLink link);


		/// <summary>
		/// Asks handler to handle the specified link, returning true if handled.
		/// </summary>
		/// <param name="link"></param>
		/// <param name="window"> </param>
		/// <returns></returns>
		void Handle(ActivityMonitorQuickLink link, IDesktopWindow window);
	}
}
