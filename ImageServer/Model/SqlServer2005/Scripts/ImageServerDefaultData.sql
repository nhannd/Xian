-- TypeEnum inserts
INSERT INTO [ImageServer].[dbo].[TypeEnum]
           ([GUID],[TypeEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'StudyProcess','StudyProcess','Processing of a new incoming study.');


-- StatusEnum inserts
INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Pending','Online','Online')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Pending','Pending','Pending')

INSERT INTO [ImageServer].[dbo].[StatusEnum]
           ([GUID],[StatusEnum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'In Progress','In Progress','In Progress')

