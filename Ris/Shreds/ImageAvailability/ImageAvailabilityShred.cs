using System;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageAvailabilityShred : Shred
	{
		private readonly ImageAvailabilityProcessor _processor;
		private bool _isStarted;

		public ImageAvailabilityShred()
		{
			_processor = new ImageAvailabilityProcessor();
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
			return SR.ShredName;
		}

		public override string GetDescription()
		{
			return SR.ShredDescription;
		}

		#endregion

		private void StartUp()
		{
			Platform.Log(LogLevel.Info, SR.ServiceStarting);

			try
			{
				_isStarted = true;
				_processor.Start();

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
				_processor.RequestStop();
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
