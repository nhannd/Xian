#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Core.Query
{
    public static class QueryHelper
    {
        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for an array of matching string values.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        public static void SetStringArrayCondition(ISearchCondition<string> cond, string[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a DICOM range matching string value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        public static void SetRangeCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("-"))
            {
                string[] vals = val.Split(new[] { '-' });
                if (val.IndexOf('-') == 0)
                    cond.LessThanOrEqualTo(vals[1]);
                else if (val.IndexOf('-') == val.Length - 1)
                    cond.MoreThanOrEqualTo(vals[0]);
                else
                    cond.Between(vals[0], vals[1]);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        public static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
                String value = val.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a <see cref="ServerEntityKey"/> reference.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        public static void SetKeyCondition(ISearchCondition<ServerEntityKey> cond, ServerEntityKey[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        public static void SetXmlStringCondition(ISearchCondition<XmlDocument> cond, string xPath, string match)
        {
            var doc = new XmlDocument();

            var xPathElem = doc.CreateElement("Select");
            doc.AppendChild(xPathElem);

            var equalElem = doc.CreateElement("XPath");
            xPathElem.AppendChild(equalElem);

            var attribElem = doc.CreateAttribute("path");
            attribElem.Value = xPath;
            xPathElem.Attributes.Append(attribElem);

            if (match.Contains("*") || match.Contains("?"))
            {
                var value = match.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');

                attribElem = doc.CreateAttribute("value");
                attribElem.Value = value;
                xPathElem.Attributes.Append(attribElem);

                cond.Like(doc);
            }
            else
            {
                attribElem = doc.CreateAttribute("value");
                attribElem.Value = match;
                xPathElem.Attributes.Append(attribElem);

                cond.EqualTo(doc);
            }
        }
    }
}
