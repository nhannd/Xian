#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines the set of actions that the utility can do.
	/// </summary>
	public enum UtilityAction
	{
		/// <summary>
		/// Builds or rebuilds the view from scratch.
		/// </summary>
		Build,

		/// <summary>
		/// Shrinks the view as much as possible by removing items that exceed the retention time.
		/// </summary>
		Shrink,

		/// <summary>
		/// Re-indexes the view, without modifying its contents.
		/// </summary>
		Reindex
	}

	/// <summary>
	/// Owls utility application, used to perform tasks such as building, re-indexing, and shrinking views.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class Utility : IApplicationRoot
	{
		#region IApplicationRoot Members

		/// <summary>
		/// Called by the platform to run the application.
		/// </summary>
		/// <remarks>
		/// It is expected that this method may block for the duration of the application's execution, if
		/// for instance, a GUI event message pump is started.
		/// </remarks>
		void IApplicationRoot.RunApplication(string[] args)
		{
			// parse command line
			var cmdLine = new OwlsUtilityCommandLine();
			try
			{
				cmdLine.Parse(args);
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
				return;
			}

			// create views, and filter according to cmd line
			var views = CollectionUtils.Cast<IView>(new WorklistViewExtensionPoint().CreateExtensions());
			if(!string.IsNullOrEmpty(cmdLine.View))
			{
				views = CollectionUtils.Select(views, v => v.GetType().FullName.EndsWith(cmdLine.View));
			}

			// init NH
			PersistentStoreRegistry.GetDefaultStore();

			// perform requested action
			switch (cmdLine.Action)
			{
				case UtilityAction.Build:
				case UtilityAction.Reindex:
					Build(views, cmdLine);
					break;
				case UtilityAction.Shrink:
					Shrink(views, cmdLine);
					break;
			}
		}

		#region Shrink

		/// <summary>
		/// Shrink the specified views as much as possible.
		/// </summary>
		/// <param name="views"></param>
		/// <param name="cmdLine"></param>
		private static void Shrink(IList<IView> views, OwlsUtilityCommandLine cmdLine)
		{
			// create shrinkers for every view
			var shrinkers = CollectionUtils.Map(views, (IView view) => view.CreateShrinker());
			foreach (var shrinker in shrinkers)
			{
				ShrinkView(shrinker, cmdLine);
			}
		}

		/// <summary>
		/// Shrink the specified view repeatedly until it won't shrink any further.
		/// </summary>
		/// <param name="shrinker"></param>
		/// <param name="cmdLine"></param>
		private static void ShrinkView(IViewShrinker shrinker, OwlsUtilityCommandLine cmdLine)
		{
			int count;
			do
			{
				using (var pscope = new PersistenceScope(PersistenceContextType.Update))
				{
					var context = (IUpdateContext)pscope.Context;
					context.ChangeSetRecorder = null;	// disable auditing

					count = shrinker.DeleteItems(context, cmdLine.BatchSize);

					pscope.Complete();
				}
			} while (count > 0);
		}

		#endregion

		#region Build

		/// <summary>
		/// Build the specified views.
		/// </summary>
		/// <param name="views"></param>
		/// <param name="cmdLine"></param>
		private static void Build(IList<IView> views, OwlsUtilityCommandLine cmdLine)
		{
			var builders = CollectionUtils.Map(views, (IView view) => view.CreateBuilder());

			var buildArgs = new CreateBuildActionsArgs
			{
				Action = cmdLine.Action,
				ReadBatchSize = cmdLine.BatchSize,
				WriteBatchSize = cmdLine.BatchSize,
				ElevatedConnectionString = cmdLine.ElevatedConnectionString
			};
			foreach (var builder in builders)
			{

				var actions = builder.CreateBuildActions(buildArgs);
				foreach (var action in actions)
				{
					ExecuteAction(action, cmdLine);
				}
			}
		}

		/// <summary>
		/// Execute the specified view build action.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdLine"></param>
		private static void ExecuteAction(IViewBuildAction action, OwlsUtilityCommandLine cmdLine)
		{
			var startTime = DateTime.Now;
			var batchStartTime = DateTime.Now;
			using (var pscope = new PersistenceScope(PersistenceContextType.Update))
			{
				var context = (IUpdateContext)pscope.Context;
				context.ChangeSetRecorder = null;	// disable auditing

				action.Execute(context,
					delegate(PopulateViewProgress progress)
					{
						if (!string.IsNullOrEmpty(progress.Message))
							Platform.Log(LogLevel.Info, progress.Message);
						else
							LogProgress(progress.ProcessedItemCount, cmdLine.BatchSize, startTime, batchStartTime);

						batchStartTime = DateTime.Now;
					});

				pscope.Complete();
			}
		}

		#endregion

		private static void LogProgress(long total, int batchSize, DateTime startTime, DateTime batchStartTime)
		{
			var now = DateTime.Now;
			var averageRate = ((double)total) / (now - startTime).TotalSeconds;
			var instRate = ((double)batchSize) / (now - batchStartTime).TotalSeconds;
			Platform.Log(LogLevel.Info, "Processed {0} items total, Avg. rate = {1} items/sec, Inst. Rate = {2} items/sec", total, averageRate, instRate);
		}

		#endregion
	}
}
