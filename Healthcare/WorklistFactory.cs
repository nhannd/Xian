#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Manages instantiation of worklists.
	/// </summary>
	public class WorklistFactory
	{
		#region Static members

		private static readonly WorklistFactory _theInstance = new WorklistFactory();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static WorklistFactory Instance
		{
			get { return _theInstance; }
		}

		#endregion

		private readonly List<Type> _worklistClasses;

		private WorklistFactory()
		{
			_worklistClasses = CollectionUtils.Map(new WorklistExtensionPoint().ListExtensions(), (ExtensionInfo info) => info.ExtensionClass);
		}

		/// <summary>
		/// Lists all known worklist classes (extensions of <see cref="WorklistExtensionPoint"/>), optionally
		/// including those marked with the <see cref="StaticWorklistAttribute"/>.
		/// </summary>
		/// <param name="includeStatic"></param>
		/// <returns></returns>
		public Type[] ListWorklistClasses(bool includeStatic)
		{
			return includeStatic ? _worklistClasses.ToArray() :
				CollectionUtils.Select(_worklistClasses, wc => !Worklist.GetIsStatic(wc)).ToArray();
		}

		/// <summary>
		/// Creates an instance of the worklist class as specified by the class name, which may be fully or
		/// only partially qualified.
		/// </summary>
		/// <param name="worklistClassName"></param>
		/// <returns></returns>
		public Worklist CreateWorklist(string worklistClassName)
		{
			return CreateWorklist(ResolvePartialClassName(worklistClassName));
		}

		/// <summary>
		/// Creates an instance of the specified worklist class.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public Worklist CreateWorklist(Type worklistClass)
		{
			return (Worklist)Activator.CreateInstance(worklistClass);
		}


		private Type ResolvePartialClassName(string worklistClassName)
		{
			var worklistClass = CollectionUtils.SelectFirst(_worklistClasses, t => t.FullName.Contains(worklistClassName));

			if (worklistClass == null)
				throw new ArgumentException(string.Format("{0} is not a valid worklist class name.", worklistClassName));

			return worklistClass;
		}
	}
}