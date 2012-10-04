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
using ClearCanvas.Enterprise.Core;
using System.Resources;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Helper class to populate or lookup value for a sepecialized <see cref="ServerEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">Specialized enumerated value type</typeparam>
    /// <typeparam name="TBroker">The broker class to be used to load the enumerated values.</typeparam>
    /// <remarks>
    /// </remarks>
    public class  ServerEnumHelper<TEnum, TBroker>
        where TEnum : ServerEnum, new()
        where TBroker : IEnumBroker<TEnum>
    {

        #region Static private members
		
        static readonly object _syncLock = new object();
    	private static bool _loaded = false;
        static List<TEnum> _list = new List<TEnum>();
        static readonly Dictionary<short, TEnum> _dict = new Dictionary<short, TEnum>();
        private static readonly IServerEnumExtension _extension;
        
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
            try
            {
                _extension = new ServerEnumExtensionPoint().CreateExtension() as IServerEnumExtension;
            }
            catch (Exception)
            {

            }
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
            dest.Description = GetLocalizedDescription(enumValue.Name, enumValue.Lookup);
            dest.LongDescription = GetLocalizedLongDescription(enumValue.Name, enumValue.Lookup);
        }

        #endregion Public methods

        #region Private Methods


        static public string GetDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_Description", enumName, enumLookupValue);
        }
        static public string GetLongDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_LongDescription", enumName, enumLookupValue);
        }

        static private string GetLocalizedDescription(string enumName, string enumLookupValue)
        {
            string key = GetDescriptionResKey(enumName, enumLookupValue);
            string desc = GetLocalizedText(key);
            return string.IsNullOrEmpty(desc) ? key : desc;
        }

        static private string GetLocalizedLongDescription(string enumName, string enumLookupValue)
        {
            string key = GetLongDescriptionResKey(enumName, enumLookupValue);
            string desc = GetLocalizedText(key);
            return string.IsNullOrEmpty(desc) ? key : desc;
        }

        static private string GetLocalizedText(string key)
        {
            if (_extension == null)
                return null;

            return _extension.GetLocalizedText(key);
        }

        #endregion

    }
    
    public interface IServerEnumExtension
    {
        string GetLocalizedText(string key);
    }

    [ExtensionPoint]
    public class ServerEnumExtensionPoint:ExtensionPoint<IServerEnumExtension>
    {
        
    }

    
}
