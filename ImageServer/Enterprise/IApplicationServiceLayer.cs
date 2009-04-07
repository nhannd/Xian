using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Enterprise
{
    public interface IApplicationServiceLayer
    {
    }

    [ExtensionPoint]
    public class ApplicationServiceExtensionPoint : ExtensionPoint<IApplicationServiceLayer>
    {
    }
}