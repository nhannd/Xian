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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Web.Common;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using ClearCanvas.ImageViewer.Web.Common.Events;
using ClearCanvas.ImageViewer.Web.Common.Messages;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Common.Messages;
using ClearCanvas.Web.Services;

namespace ClearCanvas.ImageViewer.Web
{
	//TODO (CR May 2010): use attributes, scan types.

	[ExtensionOf(typeof (ServiceKnownTypeExtensionPoint))]
	public class KnownEventTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
						typeof(ApplicationNotFoundEvent),
						typeof(ApplicationStartedEvent),
						typeof(ApplicationStoppedEvent),
						typeof(EntityUpdatedEvent),
						typeof(PropertyChangedEvent),

			       		typeof(ContextMenuEvent),
			       		typeof(SessionUpdatedEvent),
			       		typeof(MessageBoxShownEvent),
						typeof(AlertShownEvent),
                        typeof(RenderingStatsMessage),
                        typeof(StackRenderingTimesMessage)
			       	};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownMessageTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
						typeof (ActionClickedMessage),
						typeof (SetLayoutActionMessage),
			       		typeof (MouseMessage),
			       		typeof (MouseLeaveMessage),
			       		typeof (MouseMoveMessage),
			       		typeof (MouseWheelMessage),
                        typeof (DismissMessageBoxMessage),
                        typeof (DismissAlertMessage),
                        typeof (UpdatePropertyMessage)
					};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownEntityTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
			       		typeof (Viewer),
			       		typeof (Common.Entities.ImageBox),
			       		typeof (Common.Entities.Tile),
			       		
						typeof (Common.Entities.ImageBox[]),
			       		typeof (Common.Entities.Tile[]),

                        typeof(Image),
                        typeof(Size),
						typeof(Rectangle),
						typeof(Position),
						typeof(Cursor),
						typeof(Common.Entities.InformationBox),
						typeof(MessageBox),
                        typeof(WebMessageBoxActions),
						typeof(Alert),

						//TODO: if we ever include the desktop stuff, move this out of the viewer namespace
						typeof(WebActionNode[]),
						typeof(WebIconSet),
                        typeof(WebIconSize),
						typeof (WebActionNode),
			       		typeof (WebAction),
			       		typeof (WebClickAction),
			       		typeof (WebDropDownButtonAction),
			       		typeof (WebDropDownAction),
			       		typeof (WebLayoutChangerAction)
			       	};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownStartApplicationRequestTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			yield return typeof (StartViewerApplicationRequest);
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownApplicationTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			yield return typeof(Common.ViewerApplication);
		}

		#endregion
	}
}