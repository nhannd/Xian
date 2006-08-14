using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyLoaderExtensionPoint))]
    public class LocalDataStoreStudyLoader : StudyLoader
    {
        public LocalDataStoreStudyLoader()
        {

        }

        public override string Name
        {
            get
            {
                return "My DataStore";
            }
        }

        public override void LoadStudy(string studyUID)
        {
            try
            {
                IStudy study = DataAbstractionLayer.GetIDataStore().GetStudy(new Uid(studyUID));
                IEnumerable<ISopInstance> listOfSops = study.GetSopInstances();

                bool errorMessageShown = false;

                foreach (ISopInstance sop in listOfSops)
                {
                    // TODO: don't just skip non-image sop instances
                    ImageSopInstance imageObject = sop as ImageSopInstance;
                    if (null == imageObject)
                        continue;

                    // skip non local objects
                    if (!imageObject.LocationUri.IsFile)
                        continue;

                    LocalDataStoreImageSop localImage = new LocalDataStoreImageSop(imageObject);                  

                    try
                    {
                        ImageWorkspace.StudyManager.StudyTree.AddImage(localImage);
                    }
                    catch (ImageValidationException e)
                    {
                        // Only bug the user once...
                        if (!errorMessageShown)
                        {
                            Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorAtLeastOneImageFailedToLoad);
                            errorMessageShown = true;
                        }

                        // ...but log everytime
                        Platform.Log(e, LogLevel.Warn);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
                Platform.ShowMessageBox("Can't connect to data store: " + e.ToString());
            }

            base.LoadStudy(studyUID);
        }

        //private void MapImageObjectToLocalImageObject(ImageSopInstance imageObject, IDicomPropertySettable localImage)
        //{
        //    ClearCanvas.Dicom.DataStore.Study study = imageObject.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Study;
        //    ClearCanvas.Dicom.DataStore.Series series = imageObject.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Series;

        //    // string properties
        //    localImage.SetStringProperty("PatientId", study.PatientId);
        //    localImage.SetStringProperty("StudyInstanceUid", study.StudyInstanceUid);
        //    localImage.SetStringProperty("SeriesInstanceUid", series.SeriesInstanceUid);
        //    localImage.SetStringProperty("TransferSyntaxUid", imageObject.TransferSyntaxUid);
        //    localImage.SetStringProperty("SopInstanceUid", imageObject.SopInstanceUid);

        //    // integer properties
        //    localImage.SetInt32Property("SamplesPerPixel", imageObject.SamplesPerPixel);
        //    localImage.SetInt32Property("Rows", imageObject.Rows);
        //}
    }
}
