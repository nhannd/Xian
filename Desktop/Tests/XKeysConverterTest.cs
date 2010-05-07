#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class XKeysConverterTest
	{
		// these should be any two unique, non-invariant cultures
		private readonly CultureInfo _dummyCulture = CultureInfo.GetCultureInfo("en-us");
		private readonly CultureInfo _dummyCulture2 = CultureInfo.GetCultureInfo("en-ca");
		private TypeConverter _converter;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			if (_dummyCulture == null)
				throw new Exception("Error setting up test - dummyCulture should not be NULL");
			if (CultureInfo.InvariantCulture.Equals(_dummyCulture))
				throw new Exception("Error setting up test - dummyCulture should not be invariant");
			if (_dummyCulture2 == null)
				throw new Exception("Error setting up test - dummyCulture2 should not be NULL");
			if (CultureInfo.InvariantCulture.Equals(_dummyCulture2))
				throw new Exception("Error setting up test - dummyCulture2 should not be invariant");
			if (_dummyCulture2.Equals(_dummyCulture))
				throw new Exception("Error setting up test - dummyCulture2 should not be the same as dummyCulture");

			// for testing purposes, set up the converter for a specific culture to have the Enum.ToString() mapping
			// normally, you would use TypeDescriptor.GetConverter, but we want to keep the test appdomain clean of these testing mods
			XKeysConverter converter = new XKeysConverter(_dummyCulture);
			IDictionary<XKeys, string> relocalizedKeyNames = new Dictionary<XKeys, string>();
			foreach (KeyValuePair<XKeys, string> pair in converter.LocalizedKeyNames)
				relocalizedKeyNames.Add(pair.Key, Enum.GetName(typeof (XKeys), pair.Key));
			relocalizedKeyNames[XKeys.Control] = "Control"; // Enum.ToString() treats this as Control+None
			relocalizedKeyNames[XKeys.Shift] = "Shift"; // Enum.ToString() treats this as Shift+None
			relocalizedKeyNames[XKeys.Alt] = "Alt"; // Enum.ToString() treats this as Alt+None
			relocalizedKeyNames[XKeys.OemPlus] = XKeysConverter.KeySeparator.ToString(); // for special case test
			converter.LocalizedKeyNames = relocalizedKeyNames;

			_converter = converter;
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() {}

		[Test]
		public void TestXKeysValueUniqueness()
		{
			// this isn't necessarily required, but does make for a better behaved enumeration
			Dictionary<int, XKeys> uniques = new Dictionary<int, XKeys>();
			foreach (XKeys value in Enum.GetValues(typeof (XKeys)))
			{
				Assert.IsFalse(uniques.ContainsKey((int) value), "There should really only be one enumeration value for each key/modifier");
				uniques.Add((int) value, value);
			}
		}

		[Test]
		public void TestTypeConverterAttribute()
		{
			// tests that the XKeysConverter is properly defined on the XKeys type
			Assert.IsAssignableFrom(typeof (XKeysConverter), TypeDescriptor.GetConverter(typeof (XKeys)),
			                        "XKeys should be marked with a TypeConverterAttribute that specifies XKeysConverter");
		}

		[Test]
		public void TestConversionTableFallback()
		{
			// tests that a single converter instance can correctly perform the conversions
			// in either its cached culture, the invariant culture, or a third culture altogether
			AssertEquivalency("Control", (XKeys.Control), _dummyCulture, "Conversion using cached culture");
			AssertEquivalency("Ctrl", (XKeys.Control), CultureInfo.InvariantCulture, "Conversion using invariant culture fallback");
			AssertEquivalency("Ctrl", (XKeys.Control), _dummyCulture2, "Conversion using different culture fallback");
		}

		[Test]
		public void TestUnknownKeyCodeToString()
		{
			// unknown key codes should be accepted (i.e. no exceptions) but ignored for conversion to string
			// this is because newer generation keyboards can send stuff we don't know about yet

			CultureInfo invCulture = CultureInfo.InvariantCulture;
			string keyString = _converter.ConvertToString(null, invCulture, XKeys.KeyCode - 1);
			Assert.AreEqual(string.Empty, keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Control | (XKeys.KeyCode - 1));
			Assert.AreEqual("Ctrl", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Alt | (XKeys.KeyCode - 1));
			Assert.AreEqual("Alt", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Shift | (XKeys.KeyCode - 1));
			Assert.AreEqual("Shift", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Control | XKeys.Alt | (XKeys.KeyCode - 1));
			Assert.AreEqual("Ctrl+Alt", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Alt | XKeys.Shift | (XKeys.KeyCode - 1));
			Assert.AreEqual("Alt+Shift", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Control | XKeys.Shift | (XKeys.KeyCode - 1));
			Assert.AreEqual("Ctrl+Shift", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Control | XKeys.Alt | (XKeys.KeyCode - 1));
			Assert.AreEqual("Ctrl+Alt", keyString, "Unknown key codes should be accepted but ignored");

			keyString = _converter.ConvertToString(null, invCulture, XKeys.Control | XKeys.Alt | XKeys.Shift | (XKeys.KeyCode - 1));
			Assert.AreEqual("Ctrl+Alt+Shift", keyString, "Unknown key codes should be accepted but ignored");
		}

		[Test]
		public void TestEmptyStringParse()
		{
			// empty strings should parse to XKeys.None
			XKeys keyCode = (XKeys) _converter.ConvertFromString(string.Empty);
			Assert.AreEqual(XKeys.None, keyCode, "empty strings should parse to XKeys.None");
		}

		[Test]
		public void TestStringParseOrderIndifference()
		{
			// ordering of individual keys in the string should not matter
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Ctrl+Attn+Alt");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Order of key string elements should not matter");

			keyCode = (XKeys) _converter.ConvertFromString("Ctrl+Alt+Attn");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Order of key string elements should not matter");

			keyCode = (XKeys) _converter.ConvertFromString("Attn+Ctrl+Alt");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Order of key string elements should not matter");
		}

		[Test]
		public void TestStringParseWhitespaceIndifference()
		{
			// whitespace between individual keys in the string should not matter
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Ctrl\t +Attn \t+Alt");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Whitespace between key string elements should not matter");

			keyCode = (XKeys) _converter.ConvertFromString("Ctrl+ \tAlt+ Attn");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Whitespace between key string elements should not matter");

			keyCode = (XKeys) _converter.ConvertFromString("Attn + \t Ctrl\t+  Alt");
			Assert.AreEqual(XKeys.Control | XKeys.Alt | XKeys.Attn, keyCode, "Whitespace between key string elements should not matter");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("\n");
			Assert.Fail("Expected an exception because the parsed string has an invalid character");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidKeyStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("NonExistentKey");
			Assert.Fail("Expected an exception because the parsed string has an invalid key name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidKeyWithModifierStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Ctrl+NonExistentKey");
			Assert.Fail("Expected an exception because the parsed string has an invalid key name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierWithKeyStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("NonExistentKey+A");
			Assert.Fail("Expected an exception because the parsed string has an invalid key name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidKeyWithModifiersStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Ctrl+NonExistentKey+Alt");
			Assert.Fail("Expected an exception because the parsed string has an invalid key name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidKeyTrailingSeparatorStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Ctrl+");
			Assert.Fail("Expected an exception because of a trailing separator in the parse string");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidKeyLeadingSeparatorStringParse()
		{
			// on the other hand, invalid key strings should throw exceptions during conversion
			XKeys keyCode = (XKeys) _converter.ConvertFromString("+Alt");
			Assert.Fail("Expected an exception because of a leading separator in the parse string");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidMultipleKeyStringParse()
		{
			// multiple non-modifier keys are also not allowed as you can't represent that using the flags enumeration
			XKeys keyCode = (XKeys) _converter.ConvertFromString("A+Enter");
			Assert.Fail("Expected an exception because you can't have multiple non-modifier keys");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidMultipleKeyWithModifierStringParse()
		{
			// multiple non-modifier keys are also not allowed as you can't represent that using the flags enumeration
			XKeys keyCode = (XKeys) _converter.ConvertFromString("Alt+A+Enter");
			Assert.Fail("Expected an exception because you can't have multiple non-modifier keys");
		}

		[Test]
		public void TestInvariantLocalizationMapping()
		{
			// our invariant fallback should be well behaved (1-to-1 mapping to unique non-null values)
			// localizations of the mapping may do as they will - they will encounter funny parse results, but that's THEIR PROBLEM
			Dictionary<string, XKeys> uniques = new Dictionary<string, XKeys>();
			foreach (KeyValuePair<XKeys, string> pair in XKeysConverter.InvariantKeyNames)
			{
				if (string.IsNullOrEmpty(pair.Value))
				{
					Assert.Fail("Invariant mapping for {0} should not be NULL", pair.Key);
					break;
				}

				if (uniques.ContainsKey(pair.Value))
				{
					Assert.Fail("Invariant mapping for {0} should be unique (conflicts with existing mapping for {1})", pair.Key, uniques[pair.Value]);
					break;
				}

				uniques.Add(pair.Value, pair.Key);
			}
		}

		[Test]
		public void TestSingleKeys()
		{
			// test individual key conversions
			const string message = "Single Keys";
			CultureInfo culture = CultureInfo.InvariantCulture;
			for (int n = 65; n < 65 + 26; n++)
			{
				AssertEquivalency(((char) n).ToString(), (XKeys) n, culture, message);
			}
			AssertEquivalency(string.Empty, (XKeys.None), culture, message);
			AssertEquivalency("Ctrl", (XKeys.Control), culture, message);
			AssertEquivalency("Shift", (XKeys.Shift), culture, message);
			AssertEquivalency("Alt", (XKeys.Alt), culture, message);
		}

		[Test]
		public void TestSpecialCase()
		{
			// test special case where the key separator is a key name on its own
			const string message = "Special Case";
			const XKeys key = XKeys.OemPlus;
			CultureInfo culture = _dummyCulture;
			string actualKeyName = XKeysConverter.KeySeparator.ToString();
			AssertEquivalency(string.Format("{0}", actualKeyName), (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Control+{0}", actualKeyName), XKeys.Control | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Alt+{0}", actualKeyName), XKeys.Alt | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Shift+{0}", actualKeyName), XKeys.Shift | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Control+Alt+{0}", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Alt+Shift+{0}", actualKeyName), XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Control+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Shift | (XKeys) key, culture, message);
			AssertEquivalency(string.Format("Control+Alt+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);

			AssertStringParse(string.Format("{0}   ", actualKeyName), (XKeys)key, culture, message);
			AssertStringParse(string.Format("   {0}", actualKeyName), (XKeys)key, culture, message);
			AssertStringParse(string.Format("   {0}   ", actualKeyName), (XKeys)key, culture, message);
			AssertStringParse(string.Format("Control    +   {0}", actualKeyName), XKeys.Control | (XKeys)key, culture, message);
			AssertStringParse(string.Format("Control+     {0}", actualKeyName), XKeys.Control | (XKeys)key, culture, message);
			AssertStringParse(string.Format("{0}+Control", actualKeyName), XKeys.Control | (XKeys)key, culture, message);
			AssertStringParse(string.Format("{0}    +Control", actualKeyName), XKeys.Control | (XKeys)key, culture, message);
			AssertStringParse(string.Format("Alt+{0}+Control", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys)key, culture, message);
			AssertStringParse(string.Format("Alt+{0}   +Control", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys)key, culture, message);
			AssertStringParse(string.Format("Alt+    {0}+Control", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys)key, culture, message);
			AssertStringParse(string.Format("Alt+    {0}   +Control", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys)key, culture, message);
		}

		[Test]
		public void TestLocalizedKeyCombos()
		{
			// test combining keys with modifiers in the localized case
			const string message = "Localized Combinations";
			CultureInfo culture = _dummyCulture;

			foreach (string keyName in Enum.GetNames(typeof (XKeys)))
			{
				switch (keyName)
				{
					case "Modifiers":
					case "KeyCode":
					case "None":
					case "Control":
					case "Shift":
					case "Alt":
						// these aren't real keys anyway
						continue;
				}

				XKeys key = (XKeys) Enum.Parse(typeof (XKeys), keyName);
				string actualKeyName = _converter.ConvertToString(null, culture, key);
				AssertEquivalency(string.Format("Control+{0}", actualKeyName), XKeys.Control | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Alt+{0}", actualKeyName), XKeys.Alt | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Shift+{0}", actualKeyName), XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Control+Alt+{0}", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Alt+Shift+{0}", actualKeyName), XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Control+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Control+Alt+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);
			}
		}

		[Test]
		public void TestInvariantKeyCombos()
		{
			// test combining keys with modifiers in the invariant case
			const string message = "Invariant Combinations";
			CultureInfo culture = CultureInfo.InvariantCulture;

			foreach (string keyName in Enum.GetNames(typeof (XKeys)))
			{
				switch (keyName)
				{
					case "Modifiers":
					case "KeyCode":
					case "None":
					case "Control":
					case "Shift":
					case "Alt":
						// these aren't real keys anyway
						continue;
				}

				XKeys key = (XKeys) Enum.Parse(typeof (XKeys), keyName);
				string actualKeyName = _converter.ConvertToString(null, culture, key);
				AssertEquivalency(string.Format("Ctrl+{0}", actualKeyName), XKeys.Control | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Alt+{0}", actualKeyName), XKeys.Alt | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Shift+{0}", actualKeyName), XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Ctrl+Alt+{0}", actualKeyName), XKeys.Control | XKeys.Alt | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Alt+Shift+{0}", actualKeyName), XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Ctrl+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Shift | (XKeys) key, culture, message);
				AssertEquivalency(string.Format("Ctrl+Alt+Shift+{0}", actualKeyName), XKeys.Control | XKeys.Alt | XKeys.Shift | (XKeys) key, culture, message);
			}
		}

		[Test]
		public void TestInvariantKeyCombosWithReferenceImplementation()
		{
			// test combining keys with modifiers in the invariant case
			const string message = "Invariant Combinations against the original ActiveToolbarButton implementation (reference implementation)";
			CultureInfo culture = CultureInfo.InvariantCulture;

			foreach (string keyName in Enum.GetNames(typeof (XKeys)))
			{
				switch (keyName)
				{
					case "Modifiers":
					case "KeyCode":
					case "None":
					case "Control":
					case "Shift":
					case "Alt":
						// these aren't real keys anyway
						continue;
				}

				XKeys key = (XKeys) Enum.Parse(typeof (XKeys), keyName);
				foreach (var modifiers in new[]
				                          	{
				                          		XKeys.None,
				                          		XKeys.Control,
				                          		XKeys.Alt,
				                          		XKeys.Shift,
				                          		XKeys.Control | XKeys.Alt,
				                          		XKeys.Alt | XKeys.Shift,
				                          		XKeys.Control | XKeys.Shift,
				                          		XKeys.Control | XKeys.Alt | XKeys.Shift
				                          	})
				{
					AssertEquivalency(ReferenceToStringImplementation(modifiers | key), modifiers | key, culture, message);
				}
			}
		}

		private void AssertEquivalency(string sKeys, XKeys eKeys, CultureInfo culture, string message)
		{
			AssertStringFormat(sKeys, eKeys, culture, message);
			AssertStringParse(sKeys, eKeys, culture, message);
		}

		private void AssertStringFormat(string sKeys, XKeys eKeys, CultureInfo culture, string message)
		{
			string actualString = _converter.ConvertToString(null, culture, eKeys);
			//System.Diagnostics.Trace.WriteLine(actualString);
			Assert.AreEqual(sKeys, actualString, message + ": converting " + (int) eKeys + " which is " + eKeys.ToString());
		}

		private void AssertStringParse(string sKeys, XKeys eKeys, CultureInfo culture, string message)
		{
			XKeys actualEnum = (XKeys) _converter.ConvertFromString(null, culture, sKeys);
			//System.Diagnostics.Trace.WriteLine(actualEnum);
			Assert.AreEqual((int) eKeys, (int) actualEnum, message + ": converting " + sKeys + " which is " + actualEnum.ToString());
		}

		/// <remarks>
		/// A reference implementation for ToString taken originally from GetTooltipTest(IClickAction) of
		/// ClearCanvas.Desktop.View.WinForms/ActiveToolbarButton.cs/r12907.
		/// This method now uses XKeysConverter, which is why we compare our results
		/// against this reference implementation.
		/// </remarks>
		private static string ReferenceToStringImplementation(XKeys keyStroke)
		{
			bool ctrl = (keyStroke & XKeys.Control) == XKeys.Control;
			bool alt = (keyStroke & XKeys.Alt) == XKeys.Alt;
			bool shift = (keyStroke & XKeys.Shift) == XKeys.Shift;
			XKeys keyCode = keyStroke & XKeys.KeyCode;

			StringBuilder builder = new StringBuilder();
			if (keyCode != XKeys.None)
			{
				if (ctrl)
					builder.Append("Ctrl");

				if (alt)
				{
					if (ctrl)
						builder.Append("+");

					builder.Append("Alt");
				}

				if (shift)
				{
					if (ctrl || alt)
						builder.Append("+");

					builder.Append("Shift");
				}

				if (ctrl || alt || shift)
					builder.Append("+");

				builder.Append(XKeysConverter.FormatInvariant(keyCode));
			}

			return builder.ToString();
		}
	}
}

#endif