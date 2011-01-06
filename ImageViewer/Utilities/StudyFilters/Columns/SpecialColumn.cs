#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		string GetText(IStudyItem item);
		object GetValue(IStudyItem item);
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