#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Base class for all objects that serve as WCF data contracts.
    /// </summary>
    [DataContract]
    public abstract class DataContractBase : IExtensibleDataObject
    {
        private ExtensionDataObject _extensionData;

        #region IExtensibleDataObject Members

        public ExtensionDataObject ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion
    }
}
