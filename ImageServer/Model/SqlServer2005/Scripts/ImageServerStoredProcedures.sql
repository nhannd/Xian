USE [ImageServer]
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueueUids]    Script Date: 10/04/2007 12:51:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueueUids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryWorkQueueUids]
GO
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueueUid]    Script Date: 10/04/2007 12:51:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteWorkQueueUid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteWorkQueueUid]
GO
/****** Object:  StoredProcedure [dbo].[InsertInstance]    Script Date: 10/04/2007 12:51:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInstance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertInstance]
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkQueue]    Script Date: 10/04/2007 12:51:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[InsertRequestAttributes]    Script Date: 10/04/2007 12:51:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRequestAttributes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRequestAttributes]
GO
/****** Object:  StoredProcedure [dbo].[QueryRequestAttributes]    Script Date: 10/04/2007 12:51:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRequestAttributes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryRequestAttributes]
GO
/****** Object:  StoredProcedure [dbo].[QueryModalitiesInStudy]    Script Date: 10/04/2007 12:51:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryModalitiesInStudy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryModalitiesInStudy]
GO
/****** Object:  StoredProcedure [dbo].[QueryDevice]    Script Date: 10/04/2007 12:51:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryDevice]
GO
/****** Object:  StoredProcedure [dbo].[InsertDevice]    Script Date: 10/04/2007 12:51:50 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDevice]
GO
/****** Object:  StoredProcedure [dbo].[ReadServerTransferSyntaxes]    Script Date: 10/04/2007 12:51:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadServerTransferSyntaxes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReadServerTransferSyntaxes]
GO
/****** Object:  StoredProcedure [dbo].[QueryDevicePreferredTransferSyntaxes]    Script Date: 10/04/2007 12:51:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryDevicePreferredTransferSyntaxes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryDevicePreferredTransferSyntaxes]
GO
/****** Object:  StoredProcedure [dbo].[InsertFileSystemTier]    Script Date: 10/04/2007 12:51:50 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFileSystemTier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFileSystemTier]
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystem]    Script Date: 10/04/2007 12:51:50 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFilesystem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertFilesystem]
GO
/****** Object:  StoredProcedure [dbo].[ReadServerPartitions]    Script Date: 10/04/2007 12:51:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadServerPartitions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReadServerPartitions]
GO
/****** Object:  StoredProcedure [dbo].[ReadFilesystemTiers]    Script Date: 10/04/2007 12:51:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadFilesystemTiers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReadFilesystemTiers]
GO
/****** Object:  StoredProcedure [dbo].[InsertStudyStorage]    Script Date: 10/04/2007 12:51:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertStudyStorage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertStudyStorage]
GO
/****** Object:  StoredProcedure [dbo].[ReadFilesystems]    Script Date: 10/04/2007 12:51:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadFilesystems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReadFilesystems]
GO
/****** Object:  StoredProcedure [dbo].[ReadSopClasses]    Script Date: 10/04/2007 12:51:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadSopClasses]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReadSopClasses]
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionSopClasses]    Script Date: 10/04/2007 12:51:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryServerPartitionSopClasses]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryServerPartitionSopClasses]
GO
/****** Object:  StoredProcedure [dbo].[InsertServerPartition]    Script Date: 10/04/2007 12:51:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertServerPartition]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertServerPartition]
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueStudyProcess]    Script Date: 10/04/2007 12:51:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueStudyProcess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertWorkQueueStudyProcess]
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueue]    Script Date: 10/04/2007 12:51:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryWorkQueue]
GO
/****** Object:  StoredProcedure [dbo].[QueryStudyStorageLocation]    Script Date: 10/04/2007 12:51:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryStudyStorageLocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryStudyStorageLocation]
GO
/****** Object:  StoredProcedure [dbo].[QueryDevice]    Script Date: 10/04/2007 12:51:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryDevice]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 7, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[QueryDevice] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@AeTitle varchar(16)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from Device 
	WHERE ServerPartitionGUID = @ServerPartitionGUID and AeTitle = @AeTitle
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertRequestAttributes]    Script Date: 10/04/2007 12:51:52 ******/
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
/****** Object:  StoredProcedure [dbo].[QueryRequestAttributes]    Script Date: 10/04/2007 12:51:54 ******/
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
/****** Object:  StoredProcedure [dbo].[QueryWorkQueueUids]    Script Date: 10/04/2007 12:51:55 ******/
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
/****** Object:  StoredProcedure [dbo].[DeleteWorkQueueUid]    Script Date: 10/04/2007 12:51:49 ******/
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
/****** Object:  StoredProcedure [dbo].[InsertWorkQueueStudyProcess]    Script Date: 10/04/2007 12:51:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertWorkQueueStudyProcess]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 14, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkQueueStudyProcess] 
	-- Add the parameters for the stored procedure here
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

	select @PendingStatusEnum = StatusEnum from StatusEnum where Lookup = ''Pending''
	select @StudyProcessTypeEnum = TypeEnum from TypeEnum where Lookup = ''StudyProcess''

    -- Insert statements for procedure here
	SELECT @WorkQueueGUID = GUID from WorkQueue 
		where StudyStorageGUID = @StudyStorageGUID
		AND TypeEnum = @StudyProcessTypeEnum
	if @@ROWCOUNT = 0
	BEGIN
		set @WorkQueueGUID = NEWID();

		INSERT into WorkQueue (GUID, StudyStorageGUID, TypeEnum, StatusEnum, ExpirationTime, ScheduledTime)
			values  (@WorkQueueGUID, @StudyStorageGUID, @StudyProcessTypeEnum, @PendingStatusEnum, @ExpirationTime, @ScheduledTime)
	END
	ELSE
	BEGIN
		UPDATE WorkQueue set ExpirationTime = @ExpirationTime
			where GUID = @WorkQueueGUID
	END

	INSERT into WorkQueueUid(GUID, WorkQueueGUID, SeriesInstanceUid, SopInstanceUid)
		values	(newid(), @WorkQueueGUID, @SeriesInstanceUid, @SopInstanceUid)
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ReadFilesystemTiers]    Script Date: 10/04/2007 12:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadFilesystemTiers]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: 7/30/2007
-- Description:	Return the FilesystemTier table entries
-- =============================================
CREATE PROCEDURE [dbo].[ReadFilesystemTiers] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT GUID, Description, TierId from FilesystemTier;
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFileSystemTier]    Script Date: 10/04/2007 12:51:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFileSystemTier]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 17, 2007
-- Description:	Insert FileSystemTier
-- =============================================
CREATE PROCEDURE [dbo].[InsertFileSystemTier] 
	-- Add the parameters for the stored procedure here
	@Description nvarchar(128), 
	@TierId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @GUID uniqueidentifier

	SET @GUID = newid()

    -- Insert statements for procedure here
	INSERT INTO [ImageServer].[dbo].FilesystemTier 
		([GUID],[Description],[TierId])
	VALUES (@GUID, @Description, @TierId)

	SELECT * FROM FilesystemTier where GUID = @GUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertDevice]    Script Date: 10/04/2007 12:51:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDevice]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 10, 2007
-- Description:	Stored procedure for inserting into the device table.
-- =============================================
CREATE PROCEDURE [dbo].[InsertDevice] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@AeTitle varchar(16),
	@Description nvarchar(256),
	@IpAddress varchar(16),
	@Active bit,
	@Dhcp bit,
	@Port int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	INSERT into Device (GUID, ServerPartitionGUID, AeTitle, Description, IpAddress, Port, Active, Dhcp)
		values  (NEWID(), @ServerPartitionGUID, @AeTitle, @Description, @IpAddress, @Port, @Active, @Dhcp)
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertInstance]    Script Date: 10/04/2007 12:51:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInstance]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 17, 2007
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[InsertInstance] 
	-- Add the parameters for the stored procedure here
	@ServerPartitionGUID uniqueidentifier, 
	@StatusEnum smallint,
	@PatientId nvarchar(64) = null,
	@PatientName nvarchar(64) = null,
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
	@PerformedProcedureStepStartTime varchar(16) = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @SeriesGUID uniqueidentifier
	declare @StudyGUID uniqueidentifier
	declare @PatientGUID uniqueidentifier

	-- Start with the Patient table
	if @IssuerOfPatientId is null
	BEGIN
		SELECT @PatientGUID = GUID 
		FROM Patient
		WHERE ServerPartitionGUID = @ServerPartitionGUID
			AND PatientName = @PatientName
			AND PatientId = @PatientId
	END
	ELSE
	BEGIN
		SELECT @PatientGUID = GUID 
		FROM Patient
		WHERE ServerPartitionGUID = @ServerPartitionGUID
			AND PatientName = @PatientName
			AND PatientId = @PatientId
			AND IssuerOfPatientId = @IssuerOfPatientId
	END

	if @@ROWCOUNT = 0
	BEGIN
		set @PatientGUID = newid()
		INSERT into Patient (GUID, ServerPartitionGUID, PatientName, PatientId, IssuerOfPatientId, NumberOfPatientRelatedStudies, NumberOfPatientRelatedSeries, NumberOfPatientRelatedInstances)
		VALUES
			(@PatientGUID, @ServerPartitionGUID, @PatientName, @PatientId, @IssuerOfPatientId, 0,0,1)
	END
	ELSE
	BEGIN
		UPDATE Patient 
		SET NumberOfPatientRelatedInstances = NumberOfPatientRelatedInstances +1
		WHERE GUID = @PatientGUID
	END

	-- Next, the Study Table
	SELECT @StudyGUID = GUID
	FROM Study
	WHERE ServerPartitionGUID = @ServerPartitionGUID
		AND StudyInstanceUid = @StudyInstanceUid
		AND PatientGUID = @PatientGUID

	IF @@ROWCOUNT = 0
	BEGIN
		set @StudyGUID = newID()

		INSERT into Study (GUID, ServerPartitionGUID, PatientGUID,
				StudyInstanceUid, PatientName, PatientId, PatientsBirthDate,
				PatientsSex, StudyDate, StudyTime, AccessionNumber, StudyId,
				StudyDescription, ReferringPhysiciansName, NumberOfStudyRelatedSeries,
				NumberOfStudyRelatedInstances, StatusEnum)
		VALUES
				(@StudyGUID, @ServerPartitionGUID, @PatientGUID, 
				@StudyInstanceUid, @PatientName, @PatientId, @PatientsBirthDate,
				@PatientsSex, @StudyDate, @StudyTime, @AccessionNumber, @StudyId,
				@StudyDescription, @ReferringPhysiciansName, 0, 1, @StatusEnum)

		UPDATE Patient
			SET NumberOfPatientRelatedStudies = NumberOfPatientRelatedStudies + 1
			WHERE GUID = @PatientGUID
	END
	ELSE
	BEGIN
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

		INSERT into Series (GUID, ServerPartitionGUID, StudyGUID,
				SeriesInstanceUid, Modality, SeriesNumber, SeriesDescription,
				NumberOfSeriesRelatedInstances, PerformedProcedureStepStartDate,
				PerformedProcedureStepStartTime, StatusEnum)
		VALUES
				(@SeriesGUID, @ServerPartitionGUID, @StudyGUID, 
				@SeriesInstanceUid, @Modality, @SeriesNumber, @SeriesDescription,
				1,@PerformedProcedureStepStartDate, @PerformedProcedureStepStartTime,
				@StatusEnum)

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

	-- Return the resultant keys
	SELECT @ServerPartitionGUID as ServerPartitionGUID, 
			@PatientGUID as PatientGUID,
			@StudyGUID as StudyGUID,
			@SeriesGUID as SeriesGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ReadServerTransferSyntaxes]    Script Date: 10/04/2007 12:51:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadServerTransferSyntaxes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 13, 2007
-- Description:	Read the contents of the ServerTransferSyntax table
-- =============================================
CREATE PROCEDURE [dbo].[ReadServerTransferSyntaxes] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from ServerTransferSyntax
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkQueue]    Script Date: 10/04/2007 12:51:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 20, 2007
-- Description:	Procedure for updating WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[UpdateWorkQueue] 
	-- Add the parameters for the stored procedure here
	@WorkQueueGUID uniqueidentifier, 
	@StudyStorageGUID uniqueidentifier,
	@StatusEnum smallint,
	@FailureCount int,
	@ExpirationTime datetime = null,
	@ScheduledTime datetime = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @CompletedStatusEnum as int
	declare @PendingStatusEnum as int
	declare @FailedStatusEnum as int

	select @CompletedStatusEnum = StatusEnum from StatusEnum where Lookup = ''Completed''
	select @PendingStatusEnum = StatusEnum from StatusEnum where Lookup = ''Pending''
	select @FailedStatusEnum = StatusEnum from StatusEnum where Lookup = ''Failed''

	BEGIN TRANSACTION

	if @StatusEnum = @CompletedStatusEnum 
	BEGIN
		-- Completed
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1

		DELETE FROM WorkQueue where GUID = @WorkQueueGUID
	END
	ELSE if  @StatusEnum = @FailedStatusEnum
	BEGIN
		-- Failed
		UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
		WHERE GUID = @StudyStorageGUID AND Lock = 1

		UPDATE WorkQueue
		SET StatusEnum = @StatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
			FailureCount = @FailureCount
		WHERE GUID = @WorkQueueGUID
	END
	ELSE
	BEGIN
		-- Pending
		if @StatusEnum = @PendingStatusEnum
		BEGIN
			UPDATE StudyStorage set Lock = 0, LastAccessedTime = getdate() 
			WHERE GUID = @StudyStorageGUID AND Lock = 1
		END

		UPDATE WorkQueue
		SET StatusEnum = @StatusEnum, ExpirationTime = @ExpirationTime, ScheduledTime = @ScheduledTime,
			FailureCount = @FailureCount
		WHERE GUID = @WorkQueueGUID
	END

	COMMIT TRANSACTION

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryWorkQueue]    Script Date: 10/04/2007 12:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryWorkQueue]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 16, 2007
-- Description:	Select WorkQueue entries
-- =============================================
CREATE PROCEDURE [dbo].[QueryWorkQueue] 
	-- Add the parameters for the stored procedure here
	@TypeEnum smallint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @StudyStorageGUID uniqueidentifier
	declare @WorkQueueGUID uniqueidentifier
	declare @PendingStatusEnum as int
	declare @InProgressStatusEnum as int

	select @PendingStatusEnum = StatusEnum from StatusEnum where Lookup = ''Pending''
	select @InProgressStatusEnum = StatusEnum from StatusEnum where Lookup = ''In Progress''
	
    IF @TypeEnum = 0
	BEGIN
		SELECT TOP (1) @StudyStorageGUID = WorkQueue.StudyStorageGUID,
			@WorkQueueGUID = WorkQueue.GUID 
		FROM WorkQueue
		JOIN
			StudyStorage ON StudyStorage.GUID = WorkQueue.StudyStorageGUID AND StudyStorage.Lock = 0
		WHERE
			ScheduledTime < getdate() 
			AND WorkQueue.StatusEnum = @PendingStatusEnum
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
			AND WorkQueue.StatusEnum = @PendingStatusEnum
			AND WorkQueue.TypeEnum = @TypeEnum
	END

	-- We have a record, now do the updates
	UPDATE StudyStorage
		SET Lock = 1, LastAccessedTime = getdate()
	WHERE 
		Lock = 0 
		AND GUID = @StudyStorageGUID

	if (@@ROWCOUNT = 1)
	BEGIN
		UPDATE WorkQueue
			SET StatusEnum = @InProgressStatusEnum
		WHERE 
			GUID = @WorkQueueGUID
	END

	-- If the first update failed, this should select 0 records
	SELECT * 
	FROM WorkQueue
	WHERE StatusEnum = @InProgressStatusEnum
		AND GUID = @WorkQueueGUID		
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryStudyStorageLocation]    Script Date: 10/04/2007 12:51:55 ******/
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
	    SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.StatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, StorageFilesystem.StudyFolder, StorageFilesystem.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly
		FROM StudyStorage
			JOIN ServerPartition on StudyStorage.ServerPartitionGUID = ServerPartition.GUID
			JOIN StorageFilesystem on StudyStorage.GUID = StorageFilesystem.StudyStorageGUID
			JOIN Filesystem on StorageFilesystem.FilesystemGUID = Filesystem.GUID
		WHERE StudyStorage.ServerPartitionGuid = @ServerPartitionGUID and StudyStorage.StudyInstanceUid = @StudyInstanceUid
	END
	ELSE
	BEGIN
		SELECT  StudyStorage.GUID, StudyStorage.StudyInstanceUid, StudyStorage.ServerPartitionGUID, StudyStorage.LastAccessedTime, StudyStorage.StatusEnum,
				Filesystem.FilesystemPath, ServerPartition.PartitionFolder, StorageFilesystem.StudyFolder, StorageFilesystem.FilesystemGUID, Filesystem.Enabled, Filesystem.ReadOnly, Filesystem.WriteOnly
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
/****** Object:  StoredProcedure [dbo].[QueryDevicePreferredTransferSyntaxes]    Script Date: 10/04/2007 12:51:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryDevicePreferredTransferSyntaxes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: September 13, 2007
-- Description:	Qery the DevicePreferredTransferSyntax table
-- =============================================
CREATE PROCEDURE [dbo].[QueryDevicePreferredTransferSyntaxes] 
	-- Add the parameters for the stored procedure here
	@DeviceGUID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM DevicePreferredTransferSyntax where DeviceGUID = @DeviceGUID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryModalitiesInStudy]    Script Date: 10/04/2007 12:51:54 ******/
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
/****** Object:  StoredProcedure [dbo].[InsertServerPartition]    Script Date: 10/04/2007 12:51:52 ******/
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
	@PartitionFolder nvarchar(16)
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
		([GUID],[Enabled],[Description],[AeTitle],[Port],[PartitionFolder])
	VALUES (@ServerPartitionGUID, @Enabled, @Description, @AeTitle, @Port, @PartitionFolder)


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

	COMMIT TRANSACTION

	SELECT GUID, Enabled, Description, AeTitle, Port, PartitionFolder from ServerPartition

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ReadServerPartitions]    Script Date: 10/04/2007 12:51:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadServerPartitions]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ReadServerPartitions] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT GUID, Enabled, Description, AeTitle, Port, PartitionFolder from ServerPartition
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[ReadFilesystems]    Script Date: 10/04/2007 12:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadFilesystems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: 7/20/2007
-- Description:	This procedure retrieves all rows in the Filesystem table
-- =============================================
CREATE PROCEDURE [dbo].[ReadFilesystems] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT GUID, FilesystemTierGUID as FilesystemTierKey, FilesystemPath, Enabled, ReadOnly, WriteOnly, Description from Filesystem
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFilesystem]    Script Date: 10/04/2007 12:51:50 ******/
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
	@FilesystemTierGUID uniqueidentifier, 
	@FilesystemPath nvarchar(256),
	@Enabled bit = 1,
	@ReadOnly bit = 0,
	@WriteOnly bit = 0,
	@Description nvarchar(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @GUID uniqueidentifier

	SET @GUID = newid()

    -- Insert statements for procedure here
	INSERT INTO [ImageServer].[dbo].Filesystem 
		([GUID],[FilesystemTierGUID],[FilesystemPath],[Enabled],[ReadOnly],[WriteOnly],[Description])
	VALUES (@GUID, @FilesystemTierGUID, @FilesystemPath, @Enabled, @ReadOnly, @WriteOnly, @Description)

	SELECT * FROM Filesystem where GUID = @GUID	
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[QueryServerPartitionSopClasses]    Script Date: 10/04/2007 12:51:54 ******/
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
/****** Object:  StoredProcedure [dbo].[ReadSopClasses]    Script Date: 10/04/2007 12:51:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReadSopClasses]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Steve Wranovsky
-- Create date: August 13, 2007
-- Description:	Procedure for returning all SopClasses supported by the server
-- =============================================
CREATE PROCEDURE [dbo].[ReadSopClasses] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from ServerSopClass
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertStudyStorage]    Script Date: 10/04/2007 12:51:53 ******/
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
	select @PendingStatusEnum = StatusEnum from StatusEnum where Lookup = ''Pending''

	INSERT into StudyStorage(GUID, ServerPartitionGUID, StudyInstanceUid, Lock, StatusEnum) 
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
