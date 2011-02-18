#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    static public class ServerEnumDescription
    {
        static ImageServerServerEnumExtension translator = new ImageServerServerEnumExtension();
        
        public static string GetLocalizedDescription(ServerEnum enumValue)
        {
            return translator.GetLocalizedText(GetDescriptionResKey(enumValue.Name, enumValue.Lookup));
        }

        static private string GetDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_Description", enumName, enumLookupValue);
        }
        static private string GetLongDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_LongDescription", enumName, enumLookupValue);
        }
    }
}
