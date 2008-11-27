using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds
{
	public abstract class RisShredBase : Shred
	{
		private bool _isStarted;

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

		#endregion

		private void StartUp()
		{
			Platform.Log(LogLevel.Info, string.Format(SR.ShredStarting, this.GetDisplayName()));
			ShredsSettings settings = new ShredsSettings();
			try
			{
				_isStarted = true;

				foreach (IProcessor processor in GetProcessors())
				{
					processor.Initialize(settings.BatchSize, settings.SleepDurationInSeconds);
					processor.Start();
				}

				Platform.Log(LogLevel.Info, string.Format(SR.ShredStartedSuccessfully, this.GetDisplayName()));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, string.Format(SR.ShredFailedToStart, this.GetDisplayName()));
			}
		}

		private void ShutDown()
		{
			Platform.Log(LogLevel.Info, string.Format(SR.ShredStopping, this.GetDisplayName()));

			try
			{
				foreach (IProcessor processor in GetProcessors())
				{
					processor.RequestStop();
				}

				_isStarted = false;

				Platform.Log(LogLevel.Info, string.Format(SR.ShredStoppedSuccessfully, this.GetDisplayName()));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, string.Format(SR.ShredFailedToStop, this.GetDisplayName()));
			}
		}

		protected abstract IList<IProcessor> GetProcessors();
	}
}
