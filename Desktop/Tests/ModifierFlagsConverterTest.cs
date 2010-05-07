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
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class ModifierFlagsConverterTest
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
			ModifierFlagsConverter converter = new ModifierFlagsConverter(_dummyCulture);
			IDictionary<ModifierFlags, string> relocalizedNames = new Dictionary<ModifierFlags, string>();
			foreach (KeyValuePair<ModifierFlags, string> pair in converter.LocalizedNames)
				relocalizedNames.Add(pair.Key, Enum.GetName(typeof (ModifierFlags), pair.Key));
			relocalizedNames[ModifierFlags.Shift] = ModifierFlagsConverter.ModifierSeparator.ToString();
			converter.LocalizedNames = relocalizedNames;

			_converter = converter;
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() {}

		[Test]
		public void TestModifierFlagsValueUniqueness()
		{
			// this isn't necessarily required, but does make for a better behaved enumeration
			Dictionary<int, ModifierFlags> uniques = new Dictionary<int, ModifierFlags>();
			foreach (ModifierFlags value in Enum.GetValues(typeof (ModifierFlags)))
			{
				Assert.IsFalse(uniques.ContainsKey((int) value), "There should really only be one enumeration value for each modifier");
				uniques.Add((int) value, value);
			}
		}

		[Test]
		public void TestTypeConverterAttribute()
		{
			// tests that the ModifierFlagsConverter is properly defined on the ModifierFlags type
			Assert.IsAssignableFrom(typeof (ModifierFlagsConverter), TypeDescriptor.GetConverter(typeof (ModifierFlags)),
			                        "ModifierFlags should be marked with a TypeConverterAttribute that specifies ModifierFlagsConverter");
		}

		[Test]
		public void TestConversionTableFallback()
		{
			// tests that a single converter instance can correctly perform the conversions
			// in either its cached culture, the invariant culture, or a third culture altogether
			AssertEquivalency("Control", (ModifierFlags.Control), _dummyCulture, "Conversion using cached culture");
			AssertEquivalency("Ctrl", (ModifierFlags.Control), CultureInfo.InvariantCulture, "Conversion using invariant culture fallback");
			AssertEquivalency("Ctrl", (ModifierFlags.Control), _dummyCulture2, "Conversion using different culture fallback");
		}

		[Test]
		public void TestEmptyStringParse()
		{
			// empty strings should parse to ModifierFlags.None
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString(string.Empty);
			Assert.AreEqual(ModifierFlags.None, modifierCode, "empty strings should parse to ModifierFlags.None");
		}

		[Test]
		public void TestStringParseOrderIndifference()
		{
			// ordering of individual modifiers in the string should not matter
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+Shift+Alt");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Order of modifier string elements should not matter");

			modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+Alt+Shift");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Order of modifier string elements should not matter");

			modifierCode = (ModifierFlags) _converter.ConvertFromString("Shift+Ctrl+Alt");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Order of modifier string elements should not matter");
		}

		[Test]
		public void TestStringParseWhitespaceIndifference()
		{
			// whitespace between individual modifiers in the string should not matter
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl\t +Shift \t+Alt");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Whitespace between modifier string elements should not matter");

			modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+ \tAlt+ Shift");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Whitespace between modifier string elements should not matter");

			modifierCode = (ModifierFlags) _converter.ConvertFromString("Shift + \t Ctrl\t+  Alt");
			Assert.AreEqual(ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, modifierCode, "Whitespace between modifier string elements should not matter");

			modifierCode = (ModifierFlags) _converter.ConvertFromString("\t  Shift  ");
			Assert.AreEqual(ModifierFlags.Shift, modifierCode, "Whitespace between modifier string elements should not matter");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("\n");
			Assert.Fail("Expected an exception because the parsed string has an invalid character");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("NonExistentModifier");
			Assert.Fail("Expected an exception because the parsed string has an invalid modifier name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierWithNonModifierStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+NonExistentModifier");
			Assert.Fail("Expected an exception because the parsed string has an invalid modifier name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidNonModifierWithModifierStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("NonExistentModifier+Alt");
			Assert.Fail("Expected an exception because the parsed string has an invalid modifier name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierWithModifiersStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+NonExistentModifier+Alt");
			Assert.Fail("Expected an exception because the parsed string has an invalid modifier name");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierTrailingSeparatorStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("Ctrl+");
			Assert.Fail("Expected an exception because of a trailing separator in the parse string");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void TestInvalidModifierLeadingSeparatorStringParse()
		{
			// on the other hand, invalid modifier strings should throw exceptions during conversion
			ModifierFlags modifierCode = (ModifierFlags) _converter.ConvertFromString("+Alt");
			Assert.Fail("Expected an exception because of a leading separator in the parse string");
		}

		[Test]
		public void TestInvariantLocalizationMapping()
		{
			// our invariant fallback should be well behaved (1-to-1 mapping to unique non-null values)
			// localizations of the mapping may do as they will - they will encounter funny parse results, but that's THEIR PROBLEM
			Dictionary<string, ModifierFlags> uniques = new Dictionary<string, ModifierFlags>();
			foreach (KeyValuePair<ModifierFlags, string> pair in ModifierFlagsConverter.InvariantNames)
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
		public void TestSpecialCase()
		{
			// test special case where the modifier separator is a modifier name on its own
			const string message = "Special Case";
			const ModifierFlags modifier = ModifierFlags.Shift;
			CultureInfo culture = _dummyCulture;
			string actualModifierName = ModifierFlagsConverter.ModifierSeparator.ToString();
			AssertEquivalency(string.Format("{0}", actualModifierName), (ModifierFlags) modifier, culture, message);
			AssertEquivalency(string.Format("Control+{0}", actualModifierName), ModifierFlags.Control | (ModifierFlags) modifier, culture, message);
			AssertEquivalency(string.Format("Alt+{0}", actualModifierName), ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);
			AssertEquivalency(string.Format("Control+Alt+{0}", actualModifierName), ModifierFlags.Control | ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);

			AssertStringParse(string.Format("{0}   ", actualModifierName), (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("   {0}", actualModifierName), (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("   {0}   ", actualModifierName), (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Control    +   {0}", actualModifierName), ModifierFlags.Control | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Control+     {0}", actualModifierName), ModifierFlags.Control | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("{0}+Control", actualModifierName), ModifierFlags.Control | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("{0}    +Control", actualModifierName), ModifierFlags.Control | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Alt+{0}+Control", actualModifierName), ModifierFlags.Control | ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Alt+{0}   +Control", actualModifierName), ModifierFlags.Control | ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Alt+    {0}+Control", actualModifierName), ModifierFlags.Control | ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);
			AssertStringParse(string.Format("Alt+    {0}   +Control", actualModifierName), ModifierFlags.Control | ModifierFlags.Alt | (ModifierFlags) modifier, culture, message);
		}

		[Test]
		public void TestLocalizedModifierCombos()
		{
			// test combining modifiers with modifiers in the localized case
			const string message = "Localized Combinations";
			CultureInfo culture = _dummyCulture;

			AssertEquivalency(string.Format("Control"), ModifierFlags.Control, culture, message);
			AssertEquivalency(string.Format("Alt"), ModifierFlags.Alt, culture, message);
			AssertEquivalency(string.Format("+"), ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Control+Alt"), ModifierFlags.Control | ModifierFlags.Alt, culture, message);
			AssertEquivalency(string.Format("Alt++"), ModifierFlags.Alt | ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Control++"), ModifierFlags.Control | ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Control+Alt++"), ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, culture, message);
		}

		[Test]
		public void TestInvariantModifierCombos()
		{
			// test combining modifiers with modifiers in the invariant case
			const string message = "Invariant Combinations";
			CultureInfo culture = CultureInfo.InvariantCulture;

			AssertEquivalency(string.Format("Ctrl"), ModifierFlags.Control, culture, message);
			AssertEquivalency(string.Format("Alt"), ModifierFlags.Alt, culture, message);
			AssertEquivalency(string.Format("Shift"), ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt"), ModifierFlags.Control | ModifierFlags.Alt, culture, message);
			AssertEquivalency(string.Format("Alt+Shift"), ModifierFlags.Alt | ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Ctrl+Shift"), ModifierFlags.Control | ModifierFlags.Shift, culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt+Shift"), ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift, culture, message);
		}

		private void AssertEquivalency(string sModifiers, ModifierFlags eModifiers, CultureInfo culture, string message)
		{
			AssertStringFormat(sModifiers, eModifiers, culture, message);
			AssertStringParse(sModifiers, eModifiers, culture, message);
		}

		private void AssertStringFormat(string sModifiers, ModifierFlags eModifiers, CultureInfo culture, string message)
		{
			string actualString = _converter.ConvertToString(null, culture, eModifiers);
			//System.Diagnostics.Trace.WriteLine(actualString);
			Assert.AreEqual(sModifiers, actualString, message + ": converting " + (int) eModifiers + " which is " + eModifiers.ToString());
		}

		private void AssertStringParse(string sModifiers, ModifierFlags eModifiers, CultureInfo culture, string message)
		{
			ModifierFlags actualEnum = (ModifierFlags) _converter.ConvertFromString(null, culture, sModifiers);
			//System.Diagnostics.Trace.WriteLine(actualEnum);
			Assert.AreEqual((int) eModifiers, (int) actualEnum, message + ": converting " + sModifiers + " which is " + actualEnum.ToString());
		}
	}
}

#endif