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
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Core
{
    public interface IEnumBroker : IPersistenceBroker
    {
        /// <summary>
        /// Loads all enumeration values for the specified enumeration class, optionally including de-activated values.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="includeDeactivated"></param>
        /// <returns></returns>
        IList<EnumValue> Load(Type enumValueClass, bool includeDeactivated);

        /// <summary>
        /// Loads all enumeration values for the specified enumeration class, optionally including de-activated values.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <returns></returns>
        IList<TEnumValue> Load<TEnumValue>(bool includeDeactivated) where TEnumValue : EnumValue;

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
        /// Trys to find the enumeration value for the specified enumeration class and enumeration code.
        /// Throws a <see cref="EnumValueNotFoundException"/> if the class does not contain the specified code.
        /// </summary>
        /// <remarks>
        /// This method is less efficient than the corresponding <see cref="Find"/> method and should only be used 
        /// if the requested code is not known to be part of the enumeration.
        /// </remarks>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        EnumValue TryFind(Type enumValueClass, string code);

        /// <summary>
        /// Trys to find the enumeration value for the specified enumeration class and enumeration code.
        /// Throws a <see cref="EnumValueNotFoundException"/> if the class does not contain the specified code.
        /// </summary>
        /// <remarks>
        /// This method is less efficient than the corresponding <see cref="Find"/> method and should only be used 
        /// if the requested code is not known to be part of the enumeration.
        /// </remarks>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        TEnumValue TryFind<TEnumValue>(string code) where TEnumValue : EnumValue;

        /// <summary>
        /// Adds a new value to the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <returns></returns>
        EnumValue AddValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated);

        /// <summary>
        /// Updates the value of the specified enumeration class and code, with the supplied arguments.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <param name="deactivated"></param>
        /// <returns></returns>
        EnumValue UpdateValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated);

        /// <summary>
        /// Removes the value with the specified code from the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        void RemoveValue(Type enumValueClass, string code);
    }
}
