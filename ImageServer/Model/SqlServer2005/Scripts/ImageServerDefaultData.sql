-- TypeEnum inserts
INSERT INTO [ImageServer].[dbo].[TypeEnum]
           ([GUID],[TypeEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'StudyProcess','StudyProcess','Processing of a new incoming study.');


-- StatusEnum inserts
INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Online','Online','Online')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Pending','Pending','Pending')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'In Progress','In Progress','In Progress')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Completed','Completed','The Queue entry is completed.')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),203,'Failed','Failed','The Queue entry has failed.')


-- SopClass inserts
INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.1', '12-lead ECG Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.3', 'Ambulatory ECG Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.11', 'Basic Text SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.4.1', 'Basic Voice Audio Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.4', 'Blending Softcopy Presentation State Storage SOP Class', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.3.1', 'Cardiac Electrophysiology Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.65', 'Chest CAD SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.2', 'Color Softcopy Presentation State Storage SOP Class', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.33', 'Comprehensive SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1', 'Computed Radiography Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2', 'CT Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.3', 'Deformable Spatial Registration Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3', 'Digital Intra-oral X-Ray Image Storage – For Presentation', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3.1', 'Digital Intra-oral X-Ray Image Storage – For Processing', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2', 'Digital Mammography X-Ray Image Storage – For Presentation', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2.1', 'Digital Mammography X-Ray Image Storage – For Processing', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1', 'Digital X-Ray Image Storage – For Presentation', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1.1', 'Digital X-Ray Image Storage – For Processing', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.104.1', 'Encapsulated PDF Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2.1', 'Enhanced CT Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.1', 'Enhanced MR Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.22', 'Enhanced SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1.1', 'Enhanced XA Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2.1', 'Enhanced XRF Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.2', 'General ECG Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.1', 'Grayscale Softcopy Presentation State Storage SOP Class', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.29', 'Hardcopy  Grayscale Image Storage SOP Class (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.30', 'Hardcopy Color Image Storage SOP Class (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.2.1', 'Hemodynamic Waveform Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.59', 'Key Object Selection Document', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.50', 'Mammography CAD SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4', 'MR Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.2', 'MR Spectroscopy Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.2', 'Multi-frame Grayscale Byte Secondary Capture Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.3', 'Multi-frame Grayscale Word Secondary Capture Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.1', 'Multi-frame Single Bit Secondary Capture Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.4', 'Multi-frame 1 Color Secondary Capture Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.5', 'Nuclear Medicine Image  Storage (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.20', 'Nuclear Medicine Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.2', 'Ophthalmic Photography 16 Bit Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.1', 'Ophthalmic Photography 8 Bit Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.128', 'Positron Emission Tomography Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.40', 'Procedure Log Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.3', 'Pseudo-Color Softcopy Presentation State Storage SOP Class', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66', 'Raw Data Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.67', 'Real World Value Mapping Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.4', 'RT Beams Treatment Record Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.6', 'RT Brachy Treatment Record Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.2', 'RT Dose Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.1', 'RT Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.9', 'RT Ion Beams Treatment Record Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.8', 'RT Ion Plan Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.5', 'RT Plan Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.3', 'RT Structure Set Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.7', 'RT Treatment Summary Record Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7', 'Secondary Capture Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.4', 'Segmentation Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.2', 'Spatial Fiducials Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.1', 'Spatial Registration Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9', 'Standalone Curve Storage (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.10', 'Standalone Modality LUT Storage (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.8', 'Standalone Overlay Storage (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.129', 'Standalone PET Curve Storage (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11', 'Standalone VOI LUT Storage (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.3', 'Stereometric Relationship Storage', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.27', 'Stored Print Storage SOP Class (Retired)', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6.1', 'Ultrasound Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6', 'Ultrasound Image Storage (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3.1', 'Ultrasound Multi-frame Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3', 'Ultrasound Multi-frame Image Storage (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1.1', 'Video Endoscopic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2.1', 'Video Microscopic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4.1', 'Video Photographic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1', 'VL Endoscopic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2', 'VL Microscopic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4', 'VL Photographic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.3', 'VL Slide-Coordinates Microscopic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.3', 'X-Ray Angiographic Bi-Plane Image Storage (Retired)', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1', 'X-Ray Angiographic Image Storage', 0);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.67', 'X-Ray Radiation Dose SR', 1);

INSERT INTO [ImageServer].[dbo].[SopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2', 'X-Ray Radiofluoroscopic Image Storage', 0);

