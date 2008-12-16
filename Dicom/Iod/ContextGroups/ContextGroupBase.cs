using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public abstract class ContextGroupBase<T> : IEnumerable<T> where T : ContextGroupBase<T>.ContextGroupItemBase
	{
		public readonly int ContextId;
		public readonly string ContextGroupName;
		public readonly bool IsExtensible;
		public readonly DateTime Version;

		protected ContextGroupBase(int contextId, string contextGroupName, bool isExtensible, DateTime version)
		{
			Platform.CheckForNullReference(contextGroupName, "contextGroupName");

			this.ContextId = contextId;
			this.ContextGroupName = contextGroupName;
			this.IsExtensible = isExtensible;
			this.Version = version;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public abstract IEnumerator<T> GetEnumerator();

		public void Apply(T value, CodeSequenceMacro codeSequence)
		{
			value.ApplyToCodeSequence(codeSequence);
		}

		public T Parse(CodeSequenceMacro codeSequence)
		{
			return this.Parse(codeSequence, false);
		}

		public T Parse(CodeSequenceMacro codeSequence, bool compareCodingSchemeVersion)
		{
			Platform.CheckForNullReference(codeSequence, "codeSequence");

			StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
			string codeValue = codeSequence.CodeValue;
			string codingSchemeDesignator = codeSequence.CodingSchemeDesignator;
			string codingSchemeVersion = codeSequence.CodingSchemeVersion;

			foreach (T t in this)
			{
				if (comparer.Equals(t.CodeValue, codeValue) && comparer.Equals(t.CodingSchemeDesignator, codingSchemeDesignator) && (!compareCodingSchemeVersion || comparer.Equals(t.CodingSchemeVersion, codingSchemeVersion)))
					return t;
			}

			throw new InvalidCastException(string.Format("The encoded code sequence is not a member of the context group {0} (DCID{1}).", this.ContextGroupName, this.ContextId));
		}

		public abstract class ContextGroupItemBase
		{
			public readonly string CodingSchemeDesignator;
			public readonly string CodingSchemeVersion;
			public readonly string CodeValue;
			public readonly string CodeMeaning;

			protected ContextGroupItemBase(string codingSchemeDesignator, string codeValue, string codeMeaning) : this(codingSchemeDesignator, null, codeValue, codeMeaning) {}

			protected ContextGroupItemBase(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			{
				Platform.CheckForEmptyString(codingSchemeDesignator, "codingSchemeDesignator");
				Platform.CheckForEmptyString(codeValue, "codeValue");

				this.CodingSchemeDesignator = codingSchemeDesignator;
				this.CodingSchemeVersion = codingSchemeVersion;
				this.CodeValue = codeValue;
				this.CodeMeaning = codeMeaning;
			}

			public bool MatchesCodeSequence(CodeSequenceMacro codeSequence)
			{
				return MatchesCodeSequence(codeSequence, false);
			}

			public bool MatchesCodeSequence(CodeSequenceMacro codeSequence, bool compareCodingSchemeVersion)
			{
				Platform.CheckForNullReference(codeSequence, "codeSequence");

				StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
				bool result = comparer.Equals(this.CodeValue, codeSequence.CodeValue);
				result = result && comparer.Equals(this.CodingSchemeDesignator, codeSequence.CodingSchemeDesignator);
				if (compareCodingSchemeVersion)
					result = result && comparer.Equals(this.CodingSchemeVersion, codeSequence.CodingSchemeVersion);
				return result;
			}

			public void ApplyToCodeSequence(CodeSequenceMacro codeSequence)
			{
				Platform.CheckForNullReference(codeSequence, "codeSequence");

				codeSequence.CodeMeaning = this.CodeMeaning;
				codeSequence.CodeValue = this.CodeValue;
				codeSequence.CodingSchemeDesignator = this.CodingSchemeDesignator;

				if (!string.IsNullOrEmpty(this.CodingSchemeVersion))
					codeSequence.CodingSchemeVersion = this.CodingSchemeVersion;
			}

			public override string ToString() {
				return string.Format("{0} {1} ({2})", this.CodeValue, this.CodeMeaning, this.CodingSchemeDesignator, this.CodingSchemeVersion);
			}

			public static implicit operator CodeSequenceMacro(ContextGroupItemBase value)
			{
				if (value == null)
					throw new ArgumentNullException();
				CodeSequenceMacro codeSequence = new CodeSequenceMacro();
				value.ApplyToCodeSequence(codeSequence);
				return codeSequence;
			}
		}
	}
}