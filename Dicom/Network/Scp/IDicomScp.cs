#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Network.Scp
{
    /// <summary>
    /// Simplified interface for DICOM SCPs.
    /// </summary>
    public interface IDicomScp<TContext>
    {
        /// <summary>
        /// Method called by the handler during association verification.
        /// </summary>
        /// <param name="association">Parameters for the association</param>
        /// <param name="pcid">The presentation context being verified</param>
        /// <returns></returns>
        DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid);

        /// <summary>
        /// Method called when a request message is being processed.
        /// </summary>
        /// <param name="server">The <see cref="DicomServer"/> instance for the association.</param>
        /// <param name="association">Parameters for the association.</param>
        /// <param name="presentationID">The presentation context for the association.</param>
        /// <param name="message">The message to process.</param>
        /// <returns>true on success, false on failure.</returns>
        bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);

        /// <summary>
        /// Return a list of the DICOM services and transfer syntaxes supported by the interface.
        /// </summary>
        /// <returns></returns>
        IList<SupportedSop> GetSupportedSopClasses();

        /// <summary>
        /// Used to set user specific parameters to be passed to the interface instance.
        /// </summary>
        /// <param name="context">A user specific context for the <see cref="DicomScp{TContext}"/> instance.</param>
        void SetContext(TContext context);

        /// <summary>
        /// Called when an association is closed/aborted/released.  Note that the routine will be called once on the extension.
        /// </summary>
        void Cleanup();
    }
}
