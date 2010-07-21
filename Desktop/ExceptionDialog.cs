using System;
using System.Threading;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public sealed class ExceptionDialogExtensionPoint : ExtensionPoint<IExceptionDialog>
	{
	}

	[Flags]
	public enum ExceptionDialogAction
	{
		Ok = 0x1,
		Quit = 0x2
	}

	[Flags]
	public enum ExceptionDialogActions
	{
		Ok = ExceptionDialogAction.Ok,
		Quit = ExceptionDialogAction.Quit,
		QuitContinue = ExceptionDialogAction.Ok | ExceptionDialogAction.Quit
	}

	public interface IExceptionDialog
	{
		ExceptionDialogAction Show(string title, string message, Exception e, ExceptionDialogActions actions);
	}

	public abstract class ExceptionDialog : IExceptionDialog
	{
		private class MarshallingProxy : ExceptionDialog
		{
			private static IExceptionDialog _real;
			private ExceptionDialogAction _result;

			public MarshallingProxy(IExceptionDialog real)
			{
				_real = real;
			}

			private void ShowReal()
			{
				_result = _real.Show(Title, Message, Exception, Actions);
			}

			private void ShowAsync()
			{
				var displayThread = new Thread(unused => ShowReal()) { IsBackground = false };
				displayThread.SetApartmentState(ApartmentState.STA);
				displayThread.Start();
				displayThread.Join();
			}

			protected override ExceptionDialogAction Show()
			{
				var syncContext = Application.SynchronizationContext;

				//Try our best to report the error on the UI thread.
				if (syncContext == null)
				{
					ShowAsync();
				}
				else if (SynchronizationContext.Current == syncContext)
				{
					ShowReal();
				}
				else
				{
					try
					{
						syncContext.Send(ignored => ShowReal(), null);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex);
						//If we got here, it's because the application was exiting when we tried to send
						//the message to the UI thread.
						ShowAsync();
					}
				}

				return _result;
			}
		}

		private static readonly ConstructorInfo _dialogConstructor = GetDialogConstructor();

		protected string Title { get; private set; }
		protected Exception Exception { get; private set; }
		protected string Message { get; private set; }
		protected ExceptionDialogActions Actions { get; private set; }

		internal static bool CanShow
		{
			get { return _dialogConstructor != null; }
		}

		internal static void CheckCanShow()
		{
			if (!CanShow)
				throw new NotSupportedException("No exception dialog extension exists.");
		}

		private static IExceptionDialog Create()
		{
			CheckCanShow();
			return new MarshallingProxy((IExceptionDialog) _dialogConstructor.Invoke(null)); 
		}

		private static ConstructorInfo GetDialogConstructor()
		{
			try
			{
				return new ExceptionDialogExtensionPoint().CreateExtension().GetType().GetConstructor(new Type[0]);
			}
			catch (NotSupportedException)
			{
			}
			catch (ArgumentException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}

			return null;
		}

		private static void Show(string title, string message, Exception e, ExceptionDialogActions actions)
		{
			var result = Create().Show(title, message, e, actions);
			if (result == ExceptionDialogAction.Quit)
				Application.Shutdown();
		}

		internal static void Show(string message, Exception e, ExceptionDialogActions actions)
		{
			Show(Application.Name, message, e, actions);
		}

		protected abstract ExceptionDialogAction Show();

		#region IExceptionDialog Members

		ExceptionDialogAction IExceptionDialog.Show(string title, string message, Exception e, ExceptionDialogActions actions)
		{
			Title = title;
			Message = message;
			Exception = e;
			Actions = actions;

			return Show();
		}

		#endregion
	}
}