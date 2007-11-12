using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;

namespace ClearCanvas.Dicom.Validation
{
    public abstract class StringValueValidator
    {
    
        /// <summary>
        /// ****** INTERNAL USE ONLY ****
        /// 
        /// Returns a validator instance lookup key for a DicomAttribute attribute.
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static internal int GetKey(DicomAttribute attribute)
        {
            // use attribute parent collection hashcode as key
            // if the attribute doesn't have a parent collection, use its type instead
            int key;
            if (attribute.ParentCollection != null)
                key = attribute.ParentCollection.GetHashCode();
            else
                key = attribute.GetType().GetHashCode();

            return key;
        }
        
        /// <summary>
        /// Validate a string.
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="stringValue"></param>
        abstract public void ValidateString(DicomTag tag, string stringValue);

    }

    /// <summary>
    /// DAStringValidator to check if a string can be used as a DA attribute
    /// 
    /// </summary>
    /// <remarks>
    /// Flyweight pattern is used to reduce number of validator objects created.
    /// The idea is that all DicomAttribute instances of the same type 
    /// can share the same validator if they belong to the same collection.
    /// All "stand-alone" attributes can also share the same validator.
    /// 
    /// <P>Note:Because of this sharing, all methods are written to ensure they are thread-safe
    /// 
    /// </remarks>
    public class DAStringValidator : StringValueValidator
    {
        static protected Hashtable _map = new Hashtable();
        static Object _mutex = new Object();

        /// <summary>
        /// Return an instance of the DAStringValidator object
        /// Note: the returned object may be shared among threads or objects.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///</remarks>
        public static DAStringValidator GetInstance(DicomAttribute attribute)
        {
            lock (_mutex)
            {
                int key = GetKey(attribute);

                DAStringValidator validator = _map[key] as DAStringValidator;
                if (validator == null)
                {
                    validator = new DAStringValidator();
                    _map[key] = validator;
                }
                return validator;
            }
            
        }

        /// <summary>
        /// **** INTERNAL USE ONLY ****
        /// Use <seealso cref="GetInstance"/> to obtain a DAStringValidator object.
        /// </summary>
        private DAStringValidator()
        {
        }

        public override void ValidateString(DicomTag tag, string stringValue)
        {
            lock (_mutex)
            {
                if (stringValue == null || stringValue == "")
                    return;

                DateTime dt;
                string[] temp = stringValue.Split(new char[] { '\\' });
                foreach (string value in temp)
                {
                    string s = value.Trim();
                    if (s != null && s != "" && !DateParser.Parse(s, out dt))
                    {
                        throw new DicomDataException(string.Format("Invalid DA value '{0}' for  {1}", value, tag.ToString()));
                    }
                }
            }
            

        }
    }

    /// <summary>
    /// DSStringValidator to check if a string can be used as a DS attribute.
    ///
    /// </summary>
    /// <remarks>
    /// Flyweight pattern is used to reduce number of validator objects created.
    /// The idea is that all DicomAttribute instances of the same type 
    /// can share the same validator if they belong to the same collection.
    /// All "stand-alone" attributes can also share the same validator.
    /// 
    /// <P>Note:Because of this sharing, all methods are written to ensure they are thread-safe
    /// 
    /// </remarks>
    public class DSStringValidator : StringValueValidator
    {
        static protected Hashtable _map = new Hashtable();
        static Object _mutex = new Object();

        /// <summary>
        /// Return an instance of the DSStringValidator object
        /// Note: the returned object may be shared among threads or objects.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///</remarks>
        public static DSStringValidator GetInstance(DicomAttribute attribute)
        {
            lock (_mutex)
            {
                int key = GetKey(attribute);

                DSStringValidator validator = _map[key] as DSStringValidator;
                if (validator == null)
                {
                    validator = new DSStringValidator();
                    _map[key] = validator;
                }
                return validator;
            }
            
        }
        
        /// <summary>
        /// **** INTERNAL USE ONLY ****
        /// Use <seealso cref="GetInstance"/> to obtain a DSStringValidator object.
        /// </summary>
        private DSStringValidator()
        {
        }

