using System;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop
{
	public interface IChangePasswordDialog : IDisposable
	{
		bool Show();

		string UserName { get; set; }
		string Password { get; set; }
		string NewPassword { get; }
	}
}
