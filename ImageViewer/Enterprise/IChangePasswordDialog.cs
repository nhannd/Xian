using System;

namespace ClearCanvas.ImageViewer.Enterprise
{
	public interface IChangePasswordDialog : IDisposable
	{
		bool Show();

		string UserName { get; set; }
		string Password { get; set; }
		string NewPassword { get; }
	}
}
