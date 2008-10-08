using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.ServerTree;
using IImageSopProvider = ClearCanvas.ImageViewer.StudyManagement.IImageSopProvider;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer {
	/// <summary>
	/// The study composer component, which is GUI-oriented component for manipulating studies using the <see cref="StudyBuilder"/> utility in the DICOM toolkit.
	/// </summary>
	/// <remarks>
	/// <para>The study composer component does not specify a particular model for how the information is presented in a GUI; rather, it wraps the
	/// <see cref="StudyBuilder"/> utility with classes that provide GUI-oriented features like thumbnail icons for individual nodes and dialogs to
	/// select publishing destinations. The composer component should only be consumed by an adapter component that provides a proper view of the composer.</para>
	/// </remarks>
	public class StudyComposerComponent : ApplicationComponent {
		private readonly StudyBuilder _studyBuilder;
		private readonly PatientItemCollection _patients;

		public StudyComposerComponent() {
			_studyBuilder = new StudyBuilder();
			_patients = new PatientItemCollection(_studyBuilder.Patients);
		}

		public PatientItemCollection Patients {
			get { return _patients; }
		}

		internal StudyBuilder StudyBuilder {
			get { return _studyBuilder; }
		}

		public override void Stop() {
			// if it becomes possible to start multiple study composer components, then this needs to be fixed, else images in use might become disposed
			ImageItem.ClearIconCache();

			base.Stop();
		}

		#region Insert Helpers

		public ImageItem InsertImage(IPresentationImage image) {
			ImageItem node = DoInsertImage(image);
			return node;
		}

		public IList<ImageItem> InsertImages(IEnumerable<IPresentationImage> images) {
			List<ImageItem> list = new List<ImageItem>();
			foreach (IPresentationImage image in images) {
				list.Add(DoInsertImage(image));
			}
			return list.AsReadOnly();
		}

		private ImageItem DoInsertImage(IPresentationImage pImage) {
			IImageSopProvider sop = pImage as IImageSopProvider;
			DicomFile dcf = sop.ImageSop.NativeDicomObject as DicomFile;
			PatientItem patient = _patients.GetById(dcf.DataSet);
			StudyItem study = patient.Studies.GetByUid(dcf.DataSet);
			SeriesItem series = study.Series.GetByUid(dcf.DataSet);
			ImageItem image = series.Images.GetByUid(dcf, pImage);
			return image;
		}

		#endregion

		#region Other Public Methods

		public void RefreshAllIcons(Size iconSize)
		{
			foreach (PatientItem patient in this.Patients)
			{
				patient.UpdateIcon(iconSize);
				foreach (StudyItem study in patient.Studies)
				{
					study.UpdateIcon(iconSize);
					foreach (SeriesItem series in study.Series)
					{
						series.UpdateIcon(iconSize);
						foreach (ImageItem image in series.Images)
						{
							image.UpdateIcon(iconSize);
						}
					}
				}
			}
		}


		#endregion

		#region Export Methods

		[Obsolete]
		public void Export(string path) {
			_studyBuilder.Publish(path);
		}

		public void PublishToDirectory()
		{
			//TODO pick a folder and export it...
		}

		public void PublishToServer()
		{
			AENavigatorComponent aeNavigator = new AENavigatorComponent(false, false, false, false);
			SimpleComponentContainer dialogContainer = new SimpleComponentContainer(aeNavigator);
			DialogBoxAction code = this.Host.DesktopWindow.ShowDialogBox(dialogContainer, "Select Target Server");

			if (code != DialogBoxAction.Ok)
				return;

			if (aeNavigator.SelectedServers == null || aeNavigator.SelectedServers.Servers == null || aeNavigator.SelectedServers.Servers.Count == 0) {
				return;
			}

			if (aeNavigator.SelectedServers.Servers.Count > 1) {
				return;
			}

			foreach (Server destinationAE in aeNavigator.SelectedServers.Servers) {
				_studyBuilder.Publish(ServerTree.GetClientAETitle(), destinationAE.AETitle, destinationAE.Host, destinationAE.Port);
			}
		}

		#endregion

	}
}
