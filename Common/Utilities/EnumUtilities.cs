using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public static class EnumUtilities
	{
		/// <summary>
		/// Converts a string to an enum of the corresponding type of the ref parameter (enumOut).
		/// The type of the enumOut parameter must be an enum, or an exception will be thrown.
		/// </summary>
		/// <typeparam name="T">any enum type</typeparam>
		/// <param name="enumString">the input string</param>
		/// <param name="enumOut">the result of the string converted to an enum</param>
		/// <returns>false if the string does not match any members of the enum</returns>
		public static bool StringToEnum<T>(string enumString, ref T enumOut)
		{
			Type enumType = enumOut.GetType();

			string[] enumNames = Enum.GetNames(enumType);

			for (int index = 0; index < enumNames.Length; ++index)
			{
				if (String.Compare(enumNames[index], enumString, true) == 0)
				{
					enumOut = (T)(Enum.GetValues(enumType).GetValue(index));
					return true;
				}
			}

			return false;
		}
	}
}
