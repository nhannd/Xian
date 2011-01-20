#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Core.Reconcile.Discard;
using ClearCanvas.ImageServer.Core.Reconcile.MergeStudy;
using ClearCanvas.ImageServer.Core.Reconcile.ProcessAsIs;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Parses reconciliation commands in Xml format.
	/// </summary>
	/// <remarks>
	/// Currently only "MergeStudy", "CreateStudy" or "Discard" commands are supported.
	/// 
	/// <example>
	/// The following examples shows the xml of the "Discard" and "CreateStudy" commands
	/// <code>
	/// <Discard></Discard>
	/// </code>
	/// 
	/// <code>
	/// <CreateStudy>
	///     <Set TagPath="00100010" Value="John^Smith"/>
	/// </CreateStudy>
	/// </code>
	/// </example>
	/// </remarks>
	public class ReconcileCommandXmlParser
	{
		#region Public Methods
		/// <summary>
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		/// <remarks>
		/// The reconciliation commands should be specified in <ImageCommands> node.
		/// </remarks>
		public IReconcileProcessor Parse(XmlDocument doc)
		{
			//TODO: Validate the xml
			Platform.CheckForNullReference(doc, "doc");

			if (doc.DocumentElement!=null)
			{
				StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
				StudyReconcileDescriptor desc = parser.Parse(doc);
				switch(desc.Action)
				{
					case StudyReconcileAction.CreateNewStudy: return new ReconcileCreateStudyProcessor();
					case StudyReconcileAction.Discard: return new DiscardImageCommandProcessor();
					case StudyReconcileAction.Merge: return new MergeStudyCommandProcessor();
					case StudyReconcileAction.ProcessAsIs: return new ReconcileProcessAsIsProcessor();
					default:
						throw new NotSupportedException(String.Format("Reconcile Action: {0}", desc.Action));
				}
                
			}

			return null;
		}
		#endregion
	}
}