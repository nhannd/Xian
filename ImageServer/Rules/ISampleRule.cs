#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    public interface ISampleRule
    {
        string Name { get; }
        string Description { get; }
        ServerRuleTypeEnum Type { get; }
        IList<ServerRuleApplyTimeEnum> ApplyTimeList { get; }
        XmlDocument Rule { get; }
    }
}