#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;
using ClearCanvas.ImageViewer.Services.Configuration;
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
			BackgroundTask task = new BackgroundTask(
				delegate {
					_studyBuilder.Publish(path);
				}, false);

			ProgressDialog.Show(task, base.Host.DesktopWindow, true, ProgressBarStyle.Marquee);
		}

		public void PublishToDirectory()
		{
			//TODO pick a folder and export it...
		}

		public void PublishToLocalDataStore()
		{
			try {
				DicomServerConfigurationHelper.Refresh(true);
				string aeTitle = DicomServerConfigurationHelper.AETitle;
				int port = DicomServerConfigurationHelper.Port;

				BackgroundTask task = new BackgroundTask(
					delegate {
						_studyBuilder.Publish(aeTitle, aeTitle, IPAddress.Loopback.ToString(), port);
						}, false);

				ProgressDialog.Show(task, base.Host.DesktopWindow, true, ProgressBarStyle.Marquee);
			} catch (Exception e) {
				ExceptionHandler.Report(e, SR.ExceptionLocalDataStoreNotFound , base.Host.DesktopWindow);
			}
		}

		public void PublishToServer()
		{
			AENavigatorComponent aeNavigator = new AENavigatorComponent();
			aeNavigator.IsReadOnly = true;
			aeNavigator.ShowCheckBoxes = false;
			aeNavigator.ShowLocalDataStoreNode = false;
			aeNavigator.ShowTitlebar = false;
			aeNavigator.ShowTools = false;

			SimpleComponentContainer dialogContainer = new SimpleComponentContainer(aeNavigator);
			DialogBoxAction code = this.Host.DesktopWindow.ShowDialogBox(dialogContainer, SR.SelectDestination );

			if (code != DialogBoxAction.Ok)
				return;

			if (aeNavigator.SelectedServers == null || aeNavigator.SelectedServers.Servers == null || aeNavigator.SelectedServers.Servers.Count == 0) {
				return;
			}

			if (aeNavigator.SelectedServers.Servers.Count < 1) {
				return;
			}

			BackgroundTask task = new BackgroundTask(
				delegate {
					foreach (Server destinationAE in aeNavigator.SelectedServers.Servers) {
						_studyBuilder.Publish(ServerTree.GetClientAETitle(), destinationAE.AETitle, destinationAE.Host, destinationAE.Port);
					}
				}, false);

			ProgressDialog.Show(task, base.Host.DesktopWindow, true, ProgressBarStyle.Marquee);
		}

		#endregion

	}
}
