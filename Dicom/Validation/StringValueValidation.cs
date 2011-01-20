#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Validation
{
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
    public static class DAStringValidator
    {
        public static void ValidateString(DicomTag tag, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;

            string[] temp = stringValue.Split(new[] {'\\'});
            foreach (string value in temp)
            {
                string s = value.Trim();
                DateTime dt;
                if (!string.IsNullOrEmpty(s) && !DateParser.Parse(s, out dt))
                {
                    throw new DicomDataException(string.Format("Invalid DA value '{0}' for  {1}", value, tag));
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
    public static class DSStringValidator
    {

        public static void ValidateString(DicomTag tag, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;

            string[] temp = stringValue.Split(new[] { '\\' });
            foreach (string s in temp)
            {
                decimal decVal;
                if (!string.IsNullOrEmpty(s) && !decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out decVal))
                {
                    throw new DicomDataException(string.Format("Invalid DS value '{0}' for {1}", stringValue, tag));
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
    public static class DTStringValidator
    {
        public static void ValidateString(DicomTag tag, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;

            string[] temp = stringValue.Split(new[] {'\\'});
            foreach (string s in temp)
            {
                DateTime dtVal;
                if (!string.IsNullOrEmpty(s) && !DateTimeParser.Parse(s, out dtVal))
                {
                    throw new DicomDataException(string.Format("Invalid DT value {0} for {1}", s, tag));
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
    public class ISStringValidator
    {
        public static void ValidateString(DicomTag tag, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;

            string[] temp = stringValue.Split(new[] {'\\'});
            foreach (string s in temp)
            {
                decimal decVal;
                if (!string.IsNullOrEmpty(s) &&
                    !decimal.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out decVal))
                {
                    throw new DicomDataException(string.Format("Invalid IS value {0} for {1}", stringValue,
                                                               tag));
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
    public class TMStringValidator
    {
        public static void ValidateString(DicomTag tag, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;

            string[] temp = stringValue.Split(new[] {'\\'});
            foreach (string value in temp)
            {
                string s = value.Trim();
                DateTime dt;
                if (!string.IsNullOrEmpty(s) && !TimeParser.Parse(s, out dt))
                {
                    throw new DicomDataException(string.Format("Invalid TM value '{0}' for {1}", value, tag));
                }
            }
        }
    }
}
