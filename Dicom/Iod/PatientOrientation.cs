#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod
{
    public class PatientDirection
    {
        public static readonly char? Empty = null;
        public const char Unspecified = 'X';
        public const char Left = 'L';
        public const char Right = 'R';
        public const char Posterior = 'P';
        public const char Anterior = 'A';
        public const char Head = 'H';
        public const char Foot = 'F';

        public enum Component
        {
            Primary = 0,
            Secondary = 1,
            Tertiary = 2
        }
    }

    /// <summary>
	/// Represents the orientation of the image in the patient using dicom enumerated values
	/// to indicate the direction of the first row and column in the image.
	/// </summary>
	public class PatientOrientation : IEquatable<PatientOrientation>
	{
        public static PatientOrientation Empty = new PatientOrientation(String.Empty, String.Empty);
        public static PatientOrientation AxialRight = new PatientOrientation(PatientDirection.Right, PatientDirection.Posterior);
        public static PatientOrientation AxialLeft = new PatientOrientation(PatientDirection.Left, PatientDirection.Posterior);
        public static PatientOrientation SaggittalPosterior = new PatientOrientation(PatientDirection.Posterior, PatientDirection.Foot);
        public static PatientOrientation SaggittalAnterior = new PatientOrientation(PatientDirection.Anterior, PatientDirection.Foot);
        public static PatientOrientation CoronalRight = new PatientOrientation(PatientDirection.Right, PatientDirection.Foot);
        public static PatientOrientation CoronalLeft = new PatientOrientation(PatientDirection.Left, PatientDirection.Foot);

		/// <summary>
		/// Constructor.
		/// </summary>
		public PatientOrientation(string row, string column)
		{
			Row = row ?? "";
			Column = column ?? "";
		}
        /// <summary>
        /// Constructor.
        /// </summary>
		public PatientOrientation(char row, char column)
            : this(row.ToString(), column.ToString())
		{
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public PatientOrientation(char? row, char? column)
            : this(row.HasValue ? row.Value.ToString() : null, column.HasValue ? column.Value.ToString() : null)
		{
		}

        #region Public Properties

		/// <summary>
		/// Gets whether or not this <see cref="PatientOrientation"/> object is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Row == String.Empty && Column == String.Empty; }	
		}

        public bool IsRowUnspecified { get { return IsUnspecified(Row); } }
        public bool IsColumnUnspecified { get { return IsUnspecified(Column); } }

        /// <summary>
		/// Gets the direction of the first row in the image.
		/// </summary>
		public virtual string Row { get; private set; }

		/// <summary>
		/// Gets the direction of the first column in the image.
		/// </summary>
		public virtual string Column { get; private set; }

		/// <summary>
		/// Gets the primary direction of the first row in the image.
		/// </summary>
		public char? PrimaryRow
		{
            get { return GetRowDirection(PatientDirection.Component.Primary); }
		}

		/// <summary>
		/// Gets the primary direction of the first column in the image.
		/// </summary>
        public char? PrimaryColumn
		{
            get { return GetColumnDirection(PatientDirection.Component.Primary); }
        }

		/// <summary>
		/// Gets the secondary direction of the first row in the image.
		/// </summary>
        public char? SecondaryRow
		{
            get { return GetRowDirection(PatientDirection.Component.Secondary); }
		}

		/// <summary>
		/// Gets the secondary direction of the first column in the image.
		/// </summary>
        public char? SecondaryColumn
		{
            get { return GetColumnDirection(PatientDirection.Component.Secondary); }
		}

        /// <summary>
        /// Gets the tertiary direction of the first row in the image.
        /// </summary>
        public char? TertiaryRow
        {
            get { return GetRowDirection(PatientDirection.Component.Tertiary); }
        }

        /// <summary>
        /// Gets the tertiary direction of the first column in the image.
        /// </summary>
        public char? TertiaryColumn
        {
            get { return GetColumnDirection(PatientDirection.Component.Tertiary); }
        }

	    #endregion

        #region Public Methods

        /// <summary>
		/// Gets a string suitable for direct insertion into a <see cref="DicomAttributeMultiValueText"/> attribute.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0}\{1}", Row, Column);
		}

		/// <summary>
		/// Creates a <see cref="PatientOrientation"/> object from a dicom multi-valued string.
		/// </summary>
		/// <returns>
		/// Null if there are not exactly 2 parsed values in the input string.
		/// </returns>
		public static PatientOrientation FromString(string multiValuedString)
		{
			string[] values = DicomStringHelper.GetStringArray(multiValuedString);
			if (values != null && values.Length == 2)
				return new PatientOrientation(values[0], values[1]);

			return null;
		}

		#region IEquatable<PatientOrientation> Members

		public bool Equals(PatientOrientation other)
		{
			if (other == null)
				return false;

			return other.Row == Row && other.Column == Column;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			return Equals(obj as PatientOrientation);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
        }

        #endregion

        #region Private Methods

        private char? GetRowDirection(PatientDirection.Component component)
        {
            return GetRowDirection((int)component);
        }

        private char? GetColumnDirection(PatientDirection.Component component)
        {
            return GetColumnDirection((int)component);
        }

        private char? GetRowDirection(int index)
        {
            return GetDirection(Row, index);
        }

        private char? GetColumnDirection(int index)
        {
            return GetDirection(Column, index);
        }

        private static char? GetDirection(string direction, int index)
        {
            if (!string.IsNullOrEmpty(direction) && direction.Length > index)
                return direction[index];
            return null;
        }

        private static bool IsUnspecified(string direction)
        {
            return direction.Length == 0 || direction[0] == PatientDirection.Unspecified;
        }

        #endregion
    }
}
