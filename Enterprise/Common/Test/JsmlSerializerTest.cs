#if UNIT_TESTS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using NUnit.Framework;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Test
{
	[TestFixture]
	public class JsmlSerializerTest
	{
		public enum TestEnum { Enum1, Enum2 }

		/// <summary>
		/// This data contract is for testing EntityRef within a data contract.  Also test when a data contract has all its properties as null.
		/// </summary>
		[DataContract]
		class TestContract1 : DataContractBase
		{
			[DataMember]
			public EntityRef EntityRef;

			public string Jsml
			{
				get
				{
					var builder = new StringBuilder();

					if (this.EntityRef != null)
					{
						builder.Append("<Tag hash=\"true\">");
						builder.Append("\r\n  ");
						builder.AppendFormat("<EntityRef>{0}</EntityRef>", this.EntityRef.Serialize());
						builder.Append("\r\n</Tag>");
					}
					else
					{
						builder.Append("<Tag hash=\"true\" />");
					}

					return builder.ToString();
				}
			}

			public override bool Equals(object obj)
			{
				var other = obj as TestContract1;
				return Equals(this.EntityRef, other.EntityRef);
			}
		}

		/// <summary>
		/// This class is for testing multiple data members.  Also test when a particular data member is filtered out by <see cref="JsmlSerializer.SerializeOptions"/>.
		/// </summary>
		[DataContract]
		class TestContract2 : DataContractBase
		{
			[DataMember]
			public Double Double;

			[DataMember]
			public bool Bool;

			[DataMember]
			public DateTime? NullableDateTime;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;

			public string Jsml
			{
				get { return GetJsml(false); }
			}

			public string GetJsml(bool filteredOutDouble)
			{
				var builder = new StringBuilder();
				builder.Append("<Tag hash=\"true\">");

				if (!filteredOutDouble)
				{
					builder.Append("\r\n  ");
					builder.AppendFormat("<Double>{0}</Double>", this.Double);
				}

				builder.Append("\r\n  ");
				builder.AppendFormat("<Bool>{0}</Bool>", this.Bool.ToString().ToLower());

				if (this.NullableDateTime != null)
				{
					builder.Append("\r\n  ");
					builder.AppendFormat("<NullableDateTime>{0}</NullableDateTime>", DateTimeUtils.FormatISO(this.NullableDateTime.Value));
				}

				if (this.ExtendedProperties != null)
				{
					builder.Append("\r\n  ");
					builder.Append("<ExtendedProperties hash=\"true\">");
					foreach (var kvp in this.ExtendedProperties)
					{
						builder.Append("\r\n    ");
						builder.AppendFormat("<{0}>{1}</{0}>", kvp.Key, kvp.Value);
					}
					builder.Append("\r\n  ");
					builder.Append("</ExtendedProperties>");
				}

				builder.Append("\r\n</Tag>");

				return builder.ToString();
			}

			public override bool Equals(object obj)
			{
				var other = obj as TestContract2;
				return Equals(this.Double, other.Double)
					&& Equals(this.Bool, other.Bool)
					&& Equals(this.NullableDateTime, other.NullableDateTime)
					&& (this.ExtendedProperties == null || other.ExtendedProperties == null
							? Equals(this.ExtendedProperties, other.ExtendedProperties)
							: CollectionUtils.Equal((ICollection)this.ExtendedProperties, other.ExtendedProperties, true));
			}
		}

		[Test]
		public void Test_Null()
		{
			SerializeHelper(null, null);
			DeserializeHelper<object>(null, null);
			DeserializeHelper<object>(null, "");
		}

		[Test]
		public void Test_String()
		{
			// Empty string
			SerializeHelper("", "<Tag />");
			DeserializeHelper("", "<Tag />");
			DeserializeHelper("", "<Tag></Tag>");

			// Alphabets
			SerializeHelper("abcdefghijklmnopqrstuvwxyz", "<Tag>abcdefghijklmnopqrstuvwxyz</Tag>");
			DeserializeHelper("abcdefghijklmnopqrstuvwxyz", "<Tag>abcdefghijklmnopqrstuvwxyz</Tag>");

			// Numbers
			SerializeHelper("000", "<Tag>000</Tag>");
			DeserializeHelper("000", "<Tag>000</Tag>");
			SerializeHelper("1234567890", "<Tag>1234567890</Tag>");
			DeserializeHelper("1234567890", "<Tag>1234567890</Tag>");

			// Unusual characters
			SerializeHelper(@"`~!@#$%^*()-_=+[{]}\|;:,./?", @"<Tag>`~!@#$%^*()-_=+[{]}\|;:,./?</Tag>");
			DeserializeHelper(@"`~!@#$%^*()-_=+[{]}\|;:,./?", @"<Tag>`~!@#$%^*()-_=+[{]}\|;:,./?</Tag>");

			// Single and double quotes
			SerializeHelper("''", @"<Tag>''</Tag>");
			DeserializeHelper("''", @"<Tag>''</Tag>");
			SerializeHelper(@"""", @"<Tag>""</Tag>");
			DeserializeHelper(@"""", @"<Tag>""</Tag>");

			// Escaped characters
			SerializeHelper(@"&<>", @"<Tag>&amp;&lt;&gt;</Tag>");
			DeserializeHelper(@"&<>", @"<Tag>&amp;&lt;&gt;</Tag>");
		}

		[Test]
		public void Test_CR_LF_Spaces()
		{
			// CR, LF, spaces between words
			SerializeHelper("ABC\r\nDEF", "<Tag>ABC\r\nDEF</Tag>");
			DeserializeHelper("ABC\r\nDEF", "<Tag>ABC\r\nDEF</Tag>");
			SerializeHelper("ABC\rDEF", "<Tag>ABC\rDEF</Tag>");
			DeserializeHelper("ABC\rDEF", "<Tag>ABC\rDEF</Tag>");
			SerializeHelper("ABC\nDEF", "<Tag>ABC\nDEF</Tag>");
			DeserializeHelper("ABC\nDEF", "<Tag>ABC\nDEF</Tag>");
			SerializeHelper(" ABC DEF ", "<Tag> ABC DEF </Tag>");
			DeserializeHelper(" ABC DEF ", "<Tag> ABC DEF </Tag>");

			// CR, LF, spaces before words
			SerializeHelper("\r\nDEF", "<Tag>\r\nDEF</Tag>");
			DeserializeHelper("\r\nDEF", "<Tag>\r\nDEF</Tag>");
			SerializeHelper("\rDEF", "<Tag>\rDEF</Tag>");
			DeserializeHelper("\rDEF", "<Tag>\rDEF</Tag>");
			SerializeHelper("\nDEF", "<Tag>\nDEF</Tag>");
			DeserializeHelper("\nDEF", "<Tag>\nDEF</Tag>");
			SerializeHelper("  DEF", "<Tag>  DEF</Tag>");
			DeserializeHelper("  DEF", "<Tag>  DEF</Tag>");

			// CR, LF, spaces after words
			SerializeHelper("ABC\r\n", "<Tag>ABC\r\n</Tag>");
			DeserializeHelper("ABC\r\n", "<Tag>ABC\r\n</Tag>");
			SerializeHelper("ABC\r", "<Tag>ABC\r</Tag>");
			DeserializeHelper("ABC\r", "<Tag>ABC\r</Tag>");
			SerializeHelper("ABC\n", "<Tag>ABC\n</Tag>");
			DeserializeHelper("ABC\n", "<Tag>ABC\n</Tag>");
			SerializeHelper("ABC  ", "<Tag>ABC  </Tag>");
			DeserializeHelper("ABC  ", "<Tag>ABC  </Tag>");

			// CR, LF, whitespace by themselves
			SerializeHelper("\r\n", "<Tag>\r\n</Tag>");
			DeserializeHelper("\r\n", "<Tag>\r\n</Tag>");
			SerializeHelper("\r\n\r\n", "<Tag>\r\n\r\n</Tag>");
			DeserializeHelper("\r\n\r\n", "<Tag>\r\n\r\n</Tag>");
			SerializeHelper(" ", "<Tag> </Tag>");
			DeserializeHelper(" ", "<Tag> </Tag>");
			SerializeHelper("  ", "<Tag>  </Tag>");
			DeserializeHelper("  ", "<Tag>  </Tag>");
			SerializeHelper(" \r\n", "<Tag> \r\n</Tag>");
			DeserializeHelper(" \r\n", "<Tag> \r\n</Tag>");
			SerializeHelper("  \r\n", "<Tag>  \r\n</Tag>");
			DeserializeHelper("  \r\n", "<Tag>  \r\n</Tag>");
		}

		[Test]
		public void Test_Int()
		{
			SerializeHelper(0, "<Tag>0</Tag>");
			DeserializeHelper(0, "<Tag>0</Tag>");

			SerializeHelper(00, "<Tag>0</Tag>");
			DeserializeHelper(00, "<Tag>0</Tag>");

			SerializeHelper(5, "<Tag>5</Tag>");
			DeserializeHelper(5, "<Tag>5</Tag>");

			SerializeHelper(99999, "<Tag>99999</Tag>");
			DeserializeHelper(99999, "<Tag>99999</Tag>");

			SerializeHelper(-1, "<Tag>-1</Tag>");
			DeserializeHelper(-1, "<Tag>-1</Tag>");

			SerializeHelper(-99999, "<Tag>-99999</Tag>");
			DeserializeHelper(-99999, "<Tag>-99999</Tag>");
		}

		[Test]
		public void Test_Double()
		{
			SerializeHelper(0.00, "<Tag>0</Tag>");
			DeserializeHelper(0.00, "<Tag>0</Tag>");

			SerializeHelper(5.1, "<Tag>5.1</Tag>");
			DeserializeHelper(5.1, "<Tag>5.1</Tag>");

			SerializeHelper(-5.1, "<Tag>-5.1</Tag>");
			DeserializeHelper(-5.1, "<Tag>-5.1</Tag>");

			SerializeHelper(0.99999999, "<Tag>0.99999999</Tag>");
			DeserializeHelper(0.99999999, "<Tag>0.99999999</Tag>");

			SerializeHelper(0.001, "<Tag>0.001</Tag>");
			DeserializeHelper(0.001, "<Tag>0.001</Tag>");

			// Scientific notations.  Note that the serialized is always capitalized and padded.  i.e. E-08 rather than E-8
			// Make sure the lower case and e-8 and e-08 can all be deserialized.
			SerializeHelper(0.000000015, "<Tag>1.5E-08</Tag>");
			SerializeHelper(1.5E-08, "<Tag>1.5E-08</Tag>");
			SerializeHelper(15E-09, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(0.000000015, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5e-8</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5e-08</Tag>");
		}

		[Test]
		public void Test_Bool()
		{
			SerializeHelper(true, "<Tag>true</Tag>");
			DeserializeHelper(true, "<Tag>true</Tag>");

			SerializeHelper(false, "<Tag>false</Tag>");
			DeserializeHelper(false, "<Tag>false</Tag>");
		}

		[Test]
		public void Test_Enum()
		{
			SerializeHelper(TestEnum.Enum1, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));
			DeserializeHelper(TestEnum.Enum1, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));

			SerializeHelper(TestEnum.Enum2, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
			DeserializeHelper(TestEnum.Enum2, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
		}

		[Test]
		public void Test_DateTime()
		{
			var now = DateTime.Now;

			// Serializer do not support milliseconds and below.
			var nowWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			SerializeHelper(nowWithoutMilliseconds, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nowWithoutMilliseconds)));
			DeserializeHelper(nowWithoutMilliseconds, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nowWithoutMilliseconds)));
		}

		[Test]
		public void Test_NullableDateTime()
		{
			var now = DateTime.Now;
			var nowWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			DateTime? nullable = nowWithoutMilliseconds;

			SerializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nullable.Value)));
			DeserializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nullable.Value)));

			nullable = null;
			SerializeHelper(nullable, null);
			DeserializeHelper(nullable, null);
		}

		[Test]
		public void Test_List()
		{
			var emptyList = new List<object>();
			SerializeHelper(emptyList, "<Tag array=\"true\" />");
			DeserializeHelper(emptyList, "<Tag array=\"true\" />");
			DeserializeHelper(emptyList, "<Tag array=\"true\"></Tag>");

			var stringList = new List<string> {"1"};
			SerializeHelper(stringList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\"><item>1</item></Tag>");

			var intList = new List<int> {1};
			SerializeHelper(intList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\"><item>1</item></Tag>");

			var doubleList = new List<double> {1.0};
			SerializeHelper(doubleList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\"><item>1</item></Tag>");

			var boolList = new List<bool> {true, false};
			SerializeHelper(boolList, "<Tag array=\"true\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\"><item>true</item><item>false</item></Tag>");
		}

		[Test]
		public void Test_Dictionary()
		{
			var emptyDictionary = new Dictionary<string, string>();
			SerializeHelper(emptyDictionary, "<Tag hash=\"true\" />");
			DeserializeHelper(emptyDictionary, "<Tag hash=\"true\" />");
			DeserializeHelper(emptyDictionary, "<Tag hash=\"true\"></Tag>");

			var strStrDictionary = new Dictionary<string, string> {{"key", "value"}};
			SerializeHelper(strStrDictionary, "<Tag hash=\"true\">\r\n  <key>value</key>\r\n</Tag>");
			DeserializeHelper(strStrDictionary, "<Tag hash=\"true\">\r\n  <key>value</key>\r\n</Tag>");
			DeserializeHelper(strStrDictionary, "<Tag hash=\"true\"><key>value</key></Tag>");

			var strIntDictionary = new Dictionary<string, int> {{"key", 5}};
			SerializeHelper(strIntDictionary, "<Tag hash=\"true\">\r\n  <key>5</key>\r\n</Tag>");
			DeserializeHelper(strIntDictionary, "<Tag hash=\"true\">\r\n  <key>5</key>\r\n</Tag>");
			DeserializeHelper(strIntDictionary, "<Tag hash=\"true\"><key>5</key></Tag>");
		}

		[Test]
		[ExpectedException(typeof(XmlException))]
		public void Test_Dictionary_InvalidKeyType()
		{
			// Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.
			var intStrDictionary = new Dictionary<int, string>();
			intStrDictionary[0] = "value";
			SerializeHelper(intStrDictionary, "<Tag hash=\"true\">\r\n  <0>value</0>\r\n</Tag>");
			DeserializeHelper(intStrDictionary, "<Tag hash=\"true\">\r\n  <0>value</0>\r\n</Tag>");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_Dictionary_InvalidValueType()
		{
			// Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.
			var dictionary = new Dictionary<string, TimeSpan>();
			dictionary["key"] = new TimeSpan(1, 0, 0);
			SerializeHelper(dictionary, "<Tag hash=\"true\">\r\n  <key>01:00:00</key>\r\n</Tag>");
			DeserializeHelper(dictionary, "<Tag hash=\"true\">\r\n  <key>01:00:00</key>\r\n</Tag>");
		}

		[Test]
		public void Test_EntityRef()
		{
			var guidEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0");
			SerializeHelper(guidEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0</Tag>");
			DeserializeHelper(guidEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0</Tag>");

			var stringEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1");
			SerializeHelper(stringEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1</Tag>");
			DeserializeHelper(stringEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1</Tag>");

			var intEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2");
			SerializeHelper(intEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2</Tag>");
			DeserializeHelper(intEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2</Tag>");

			var longEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3");
			SerializeHelper(longEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3</Tag>");
			DeserializeHelper(longEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3</Tag>");

			// Null EntityRef is acceptable.
			DeserializeHelper<EntityRef>(null, "");
		}

		[Test]
		[ExpectedException(typeof(SerializationException))]
		public void Test_EntityRef_EmptyTag()
		{
			// We always expect EntityRef jsml to contain something.  An empty EntityRef is not allowed.
			JsmlSerializer.Deserialize<EntityRef>("<Tag />");
		}

		[Test]
		public void Test_DataContract()
		{
			var contract1 = new TestContract1();
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);

			contract1.EntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:2");
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);

			var contract2 = new TestContract2();
			SerializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.Jsml);

			var now = DateTime.Now;
			contract2.Double = 5.0;
			contract2.Bool = true;
			contract2.NullableDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			contract2.ExtendedProperties = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
			SerializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.Jsml);
		}

		[Test]
		public void Test_SerializeOptions()
		{
			var now = DateTime.Now;
			var options = new JsmlSerializer.SerializeOptions { MemberFilter = (m => m.Name != "Double") }; 
			var contract2 = new TestContract2
					{
						Double = 5.0,
						Bool = true,
						NullableDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second),
						ExtendedProperties = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}}
					};

			var jsmlWithoutDouble = contract2.GetJsml(true);
			SerializeHelper(contract2, jsmlWithoutDouble, options);
		}

		#region Private helpers

		private static void SerializeHelper(object input, string expectedJsml)
		{
			SerializeHelper(input, expectedJsml, JsmlSerializer.SerializeOptions.Default);
		}

		private static void SerializeHelper(object input, string expectedJsml, JsmlSerializer.SerializeOptions options)
		{
			var jsml = JsmlSerializer.Serialize(input, "Tag", options);
			Assert.AreEqual(expectedJsml, jsml);
		}

		private static void DeserializeHelper<T>(T expectedValue, string jsml)
		{
			var value = JsmlSerializer.Deserialize<T>(jsml);

			if (null == expectedValue)
			{
				Assert.IsNull(value);
			}
			else if (expectedValue is ICollection)
			{
				Assert.IsTrue(CollectionUtils.Equal((ICollection) expectedValue, (ICollection) value, true));
			}
			else
			{
				Assert.IsTrue(expectedValue.Equals(value));
			}
		}

		#endregion
	}
}

#endif