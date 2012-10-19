#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Helper class for RIS client-side auditing.
	/// </summary>
	/// <remarks>
	/// Static methods on this class are safe for concurrent access by multiple threads.
	/// </remarks>
	public static class AuditHelper
	{
		/// <summary>
		/// Defines the set of operations that are audited.
		/// </summary>
		public static class Operations
		{
			public const string FolderItemPreview = "FolderItem:Preview";
			public const string DocumentWorkspaceOpen = "Workspace:Open";
		}

		private static readonly AuditLog _auditLog = new AuditLog(ProductInformation.Component, "RIS");

		/// <summary>
		/// Log that the specified document workspace was opened.
		/// </summary>
		/// <param name="document"></param>
		public static void DocumentWorkspaceOpened(Document document)
		{
			var data = document.GetAuditData();
			if(data != null)
			{
				Log(data.Operation, data);
			}
		}

		/// <summary>
		/// Log that the preview page for the specified folder items was viewed.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="selectedItems"></param>
		public static void FolderItemPreviewed(IFolder folder, object[] selectedItems)
		{
			// the folder system can be null sometimes (e.g. a container folder),
			// in which case this can't be audited (and presumably doesn't need to be)
			if (folder.FolderSystem == null)
				return;

			var datas = folder.FolderSystem.GetPreviewAuditData(folder, selectedItems);
			foreach (var auditData in datas)
			{
				Log(auditData.Operation, auditData);
			}
		}

		/// <summary>
		/// Log the specified operation.
		/// </summary>
		/// <param name="operation">The name of the operation performed.</param>
		/// <param name="detailsDataContract">The audit message details.</param>
		private static void Log(string operation, object detailsDataContract)
		{
			if (LoginSession.Current == null)
				return;

			lock (_auditLog)
			{
				_auditLog.WriteEntry(operation, JsmlSerializer.Serialize(detailsDataContract, "Audit"));
			}
		}

	}
}
