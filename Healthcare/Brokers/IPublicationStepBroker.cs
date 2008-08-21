using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Defines the interface for a <see cref="PublicationStep"/> broker
	/// </summary>
	public interface IPublicationStepBroker : IEntityBroker<PublicationStep, PublicationStepSearchCriteria>
	{
	}
}