using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    class StudyStorageUtility
    {
        public static StudyStorageLocation GetStudyStorageLocation(HttpListenerRequest request)
        {
            string studyUid = request.QueryString["studyUID"];

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                // TODO: Optimize this logic... we are querying the database for each requested image.
                // On the other hand, the current code can detect the situation where the study becomes locked (for update)
                // while the streaming is underway and return error to the client.
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

                    return storage;
                }
            }
        }

    }
}
