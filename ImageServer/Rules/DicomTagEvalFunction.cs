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
    /// The exception that is thrown when the referenced dicom tag doesn't exist when <see cref="DicomTagEvalFunction"/> is evaluated.
    /// doesn't exist or has no value.
    /// </summary>
    public class ReferencedTagNotFoundException : Exception
    {
        public ReferencedTagNotFoundException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Evaluates the value of a dicom tag in a <see cref="ServerActionContext"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides methods to retrieve the actual value of the referenced dicom tag when applied to a specific <see cref="ServerActionContext"/>.
    /// The interested tag is specified when instantiating the <see cref="DicomTagEvalFunction"/> object. The actual dicom tag value can be retrieved 
    /// via <see cref="GetValue{T}(ServerActionContext)"/>. Type conversion will be performed automatically when possible.
    /// </para>
    /// <example>
    /// <para>
    /// The following code retrieves the patient's birthdate associated with the Dicom message in the context and stores 
    /// the returned value in a DateTime object:
    /// </para>
    /// 
    /// <code>
    /// ServerActionContext context = ....
    /// DicomTagEvalFunction reference = new DicomTagEvalFunction(DicomTags.PatientsBirthDate);
    /// DateTime value = reference.GetValue<DateTime?>(context, null); // retrieve the patient's birthdate
    /// </code>
    /// </example>
    /// </remarks>
    public class DicomTagEvalFunction : IFunction<ServerActionContext>
    {
        #region Private Members
        private readonly DicomTag _tag;
        #endregion


        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="DicomTagEvalFunction"/> for a specified tag.
        /// </summary>
        /// <param name="tag"></param>
        public DicomTagEvalFunction(DicomTag tag)
        {
            _tag = tag;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the referenced Dicom tag whose value is to be evaluated.
        /// </summary>
        /// <remarks>
        /// <b>null</b> is return if the reference field doesn't represent a tag or represent an invalid tag.
        /// </remarks>
        public DicomTag Tag
        {
            get
            {
               return _tag;
            }
        }

        #endregion

        #region Protected Static Methods
        /// <summary>
        /// Returns the value of a Dicom tag in the specified context and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static T GetDicomValue<T>(DicomTag tag, ServerActionContext context)
        {
            Platform.CheckForNullReference(tag, "tag");
            Platform.CheckForNullReference(context, "context");

            DicomAttribute attr = null;
            if (context.Message.DataSet.Contains(tag))
                attr = context.Message.DataSet[tag];
            else if (context.Message.MetaInfo.Contains(tag))
                attr = context.Message.MetaInfo[tag];

            if (attr == null)
            {
                throw new ReferencedTagNotFoundException(String.Format("Referenced tag {0} is empty", tag));
            }

            if (attr.IsEmpty || attr.IsNull)
            {
                throw new ReferencedTagNotFoundException(String.Format("Referenced tag {0} doesn't exist", tag));
            }

            bool ok = false;

            if (typeof (T) == typeof (string))
            {
                string result;
                ok = attr.TryGetString(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve string value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (int))
            {
                int result;
                ok = attr.TryGetInt32(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve int value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (uint))
            {
                uint result;
                ok = attr.TryGetUInt32(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve string uint for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (long))
            {
                long result;
                ok = attr.TryGetInt64(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve string long for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (ulong))
            {
                ulong result;
                ok = attr.TryGetUInt64(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve ulong value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (float))
            {
                float result;
                ok = attr.TryGetFloat32(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve float value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (double))
            {
                double result;
                ok = attr.TryGetFloat64(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve double value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (DateTime))
            {
                DateTime result;
                ok = attr.TryGetDateTime(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve DateTime value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else if (typeof (T) == typeof (DateTime?))
            {
                DateTime result;
                ok = attr.TryGetDateTime(0, out result);
                if (!ok)
                    throw new TypeConversionException(String.Format(
                                                        "Unable to retrieve DateTime? value for referenced tag {0}", tag));

                return (T) (object) result;
            }
            else
            {
                throw new TypeConversionException(
                    String.Format("GetReferencedDicomValue() called with unsupported type: {0}", typeof (T)));
            }
        }

        #endregion


        #region IFunction<ServerActionContext> Members


        /// <summary>
        /// Evaluates the value of the dicom tag in the given context.
        /// </summary>
        /// <typeparam name="T">Expected type of the return value</typeparam>
        /// <param name="input"></param>
        /// <param name="defaultValue">Default returned value if the interested tag doesn't exist in the context</param>
        /// <returns></returns>
        public T GetValue<T>(ServerActionContext input, T defaultValue)
        {
            try
            {
                return GetValue<T>(input);
            }
            catch (ReferencedTagNotFoundException)
            {
                Platform.Log(LogLevel.Warn, "Referenced tag {0} doesn't exist or is empty. Use default value {1}", _tag, defaultValue);
                return defaultValue;
            }
        }

        /// <summary>
        /// Evaluates the value of the dicom tag in the given context.
        /// </summary>
        /// <typeparam name="T">Expected type of the return value</typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ReferencedTagNotFoundException"/> thrown when <see cref="Tag"/> doesn't exist in the context.</exception>
        public T GetValue<T>(ServerActionContext input)
        {
            return GetDicomValue<T>(_tag, input);
        }

        #endregion
    }
}