using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Publication
{
    /// <summary>
    /// Defines the interface for processing publication step after it is published.
    /// </summary>
    public interface IPublicationStepProcessor
    {
        void Process(PublicationStep step, IPersistenceContext context);
    }
}
