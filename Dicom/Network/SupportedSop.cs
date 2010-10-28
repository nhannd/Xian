#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Structure used to represent the supported SOP Classes for Scu/Scp operations.
    /// </summary>
    public struct SupportedSop
    {
        private SopClass _sopClass;
        private IList<TransferSyntax> _syntaxList;

        /// <summary>
        /// The <see cref="ClearCanvas.Dicom.SopClass"/> instance supported.
        /// </summary>
        public SopClass SopClass
        {
            get { return _sopClass; }
            set { _sopClass = value; }
        }

        /// <summary>
        /// A list of transfer syntaxes supported by the <see cref="SopClass"/>.
        /// </summary>
        public IList<TransferSyntax> SyntaxList
        {
            get {
                if (_syntaxList == null)
                    _syntaxList = new List<TransferSyntax>();
                return _syntaxList; 
            }
        }

        /// <summary>
        /// Used to add a supported transfer syntax.
        /// </summary>
        /// <param name="syntax">The transfer syntax supproted by the SOP Class.</param>
        public void AddSyntax(TransferSyntax syntax)
        {
            SyntaxList.Add(syntax);
        }
    }
}