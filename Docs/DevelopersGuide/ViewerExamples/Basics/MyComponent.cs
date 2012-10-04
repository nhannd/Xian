#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

// ... (other using namespace statements here)

namespace MyPlugin.Basics
{
	// your component's view extension point class
	[ExtensionPoint()]
	public class MyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	// your component
	[AssociateView(typeof (MyComponentViewExtensionPoint))]
	public class MyComponent : ApplicationComponent
	{
		// your component code here
	}
}