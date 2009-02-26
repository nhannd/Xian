using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageServer.Common.Authentication
{
    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
    public class DefineAuthorityGroups : IDefineAuthorityGroups
    {
        #region IDefineAuthorityGroups Members

        public AuthorityGroupDefinition[] GetAuthorityGroups()
        {
            AuthorityGroupDefinition admins = new AuthorityGroupDefinition("PACS Admins",
                                            new string[]
                                                {
                                                    AuthorityTokens.Admin.Alert.Delete,
                                                    AuthorityTokens.Admin.Alert.View,
                                                    AuthorityTokens.Admin.ApplicationLog.Search,
                                                    AuthorityTokens.Admin.Configuration.Devices,
                                                    AuthorityTokens.Admin.Configuration.FileSystems,
                                                    AuthorityTokens.Admin.Configuration.ServerPartitions,
                                                    AuthorityTokens.Admin.Configuration.ServerRules,
                                                    AuthorityTokens.Admin.Configuration.ServiceScheduling,

                                                    AuthorityTokens.ArchiveQueue.Delete,
                                                    AuthorityTokens.ArchiveQueue.Search,

                                                    AuthorityTokens.RestoreQueue.Delete,
                                                    AuthorityTokens.RestoreQueue.Search,

                                                    AuthorityTokens.Study.Delete,
                                                    AuthorityTokens.Study.Edit,
                                                    AuthorityTokens.Study.Move,
                                                    AuthorityTokens.Study.Restore,
                                                    AuthorityTokens.Study.Search,
                                                    AuthorityTokens.Study.View,

                                                    AuthorityTokens.StudyIntegrityQueue.Search,
                                                    AuthorityTokens.StudyIntegrityQueue.Reconcile,

                                                    AuthorityTokens.WorkQueue.Delete,
                                                    AuthorityTokens.WorkQueue.Reprocess,
                                                    AuthorityTokens.WorkQueue.Reschedule,
                                                    AuthorityTokens.WorkQueue.Reset,
                                                    AuthorityTokens.WorkQueue.Search,
                                                    AuthorityTokens.WorkQueue.View,
                                                    
                                                });

            AuthorityGroupDefinition users = new AuthorityGroupDefinition("PACS Users",
                                            new string[]
                                                {
                                                    //AuthorityTokens.Admin.Alert.Delete,
                                                    //AuthorityTokens.Admin.Alert.View,
                                                    //AuthorityTokens.Admin.ApplicationLog.Search,
                                                    //AuthorityTokens.Admin.Configuration.Devices,
                                                    //AuthorityTokens.Admin.Configuration.FileSystems,
                                                    //AuthorityTokens.Admin.Configuration.ServerPartitions,
                                                    //AuthorityTokens.Admin.Configuration.ServerRules,
                                                    //AuthorityTokens.Admin.Configuration.ServiceScheduling,

                                                    AuthorityTokens.ArchiveQueue.Delete,
                                                    AuthorityTokens.ArchiveQueue.Search,

                                                    AuthorityTokens.RestoreQueue.Delete,
                                                    AuthorityTokens.RestoreQueue.Search,

                                                    AuthorityTokens.Study.Delete,
                                                    AuthorityTokens.Study.Edit,
                                                    AuthorityTokens.Study.Move,
                                                    AuthorityTokens.Study.Restore,
                                                    AuthorityTokens.Study.Search,
                                                    AuthorityTokens.Study.View,

                                                    AuthorityTokens.StudyIntegrityQueue.Search,
                                                    AuthorityTokens.StudyIntegrityQueue.Reconcile,

                                                    AuthorityTokens.WorkQueue.Delete,
                                                    AuthorityTokens.WorkQueue.Reprocess,
                                                    AuthorityTokens.WorkQueue.Reschedule,
                                                    AuthorityTokens.WorkQueue.Reset,
                                                    AuthorityTokens.WorkQueue.Search,
                                                    AuthorityTokens.WorkQueue.View,
                                                    
                                                });


            return new AuthorityGroupDefinition[] {admins, users};
        }

        #endregion
    }

}
