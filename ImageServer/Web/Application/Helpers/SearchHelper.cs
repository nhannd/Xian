#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    public class SearchHelper
    {
        public static string TrailingWildCard(string searchText)
        {
            if(ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if(!searchText.Contains("*")) searchText = searchText + "*";
            }
            return searchText;
        }

        public static string LeadingWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if(!searchText.Contains("*")) searchText = "*" + searchText;
            }
            return searchText;
        }

        public static string LeadingAndTrailingWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if(!searchText.Contains("*"))searchText = "*" + searchText + "*";
            }
            return searchText;
        }

        public static string NameWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if (!searchText.Contains("*"))
                {
                    string[] names = searchText.Split(',');
                    if (names.Length == 2)
                    {
                        searchText = names[0].Trim() + "*" + names[1].Trim() + "*";
                    }
                    else
                    {
                        searchText = "*" + searchText + "*";
                    }
                }
            }
            return searchText;
        }
    }
}
