using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Common.Utilities;

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
			string result = format;
            result = result.Replace("%P", portable == false ? "" : "Portable");
			result = result.Replace("%L", laterality == null || laterality.Code == "N" ? "" : laterality.Value);

			string nullResult = format;
            nullResult = nullResult.Replace("%P", "");
			nullResult = nullResult.Replace("%L", "");

			if (string.Compare(result, nullResult) == 0)
				return typeName;

			if (portable == false || laterality == null || laterality.Code == "N")
				result = result.Replace(nullResult, "");

			return string.Format("{0} ({1})", typeName, result.Trim());
        }
	}
}
