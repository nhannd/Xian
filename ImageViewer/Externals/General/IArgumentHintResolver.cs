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
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public interface IArgumentHintResolver : IDisposable
	{
		string Resolve(string input);
		string Resolve(string input, bool resolveMultiValuedHints, string multiValueSeparator);
	}

	internal sealed class ArgumentHintResolver : IArgumentHintResolver
	{
		private static readonly Regex _pattern = new Regex(@"\$(\w*?)\$", RegexOptions.Compiled);
		private readonly Dictionary<string, ArgumentHintValue> _resolvedHints;
		private readonly IList<IArgumentHint> _hints;

		public ArgumentHintResolver() : this(null) {}

		public ArgumentHintResolver(IEnumerable<IArgumentHint> hints)
		{
			if (hints != null)
				this._hints = new List<IArgumentHint>(hints);
			else
				this._hints = new List<IArgumentHint>();

			this._resolvedHints = new Dictionary<string, ArgumentHintValue>();
			this._resolvedHints.Add("", new ArgumentHintValue("$"));
		}

		public void Dispose()
		{
			foreach (IArgumentHint hint in _hints)
				hint.Dispose();
			_hints.Clear();
		}

		public string Resolve(string input)
		{
			return Resolve(input, false, string.Empty);
		}

		public string Resolve(string input, bool resolveMultiValuedHints, string multiValueSeparator)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			int lastCopiedChar = -1;
			StringBuilder sb = new StringBuilder();

			foreach (Match m in _pattern.Matches(input))
			{
				string key = m.Groups[1].Value;
				if (!this._resolvedHints.ContainsKey(key))
				{
					// find a hint that provides this key
					foreach (IArgumentHint hint in this._hints)
					{
						ArgumentHintValue value = hint[key];
						if (value.IsNull)
							continue;
						this._resolvedHints.Add(key, value);
						break;
					}

					if (!this._resolvedHints.ContainsKey(key))
						return null; // unable to resolve arguments in this string
				}

				ArgumentHintValue hintValue = _resolvedHints[key];
				string resolvedHint;
				if (resolveMultiValuedHints)
					resolvedHint = hintValue.ToString(multiValueSeparator);
				else if (!hintValue.IsMultiValued)
					resolvedHint = hintValue.ToString();
				else
					return null; // unable to resolve argument with multivalue hints disabled

				++lastCopiedChar;
				sb.Append(input.Substring(lastCopiedChar, m.Index - lastCopiedChar));
				sb.Append(resolvedHint);
				lastCopiedChar = m.Index + m.Length - 1;
			}
			++lastCopiedChar;
			sb.Append(input.Substring(lastCopiedChar, input.Length - lastCopiedChar));

			return sb.ToString();
		}
	}
}