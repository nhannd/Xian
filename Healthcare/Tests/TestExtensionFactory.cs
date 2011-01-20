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

namespace ClearCanvas.Healthcare.Tests
{
	class TestExtensionFactory : IExtensionFactory
	{
		#region IExtensionFactory Members

		public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
		{
			if (extensionPoint.GetType() == typeof(ProcedureStepBuilderExtensionPoint))
			{
				return new object[] { new ModalityProcedureStepBuilder() };
			}

			return new object[] { };
		}

		public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
