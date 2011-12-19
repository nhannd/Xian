#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Defines the base interface to an HP "contributor".  A contributor is an object that contributes
	/// to a hanging protocol.
	/// </summary>
	public interface IHpContributor : IHpSerializableElement
	{
		/// <summary>
		/// Gets a GUID identifying this class of contributor (must return a constant value) for serialization purposes.
		/// </summary>
		Guid ContributorId { get; }

        //TODO (CR June 2011): Not sure about this, but I guess it doesn't do any harm.

        /// <summary>
        /// Gets a friendly name for the contributor that could be shown to the user, if needed.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a friendly description for the contributor that could be shown to the user, if needed.
        /// </summary>
        string Description { get; }

		/// <summary>
		/// Called by the user-interface to obtain the set of properties that can be edited by the user.
		/// </summary>
		/// <returns></returns>
		IHpProperty[] GetProperties();

		/// <summary>
		/// Gets a value indicating whether this contributor requires the patient history (prior studies)
		/// to be loaded.
		/// </summary>
		bool RequiresPatientHistory { get; }
	}
}
