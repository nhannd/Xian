#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common
{
    /// <summary>
    /// For clients that don't have type information.
    /// </summary>
    [DataContract(Namespace = Namespace.Value)]
    public abstract class DataContractBase
    {
        protected DataContractBase()
        {
            Name = GetType().Name;
            Name = GetType().FullName;
        }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string FullName { get; set; }
    }
}
