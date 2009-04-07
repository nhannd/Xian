using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    public interface IApplicationServiceLayer
    {
    }

    [ExtensionPoint]
    public class ApplicationServiceExtensionPoint : ExtensionPoint<IApplicationServiceLayer>
    {
    }
}
