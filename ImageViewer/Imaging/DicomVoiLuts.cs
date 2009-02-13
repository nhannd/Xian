using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IDicomVoiLutsProvider {
		IDicomVoiLuts DicomVoiLuts { get; }
	}

	public interface IDicomVoiLuts
	{
		IList<Window> ImageVoiLinearLuts { get; }
		IList<string> ImageVoiLinearLutExplanations { get; }
		IList<VoiDataLut> ImageVoiDataLuts { get; }
		string ImageSopInstanceUid { get; }
		int ImageSopFrameNumber { get; }

		IList<Window> PresentationVoiLinearLuts { get; }
		IList<string> PresentationVoiLinearLutExplanations { get; }
		IList<VoiDataLut> PresentationVoiDataLuts { get; }
		string PresentationStateSopInstanceUid { get; }
	}

	internal sealed class DicomVoiLuts : IDicomVoiLuts {
		private readonly IImageSopProvider _image;

		internal DicomVoiLuts(IImageSopProvider image)
		{
			_image = image;
		}

		#region Presentation Luts

		[CloneIgnore]
		private readonly List<Window> _presentationVoiLinearLuts = new List<Window>();

		[CloneIgnore]
		private readonly List<string> _presentationVoiLinearLutExplanations = new List<string>();

		[CloneIgnore]
		private readonly List<VoiDataLut> _presentationVoiDataLuts = new List<VoiDataLut>();

		[CloneIgnore]
		private string _sourcePresentationSopUid = "";

		public string PresentationStateSopInstanceUid
		{
			get { return _sourcePresentationSopUid; }
		}

		public IList<Window> PresentationVoiLinearLuts
		{
			get { return _presentationVoiLinearLuts.AsReadOnly(); }
		}

		public IList<string> PresentationVoiLinearLutExplanations
		{
			get { return _presentationVoiLinearLutExplanations.AsReadOnly(); }
		}

		public IList<VoiDataLut> PresentationVoiDataLuts {
			get { return _presentationVoiDataLuts.AsReadOnly(); }
		}

		internal void ReinitializePresentationLuts(string sourceSopUid)
		{
			_sourcePresentationSopUid = sourceSopUid;
			_presentationVoiLinearLuts.Clear();
			_presentationVoiLinearLutExplanations.Clear();
			_presentationVoiDataLuts.Clear();
		}

		internal void AddPresentationLinearLut(double width, double center, string explanation)
		{
			_presentationVoiLinearLuts.Add(new Window(width, center));
			_presentationVoiLinearLutExplanations.Add(explanation);
		}

		internal void AddPresentationDataLut(VoiDataLut dataLut)
		{
			_presentationVoiDataLuts.Add(dataLut);
		}

		#endregion

		#region Image Luts

		public string ImageSopInstanceUid
		{
			get { return _image.ImageSop.SopInstanceUID; }
		}

		public int ImageSopFrameNumber
		{
			get { return _image.Frame.FrameNumber; }
		}

		public IList<Window> ImageVoiLinearLuts {
			get
			{
				Window[] windows = _image.Frame.WindowCenterAndWidth;
				if (windows == null || windows.Length == 0)
					return new List<Window>().AsReadOnly();
				return new List<Window>(windows).AsReadOnly();
			}
		}

		public IList<string> ImageVoiLinearLutExplanations {
			get
			{
				string[] explanations = _image.Frame.WindowCenterAndWidthExplanation;
				if (explanations == null || explanations.Length == 0)
					return new List<string>().AsReadOnly();
				return new List<string>(explanations).AsReadOnly();
			}
		}

		public IList<VoiDataLut> ImageVoiDataLuts {
			get { return _image.ImageSop.VoiDataLuts; }
		}

		#endregion
	}
}