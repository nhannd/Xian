using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Common.Messages;

namespace ClearCanvas.Web.Services
{
    [ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
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
						typeof(EntityEvent),
						typeof(PropertyChangedEvent),
                        typeof(ListPropertyChangedEvent),
                        typeof(ListChangeType),

			       		typeof(SessionUpdatedEvent),
			       		typeof(MessageBoxShownEvent),
						typeof(AlertShownEvent),
                        typeof(DialogBoxDismissedEvent),
                        typeof(DialogBoxShownEvent),
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
                        typeof(Image),
                        typeof(Size),
						typeof(Rectangle),
						typeof(Position),
						typeof(MessageBox),
                        typeof(WebMessageBoxActions),
                        typeof(DialogBox),
                        typeof(ProgressComponent),
						typeof(Alert),

						//TODO: if we ever include the desktop stuff, move this out of the viewer namespace
						typeof(WebActionNode[]),
						typeof(WebIconSet),
                        typeof(WebIconSize),
						typeof (WebActionNode),
			       		typeof (WebAction),
			       		typeof (WebClickAction),
			       		typeof (WebDropDownButtonAction),
			       		typeof (WebDropDownAction)
			       	};
        }

        #endregion
    }


}
