#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Resources;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    [ExtensionOf(typeof(ServerEnumExtensionPoint))]
    public class ServerEnumDescription : IServerEnumDescriptionTranslator
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("ClearCanvas.ImageServer.Model.ServerEnumDescriptions", typeof(ServerEnumDescription).Assembly);

        
        public static string GetLocalizedDescription(ServerEnum enumValue)
        {
            return GetLocalizedText(GetDescriptionResKey(enumValue.Name, enumValue.Lookup));
        }

        public static string GetLocalizedLongDescription(ServerEnum enumValue)
        {
            return GetLocalizedText(GetLongDescriptionResKey(enumValue.Name, enumValue.Lookup));
        }

        #region IServerEnumDescriptionTranslator implementation

        string IServerEnumDescriptionTranslator.GetLocalizedLongDescription(ServerEnum serverEnum)
        {
            return GetLocalizedLongDescription(serverEnum);
        }

        string IServerEnumDescriptionTranslator.GetLocalizedDescription(ServerEnum serverEnum)
        {
            return GetLocalizedDescription(serverEnum);
        }

        #endregion

        #region Helper Methods


        static private string GetDescriptionResKey(string enumName, string enumLookupValue)
        {
            // This should match what's in ResxGenerator
            return string.Format("{0}_{1}_Description", enumName, enumLookupValue);
        }
        static private string GetLongDescriptionResKey(string enumName, string enumLookupValue)
        {
            // This should match what's in ResxGenerator
            return string.Format("{0}_{1}_LongDescription", enumName, enumLookupValue);
        }

        static private string GetLocalizedText(string key)
        {
            return _resourceManager.GetString(key);
        }

        #endregion

    }
}
