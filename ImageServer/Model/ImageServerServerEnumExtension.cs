#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

using ClearCanvas.ImageServer.Enterprise;
using System.Resources;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Model
{
    [ExtensionOf(typeof(ServerEnumExtensionPoint))]
    public class ImageServerServerEnumExtension : IServerEnumExtension
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("ClearCanvas.ImageServer.Model.ServerEnumDescriptions", typeof (ImageServerServerEnumExtension).Assembly);

        public string GetLocalizedText(string key)
        {
            return _resourceManager.GetString(key);
        }
    }
}
