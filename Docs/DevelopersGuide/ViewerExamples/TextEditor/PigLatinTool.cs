#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace MyPlugin.TextEditor
{
	[MenuAction("apply", "global-menus/Text Editor/Pig Latinize", "Apply")]
	[ExtensionOf(typeof (TextEditorToolExtensionPoint))]
	public class PigLatinTool : Tool<ITextEditorToolContext>
	{
		public void Apply()
		{
			this.Context.Text = PigLatinize(this.Context.Text);
		}

		private static string PigLatinize(string input)
		{
			#region Implementation Code
			Regex pattern = new Regex("[A-Za-z]+");
			return pattern.Replace(input,
			                       delegate(Match m)
			                       	{
			                       		string word = m.Groups[0].Value;
			                       		bool isCapitalized = char.IsUpper(word[0]);
			                       		word = word.ToLowerInvariant();

			                       		int charsAtBeginning = 1;
			                       		if (word.StartsWith("ci")
			                       		    || word.StartsWith("ck")
			                       		    || word.StartsWith("ng")
			                       		    || word.StartsWith("ph")
			                       		    || word.StartsWith("qu")
			                       		    || word.StartsWith("rh")
			                       		    || word.StartsWith("sc")
			                       		    || word.StartsWith("sh")
			                       		    || word.StartsWith("th")
			                       		    || word.StartsWith("wh")
			                       		    || word.StartsWith("wr"))
			                       		{
			                       			charsAtBeginning = 2;
			                       		}
			                       		else if (word.StartsWith("a")
			                       		         || word.StartsWith("e")
			                       		         || word.StartsWith("i")
			                       		         || word.StartsWith("o")
			                       		         || word.StartsWith("u")
			                       		         || word.StartsWith("ya")
			                       		         || word.StartsWith("ye")
			                       		         || word.StartsWith("yi")
			                       		         || word.StartsWith("yo")
			                       		         || word.StartsWith("yu"))
			                       		{
			                       			charsAtBeginning = 0;
			                       		}

			                       		word = string.Format("{0}{1}ay",
			                       		                     word.Substring(charsAtBeginning, word.Length - charsAtBeginning),
			                       		                     word.Substring(0, charsAtBeginning));

			                       		if (isCapitalized)
			                       			word = char.ToUpperInvariant(word[0]) + word.Substring(1, word.Length - 1);
			                       		return word;
			                       	});
			#endregion
		}
	}
}