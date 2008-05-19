#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Provides methods to parse the reference value parameter in the server rule action specifications into objects
    /// that can be used to retrieve the actual value based on the context of the action.
    /// </summary>
    static public class ReferenceValueParser
    {
        #region Public Static Methods
        /// <summary>
        /// Returns the <see cref="IFunction{ServerActionContext}"/> corresponding to the specified text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static public IFunction<ServerActionContext> Parse(string text)
        {
            Platform.CheckForNullReference(text, "text");

            // if the text starts with "$", it's a dicom tag reference
            if (text.StartsWith("$"))
            {
                DicomTag tag = ParseReferencedTag(text);
                return new DicomTagEvalFunction(tag);
            }
            else
            {
                return new ConstantFunction(text);
            }

        }

        #endregion

        #region Private Static Methods
        /// <summary>
        /// Returns the <see cref="DicomTag"/> corresponds to the specified reference field.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static DicomTag ParseReferencedTag(string reference)
        {
            if (reference.Length < 2)
                throw new Exception("Expected referenced tag name following \'$\'");

            string tagName = reference.Substring(1);
            DicomTag tag = DicomTagDictionary.GetDicomTag(tagName);
            if (tag == null)
            {
                throw new Exception(String.Format("Specified tag name '{0}' is invalid or is a private tag", reference));
            }

            return tag;
        }
        #endregion
    }

    /// <summary>
    /// Elapsulates a reference value that is constant. 
    /// </summary>
    public class ConstantFunction : IFunction<ServerActionContext>
    {
        private readonly object _value;

        public ConstantFunction(object value)
        {
            Platform.CheckForNullReference(value, "value");

            _value = value;
        }

        #region IFunction<ServerActionContext> Members


        public T GetValue<T>(ServerActionContext input, T defaultValue)
        {
            try
            {
                return GetValue<T>(input);
            }
            catch (TypeConversionException)
            {
                return defaultValue;
            }
        }

        #endregion

        #region IFunction<ServerActionContext> Members

        public T GetValue<T>(ServerActionContext input)
        {
            if (typeof(T) == _value.GetType())
                return (T)_value;

            if (typeof(T) == typeof(string))
                return (T)(object)_value.ToString();

            else if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(_value.ToString());
            else if (typeof(T) == typeof(uint))
                return (T)(object)uint.Parse(_value.ToString());
            else if (typeof(T) == typeof(long))
                return (T)(object)long.Parse(_value.ToString());
            else if (typeof(T) == typeof(ulong))
                return (T)(object)ulong.Parse(_value.ToString());
            else if (typeof(T) == typeof(float))
                return (T)(object)float.Parse(_value.ToString());
            else if (typeof(T) == typeof(double))
                return (T)(object)double.Parse(_value.ToString());
            else if (typeof(T) == typeof(DateTime))
                return (T)(object)DateTime.Parse(_value.ToString());

            throw new TypeConversionException(String.Format("Unable to convert input type {0} to output type {1}", _value.GetType(), typeof(T)));
        }

        #endregion
    }
}
