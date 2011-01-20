#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class DicomNameUtils
    {
        [Flags]
        public enum NormalizeOptions
        {
            /// <summary>
            /// Remove redundant space character(s), including leading, trailing or in-between double-spaces
            /// </summary>
            TrimSpaces,

            /// <summary>
            /// Remove empty trailing component(s)
            /// </summary>
            TrimEmptyEndingComponents
        }

        static bool IsSet(NormalizeOptions value, NormalizeOptions flag)
        {
            return (value & flag) == flag;
        }

        public static String Normalize(string name, NormalizeOptions options)
        {
            string value = name.Trim();
            if (IsSet(options, NormalizeOptions.TrimSpaces))
            {
                value = Regex.Replace(value, "[ ]+", " "); // remove 
                value = Regex.Replace(value, "[ ]+\\^[ ]+|\\^[ ]+|[ ]+\\^", "^"); 
            }

            if (IsSet(options, NormalizeOptions.TrimEmptyEndingComponents))
            {
                value = Regex.Replace(value, "[\\^]*$", ""); // remove}
            }
            return value;
        }

        public static bool LookLikeSameNames(string name1, string name2)
        {
            name1 = StringUtilities.EmptyIfNull(name1);
            name2 = StringUtilities.EmptyIfNull(name2);
            string normalizedS1 = DicomNameUtils.Normalize(name1, DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);
            string normalizedS2 = DicomNameUtils.Normalize(name2, DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);


            // if both have "^", may need manual reconciliation
            // eg: "John ^ Smith" vs  "John  Smith^^" ==> manual
            //     "John ^ Smith" vs  "John ^ Smith^^" ==> auto
            if (name1.Contains("^") && name2.Contains("^"))
            {
                PersonName n1 = new PersonName(normalizedS1);
                PersonName n2 = new PersonName(normalizedS2);
                if (n1.AreSame(n2, PersonNameComparisonOptions.CaseInsensitive))
                    return true;
                else
                    return false;
            }


            if (normalizedS1.Length != normalizedS2.Length) return false;

            normalizedS1 = normalizedS1.ToUpper();
            normalizedS2 = normalizedS2.ToUpper();

            if (normalizedS1.Equals(normalizedS2))
                return true;

            for (int i = 0; i < normalizedS1.Length; i++)
            {
                // If S1[i] is ^ or space, S2[i] must be either ^ or space to be considered being the same
                // Otherwise, S1[i] must be the same as S2[i].
                if (normalizedS1[i] == '^' || normalizedS1[i] == ' ')
                {
                    if (normalizedS2[i] != '^' && normalizedS2[i] != ' ')
                        return false;
                }
                else
                {
                    if (normalizedS1[i] != normalizedS2[i])
                        return false;

                }
            }
            return true;
        }

        /// <summary>
        /// Returns a name that is the same as the two other names.
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <returns></returns>
        public static string ResolvePatientName(string name1, string name2)
        {
            if (!LookLikeSameNames(name1, name2))
                return null;

            name1 = Normalize(name1, NormalizeOptions.TrimSpaces | NormalizeOptions.TrimEmptyEndingComponents);
            name2 = Normalize(name2, NormalizeOptions.TrimSpaces | NormalizeOptions.TrimEmptyEndingComponents);

            if (name1.Length != name2.Length)
            {
                throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
            }

            StringBuilder value = new StringBuilder();
            for (int i = 0; i < name1.Length; i++)
            {
                if (Char.ToUpper(name1[i]) == Char.ToUpper(name2[i]))
                {
                    value.Append(name1[i]);
                }
                else
                {
                    if (name1[i] != '^' && name2[i] != '^')
                    {
                        throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
                    }
                    else // one of them is ^
                    {
                        if (name1[i] != ' ' && name2[i] != ' ')
                        {
                            throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
                        }
                        else
                        {
                            value.Append('^');
                        }
                    }
                }
            }
            return value.ToString();
        }

    }
}
