using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class StaffNameAndRoleFormat
	{
		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)".  Name is formatted according to the default person name format as 
		/// specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public static string Format(StaffSummary staff)
		{
			return Format(staff, FormatSettings.Default.PersonNameDefaultFormat);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)".  Name is formatted according to the default person name format as 
		/// specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public static string Format(StaffDetail staff)
		{
			return Format(staff, FormatSettings.Default.PersonNameDefaultFormat);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)" with the name formatted according to the specified format string.
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %F - full family name
		///     %f - family name initial
		///     %G - full given name
		///     %g - given name initial
		///     %M - full middle name
		///     %m - middle initial
		/// </remarks>
		/// <param name="staff"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(StaffSummary staff, string format)
		{
			return string.Format("{0} ({1})", PersonNameFormat.Format(staff.Name, format), staff.StaffType.Value);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)" with the name formatted according to the specified format string.
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %F - full family name
		///     %f - family name initial
		///     %G - full given name
		///     %g - given name initial
		///     %M - full middle name
		///     %m - middle initial
		/// </remarks>
		/// <param name="staff"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(StaffDetail staff, string format)
		{
			return string.Format("{0} ({1})", PersonNameFormat.Format(staff.Name, format), staff.StaffType.Value);
		}
	}
}
