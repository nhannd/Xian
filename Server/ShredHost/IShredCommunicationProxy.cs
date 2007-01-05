using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceContract]
    public interface IShredCommunication
    {
        /// <summary>
        /// Requests the shred stop processing. All
        /// resources should be deallocated.
        /// </summary>
        [OperationContract]
        void Stop();
        /// <summary>
        /// Requests the shred pause processing
        /// This is different from Stop in the sense
        /// that resources are not deallocated.
        /// </summary>
        [OperationContract]
        void Pause();
        /// <summary>
        /// Unpause a paused shred.
        /// </summary>
        [OperationContract]
        void Unpause();
    }
}
