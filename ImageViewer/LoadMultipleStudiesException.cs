using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	#region Exception Policy

	[ExceptionPolicyFor(typeof(LoadMultipleStudiesException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal class LoadMultipleStudiesExceptionHandler : IExceptionPolicy
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LoadMultipleStudiesExceptionHandler()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			LoadMultipleStudiesException exception = (LoadMultipleStudiesException) e;

			exceptionHandlingContext.Log(LogLevel.Error, e);
			
			StringBuilder summary = new StringBuilder();

			summary.AppendLine(SR.MessageLoadMultipleStudiesFailurePrefix);
			summary.AppendLine(exception.GetExceptionSummary());
			summary.Append(SR.MessageContactPacsAdmin);

			exceptionHandlingContext.ShowMessageBox(summary.ToString());
		}

		#endregion
	}

	#endregion

	/// <summary>
	/// Exception thrown when multiple studies have failed to load.
	/// </summary>
	/// <seealso cref="ImageViewerComponent.LoadStudies"/>
	public class LoadMultipleStudiesException : Exception
	{
		internal LoadMultipleStudiesException(ICollection<Exception> exceptions, int totalStudies)
			: this(FormatMessage(exceptions, totalStudies), exceptions, totalStudies)
		{
		}

		internal LoadMultipleStudiesException(string message, IEnumerable<Exception> exceptions, int totalStudies)
			: base(message)
		{
			TotalStudies = totalStudies;
			LoadStudyExceptions = new List<Exception>(exceptions).AsReadOnly();
		}

		/// <summary>
		/// Gets the total number of studies that were loaded, or attempted to load.
		/// </summary>
		public readonly int TotalStudies;

		/// <summary>
		/// Gets all of the individual exceptions that occurred per study.
		/// </summary>
		public readonly IList<Exception> LoadStudyExceptions;

		/// <summary>
		/// Gets whether or not all the studies failed to load.
		/// </summary>
		public bool AllStudiesFailed
		{
			get { return LoadStudyExceptions.Count >= TotalStudies; }
		}

		/// <summary>
		/// Gets the number of studies that failed to load because they are offline.
		/// </summary>
		public int OfflineCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
				                              delegate(Exception e) { return e is OfflineLoadStudyException; }).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that failed to load because they are nearline.
		/// </summary>
		public int NearlineCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
				                              delegate(Exception e) { return e is NearlineLoadStudyException; }).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that could only be partially loaded.
		/// </summary>
		public int IncompleteCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
							delegate(Exception e)
							{
								if (e is LoadStudyException)
									return ((LoadStudyException)e).PartiallyLoaded;
								else
									return false;
							}).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that failed to load because they are in use.
		/// </summary>
		public int InUseCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
				                              delegate(Exception e) { return e is InUseLoadStudyException; }).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that failed to load because they could not be found.
		/// </summary>
		public int NotFoundCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
							delegate(Exception e) { return e is NotFoundLoadStudyException; }).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that failed to load because there was no appropriate <see cref="IStudyLoader"/>.
		/// </summary>
		public int StudyLoaderNotFoundCount
		{
			get
			{
				return CollectionUtils.Select(LoadStudyExceptions,
							delegate(Exception e) { return e is StudyLoaderNotFoundException; }).Count;
			}
		}

		/// <summary>
		/// Gets the number of studies that failed to load for unknown reasons.
		/// </summary>
		public int UnknownFailureCount
		{
			get
			{
				return LoadStudyExceptions.Count - OfflineCount - NearlineCount - InUseCount -
					NotFoundCount - IncompleteCount - StudyLoaderNotFoundCount;
			}
		}

		/// <summary>
		/// Gets a text summary of the exception(s).
		/// </summary>
		public string GetExceptionSummary()
		{
			StringBuilder summary = new StringBuilder();

			int numberOffline = OfflineCount;
			int numberNearline = NearlineCount;
			int numberIncomplete = IncompleteCount;
			int numberInUse = InUseCount;
			int numberNotFound = NotFoundCount;
			int numberUnknown = UnknownFailureCount;

			if (numberIncomplete > 0)
			{
				if (numberIncomplete == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyIncomplete);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesIncomplete, numberIncomplete);
				}
			}

			if (numberNotFound > 0)
			{
				if (summary.Length > 0)
					summary.AppendLine();

				if (numberNotFound == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyNotFound);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesNotFound, numberNotFound);
				}
			}

			if (numberInUse > 0)
			{
				if (summary.Length > 0)
					summary.AppendLine();

				if (numberInUse == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyInUse);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesInUse, numberInUse);
				}
			}

			if (numberOffline > 0)
			{
				if (summary.Length > 0)
					summary.AppendLine();

				if (numberOffline == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyOffline);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesOffline, numberOffline);
				}
			}

			if (numberNearline > 0)
			{
				if (summary.Length > 0)
					summary.AppendLine();

				if (numberNearline == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyNearline);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesNearline, numberNearline);
				}
			}

			if (numberUnknown > 0)
			{
				if (summary.Length > 0)
					summary.AppendLine();

				if (numberUnknown == 1)
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageOneStudyNotLoaded);
				}
				else
				{
					summary.Append(" - ");
					summary.AppendFormat(SR.MessageFormatXStudiesNotLoaded, numberUnknown);
				}
			}

			return summary.ToString();
		}

		private static string FormatMessage(ICollection<Exception> exceptions, int totalStudies)
		{
			return String.Format("{0} of {1} studies produced one or more errors while loading.", exceptions.Count, totalStudies);
		}
	}
}
