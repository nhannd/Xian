using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Defines an interface to a strategy for determining the Image Availability of a Procedure.
	/// </summary>
	public interface IImageAvailabilityStrategy
	{
		/// <summary>
		/// Computes the <see cref="Healthcare.ImageAvailability"/> for a given <see cref="Procedure"/>.
		/// </summary>
		/// <remarks>
		/// This method must compute and return the image availability for the specified procedure.  The read-context
		/// is provided in case it is necessary to query parts of the model that are not reachable from the procedure.
		/// </remarks>
		/// <param name="procedure"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		Healthcare.ImageAvailability ComputeProcedureImageAvailability(Procedure procedure, IReadContext context);
	}

	[ExtensionPoint]
	public class ImageAvailabilityStrategyExtensionPoint : ExtensionPoint<IImageAvailabilityStrategy>
	{
	}
}
