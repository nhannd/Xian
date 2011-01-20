#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
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
		static readonly object _syncLock = new object();
    	private static bool _loaded = false;
        static List<TEnum> _list = new List<TEnum>();
        static readonly Dictionary<short, TEnum> _dict = new Dictionary<short, TEnum>();
        #endregion Static private members

        #region Constructors
        /// <summary>
        /// ***** FOR INTERNAL USE ONLY ******
        /// </summary>
        private ServerEnumHelper()
        {
        }

        #endregion Constructors

        #region Public methods
		/// <summary>
		/// Load enumerated values from the db.
		/// </summary>
		/// <remarks>
		/// This code was originally contained within a static
		/// constructor for this class.  This was causing problems, however,
		/// when we would attempt to init a process, and the database was
		/// down.  I moved the code to a static method.
		/// </remarks>
		private static void LoadEnum()
		{
			lock (_syncLock)
			{
				if (_loaded)
					return;

				using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
					TBroker broker = read.GetBroker<TBroker>();
					_list = broker.Execute();
					foreach (TEnum value in _list)
					{
						_dict.Add(value.Enum, value);
					}
				}

				_loaded = true;
			}
		}
        public static TEnum GetEnum(string lookup)
        {
			LoadEnum();
            foreach (TEnum value in _dict.Values)
            {
                if (value.Lookup.Equals(lookup))
                    return value;
            }
            throw new PersistenceException(string.Format("Unknown {0} {1}", typeof(TEnum).Name, lookup), null);
        }

        public static List<TEnum> GetAll()
        {
			LoadEnum();
            return _list;
        }

        public static void SetEnum(TEnum dest, short val)
        {
			LoadEnum();
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
