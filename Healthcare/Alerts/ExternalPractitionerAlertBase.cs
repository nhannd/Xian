#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Alerts
{
	[ExtensionPoint]
	public class ExternalPractitionerAlertExtensionPoint : ExtensionPoint<IExternalPractitionerAlert>
	{
	}

	public interface IExternalPractitionerAlert : IAlert<ExternalPractitioner>
	{
	}

	public abstract class ExternalPractitionerAlertBase : AlertBase<ExternalPractitioner>, IExternalPractitionerAlert
	{
	}
}
