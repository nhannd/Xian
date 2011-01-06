#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CannedTextCategoryEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class CannedTextCategoryEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextCategoryEditorComponent class.
	/// </summary>
	[AssociateView(typeof(CannedTextCategoryEditorComponentViewExtensionPoint))]
	public class CannedTextCategoryEditorComponent : ApplicationComponent
	{
		private readonly List<string> _categoryChoices;
		private string _category;

		public CannedTextCategoryEditorComponent(List<string> categoryChoices, string initialCategory)
		{
			_categoryChoices = categoryChoices;
			_category = initialCategory;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			// Insert a blank choice as the first element
			_categoryChoices.Insert(0, "");

			base.Start();
		}

		[ValidateNotNull]
		public string Category
		{
			get { return _category; }
			set
			{
				_category = value;
				this.Modified = true;
			}
		}

		public IList CategoryChoices
		{
			get { return _categoryChoices; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}
