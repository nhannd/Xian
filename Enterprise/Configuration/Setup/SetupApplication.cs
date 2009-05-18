#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;

namespace ClearCanvas.Enterprise.Configuration.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class SetupApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            SetupCommandLine cmdLine = new SetupCommandLine();
            try
            {
                cmdLine.Parse(args);
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
				{
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;

                    IConfigurationSettingsGroupBroker broker = scope.Context.GetBroker<IConfigurationSettingsGroupBroker>();

					// import authority tokens
                    List<SettingsGroupDescriptor> descriptors = SettingsGroupDescriptor.ListInstalledSettingsGroups(true);
                    foreach (SettingsGroupDescriptor descriptor in descriptors)
                    {
                        ConfigurationSettingsGroupSearchCriteria where = ConfigurationSettingsGroup.GetCriteria(descriptor);
                        ConfigurationSettingsGroup group = 
                                CollectionUtils.FirstElement(broker.Find(where));
                        if (group == null)
                        {
                            // group doesn't exist, need to create it
                            group = new ConfigurationSettingsGroup();
                            group.UpdateFromDescriptor(descriptor);
                            scope.Context.Lock(group, DirtyState.New);
                        }
                        else
                        {
                            // update group from descriptor
                            group.UpdateFromDescriptor(descriptor);
                        }

                        UpdateProperties(group, descriptor);
                    }

					scope.Complete();
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
        }

        #endregion

        private void UpdateProperties(ConfigurationSettingsGroup group, SettingsGroupDescriptor groupDescriptor)
        {
            group.SettingsProperties.Clear();
            List<SettingsPropertyDescriptor> descriptors = SettingsPropertyDescriptor.ListSettingsProperties(groupDescriptor);
            foreach (SettingsPropertyDescriptor descriptor in descriptors)
            {
                ConfigurationSettingsProperty property = new ConfigurationSettingsProperty();
                property.UpdateFromDescriptor(descriptor);
                group.SettingsProperties.Add(property);
            }
        }
    }
}
