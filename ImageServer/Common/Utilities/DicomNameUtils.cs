#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
