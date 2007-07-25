using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.IO;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class InvokeOperationResponse : DataContractBase
    {
        /// <summary>
        /// The response object from the invoked service operation.
        /// </summary>
        [DataMember]
        public JsmlBlob ResponseJsml;
    }
}
