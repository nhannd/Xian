SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Altering [dbo].[AlertLevelEnum]'
GO
ALTER TABLE [dbo].[AlertLevelEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyIntegrityReasonEnum]'
GO
ALTER TABLE [dbo].[StudyIntegrityReasonEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyStatusEnum]'
GO
ALTER TABLE [dbo].[StudyStatusEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyHistoryTypeEnum]'
GO
ALTER TABLE [dbo].[StudyHistoryTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating [dbo].[StudyDeleteRecord]'
GO
CREATE TABLE [dbo].[StudyDeleteRecord]
(
[GUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_StudyDeleteRecord_GUID] DEFAULT (newid()),
[Timestamp] [datetime] NOT NULL,
[Reason] [nvarchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ServerPartitionAE] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[FilesystemGUID] [uniqueidentifier] NOT NULL,
[BackupPath] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StudyInstanceUid] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AccessionNumber] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PatientId] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PatientsName] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StudyId] [nvarchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StudyDescription] [nvarchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StudyDate] [varchar] (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StudyTime] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ArchiveInfo] [xml] NULL,
[ExtendedInfo] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_StudyDeleteRecord] on [dbo].[StudyDeleteRecord]'
GO
ALTER TABLE [dbo].[StudyDeleteRecord] ADD CONSTRAINT [PK_StudyDeleteRecord] PRIMARY KEY CLUSTERED  ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord] on [dbo].[StudyDeleteRecord]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_StudyDeleteRecord] ON [dbo].[StudyDeleteRecord] ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_Timestamp] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_Timestamp] ON [dbo].[StudyDeleteRecord] ([Timestamp])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_ServerPartition] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_ServerPartition] ON [dbo].[StudyDeleteRecord] ([ServerPartitionAE])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_StudyInstanceUidServerPartition] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_StudyInstanceUidServerPartition] ON [dbo].[StudyDeleteRecord] ([StudyInstanceUid], [ServerPartitionAE])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_StudyInstanceUid] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_StudyInstanceUid] ON [dbo].[StudyDeleteRecord] ([StudyInstanceUid])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_AcccessionNumber] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_AcccessionNumber] ON [dbo].[StudyDeleteRecord] ([AccessionNumber])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_PatientId] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_PatientId] ON [dbo].[StudyDeleteRecord] ([PatientId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyDeleteRecord_PatientsName] on [dbo].[StudyDeleteRecord]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_PatientsName] ON [dbo].[StudyDeleteRecord] ([PatientsName])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating [dbo].[ApplicationLog]'
GO
CREATE TABLE [dbo].[ApplicationLog]
(
[GUID] [uniqueidentifier] NOT NULL ROWGUIDCOL CONSTRAINT [DF_ApplicationLog_GUID] DEFAULT (newid()),
[Host] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Timestamp] [datetime] NOT NULL,
[LogLevel] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Thread] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Message] [varchar] (3000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Exception] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_ApplicationLog] on [dbo].[ApplicationLog]'
GO
CREATE CLUSTERED INDEX [IX_ApplicationLog] ON [dbo].[ApplicationLog] ([Timestamp])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_ApplicationLog] on [dbo].[ApplicationLog]'
GO
ALTER TABLE [dbo].[ApplicationLog] ADD CONSTRAINT [PK_ApplicationLog] PRIMARY KEY NONCLUSTERED  ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[FilesystemTierEnum]'
GO
ALTER TABLE [dbo].[FilesystemTierEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ArchiveTypeEnum]'
GO
ALTER TABLE [dbo].[ArchiveTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[DuplicateSopPolicyEnum]'
GO
ALTER TABLE [dbo].[DuplicateSopPolicyEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[AlertCategoryEnum]'
GO
ALTER TABLE [dbo].[AlertCategoryEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[QueueStudyStateEnum]'
GO
ALTER TABLE [dbo].[QueueStudyStateEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ServerPartition]'
GO
ALTER TABLE [dbo].[ServerPartition] ADD
[AuditDeleteStudy] [bit] NOT NULL CONSTRAINT [DF_ServerPartition_AuditDeleteStudy] DEFAULT ((0))
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ServerRuleTypeEnum]'
GO
ALTER TABLE [dbo].[ServerRuleTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ServerRuleApplyTimeEnum]'
GO
ALTER TABLE [dbo].[ServerRuleApplyTimeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[WorkQueueStatusEnum]'
GO
ALTER TABLE [dbo].[WorkQueueStatusEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ServiceLockTypeEnum]'
GO
ALTER TABLE [dbo].[ServiceLockTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[WorkQueueTypeEnum]'
GO
ALTER TABLE [dbo].[WorkQueueTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[WorkQueuePriorityEnum]'
GO
ALTER TABLE [dbo].[WorkQueuePriorityEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[ArchiveQueueStatusEnum]'
GO
ALTER TABLE [dbo].[ArchiveQueueStatusEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[FilesystemQueueTypeEnum]'
GO
ALTER TABLE [dbo].[FilesystemQueueTypeEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[RestoreQueueStatusEnum]'
GO
ALTER TABLE [dbo].[RestoreQueueStatusEnum] ALTER COLUMN [LongDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating [dbo].[CannedText]'
GO
CREATE TABLE [dbo].[CannedText]
(
[GUID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Category] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Text] [nvarchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_CannedText] on [dbo].[CannedText]'
GO
ALTER TABLE [dbo].[CannedText] ADD CONSTRAINT [PK_CannedText] PRIMARY KEY CLUSTERED  ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_CannedText_Name] on [dbo].[CannedText]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CannedText_Name] ON [dbo].[CannedText] ([Name])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_CannedText_Category] on [dbo].[CannedText]'
GO
CREATE NONCLUSTERED INDEX [IX_CannedText_Category] ON [dbo].[CannedText] ([Category])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[StudyDeleteRecord]'
GO
ALTER TABLE [dbo].[StudyDeleteRecord] ADD
CONSTRAINT [FK_StudyDeleteRecord_Filesystem] FOREIGN KEY ([FilesystemGUID]) REFERENCES [dbo].[Filesystem] ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServerRuleApplyTimeEnum]'
GO
UPDATE [dbo].[ServerRuleApplyTimeEnum]
SET LongDescription = 'Apply rule after a Study has been processed'
WHERE Lookup='StudyProcessed'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[FilesystemQueueTypeEnum]'
GO
UPDATE [dbo].[FilesystemQueueTypeEnum]
SET LongDescription = 'A record telling when a study is eligable for deletion.  The study will be completely removed from the system.'
WHERE Lookup='DeleteStudy'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[FilesystemQueueTypeEnum]'
GO
UPDATE [dbo].[FilesystemQueueTypeEnum]
SET LongDescription = 'A record telling when a study can be purged from a filesystem.  Only archived studies can be purged.  The study will remain archived and can be restored.'
WHERE Lookup='PurgeStudy'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[FilesystemQueueTypeEnum]'
GO
UPDATE [dbo].[FilesystemQueueTypeEnum]
SET LongDescription = 'A record telling when a study is eligable to be migrated to a lower tier filesystem.'
WHERE Lookup='TierMigrate'
GO
PRINT N'Updating [dbo].[FilesystemQueueTypeEnum]'
GO
UPDATE [dbo].[FilesystemQueueTypeEnum]
SET LongDescription = 'A record telling when a study is eligable for lossless compression and the type of compression to be performed on the study.'
WHERE Lookup='LosslessCompress'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[FilesystemQueueTypeEnum]'
GO
UPDATE [dbo].[FilesystemQueueTypeEnum]
  SET LongDescription = 'A record telling when a study is eligable for lossy compression and the type of compression to be performed.'
WHERE Lookup='LossyCompress'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
  SET LongDescription = 'This services checks if a filesystem is above its high watermark.  If the filesystem is above the high watermark it migrates studies, deletes studies, and purges studies until the low watermark is reached.'
WHERE Lookup='FilesystemDelete'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
   SET LongDescription = 'This service re-inventories the studies stored on a filesystem.  It scans the contents of the filesystem, and if a study is not already stored in the database, it will insert records to process the study into the WorkQueue.'
WHERE Lookup='FilesystemReinventory'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
  SET LongDescription = 'This service scans the contents of a filesystem and reapplies Study Processing rules to all studies on the filesystem.'
WHERE Lookup='FilesystemStudyProcess'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
  SET LongDescription = 'This service checks for studies that are eligable to be lossless compressed on a filesystem.  It works independently from the watermarks configured for the filesystem and will insert records into the WorkQueue to compress the studies as soon as they are eligable.'
WHERE Lookup='FilesystemLosslessCompress'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
  SET LongDescription = 'This service checks for studies that are eligable to be lossy compressed on a filesystem.  It works independently from the watermarks configured for the filesystem and will insert records into the WorkQueue to compress the studies as soon as they are eligable.'
where Lookup='FilesystemLossyCompress'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[ServiceLockTypeEnum]'
GO
INSERT INTO [dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'ArchiveApplicationLog','Archive Application Log','This service removes application log entries from the database and archives them in zip files to a filesystem.  When initially run, it selects a filesystem from the lowest filesystem tier configured on the system.')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[ServiceLockTypeEnum]'
GO
INSERT INTO [dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'PurgeAlerts','Purge Alerts','This service by default removes Alert records from the database after a configurable time.  If configured it can save the alerts in zip files on a filesystem.  When initially run, it selects a filesystem from the lowest filesystem tier configured on the system to archive to.')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[QueueStudyStateEnum]'
GO
UPDATE [dbo].[QueueStudyStateEnum]
  SET LongDescription = 'The study is scheduled for deletion.'
WHERE Lookup='WebDeleteScheduled'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[StudyHistoryTypeEnum]'
GO
INSERT INTO [dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'WebEdited','Web GUI Edited','Study was edited via the Web GUI')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[ServiceLock]'
GO
DECLARE @ArchiveApplicationLogServiceLockTypeEnum smallint
SELECT @ArchiveApplicationLogServiceLockTypeEnum = Enum FROM [dbo].ServiceLockTypeEnum WHERE [Lookup] = 'ArchiveApplicationLog'

INSERT INTO [dbo].[ServiceLock]
	([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
VALUES (newid(),@ArchiveApplicationLogServiceLockTypeEnum,0,getdate(),null,1)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[ServiceLock]'
GO
DECLARE @PurgeAlertsServiceLockTypeEnum smallint
SELECT @PurgeAlertsServiceLockTypeEnum = Enum FROM [dbo].ServiceLockTypeEnum WHERE [Lookup] = 'PurgeAlerts'

INSERT INTO [dbo].[ServiceLock]
	([ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
VALUES (@PurgeAlertsServiceLockTypeEnum,0,getdate(),null,1)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[CannedText]'
GO
INSERT INTO [dbo].[CannedText]([GUID],[Name],[Category],[Text])
     VALUES(newid(), 'Corrupted study', 'DeleteStudyReason', 'Study is corrupted.')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new row into [dbo].[CannedText]'
GO
INSERT INTO [dbo].[CannedText]([GUID],[Name],[Category],[Text])
     VALUES(newid(), 'Invalid data', 'DeleteStudyReason', 'Study contains some invalid data.')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO
