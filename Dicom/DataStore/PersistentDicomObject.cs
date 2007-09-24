using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
	public abstract class PersistentDicomObject
	{
		private event EventHandler _changed;

		protected PersistentDicomObject()
		{
		}

		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		protected void SetValueTypeMember<T>(ref T member, T newValue)
			where T : struct
		{
			if (member.Equals(newValue))
				return;

			member = newValue;
			OnChanged();
		}

		protected void OnChanged()
		{
			EventsHelper.Fire(_changed, this, EventArgs.Empty);
		}

		protected void SetClassMember<T>(ref T member, T newValue)
			where T : class
		{
			if (Nullable.Equals(member, newValue))
				return;

			member = newValue;
			OnChanged();
		}

		protected void SetNullableTypeMember<T>(ref T? member, T? newValue)
			where T : struct
		{
			if (Nullable.Equals(member, newValue))
				return;

			member = newValue;
			OnChanged();
		}
	}
}
