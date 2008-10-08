using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services.Automation;
using ClearCanvas.ImageViewer.Services.StudyLocator;

namespace ClearCanvas.Ris.Client.ViewerIntegration
{
	[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
	public class ViewerAutomationAdapter : IViewerIntegration
	{
		#region IViewerIntegration Members

		public void Open(string accessionNumber)
		{
			IList<StudyRootStudyIdentifier> studies = GetStudies(accessionNumber);

			IViewerAutomation automation = null;

			try
			{
				automation = Platform.GetService<IViewerAutomation>();

				OpenStudiesRequest request = new OpenStudiesRequest();
				request.ActivateIfAlreadyOpen = true;
				request.StudiesToOpen = CollectionUtils.Map<StudyRootStudyIdentifier, OpenStudyInfo>(studies,
																delegate(StudyRootStudyIdentifier id)
																{
																	return new OpenStudyInfo(id);
																});

				if (request.StudiesToOpen.Count == 0)
					throw new ArgumentException("Unable to locate any studies matching the given accession number.");

				automation.OpenStudies(request);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error opening studies via automation.");
				throw;
			}
			finally
			{
				if (automation is IDisposable)
					(automation as IDisposable).Dispose();
			}
		}

		public void Close(string accessionNumber)
		{
			IList<StudyRootStudyIdentifier> studies = GetStudies(accessionNumber);

			IViewerAutomation automation = null;
			try
			{
				automation = Platform.GetService<IViewerAutomation>();
				GetActiveViewerSessionsResult result = automation.GetActiveViewerSessions();
				List<ViewerSession> matchingSessions = CollectionUtils.Select(result.ActiveViewerSessions, 
					delegate(ViewerSession session)
						{
							return CollectionUtils.Contains(studies, 
								delegate(StudyRootStudyIdentifier identifier)
									{
										return identifier.StudyInstanceUid == session.PrimaryStudyInstanceUid;
									});
						});

				if (matchingSessions.Count > 0)
				{
					foreach (ViewerSession session in matchingSessions)
					{
						CloseViewerSessionRequest request = new CloseViewerSessionRequest();
						request.ViewerSession = session;
						automation.CloseViewerSession(request);
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error opening studies via automation.");
				throw;
			}
			finally
			{
				if (automation is IDisposable)
					(automation as IDisposable).Dispose();
			}
		}

		public void Activate(string accessionNumber)
		{
			IList<StudyRootStudyIdentifier> studies = GetStudies(accessionNumber);

			IViewerAutomation automation = null;
			try
			{
				automation = Platform.GetService<IViewerAutomation>();
				GetActiveViewerSessionsResult result = automation.GetActiveViewerSessions();
				ViewerSession matchingSession = CollectionUtils.SelectFirst(result.ActiveViewerSessions,
					delegate(ViewerSession session)
					{
						return CollectionUtils.Contains(studies,
							delegate(StudyRootStudyIdentifier identifier)
							{
								return identifier.StudyInstanceUid == session.PrimaryStudyInstanceUid;
							});
					});

				if (matchingSession != null)
				{
					ActivateViewerSessionRequest request = new ActivateViewerSessionRequest();
					request.ViewerSession = matchingSession;
					automation.ActivateViewerSession(request);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error opening studies via automation.");
				throw;
			}
			finally
			{
				if (automation is IDisposable)
					(automation as IDisposable).Dispose();
			}
		}

		#endregion

		private static IList<StudyRootStudyIdentifier> GetStudies(string accessionNumber)
		{
			IStudyLocator studyLocator;
			IList<StudyRootStudyIdentifier> results;

			try
			{
				studyLocator = Platform.GetService<IStudyLocator>();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error locating study locator service.");
				throw;
			}

			try
			{
				results = studyLocator.FindByAccessionNumber(accessionNumber);
			}
			finally
			{
				if (studyLocator is IDisposable)
					(studyLocator as IDisposable).Dispose();
			}

			return CollectionUtils.Sort(results, SortStudies);
		}

		private static int SortStudies(StudyRootStudyIdentifier x, StudyRootStudyIdentifier y)
		{
			DateTime? studyDateX = DateParser.Parse(x.StudyDate);
			DateTime? studyTimeX = DateParser.Parse(x.StudyTime);

			DateTime? studyDateY = DateParser.Parse(y.StudyDate);
			DateTime? studyTimeY = DateParser.Parse(y.StudyTime);

			DateTime? studyDateTimeX = studyDateX;
			if (studyDateTimeX != null && studyTimeX != null)
				studyDateTimeX = studyDateTimeX.Value.Add(studyTimeX.Value.TimeOfDay);

			DateTime? studyDateTimeY = studyDateY;
			if (studyDateTimeY != null && studyTimeY != null)
				studyDateTimeY = studyDateTimeY.Value.Add(studyTimeY.Value.TimeOfDay);

			if (studyDateTimeX == null)
			{
				if (studyDateTimeY == null)
					return Math.Sign(x.StudyInstanceUid.CompareTo(y.StudyInstanceUid));
				else
					return 1; // > because we want x at the end.
			}
			else if (studyDateY == null)
				return -1; // < because we want x at the beginning.

			//Return negative of x compared to y because we want most recent first.
			return -Math.Sign(studyDateTimeX.Value.CompareTo(studyDateTimeY));
		}
	}
}
