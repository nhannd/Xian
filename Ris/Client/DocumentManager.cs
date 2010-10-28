#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public static class DocumentManager
	{
		private readonly static List<IFolderSystem> _folderSystems = new List<IFolderSystem>();
		private static readonly Dictionary<string, Document> _documentMap = new Dictionary<string, Document>();

		public static Document Get(string documentKey)
		{
			return !string.IsNullOrEmpty(documentKey) && _documentMap.ContainsKey(documentKey) ? _documentMap[documentKey] : null;
		}

		public static TDocument Get<TDocument>(EntityRef subject)
			where TDocument : Document
		{
			var documentKey = GenerateDocumentKey(typeof(TDocument), subject);
			return (TDocument) Get(documentKey);
		}

		public static List<TDocument> GetAll<TDocument>()
			where TDocument : Document
		{
			var documents = new List<TDocument>();
			var documentKeyBase = GenerateDocumentKey(typeof(TDocument), null);

			foreach (var key in _documentMap.Keys)
			{
				if (!string.IsNullOrEmpty(documentKeyBase) && key.Contains(documentKeyBase))
					documents.Add((TDocument) _documentMap[key]);
			}

			return documents;
		}

		public static string GenerateDocumentKey(Document doc, EntityRef subject)
		{
			return GenerateDocumentKey(doc.GetType(), subject);
		}

		public static void RegisterDocument(Document document)
		{
			if (!_documentMap.ContainsKey(document.Key))
				_documentMap[document.Key] = document;
		}

		public static void UnregisterDocument(Document document)
		{
			if (_documentMap.ContainsKey(document.Key))
				_documentMap.Remove(document.Key);
		}

		public static void RegisterFolderSystem(IFolderSystem folderSystem)
		{
			if (!_folderSystems.Contains(folderSystem))
				_folderSystems.Add(folderSystem);
		}

		public static void UnregisterFolderSystem(IFolderSystem folderSystem)
		{
			if (_folderSystems.Contains(folderSystem))
				_folderSystems.Remove(folderSystem);
		}

		public static void InvalidateFolder(Type folderType)
		{
			CollectionUtils.ForEach(_folderSystems,
				folderSystem => folderSystem.InvalidateFolders(folderType));
		}

		#region Private helper

		private static string GenerateDocumentKey(Type documentType, EntityRef subject)
		{
			return subject == null
				? string.Format("{0}", documentType)
				: string.Format("{0}+{1}", documentType, subject.ToString(false));
		}

		#endregion
	}
}
