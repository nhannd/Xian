#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    public class EnumUtils
    {
        /// <summary>
        /// Converts a <see cref="EnumValue"/> to a <see cref="EnumValueInfo"/> object.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static EnumValueInfo GetEnumValueInfo(EnumValue enumValue)
        {
            return enumValue == null ? null : new EnumValueInfo(enumValue.Code, enumValue.Value, enumValue.Description);
        }

        /// <summary>
        /// Converts a C# enum value to a <see cref="EnumValueInfo"/> object.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="code"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static EnumValueInfo GetEnumValueInfo<TEnum>(TEnum code, IPersistenceContext context)
            where TEnum : struct
        {
            EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
                typeof(TEnum).GetCustomAttributes(typeof(EnumValueClassAttribute), false));

            if(attr == null)
                throw new ArgumentException(string.Format("{0} is not marked with the EnumValueClassAttribute", typeof(TEnum).FullName));

            EnumValue enumValue = context.GetBroker<IEnumBroker>().Find(attr.EnumValueClass, code.ToString());
            return GetEnumValueInfo(enumValue);
        }

        /// <summary>
        /// Reads the value property of the specified enumValue in a null-safe manner
        /// (e.g returns null if the argument is null).
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayValue(EnumValue enumValue)
        {
            return enumValue == null ? null : enumValue.Value;
        }

        /// <summary>
        /// Gets the value corresponding to the specified enum code.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="code"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetValue<TEnum>(TEnum code, IPersistenceContext context)
            where TEnum : struct
        {
            return GetEnumValueInfo<TEnum>(code, context).Value;
        }

        /// <summary>
        /// Returns a list of <see cref="EnumValueInfo"/> objects representing the active values in the specified enumeration class.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<EnumValueInfo> GetEnumValueList<TEnumValue>(IPersistenceContext context)
            where TEnumValue : EnumValue
        {
            return CollectionUtils.Map<EnumValue, EnumValueInfo, List<EnumValueInfo>>(context.GetBroker<IEnumBroker>().Load<TEnumValue>(false),
                delegate(EnumValue ev)
                {
                    return new EnumValueInfo(ev.Code, ev.Value, ev.Description);
                });
        }

        /// <summary>
        /// Converts a <see cref="EnumValueInfo"/> to a subclass of <see cref="EnumValue"/>.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TEnumValue GetEnumValue<TEnumValue>(EnumValueInfo info, IPersistenceContext context)
            where TEnumValue : EnumValue
        {
            return info == null ? null : context.GetBroker<IEnumBroker>().Find<TEnumValue>(info.Code);
        }

        /// <summary>
        /// Converts a <see cref="EnumValueInfo"/> to a C# enum value.  If info is null,
        /// the default (0) value for the enum is returned.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TEnum GetEnumValue<TEnum>(EnumValueInfo info)
            where TEnum : struct
        {
            string code = info != null ? info.Code : Enum.GetName(typeof(TEnum), 0);
            return (TEnum)Enum.Parse(typeof(TEnum), code);
        }
    }
}
