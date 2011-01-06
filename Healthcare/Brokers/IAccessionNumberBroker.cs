#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Custom broker that provides Accession numbers.
    /// </summary>
    public interface IAccessionNumberBroker : IPersistenceBroker
    {
		/// <summary>
		/// Peeks at the next accession number in the sequence, but does not advance the sequence.
		/// </summary>
		/// <returns></returns>
    	string PeekNextAccessionNumber();

		/// <summary>
		/// Gets the next accession number in the sequence, advancing the sequence by 1.
		/// </summary>
		/// <returns></returns>
		string GetNextAccessionNumber();
    }
}
