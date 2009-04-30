#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
