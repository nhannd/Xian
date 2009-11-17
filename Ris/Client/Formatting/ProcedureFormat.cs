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
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class ProcedureFormat
	{
		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static string Format(ProcedureSummary p)
		{
			return Format(p.Type.Name, p.Portable, p.Laterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static string Format(ProcedureDetail p)
		{
			return Format(p.Type.Name, p.Portable, p.Laterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static string Format(OrderListItem item)
		{
			return Format(item.ProcedureType.Name, item.ProcedurePortable, item.ProcedureLaterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static string Format(WorklistItemSummaryBase item)
		{
			return Format(item.ProcedureName, item.ProcedurePortable, item.ProcedureLaterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static string Format(PriorProcedureSummary item)
		{
			return Format(item.ProcedureType.Name, item.ProcedurePortable, item.ProcedureLaterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure portable and laterality similar to "Portable/Laterality".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %P - portable
		///     %L - laterality
		/// </remarks>
		/// <param name="portable"></param>
		/// <param name="laterality"></param>
		/// <returns></returns>
		public static string FormatModifier(bool portable, EnumValueInfo laterality)
		{
			return FormatModifier(portable, laterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
		}

		/// <summary>
		/// Formats the procedure name, portable and laterality similar to "Name (Portable/Laterality)".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %P - portable
		///     %L - laterality
		/// </remarks>
		/// <param name="typeName"></param>
		/// <param name="portable"></param>
		/// <param name="laterality"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(string typeName, bool portable, EnumValueInfo laterality, string format)
        {
			var modifier = FormatModifier(portable, laterality, format);

			return string.IsNullOrEmpty(modifier) 
				? typeName 
				: string.Format("{0} ({1})", typeName, modifier);
        }

		/// <summary>
		/// Formats the procedure portable and laterality similar to "Portable/Laterality".  
		/// Name is formatted according to the default person name format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %P - portable
		///     %L - laterality
		/// </remarks>
		/// <param name="portable"></param>
		/// <param name="laterality"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string FormatModifier(bool portable, EnumValueInfo laterality, string format)
		{
			string result = format;
			result = result.Replace("%P", portable == false ? "" : "Portable");
			result = result.Replace("%L", laterality == null || laterality.Code == "N" ? "" : laterality.Value);

			string nullResult = format;
			nullResult = nullResult.Replace("%P", "");
			nullResult = nullResult.Replace("%L", "");

			if (string.Compare(result, nullResult) == 0)
				return null;

			if (portable == false || laterality == null || laterality.Code == "N")
				result = result.Replace(nullResult, "");

			return result.Trim();
		}
	}
}
