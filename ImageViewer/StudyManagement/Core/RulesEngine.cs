#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	public static class RulesEngine
	{
		#region DefaultEngine class

		/// <summary>
		/// Defines a default rules engine implementation, that is only capable of applying the default deletion rule.
		/// </summary>
		class DefaultEngine : IRulesEngine
		{
			public void ApplyStudyRules(StudyEntry study, RulesEngineOptions options)
			{
				ApplyDefaultDeletionRule(options, study);
			}

			public void ApplyStudyRule(StudyEntry study, string ruleId, RulesEngineOptions options)
			{
				ApplyDefaultDeletionRule(options, study);
			}

			private void ApplyDefaultDeletionRule(RulesEngineOptions context, StudyEntry study)
			{
				if (!context.ApplyDeleteActions)
					return;

			    // TODO (CR Jun 2012): Again, seem to use "work item" mutex for all database updates. Should just pass in a boolean.
				using (var dac = new DataAccessContext(DataAccessContext.WorkItemMutex))
				{
					var broker = dac.GetStudyBroker();
					var dbStudy = broker.GetStudy(study.Study.StudyInstanceUid);

					var storageConfiguration = StudyStore.GetConfiguration();
					var defaultRule = storageConfiguration.DefaultDeletionRule;
					if (defaultRule.Enabled)
					{
						dbStudy.SetDeleteTime(defaultRule.TimeValue, defaultRule.TimeUnit, TimeOrigin.ReceivedDate, false);
					}
					else
					{
						dbStudy.ClearDeleteTime();
					}
							
					dac.Commit();
				}
			}
		}

		#endregion

		/// <summary>
		/// Returns an instance of the study management rules engine.
		/// </summary>
		/// <returns></returns>
		public static IRulesEngine Create()
		{
			var ep = new RulesEngineExtensionPoint();
			return (IRulesEngine)ep.CreateExtensions().FirstOrDefault() ?? new DefaultEngine();
		}
	}
}
