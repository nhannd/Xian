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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Helper class to populate or lookup value for a sepecialized <see cref="ServerEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">Specialized enumerated value type</typeparam>
    /// <typeparam name="TBroker">The broker class to be used to load the enumerated values.</typeparam>
    /// <remarks>
    /// </remarks>
    sealed public class  ServerEnumHelper<TEnum, TBroker>
        where TEnum : ServerEnum, new()
        where TBroker : IEnumBroker<TEnum>
    {
        #region Static private members
        static IList<TEnum> _list = new List<TEnum>();
        static Dictionary<short, TEnum> _dict = new Dictionary<short, TEnum>();
        #endregion Static private members

        #region Constructors
        /// <summary>
        /// ***** FOR INTERNAL USE ONLY ******
        /// </summary>
        private ServerEnumHelper()
        {
        }

        static ServerEnumHelper()
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                TBroker broker = read.GetBroker<TBroker>();
                _list = broker.Execute();
                foreach (TEnum value in _list)
                {
                    _dict.Add(value.Enum, value);
                }
            }
        }
        #endregion Constructors

        #region Public methods
        public static TEnum GetEnum(string lookup)
        {
            foreach (TEnum value in _dict.Values)
            {
                if (value.Lookup.Equals(lookup))
                    return value;
            }
            throw new PersistenceException(string.Format("Unknown {0} {1}", typeof(TEnum).Name, lookup), null);
        }

        public static IList<TEnum> GetAll()
        {
            return _list;
        }

        public static void SetEnum(TEnum dest, short val)
        {
            TEnum enumValue;
            if (false == _dict.TryGetValue(val, out enumValue))
                throw new PersistenceException(string.Format("Unknown {0} value: {1}", typeof(TEnum).Name, val), null);

            dest.Enum = enumValue.Enum;
            dest.Lookup = enumValue.Lookup;
            dest.Description = enumValue.Description;
            dest.LongDescription = enumValue.LongDescription;
        }

        #endregion Public methods

    }
}
