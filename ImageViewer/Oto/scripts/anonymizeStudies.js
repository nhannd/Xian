function main()
{
	// expects a CSV file of the form:
	// OriginalAccessionNumber,PatientId,PatientsName,PatientsSex,PatientsBirthDate,AcessionNumber
	
	var rows = readData("D:\\junk\\foo.txt").map(
		function(row)
		{
			return {
				OriginalAccession : row[0],
				PatientId: row[1],
				PatientsName: row[2],
				PatientsSex: row[3],
				PatientsBirthDate: DateTime.Parse(row[4]),
				AccessionNumber: row[5]
				};
		});
	
	// process each data row
	for(var i=0; i < rows.length; i++)
	{
		if(CancelRequested)
			break;
		
		processRow(rows[i]);
	}
}

function processRow(row)
{
	var service = CreateService("DicomDataStoreService");
	var study = service.QueryStudies(
		{
			AccessionNumber: row.OriginalAccession
		}).Results.first();
		
	if(!study)
	{
		LogInfo(String.Format("No study found with A# {0}... skipping.", row.OriginalAccession));
		return;
	}
	
	LogInfo(String.Format("Anonymizing A# {0}...", row.OriginalAccession));
	
	row.StudyInstanceUID = study.StudyInstanceUID;
	row.OutputDirectory = "D:\\junk\\" + row.OriginalAccession;
	row.StudyDate = study.StudyDate;
	row.StudyDescription =study.StudyDescription;
	
	var anonService = CreateService("DicomAnonymizationService");
	anonService.AnonymizeStudy(row);
		
	LogInfo("Finished A# " + row.OriginalAccession);
}

function readData(filePath)
{
	var fs = File.OpenText(filePath);

	var line;
	var rows = $A();
	while ((line = fs.ReadLine()) != null) 
	{
		var row = $A(line.Split(','));
		rows.add(row);
	}
	
	fs.Dispose();
	
	return rows;
}
