using System;
using System.Collections.Generic;
using System.Text;
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
        /// </remarks>
        /// <returns></returns>
        public static string Format(ExternalPractitionerContactPointDetail cp, string format)
        {
            string result = format;
            result = result.Replace("%N", StringUtilities.EmptyIfNull(cp.Name));
            result = result.Replace("%D", StringUtilities.EmptyIfNull(cp.Description));
            result = result.Replace("%A", cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress));
            result = result.Replace("%F", cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber));
            result = result.Replace("%T", cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber));

            return result.Trim();
        }
    }
}
