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


set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


PRINT N'Updating InsertArchiveQueue stored procedure'
GO
-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 11, 2008
-- Update date: Oct 28, 2011
-- Description:	Insert and/or update the appropriate ArchiveQueue records
-- 
-- Oct 28, 2011:	(#8866) Ensured existing entries are reset (if failed) and only insert new ones if no entries are found for the studies
-- Oct 15, 2008:	Removed Update parameter and insert new entry if the study has been archive so that edit can trigger rearchive
--
-- =============================================
ALTER PROCEDURE [dbo].[InsertArchiveQueue] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @StudyDeleteTypeEnum smallint
	DECLARE @StudyPurgeTypeEnum smallint
	DECLARE @PendingArchiveQueueStatus smallint
	DECLARE @FailedArchiveQueueStatus smallint
	DECLARE @StudyDeleteCount int
	DECLARE @StudyPurgeCount int
	DECLARE @ArchiveStudyStorageCount int
	DECLARE @ArchiveDelayHours int
	DECLARE	@PartitionArchiveGUID uniqueidentifier

	SELECT @StudyDeleteTypeEnum = Enum from FilesystemQueueTypeEnum where Lookup = 'DeleteStudy'
	SELECT @StudyPurgeTypeEnum = Enum from FilesystemQueueTypeEnum where Lookup = 'PurgeStudy'
	SELECT @PendingArchiveQueueStatus = Enum from ArchiveQueueStatusEnum where Lookup = 'Pending'
	SELECT @FailedArchiveQueueStatus = Enum from ArchiveQueueStatusEnum where Lookup = 'Failed'

	BEGIN TRANSACTION

	-- Check if there's any DeleteStudy records in the db
	SELECT @StudyDeleteCount=count(*) FROM FilesystemQueue 
		WHERE FilesystemQueueTypeEnum=@StudyDeleteTypeEnum
			AND StudyStorageGUID=@StudyStorageGUID

	-- Check if there's any PurgeStudy records in the db
	SELECT @StudyPurgeCount=count(*) FROM FilesystemQueue 
		WHERE FilesystemQueueTypeEnum=@StudyPurgeTypeEnum
			AND StudyStorageGUID=@StudyStorageGUID


	IF @StudyDeleteCount = 0
	BEGIN
		-- Use a cursor to find all the configured ArchiveQueue entries
		DECLARE PartitionArchiveCursor Cursor FOR
			SELECT GUID, ArchiveDelayHours from dbo.PartitionArchive WHERE ServerPartitionGUID=@ServerPartitionGUID AND Enabled=1 AND ReadOnly=0
		Open PartitionArchiveCursor
		Fetch NEXT FROM PartitionArchiveCursor INTO @PartitionArchiveGUID, @ArchiveDelayHours
		While (@@FETCH_STATUS <> -1)
		BEGIN
			-- Check if the study's been already archived
			SELECT @ArchiveStudyStorageCount=count(*) FROM ArchiveStudyStorage 
				WHERE StudyStorageGUID=@StudyStorageGUID
				AND PartitionArchiveGUID = @PartitionArchiveGUID
			
			DECLARE @ArchiveQueueGUID uniqueidentifier
			DECLARE @ScheduledTime datetime

			set @ScheduledTime = getdate()
			set @ScheduledTime = dateadd(hour, @ArchiveDelayHours, @ScheduledTime)

			SELECT @ArchiveQueueGUID = GUID from ArchiveQueue 
			WHERE StudyStorageGUID = @StudyStorageGUID
				AND PartitionArchiveGUID = @PartitionArchiveGUID
			if @@ROWCOUNT = 0
			BEGIN
				-- There's no archive entry, insert one
				SET @ArchiveQueueGUID = NEWID();

				INSERT into ArchiveQueue (GUID, PartitionArchiveGUID, StudyStorageGUID, ArchiveQueueStatusEnum, ScheduledTime)
				values  (@ArchiveQueueGUID, @PartitionArchiveGUID, @StudyStorageGUID, @PendingArchiveQueueStatus, @ScheduledTime)
			END
			ELSE
			BEGIN
				-- Reset/reschedule existing entries, make sure the failed ones are reset to pending
				UPDATE ArchiveQueue 
				SET ScheduledTime = @ScheduledTime,
					ArchiveQueueStatusEnum=@PendingArchiveQueueStatus
				WHERE StudyStorageGUID = @StudyStorageGUID
					AND PartitionArchiveGUID = @PartitionArchiveGUID
					AND (ArchiveQueueStatusEnum in (@PendingArchiveQueueStatus, @FailedArchiveQueueStatus))
			END

			Fetch NEXT FROM PartitionArchiveCursor INTO @PartitionArchiveGUID, @ArchiveDelayHours
		END
		CLOSE PartitionArchiveCursor
		DEALLOCATE PartitionArchiveCursor	
		
		IF @StudyPurgeCount > 0
		BEGIN
			DELETE FROM FilesystemQueue 
			WHERE FilesystemQueueTypeEnum=@StudyPurgeTypeEnum
			AND StudyStorageGUID=@StudyStorageGUID
		END
	END
	ELSE
	BEGIN
		-- Delete from the ArchiveQueue, the study is scheduled for deletion
		-- Only delete entries that are Pending or Failed. In most cases this should delete no rows
		DELETE FROM ArchiveQueue
			WHERE StudyStorageGUID = @StudyStorageGUID
				AND ArchiveQueueStatusEnum = @PendingArchiveQueueStatus
	END

	COMMIT TRANSACTION
END
GO


PRINT N'Adding new ServiceLockTypeEnum, SyncDataAccess'
GO
INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),300,'SyncDataAccess','Synchronize Data Access','This service periodically synchronizes the deletion status of Authority Groups on the Administrative Services with Data Access granted to studies on the ImageServer.')
GO
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding new ServerRuleTypeEnum, DataAccess'
GO
INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'DataAccess','Data Access','A rule to specify the Authority Groups that have access to a study')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding Table DataAccessGroup'

CREATE TABLE dbo.DataAccessGroup
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	AuthorityGroupOID uniqueidentifier NOT NULL,
	Deleted bit NOT NULL
	)  ON STATIC
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	DF_DataAccessGroup_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	DF_DataAccessGroup_Deleted DEFAULT 0 FOR Deleted
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	PK_DataAccessGroup PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO
CREATE NONCLUSTERED INDEX IX_DataAccessGroup_AuthorityGroupOID ON dbo.DataAccessGroup
	(
	AuthorityGroupOID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.DataAccessGroup SET (LOCK_ESCALATION = TABLE)
GO

PRINT N'Adding Table StudyDataAccess'
GO
CREATE TABLE dbo.StudyDataAccess
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	StudyStorageGUID uniqueidentifier NOT NULL,
	DataAccessGroupGUID uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Table for granting access to studies via Authority Groups'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'StudyDataAccess', NULL, NULL
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	DF_StudyDataAccess_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	PK_StudyDataAccess PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_StudyDataAccess_DataAcessGroupGUID ON dbo.StudyDataAccess
	(
	DataAccessGroupGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
CREATE NONCLUSTERED INDEX IX_StudyDataAccess_StudyStorageGUID ON dbo.StudyDataAccess
	(
	StudyStorageGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	FK_StudyDataAccess_DataAccessGroup FOREIGN KEY
	(
	DataAccessGroupGUID
	) REFERENCES dbo.DataAccessGroup
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	FK_StudyDataAccess_StudyStorage FOREIGN KEY
	(
	StudyStorageGUID
	) REFERENCES dbo.StudyStorage
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
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
