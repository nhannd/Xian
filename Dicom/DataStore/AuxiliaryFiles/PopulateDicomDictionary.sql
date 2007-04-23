CREATE TABLE temp_index ( next_index INTEGER );
INSERT INTO temp_index values ( 0 );
INSERT INTO DicomDictionaryContainer_ (EntryOid_) VALUES(newid());
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PatientsName', '(0010,0010)', 0, 'PN', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PatientId', '(0010,0020)', 0,  'LO', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PatientsBirthDate', '(0010,0030)', 0, 'DA', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PatientsSex', '(0010,0040)', 0,  'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'StudyDate', '(0008,0020)', 0,  'DA', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'StudyTime', '(0008,0030)', 0,  'TM', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'AccessionNumber', '(0008,0050)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'StudyId', '(0020,0010)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'StudyInstanceUid', '(0020,000d)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'StudyDescription', '(0008,1030)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'ProcedureCodeSequenceCodeValue', '(0008,1032)\(0008,0100)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'ProcedureCodeSequenceCodingSchemeDesignator', '(0008,1032)\(0008,0102)', 0, 'SH', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'AdmittingDiagnosesDescription', '(0008,1080)', 0, 'LO', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'Modality', '(0008,0060)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'ModalitiesInStudy', '(0008,0061)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SOPClassesInStudy', '(0008,0062)', 1, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SeriesNumber', '(0020,0011)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SeriesInstanceUid', '(0020,000e)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'Laterality', '(0020,0060)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'InstanceNumber', '(0020,0013)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SopInstanceUid', '(0008,0018)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SopClassUid', '(0008,0016)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'TransferSyntaxUid', '(0002,0010)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PhotometricInterpretation', '(0028,0004)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'Rows', '(0028,0010)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'Columns', '(0028,0011)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PixelSpacing', '(0028,0030)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'WindowCenter', '(0028,1050)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'WindowWidth', '(0028,1051)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'RescaleIntercept', '(0028,1052)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'RescaleSlope', '(0028,1053)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PixelRepresentation', '(0028,0103)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'BitsStored', '(0028,0101)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'BitsAllocated', '(0028,0100)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'HighBit', '(0028,0102)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PlanarConfiguration', '(0028,0006)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'FrameOfReferenceUid', '(0020,0052)', 0, 'UI', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'ImagePositionPatient', '(0020,0032)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'ImageOrientationPatient', '(0020,0037)', 0, 'DS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SamplesPerPixel', '(0028,0002)', 0, 'US', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'TestName', '(0004,0004)', 0, 'UN', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'SpecificCharacterSet', '(0008,0005)', 0, 'CS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
INSERT INTO DictionaryEntries_ (EntryOid_, TagName_, Path_, IsComputed_, ValueRepresentation_, Index_)
SELECT EntryOid_, 'PixelAspectRatio', '(0028,0034)', 0, 'IS', temp_index.next_index from DicomDictionaryContainer_, temp_index;
UPDATE temp_index SET next_index = next_index + 1;
DROP TABLE temp_index;
