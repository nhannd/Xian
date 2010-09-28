#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public sealed class ExceptionDialogFactoryExtensionPoint : ExtensionPoint<IExceptionDialogFactory>
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

	public interface IExceptionDialogFactory
	{
		IExceptionDialog CreateExceptionDialog();
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
				var displayThread = new Thread(ignored => ShowReal()) { IsBackground = false };
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

		private static readonly IExceptionDialogFactory _factory = CreateFactory();

		protected string Title { get; private set; }
		protected Exception Exception { get; private set; }
		protected string Message { get; private set; }
		protected ExceptionDialogActions Actions { get; private set; }

		internal static bool CanShow
		{
			get { return _factory != null; }
		}

		internal static void CheckCanShow()
		{
			if (!CanShow)
				throw new NotSupportedException("No exception dialog extension exists.");
		}

		private static IExceptionDialog Create()
		{
			CheckCanShow();
			return new MarshallingProxy(_factory.CreateExceptionDialog()); 
		}

		private static IExceptionDialogFactory CreateFactory()
		{
			try
			{
				return (IExceptionDialogFactory)new ExceptionDialogFactoryExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
			}
			catch (Exception e)
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