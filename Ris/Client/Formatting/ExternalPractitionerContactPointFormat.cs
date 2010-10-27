#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class ExternalPractitionerContactPointFormat
    {
        /// <summary>
        /// Formats the address according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        public static string Format(ExternalPractitionerContactPointDetail cp)
        {
            return Format(cp, FormatSettings.Default.ExternalPractitionerContactPointDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - contact point name
        ///     %D - contact point description
        ///     %A - contact point address
        ///     %F - contact point fax
        ///     %T - contact point phone
        ///     %E - contact point email address
        /// </remarks>
        /// <returns></returns>
        public static string Format(ExternalPractitionerContactPointDetail cp, string format)
        {
            var result = format;
            result = result.Replace("%N", StringUtilities.EmptyIfNull(cp.Name));
            result = result.Replace("%D", StringUtilities.EmptyIfNull(cp.Description));
            result = result.Replace("%A", cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress));
            result = result.Replace("%F", cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber));
            result = result.Replace("%T", cp.CurrentPhoneNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber));
            result = result.Replace("%E", cp.CurrentEmailAddress == null ? "" : cp.CurrentEmailAddress.Address);

            return result.Trim();
        }
    }
}
