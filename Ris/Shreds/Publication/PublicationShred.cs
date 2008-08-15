using System;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PublicationShred : Shred
	{
		private readonly IPublisher _publisher;
		private bool _isStarted;

		public PublicationShred()
		{
			_publisher = new Publisher();
		}

		#region Shred overrides

		public override void Start()
		{
			if (!_isStarted)
			{
				StartUp();
			}
		}

		public override void Stop()
		{
			if (_isStarted)
			{
				ShutDown();
			}
		}

		public override string GetDisplayName()
		{
			return SR.Publication;
		}

		public override string GetDescription()
		{
			return SR.PublicationDescription;
		}

		#endregion

		private void StartUp()
		{
			Platform.Log(LogLevel.Info, SR.ServiceStarting);

			try
			{
				_publisher.Start();
				_isStarted = true;

				Platform.Log(LogLevel.Info, SR.ServiceStartedSuccessfully);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, SR.ServiceFailedToStart);
			}
		}

		private void ShutDown()
		{
			Platform.Log(LogLevel.Info, SR.ServiceStopping);

			try
			{
				_publisher.RequestStop();
				_isStarted = false;

				Platform.Log(LogLevel.Info, SR.ServiceStoppedSuccessfully);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, SR.ServiceFailedToStop);
			}
		}
	}
}