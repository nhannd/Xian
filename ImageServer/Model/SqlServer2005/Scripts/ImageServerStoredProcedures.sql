USE [ImageServer]
GO
/****** Object:  StoredProcedure [dbo].[QueryFilesystemQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryFilesystemQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryFilesystemQueue]
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueDeleteStudy]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueDeleteStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueueDeleteStudy]
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
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueAutoRoute]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueAutoRoute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueueAutoRoute]
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueStudyProcess]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueStudyProcess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueueStudyProcess]
GO
/****** Object:  StoredProcedure [dbo].[ResetWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResetWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ResetWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionSopClasses]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionSopClasses]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryServerPartitionSopClasses]
GO
/****** Object:  StoredProcedure [dbo].[QueryModalitiesInStudy]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryModalitiesInStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryModalitiesInStudy]
GO
/****** Object:  StoredProcedure [dbo].[InsertInstance]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInstance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertInstance]
GO
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueueUid]    Script Date: 01/08/2008 16:04:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteWorkQueueUid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteWorkQueueUid]
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueueUids]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueueUids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryWorkQueueUids]
GO
/****** Object:  StoredProcedure [dbo].[InsertRequestAttributes]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRequestAttributes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRequestAttributes]
GO
/****** Object:  StoredProcedure [dbo].[QueryRequestAttributes]    Script Date: 01/08/2008 16:04:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRequestAttributes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryRequestAttributes]
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
	@Accession nvarchar(16) = null,
	@StudyDescription nvarchar(64) = null,
	@ScheduledTime datetime = null,
	@Type smallint = null,
	@Status smallint = null
AS
BEGIN
	Declare @stmt nvarchar(1024);
	Declare @where nvarchar(1024);

	-- Build SELECT statement based on the paramters
	
	SET @stmt =			''SELECT WorkQueue.* FROM WorkQueue ''
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
		
		SET @where = @where + ''WorkQueue.WorkQueueTypeEnum = '' + CONVERT(varchar(10), @Type)
	END
	
	IF (@Status IS NOT NULL)
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''WorkQueue.WorkQueueStatusEnum = '' +  CONVERT(varchar(10),@Status)
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

	IF (@Accession IS NOT NULL and @Accession<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.AccessionNumber Like ''''%'' + @Accession + ''%'''' ''
	END

	IF (@StudyDescription IS NOT NULL and @StudyDescription<>'''')
	BEGIN
		IF (@where<>'''')
			SET @where = @where + '' AND ''

		SET @where = @where + ''Study.StudyDescription Like ''''%'' + @StudyDescription + ''%'''' ''
	END


	if (@where<>'''')
		SET @stmt = @stmt + '' WHERE '' + @where

	SET @stmt = @stmt + '' ORDER BY Study.PatientID ASC''

	--PRINT @stmt

	EXEC(@stmt)

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
	if @StudyStorageGUID is null
	BEGIN
	    SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.StudyStatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, StorageFilesystem.StudyFolder, StorageFilesystem.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly,
				Filesystem.FilesystemTierEnum
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN StorageFilesystem on StudyStorage.GUID = StorageFilesystem.StudyStorageGUID
			JOIN Filesystem on StorageFilesystem.FilesystemGUID = Filesystem.GUID
		WHERE StudyStorage.ServerPartitionGuid = @ServerPartitionGUID and StudyStorage.StudyInstanceUid = @StudyInstanceUid
	END
	ELSE
	BEGIN
		SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.StudyStatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, StorageFilesystem.StudyFolder, StorageFilesystem.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly,
				Filesystem.FilesystemTierEnum
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN StorageFilesystem on StudyStorage.GUID = StorageFilesystem.StudyStorageGUID
			JOIN Filesystem on StorageFilesystem.FilesystemGUID = Filesystem.GUID
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
-- Description:	Insert a ServerPartition row
-- =============================================
CREATE PROCEDURE [dbo].[InsertServerPartition] 
	-- Add the parameters for the stored procedure here
	@Enabled bit, 
	@Description nvarchar(128),
	@AeTitle varchar(16),
	@Port int,
	@PartitionFolder nvarchar(16),
	@AcceptAnyDevice bit = 1,
	@AutoInsertDevice bit = 1,
	@DefaultRemotePort int = 104
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SopClassGUID uniqueidentifier
	DECLARE @ServerPartitionGUID uniqueidentifier

	SET @ServerPartitionGUID = newid()

    -- Insert statements for procedure here

	-- Wrap in a transaction
	BEGIN TRANSACTION

	INSERT INTO [ImageServer].[dbo].[ServerPartition] 
		([GUID],[Enabled],[Description],[AeTitle],[Port],[PartitionFolder],[AcceptAnyDevice],[AutoInsertDevice],[DefaultRemotePort])
	VALUES (@ServerPartitionGUID, @Enabled, @Description, @AeTitle, @Port, @PartitionFolder, @AcceptAnyDevice, @AutoInsertDevice, @DefaultRemotePort)


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

	-- Now, put in default rules for the partition
	DECLARE  @StudyServerRuleApplyTimeEnum smallint
	DECLARE  @StudyDeleteServerRuleTypeEnum smallint
	DECLARE  @Tier1RetentionServerRuleTypeEnum smallint
	DECLARE  @OnlineRetentionServerRuleTypeEnum smallint

	-- Get the Study Processed Rule Apply Time
	SELECT @StudyServerRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = ''StudyProcessed''

	-- Get all 3 types of Retention Rules
	SELECT @StudyDeleteServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''StudyDelete''
	SELECT @Tier1RetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''Tier1Retention''
	SELECT @OnlineRetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = ''OnlineRetention''

	-- Insert a default StudyDelete rule
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Delete'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @StudyDeleteServerRuleTypeEnum, 1, 1,
				''<rule id="Default Delete">
					<condition>
					</condition>
					<action><study-delete time="10" timeUnits="days"/></action>
				</rule>'' )

	-- Insert a default Tier1Retention rule
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Tier1 Retention'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @Tier1RetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Tier1 Retention">
					<condition>
					</condition>
					<action><tier1-retention time="3" timeUnits="weeks"/></action>
				</rule>'' )

	-- Insert a default Online Retention Rule
	INSERT INTO [ImageServer].[dbo].[ServerRule]
			   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
		 VALUES
			   (newid(),''Default Online Retention'',@ServerPartitionGUID, @StudyServerRuleApplyTimeEnum, @OnlineRetentionServerRuleTypeEnum, 1, 1,
				''<rule id="Default Online Retention">
					<condition>
					</condition>
					<action><online-retention time="4" timeUnits="weeks"/></action>
				</rule>'' )

	COMMIT TRANSACTION

	SELECT * from ServerPartition

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueDeleteStudy]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueDeleteStudy]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: November 14, 2007
-- Description:	Stored procedure for inserting DeleteStudy WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueueDeleteStudy] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier, 
	@ServerPartitionGUID uniqueidentifier,
	@ExpirationTime datetime,
	@ScheduledTime datetime,
	@DeleteFilesystemQueue bit 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @WorkQueueGUID as uniqueidentifier

	declare @PendingStatusEnum as smallint
	declare @DeleteStudyTypeEnum as smallint
	declare @DeleteStudyFilesystemQueueTypeEnum smallint

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @DeleteStudyTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''DeleteStudy''
	select @DeleteStudyFilesystemQueueTypeEnum = Enum from FilesystemQueueTypeEnum where Lookup = ''DeleteStudy''

	BEGIN TRANSACTION

    -- Insert statements for procedure here
	SELECT @WorkQueueGUID = GUID from WorkQueue 
		where StudyStorageGUID = @StudyStorageGUID
		AND WorkQueueTypeEnum = @DeleteStudyTypeEnum
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime)
			values  (@WorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @DeleteStudyTypeEnum, @PendingStatusEnum, @ExpirationTime, @ScheduledTime)
		IF @DeleteFilesystemQueue = 1
		BEGIN
			DELETE FROM FilesystemQueue
			WHERE StudyStorageGUID = @StudyStorageGUID AND FilesystemQueueTypeEnum = @DeleteStudyFilesystemQueueTypeEnum
		END
	END
	ELSE
	BEGIN
		UPDATE WorkQueue set ExpirationTime = @ExpirationTime
			where GUID = @WorkQueueGUID
	END

	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkQueue]    Script Date: 01/08/2008 16:04:34 ******/
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
-- =============================================
CREATE PROCEDURE [dbo].[UpdateWorkQueue] 
	-- Add the parameters for the stored procedure here
	@ProcessorID varchar(256),
	@WorkQueueGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@WorkQueueStatusEnum smallint,
	@FailureCount int,
	@ExpirationTime datetime = null,
	@ScheduledTime datetime = null
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

	declare @CompletedStatusEnum as int
	declare @PendingStatusEnum as int
	declare @FailedStatusEnum as int

	select @CompletedStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Completed''
	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @FailedStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Failed''

	BEGIN TRANSACTION

	if @WorkQueueStatusEnum = @CompletedStatusEnum 
	BEGIN
		-- Completed
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1

		DELETE FROM WorkQueue where GUID = @WorkQueueGUID
	END
	ELSE if  @WorkQueueStatusEnum = @FailedStatusEnum
	BEGIN
		-- Failed
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1

		UPDATE WorkQueue
		SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
			FailureCount = @FailureCount,
			ProcessorID = @ProcessorID
		WHERE GUID = @WorkQueueGUID
	END
	ELSE
	BEGIN
		-- Pending
		if @WorkQueueStatusEnum = @PendingStatusEnum
		BEGIN
			UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
			WHERE GUID = @StudyStorageGUID AND Lock = 1
		END

		UPDATE WorkQueue
		SET WorkQueueStatusEnum = @WorkQueueStatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
			FailureCount = @FailureCount, ProcessorID = @ProcessorID
		WHERE GUID = @WorkQueueGUID
	END

	COMMIT TRANSACTION

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueAutoRoute]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueAutoRoute]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: October 30, 2007
-- Description:	Stored procedure for inserting AutoRoute WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueueAutoRoute] 
	-- Add the parameters for the stored procedure here
	@StudyStorageGUID uniqueidentifier, 
	@ServerPartitionGUID uniqueidentifier,
	@DeviceGUID uniqueidentifier,
	@SeriesInstanceUid varchar(64),
	@SopInstanceUid varchar(64),
	@ExpirationTime datetime,
	@ScheduledTime datetime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @WorkQueueGUID as uniqueidentifier

	declare @PendingStatusEnum as int
	declare @AutoRouteTypeEnum as int

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @AutoRouteTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''AutoRoute''

	BEGIN TRANSACTION

    -- Insert statements for procedure here
	SELECT @WorkQueueGUID = GUID from WorkQueue 
		where StudyStorageGUID = @StudyStorageGUID
		AND WorkQueueTypeEnum = @AutoRouteTypeEnum
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, DeviceGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime)
			values  (@WorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @DeviceGUID, @AutoRouteTypeEnum, @PendingStatusEnum, @ExpirationTime, @ScheduledTime)
	END
	ELSE
	BEGIN
		UPDATE WorkQueue set ExpirationTime = @ExpirationTime
			where GUID = @WorkQueueGUID
	END

	INSERT into WorkQueueUid(GUID, WorkQueueGUID, SeriesInstanceUid, SopInstanceUid)
		values	(newid(), @WorkQueueGUID, @SeriesInstanceUid, @SopInstanceUid)

	COMMIT TRANSACTION
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueStudyProcess]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueStudyProcess]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 14, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueueStudyProcess] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier,
	@StudyStorageGUID uniqueidentifier,
	@SeriesInstanceUid varchar(64),
	@SopInstanceUid varchar(64),
	@ExpirationTime datetime,
	@ScheduledTime datetime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @WorkQueueGUID as uniqueidentifier

	declare @PendingStatusEnum as int
	declare @StudyProcessTypeEnum as int

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @StudyProcessTypeEnum = Enum from WorkQueueTypeEnum where Lookup = ''StudyProcess''

	BEGIN TRANSACTION

    -- Insert statements for procedure here
	SELECT @WorkQueueGUID = GUID from WorkQueue 
		where StudyStorageGUID = @StudyStorageGUID
		AND WorkQueueTypeEnum = @StudyProcessTypeEnum
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, ServerPartitionGUID, StudyStorageGUID, WorkQueueTypeEnum, WorkQueueStatusEnum, ExpirationTime, ScheduledTime)
			values  (@WorkQueueGUID, @ServerPartitionGUID, @StudyStorageGUID, @StudyProcessTypeEnum, @PendingStatusEnum, @ExpirationTime, @ScheduledTime)
	END
	ELSE
	BEGIN
		UPDATE WorkQueue set ExpirationTime = @ExpirationTime
			where GUID = @WorkQueueGUID
	END

	INSERT into WorkQueueUid(GUID, WorkQueueGUID, SeriesInstanceUid, SopInstanceUid)
		values	(newid(), @WorkQueueGUID, @SeriesInstanceUid, @SopInstanceUid)

	COMMIT TRANSACTION
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
-- Description:	Select WorkQueue entries
-- History:
--		Oct 29, 2007:	Add @ProcessorID
--				
-- =============================================
CREATE PROCEDURE [dbo].[QueryWorkQueue] 
	-- Add the parameters for the stored procedure here
	@ProcessorID varchar(256), 
	@WorkQueueTypeEnum smallint = 0
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


	declare @StudyStorageGUID uniqueidentifier
	declare @WorkQueueGUID uniqueidentifier
	declare @PendingStatusEnum as int
	declare @InProgressStatusEnum as int

	select @PendingStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''Pending''
	select @InProgressStatusEnum = Enum from WorkQueueStatusEnum where Lookup = ''In Progress''
	
    IF @WorkQueueTypeEnum = 0
	BEGIN
		SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
			@WorkQueueGUID = WorkQueue.GUID 
		FROM WorkQueue
		JOIN
			StudyStorage ON StudyStorage.GUID = WorkQueue.StudyStorageGUID AND StudyStorage.Lock = 0
		WHERE
			ScheduledTime < getdate() 
			AND (  WorkQueue.WorkQueueStatusEnum = @PendingStatusEnum )
		ORDER BY WorkQueue.ScheduledTime
	END
	ELSE
	BEGIN
		SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
			@WorkQueueGUID = WorkQueue.GUID 
		FROM WorkQueue
		JOIN
			StudyStorage ON StudyStorage.GUID = WorkQueue.StudyStorageGUID AND StudyStorage.Lock = 0
		WHERE
			ScheduledTime < getdate() 
			AND WorkQueue.WorkQueueStatusEnum = @PendingStatusEnum
			AND WorkQueue.WorkQueueTypeEnum = @WorkQueueTypeEnum
		ORDER BY WorkQueue.ScheduledTime
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
	END

	COMMIT TRANSACTION

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

	SET @GUID = newid()
	SELECT @FilesystemDeleteServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemDelete''
	SELECT @FilesystemReinventoryServiceLockTypeEnum = Enum FROM ServiceLockTypeEnum WHERE [Lookup] = ''FilesystemReinventory''

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

    -- Insert statements for procedure here
	declare @ServiceLockGUID uniqueidentifier
	
    IF @ServiceLockTypeEnum = 0
	BEGIN
		SELECT TOP (1) @ServiceLockGUID = ServiceLock.GUID 
		FROM ServiceLock
		WHERE
			Enabled = 1
			AND ScheduledTime < getdate() 
			AND ( ServiceLock.Lock = 0 )
		ORDER BY ServiceLock.ScheduledTime
	END
	ELSE
	BEGIN
		SELECT TOP (1) @ServiceLockGUID = ServiceLock.GUID 
		FROM ServiceLock
		WHERE
			Enabled = 1
			AND ScheduledTime < getdate() 
			AND ServiceLock.ServiceLockTypeEnum = @ServiceLockTypeEnum
			AND ( ServiceLock.Lock = 0 )
		ORDER BY ServiceLock.ScheduledTime
	END

	-- We have a record, now do the updates

	UPDATE ServiceLock
		SET Lock = 1, ProcessorId = @ProcessorId
	WHERE 
		Lock = 0 
		AND GUID = @ServiceLockGUID

	if (@@ROWCOUNT = 0)
	BEGIN
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
		IF @Lock = 0
		BEGIN
			UPDATE ServiceLock SET ProcessorId = null, ScheduledTime = getdate() 
			WHERE GUID = @ServiceLockGUID
		END
		ELSE
		BEGIN
			UPDATE ServiceLock SET Lock = 0, ScheduledTime = getdate()
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
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertFilesystemQueue] 
	-- Add the parameters for the stored procedure here
	@FilesystemQueueTypeEnum smallint, 
	@StudyStorageGUID uniqueidentifier,
	@FilesystemGUID uniqueidentifier,
	@ScheduledTime datetime,
	@SeriesInstanceUid varchar(64) = null
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
			   ([GUID],[FilesystemQueueTypeEnum],[StudyStorageGUID],[FilesystemGUID],[ScheduledTime],[SeriesInstanceUid])
		 VALUES
			   (newid(), @FilesystemQueueTypeEnum, @StudyStorageGUID, @FilesystemGUID, @ScheduledTime, @SeriesInstanceUid)		
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

	SELECT @NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies 
	FROM Patient
	WHERE GUID = @PatientGUID


	-- Begin the transaction, keep all the deletes in a single transaction
	BEGIN TRANSACTION

	-- Delete the Study / Series / RequestAttributes tables, reduce counts or delete from Patient table
	DELETE FROM RequestAttributes 
	WHERE SeriesGUID IN (select SeriesGUID from Series where StudyGUID = @StudyGUID)

	DELETE FROM Series
	WHERE StudyGUID = @StudyGUID

	DELETE FROM Study
	WHERE GUID = @StudyGUID

	if @NumberOfPatientRelatedStudies > 1
	BEGIN
		UPDATE Patient
		SET	NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies -1,
			NumberOfPatientRelatedSeries = NumberOfPatientRelatedSeries - @NumberOfStudyRelatedSeries,
			NumberOfPatientRelatedInstances = NumberOfPatientRelatedInstances - @NumberOfStudyRelatedInstances
		WHERE GUID = @PatientGUID
	END
	ELSE
	BEGIN
		DELETE FROM Patient
		WHERE GUID = @PatientGUID
	END
	
    -- Now cleanup the more management related tables.
	DELETE FROM FilesystemQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM StorageFilesystem
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM WorkQueueUid
	WHERE WorkQueueGUID IN (SELECT GUID from WorkQueue WHERE StudyStorageGUID = @StudyStorageGUID)

	DELETE FROM WorkQueue 
	WHERE StudyStorageGUID = @StudyStorageGUID

	DELETE FROM StudyStorage
	WHERE GUID = @StudyStorageGUID

	COMMIT TRANSACTION

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

    -- Insert statements for procedure here
	SELECT TOP (@Results) * 
	FROM FilesystemQueue
	WHERE
		FilesystemGUID = @FilesystemGUID
		AND FilesystemQueueTypeEnum = @FilesystemQueueTypeEnum
		AND ScheduledTime < @ScheduledTime
	ORDER BY ScheduledTime

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueueUid]    Script Date: 01/08/2008 16:04:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteWorkQueueUid]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 17, 2007
-- Description:	Delete a WorkQueueUid entry
-- =============================================
CREATE PROCEDURE [dbo].[DeleteWorkQueueUid] 
	-- Add the parameters for the stored procedure here
	@WorkQueueUidGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Do the delete
	DELETE FROM WorkQueueUid 
	WHERE GUID = @WorkQueueUidGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueueUids]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueueUids]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 17, 2007
