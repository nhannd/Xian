#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Common.Specifications
{
    public interface ISpecificationXmlSource
    {
        string DefaultExpressionLanguage { get; }
        XmlElement GetSpecificationXml(string id);
        IDictionary<string, XmlElement> GetAllSpecificationsXml();
    }
}
