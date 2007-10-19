#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Reflection;
using ClearCanvas.Common;
using NHibernate;
using NHibernate.Cfg;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
    {
	    private static readonly Configuration _hibernateConfiguration;
		private static readonly ISessionFactory _sessionFactory;

		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, IDicomDictionary> _dicomDictionaries;

		private DataAccessLayer()
		{
		}

		static DataAccessLayer()
		{
			_hibernateConfiguration = new Configuration();
			string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
			_hibernateConfiguration.Configure(assemblyName + ".cfg.xml");
			_hibernateConfiguration.AddAssembly(assemblyName);
			_sessionFactory = _hibernateConfiguration.BuildSessionFactory();

			_dicomDictionaries = new Dictionary<string, IDicomDictionary>();
		}

		private static ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

		public static IDataStoreReader GetIDataStoreReader()
        {
			return new DataStoreReader(SessionManager.Get());
        }

		public static IDicomPersistentStore GetIDicomPersistentStore()
		{
			return new DicomPersistentStore();
		}

		public static IDataStoreStudyRemover GetIDataStoreStudyRemover()
		{
			return new DataStoreWriter(SessionManager.Get());
		}

		internal static IDataStoreWriter GetIDataStoreWriter()
        {
			return new DataStoreWriter(SessionManager.Get());
        }
		
		internal static IDicomDictionary GetIDicomDictionary()
		{
			return GetIDicomDictionary(DicomDictionary.DefaultDictionaryName);
		}

		internal static IDicomDictionary GetIDicomDictionary(string dictionaryName)
		{
			lock (_syncLock)
			{
				if (_dicomDictionaries.ContainsKey(dictionaryName))
					return _dicomDictionaries[dictionaryName];

				DicomDictionary newDictionary = new DicomDictionary(SessionManager.Get(), dictionaryName);
				_dicomDictionaries[dictionaryName] = newDictionary;
				return newDictionary;
			}
		}

		#region IInitializeAssociatedObject Members

		public static void InitializeAssociatedObject(object entity, object association)
		{
			Platform.CheckForNullReference(entity, "entity");
			Platform.CheckForNullReference(association, "association");

			using (ISessionManager sessionManager = SessionManager.Get())
			{
				try
				{
					if (sessionManager.Session.Contains(association))
						return;

					sessionManager.BeginReadTransaction();
					sessionManager.Session.Lock(entity, LockMode.Read);
					NHibernateUtil.Initialize(association);
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatFailedToInitializeAssociation, association.GetType(), entity.GetType());
					throw new DataStoreException(message, e);
				}
			}
		}

		#endregion
	}
}
