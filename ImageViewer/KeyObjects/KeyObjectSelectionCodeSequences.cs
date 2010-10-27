#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// Static class defining DICOM code sequences used in key object selections.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public static class KeyObjectSelectionCodeSequences
	{
		/// <summary>
		/// Gets the code for DCM 113011 Document Title Modifier.
		/// </summary>
		public static readonly Code DocumentTitleModifier = new Code(113011, "Document Title Modifier");

		/// <summary>
		/// Gets the code for DCM 113012 Key Object Description.
		/// </summary>
		public static readonly Code KeyObjectDescription = new Code(113012, "Key Object Description");

		/// <summary>
		/// A DICOM code sequence used in key object selections.
		/// </summary>
		public sealed class Code : ContextGroupBase<Code>.ContextGroupItemBase
		{
			internal Code(int codeValue, string codeMeaning) : base("DCM", codeValue.ToString(), codeMeaning) {}
		}
	}
}