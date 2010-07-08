using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Configuration
{
	[ExtensionPoint]
	public class UserUpgradeProgressDialogExtensionPoint : ExtensionPoint<IUserUpgradeProgressDialog>
	{}

	public interface IUserUpgradeProgressDialog
	{
		void Show(IUserUpgradeStrategy strategy);
	}
}