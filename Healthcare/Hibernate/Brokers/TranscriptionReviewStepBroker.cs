using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	class TranscriptionReviewStepBroker : EntityBroker<TranscriptionReviewStep, TranscriptionReviewStepSearchCriteria>, ITranscriptionReviewStepBroker
	{
	}
}
