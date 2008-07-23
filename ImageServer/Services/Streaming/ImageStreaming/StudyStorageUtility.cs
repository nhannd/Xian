using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    class StudyStorageUtility
    {
        private static Dictionary<string, ServerPartition> _partitions;

        private static ServerPartitionMonitor _monitor = ServerPartitionMonitor.Instance;

        public static StudyStorageLocation GetStudyStorageLocation(string serverAE, HttpListenerRequest request)
        {
            Platform.CheckForNullReference(serverAE, "serverAE");
            Platform.CheckForNullReference(request, "request");

            
            ServerPartition partition = _monitor.GetPartition(serverAE);

            if (partition==null)
                throw new WADOException(HttpStatusCode.NotFound, String.Format("Server with AE Title: {0} doesn't exist.", serverAE));

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                // TODO: Optimize this logic... we are querying the database for each requested image.
                // On the other hand, the current code can detect the situation where the study becomes locked (for update)
                // while the streaming is underway and return error to the client.

                
                IQueryStudyStorageLocation broker = ctx.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters parameters = new StudyStorageLocationQueryParameters();
                parameters.ServerPartitionKey = partition.GetKey();
                parameters.StudyInstanceUid = request.QueryString["studyUID"];
                IList<StudyStorageLocation> storages = broker.Execute(parameters);

                if (storages == null || storages.Count == 0)
                {
                    throw new WADOException(HttpStatusCode.NotFound, String.Format("Requested object doesn't exist on server {0}", serverAE));
                }
                else
                {
                    
                    return storages[0];
                }
            }
        }

    }
}
