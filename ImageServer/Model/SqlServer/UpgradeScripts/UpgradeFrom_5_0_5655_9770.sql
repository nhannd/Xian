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

PRINT N'Adding new columns to Study table'
GO

ALTER TABLE dbo.Study ADD
	ResponsiblePerson nvarchar(64) NULL,
	ResponsibleOrganization nvarchar(64) NULL,
	QueryXml xml NULL
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ServerPartitionDataAccess table'

/****** Object:  Table [dbo].[ServerPartitionDataAccess]    Script Date: 01/01/2012 23:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerPartitionDataAccess](
	[GUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ServerPartitionDataAccess_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[DataAccessGroupGUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ServerPartitionDataAccess] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_DataAccessGroupGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[DataAccessGroupGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_ServerPartitionGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[ServerPartitionGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_DataAccessGroup] FOREIGN KEY([DataAccessGroupGUID])
REFERENCES [dbo].[DataAccessGroup] ([GUID])
GO

ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating DeleteServerPartition stored procedure'

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

-- =============================================
-- Author:		Thanh Huynh
-- Create date: April 24, 2008
-- Update date: Jan 4, 2012
-- Description:	Completely delete a Server Partition from the database.
--				This involves deleting devies, rules, 
-- =============================================
ALTER PROCEDURE [dbo].[DeleteServerPartition] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier,
	@DeleteStudies bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @DeviceGUID uniqueidentifier
	Declare @StudyStorageGUID uniqueidentifier

	/* DELETE DEVICE AND RELATED TABLES */
	DECLARE DeviceCursor Cursor For Select GUID from dbo.Device where ServerPartitionGUID=@ServerPartitionGUID
	Open DeviceCursor
	Fetch NEXT FROM DeviceCursor INTO @DeviceGUID
	While (@@FETCH_STATUS <> -1)
	BEGIN
		-- PRINT 'Deleting DevicePreferredTransferSyntax'
		delete dbo.DevicePreferredTransferSyntax where DeviceGUID=@DeviceGUID
		--PRINT 'Deleting WorkQueueUid'
		delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where DeviceGUID=@DeviceGUID)
		--PRINT 'Deleting WorkQueue'
		delete dbo.WorkQueue where DeviceGUID=@DeviceGUID
		Fetch NEXT FROM DeviceCursor INTO @DeviceGUID
	END
	CLOSE DeviceCursor
	DEALLOCATE DeviceCursor	
	--PRINT 'Deleting Device'
	delete dbo.Device where ServerPartitionGUID=@ServerPartitionGUID

	/* DELETE STUDYSTORAGE AND RELATED TABLES  */
	DECLARE StudyStorageCursor Cursor For Select GUID from dbo.StudyStorage where ServerPartitionGUID=@ServerPartitionGUID
	Open StudyStorageCursor
	Fetch NEXT FROM StudyStorageCursor INTO @StudyStorageGUID
	While (@@FETCH_STATUS <> -1)
	BEGIN
		--PRINT 'Deleting FilesystemQueue'
		delete dbo.FilesystemQueue where StudyStorageGUID=@StudyStorageGUID
		--PRINT 'Deleting FilesystemStudyStorage'
		delete dbo.FilesystemStudyStorage where StudyStorageGUID=@StudyStorageGUID
		--PRINT 'Deleting WorkQueueUid'
		delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID)
		--PRINT 'Deleting WorkQueue'
		delete dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID
		delete dbo.StudyHistory where StudyStorageGUID=@StudyStorageGUID
		Fetch NEXT FROM StudyStorageCursor INTO @StudyStorageGUID
	END
	CLOSE StudyStorageCursor
	DEALLOCATE StudyStorageCursor	
	--PRINT 'Deleting StudyStorage'
	delete dbo.StudyStorage where ServerPartitionGUID=@ServerPartitionGUID
	
	/* DELETE WORKQUEUE AND RELATED TABLES */
	--PRINT 'Deleting WorkQueueUid'
	delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID)
	--PRINT 'Deleting WorkQueue'
	delete dbo.WorkQueue where ServerPartitionGUID=@ServerPartitionGUID
	--PRINT 'Deleting PartitionSopClass'
	delete dbo.PartitionSopClass where ServerPartitionGUID=@ServerPartitionGUID
	--PRINT 'Deleting PartitionTransferSyntax'
	delete dbo.PartitionTransferSyntax where ServerPartitionGUID=@ServerPartitionGUID

	--PRINT 'Deleting ServerRule'
	delete dbo.ServerRule where ServerPartitionGUID=@ServerPartitionGUID

	-- PRINT 'Deleting ServerPartitionDataAccess'
	delete dbo.ServerPartitionDataAccess where ServerPartitionGUID= @ServerPartitionGUID

	IF @DeleteStudies=1
	BEGIN
		/* DELETE STUDY, PATIENT AND RELATED TABLES */
		delete dbo.RequestAttributes where SeriesGUID in (Select GUID from dbo.Series where ServerPartitionGUID=@ServerPartitionGUID)
		delete dbo.Series where ServerPartitionGUID=@ServerPartitionGUID
		delete dbo.Study where ServerPartitionGUID=@ServerPartitionGUID
		delete dbo.Patient where ServerPartitionGUID=@ServerPartitionGUID		
	END

	--PRINT 'Deleting ServerPartition'
	delete dbo.ServerPartition where GUID=@ServerPartitionGUID
	
END



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
