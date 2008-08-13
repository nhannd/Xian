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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using System.IO;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
    {
	    private static readonly Configuration _hibernateConfiguration;
		private static readonly ISessionFactory _sessionFactory;
		private static volatile string _fileStoreDirectory;

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
		}

		internal static Configuration HibernateConfiguration
		{
			get { return _hibernateConfiguration; }	
		}

		private static ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

		public static void SetFileStoreDirectory(string directory)
		{
			if (String.IsNullOrEmpty(directory))
				throw new ArgumentException("The specified directory must not be empty.");
			if (!Directory.Exists(directory))
				throw new ArgumentException(String.Format("The specified directory does not exist ({0})", directory));

			_fileStoreDirectory = Path.GetFullPath(directory);
		}

		public static IDataStoreReader GetIDataStoreReader()
        {
			return new DataStoreReader(SessionManager.Get());
        }

		public static IDicomPersistentStoreValidator GetIDicomPersistentStoreValidator()
		{
			return new DicomPersistentStoreValidator();
		}

		public static IDicomPersistentStore GetIDicomPersistentStore()
		{
			if (_fileStoreDirectory == null)
				throw new InvalidOperationException("The file store directory must be set before the persistent store can be used.");

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

		internal static string GetFileStoreDirectory()
		{
			return _fileStoreDirectory;
		}

		#region Helper Methods

		internal static IEnumerable<T> Cast<T>(IEnumerable original)
		{
			foreach (T item in original)
				yield return item;
		}

		private static bool ContainsWildCharacters(string criteria)
		{
			return criteria.Contains("*") || criteria.Contains("?");
		}

		#endregion
	}
}
