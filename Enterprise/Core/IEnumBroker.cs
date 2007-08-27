using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public interface IEnumBroker : IPersistenceBroker
    {
        /// <summary>
        /// Loads all enumeration values for the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <returns></returns>
        IList<EnumValue> Load(Type enumValueClass);

        /// <summary>
        /// Loads all enumeration values for the specified enumeration class.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <returns></returns>
        IList<TEnumValue> Load<TEnumValue>() where TEnumValue : EnumValue;

        /// <summary>
        /// Finds the enumeration value for the specified enumeration class and enumeration code.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        EnumValue Find(Type enumValueClass, string code);

        /// <summary>
        /// Finds the enumeration value for the specified enumeration class and enumeration code.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        TEnumValue Find<TEnumValue>(string code) where TEnumValue : EnumValue;

        /// <summary>
        /// Adds a new value to the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        EnumValue AddValue(Type enumValueClass, string code, string value, string description);

        /// <summary>
        /// Updates the value of the specified enumeration class and code, with the supplied arguments.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        EnumValue UpdateValue(Type enumValueClass, string code, string value, string description);

        /// <summary>
        /// Removes the value with the specified code from the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        void RemoveValue(Type enumValueClass, string code);
   }
}
