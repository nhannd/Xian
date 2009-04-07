using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{
    public class AuthorityTokens
    {
        /// <summary>
        /// Tokens that allow access to administrative functionality.
        /// </summary>
        public static class Admin
        {
            /// <summary>
            /// Tokens that allow access to configuration.
            /// </summary>
            public static class Configuration
            {
                [AuthorityToken(Description = "Allow configuration of devices.")]
                public const string Devices = "PACS/Configure/Devices";

                [AuthorityToken(Description = "Allow configuration of server partitions.")]
                public const string ServerPartitions = "PACS/Configure/ServerPartitions";

                [AuthorityToken(Description = "Allow configuration of file systems.")]
                public const string FileSystems = "PACS/Configure/FileSystems";

                [AuthorityToken(Description = "Allow configuration of server rules.")]
                public const string ServerRules = "PACS/Configure/ServerRules";

                [AuthorityToken(Description = "Allow configuration of service scheduling.")]
                public const string ServiceScheduling = "PACS/Configure/ServiceScheduling";

                [AuthorityToken(Description = "Allow configuration of partition archives.")]
                public const string PartitionArchive = "PACS/Configure/PartitionArchive";

            }

            /// <summary>
            /// Tokens that allow access to alerts.
            /// </summary>
            public static class Alert
            {
                [AuthorityToken(Description = "Allow viewing alerts on the systems.")]
                public const string View = "PACS/Alert/View";

                [AuthorityToken(Description = "Allow deleting alerts on the systems.")]
                public const string Delete = "PACS/Alert/Delete";
            }

            /// <summary>
            /// Tokens that allow access to application logs.
            /// </summary>
            public static class ApplicationLog
            {
                [AuthorityToken(Description = "Allow searching for application log.")]
                public const string Search = "PACS/ApplicationLog/Search";
            }

            /// <summary>
            /// Tokens that allow access to study deletion history.
            /// </summary>
            public static class StudyDeleteHistory
            {
                [AuthorityToken(Description = "Allow searching for study delete history.")]
                public const string Search = "PACS/StudyDeleteHistory/Search";

                [AuthorityToken(Description = "Allow viewing details of the study deletion record.")]
                public const string View = "PACS/StudyDeleteHistory/View";

                [AuthorityToken(Description = "Allow deleting study delete history records.")]
                public const string Delete = "PACS/StudyDeleteHistory/Delete";
            }

        }

        /// <summary>
        /// Tokens that allow access to study functionalities.
        /// </summary>
        public static class Study
        {
            [AuthorityToken(Description = "Allow searching for studies.")]
            public const string Search = "PACS/Study/Search";

            [AuthorityToken(Description = "Allow viewing study details.")]
            public const string View = "PACS/Study/View";

            [AuthorityToken(Description = "Allow moving studies.")]
            public const string Move = "PACS/Study/Move";

            [AuthorityToken(Description = "Allow deleting studies.")]
            public const string Delete = "PACS/Study/Delete";

            [AuthorityToken(Description = "Allow editing studies.")]
            public const string Edit = "PACS/Study/Edit";

            [AuthorityToken(Description = "Allow restoring studies.")]
            public const string Restore = "PACS/Study/Restore";
        }

        /// <summary>
        /// Tokens that allow access to Work Queue functionalities.
        /// </summary>
        public static class WorkQueue
        {
            [AuthorityToken(Description = "Allow searching for work queue items.")]
            public const string Search = "PACS/WorkQueue/Search";

            [AuthorityToken(Description = "Allow viewing work queue entry details.")]
            public const string View = "PACS/WorkQueue/View";

            [AuthorityToken(Description = "Allow rescheduling work queue items.")]
            public const string Reschedule = "PACS/WorkQueue/Reschedule";

            [AuthorityToken(Description = "Allow reseting work queue items.")]
            public const string Reset = "PACS/WorkQueue/Reset";

            [AuthorityToken(Description = "Allow deleting work queue items.")]
            public const string Delete = "PACS/WorkQueue/Delete";

            [AuthorityToken(Description = "Allow reprocessing work queue items.")]
            public const string Reprocess = "PACS/WorkQueue/Reprocess";
        }

        /// <summary>
        /// Tokens that allow access to Archive Queue functionalities.
        /// </summary>
        public static class ArchiveQueue
        {
            [AuthorityToken(Description = "Allow searching for archive queue entries.")]
            public const string Search = "PACS/ArchiveQueue/Search";

            [AuthorityToken(Description = "Allow deleting archive queue entries.")]
            public const string Delete = "PACS/ArchiveQueue/Delete";
        }

        /// <summary>
        /// Tokens that allow access to Restore Queue functionalities.
        /// </summary>
        public static class RestoreQueue
        {
            [AuthorityToken(Description = "Allow searching for restore queue entries.")]
            public const string Search = "PACS/RestoreQueue/Search";

            [AuthorityToken(Description = "Allow deleting restore queue entries.")]
            public const string Delete = "PACS/RestoreQueue/Delete";
        }

        /// <summary>
        /// Tokens that allow access to Study Integrity Queue functionalities.
        /// </summary>
        public static class StudyIntegrityQueue
        {
            [AuthorityToken(Description = "Allow searching for study integrity queue entries.")]
            public const string Search = "PACS/StudyIntegrityQueue/Search";

            [AuthorityToken(Description = "Allow reconciling studies in the study integrity queue.")]
            public const string Reconcile = "PACS/StudyIntegrityQueue/Reconcile";
        }
    }
}