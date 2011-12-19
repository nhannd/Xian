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
using Microsoft.Build.Framework;

namespace ClearCanvas.Utilities.BuildTasks
{
	/// <summary>
	/// A task for deleting nodes from an XML document.
	/// </summary>
	/// <remarks>
	/// This task deletes the nodes selected by the <see cref="XmlTaskBase.XPath"/> expression.
	/// </remarks>
	public class XmlDelete : XmlTaskBase
	{
		/// <summary>
		/// Outputs whether or not the selected nodes were deleted.
		/// </summary>
		[Output]
		public bool Deleted { get; set; }

		protected override bool PerformTask(XmlNodeList xmlNodes)
		{
			foreach (XmlNode xmlNode in xmlNodes)
			{
				if (xmlNode is XmlAttribute)
				{
					var xmlAttribute = (XmlAttribute) xmlNode;
					if (xmlAttribute.OwnerElement != null)
						xmlAttribute.OwnerElement.Attributes.Remove(xmlAttribute);
				}
				else if (xmlNode is XmlEntity || xmlNode is XmlNotation)
				{
					if (xmlNode.OwnerDocument != null)
						xmlNode.OwnerDocument.RemoveChild(xmlNode);
				}
				else if (xmlNode.ParentNode != null)
				{
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
				else
				{
					throw new NotSupportedException("Attempt to remove unsupported XmlNode");
				}
			}

			FlagModified(Deleted = xmlNodes.Count > 0);
			return true;
		}
	}
}