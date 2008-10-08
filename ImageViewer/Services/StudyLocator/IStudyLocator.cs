using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.StudyLocator
{
	public class StudyLocatorNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/imageViewer/studyLocator";
	}

	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IStudyLocator", Namespace = StudyLocatorNamespace.Value)]
	public interface IStudyLocator : IStudyRootQuery
	{
		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> FindByStudyInstanceUid(string[] studyInstanceUids);

		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> FindByAccessionNumber(string accessionNumber);
	}
}
