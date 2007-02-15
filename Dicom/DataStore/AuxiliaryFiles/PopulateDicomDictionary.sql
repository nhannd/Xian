CREATE TABLE temp_index ( next_index INTEGER );
INSERT INTO temp_index values ( 0 );
INSERT INTO DicomDictionaryContainer_ (EntryOid_) select next_hi from hibernate_unique_key;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PatientsName', '(0010,0010)', 0, 'PN', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PatientId', '(0010,0020)', 0,  'LO', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PatientsBirthDate', '(0010,0030)', 0, 'DA', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PatientsSex', '(0010,0040)', 0,  'CS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'StudyDate', '(0008,0020)', 0,  'DA', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'StudyTime', '(0008,0030)', 0,  'TM', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'AccessionNumber', '(0008,0050)', 0, 'SH', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'StudyId', '(0020,0010)', 0, 'SH', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'StudyInstanceUid', '(0020,000d)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'StudyDescription', '(0008,1030)', 0, 'LO', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'ProcedureCodeSequenceCodeValue', '(0008,1032)\(0008,0100)', 0, 'SH', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'ProcedureCodeSequenceCodingSchemeDesignator', '(0008,1032)\(0008,0102)', 0, 'SH', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'AdmittingDiagnosesDescription', '(0008,1080)', 0, 'LO', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'Modality', '(0008,0060)', 0, 'CS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'ModalitiesInStudy', '(0008,0061)', 1, 'CS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SOPClassesInStudy', '(0008,0062)', 1, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SeriesNumber', '(0020,0011)', 0, 'IS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SeriesInstanceUid', '(0020,000e)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'Laterality', '(0020,0060)', 0, 'CS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'InstanceNumber', '(0020,0013)', 0, 'IS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SopInstanceUid', '(0008,0018)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SopClassUid', '(0008,0016)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'TransferSyntaxUid', '(0002,0010)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PhotometricInterpretation', '(0028,0004)', 0, 'CS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'Rows', '(0028,0010)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'Columns', '(0028,0011)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PixelSpacing', '(0028,0030)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'WindowCenter', '(0028,1050)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'WindowWidth', '(0028,1051)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'RescaleIntercept', '(0028,1052)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'RescaleSlope', '(0028,1053)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PixelRepresentation', '(0028,0103)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'BitsStored', '(0028,0101)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'BitsAllocated', '(0028,0100)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'HighBit', '(0028,0102)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'PlanarConfiguration', '(0028,0006)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'FrameOfReferenceUid', '(0020,0052)', 0, 'UI', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'ImagePositionPatient', '(0020,0032)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'ImageOrientationPatient', '(0020,0037)', 0, 'DS', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'SamplesPerPixel', '(0028,0002)', 0, 'US', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT next_hi, 'TestName', '(0004,0004)', 0, 'UN', temp_index.next_index from hibernate_unique_key, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
DROP TABLE temp_index;
UPDATE hibernate_unique_key SET next_hi = next_hi + 1;
