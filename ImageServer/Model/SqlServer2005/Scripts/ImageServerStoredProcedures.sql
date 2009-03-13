USE [ImageServer]
GO
/****** Object:  StoredProcedure [dbo].[QueryFilesystemQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryFilesystemQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryFilesystemQueue]
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueFromFilesystemQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueFromFilesystemQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueueFromFilesystemQueue]
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystemQueue]    Script Date: 01/08/2008 16:04:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFilesystemQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFilesystemQueue]
GO
/****** Object:  StoredProcedure [dbo].[DeleteStudyStorage]    Script Date: 01/08/2008 16:04:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteStudyStorage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteStudyStorage]
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystem]    Script Date: 01/08/2008 16:04:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFilesystem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFilesystem]
GO
/****** Object:  StoredProcedure [dbo].[QueryServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServiceLock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryServiceLock]
GO
/****** Object:  StoredProcedure [dbo].[ResetServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResetServiceLock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ResetServiceLock]
GO
/****** Object:  StoredProcedure [dbo].[UpdateServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateServiceLock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateServiceLock]
GO
/****** Object:  StoredProcedure [dbo].[InsertServerPartition]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertServerPartition]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertServerPartition]
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[ResetWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResetWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ResetWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionSopClasses]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionSopClasses]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryServerPartitionSopClasses]
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionTransferSyntaxes]    Script Date: 06/24/2008 12:29:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionTransferSyntaxes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryServerPartitionTransferSyntaxes]
GO
/****** Object:  StoredProcedure [dbo].[QueryModalitiesInStudy]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryModalitiesInStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryModalitiesInStudy]
GO
/****** Object:  StoredProcedure [dbo].[InsertInstance]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInstance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertInstance]
GO
/****** Object:  StoredProcedure [dbo].[InsertRequestAttributes]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRequestAttributes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRequestAttributes]
GO
/****** Object:  StoredProcedure [dbo].[InsertStudyStorage]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertStudyStorage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertStudyStorage]
GO
/****** Object:  StoredProcedure [dbo].[WebQueryWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[WebQueryWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryStudyStorageLocation]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryStudyStorageLocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryStudyStorageLocation]
GO
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueue]    Script Date: 04/26/2008 00:28:22 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[DeleteServerPartition]    Script Date: 04/26/2008 00:28:22 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteServerPartition]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteServerPartition]
GO
/****** Object:  StoredProcedure [dbo].[InsertArchiveQueue]    Script Date: 07/11/2008 13:04:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertArchiveQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertArchiveQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryArchiveQueue]    Script Date: 07/14/2008 10:43:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryArchiveQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryArchiveQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryRestoreQueue]    Script Date: 07/14/2008 10:43:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRestoreQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryRestoreQueue]
GO
/****** Object:  StoredProcedure [dbo].[UpdateArchiveQueue]    Script Date: 07/14/2008 10:43:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateArchiveQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateArchiveQueue]
GO
/****** Object:  StoredProcedure [dbo].[UpdateRestoreQueue]    Script Date: 07/14/2008 10:43:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateRestoreQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateRestoreQueue]
GO
/****** Object:  StoredProcedure [dbo].[DeleteFilesystemStudyStorage]    Script Date: 07/16/2008 15:46:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteFilesystemStudyStorage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteFilesystemStudyStorage]
GO
/****** Object:  StoredProcedure [dbo].[InsertRestoreQueue]    Script Date: 07/21/2008 16:11:22 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRestoreQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRestoreQueue]
GO
/****** Object:  StoredProcedure [dbo].[WebQueryArchiveQueue]    Script Date: 08/05/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryArchiveQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[WebQueryArchiveQueue]
GO
/****** Object:  StoredProcedure [dbo].[WebQueryRestoreQueue]    Script Date: 08/21/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryRestoreQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[WebQueryRestoreQueue]
GO

/****** Object:  StoredProcedure [dbo].[InsertStudyIntegrityQueue]    Script Date: 09/17/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertStudyIntegrityQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertStudyIntegrityQueue]
GO

/****** Object:  StoredProcedure [dbo].[AttachStudyToPatient]    Script Date: 10/09/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttachStudyToPatient]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AttachStudyToPatient]
GO

/****** Object:  StoredProcedure [dbo].[CreatePatientForStudy]    Script Date: 10/09/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreatePatientForStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreatePatientForStudy]
GO

/****** Object:  StoredProcedure [dbo].[LockStudy]    Script Date: 10/15/2008 17:35:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LockStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[LockStudy]
GO


/****** Object:  StoredProcedure [dbo].[LockStudy]    Script Date: 10/15/2008 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LockStudy]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: Oct 15, 2008
-- Description:	Lock/Unlock a study
-- =============================================
CREATE PROCEDURE [dbo].[LockStudy] 
	@StudyStorageGUID uniqueidentifier,
	@Lock bit = null,
	@QueueStudyStateEnum smallint = null,
	@Successful bit output,
	@FailureReason nvarchar(1024)=null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @Successful= 1

	if @QueueStudyStateEnum is not null
	BEGIN
		declare @IdleQueueStudyStateEnum smallint
		SELECT @IdleQueueStudyStateEnum = Enum from QueueStudyStateEnum where Lookup = ''Idle''

		if @QueueStudyStateEnum = @IdleQueueStudyStateEnum
		BEGIN
			UPDATE StudyStorage
			SET	QueueStudyStateEnum = @IdleQueueStudyStateEnum,
				LastAccessedTime = getdate()
			WHERE GUID = @StudyStorageGUID

			IF (@@ROWCOUNT=0)
				SET @Successful=0
		END
		ELSE
		BEGIN
			UPDATE StudyStorage
			SET	QueueStudyStateEnum = @QueueStudyStateEnum,
				LastAccessedTime = getdate()
			WHERE GUID = @StudyStorageGUID AND QueueStudyStateEnum = @IdleQueueStudyStateEnum

			IF (@@ROWCOUNT=0)
				SET @Successful=0
		END
	END
	ELSE if @Lock is not null
	BEGIN
		IF @Lock=1
		BEGIN
			UPDATE StudyStorage 
			SET Lock=1, LastAccessedTime = getdate()
			WHERE GUID=@StudyStorageGUID AND Lock=0

			IF (@@ROWCOUNT=0)
				SET @Successful=0
			
		END
		ELSE
		BEGIN
			-- unlock if the study is being locked
			UPDATE StudyStorage 
			SET Lock=0, LastAccessedTime = getdate()
			WHERE GUID=@StudyStorageGUID AND Lock=1

			IF (@@ROWCOUNT=0)
				SET @Successful=0
		END
	END
	ELSE
	BEGIN
		SET @Successful=0
	END

	IF @Successful=0
	BEGIN
		SELECT @FailureReason=QueueStudyStateEnum.LongDescription 
		FROM QueueStudyStateEnum 
		JOIN StudyStorage ON StudyStorage.QueueStudyStateEnum = QueueStudyStateEnum.Enum
		WHERE StudyStorage.GUID = @StudyStorageGUID
	END
END
'
END
GO


/****** Object:  StoredProcedure [dbo].[WebQueryWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: December 16, 2007
-- Description:	Query WorkQueue entries based on criteria
--				
-- =============================================
CREATE PROCEDURE [dbo].[WebQueryWorkQueue] 
	@ServerPartitionGUID uniqueidentifier = null,
	@PatientID nvarchar(64) = null,
	@PatientsName nvarchar(64) = null,
	@ScheduledTime datetime = null,
	@Type nvarchar(64) = null,
	@Status nvarchar(64) = null,
	@Priority smallint = null,
	@StartIndex int,
	@MaxRowCount int = 25,
	@ResultCount int OUTPUT
AS
BEGIN
	Declare @stmt nvarchar(1024);
	Declare @where nvarchar(1024);
	Declare @count nvarchar(1024);

	-- Build SELECT statement based on the paramters
	
	SET @stmt =			''SELECT WorkQueue.*, ROW_NUMBER() OVER(ORDER BY ScheduledTime ASC) as RowNum FROM WorkQueue ''
	SET @stmt = @stmt + ''LEFT JOIN StudyStorage on StudyStorage.GUID = WorkQueue.StudyStorageGUID ''
	SET @stmt = @stmt + ''LEFT JOIN Study on Study.ServerPartitionGUID=StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid=StudyStorage.StudyInstanceUid ''
	
	SET @where = ''''

	IF (@ServerPartitionGUID IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''WorkQueue.ServerPartitionGUID = '''''' +  CONVERT(varchar(250),@ServerPartitionGUID) +''''''''
	END



	IF (@Type IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''
		
		SET @where = @where + ''WorkQueue.WorkQueueTypeEnum in '' + @Type
	END
	
	IF (@Status IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''WorkQueue.WorkQueueStatusEnum in '' +  @Status
	END

	IF (@Priority IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''WorkQueue.WorkQueuePriorityEnum = '' +  CONVERT(varchar(10),@Priority)
	END

	IF (@ScheduledTime IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''WorkQueue.ScheduledTime between '''''' +  CONVERT(varchar(30), @ScheduledTime, 101 ) +'''''' and '''''' + CONVERT(varchar(30), DATEADD(DAY, 1, @ScheduledTime), 101 ) + ''''''''
	END


	IF (@PatientID IS NOT NULL and @PatientID<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientID Like ''''%'' + @PatientID + ''%'''' ''
	END

	IF (@PatientsName IS NOT NULL and @PatientsName<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientsName Like ''''%'' + @PatientsName + ''%'''' ''
	END

	if (@where<>'''')
		SET @stmt = @stmt + '' WHERE '' + @where

	--PRINT @stmt
	SET @stmt = ''SELECT W.GUID, W.ServerPartitionGUID, W.StudyStorageGUID, W.DeviceGUID, W.WorkQueueTypeEnum, W.WorkQueueStatusEnum, W.WorkQueuePriorityEnum, W.ProcessorID, W.ExpirationTime, W.ScheduledTime, W.InsertTime, W.FailureCount, W.FailureDescription, W.Data FROM ('' + @stmt
	SET @stmt = @stmt + '') AS W WHERE W.RowNum BETWEEN '' + str(@StartIndex) + '' AND ('' + str(@StartIndex) + '' + '' + str(@MaxRowCount) + '') - 1''

	EXEC(@stmt)

	SET @count = ''SELECT @recordCount = count(*) FROM WorkQueue ''
	if (@where<>'''')
	BEGIN
		SET @count = @count + ''LEFT JOIN StudyStorage on StudyStorage.GUID = WorkQueue.StudyStorageGUID ''
		SET @count = @count + ''LEFT JOIN Study on Study.ServerPartitionGUID=StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid=StudyStorage.StudyInstanceUid ''	
		SET @count = @count + ''WHERE '' + @where
	END

	DECLARE @recCount int
	
	EXEC sp_executesql  @count, N''@recordCount int OUT'', @recCount OUT

	set @ResultCount = @recCount

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryStudyStorageLocation]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryStudyStorageLocation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: 7/30/2007
-- Description:	
-- History:
--		10/24/2008	:	Added IsReconcileRequired property into the result set.
--		07/04/2008	:	Modify to return storage location based on the study instance uid 
--						when StudyStorageGUID and ServerPartitionGUID aren''t provided. Used for image streaming service.
-- =============================================
CREATE PROCEDURE [dbo].[QueryStudyStorageLocation] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier = null,
	@ServerPartitionGUID uniqueidentifier = null, 
	@StudyInstanceUid varchar(64) = null 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @IsReconcileRequired bit
	SET @IsReconcileRequired =0;

	IF @StudyStorageGUID is null and @ServerPartitionGUID is null
	BEGIN
		SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.InsertTime, StudyStorage.StudyStatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, FilesystemStudyStorage.StudyFolder, FilesystemStudyStorage.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly,
				Filesystem.FilesystemTierEnum, StudyStorage.Lock, FilesystemStudyStorage.ServerTransferSyntaxGUID, ServerTransferSyntax.Uid as TransferSyntaxUid, FilesystemStudyStorage.GUID as FilesystemStudyStorageGUID,
				StudyStorage.QueueStudyStateEnum, @IsReconcileRequired as ''IsReconcileRequired''
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN FilesystemStudyStorage on StudyStorage.GUID = FilesystemStudyStorage.StudyStorageGUID
			JOIN Filesystem on FilesystemStudyStorage.FilesystemGUID = Filesystem.GUID
			JOIN ServerTransferSyntax on ServerTransferSyntax.GUID = FilesystemStudyStorage.ServerTransferSyntaxGUID
		WHERE StudyStorage.StudyInstanceUid = @StudyInstanceUid
	END
	ELSE IF @StudyStorageGUID is null
	BEGIN
		IF EXISTS(SELECT GUID FROM StudyIntegrityQueue WITH(NOLOCK) 
		WHERE StudyStorageGUID=@StudyStorageGUID)
			SET @IsReconcileRequired = 1;
	
	    SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.InsertTime, StudyStorage.StudyStatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, FilesystemStudyStorage.StudyFolder, FilesystemStudyStorage.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly,
				Filesystem.FilesystemTierEnum, StudyStorage.Lock, FilesystemStudyStorage.ServerTransferSyntaxGUID, ServerTransferSyntax.Uid as TransferSyntaxUid, FilesystemStudyStorage.GUID as FilesystemStudyStorageGUID,
				StudyStorage.QueueStudyStateEnum, @IsReconcileRequired  as ''IsReconcileRequired''
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN FilesystemStudyStorage on StudyStorage.GUID = FilesystemStudyStorage.StudyStorageGUID
			JOIN Filesystem on FilesystemStudyStorage.FilesystemGUID = Filesystem.GUID
			JOIN ServerTransferSyntax on ServerTransferSyntax.GUID = FilesystemStudyStorage.ServerTransferSyntaxGUID
		WHERE StudyStorage.ServerPartitionGUID = @ServerPartitionGUID and StudyStorage.StudyInstanceUid = @StudyInstanceUid
	END
	ELSE
	BEGIN
		SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.InsertTime, StudyStorage.StudyStatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, FilesystemStudyStorage.StudyFolder, FilesystemStudyStorage.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly,
				Filesystem.FilesystemTierEnum, StudyStorage.Lock, FilesystemStudyStorage.ServerTransferSyntaxGUID, ServerTransferSyntax.Uid as TransferSyntaxUid, FilesystemStudyStorage.GUID as FilesystemStudyStorageGUID,
				StudyStorage.QueueStudyStateEnum, @IsReconcileRequired  as ''IsReconcileRequired''
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN FilesystemStudyStorage on StudyStorage.GUID = FilesystemStudyStorage.StudyStorageGUID
			JOIN Filesystem on FilesystemStudyStorage.FilesystemGUID = Filesystem.GUID
			JOIN ServerTransferSyntax on ServerTransferSyntax.GUID = FilesystemStudyStorage.ServerTransferSyntaxGUID
		WHERE StudyStorage.GUID = @StudyStorageGUID
	END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertServerPartition]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertServerPartition]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 13, 2007
-- Modify date: Dev 17, 2008
-- Description:	Insert a ServerPartition row
-- =============================================
CREATE PROCEDURE [dbo].[InsertServerPartition] 
	-- Add the parameters for the stored procedure here
	@Enabled bit, 
	@Description nvarchar(128),
	@AeTitle varchar(16),
	@Port int,
	@PartitionFolder nvarchar(16),
	@DuplicateSopPolicyEnum smallint,
	@AcceptAnyDevice bit = 1,
	@AutoInsertDevice bit = 1,
	@DefaultRemotePort int = 104,
    @MatchPatientsName bit = 1,
    @MatchPatientId bit = 1,
    @MatchAccessionNumber bit = 1,
    @MatchPatientsBirthDate bit = 1,
    @MatchIssuerOfPatientId bit = 1,
    @MatchPatientsSex bit = 1,
	@AuditDeleteStudy bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SopClassGUID uniqueidentifier
	DECLARE @TransferSyntaxGUID uniqueidentifier
	DECLARE @ServerPartitionGUID uniqueidentifier

	SET @ServerPartitionGUID = newid()

    -- Insert statements for procedure here

	-- Wrap in a transaction
	BEGIN TRANSACTION

	INSERT INTO [ImageServer].[dbo].[ServerPartition] 
			([GUID],[Enabled],[Description],[AeTitle],[Port],[PartitionFolder],[AcceptAnyDevice],[AutoInsertDevice],[DefaultRemotePort],[DuplicateSopPolicyEnum],
			[MatchPatientsName], [MatchPatientId], [MatchAccessionNumber], [MatchPatientsBirthDate], [MatchIssuerOfPatientId], [MatchPatientsSex], [AuditDeleteStudy])
	VALUES (@ServerPartitionGUID, @Enabled, @Description, @AeTitle, @Port, @PartitionFolder, @AcceptAnyDevice, @AutoInsertDevice, @DefaultRemotePort, @DuplicateSopPolicyEnum,
			@MatchPatientsName, @MatchPatientId, @MatchAccessionNumber, @MatchPatientsBirthDate, @MatchIssuerOfPatientId, @MatchPatientsSex, @AuditDeleteStudy)

	-- Populate PartitionSopClass
	DECLARE cur_sopclass CURSOR FOR 
		SELECT GUID FROM ServerSopClass;

	OPEN cur_sopclass;

	FETCH NEXT FROM cur_sopclass INTO @SopClassGUID;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO [ImageServer].[dbo].[PartitionSopClass]
			([GUID],[ServerPartitionGUID],[ServerSopClassGUID],[Enabled])
		VALUES (newid(), @ServerPartitionGUID, @SopClassGUID, 1)

		FETCH NEXT FROM cur_sopclass INTO @SopClassGUID;	
	END 

	CLOSE cur_sopclass;
	DEALLOCATE cur_sopclass;

	-- Populate PartitionTransferSyntax
	DECLARE cur_transfersyntax CURSOR FOR 
		SELECT GUID FROM ServerTransferSyntax;

	OPEN cur_transfersyntax;

	FETCH NEXT FROM cur_transfersyntax INTO @TransferSyntaxGUID;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO [ImageServer].[dbo].[PartitionTransferSyntax]
			([GUID],[ServerPartitionGUID],[ServerTransferSyntaxGUID],[Enabled])
		VALUES (newid(), @ServerPartitionGUID, @TransferSyntaxGUID, 1)

		FETCH NEXT FROM cur_transfersyntax INTO @TransferSyntaxGUID;	
	END 

	CLOSE cur_transfersyntax;
	DEALLOCATE cur_transfersyntax;


	-- Now, put in default rules for the partition
	DECLARE  @StudyServerRuleApplyTimeEnum smallint
	DECLARE  @StudyArchiveServerRuleApplyTimeEnum smallint
	DECLARE  @StudyDeleteServerRuleTypeEnum smallint
	DECLARE  @Tier1RetentionServerRuleTypeEnum smallint
	DECLARE  @OnlineRetentionServerRuleTypeEnum smallint
	DECLARE  @StudyRestoreServerRuleApplyTimeEnum smallint
	DECLARE  @StudyCompressServerRuleTypeEnum smallint

	-- Get the Study Processed Rule Apply Time
	SELECT @StudyServerRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = ''StudyProcessed''
	SELECT @StudyArchiveServerRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = ''StudyArchived''
	SELECT @StudyRestoreServerRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = ''StudyRestored''

	-- Get all 3 types of Retention Rules
	SELECT @StudyDeleteServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''StudyDelete''
	SELECT @Tier1RetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''Tier1Retention''
	SELECT @OnlineRetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''OnlineRetention''
	SELECT @StudyCompressServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''StudyCompress''

	-- Insert a default StudyDelete rule
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Delete'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @StudyDeleteServerRuleTypeEnum, 1, 1,
				''<rule id="Default Delete">
					<condition>
					</condition>
					<action><study-delete time="10" unit="days"/></action>
				</rule>'' )

	-- Insert a default Tier1Retention rule
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Tier1 Retention'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @Tier1RetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Tier1 Retention">
					<condition>
					</condition>
					<action><tier1-retention time="3" unit="weeks"/></action>
				</rule>'' )

	-- Insert a default Tier1Retention rule for restores
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Restore Tier1 Retention'',@ServerPartitionGUID, @StudyRestoreServerRuleApplyTimeEnum, @Tier1RetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Tier1 Retention">
					<condition>
					</condition>
					<action><tier1-retention time="1" unit="weeks"/></action>
				</rule>'' )

	-- Insert a default Online Retention Rule for study processed
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Online Retention'',@ServerPartitionGUID, @StudyArchiveServerRuleApplyTimeEnum, @OnlineRetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Online Retention">
					<condition>
					</condition>
					<action><online-retention time="4" unit="weeks"/></action>
				</rule>'' )

	-- Insert a default Online Retention Rule for restores
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Restore Online Retention'',@ServerPartitionGUID, @StudyRestoreServerRuleApplyTimeEnum, @OnlineRetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Restore Online Retention">
					<condition>
					</condition>
					<action><online-retention time="1" unit="weeks"/></action>
				</rule>'' )

	-- Insert an exempt rule for Compression
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[ExemptRule],[RuleXml])
		 VALUES
			   (newid(),''Compression Exempt Rule'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @StudyCompressServerRuleTypeEnum, 1, 0, 1,
				''<rule>
				  <condition expressionLanguage="dicom">
					<or>
					  <!-- RLE -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.5" />
					  <!-- JPEG Baseline -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.4.50" />
					  <!-- JPEG Extended -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.4.51" />
					  <!-- JPEG Lossless -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.4.70" />
					  <!-- JPEG 2000 Lossless -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.4.90" />
					  <!-- JPEG 2000 Lossless/Lossy -->
					  <equal test="$TransferSyntaxUid" refValue="1.2.840.10008.1.2.4.91" />
					</or>
				  </condition>
				  <action>
					<no-op />
				  </action>
				</rule>'' )
	COMMIT TRANSACTION

	SELECT * from ServerPartition

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueFromFilesystemQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueFromFilesystemQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 17, 2008
-- Description:	Stored procedure for inserting WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueueFromFilesystemQueue] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier, 
	@ServerPartitionGUID uniqueidentifier,
	@ExpirationTime datetime,
	@ScheduledTime datetime,
	@DeleteFilesystemQueue bit, 
	@WorkQueueTypeEnum smallint,
	@FilesystemQueueTypeEnum smallint,
	@Data xml = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @WorkQueueGUID as uniqueidentifier

	declare @PendingStatusEnum as smallint
	declare @IdleStatusEnum as smallint

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @IdleStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Idle''

	BEGIN TRANSACTION

    -- Insert statements for procedure here
	SELECT @WorkQueueGUID = GUID from WorkQueue 
		where StudyStorageGUID = @StudyStorageGUID
		AND WorkQueueTypeEnum = @WorkQueueTypeEnum
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime, Data)
			values  (@WorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @WorkQueueTypeEnum, @PendingStatusEnum, @ExpirationTime, @ScheduledTime, @Data)
		IF @DeleteFilesystemQueue = 1
		BEGIN
			DELETE FROM FilesystemQueue
			WHERE StudyStorageGUID = @StudyStorageGUID AND FilesystemQueueTypeEnum = @FilesystemQueueTypeEnum
		END
	END
	ELSE
	BEGIN
		UPDATE WorkQueue 
			set ExpirationTime = @ExpirationTime
			where GUID = @WorkQueueGUID
	END

	SELECT * FROM WorkQueue WHERE GUID = @WorkQueueGUID

	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkQueue]    Script Date: 04/26/2008 00:28:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 20, 2007
-- Description:	Procedure for updating WorkQueue entries
-- History
--	Oct 29, 2007: Add @ProcessorID
--  May 14, 2008, Changed order so StudyLocks are released after updates
--  Oct 01, 2008, Added UpdateQueueStudyState
--  Oct 23, 2008, Removed UpdateQueueStudyState
-- =============================================
CREATE PROCEDURE [dbo].[UpdateWorkQueue] 
	-- Add the parameters for the stored procedure here
	@ProcessorID varchar(256),
	@WorkQueueGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@WorkQueueStatusEnum smallint,
	@FailureCount int,
	@ExpirationTime datetime = null,
	@ScheduledTime datetime = null,
	@FailureDescription nvarchar(256) = null,
	@QueueStudyStateEnum smallint = null
AS
BEGIN

	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.[UpdateWorkQueue]] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ServerPartitionGUID uniqueidentifier
	declare @StudyInstanceUid varchar(64)
	declare @CompletedStatusEnum as smallint
	declare @PendingStatusEnum as smallint
	declare @FailedStatusEnum as smallint
	declare @IdleStatusEnum as smallint

	select @CompletedStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Completed''
	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @FailedStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Failed''
	select @IdleStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Idle''
	
	BEGIN TRANSACTION

	if @WorkQueueStatusEnum = @CompletedStatusEnum 
	BEGIN
		SELECT @ServerPartitionGUID=ServerPartitionGUID, @StudyInstanceUid=StudyInstanceUid
		FROM StudyStorage WHERE GUID = @StudyStorageGUID 

		-- Completed
		DELETE FROM WorkQueue where GUID = @WorkQueueGUID

		if @QueueStudyStateEnum is not NULL
		BEGIN
			UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate(), QueueStudyStateEnum=@QueueStudyStateEnum  
			WHERE GUID = @StudyStorageGUID AND Lock = 1
		END
		ELSE
		BEGIN		
			UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
			WHERE GUID = @StudyStorageGUID AND Lock = 1
		END
	END
	ELSE if  @WorkQueueStatusEnum = @FailedStatusEnum
	BEGIN
		-- Failed
		IF @FailureDescription is NULL
		BEGIN
			UPDATE WorkQueue
			SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
				FailureCount = @FailureCount,
				ProcessorID = @ProcessorID
			WHERE GUID = @WorkQueueGUID
		END
		ELSE
		BEGIN
			UPDATE WorkQueue
			SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
				FailureCount = @FailureCount,
				ProcessorID = @ProcessorID,
				FailureDescription = @FailureDescription
			WHERE GUID = @WorkQueueGUID
		END
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	ELSE if @WorkQueueStatusEnum = @PendingStatusEnum
	BEGIN
		-- Pending
		IF @FailureDescription is NULL
		BEGIN
			UPDATE WorkQueue
			SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
				FailureCount = @FailureCount, ProcessorID = @ProcessorID
			WHERE GUID = @WorkQueueGUID
		END
		ELSE
		BEGIN
			UPDATE WorkQueue
			SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
				FailureCount = @FailureCount, ProcessorID = @ProcessorID, FailureDescription = @FailureDescription
			WHERE GUID = @WorkQueueGUID
		END
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	ELSE
	BEGIN
		-- Idle
		UPDATE WorkQueue
		SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
			FailureCount = @FailureCount, ProcessorID = @ProcessorID
		WHERE GUID = @WorkQueueGUID
		
		if @WorkQueueStatusEnum = @IdleStatusEnum
		BEGIN
			UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
			WHERE GUID = @StudyStorageGUID AND Lock = 1
		END
	END

	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: October 30, 2007
-- Description:	Stored procedure for inserting General WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueue] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier, 
	@ServerPartitionGUID uniqueidentifier,
	@WorkQueueTypeEnum smallint,
	@WorkQueuePriorityEnum smallint,
	@ScheduledTime datetime, 
	@ExpirationTime datetime,
	@DeviceGUID uniqueidentifier = null,
	@StudyHistoryGUID uniqueidentifier = null,
	@Data xml = null,
	@SeriesInstanceUid varchar(64) = null,
	@SopInstanceUid varchar(64) = null,
	@Duplicate bit = 0,
	@Extension varchar(10) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @WorkQueueGUID as uniqueidentifier

	declare @PendingStatusEnum as int
	declare @IdleStatusEnum as int
	declare @AutoRouteTypeEnum as int

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @IdleStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Idle''

	BEGIN TRANSACTION

    -- Insert statements for procedure here
	IF @DeviceGUID is not null
	BEGIN
		SELECT @WorkQueueGUID = GUID from WorkQueue 
			where StudyStorageGUID = @StudyStorageGUID
			AND WorkQueueTypeEnum = @WorkQueueTypeEnum
			AND DeviceGUID = @DeviceGUID
	END
	ELSE IF @StudyHistoryGUID is not null
	BEGIN
		SELECT @WorkQueueGUID = GUID from WorkQueue 
			where StudyStorageGUID = @StudyStorageGUID
			AND WorkQueueTypeEnum = @WorkQueueTypeEnum
			AND StudyHistoryGUID = @StudyHistoryGUID
	END
	ELSE
	BEGIN
		SELECT @WorkQueueGUID = GUID from WorkQueue 
			where StudyStorageGUID = @StudyStorageGUID
			AND WorkQueueTypeEnum = @WorkQueueTypeEnum
	END
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, DeviceGUID, StudyHistoryGUID, Data, WorkQueueTypeEnum, WorkQueueStatusEnum, WorkQueuePriorityEnum, ExpirationTime, ScheduledTime)
			values  (@WorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @DeviceGUID, @StudyHistoryGUID, @Data, @WorkQueueTypeEnum, @PendingStatusEnum, @WorkQueuePriorityEnum, @ExpirationTime, @ScheduledTime)
	END
	ELSE
	BEGIN
		UPDATE WorkQueue 
			set ExpirationTime = @ExpirationTime,
			ScheduledTime = @ScheduledTime
		WHERE GUID = @WorkQueueGUID
	END

	if @SeriesInstanceUid is not null or @SopInstanceUid is not null
	BEGIN
		INSERT into WorkQueueUid(GUID, WorkQueueGUID, SeriesInstanceUid, SopInstanceUid, Duplicate, Extension)
			values	(newid(), @WorkQueueGUID, @SeriesInstanceUid, @SopInstanceUid, @Duplicate, @Extension)
	END

	COMMIT TRANSACTION

	SELECT * from WorkQueue where GUID = @WorkQueueGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ResetWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResetWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- =============================================
-- Author:		Thanh Huynh
-- Create date: Oct 29, 2007
-- Description:	Cleanup work queue. 
--				Reset all "in progress" items to "Pending" or "Failed" depending on their retry counts
--
-- =============================================
CREATE PROCEDURE [dbo].[ResetWorkQueue]
	@ProcessorID varchar(256),
	@MaxFailureCount int,
	@RescheduleTime datetime,
	@FailedExpirationTime datetime,
	@RetryExpirationTime datetime
	
AS
BEGIN
	
	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.ResetWorkQueueItems] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION

		declare @PendingStatusEnum as int
		declare @InProgressStatusEnum as int
		declare @FailedStatusEnum as int
		declare @WorkQueueGUID uniqueidentifier
		

		select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
		select @InProgressStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''In Progress''
		select @FailedStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Failed''


		/* All entries that are in progress and failure count = MaxFailureCount should be failed */

		/* Temporary tables to hold all items that will be reset */
		CREATE TABLE #FailedList(WorkQueueGuid uniqueidentifier, StudyStorageGUID uniqueidentifier)
		CREATE TABLE #RetryList(WorkQueueGuid uniqueidentifier, StudyStorageGUID uniqueidentifier)
		
		/* fill the tables */
		INSERT INTO #FailedList (WorkQueueGuid, StudyStorageGUID)
		SELECT dbo.WorkQueue.GUID, dbo.StudyStorage.GUID
		FROM dbo.WorkQueue 
		LEFT JOIN	dbo.StudyStorage ON dbo.WorkQueue.StudyStorageGUID=dbo.StudyStorage.GUID
		WHERE ProcessorID=@ProcessorID 
				AND WorkQueue.WorkQueueStatusEnum=@InProgressStatusEnum 
				AND WorkQueue.FailureCount+1 >= @MaxFailureCount 


		INSERT INTO #RetryList (WorkQueueGuid, StudyStorageGUID)
		SELECT dbo.WorkQueue.GUID, dbo.StudyStorage.GUID
		FROM dbo.WorkQueue 
		LEFT JOIN	dbo.StudyStorage ON dbo.WorkQueue.StudyStorageGUID=dbo.StudyStorage.GUID
		WHERE ProcessorID=@ProcessorID 
				AND WorkQueue.WorkQueueStatusEnum=@InProgressStatusEnum 
				AND WorkQueue.FailureCount+1 < @MaxFailureCount

		/* unlock all studies in the "failed" list */
		/* and then fail those entries */
		UPDATE dbo.StudyStorage
		SET Lock = 0
		WHERE GUID IN (SELECT StudyStorageGUID FROM #FailedList)
		
		UPDATE dbo.WorkQueue
		SET WorkQueueStatusEnum = @FailedStatusEnum,	/* Status=FAILED */
			FailureCount = FailureCount+1,
			ExpirationTime = @FailedExpirationTime
		WHERE	GUID IN (SELECT WorkQueueGuid FROM #FailedList)

		/* unlock all studies in the "retry" list */
		/* and then reschedule those entries */
		UPDATE dbo.StudyStorage
		SET Lock = 0
		WHERE GUID IN (SELECT StudyStorageGUID FROM #RetryList)
			
		UPDATE dbo.WorkQueue 
		SET WorkQueueStatusEnum = @PendingStatusEnum,	/* Status=PENDING */
			ProcessorID=NULL,					/* may be picked up by another processor */
			FailureCount = FailureCount+1,		/* has failed once. This is needed to prevent endless reset later on*/
			ScheduledTime = @RescheduleTime,
			ExpirationTime = @RetryExpirationTime
		WHERE	GUID IN (SELECT WorkQueueGuid FROM #RetryList)


	COMMIT TRANSACTION

	/* Return the list of modified entries */
	SELECT * 
	FROM WorkQueue
	WHERE ( GUID IN (SELECT WorkQueueGuid FROM #RetryList) OR 
			GUID IN (SELECT WorkQueueGuid FROM #FailedList))


	DROP TABLE #RetryList
	DROP TABLE #FailedList

END

' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 16, 2007
-- Update date: October 8, 2008
-- Description:	Select WorkQueue entries
-- History:
--		Oct 29, 2007:	Add @ProcessorID
--		Jan 9, 2008:	Fixed clustering bug
--      Sep 4, 2008:    Added @WorkQueueStatusEnumList parameter
--      Oct 8, 2008:    Added @WorkQueuePriorityEnum parameter
-- =============================================
CREATE PROCEDURE [dbo].[QueryWorkQueue] 
	@ProcessorID varchar(256),
	@WorkQueueTypeEnumList varchar(300) = null,
	@WorkQueuePriorityEnum smallint = null

AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.QueryWorkQueue] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end


	SET NOCOUNT ON;

	-- Added READPAST locking hint to this procedure.  This should cause the query
    -- to just skip rows that are locked, going forward to any other row that 
    -- satisfies the query.  This mode is specifically recommended for work queue type tables.

	declare @StudyStorageGUID uniqueidentifier
	declare @WorkQueueGUID uniqueidentifier
	declare @PendingStatusEnum as int
	declare @IdleStatusEnum as int
	declare @InProgressStatusEnum as int

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @IdleStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Idle''
	select @InProgressStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''In Progress''
	
    IF @WorkQueueTypeEnumList is null
	BEGIN
		if @WorkQueuePriorityEnum is null
		BEGIN
			SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
				@WorkQueueGUID = WorkQueue.GUID 
			FROM WorkQueue WITH (READPAST)
			WHERE
				ScheduledTime < getdate() 
				AND EXISTS (SELECT GUID FROM StudyStorage WITH (READPAST) WHERE WorkQueue.StudyStorageGUID = StudyStorage.GUID AND StudyStorage.Lock = 0)
				AND (  WorkQueue.WorkQueueStatusEnum in (@PendingStatusEnum,@IdleStatusEnum)  )
			ORDER BY WorkQueue.ScheduledTime
		END
		ELSE
		BEGIN
			SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
				@WorkQueueGUID = WorkQueue.GUID 
			FROM WorkQueue WITH (READPAST)
			WHERE
				ScheduledTime < getdate() 
				AND EXISTS (SELECT GUID FROM StudyStorage WITH (READPAST) WHERE WorkQueue.StudyStorageGUID = StudyStorage.GUID AND StudyStorage.Lock = 0)
				AND (  WorkQueue.WorkQueueStatusEnum in (@PendingStatusEnum,@IdleStatusEnum)  )
				AND WorkQueuePriorityEnum = @WorkQueuePriorityEnum
			ORDER BY WorkQueue.ScheduledTime
		END
	END
	ELSE
	BEGIN
		-- Use a temp table, and join on it for retrieving multiple types
		DECLARE @TempList table
		(
			Enum smallint
		)

		DECLARE @Enum varchar(10), @Pos int

		SET @WorkQueueTypeEnumList = LTRIM(RTRIM(@WorkQueueTypeEnumList))+ '',''
		SET @Pos = CHARINDEX('','', @WorkQueueTypeEnumList, 1)

		IF REPLACE(@WorkQueueTypeEnumList, '','', '''') <> ''''
		BEGIN
			WHILE @Pos > 0
			BEGIN
				SET @Enum = LTRIM(RTRIM(LEFT(@WorkQueueTypeEnumList, @Pos - 1)))
				IF @Enum <> ''''
				BEGIN
					INSERT INTO @TempList (Enum) VALUES (CAST(@Enum AS smallint)) --Use Appropriate conversion
				END
				SET @WorkQueueTypeEnumList = RIGHT(@WorkQueueTypeEnumList, LEN(@WorkQueueTypeEnumList) - @Pos)
				SET @Pos = CHARINDEX('','', @WorkQueueTypeEnumList, 1)
			END
		END	

		if @WorkQueuePriorityEnum is null
		BEGIN
			SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
					@WorkQueueGUID = WorkQueue.GUID 
			FROM WorkQueue WITH (READPAST)
			JOIN
				StudyStorage ON StudyStorage.GUID = WorkQueue.StudyStorageGUID AND StudyStorage.Lock = 0
			WHERE
				ScheduledTime < getdate() 
				AND EXISTS (SELECT GUID FROM StudyStorage WITH (READPAST) WHERE WorkQueue.StudyStorageGUID = StudyStorage.GUID AND StudyStorage.Lock = 0)
				AND WorkQueue.WorkQueueStatusEnum in (@PendingStatusEnum,@IdleStatusEnum)
				AND WorkQueue.WorkQueueTypeEnum in (select Enum from @TempList)
			ORDER BY WorkQueue.ScheduledTime
		END
		ELSE
		BEGIN
			SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
					@WorkQueueGUID = WorkQueue.GUID 
			FROM WorkQueue WITH (READPAST)
			JOIN
				StudyStorage ON StudyStorage.GUID = WorkQueue.StudyStorageGUID AND StudyStorage.Lock = 0
			WHERE
				ScheduledTime < getdate() 
				AND EXISTS (SELECT GUID FROM StudyStorage WITH (READPAST) WHERE WorkQueue.StudyStorageGUID = StudyStorage.GUID AND StudyStorage.Lock = 0)
				AND WorkQueue.WorkQueueStatusEnum in (@PendingStatusEnum,@IdleStatusEnum)
				AND WorkQueue.WorkQueueTypeEnum in (select Enum from @TempList)
				AND WorkQueuePriorityEnum = @WorkQueuePriorityEnum
			ORDER BY WorkQueue.ScheduledTime
		END		
	END

	-- We have a record, now do the updates
	BEGIN TRANSACTION

	UPDATE StudyStorage
		SET Lock = 1, LastAccessedTime = getdate()
	WHERE 
		Lock = 0 
		AND GUID = @StudyStorageGUID

	if (@@ROWCOUNT = 1)
	BEGIN
		UPDATE WorkQueue
			SET WorkQueueStatusEnum  = @InProgressStatusEnum,
				ProcessorID = @ProcessorID
		WHERE 
			GUID = @WorkQueueGUID
			
		COMMIT TRANSACTION
	END
	ELSE
	BEGIN
		-- In case the lock failed, reset GUID
		SET @WorkQueueGUID = newid()
		
		ROLLBACK TRANSACTION
	END
	

	-- If the first update failed, this should select 0 records
	SELECT * 
	FROM WorkQueue
	WHERE WorkQueueStatusEnum = @InProgressStatusEnum
		AND GUID = @WorkQueueGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionSopClasses]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionSopClasses]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 13, 2007
-- Description:	Select all the SOP Classes for a Partition
-- =============================================
CREATE PROCEDURE [dbo].[QueryServerPartitionSopClasses] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	PartitionSopClass.GUID,
			PartitionSopClass.ServerPartitionGUID, 
			PartitionSopClass.ServerSopClassGUID,
			PartitionSopClass.Enabled,
			ServerSopClass.SopClassUid,
			ServerSopClass.Description,
			ServerSopClass.NonImage
	FROM PartitionSopClass
	JOIN ServerSopClass on PartitionSopClass.ServerSopClassGUID = ServerSopClass.GUID
	WHERE PartitionSopClass.ServerPartitionGUID = @ServerPartitionGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionTransferSyntaxes]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionTransferSyntaxes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 13, 2007
-- Description:	Select all the Transfer Syntaxes for a Partition
-- =============================================
CREATE PROCEDURE [dbo].[QueryServerPartitionTransferSyntaxes] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	PartitionTransferSyntax.GUID,
			PartitionTransferSyntax.ServerPartitionGUID, 
			PartitionTransferSyntax.ServerTransferSyntaxGUID,
			PartitionTransferSyntax.Enabled,
			ServerTransferSyntax.Uid,
			ServerTransferSyntax.Description,
			ServerTransferSyntax.Lossless
	FROM PartitionTransferSyntax
	JOIN ServerTransferSyntax on PartitionTransferSyntax.ServerTransferSyntaxGUID = ServerTransferSyntax.GUID
	WHERE PartitionTransferSyntax.ServerPartitionGUID = @ServerPartitionGUID
	ORDER BY ServerTransferSyntax.Lossless DESC
	
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystem]    Script Date: 01/08/2008 16:04:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFilesystem]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 17, 2007
-- Modification date: May 5, 2008
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertFilesystem] 
	-- Add the parameters for the stored procedure here
	@FilesystemTierEnum smallint, 
	@FilesystemPath nvarchar(256),
	@Enabled bit = 1,
	@ReadOnly bit = 0,
	@WriteOnly bit = 0,
	@Description nvarchar(128),
	@HighWatermark decimal(6,2) = 90.00,
	@LowWatermark decimal(6,2) = 80.00
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Variables
	DECLARE @GUID uniqueidentifier
	DECLARE @FilesystemDeleteServiceLockTypeEnum smallint
	DECLARE @FilesystemReinventoryServiceLockTypeEnum smallint
	DECLARE @FilesystemStudyProcessServiceLockTypeEnum smallint
	DECLARE @FilesystemLosslessCompressServiceLockTypeEnum smallint
	DECLARE @FilesystemLossyCompressServiceLockTypeEnum smallint
	DECLARE @FilesystemRebuildXmlServiceLockTypeEnum smallint

	SET @GUID = newid()
	SELECT @FilesystemDeleteServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemDelete''
	SELECT @FilesystemReinventoryServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemReinventory''
	SELECT @FilesystemStudyProcessServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemStudyProcess''
	SELECT @FilesystemLosslessCompressServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemLosslessCompress''
	SELECT @FilesystemLossyCompressServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemLossyCompress''
	SELECT @FilesystemRebuildXmlServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemRebuildXml''

    -- Insert statements
	BEGIN TRANSACTION

	INSERT INTO [ImageServer].[dbo].Filesystem 
		([GUID],[FilesystemTierEnum],[FilesystemPath],[Enabled],[ReadOnly],[WriteOnly],[Description], [HighWatermark], [LowWatermark])
	VALUES (@GUID, @FilesystemTierEnum, @FilesystemPath, @Enabled, @ReadOnly, @WriteOnly, @Description, @HighWatermark, @LowWatermark)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemDeleteServiceLockTypeEnum,0,getdate(),@GUID,1)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemReinventoryServiceLockTypeEnum,0,getdate(),@GUID,0)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemStudyProcessServiceLockTypeEnum,0,getdate(),@GUID,0)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemLosslessCompressServiceLockTypeEnum,0,getdate(),@GUID,1)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemLossyCompressServiceLockTypeEnum,0,getdate(),@GUID,1)

	INSERT INTO [ImageServer].[dbo].ServiceLock
		([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
	VALUES (newid(),@FilesystemRebuildXmlServiceLockTypeEnum,0,getdate(),@GUID,0)

	COMMIT TRANSACTION

	SELECT * FROM Filesystem where GUID = @GUID	
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServiceLock]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 14, 2007
-- Description:	Query for ServiceLock rows
-- =============================================
CREATE PROCEDURE [dbo].[QueryServiceLock] 
	-- Add the parameters for the stored procedure here
	@ProcessorId varchar(256), 
	@ServiceLockTypeEnum smallint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.QueryServiceLock] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end

   	-- Added READPAST locking hint to this procedure.  This should cause the query
    -- to just skip rows that are locked, going forward to any other row that 
    -- satisfies the query.  This mode is specifically recommended for work queue type tables.

	declare @ServiceLockGUID uniqueidentifier
	
    IF @ServiceLockTypeEnum = 0
	BEGIN
		SELECT TOP (1) @ServiceLockGUID = ServiceLock.GUID 
		FROM ServiceLock WITH (READPAST)
		WHERE
			Enabled = 1
			AND ScheduledTime < getdate() 
			AND ( ServiceLock.Lock = 0 )
		ORDER BY ServiceLock.ScheduledTime
	END
	ELSE
	BEGIN
		SELECT TOP (1) @ServiceLockGUID = ServiceLock.GUID 
		FROM ServiceLock WITH (READPAST)
		WHERE
			Enabled = 1
			AND ScheduledTime < getdate() 
			AND ServiceLock.ServiceLockTypeEnum = @ServiceLockTypeEnum
			AND ( ServiceLock.Lock = 0 )
		ORDER BY ServiceLock.ScheduledTime
	END

	-- We have a record, now do the updates
	IF @@ROWCOUNT != 0
	BEGIN
		UPDATE ServiceLock
			SET Lock = 1, ProcessorId = @ProcessorId
		WHERE 
			Lock = 0 
			AND GUID = @ServiceLockGUID

		if @@ROWCOUNT = 0
		BEGIN
			set @ServiceLockGUID = newid()
		END
	END
	ELSE
	BEGIN
		-- No valid records
		set @ServiceLockGUID = newid()
	END

	-- If the first update failed, this should select 0 records
	SELECT * 
	FROM ServiceLock
	WHERE 
		GUID = @ServiceLockGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ResetServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResetServiceLock]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 19, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ResetServiceLock] 
	-- Add the parameters for the stored procedure here
	@ProcessorId varchar(256), 
	@ServiceLockTypeEnum smallint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


    -- Insert statements for procedure here

	BEGIN TRANSACTION

	declare @ServiceLockGUID uniqueidentifier
	declare @Lock bit

	DECLARE cur_servicelock CURSOR FOR 
		SELECT GUID, Lock FROM ServiceLock WHERE ProcessorId = @ProcessorId;

	OPEN cur_servicelock;

	FETCH NEXT FROM cur_servicelock INTO @ServiceLockGUID, @Lock;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @Lock = 1
		BEGIN
			UPDATE ServiceLock SET Lock = 0, ScheduledTime = getdate()
			WHERE GUID = @ServiceLockGUID
		END
		ELSE
		BEGIN
			UPDATE ServiceLock SET ProcessorId = null
			WHERE GUID = @ServiceLockGUID
		END

		FETCH NEXT FROM cur_servicelock INTO @ServiceLockGUID, @Lock;	
	END 

	CLOSE cur_servicelock;
	DEALLOCATE cur_servicelock;

	COMMIT TRANSACTION

	SELECT * 
	FROM ServiceLock 
	WHERE ProcessorId = @ProcessorId

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateServiceLock]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateServiceLock]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 14, 2007
-- Description:	Update the ServiceLock table
-- =============================================
CREATE PROCEDURE [dbo].[UpdateServiceLock] 
	-- Add the parameters for the stored procedure here
	@ProcessorId varchar(256), 
	@ServiceLockGUID uniqueidentifier,
	@Lock bit,
	@ScheduledTime datetime,
	@Enabled bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		UPDATE ServiceLock
		SET Lock = @Lock, ScheduledTime = @ScheduledTime,
			ProcessorID = @ProcessorID, Enabled = @Enabled
		WHERE GUID = @ServiceLockGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystemQueue]    Script Date: 01/08/2008 16:04:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFilesystemQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 14, 2007
-- Modified date: May 21, 2008
-- Description:	Insert into FilesystemQueue
-- =============================================
CREATE PROCEDURE [dbo].[InsertFilesystemQueue] 
	-- Add the parameters for the stored procedure here
	@FilesystemQueueTypeEnum smallint, 
	@StudyStorageGUID uniqueidentifier,
	@FilesystemGUID uniqueidentifier,
	@ScheduledTime datetime,
	@SeriesInstanceUid varchar(64) = null,
	@QueueXml xml = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @FilesystemQueueGUID uniqueidentifier
	DECLARE @ScheduledTimeInDb datetime

	SELECT @FilesystemQueueGUID = GUID, @ScheduledTimeInDb = ScheduledTime
	FROM FilesystemQueue
	WHERE StudyStorageGUID = @StudyStorageGUID AND FilesystemQueueTypeEnum = @FilesystemQueueTypeEnum

	IF @@ROWCOUNT > 0
	BEGIN
		IF @ScheduledTime > @ScheduledTimeInDb
		BEGIN
			UPDATE FilesystemQueue
			SET ScheduledTime = @ScheduledTime
			WHERE GUID = @FilesystemQueueGUID
		END
	END
	ELSE
	BEGIN
	-- Insert statements	
		INSERT INTO [ImageServer].[dbo].[FilesystemQueue]
			   ([GUID],[FilesystemQueueTypeEnum],[StudyStorageGUID],[FilesystemGUID],[ScheduledTime],[SeriesInstanceUid],[QueueXml])
		 VALUES
			   (newid(), @FilesystemQueueTypeEnum, @StudyStorageGUID, @FilesystemGUID, @ScheduledTime, @SeriesInstanceUid, @QueueXml)		
	END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteStudyStorage]    Script Date: 01/08/2008 16:04:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteStudyStorage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 19, 2007
-- Update date: Oct 01, 2008
-- Description:	Completely delete a Study from the database
-- =============================================
CREATE PROCEDURE [dbo].[DeleteStudyStorage] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyInstanceUid varchar(64)
	declare @StudyGUID uniqueidentifier
	declare @PatientGUID uniqueidentifier
	declare @NumberOfStudyRelatedSeries int
	declare @NumberOfStudyRelatedInstances int
	declare @NumberOfPatientRelatedStudies int

	-- Select key values
	SELECT @StudyInstanceUid = StudyInstanceUid FROM StudyStorage WHERE GUID = @StudyStorageGUID

	SELECT @StudyGUID = GUID, 
		@PatientGUID = PatientGUID, 
		@NumberOfStudyRelatedSeries = NumberOfStudyRelatedSeries, 
		@NumberOfStudyRelatedInstances = NumberOfStudyRelatedInstances 
	FROM Study 
	WHERE StudyInstanceUid = @StudyInstanceUid and ServerPartitionGUID = @ServerPartitionGUID

	
    -- Now cleanup the more management related tables.
	DELETE FROM FilesystemQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM FilesystemStudyStorage
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM ArchiveQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM RestoreQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM ArchiveStudyStorage
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM WorkQueueUid
	WHERE WorkQueueGUID IN (SELECT GUID from WorkQueue WHERE StudyStorageGUID = @StudyStorageGUID)

	DELETE FROM WorkQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM StudyHistory
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM StudyIntegrityQueueUid
	WHERE StudyIntegrityQueueGUID IN (SELECT GUID from StudyIntegrityQueue WHERE StudyStorageGUID = @StudyStorageGUID)

	DELETE FROM StudyIntegrityQueue
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM StudyStorage
	WHERE GUID = @StudyStorageGUID

	-- Delete the Study / Series / RequestAttributes tables, reduce counts or delete from Patient table
	DELETE FROM RequestAttributes 
	WHERE SeriesGUID IN (select SeriesGUID from Series where StudyGUID = @StudyGUID)

	DELETE FROM Series
	WHERE StudyGUID = @StudyGUID

	DELETE FROM Study
	WHERE GUID = @StudyGUID

	UPDATE Patient
	SET	NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies -1,
		NumberOfPatientRelatedSeries = NumberOfPatientRelatedSeries - @NumberOfStudyRelatedSeries,
		NumberOfPatientRelatedInstances = NumberOfPatientRelatedInstances - @NumberOfStudyRelatedInstances
	WHERE GUID = @PatientGUID
	
	UPDATE dbo.ServerPartition SET StudyCount=StudyCount-1
	WHERE GUID=@ServerPartitionGUID

	-- Do afterwards, in case multiple studies for the same patient are being deleted at once.
	SELECT @NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies 
	FROM Patient
	WHERE GUID = @PatientGUID

	if @NumberOfPatientRelatedStudies = 0
	BEGIN
		DELETE FROM Patient
		WHERE GUID = @PatientGUID
	END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryFilesystemQueue]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryFilesystemQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 14, 2007
-- Description:	Query for candidates from FilesystemQueue
-- =============================================
CREATE PROCEDURE [dbo].[QueryFilesystemQueue] 
	-- Add the parameters for the stored procedure here
	@FilesystemGUID uniqueidentifier, 
	@FilesystemQueueTypeEnum smallint,
	@ScheduledTime datetime,
	@Results int		
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   	-- Added READPAST locking hint to this procedure.  This should cause the query
    -- to just skip rows that are locked, going forward to any other row that 
    -- satisfies the query.  This mode is specifically recommended for work queue type tables.

	SELECT TOP (@Results) * 
	FROM FilesystemQueue WITH (READPAST)
	WHERE
		FilesystemGUID = @FilesystemGUID
		AND FilesystemQueueTypeEnum = @FilesystemQueueTypeEnum
		AND ScheduledTime < @ScheduledTime
	ORDER BY ScheduledTime

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertRequestAttributes]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRequestAttributes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 22, 2007
-- Description:	Insert RequestAttribute table entries
-- =============================================
CREATE PROCEDURE [dbo].[InsertRequestAttributes] 
	-- Add the parameters for the stored procedure here
	@SeriesGUID uniqueidentifier, 
	@RequestedProcedureId nvarchar(16) = null,
	@ScheduledProcedureStepId nvarchar(16) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT GUID from RequestAttributes 
	WHERE
		SeriesGUID = @SeriesGUID
		AND RequestedProcedureId = @RequestedProcedureId
		AND ScheduledProcedureStepId = @ScheduledProcedureStepId

	if @@ROWCOUNT = 0
	BEGIN
		INSERT into RequestAttributes
			(GUID, SeriesGUID, RequestedProcedureId, ScheduledProcedureStepId)
		VALUES
			(newid(), @SeriesGUID, @RequestedProcedureId, @ScheduledProcedureStepId)
	END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryModalitiesInStudy]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryModalitiesInStudy]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 29, 2007
-- Description:	Select modalties associated with a study
-- =============================================
CREATE PROCEDURE [dbo].[QueryModalitiesInStudy] 
	-- Add the parameters for the stored procedure here
	@StudyGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT Modality from Series where StudyGUID = @StudyGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertInstance]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInstance]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 17, 2007
-- Modified:    April 24, 2008
-- Description:	Main insert routine for handling when new images are processed.  This routine
--              determines if Patient/Study/Series need to be inserted, or the counts updated.
-- =============================================
CREATE PROCEDURE [dbo].[InsertInstance] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@PatientId nvarchar(64) = null,
	@PatientsName nvarchar(64) = null,
	@IssuerOfPatientId nvarchar(64) = null,
	@StudyInstanceUid varchar(64),
	@PatientsBirthDate varchar(8) = null,
	@PatientsSex varchar(2) = null,
	@StudyDate varchar(8) = null,
	@StudyTime varchar(16) = null,
	@AccessionNumber nvarchar(16) = null,
	@StudyId nvarchar(16) = null,
	@StudyDescription nvarchar(64) = null,
	@ReferringPhysiciansName nvarchar(64) = null,
	@SeriesInstanceUid varchar(64),
	@Modality varchar(16),
	@SeriesNumber varchar(12) = null,
	@SeriesDescription nvarchar(64) = null,
	@PerformedProcedureStepStartDate varchar(8) = null,
	@PerformedProcedureStepStartTime varchar(16) = null,
	@SourceApplicationEntityTitle varchar(16) = null,
	@SpecificCharacterSet varchar(128) = null,
	@PatientsAge varchar(4) = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @SeriesGUID uniqueidentifier
	declare @StudyGUID uniqueidentifier
	declare @PatientGUID uniqueidentifier
	declare @InsertPatient bit
	declare @InsertStudy bit
	declare @InsertSeries bit

	set @InsertPatient = 0
	set @InsertStudy = 0
	set @InsertSeries = 0

	BEGIN TRANSACTION

	-- First, check for the existance of the Study
	SELECT @StudyGUID = GUID,
		   @PatientGUID = PatientGUID
	FROM Study
	WHERE ServerPartitionGUID = @ServerPartitionGUID
		AND StudyInstanceUid = @StudyInstanceUid

	IF @@ROWCOUNT = 0
	BEGIN
		-- No Study, Check for the Patient table
		if @IssuerOfPatientId is null
		BEGIN
			SELECT @PatientGUID = GUID 
			FROM Patient
			WHERE ServerPartitionGUID = @ServerPartitionGUID
				AND PatientsName = @PatientsName
				AND PatientId = @PatientId
		END
		ELSE
		BEGIN
			SELECT @PatientGUID = GUID 
			FROM Patient
			WHERE ServerPartitionGUID = @ServerPartitionGUID
				AND PatientsName = @PatientsName
				AND PatientId = @PatientId
				AND IssuerOfPatientId = @IssuerOfPatientId
		END

		IF @@ROWCOUNT = 0
		BEGIN
			set @PatientGUID = newid()
			set @InsertPatient = 1

			INSERT into Patient (GUID, ServerPartitionGUID, PatientsName, PatientId, IssuerOfPatientId, NumberOfPatientRelatedStudies, NumberOfPatientRelatedSeries, NumberOfPatientRelatedInstances,SpecificCharacterSet)
			VALUES
				(@PatientGUID, @ServerPartitionGUID, @PatientsName, @PatientId, @IssuerOfPatientId, 0,0,1,@SpecificCharacterSet)
		END
		ELSE
		BEGIN
			UPDATE Patient 
			SET NumberOfPatientRelatedInstances = NumberOfPatientRelatedInstances + 1
			WHERE GUID = @PatientGUID
		END

		set @StudyGUID = newid()
		set @InsertStudy = 1

		INSERT into Study (GUID, ServerPartitionGUID, PatientGUID,
				StudyInstanceUid, PatientsName, PatientId, IssuerOfPatientId, PatientsBirthDate, PatientsAge,
				PatientsSex, StudyDate, StudyTime, AccessionNumber, StudyId,
				StudyDescription, ReferringPhysiciansName, NumberOfStudyRelatedSeries,
				NumberOfStudyRelatedInstances,SpecificCharacterSet)
		VALUES
				(@StudyGUID, @ServerPartitionGUID, @PatientGUID, 
				@StudyInstanceUid, @PatientsName, @PatientId, @IssuerOfPatientId, @PatientsBirthDate, @PatientsAge,
				@PatientsSex, @StudyDate, @StudyTime, @AccessionNumber, @StudyId,
				@StudyDescription, @ReferringPhysiciansName, 0, 1,@SpecificCharacterSet)

		UPDATE dbo.ServerPartition SET StudyCount=StudyCount+1
		WHERE GUID=@ServerPartitionGUID
	

		UPDATE Patient 
		SET NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies + 1
		WHERE GUID = @PatientGUID

	END
	ELSE
	BEGIN
		UPDATE Patient 
			SET NumberOfPatientRelatedInstances = NumberOfPatientRelatedInstances + 1
			WHERE GUID = @PatientGUID

		-- Update Study, Patient TablesNext, the Study Table
		UPDATE Study 
		SET NumberOfStudyRelatedInstances = NumberOfStudyRelatedInstances + 1
		WHERE GUID = @StudyGUID

	END

	-- Finally, the Series Table
	SELECT @SeriesGUID = GUID
	FROM Series
	WHERE 
		ServerPartitionGUID = @ServerPartitionGUID
		AND StudyGUID = @StudyGUID
		AND SeriesInstanceUid = @SeriesInstanceUid

	IF @@ROWCOUNT = 0
	BEGIN
		set @SeriesGUID = newid()
		set @InsertSeries = 1

		INSERT into Series (GUID, ServerPartitionGUID, StudyGUID,
				SeriesInstanceUid, Modality, SeriesNumber, SeriesDescription,
				NumberOfSeriesRelatedInstances, PerformedProcedureStepStartDate,
				PerformedProcedureStepStartTime, SourceApplicationEntityTitle)
		VALUES
				(@SeriesGUID, @ServerPartitionGUID, @StudyGUID, 
				@SeriesInstanceUid, @Modality, @SeriesNumber, @SeriesDescription,
				1,@PerformedProcedureStepStartDate, @PerformedProcedureStepStartTime, 
				@SourceApplicationEntityTitle)

		UPDATE Study
			SET NumberOfStudyRelatedSeries = NumberOfStudyRelatedSeries + 1
		WHERE GUID = @StudyGUID

		UPDATE Patient
			SET NumberOfPatientRelatedSeries = NumberOfPatientRelatedSeries + 1
		WHERE GUID = @PatientGUID
	END
	ELSE
	BEGIN
		UPDATE Series
			SET NumberOfSeriesRelatedInstances = NumberOfSeriesRelatedInstances + 1
		WHERE GUID = @SeriesGUID
	END
	
	COMMIT TRANSACTION

	-- Return the resultant keys
	SELECT @ServerPartitionGUID as ServerPartitionGUID, 
			@PatientGUID as PatientGUID,
			@StudyGUID as StudyGUID,
			@SeriesGUID as SeriesGUID,
			@InsertPatient as InsertPatient,
			@InsertStudy as InsertStudy,
			@InsertSeries as InsertSeries
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertStudyStorage]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertStudyStorage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: 7/30/2007
-- Description:	Called when a new study is received.
-- =============================================
CREATE PROCEDURE [dbo].[InsertStudyStorage] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyInstanceUid varchar(64),
	@Folder varchar(8),
	@FilesystemGUID uniqueidentifier,
	@TransferSyntaxUid varchar(64),
	@StudyStatusEnum smallint,
	@QueueStudyStateEnum smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyStorageGUID as uniqueidentifier
	declare @ServerTransferSyntaxGUID as uniqueidentifier

	select @ServerTransferSyntaxGUID = GUID from ServerTransferSyntax where Uid = @TransferSyntaxUid

	SELECT @StudyStorageGUID=GUID FROM StudyStorage 
	WHERE ServerPartitionGUID = @ServerPartitionGUID AND StudyInstanceUid = @StudyInstanceUid
	IF @@ROWCOUNT = 0
	BEGIN
		set @StudyStorageGUID = NEWID()
	
		INSERT into StudyStorage(GUID, ServerPartitionGUID, StudyInstanceUid, Lock, StudyStatusEnum, QueueStudyStateEnum) 
			values (@StudyStorageGUID, @ServerPartitionGUID, @StudyInstanceUid, 0, @StudyStatusEnum, @QueueStudyStateEnum)
	END
	ELSE
	BEGIN
		declare @StudyGUID as uniqueidentifier

		SELECT @StudyGUID = GUID FROM Study WHERE ServerPartitionGUID = @ServerPartitionGUID AND StudyInstanceUid = @StudyInstanceUid
		UPDATE StudyStorage SET StudyStatusEnum = @StudyStatusEnum WHERE ServerPartitionGUID = @ServerPartitionGUID AND StudyInstanceUid = @StudyInstanceUid

	END

	INSERT into FilesystemStudyStorage(GUID, StudyStorageGUID, FilesystemGUID, StudyFolder, ServerTransferSyntaxGUID)
		values (NEWID(), @StudyStorageGUID, @FilesystemGUID, @Folder, @ServerTransferSyntaxGUID)


	-- Return the study location
	declare @RC int

	-- Have to include all parameters!
	EXECUTE @RC = [ImageServer].[dbo].[QueryStudyStorageLocation] 
		@StudyStorageGUID
		,@ServerPartitionGUID
		,@StudyInstanceUid
END
' 
END
GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

/****** Object:  StoredProcedure [dbo].[DeleteServerPartition]    Script Date: 04/24/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteServerPartition]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: April 24, 2008
-- Update date: April 24, 2008
-- Description:	Completely delete a Server Partition from the database.
--				This involves deleting devies, rules, 
-- =============================================
CREATE PROCEDURE [dbo].[DeleteServerPartition] 
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
		-- PRINT ''Deleting DevicePreferredTransferSyntax''
		delete dbo.DevicePreferredTransferSyntax where DeviceGUID=@DeviceGUID
		--PRINT ''Deleting WorkQueueUid''
		delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where DeviceGUID=@DeviceGUID)
		--PRINT ''Deleting WorkQueue''
		delete dbo.WorkQueue where DeviceGUID=@DeviceGUID
		Fetch NEXT FROM DeviceCursor INTO @DeviceGUID
	END
	CLOSE DeviceCursor
	DEALLOCATE DeviceCursor	
	--PRINT ''Deleting Device''
	delete dbo.Device where ServerPartitionGUID=@ServerPartitionGUID

	/* DELETE STUDYSTORAGE AND RELATED TABLES  */
	DECLARE StudyStorageCursor Cursor For Select GUID from dbo.StudyStorage where ServerPartitionGUID=@ServerPartitionGUID
	Open StudyStorageCursor
	Fetch NEXT FROM StudyStorageCursor INTO @StudyStorageGUID
	While (@@FETCH_STATUS <> -1)
	BEGIN
		--PRINT ''Deleting FilesystemQueue''
		delete dbo.FilesystemQueue where StudyStorageGUID=@StudyStorageGUID
		--PRINT ''Deleting FilesystemStudyStorage''
		delete dbo.FilesystemStudyStorage where StudyStorageGUID=@StudyStorageGUID
		--PRINT ''Deleting WorkQueueUid''
		delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID)
		--PRINT ''Deleting WorkQueue''
		delete dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID
		delete dbo.StudyHistory where StudyStorageGUID=@StudyStorageGUID
		Fetch NEXT FROM StudyStorageCursor INTO @StudyStorageGUID
	END
	CLOSE StudyStorageCursor
	DEALLOCATE StudyStorageCursor	
	--PRINT ''Deleting StudyStorage''
	delete dbo.StudyStorage where ServerPartitionGUID=@ServerPartitionGUID
	
	/* DELETE WORKQUEUE AND RELATED TABLES */
	--PRINT ''Deleting WorkQueueUid''
	delete dbo.WorkQueueUid where WorkQueueGUID in (select GUID from dbo.WorkQueue where StudyStorageGUID=@StudyStorageGUID)
	--PRINT ''Deleting WorkQueue''
	delete dbo.WorkQueue where ServerPartitionGUID=@ServerPartitionGUID
	--PRINT ''Deleting PartitionSopClass''
	delete dbo.PartitionSopClass where ServerPartitionGUID=@ServerPartitionGUID
	--PRINT ''Deleting PartitionTransferSyntax''
	delete dbo.PartitionTransferSyntax where ServerPartitionGUID=@ServerPartitionGUID

	--PRINT ''Deleting ServerRule''
	delete dbo.ServerRule where ServerPartitionGUID=@ServerPartitionGUID

	IF @DeleteStudies=1
	BEGIN
		/* DELETE STUDY, PATIENT AND RELATED TABLES */
		delete dbo.RequestAttributes where SeriesGUID in (Select GUID from dbo.Series where ServerPartitionGUID=@ServerPartitionGUID)
		delete dbo.Series where ServerPartitionGUID=@ServerPartitionGUID
		delete dbo.Study where ServerPartitionGUID=@ServerPartitionGUID
		delete dbo.Patient where ServerPartitionGUID=@ServerPartitionGUID		
	END

	--PRINT ''Deleting ServerPartition''
	delete dbo.ServerPartition where GUID=@ServerPartitionGUID
	
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueue]    Script Date: 04/26/2008 00:28:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: April 24, 2008
-- Update date: Oct 14, 2008
-- Description:	Stored procedure for deleting WorkQueue entries
--
-- Oct 14, 2008: Call UpdateQueueStudyState to update the study status
-- Oct 23, 2008: Removed UpdateQueueStudyState
-- =============================================
CREATE PROCEDURE [dbo].[DeleteWorkQueue] 
	-- Add the parameters for the stored procedure here
	@WorkQueueGUID uniqueidentifier,
	@ServerPartitionGUID uniqueidentifier,
	@WorkQueueTypeEnum smallint,
	@StudyStorageGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyProcessTypeEnum as smallint
	select @StudyProcessTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''StudyProcess''
	declare @ReconcileStudyTypeEnum as smallint
	select @ReconcileStudyTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''ReconcileStudy''

	
	declare @PendingStatusEnum as smallint
	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	
	declare @HighPriorityEnum as smallint
	select @HighPriorityEnum = Enum from WorkQueuePriorityEnum where Lookup = ''High''

	BEGIN TRANSACTION

	UPDATE StudyStorage
		SET Lock = 1, LastAccessedTime = getdate()
	WHERE 
		Lock = 0 
		AND GUID = @StudyStorageGUID

	if (@@ROWCOUNT = 1)
	BEGIN
		-- Make sure we lock the study, so no one else can get it
		COMMIT TRANSACTION

		BEGIN TRANSACTION

		-- Create ''CleanupStudy'' when deleting ''StudyProcess'' 
		IF (@workQueueTypeEnum = @StudyProcessTypeEnum)
		BEGIN
			declare @CleanupStudyTypeEnum as smallint
			select @CleanupStudyTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''CleanupStudy''
			declare @NewWorkQueueGUID uniqueidentifier
			set @NewWorkQueueGUID = NEWID();

			INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime, WorkQueuePriorityEnum)
				values  (@NewWorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @CleanupStudyTypeEnum, @PendingStatusEnum, getdate(), getdate(),@HighPriorityEnum)

			UPDATE WorkQueueUid set WorkQueueGUID = @NewWorkQueueGUID WHERE WorkQueueGUID = @WorkQueueGUID

			DELETE FROM WorkQueue where GUID = @WorkQueueGUID			
		END
		-- Create ''ReconcileCleanup'' when deleting ''ReconcileStudy'' 
		ELSE IF (@workQueueTypeEnum = @ReconcileStudyTypeEnum)
		BEGIN
			DECLARE @StudyHistoryGUID as uniqueidentifier
			DECLARE @CleanupReconcileTypeEnum as smallint
			SELECT @CleanupReconcileTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''ReconcileCleanup''
			
			SET @NewWorkQueueGUID = NEWID();
			SELECT @StudyHistoryGUID=StudyHistoryGUID FROM WorkQueue WHERE GUID=@WorkQueueGUID
			
			INSERT WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime, WorkQueuePriorityEnum)
				values  (@NewWorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @CleanupReconcileTypeEnum, @PendingStatusEnum, getdate(), getdate(),@HighPriorityEnum)
			
			UPDATE newrec
			SET newrec.Data = oldrec.Data
			FROM WorkQueue newrec, WorkQueue oldrec
			WHERE oldrec.GUID=@WorkQueueGUID and newrec.GUID=@NewWorkQueueGUID

			UPDATE WorkQueueUid set WorkQueueGUID = @NewWorkQueueGUID WHERE WorkQueueGUID = @WorkQueueGUID

			DELETE FROM WorkQueue where GUID = @WorkQueueGUID

			-- Delete the study history to force user to manually deal with new images later.
			DELETE StudyHistory WHERE GUID=@StudyHistoryGUID

			
		END
		ELSE
		BEGIN
			DELETE FROM WorkQueueUid WHERE WorkQueueGUID = @WorkQueueGUID
			DELETE FROM WorkQueue WHERE GUID = @WorkQueueGUID;
		END			

		UPDATE StudyStorage
			SET Lock = 0, LastAccessedTime = getdate()
		WHERE 
			Lock = 1 
			AND GUID = @StudyStorageGUID

		COMMIT TRANSACTION
	END
	ELSE
	BEGIN
		ROLLBACK TRANSACTION
		RAISERROR (N''Study could not be locked for deletion of WorkQueue entry.'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	END

	--EXEC UpdateQueueStudyState @StudyStorageGUID

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertArchiveQueue]    Script Date: 07/11/2008 13:04:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertArchiveQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 11, 2008
-- Update date: Oct 15, 2008
-- Description:	Insert and/or update the appropriate ArchiveQueue records
-- 
-- Oct 15, 2008:	Removed Update parameter and insert new entry if the study has been archive so that edit can trigger rearchive
--
-- =============================================
CREATE PROCEDURE [dbo].[InsertArchiveQueue] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @StudyDeleteTypeEnum smallint
	DECLARE @PendingArchiveQueueStatus smallint
	DECLARE @StudyDeleteCount int
	DECLARE @ArchiveStudyStorageCount int
	DECLARE @ArchiveDelayHours int
	DECLARE	@PartitionArchiveGUID uniqueidentifier

	SELECT @StudyDeleteTypeEnum = Enum from FilesystemQueueTypeEnum where Lookup = ''DeleteStudy''
	SELECT @PendingArchiveQueueStatus = Enum from ArchiveQueueStatusEnum where Lookup = ''Pending''

	BEGIN TRANSACTION

	-- Check if there''s any DeleteStudy records in the db
	SELECT @StudyDeleteCount=count(*) FROM FilesystemQueue 
		WHERE FilesystemQueueTypeEnum=@StudyDeleteTypeEnum
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
			-- Check if the study''s been already archived
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
				AND ArchiveQueueStatusEnum = @PendingArchiveQueueStatus
			if @@ROWCOUNT = 0
			BEGIN
				-- There''s no Pending archive entry, insert one
				SET @ArchiveQueueGUID = NEWID();

				INSERT into ArchiveQueue (GUID, PartitionArchiveGUID, StudyStorageGUID, ArchiveQueueStatusEnum, ScheduledTime)
				values  (@ArchiveQueueGUID, @PartitionArchiveGUID, @StudyStorageGUID, @PendingArchiveQueueStatus, @ScheduledTime)
			END
			ELSE
			BEGIN
				UPDATE ArchiveQueue SET ScheduledTime = @ScheduledTime
				WHERE StudyStorageGUID = @StudyStorageGUID
				AND PartitionArchiveGUID = @PartitionArchiveGUID
				AND ArchiveQueueStatusEnum = @PendingArchiveQueueStatus
			END

			Fetch NEXT FROM PartitionArchiveCursor INTO @PartitionArchiveGUID, @ArchiveDelayHours
		END
		CLOSE PartitionArchiveCursor
		DEALLOCATE PartitionArchiveCursor	
	END
	ELSE
	BEGIN
		-- Delete from the ArchiveQueue, the study is scheduled for deletion
		-- In most cases this should delete no rows
		DELETE FROM ArchiveQueue
			WHERE StudyStorageGUID = @StudyStorageGUID
				AND ArchiveQueueStatusEnum = @PendingArchiveQueueStatus
	END

	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateArchiveQueue]    Script Date: 07/14/2008 10:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateArchiveQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 14, 2008
-- Description:	Update an ArchiveQueue row
-- =============================================
CREATE PROCEDURE [dbo].[UpdateArchiveQueue] 
	@ArchiveQueueGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@ScheduledTime datetime = null,
	@ArchiveQueueStatusEnum smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 	declare @CompletedStatusEnum as int
	declare @PendingStatusEnum as int
	declare @FailedStatusEnum as int

	select @CompletedStatusEnum = Enum from ArchiveQueueStatusEnum where Lookup = ''Completed''
	select @PendingStatusEnum = Enum from ArchiveQueueStatusEnum where Lookup = ''Pending''
	select @FailedStatusEnum = Enum from ArchiveQueueStatusEnum where Lookup = ''Failed''
	
	BEGIN TRANSACTION

	if @ArchiveQueueStatusEnum = @CompletedStatusEnum 
	BEGIN
		-- Completed
		DELETE FROM ArchiveQueue where GUID = @ArchiveQueueGUID
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	ELSE 
	BEGIN
		UPDATE ArchiveQueue
		SET ArchiveQueueStatusEnum = @ArchiveQueueStatusEnum, ScheduledTime = @ScheduledTime,
			ProcessorID = Null
		WHERE GUID = @ArchiveQueueGUID
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	
	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryArchiveQueue]    Script Date: 07/14/2008 10:43:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryArchiveQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 14, 2008
-- Description:	Query for entries in the ArchiveQueue
-- =============================================
CREATE PROCEDURE [dbo].[QueryArchiveQueue] 
	-- Add the parameters for the stored procedure here
	@PartitionArchiveGUID uniqueidentifier,
	@ProcessorID varchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.QueryArchiveQueue] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end

	-- Added READPAST locking hint to this procedure.  This should cause the query
    -- to just skip rows that are locked, going forward to any other row that 
    -- satisfies the query.  This mode is specifically recommended for work queue type tables.

	declare @StudyStorageGUID uniqueidentifier
	declare @ArchiveQueueGUID uniqueidentifier
	declare @PendingStatusEnum as int
	declare @InProgressStatusEnum as int

	select @PendingStatusEnum = Enum from ArchiveQueueStatusEnum where Lookup = ''Pending''
	select @InProgressStatusEnum = Enum from ArchiveQueueStatusEnum where Lookup = ''In Progress''
	
	SELECT TOP (1) @StudyStorageGUID = ArchiveQueue.StudyStorageGUID,
		@ArchiveQueueGUID = ArchiveQueue.GUID 
	FROM ArchiveQueue WITH (READPAST)
	JOIN
		StudyStorage ON StudyStorage.GUID = ArchiveQueue.StudyStorageGUID AND StudyStorage.Lock = 0
	WHERE
		ScheduledTime < getdate() 
		AND ArchiveQueue.PartitionArchiveGUID = @PartitionArchiveGUID
		AND ArchiveQueue.ArchiveQueueStatusEnum = @PendingStatusEnum
		AND NOT EXISTS (SELECT GUID FROM WorkQueue WHERE ArchiveQueue.StudyStorageGUID = WorkQueue.StudyStorageGUID)
	ORDER BY ArchiveQueue.ScheduledTime

	if @@ROWCOUNT != 0
	BEGIN
		-- We have a record, now do the updates
		BEGIN TRANSACTION

		UPDATE StudyStorage
			SET Lock = 1, LastAccessedTime = getdate()
		WHERE 
			Lock = 0 
			AND GUID = @StudyStorageGUID

		if (@@ROWCOUNT = 1)
		BEGIN
			UPDATE ArchiveQueue
				SET ArchiveQueueStatusEnum  = @InProgressStatusEnum,
					ProcessorID = @ProcessorID
			WHERE 
				GUID = @ArchiveQueueGUID
				
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			-- In case the lock failed, reset GUID
			SET @ArchiveQueueGUID = newid()
			
			ROLLBACK TRANSACTION
		END
	END
	ELSE
	BEGIN
		-- No matching rows, just create a GUID that will not find any rows.
		SET @ArchiveQueueGUID = newid()
	END	

	-- If the first update failed, this should select 0 records
	SELECT * 
	FROM ArchiveQueue
	WHERE ArchiveQueueStatusEnum = @InProgressStatusEnum
		AND GUID = @ArchiveQueueGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryRestoreQueue]    Script Date: 07/14/2008 10:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRestoreQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 14, 2008
-- Description:	Query for entries in the RestoreQueue
-- =============================================
CREATE PROCEDURE [dbo].[QueryRestoreQueue] 
	@PartitionArchiveGUID uniqueidentifier,
	@RestoreQueueStatusEnum smallint,
	@ProcessorID varchar(256)
AS
BEGIN
	SET NOCOUNT ON;

	if (@ProcessorID is NULL)
	begin
		RAISERROR (N''Calling [dbo.QueryRestoreQueue] with @ProcessorID = NULL'', 18 /* severity.. >=20 means fatal but needs sysadmin role*/, 1 /*state*/)
		RETURN 50000
	end

	-- Added READPAST locking hint to this procedure.  This should cause the query
    -- to just skip rows that are locked, going forward to any other row that 
    -- satisfies the query.  This mode is specifically recommended for work queue type tables.

	declare @StudyStorageGUID uniqueidentifier
	declare @RestoreQueueGUID uniqueidentifier
	declare @InProgressStatusEnum as int

	select @InProgressStatusEnum = Enum from RestoreQueueStatusEnum where Lookup = ''In Progress''
	
	SELECT TOP (1) @StudyStorageGUID = RestoreQueue.StudyStorageGUID,
		@RestoreQueueGUID = RestoreQueue.GUID 
	FROM RestoreQueue WITH (READPAST)
	JOIN
		StudyStorage ON StudyStorage.GUID = RestoreQueue.StudyStorageGUID AND StudyStorage.Lock = 0
	JOIN
		ArchiveStudyStorage ON ArchiveStudyStorage.GUID = RestoreQueue.ArchiveStudyStorageGUID
	WHERE
		ScheduledTime < getdate() 
		AND ArchiveStudyStorage.PartitionArchiveGUID = @PartitionArchiveGUID
		AND RestoreQueue.RestoreQueueStatusEnum = @RestoreQueueStatusEnum
	ORDER BY RestoreQueue.ScheduledTime
	
	IF @@ROWCOUNT != 0
	BEGIN
		-- We have a record, now do the updates
		BEGIN TRANSACTION

		UPDATE StudyStorage
			SET Lock = 1, LastAccessedTime = getdate()
		WHERE 
			Lock = 0 
			AND GUID = @StudyStorageGUID

		if (@@ROWCOUNT = 1)
		BEGIN
			UPDATE RestoreQueue
				SET RestoreQueueStatusEnum  = @InProgressStatusEnum,
					ProcessorID = @ProcessorID
			WHERE 
				GUID = @RestoreQueueGUID
				
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			-- In case the lock failed, reset GUID
			SET @RestoreQueueGUID = newid()
			
			ROLLBACK TRANSACTION
		END
	END
	ELSE
	BEGIN	
		-- No eligible rows, just reset the GUID
		SET @RestoreQueueGUID = newid()
	END	

	-- If the first update failed, this should select 0 records
	SELECT * 
	FROM RestoreQueue
	WHERE RestoreQueueStatusEnum = @InProgressStatusEnum
		AND GUID = @RestoreQueueGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateRestoreQueue]    Script Date: 07/14/2008 10:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateRestoreQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 14, 2008
-- Description:	Update an RestoreQueue row
-- =============================================
CREATE PROCEDURE [dbo].[UpdateRestoreQueue] 
	@RestoreQueueGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@ScheduledTime datetime = null,
	@RestoreQueueStatusEnum smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 	declare @CompletedStatusEnum as int
	declare @PendingStatusEnum as int
	declare @FailedStatusEnum as int

	select @CompletedStatusEnum = Enum from RestoreQueueStatusEnum where Lookup = ''Completed''
	select @PendingStatusEnum = Enum from RestoreQueueStatusEnum where Lookup = ''Pending''
	select @FailedStatusEnum = Enum from RestoreQueueStatusEnum where Lookup = ''Failed''
	
	BEGIN TRANSACTION

	if @RestoreQueueStatusEnum = @CompletedStatusEnum 
	BEGIN
		-- Completed
		DELETE FROM RestoreQueue where GUID = @RestoreQueueGUID
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	ELSE 
	BEGIN
		UPDATE RestoreQueue
		SET RestoreQueueStatusEnum = @RestoreQueueStatusEnum, ScheduledTime = @ScheduledTime,
			ProcessorID = Null
		WHERE GUID = @RestoreQueueGUID
		
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1
	END
	
	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteFilesystemStudyStorage]    Script Date: 07/16/2008 15:46:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteFilesystemStudyStorage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 16, 2008
-- Description:	Make a study go offline/nearline
-- =============================================
CREATE PROCEDURE [dbo].[DeleteFilesystemStudyStorage] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@StudyStatusEnum smallint

AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyInstanceUid varchar(64)
	declare @StudyGUID uniqueidentifier
	declare @IdleQueueStudyStateEnum smallint 

	SELECT @IdleQueueStudyStateEnum=Enum FROM QueueStudyStateEnum WHERE Lookup=''Idle''

	-- Select key values
	SELECT @StudyInstanceUid = StudyInstanceUid FROM StudyStorage WHERE GUID = @StudyStorageGUID

	SELECT @StudyGUID = GUID
	FROM Study 
	WHERE StudyInstanceUid = @StudyInstanceUid and ServerPartitionGUID = @ServerPartitionGUID

	-- Begin the transaction, keep all the deletes/updates in a single transaction
	BEGIN TRANSACTION

	UPDATE StudyStorage SET StudyStatusEnum = @StudyStatusEnum
	WHERE GUID = @StudyStorageGUID
	
    -- Now cleanup the more management related tables.
	DELETE FROM FilesystemQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM FilesystemStudyStorage
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM WorkQueueUid
	WHERE WorkQueueGUID IN (SELECT GUID from WorkQueue WHERE StudyStorageGUID = @StudyStorageGUID)

	DELETE FROM WorkQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	UPDATE StudyStorage
	SET Lock = 0, LastAccessedTime = getdate() 
	WHERE Lock = 1 AND GUID = @StudyStorageGUID

	COMMIT TRANSACTION

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertRestoreQueue]    Script Date: 07/21/2008 16:11:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRestoreQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: July 21, 2008
-- Description:	Insert a RestoreQueue record, if one doesn''t
--              already exist for the Study.
-- =============================================
CREATE PROCEDURE [dbo].[InsertRestoreQueue] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier,
	@ArchiveStudyStorageGUID uniqueidentifier = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @PendingRestoreQueueStatus smallint
	DECLARE @RestoreQueueGUID uniqueidentifier
	DECLARE	@NewArchiveStudyStorageGUID uniqueidentifier
	DECLARE @RestoreQueueStudyStateEnum smallint
	DECLARE @Successful bit
	
	SELECT @PendingRestoreQueueStatus = Enum from RestoreQueueStatusEnum where Lookup = ''Pending''
	SELECT @RestoreQueueStudyStateEnum = Enum from QueueStudyStateEnum where Lookup = ''RestoreScheduled''

	BEGIN TRANSACTION

	IF @ArchiveStudyStorageGUID IS null
	BEGIN
		SELECT TOP 1 @NewArchiveStudyStorageGUID = GUID FROM ArchiveStudyStorage
		WHERE StudyStorageGUID = @StudyStorageGUID
		ORDER BY ArchiveTime DESC
		IF @@ROWCOUNT = 0
		BEGIN
			-- A bit ugly, the study does not appear to be archived, so can''t be restored.
			-- set the GUID to an invalid value, so no rows are returned.
			SET @RestoreQueueGUID = newid()	
		END
		ELSE
		BEGIN
			SELECT @RestoreQueueGUID = GUID FROM RestoreQueue
			WHERE ArchiveStudyStorageGUID = @NewArchiveStudyStorageGUID
			IF @@ROWCOUNT = 0
			BEGIN
				
				EXECUTE [ImageServer].[dbo].[LockStudy] 
						@StudyStorageGUID  ,null  ,@RestoreQueueStudyStateEnum  ,@Successful OUTPUT

				IF @Successful = 0
				BEGIN
					-- Couldn''t lock the study, just return no rows.
					SET @RestoreQueueGUID = newid()
				END
				ELSE
				BEGIN
					SET @RestoreQueueGUID = newid()
					INSERT INTO RestoreQueue (GUID, ArchiveStudyStorageGUID, StudyStorageGUID, ScheduledTime, RestoreQueueStatusEnum, ProcessorId)
					VALUES	(@RestoreQueueGUID, @NewArchiveStudyStorageGUID, @StudyStorageGUID, getdate(), @PendingRestoreQueueStatus, null)
				END
			END
		END
	END
	ELSE
	BEGIN
		SELECT @RestoreQueueGUID = GUID FROM RestoreQueue
		WHERE ArchiveStudyStorageGUID = @ArchiveStudyStorageGUID
		IF @@ROWCOUNT = 0
		BEGIN
			EXECUTE [ImageServer].[dbo].[LockStudy] 
					@StudyStorageGUID  ,null  ,@RestoreQueueStudyStateEnum  ,@Successful OUTPUT

			IF @Successful = 0
			BEGIN
				-- Couldn''t lock the study, just return no rows.
				SET @RestoreQueueGUID = newid()
			END
			ELSE
			BEGIN

				SET @RestoreQueueGUID = newid()
				INSERT INTO RestoreQueue (GUID, ArchiveStudyStorageGUID, StudyStorageGUID, ScheduledTime, RestoreQueueStatusEnum, ProcessorId)
				VALUES	(@RestoreQueueGUID, @ArchiveStudyStorageGUID, @StudyStorageGUID, getdate(), @PendingRestoreQueueStatus, null)
			END
		END
	END

	COMMIT TRANSACTION

	SELECT * FROM RestoreQueue WHERE GUID = @RestoreQueueGUID
END
' 
END
GO

/****** Object:  StoredProcedure [dbo].[WebQueryArchiveQueue]    Script Date: 08/14/2008 15:21:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryArchiveQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 5, 2008
-- Description:	Query ArchiveQueue entries based on criteria
--				
-- =============================================
CREATE PROCEDURE [dbo].[WebQueryArchiveQueue] 
	@ServerPartitionGUID uniqueidentifier = null,
	@PatientId nvarchar(64) = null,
	@PatientsName nvarchar(64) = null,
	@AccessionNumber nvarchar(16) = null,
	@ScheduledTime datetime = null,
	@ArchiveQueueStatusEnum smallint = null,
	@StartIndex int,
	@MaxRowCount int = 25,
	@ResultCount int OUTPUT
AS
BEGIN
	Declare @stmt nvarchar(1024);
	Declare @where nvarchar(1024);
	Declare @count nvarchar(1024);

	-- Build SELECT statement based on the paramters
	
	SET @stmt =			''SELECT ArchiveQueue.*, ROW_NUMBER() OVER(ORDER BY ScheduledTime ASC) as RowNum FROM ArchiveQueue ''
	SET @stmt = @stmt + ''LEFT JOIN StudyStorage on StudyStorage.GUID = ArchiveQueue.StudyStorageGUID ''
	SET @stmt = @stmt + ''LEFT JOIN Study on Study.ServerPartitionGUID = StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid = StudyStorage.StudyInstanceUid ''
	SET @stmt = @stmt + ''JOIN PartitionArchive on PartitionArchive.GUID = ArchiveQueue.PartitionArchiveGUID ''
	
	SET @where = ''''

	IF (@ServerPartitionGUID IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''PartitionArchive.ServerPartitionGUID = '''''' +  CONVERT(varchar(250),@ServerPartitionGUID) +''''''''
	END
	
	IF (@ArchiveQueueStatusEnum IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''ArchiveQueue.ArchiveQueueStatusEnum = '' +  CONVERT(varchar(10),@ArchiveQueueStatusEnum)
	END

	IF (@ScheduledTime IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''ArchiveQueue.ScheduledTime between '''''' +  CONVERT(varchar(30), @ScheduledTime, 101 ) +'''''' and '''''' + CONVERT(varchar(30), DATEADD(DAY, 1, @ScheduledTime), 101 ) + ''''''''
	END

	IF (@PatientsName IS NOT NULL and @PatientsName<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientsName Like ''''%'' + @PatientsName + ''%'''' ''
	END

	IF (@PatientId IS NOT NULL and @PatientId<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientId Like ''''%'' + @PatientId + ''%'''' ''
	END

	IF (@AccessionNumber IS NOT NULL and @AccessionNumber<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.AccessionNumber Like ''''%'' + @AccessionNumber + ''%'''' ''
	END


	if (@where<>'''')
		SET @stmt = @stmt + '' WHERE '' + @where

	PRINT @stmt
	SET @stmt = ''SELECT A.GUID, A.PartitionArchiveGUID, A.ScheduledTime, A.StudyStorageGUID, A.ArchiveQueueStatusEnum, A.ProcessorId FROM ('' + @stmt
	SET @stmt = @stmt + '') AS A WHERE A.RowNum BETWEEN '' + str(@StartIndex) + '' AND ('' + str(@StartIndex) + '' + '' + str(@MaxRowCount) + '') - 1''

	EXEC(@stmt)

	SET @count = ''SELECT @recordCount = count(*) FROM ArchiveQueue JOIN PartitionArchive on PartitionArchive.GUID = ArchiveQueue.PartitionArchiveGUID ''
	if (@where<>'''')
	BEGIN
		SET @count = @count + ''LEFT JOIN StudyStorage on StudyStorage.GUID = ArchiveQueue.StudyStorageGUID ''
		SET @count = @count + ''LEFT JOIN Study on Study.ServerPartitionGUID = StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid = StudyStorage.StudyInstanceUid ''
		SET @count = @count + ''WHERE '' + @where
	END

	DECLARE @recCount int
	
	EXEC sp_executesql  @count, N''@recordCount int OUT'', @recCount OUT
	print @count
	set @ResultCount = @recCount

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[WebQueryRestoreQueue]    Script Date: 08/21/2008 15:21:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebQueryRestoreQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 21, 2008
-- Description:	Query Restore entries based on criteria
--				
-- =============================================
CREATE PROCEDURE [dbo].[WebQueryRestoreQueue] 
	@ServerPartitionGUID uniqueidentifier = null,
	@PatientId nvarchar(64) = null,
	@PatientsName nvarchar(64) = null,
	@AccessionNumber nvarchar(16) = null,
	@ScheduledTime datetime = null,
	@RestoreQueueStatusEnum smallint = null,
	@StartIndex int,
	@MaxRowCount int = 25,
	@ResultCount int OUTPUT
AS
BEGIN
	Declare @stmt nvarchar(1024);
	Declare @where nvarchar(1024);
	Declare @count nvarchar(1024);

	-- Build SELECT statement based on the paramters
	
	SET @stmt =			''SELECT RestoreQueue.*, ROW_NUMBER() OVER(ORDER BY ScheduledTime ASC) as RowNum FROM RestoreQueue ''
	SET @stmt = @stmt + ''JOIN StudyStorage on StudyStorage.GUID = RestoreQueue.StudyStorageGUID ''
	SET @stmt = @stmt + ''JOIN ArchiveStudyStorage on ArchiveStudyStorage.GUID = RestoreQueue.ArchiveStudyStorageGUID ''
	SET @stmt = @stmt + ''LEFT JOIN Study on Study.ServerPartitionGUID = StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid = StudyStorage.StudyInstanceUid ''
	SET @stmt = @stmt + ''JOIN PartitionArchive on PartitionArchive.GUID = ArchiveStudyStorage.PartitionArchiveGUID ''
	
	SET @where = ''''

	IF (@ServerPartitionGUID IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''PartitionArchive.ServerPartitionGUID = '''''' +  CONVERT(varchar(250),@ServerPartitionGUID) +''''''''
	END
	
	IF (@RestoreQueueStatusEnum IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''RestoreQueue.RestoreQueueStatusEnum = '' +  CONVERT(varchar(10),@RestoreQueueStatusEnum)
	END

	IF (@ScheduledTime IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''RestoreQueue.ScheduledTime between '''''' +  CONVERT(varchar(30), @ScheduledTime, 101 ) +'''''' and '''''' + CONVERT(varchar(30), DATEADD(DAY, 1, @ScheduledTime), 101 ) + ''''''''
	END

	IF (@PatientsName IS NOT NULL and @PatientsName<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientsName Like ''''%'' + @PatientsName + ''%'''' ''
	END

	IF (@PatientId IS NOT NULL and @PatientId<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.PatientId Like ''''%'' + @PatientId + ''%'''' ''
	END

	IF (@AccessionNumber IS NOT NULL and @AccessionNumber<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.AccessionNumber Like ''''%'' + @AccessionNumber + ''%'''' ''
	END


	if (@where<>'''')
		SET @stmt = @stmt + '' WHERE '' + @where

	PRINT @stmt
	SET @stmt = ''SELECT A.GUID, A.ArchiveStudyStorageGUID, A.ScheduledTime, A.StudyStorageGUID, A.RestoreQueueStatusEnum, A.ProcessorId FROM ('' + @stmt
	SET @stmt = @stmt + '') AS A WHERE A.RowNum BETWEEN '' + str(@StartIndex) + '' AND ('' + str(@StartIndex) + '' + '' + str(@MaxRowCount) + '') - 1''

	EXEC(@stmt)

	SET @count = ''SELECT @recordCount = count(*) FROM RestoreQueue JOIN ArchiveStudyStorage on ArchiveStudyStorage.GUID = RestoreQueue.ArchiveStudyStorageGUID JOIN PartitionArchive on PartitionArchive.GUID = ArchiveStudyStorage.PartitionArchiveGUID ''
	if (@where<>'''')
	BEGIN
		SET @count = @count + ''JOIN StudyStorage on StudyStorage.GUID = RestoreQueue.StudyStorageGUID ''
		SET @count = @count + ''LEFT JOIN Study on Study.ServerPartitionGUID = StudyStorage.ServerPartitionGUID and Study.StudyInstanceUid = StudyStorage.StudyInstanceUid ''
		SET @count = @count + ''WHERE '' + @where
	END

	DECLARE @recCount int
	
	EXEC sp_executesql  @count, N''@recordCount int OUT'', @recCount OUT
	print @count
	set @ResultCount = @recCount

END
' 
END
GO

/****** Object:  StoredProcedure [dbo].[InsertStudyIntegrityQueue]    Script Date: 09/05/2008 15:21:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertStudyIntegrityQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: September 05, 2008
-- Last update: Nov 06, 2008
-- Description:	Insert or update StudyIntegrity Queue based on supplied data
--				
-- Nov 06, 2008: Change to insert [StudyIntegrityQueueUid] record only if it doesn''t exist.
--
-- =============================================
CREATE PROCEDURE [dbo].[InsertStudyIntegrityQueue] 
	-- Add the parameters for the stored procedure here
	@Description nvarchar(1024),
	@ServerPartitionGUID uniqueidentifier,
	@StudyStorageGUID uniqueidentifier,
	@StudyInstanceUid varchar(64),
	@SeriesInstanceUid varchar(64),
	@SeriesDescription nvarchar(64),
	@SopInstanceUid varchar(64),
	@StudyData xml,
	@QueueData xml=NULL,
	@StudyIntegrityReasonEnum smallint
AS
BEGIN
	
	DECLARE @Guid uniqueidentifier
	DECLARE @QueueStudyStateEnumReconcileRequired smallint
	
	BEGIN TRANSACTION

	SELECT @QueueStudyStateEnumReconcileRequired = Enum FROM QueueStudyStateEnum WHERE Lookup=''ReconcileRequired''

	-- Look for existing StudyIntegrityQueue entry
	SELECT TOP 1 @Guid=GUID 
	FROM	[dbo].[StudyIntegrityQueue]
	WHERE	[ServerPartitionGUID]=@ServerPartitionGUID 
			AND  [StudyStorageGUID]=@StudyStorageGUID
			AND	 CONVERT(nvarchar(max), [StudyData]) = CONVERT(nvarchar(max), @StudyData)
	
	IF @@ROWCOUNT = 0
	BEGIN
		-- PRINT ''Not found''
		SET @Guid=newid()

		INSERT INTO [dbo].[StudyIntegrityQueue]([GUID],[ServerPartitionGUID],[InsertTime],[StudyStorageGUID],[Description],[StudyData],[QueueData],[StudyIntegrityReasonEnum])
		VALUES (@Guid,@ServerPartitionGUID,getdate(),@StudyStorageGUID,@Description,@StudyData,@QueueData,@StudyIntegrityReasonEnum)
	END


	IF NOT EXISTS(SELECT GUID FROM [StudyIntegrityQueueUid] 
				WHERE [StudyIntegrityQueueGUID]=@Guid AND [SeriesInstanceUid]=@SeriesInstanceUid AND [SopInstanceUid]=@SopInstanceUid)
	BEGIN
		INSERT INTO [dbo].[StudyIntegrityQueueUid]([GUID],[StudyIntegrityQueueGUID],[SeriesInstanceUid],[SeriesDescription],[SopInstanceUid])
		VALUES (newid(),@Guid,@SeriesInstanceUid,@SeriesDescription,@SopInstanceUid)
	END

	COMMIT TRANSACTION
	
	SELECT * FROM [dbo].[StudyIntegrityQueue] WHERE GUID=@Guid

END
'
END
GO


/****** Object:  StoredProcedure [dbo].[AttachStudyToPatient]    Script Date: 10/09/2008 11:53:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttachStudyToPatient]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: Oct 09, 2008
-- Description:	Attach a study to a new patient and update all object counts for the old and new patient.
--
-- =============================================
CREATE PROCEDURE [dbo].[AttachStudyToPatient]
	-- Add the parameters for the stored procedure here
	@StudyGUID uniqueidentifier, 
	@NewPatientGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ServerPartitionGUID uniqueidentifier
	DECLARE @CurrentPatientGUID uniqueidentifier
	DECLARE @StudyInstanceUid varchar(64)

	SELECT @ServerPartitionGUID=Study.ServerPartitionGUID,
		   @StudyInstanceUid = Study.StudyInstanceUid, @CurrentPatientGUID=PatientGUID
	FROM Study WHERE Study.GUID=@StudyGUID


	UPDATE Study 
	SET PatientGUID=@NewPatientGUID
	WHERE GUID=	@StudyGUID

	UPDATE Study 
	SET Study.PatientsName=Patient.PatientsName,
		Study.PatientId=Patient.PatientId,
		Study.IssuerOfPatientId = Patient.IssuerOfPatientId	
	FROM Study JOIN Patient ON Patient.GUID=Study.PatientGUID
	WHERE Study.GUID=@StudyGUID
	
	UPDATE Patient 
	SET NumberOfPatientRelatedStudies=NumberOfPatientRelatedStudies+1,
		NumberOfPatientRelatedSeries=NumberOfPatientRelatedSeries+(SELECT Count(GUID)
									From Series WITH(READPAST) 			
									WHERE ServerPartitionGUID=@ServerPartitionGUID AND StudyGUID=@StudyGUID),
		NumberOfPatientRelatedInstances=NumberOfPatientRelatedInstances+(
			SELECT SUM(NumberOfSeriesRelatedInstances)
			From Series WITH(READPAST) 
			WHERE ServerPartitionGUID=@ServerPartitionGUID AND StudyGUID=@StudyGUID)

	WHERE GUID=@NewPatientGUID

	
	DECLARE @StudyCount int

	SELECT @StudyCount =NumberOfPatientRelatedStudies
	FROM Patient WHERE GUID=@CurrentPatientGUID

	PRINT @StudyCount
	IF @StudyCount<=1
	BEGIN
		DELETE Patient WHERE GUID=@CurrentPatientGUID
	END
	ELSE
	BEGIN
		UPDATE Patient Set NumberOfPatientRelatedStudies=NumberOfPatientRelatedStudies-1
		WHERE GUID=@CurrentPatientGUID 
	END
END
'
END

GO


/****** Object:  StoredProcedure [dbo].[CreatePatientForStudy]    Script Date: 10/09/2008 11:53:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreatePatientForStudy]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Thanh Huynh
-- Create date: Oct 09, 2008
-- Description:	Create a new Patient for a study
-- =============================================
CREATE PROCEDURE [dbo].[CreatePatientForStudy]
	-- Add the parameters for the stored procedure here
	@StudyGUID uniqueidentifier, 
	@PatientsName nvarchar(64),
	@PatientId    nvarchar(64),
	@IssuerOfPatientId nvarchar(64)=null,
	@SpecificCharacterSet varchar(128)=null	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ServerPartitionGUID uniqueidentifier
	DECLARE @PatientGUID uniqueidentifier
	DECLARE @CurrentPatientGUID uniqueidentifier
	DECLARE @StudyInstanceUid varchar(64)
	DECLARE @NumStudiesOwnedByCurrentPatient int
	DECLARE @NumSeries int
	DECLARE @NumInstances int

	SELECT	@ServerPartitionGUID=Study.ServerPartitionGUID,
			@StudyInstanceUid = Study.StudyInstanceUid,
			@CurrentPatientGUID=PatientGUID,
			@NumStudiesOwnedByCurrentPatient = Patient.NumberOfPatientRelatedStudies,
			@NumSeries = NumberOfStudyRelatedSeries,
			@NumInstances = NumberOfStudyRelatedInstances
	FROM Study 
	JOIN Patient ON Patient.GUID = Study.PatientGUID
	WHERE Study.GUID=@StudyGUID

	SET @PatientGUID = newid()

	INSERT INTO [Patient]([GUID],[ServerPartitionGUID],[PatientsName],[PatientId],[IssuerOfPatientId],
			[NumberOfPatientRelatedStudies],[NumberOfPatientRelatedSeries],[NumberOfPatientRelatedInstances],[SpecificCharacterSet])
    VALUES
           (@PatientGUID,@ServerPartitionGUID,@PatientsName,@PatientId,@IssuerOfPatientId,0,0,0,@SpecificCharacterSet)

	-- Attach study to the new patient
	UPDATE Study 
	SET PatientGUID=@PatientGUID
	WHERE GUID=@StudyGUID

	-- We may consider copy the fields in the Patient record to the Study record.
	-- Howerver, the Study record should reflect the data in the images and it may also contain information that''s not passed
	-- in to this stored procedure. It''s assumed that the aplication has access to the images so it can update the Study later.
	
	-- Update object count for the new patient	
	UPDATE Patient
	SET [NumberOfPatientRelatedStudies]=1,
		[NumberOfPatientRelatedSeries]=@NumSeries,
		[NumberOfPatientRelatedInstances]=@NumInstances
	WHERE GUID=@PatientGUID

	-- Update current patient, delete it if there''s no attached study.
	IF @NumStudiesOwnedByCurrentPatient=1
	BEGIN
		DELETE Patient WHERE GUID = @CurrentPatientGUID
	END
	ELSE
	BEGIN
		UPDATE Patient
		SET [NumberOfPatientRelatedStudies]=@NumStudiesOwnedByCurrentPatient-1,
			[NumberOfPatientRelatedSeries]=[NumberOfPatientRelatedSeries]-@NumSeries,
			[NumberOfPatientRelatedInstances]=[NumberOfPatientRelatedInstances]-@NumInstances
		WHERE GUID=@CurrentPatientGUID
	END
	
	
	SET NOCOUNT OFF;

	SELECT * FROM Patient WHERE GUID=@PatientGUID
	
END
'
END
GO

