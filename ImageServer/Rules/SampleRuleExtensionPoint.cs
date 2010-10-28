#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Plugin for Sample Rules used in the Web GUI.
    /// </summary>
    [ExtensionPoint()]
    public class SampleRuleExtensionPoint : ExtensionPoint<ISampleRule>
    {
    }
}