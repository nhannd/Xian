#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml.Serialization;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Represents the result of the comparison when two sets of attributes are compared using <see cref="DicomAttributeCollection.Equals()"/>.
    /// </summary>
    public class DicomAttributeComparisonResult
    {
        #region Public Overrides
		public override string  ToString()
		{
			return Details;
		}
    	#endregion

        #region Public Properties

    	/// <summary>
    	/// Type of differences.
    	/// </summary>
    	[XmlAttribute]
    	public ComparisonResultType ResultType { get; set; }

    	/// <summary>
    	/// The name of the offending tag. This can be null if the difference is not tag specific.
    	/// </summary>
    	[XmlAttribute]
    	public String TagName { get; set; }

    	/// <summary>
    	/// Detailed text describing the problem.
    	/// </summary>
    	public string Details { get; set; }

    	#endregion

    }
}