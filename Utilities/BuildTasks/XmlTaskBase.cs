#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	/// <summary>
	/// Base class for MSBuild <see cref="Task"/>s that operate on XML documents.
	/// </summary>
	public abstract class XmlTaskBase : Task
	{
		/// <summary>
		/// Initializes an <see cref="XmlTaskBase"/>.
		/// </summary>
		protected XmlTaskBase()
		{
			ErrorIfNotExists = true;
		}

		/// <summary>
		/// Gets or sets the path to the XML document file.
		/// </summary>
		[Required]
		public string File { get; set; }

		/// <summary>
		/// Gets or sets the XPath that refers to the XML nodes on which the operation will be performed.
		/// </summary>
		[Required]
		public string XPath { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not it is an error if the nodes selected by <see cref="XPath"/> do not exist. The default value is True.
		/// </summary>
		public bool ErrorIfNotExists { get; set; }

		/// <summary>
		/// Gets the current state of the loaded <see cref="XmlDocument"/>.
		/// </summary>
		/// <remarks>
		/// This property is only available while an <see cref="XmlTaskBase"/> is being executed.
		/// </remarks>
		protected XmlDocument XmlDocument { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not any changes were made to <see cref="XmlDocument"/>.
		/// </summary>
		protected bool Modified { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not it is an error if the XML document is empty or the file does not exist.
		/// </summary>
		protected virtual bool AllowEmptyDocument
		{
			get { return false; }
		}

		/// <summary>
		/// Flags the <see cref="Modified"/> flag if it is not already set.
		/// </summary>
		/// <param name="modified">A value indicating whether or not the flag should be set if it is not already set.</param>
		protected void FlagModified(bool modified)
		{
			Modified = Modified || modified;
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns>True if the task succeeded; False otherwise.</returns>
		public override sealed bool Execute()
		{
			// validate parameters
			string validationMessage;
			if (!ValidateParameters(out validationMessage))
			{
				Log.LogError(validationMessage);
				return false;
			}

			XmlDocument = new XmlDocument();
			Modified = false;
			try
			{
				if (!string.IsNullOrEmpty(File) && System.IO.File.Exists(File))
				{
					// load the file
					try
					{
						XmlDocument.Load(File);
					}
					catch (XmlException ex)
					{
						Log.LogError("Invalid XML near line {0}, position {1}", ex.LineNumber, ex.LinePosition);
						return false;
					}
				}
				else if (!AllowEmptyDocument)
				{
					// if empty documents are not allowed, error out now
					Log.LogError("File not found: {0}", File);
					return false;
				}

				// perform the task
				var result = PerformTask();
				if (result && Modified)
				{
					// save output if task was successful and changes were made
					if (!string.IsNullOrEmpty(File))
						XmlDocument.Save(File);
				}
				return result;
			}
#if !DEBUG
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}		
#endif
			finally
			{
				XmlDocument = null;
				Modified = false;
			}
		}

		/// <summary>
		/// Called to validate task parameters before the task is actually executed.
		/// </summary>
		/// <param name="message">A message to be logged if validation did not succeed.</param>
		/// <returns>True if validation succeeded; False otherwise.</returns>
		protected virtual bool ValidateParameters(out string message)
		{
			if (string.IsNullOrEmpty(File))
			{
				message = "File is a required parameter";
				return false;
			}

			if (string.IsNullOrEmpty(XPath))
			{
				message = "XPath is a required parameter";
				return false;
			}

			message = null;
			return true;
		}

		/// <summary>
		/// Called to execute the task.
		/// </summary>
		/// <param name="xmlNodes">A list of <see cref="XmlNode"/>s on which the operation is to be performed.</param>
		/// <returns>True if the task succeeded; False otherwise.</returns>
		protected abstract bool PerformTask(XmlNodeList xmlNodes);

		private bool PerformTask()
		{
			try
			{
				var xmlNodes = XmlDocument.SelectNodes(XPath) ?? new EmptyXmlNodeList();
				if (ErrorIfNotExists && xmlNodes.Count == 0)
				{
					Log.LogError("No results for XPath expression {0}", XPath);
					return false;
				}
				return PerformTask(xmlNodes);
			}
			catch (XPathException)
			{
				Log.LogError("Invalid XPath expression {0}", XPath);
				return false;
			}
		}

		/// <summary>
		/// Implementation of an empty <see cref="XmlNodeList"/>.
		/// </summary>
		private class EmptyXmlNodeList : XmlNodeList
		{
			private static readonly object[] _empty = new object[0];

			public override int Count
			{
				get { return 0; }
			}

			public override IEnumerator GetEnumerator()
			{
				return _empty.GetEnumerator();
			}

			public override XmlNode Item(int index)
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
}