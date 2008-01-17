using ClearCanvas.Common;

namespace ClearCanvas.Ris.Shreds.Publication
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class PublicationConsoleApp : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            IPublisher publisher = new Publisher();
            publisher.Start();
        }

        #endregion
    }
}
