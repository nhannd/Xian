using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public interface IEnumBroker : IPersistenceBroker
    {
        IList<EnumValue> Load(Type enumValueClass);
        IList<TEnumValue> Load<TEnumValue>() where TEnumValue : EnumValue;

        EnumValue Lookup(Type enumValueClass, string code);
        TEnumValue Lookup<TEnumValue>(string code) where TEnumValue : EnumValue;
    }

    /// <summary>
    /// Defines the interface for a broker that returns a list of <see cref="EnumValue"/>.
    /// </summary>
    /// <typeparam name="E">The <see cref="EnumValue"/> sub-class that this broker returns.</typeparam>
    //public interface IEnumBroker<TEnum> : IPersistenceBroker
    //{
        /// <summary>
        /// Loads the domain enumeration from a persistent store.
        /// </summary>
        /// <returns></returns>
        //IList<TEnum> Load();

        //TEnum Lookup(string code);
    //}
}
