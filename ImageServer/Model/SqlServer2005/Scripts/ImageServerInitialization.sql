
/* Delete the standard config tables
delete from Device
delete from ServerRule
delete from ServiceLock
delete from Filesystem
delete from PartitionArchive
delete from PartitionTransferSyntax
delete from PartitionSopClass
delete from ServerPartition
*/

DECLARE @AeTitle varchar(16)
DECLARE @FilesystemPath nvarchar(256)
DECLARE @ServerPartitionGuid uniqueidentifier
DECLARE @HsmArchiveXml xml
DECLARE @DuplicateSopPolicy smallint

-- Set Default Values
set @AeTitle = 'ImageServer'
set @FilesystemPath = 'C:\ImageServer'
set @HsmArchiveXml = '<HsmArchive><RootDir>C:\ImageServer\Archive</RootDir></HsmArchive>'


-- Insert ServerPartitions
SELECT @DuplicateSopPolicy=Enum FROM DuplicateSopPolicyEnum WHERE Lookup='SendSuccess'

DECLARE @RC int
EXECUTE @RC = [ImageServer].[dbo].[InsertServerPartition]
   1
  ,'Primary Partition'
  ,@AeTitle
  ,'105'
  ,'Primary'
  ,@DuplicateSopPolicy

SELECT @ServerPartitionGuid=[GUID] FROM ServerPartition WHERE AeTitle = @AeTitle


-- Insert Filesystems
DECLARE @FilesystemTierEnum smallint

SELECT @FilesystemTierEnum=Enum FROM FilesystemTierEnum where Lookup = 'Tier1'


EXECUTE @RC = [ImageServer].[dbo].[InsertFilesystem]
   @FilesystemTierEnum
  ,@FilesystemPath
  ,1
  ,0
  ,0
  ,'Primary Storage'



-- Insert Archive
declare @HsmArchiveType smallint

SELECT @HsmArchiveType = Enum from ArchiveTypeEnum where Lookup = 'HsmArchive'

INSERT INTO [ImageServer].[dbo].PartitionArchive ([GUID],[ServerPartitionGUID],[ArchiveTypeEnum],[Description],[Enabled],[ReadOnly],[ArchiveDelayHours],[ConfigurationXml])
VALUES (newid(), @ServerPartitionGuid, @HsmArchiveType, 'Primary Archive', 1, 0, 12, @HsmArchiveXml)


-- Insert Rules
DECLARE  @StudyProcessedRuleApplyTimeEnum smallint
DECLARE  @StudyArchiveRuleApplyTimeEnum smallint
DECLARE  @StudyDeleteServerRuleTypeEnum smallint
DECLARE  @Tier1RetentionServerRuleTypeEnum smallint
DECLARE  @OnlineRetentionServerRuleTypeEnum smallint
DECLARE  @StudyCompressServerRuleTypeEnum smallint

-- Get the Study Processed Rule Apply Time
SELECT @StudyProcessedRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = 'StudyProcessed'
SELECT @StudyArchiveRuleApplyTimeEnum = Enum FROM ServerRuleApplyTimeEnum WHERE Lookup = 'StudyArchived'

-- Get all 3 types of Retention Rules
SELECT @StudyDeleteServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = 'StudyDelete'
SELECT @Tier1RetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = 'Tier1Retention'
SELECT @OnlineRetentionServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = 'OnlineRetention'
SELECT @StudyCompressServerRuleTypeEnum = Enum FROM ServerRuleTypeEnum WHERE Lookup = 'StudyCompress'

INSERT INTO [ImageServer].[dbo].[ServerRule]
		   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[RuleXml])
	 VALUES
		   (newid(),'Pediatric Retention',@ServerPartitionGuid, @StudyArchiveRuleApplyTimeEnum, @OnlineRetentionServerRuleTypeEnum, 1, 0,
			'<rule id="Pediatric Retention">
			  <condition expressionLanguage="dicom">
				<dicom-age-less-than test="$PatientsBirthDate" units="years" refValue="21" />
			  </condition>
			  <action>
				<study-delete time="21" timeUnits="patientAge" />
			  </action>
			</rule>' )

INSERT INTO [ImageServer].[dbo].[ServerRule]
		   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[ExemptRule], [RuleXml])
	 VALUES
		   (newid(),'Delete Exempt Rule',@ServerPartitionGuid, @StudyProcessedRuleApplyTimeEnum, @StudyDeleteServerRuleTypeEnum, 1, 0, 1,
			'<rule>
			  <condition expressionLanguage="dicom">
				<or>
				  <equal test="$Modality" refValue="MR" />
				  <equal test="$Modality" refValue="CT" />
				  <equal test="$Modality" refValue="CR" />
				  <equal test="$Modality" refValue="DX" />
				  <equal test="$Modality" refValue="MG" />
				  <equal test="$Modality" refValue="US" />
				</or>
			  </condition>
			  <action>
				<no-op />
			  </action>
			</rule>' )


INSERT INTO [ImageServer].[dbo].[ServerRule]
		   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[ExemptRule], [RuleXml])
	 VALUES
		   (newid(),'Compress US RLE Rule',@ServerPartitionGuid, @StudyProcessedRuleApplyTimeEnum, @StudyCompressServerRuleTypeEnum, 1, 0, 0,
			   '<rule>
				  <condition expressionLanguage="dicom">
					<equal test="$Modality" refValue="US" />
				  </condition>
				  <action>
					<rle time="10" unit="weeks" refValue="$StudyDate" />
				  </action>
				</rule>' )


INSERT INTO [ImageServer].[dbo].[ServerRule]
		   ([GUID],[RuleName],[ServerPartitionGUID],[ServerRuleApplyTimeEnum],[ServerRuleTypeEnum],[Enabled],[DefaultRule],[ExemptRule], [RuleXml])
	 VALUES
		   (newid(),'Compress CT MR CR J2K Rule',@ServerPartitionGuid, @StudyProcessedRuleApplyTimeEnum, @StudyCompressServerRuleTypeEnum, 1, 0, 0,
			'<rule>
			  <condition
				expressionLanguage="dicom">
				<or>
				  <equal test="$Modality"  refValue="CT" />
				  <equal test="$Modality"  refValue="MR" />
				  <equal test="$Modality"  refValue="CR" />
				</or>
			  </condition>
			  <action>
				<jpeg-2000-lossless time="10" unit="weeks" refValue="$StudyDate"/>
			  </action>
			</rule>' )