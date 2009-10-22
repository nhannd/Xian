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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public static class DocumentManager
	{
		private readonly static List<IFolderSystem> _folderSystems = new List<IFolderSystem>();

		public static Workspace Get<TDocument>(EntityRef subject)
			where TDocument : Document
		{
			var documentKey = GenerateDocumentKey(typeof(TDocument), subject);
			return Get(documentKey);
		}

		public static List<Workspace> GetAll<TDocument>()
			where TDocument : Document
		{
			var documents = new List<Workspace>();
			var documentKeyBase = GenerateDocumentKey(typeof(TDocument), null);

			foreach (var window in Desktop.Application.DesktopWindows)
			{
				foreach (var workspace in window.Workspaces)
				{
					if (!string.IsNullOrEmpty(workspace.Name) && workspace.Name.Contains(documentKeyBase))
						documents.Add(workspace);
				}
			}

			return documents;
		}

		public static Workspace Get(string documentKey)
		{
			foreach (var window in Desktop.Application.DesktopWindows)
			{
				if (window.Workspaces.Contains(documentKey))
					return window.Workspaces[documentKey];
			}

			return null;
		}

		public static string GenerateDocumentKey(Document doc, EntityRef subject)
		{
			return GenerateDocumentKey(doc.GetType(), subject);
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
