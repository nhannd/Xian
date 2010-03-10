using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Alerts
{
	[ExtensionPoint]
	public class ExternalPractitionerAlertExtensionPoint : ExtensionPoint<IExternalPractitionerAlert>
	{
	}

	public interface IExternalPractitionerAlert : IAlert<ExternalPractitioner>
	{
	}

	public abstract class ExternalPractitionerAlertBase : AlertBase<ExternalPractitioner>, IExternalPractitionerAlert
	{
	}
}
