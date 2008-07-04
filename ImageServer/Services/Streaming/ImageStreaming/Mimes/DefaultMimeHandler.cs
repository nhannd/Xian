using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Mimes
{
    [ExtensionOf(typeof(WADOMimeTypeExtensionPoint))]
    public class DefaultMimeHandler : IWADOMimeTypeHandler
    {
        private string[] _mimes = new string[] { "*/*", "application/dicom" };

        #region IWADOMimeTypeHandler Members

        public string[] SupportedRequestedMimeTypes
        {
            get
            {
                return _mimes;
            }
        }


        public WADOResponse GetResponseContent(HttpListenerRequest request)
        {
            WADOResponse response = new WADOResponse();
            string studyUid = request.QueryString["studyUID"];
            string seriesUid = request.QueryString["seriesUid"];
            string objectUid = request.QueryString["objectUid"];

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IQueryStudyStorageLocation broker = ctx.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters parameters = new StudyStorageLocationQueryParameters();
                parameters.StudyInstanceUid = studyUid;

                IList<StudyStorageLocation> storages = broker.Execute(parameters);

                if (storages == null || storages.Count == 0)
                {
                    throw new WADOException((int)HttpStatusCode.NotFound, "Requested object doesn't exist");
                }
                else
                {
                    StudyStorageLocation storage = storages[0];

                    // TODO: Lock the study.. 

                    string path = Path.Combine(storage.GetStudyPath(), seriesUid);
                    path = Path.Combine(path, objectUid + ".dcm");

                    if (!File.Exists(path))
                    {
                        throw new WADOException((int)HttpStatusCode.NoContent,
                                                String.Format("Could not locate the requested object. File doesn't exist: {0}", path));
                    }
                    else
                    {
                        response.ContentType = "application/dicom";
                        response.Stream = GetContentStream(path);

                    }

                }
            }

            return response;
        }

        #endregion

        private static Stream GetContentStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }

}
