using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	public interface IUserUpgradeStrategy
	{
		int TotalSteps { get; }
		int CurrentStep { get; }

		int SuccessfulCount { get; }
		int FailedCount { get; }
		int RemainingCount { get; }

		event EventHandler ProgressChanged;

		bool IsRunning { get; }
		void Run();
	}

	public class UserUpgradeStrategy : IUserUpgradeStrategy
	{
		private event EventHandler _progressChanged;

		private UserUpgradeStrategy(ICollection<UserUpgradeStep> steps)
		{
			Steps = steps;
		}

		private ICollection<UserUpgradeStep> Steps { get; set; }
	
		public int TotalSteps { get { return Steps.Count; } }
		public int CurrentStep { get; private set; }

		public int SuccessfulCount { get; private set; }
		public int FailedCount { get; private set; }
		public int RemainingCount { get; private set; }
	
		public event EventHandler ProgressChanged
		{
			add { _progressChanged += value; }
			remove { _progressChanged -= value; }
		}

		public static IUserUpgradeStrategy Create()
		{
			if (!UpgradeSettings.IsUserUpgradeEnabled())
				return null;

			ICollection<UserUpgradeStep> steps = UserUpgradeStep.CreateAll();
			if (steps.Count == 0)
				return null;

			return new UserUpgradeStrategy(steps);
		}

		public bool IsRunning { get; private set; }

		public void Run()
		{
			if (IsRunning)
				return;

			IsRunning = true;
			
			CodeClock clock = new CodeClock();
			clock.Start();
			
			foreach (UserUpgradeStep step in Steps)
			{
				try
				{
					++CurrentStep;
					step.Run();
					++SuccessfulCount;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "User upgrade step failed: {0}", step.Identifier);
					++FailedCount;
				}
				finally
				{
					--RemainingCount;
					EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
				}
			}

			clock.Stop();
			Platform.Log(LogLevel.Debug, "User upgrade took: {0}", clock);
		}
	}
}