-- Description:	Seleect WorkQueueUid rows related to a WorkQueue instance
-- =============================================
CREATE PROCEDURE [dbo].[QueryWorkQueueUids] 
	-- Add the parameters for the stored procedure here
	@WorkQueueGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM WorkQueueUid
	WHERE WorkQueueGUID = @WorkQueueGUID
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
/****** Object:  StoredProcedure [dbo].[QueryRequestAttributes]    Script Date: 01/08/2008 16:04:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRequestAttributes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 22, 2007
-- Description:	Select Requested attributes for a series
-- =============================================
CREATE PROCEDURE [dbo].[QueryRequestAttributes] 
	-- Add the parameters for the stored procedure here
	@SeriesGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * 
	FROM RequestAttributes
	WHERE SeriesGUID = @SeriesGUID
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
-- Modified:    December 12, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertInstance] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StudyStatusEnum smallint,
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
	@SourceApplicationEntityTitle varchar(16) = null
	
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

			INSERT into Patient (GUID, ServerPartitionGUID, PatientsName, PatientId, IssuerOfPatientId, NumberOfPatientRelatedStudies, NumberOfPatientRelatedSeries, NumberOfPatientRelatedInstances)
			VALUES
				(@PatientGUID, @ServerPartitionGUID, @PatientsName, @PatientId, @IssuerOfPatientId, 0,0,1)
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
				StudyInstanceUid, PatientsName, PatientId, PatientsBirthDate,
				PatientsSex, StudyDate, StudyTime, AccessionNumber, StudyId,
				StudyDescription, ReferringPhysiciansName, NumberOfStudyRelatedSeries,
				NumberOfStudyRelatedInstances, StudyStatusEnum)
		VALUES
				(@StudyGUID, @ServerPartitionGUID, @PatientGUID, 
				@StudyInstanceUid, @PatientsName, @PatientId, @PatientsBirthDate,
				@PatientsSex, @StudyDate, @StudyTime, @AccessionNumber, @StudyId,
				@StudyDescription, @ReferringPhysiciansName, 0, 1, @StudyStatusEnum)

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
				PerformedProcedureStepStartTime, SourceApplicationEntityTitle, StudyStatusEnum)
		VALUES
				(@SeriesGUID, @ServerPartitionGUID, @StudyGUID, 
				@SeriesInstanceUid, @Modality, @SeriesNumber, @SeriesDescription,
				1,@PerformedProcedureStepStartDate, @PerformedProcedureStepStartTime, 
				@SourceApplicationEntityTitle,	@StudyStatusEnum)

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
	@FilesystemGUID uniqueidentifier
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyStorageGUID as uniqueidentifier
	declare @PendingStatusEnum as int

	set @StudyStorageGUID = NEWID()
	select @PendingStatusEnum = Enum from StudyStatusEnum where Lookup = ''Pending''

	INSERT into StudyStorage(GUID, ServerPartitionGUID, StudyInstanceUid, Lock, StudyStatusEnum) 
		values (@StudyStorageGUID, @ServerPartitionGUID, @StudyInstanceUid, 0, @PendingStatusEnum)

	INSERT into StorageFilesystem(GUID, StudyStorageGUID, FilesystemGUID, StudyFolder)
		values (NEWID(), @StudyStorageGUID, @FilesystemGUID, @Folder)


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
