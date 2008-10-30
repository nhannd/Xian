var inputFile = "D:\\junk\\foo.txt";
var outputDir = "D:\\junk";

function main()
{
	// expects a CSV file
	var rows = readData(inputFile).map(
		function(row)
		{
			return {
				OriginalPatientId : row[0],
				OriginalAccession : row[1],
				PatientId: row[2],
				PatientsName: row[3] + "^" + row[4],
				PatientsSex: row[5],
				AccessionNumber: row[7]
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
	
	// set the UID of the study to anonymize
	row.StudyInstanceUID = study.StudyInstanceUID;
	
	// place into output folder named after the new accession number
	row.OutputDirectory = String.Format("{0}\\{1}", outputDir, row.AccessionNumber);
	
	// this information is passed to the anonymizer unchanged
	row.StudyDate = study.StudyDate;
	row.StudyDescription = study.StudyDescription;
	
	var anonService = CreateService("DicomAnonymizationService");
	anonService.AnonymizeStudy(row);
		
	LogInfo("Finished A# " + row.OriginalAccession + ", new A# " + row.AccessionNumber);
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
