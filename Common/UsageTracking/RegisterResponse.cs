using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Request object for the <see cref="IUsageTracking.Register"/>
    /// </summary>
    [DataContract]
    public class RegisterResponse : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// A message to be displayed on the client.
        /// </summary>
        [DataMember]
        public DisplayMessage Message { get; set; }       
    }
}
