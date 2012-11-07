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
			       		typeof(ContextMenuEvent),
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
						typeof (SetLayoutActionMessage),
			       		typeof (MouseMessage),
			       		typeof (MouseLeaveMessage),
			       		typeof (MouseMoveMessage),
			       		typeof (MouseWheelMessage)
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