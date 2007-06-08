CREATE TABLE temp_uid ( current_uid UNIQUEIDENTIFIER NOT NULL UNIQUE);
CREATE TABLE temp_index ( next_index INTEGER );

/*
	Create the default study querying dictionary for the viewer which performs the database query using unicode string input from the Gui.
	Notice how only those values that are currently queryable (from the study table) are included in the dictionary, since only study
	level queries are currently supported.
*/

DELETE FROM temp_uid;
DELETE FROM temp_index;
INSERT INTO temp_uid VALUES (newid());
INSERT INTO temp_index values ( 0 );

INSERT INTO DicomDictionaryContainer_ (EntryOid_, DictionaryName_)
select current_uid, 'study-query-unicode' from temp_uid;

INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsName', '(0010,0010)', 0, 'PN', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientId', '(0010,0020)', 0,  'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsSex', '(0010,0040)', 0,  'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDate', '(0008,0020)', 0,  'DA', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'AccessionNumber', '(0008,0050)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyId', '(0020,0010)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyInstanceUid', '(0020,000d)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDescription', '(0008,1030)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ProcedureCodeSequenceCodeValue', '(0008,1032)\(0008,0100)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'AdmittingDiagnosesDescription', '(0008,1080)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ModalitiesInStudy', '(0008,0061)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SOPClassesInStudy', '(0008,0062)', 1, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;

/*
	Create the default results dictionary (Unicode) which returns certain column values independently of the columns used for querying.
	The main differences right now are the dates/times.  Specifically, the StudyDate column is used for querying (it is a DateTime column),
	but the 'raw' StudyDate from the Dicom Header is what is actually returned in the query results.  Notice how only the study level columns
	are specified since it is the only query level currently supported.
*/

DELETE FROM temp_uid;
DELETE FROM temp_index;
INSERT INTO temp_uid VALUES (newid());
INSERT INTO temp_index values ( 0 );

INSERT INTO DicomDictionaryContainer_ (EntryOid_, DictionaryName_)
select current_uid, 'study-query-results-unicode' from temp_uid;

INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsName', '(0010,0010)', 0, 'PN', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientId', '(0010,0020)', 0,  'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsBirthDateRaw', '(0010,0030)', 0, 'DA', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsSex', '(0010,0040)', 0,  'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDateRaw', '(0008,0020)', 0,  'DA', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyTimeRaw', '(0008,0030)', 0,  'TM', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'AccessionNumber', '(0008,0050)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyId', '(0020,0010)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyInstanceUid', '(0020,000d)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDescription', '(0008,1030)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ProcedureCodeSequenceCodeValue', '(0008,1032)\(0008,0100)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ProcedureCodeSequenceCodingSchemeDesignator', '(0008,1032)\(0008,0102)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ModalitiesInStudy', '(0008,0061)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SOPClassesInStudy', '(0008,0062)', 1, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;

/*
	Create the default dictionary (Unicode) that contains all the tags/columns in all the existing tables.  The 'raw' date and time columns are specified because they return
	the values contained in the header vs. the ones used for querying.
*/

DELETE FROM temp_uid;
DELETE FROM temp_index;
INSERT INTO temp_uid VALUES (newid());
INSERT INTO temp_index values ( 0 );

INSERT INTO DicomDictionaryContainer_ (EntryOid_, DictionaryName_)
select current_uid, 'default-unicode' from temp_uid;

INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsName', '(0010,0010)', 0, 'PN', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientId', '(0010,0020)', 0,  'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsBirthDateRaw', '(0010,0030)', 0, 'DA', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PatientsSex', '(0010,0040)', 0,  'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDateRaw', '(0008,0020)', 0,  'DA', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyTimeRaw', '(0008,0030)', 0,  'TM', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'AccessionNumber', '(0008,0050)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyId', '(0020,0010)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyInstanceUid', '(0020,000d)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'StudyDescription', '(0008,1030)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ProcedureCodeSequenceCodeValue', '(0008,1032)\(0008,0100)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ProcedureCodeSequenceCodingSchemeDesignator', '(0008,1032)\(0008,0102)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'Modality', '(0008,0060)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ModalitiesInStudy', '(0008,0061)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SOPClassesInStudy', '(0008,0062)', 1, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SeriesNumber', '(0020,0011)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SeriesInstanceUid', '(0020,000e)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'Laterality', '(0020,0060)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'InstanceNumber', '(0020,0013)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SopInstanceUid', '(0008,0018)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SopClassUid', '(0008,0016)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'TransferSyntaxUid', '(0002,0010)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PhotometricInterpretation', '(0028,0004)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'Rows', '(0028,0010)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'Columns', '(0028,0011)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PixelSpacing', '(0028,0030)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'WindowCenter', '(0028,1050)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'WindowWidth', '(0028,1051)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'RescaleIntercept', '(0028,1052)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'RescaleSlope', '(0028,1053)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PixelRepresentation', '(0028,0103)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'BitsStored', '(0028,0101)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'BitsAllocated', '(0028,0100)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'HighBit', '(0028,0102)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PlanarConfiguration', '(0028,0006)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'FrameOfReferenceUid', '(0020,0052)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ImagePositionPatient', '(0020,0032)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'ImageOrientationPatient', '(0020,0037)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SamplesPerPixel', '(0028,0002)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'SpecificCharacterSet', '(0008,0005)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT current_uid, 'PixelAspectRatio', '(0028,0034)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_ as container inner join temp_uid on container.EntryOid_ = temp_uid.current_uid , temp_index;
UPDATE temp_index SET next_index = next_index + 1;

DROP TABLE temp_index;
DROP TABLE temp_uid;