        public override void ValidateString(DicomTag tag, string stringValue)
        {
            lock (_mutex)
            {
                if (stringValue == null || stringValue == "")
                    return;

                string[] temp = stringValue.Split(new char[] { '\\' });
                decimal decVal;
                foreach (string s in temp)
                {
                    if (s != null && s != "" && !decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out decVal))
                    {
                        throw new DicomDataException(string.Format("Invalid DS value '{0}' for {1}", stringValue, tag.ToString()));
                    }
                }
            }
            
        }
    }

    /// <summary>
    /// DTStringValidator  to check if a string can be used as a DT attribute
    /// </summary>
    /// <remarks>
    /// Flyweight pattern is used to reduce number of validator objects created.
    /// The idea is that all DicomAttribute instances of the same type 
    /// can share the same validator if they belong to the same collection.
    /// All "stand-alone" attributes can also share the same validator.
    /// 
    /// <P>Note:Because of this sharing, all methods are written to ensure they are thread-safe
    /// 
    /// </remarks>
    public class DTStringValidator : StringValueValidator
    {
        static protected Hashtable _map = new Hashtable();
        static Object _mutex = new Object();

        /// <summary>
        /// Return an instance of the DTStringValidator object
        /// Note: the returned object may be shared among threads or objects.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///</remarks>
        public static DTStringValidator GetInstance(DicomAttribute attribute)
        {
            lock (_mutex)
            {
                int key = GetKey(attribute);

                DTStringValidator validator = _map[key] as DTStringValidator;
                if (validator == null)
                {
                    validator = new DTStringValidator();
                    _map[key] = validator;
                }
                return validator;
            }
        }

        /// <summary>
        /// **** INTERNAL USE ONLY ****
        /// Use <seealso cref="GetInstance"/> to obtain a DTStringValidator object.
        /// </summary>
        private DTStringValidator()
        {
        }
        public override void ValidateString(DicomTag tag, string stringValue)
        {
            lock (_mutex)
            {
                if (stringValue == null || stringValue == "")
                    return;

                string[] temp = stringValue.Split(new char[] { '\\' });
                DateTime dtVal;
                foreach (string s in temp)
                {
                    if (s != null && s != "" && !DateTimeParser.Parse(s, out dtVal))
                    {
                        throw new DicomDataException(string.Format("Invalid DT value {0} for {1}", s, tag.ToString()));
                    }
                }
            }
        }
    }

    /// <summary>
    /// ISStringValidator to check if a string can be used as a IS attribute
    /// </summary>
    /// <remarks>
    /// Flyweight pattern is used to reduce number of validator objects created.
    /// The idea is that all DicomAttribute instances of the same type 
    /// can share the same validator if they belong to the same collection.
    /// All "stand-alone" attributes can also share the same validator.
    /// 
    /// <P>Note:Because of this sharing, all methods are written to ensure they are thread-safe
    /// 
    /// </remarks>
    public class ISStringValidator : StringValueValidator
    {
        static protected Hashtable _map = new Hashtable();
        static Object _mutex = new Object();


        /// <summary>
        /// Return an instance of the ISStringValidator object
        /// Note: the returned object may be shared among threads or objects.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///</remarks>
        public static ISStringValidator GetInstance(DicomAttribute attribute)
        {
            lock (_mutex)
            {
                int key = GetKey(attribute);

                ISStringValidator validator = _map[key] as ISStringValidator;
                if (validator == null)
                {
                    validator = new ISStringValidator();
                    _map[key] = validator;
                }
                return validator;
            }
        }

        /// <summary>
        /// **** INTERNAL USE ONLY ****
        /// Use <seealso cref="GetInstance"/> to obtain a ISStringValidator object.
        /// </summary>
        private ISStringValidator()
        {
        }
        public override void ValidateString(DicomTag tag, string stringValue)
        {
            lock (_mutex)
            {
                if (stringValue == null || stringValue == "")
                    return;

                string[] temp = stringValue.Split(new char[] { '\\' });
                decimal decVal;
                foreach (string s in temp)
                {
                    if (s != null && s != "" && !decimal.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out decVal))
                    {
                        throw new DicomDataException(string.Format("Invalid IS value {0} for {1}", stringValue, tag.ToString()));
                    }
                }
            }
        }
    }

    /// <summary>
    /// TMStringValidator to check if a string can be used as a TM attribute
    /// </summary>
    /// <remarks>
    /// Flyweight pattern is used to reduce number of validator objects created.
    /// The idea is that all DicomAttribute instances of the same type 
    /// can share the same validator if they belong to the same collection.
    /// All "stand-alone" attributes can also share the same validator.
    /// 
    /// <P>Note:Because of this sharing, all methods are written to ensure they are thread-safe
    /// 
    /// </remarks>
    public class TMStringValidator : StringValueValidator
    {
        static protected Hashtable _map = new Hashtable();
        static Object _mutex = new Object();

        /// <summary>
        /// Return an instance of the TMStringValidator object
        /// Note: the returned object may be shared among threads or objects.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///</remarks>
        public static TMStringValidator GetInstance(DicomAttribute attribute)
        {
            lock (_mutex)
            {
                int key = GetKey(attribute);

                TMStringValidator validator = _map[key] as TMStringValidator;
                if (validator == null)
                {
                    validator = new TMStringValidator();
                    _map[key] = validator;
                }
                return validator;
            }
        }

        /// <summary>
        /// **** INTERNAL USE ONLY ****
        /// Use <seealso cref="GetInstance"/> to obtain a TMStringValidator object.
        /// </summary>
        private TMStringValidator()
        {
        }
        public override void ValidateString(DicomTag tag, string stringValue)
        {
            lock (_mutex)
            {
                if (stringValue == null || stringValue == "")
                    return;

                DateTime dt;
                string[] temp = stringValue.Split(new char[] { '\\' });
                foreach (string value in temp)
                {
                    string s = value.Trim();
                    if (s != null && s != "" && !TimeParser.Parse(s, out dt))
                    {
                        throw new DicomDataException(string.Format("Invalid TM value '{0}' for {1}", value, tag.ToString()));
                    }
                }
            }
        }
    }
}
