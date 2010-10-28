#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
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
		/// <param name="p"></param>
		/// <returns></returns>
		public static string Format(ProcedureRequisition p)
		{
			return Format(p.ProcedureType.Name, p.PortableModality, p.Laterality, FormatSettings.Default.ProcedurePortableLateralityDefaultFormat);
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
			var result = format;
			result = result.Replace("%P", portable == false ? "" : "Portable");
			result = result.Replace("%L", laterality == null || laterality.Code == "N" ? "" : laterality.Value);

			var nullResult = format;
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
