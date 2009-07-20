using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionPoint]
	public sealed class SpecialColumnExtensionPoint : ExtensionPoint<ISpecialColumn> {}

	public interface ISpecialColumn
	{
		string Name { get; }
		string Key { get; }
		IStudyFilter Owner { get; }
		string GetText(StudyItem item);
		object GetValue(StudyItem item);
		Type GetValueType();
	}

	public abstract class SpecialColumn<T> : StudyFilterColumnBase<T>, ISpecialColumn
	{
		private readonly string _key;
		private readonly string _name;

		protected SpecialColumn(string name, string key)
		{
			_name = name;
			_key = key;
		}

		public override sealed string Name
		{
			get { return _name; }
		}

		public override sealed string Key
		{
			get { return _key; }
		}
	}
}