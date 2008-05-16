using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Encapsulate the reference parameter used in the server rule action specifications. 
    /// </summary>
    /// <remarks>
    /// This class provides methods to map and retrieve the actual value of the reference field when applied to a specific <see cref="Context"/>.
    /// The value can be retrieved via <see cref="Value"/> which returns an appropriate object based on 
    /// the reference field. For Dicom attribute reference, a <See cref="DicomAttribute"/> object will be returned. 
    /// The tag that is referenced can be determined via <see cref="Tag"/>
    /// 
    /// <see cref="GetDicomValue{T}(T)"/> can be used to retrieve and convert the value of the referenced Dicom attribute to specific type.
    /// For eg, to retrieve the patient's birthdate as a DateTime object:
    /// 
    /// <code>
    /// ReferenceValue reference = new ReferenceValue("$PatientsBirthDate");
    /// reference.Context = ... // assign the context where the reference is applied to.
    /// DateTime value = Reference.GetDicomValue<DateTime>(Platform.Time); // retrieve the patient's birthdate
    /// </code>
    /// 
    /// </remarks>
    public class ReferenceValue
    {
        #region Private Members
        private readonly string _reference;
        private ServerActionContext _context;
        private DicomTag _tag;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ReferenceValue"/> based on a value of reference paramter.
        /// </summary>
        /// <param name="reference"></param>
        public ReferenceValue(string reference)
        {
            _reference = reference;

        }

        #endregion


        /// <summary>
        /// Returns a value indicating whether the reference paramter is a reference to a DICOM tag.
        /// </summary>
        /// <remarks>
        /// A Dicom tag reference starts with "$" followed by the name of the tag. For e.g., $PatientsName
        /// </remarks>
        public bool IsDicomTag
        {
            get
            {
                return (String.IsNullOrEmpty(_reference) == false && _reference[0] == '$');
            }
        }

        /// <summary>
        /// Returns the Dicom tag corresponding referenced by the reference parameter.
        /// </summary>
        /// <remarks>
        /// <b>null</b> is return if the reference parameter doesn't represent a tag or represent an invalid tag.
        /// </remarks>
        public DicomTag Tag
        {
            get
            {
                if (IsDicomTag)
                {
                    if (_tag == null)
                        _tag = ResolveReferencedTag(_reference);

                    return _tag;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the raw value of the reference paramter.
        /// </summary>
        /// <remarks>
        /// If the reference paramter refers to the dicom tag, the raw value is the <see cref="DicomAttribute"/> if it exists in
        /// the context. If the referene paramter is not a dicom tag, it is the value of the parameter itself.
        /// 
        /// The <see cref="Context"/> must be set prior to calling <see cref="Value"/>
        /// </remarks>
        public object Value
        {
            get
            {
                if (IsDicomTag)
                {
                    if (Context == null)
                        throw new Exception("Must set the context before calling ReferenceValue.Value");

                    DicomTag tag = Tag;
                    if (tag != null)
                    {
                        if (Context.Message.DataSet.Contains(tag))
                            return Context.Message.DataSet[tag].Values;
                        else if (Context.Message.MetaInfo.Contains(tag))
                            return Context.Message.MetaInfo[tag].Values;
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return _reference;
            }
        }

        /// <summary>
        /// Sets or gets the context the <see cref="ReferenceValue"/> will be applied to.
        /// </summary>
        public ServerActionContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// Retrieve the Dicom value of the referenced tag. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValueIfNotExist"></param>
        /// <returns></returns>
        public T GetDicomValue<T>(T defaultValueIfNotExist)
        {
            try
            {
                return GetDicomValue<T>(Tag, Context);
            }
            catch (ReferencedTagNotFoundException e)
            {
                Platform.Log(LogLevel.Warn, "Referenced tag {0} doesn't exist or is empty. Use default value {1}", Tag, defaultValueIfNotExist);
                return defaultValueIfNotExist;
            }
        }

        /// <summary>
        /// Returns the <see cref="DicomTag"/> of the referenced paramter.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        static protected DicomTag ResolveReferencedTag(string reference)
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <param name="defaultValueIfNotExist"></param>
        /// <returns></returns>
        static public T GetDicomValue<T>(DicomTag tag, ServerActionContext context, T defaultValueIfNotExist)
        {
            try
            {
                return GetDicomValue<T>(tag, context);
            }
            catch (ReferencedTagNotFoundException e)
            {
                Platform.Log(LogLevel.Warn, "Referenced tag {0} doesn't exist or is empty. Use default value {1}", tag, defaultValueIfNotExist);
                return defaultValueIfNotExist;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        static public T GetDicomValue<T>(DicomTag tag, ServerActionContext context)
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
                throw new ReferencedTagNotFoundException(context, String.Format("Referenced tag {0} is empty", tag));
            }

            if (attr.IsEmpty || attr.IsNull)
            {
                throw new ReferencedTagNotFoundException(context, String.Format("Referenced tag {0} doesn't exist", tag));
            }

            bool ok = false;

            if (typeof(T) == typeof(string))
            {
                string result;
                ok = attr.TryGetString(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve string value for referenced tag {0}", tag));

                return (T)(object)result;

            }
            else if (typeof(T) == typeof(int))
            {
                int result;
                ok = attr.TryGetInt32(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve int value for referenced tag {0}", tag));

                return (T)(object)result;

            }
            else if (typeof(T) == typeof(uint))
            {
                uint result;
                ok = attr.TryGetUInt32(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve string uint for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(long))
            {
                long result;
                ok = attr.TryGetInt64(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve string long for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(ulong))
            {
                ulong result;
                ok = attr.TryGetUInt64(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve ulong value for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(float))
            {
                float result;
                ok = attr.TryGetFloat32(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve float value for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(double))
            {
                double result;
                ok = attr.TryGetFloat64(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve double value for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                DateTime result;
                ok = attr.TryGetDateTime(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve DateTime value for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(DateTime?))
            {
                DateTime result;
                ok = attr.TryGetDateTime(0, out result);
                if (!ok)
                    throw new ServerActionException(context, String.Format("Unable to retrieve DateTime? value for referenced tag {0}", tag));

                return (T)(object)result;
            }
            else
            {
                throw new ApplicationException(
                    String.Format("GetReferencedDicomValue() called with unsupported type: {0}", typeof(T)));
            }

        }


    }
}
