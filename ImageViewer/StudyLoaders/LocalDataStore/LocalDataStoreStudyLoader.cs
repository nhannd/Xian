namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using ClearCanvas.Common;
    using ClearCanvas.ImageViewer.StudyManagement;
	using ClearCanvas.ImageViewer.Imaging;
	using ClearCanvas.DataStore;
    using ClearCanvas.Dicom;

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
                _connectionString.Load();
                DatabaseConnector database = new DatabaseConnector(_connectionString);
                database.SetupConnector();
                List<LocationUri> locationArray = database.SopInstanceLocationQuery(new Uid(studyUID));

                bool errorMessageShown = false;

                foreach (LocationUri locationUri in locationArray)
                {
                    LocalDataStoreImageSop image = new LocalDataStoreImageSop(locationUri.LocationPart);

                    database.SopInstanceColumnDataLoad(image as IDicomPropertySettable, locationUri);

                    try
                    {
                        ImageWorkspace.StudyManager.StudyTree.AddImage(image);
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
                database.TeardownConnector();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                // TODO
                Platform.ShowMessageBox("Can't connect to data store: " + e.ToString());
            }

            base.LoadStudy(studyUID);
        }

        private ApplicationConnectionString _connectionString = new ApplicationConnectionString();
    }
}
