using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	#region Exception Policy

	[ExceptionPolicyFor(typeof(LoadMultipleStudiesException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class LoadMultipleStudiesExceptionHandler : IExceptionPolicy
	{
		public LoadMultipleStudiesExceptionHandler()
		{
		}

		#region IExceptionPolicy Members

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

		public readonly int TotalStudies;
		public readonly IList<Exception> LoadStudyExceptions;

		public bool AllStudiesFailed
		{
			get { return LoadStudyExceptions.Count >= TotalStudies; }
		}

		public int GetNumberOffline()
		{
			return CollectionUtils.Select(LoadStudyExceptions,
						delegate(Exception e) { return e is OfflineLoadStudyException; }).Count;
		}

		public int GetNumberNearline()
		{
			return CollectionUtils.Select(LoadStudyExceptions,
						delegate(Exception e) { return e is NearlineLoadStudyException; }).Count;
		}

		public int GetNumberIncomplete()
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

		public int GetNumberInUse()
		{
			return CollectionUtils.Select(LoadStudyExceptions,
						delegate(Exception e) { return e is InUseLoadStudyException; }).Count;
		}

		public int GetNumberNotFound()
		{
			return CollectionUtils.Select(LoadStudyExceptions,
						delegate(Exception e) { return e is NotFoundLoadStudyException; }).Count;
		}

		public int GetNumberStudyLoaderNotFound()
		{
			return CollectionUtils.Select(LoadStudyExceptions,
						delegate(Exception e) { return e is StudyLoaderNotFoundException; }).Count;
		}

		public int GetNumberUnknownFailures()
		{
			return LoadStudyExceptions.Count - GetNumberOffline() - GetNumberNearline() - GetNumberIncomplete()
			       - GetNumberInUse() - GetNumberNotFound() - GetNumberStudyLoaderNotFound();
		}

		public string GetExceptionSummary()
		{
			StringBuilder summary = new StringBuilder();

			int numberOffline = GetNumberOffline();
			int numberNearline = GetNumberNearline();
			int numberIncomplete = GetNumberIncomplete();
			int numberInUse = GetNumberInUse();
			int numberNotFound = GetNumberNotFound();
			int numberUnknown = GetNumberUnknownFailures();

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
