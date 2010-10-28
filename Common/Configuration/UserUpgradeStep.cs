#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Collections;

namespace ClearCanvas.Common.Configuration
{
	public class UserUpgradeStepFactoryExtensionPoint : ExtensionPoint<IUserUpgradeStepFactory>
	{
	}

	public interface IUserUpgradeStepFactory
	{
		ICollection<UserUpgradeStep> CreateSteps();
	}

	public abstract class UserUpgradeStep
	{
		[ThreadStatic] private static Stack<string> _stepsInProgress;

		private static Stack<string> StepsInProgress
		{
			get
			{
				if (_stepsInProgress == null)
					_stepsInProgress = new Stack<string>();
				return _stepsInProgress;
			}	
		}

		private bool IsInProgress
		{
			get { return StepsInProgress.Contains(Identifier); }
		}

		#region IUserUpgradeStep Members

		public abstract string Identifier { get; }

		public bool Run()
		{
			UpgradeSettings.CheckUserUpgradeEnabled();

			if (IsInProgress || IsCompleted)
				return false;

			StepsInProgress.Push(Identifier);

			try
			{
				bool success = PerformUpgrade();
				OnCompleted();
				return success;
			}
			finally
			{
				StepsInProgress.Pop();
			}
		}

		#endregion

		protected abstract bool PerformUpgrade();

		#region Internal

		private bool IsCompleted
		{
			get { return UpgradeSettings.Default.IsUserUpgradeStepCompleted(Identifier); }
		}

		private void OnCompleted()
		{
			UpgradeSettings.Default.OnUserUpgradeStepCompleted(Identifier);
		}

		#region Static Factory

		internal static ICollection<UserUpgradeStep> CreateAll()
		{
			List<UserUpgradeStep> steps = new List<UserUpgradeStep>();

			try
			{
				foreach (IUserUpgradeStepFactory factory in new UserUpgradeStepFactoryExtensionPoint().CreateExtensions())
				{
					foreach (UserUpgradeStep step in factory.CreateSteps())
					{
						if (!step.IsCompleted)
							steps.Add(step);
					}
				}
			}
			catch (NotSupportedException)
			{
			}

            foreach (UserUpgradeStep step in new UserSettingsUpgradeStepFactory().CreateSteps())
            {
                if (!step.IsCompleted)
                    steps.Add(step);
            }
            
            return steps;
		}

		#endregion
		#endregion
	}
}