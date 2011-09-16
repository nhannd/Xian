using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class ServerMessageSender
    {
        public bool Faulted
        {
            get
            {
                if (_proxy != null && _proxy.State != CommunicationState.Faulted && _proxy.InnerChannel != null && _proxy.InnerChannel.State != CommunicationState.Faulted)
                    return false;

                return true;
            }
        }

        public ApplicationServiceClient Proxy
        {
            get { return _proxy; }
        }
    }
}
