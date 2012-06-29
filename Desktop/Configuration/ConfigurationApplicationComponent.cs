#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Configuration
{
	public abstract class ConfigurationApplicationComponentContainer: ApplicationComponentContainer, IConfigurationApplicationComponent
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigurationApplicationComponentContainer()
		{
		}

		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		public abstract void Save();

		#region IConfigurationApplicationComponent Members

		void IConfigurationApplicationComponent.Save()
		{
			this.Save();
			base.Modified = false;
		}

		#endregion
	}

	/// <summary>
	/// A component that hosts a configuration page, where some settings need to
	/// be saved when the user dismisses it.
	/// </summary>
	public abstract class ConfigurationApplicationComponent : ApplicationComponent, IConfigurationApplicationComponent
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigurationApplicationComponent()
		{
		}

		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		public abstract void Save();

		#region IConfigurationApplicationComponent Members

		void IConfigurationApplicationComponent.Save()
		{
			this.Save();
			Modified = false;
		}

		#endregion
	}
}
