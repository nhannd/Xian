/*
	This file contains a server-side Oto script that anonymizes a RIS database.
	The contents of main() will need to be adjusted appropriately prior to execution.
*/

var Healthcare;	// Healthcare model
var store;	// persistent store
var mrnSequence;
var visitNumberSequence;
var accessionSequence;
var nameSequence;
var staffIdSequence;
var billingNumberSequence;
var licenseNumberSequence;

function main()
{
	// initialize the dummy data sources
	mrnSequence = createIdentifierSequence(5000000);
	visitNumberSequence = createIdentifierSequence(400000000);
	accessionSequence = createIdentifierSequence(700000000);
	staffIdSequence = createIdentifierSequence(100000);
	billingNumberSequence = createIdentifierSequence(1000000);
	licenseNumberSequence = createIdentifierSequence(1000000);
	nameSequence = createPersonNameSequence(
		readLines("D:\\junk\\family_names.txt"),
		readLines("D:\\junk\\male_names.txt"),
		readLines("D:\\junk\\female_names.txt"));
	
	// init Healthcare model
	initModel();
	
	//anonymizePatients("D:\\junk\\patients.txt", true);
	//anonymizeStaff("D:\\junk\\staff.txt", true);
	anonymizePractitioner("D:\\junk\\practitioners.txt", true);	
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
// Anonymization routines

function anonymizePatients(outputFile, dryRun)
{
	processInputSequence(
		{
			inputSequence: createInputSequence(Healthcare.PatientProfile),
			processFunc: function(item)
			{
				return processPatient(item);
			},
			outputFileName: outputFile,
			dryRun: true
		}
	);
}

function anonymizeStaff(outputFile, dryRun)
{
	processInputSequence(
		{
			inputSequence: createInputSequence(Healthcare.Staff),
			processFunc: function(item)
			{
				return processStaff(item);
			},
			outputFileName: outputFile,
			dryRun: dryRun
		}
	);
}

function anonymizePractitioner(outputFile, dryRun)
{
	processInputSequence(
		{
			inputSequence: createInputSequence(Healthcare.ExternalPractitioner),
			processFunc: function(item)
			{
				return processPractitioner(item);
			},
			outputFileName: outputFile,
			dryRun: dryRun
		}
	);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////
// Anonymization processors
function processPatient(profile)
{
	var results = $A();
	
	// remember the original mrn and name
	var originalMrn = profile.Mrn.Id;
	var originalName = profile.Name;
	
	// process the patient profile
	processProfile(profile);
	
	// process all visits associated with patient
	var visitQuery = Healthcare.Visit.NewQuery();
	visitQuery.Where.Patient.EqualTo(profile.Patient);
	visitQuery.OrderBy.VisitNumber.Id.SortAsc(0);
	$A(visitQuery.All()).each(function(v) { processVisit(v); });
	
	// process all orders/reports associated with patient
	var orderQuery = Healthcare.Order.NewQuery();
	orderQuery.Where.Patient.EqualTo(profile.Patient);
	orderQuery.OrderBy.AccessionNumber.SortAsc(0);
	$A(orderQuery.All()).each(
		function(o)
		{
			var originalAccession = o.AccessionNumber;	// remember the original accession number
			processOrder(o);

			// for each order, add a row to the results vector
			results.add([
				originalMrn,
				originalAccession,
				profile.Mrn.Id,
				profile.Name.FamilyName,
				profile.Name.GivenName,
				profile.Sex,
				profile.DateOfBirth.ToString("yyyy-MM-dd"),
				o.AccessionNumber
			]);
			
			// obtain unique set of reports associated with this order
			var reports = $A(o.Procedures)
							.reduce($A(), function(m, p) { return $A(m.concat($A(p.Reports))); })
							.unique();
							
			// process reports
			
			// create a map of text substitutions to be applied to the report text
			var substitutions = {};
			substitutions[originalMrn] = profile.Mrn.Id;
			substitutions[originalAccession] = o.AccessionNumber;
			substitutions[originalName.FamilyName] = profile.Name.FamilyName;
			substitutions[originalName.GivenName] = profile.Name.GivenName;
			
			// apply substitutions to each report
			reports.each(function(r) { processReport(r, substitutions); });
			
		});
	return results;	
}

function processProfile(profile)
{
	LogInfo("Processing patient " + profile.Mrn);
	
	profile.Mrn.Id = mrnSequence.next();
	profile.Name = nameSequence.next(profile.Sex);
	profile.Healthcard = null;	// blank out healthcard info

	// clear all addresses, phone #s, etc
	profile.Addresses.Clear();
	profile.TelephoneNumbers.Clear();
	profile.EmailAddresses.Clear();
	profile.ContactPersons.Clear();
}

function processVisit(visit)
{
	LogInfo("Processing visit " + visit.VisitNumber);
	visit.VisitNumber.Id = visitNumberSequence.next();
	visit.ExtendedProperties.Clear();	// remove insurance information
}

function processOrder(order)
{
	LogInfo("Processing order " + order.AccessionNumber);
	order.AccessionNumber = accessionSequence.next();
	order.Notes.Clear();	// remove all order notes since they may contain sensitive information
}

function processReport(report, substitutions)
{
	$A(report.Parts).each(
		function(part)
		{
			if(part.ExtendedProperties.ContainsKey("ReportContent"))
			{
				var s = part.ExtendedProperties["ReportContent"];
				for(var k in substitutions)
				{
					s.replace(k, substitutions[k]);
				}
				
				part.ExtendedProperties["ReportContent"] = s;
			}
		});
}

function processStaff(staff)
{
	LogInfo("Processing staff " + staff.Id);
	
	var originalId = staff.Id;
	var originalName = staff.Name;
	
	staff.Id = staffIdSequence.next();
	staff.Name = nameSequence.next(staff.Sex);

	// clear all addresses, phone #s, etc
	staff.Addresses.Clear();
	staff.TelephoneNumbers.Clear();
	staff.EmailAddresses.Clear();
	staff.ExtendedProperties.Clear();
	
	return $A([ [originalId, staff.Id, staff.Name.FamilyName, staff.Name.GivenName] ]);
}

function processPractitioner(prac)
{
	LogInfo("Processing practitioner " + prac.Name);
	
	// don't need to anonymize the names or clear the contact points, since all that is public knowledge in any case
	
	// anonymize the license and billing numbers
	var originalLicense = prac.LicenseNumber;
	var originalBilling = prac.BillingNumber;
	prac.LicenseNumber = licenseNumberSequence.next();
	prac.BillingNumber = billingNumberSequence.next();

	// clear HMRI number, etc.
	prac.ExtendedProperties.Clear();
	
	return $A([ [originalLicense, originalBilling, prac.LicenseNumber, prac.BillingNumber, prac.Name.FamilyName, prac.Name.GivenName] ]);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helpers

/* Creates an object that represents a sequence of input items for processing
	queryProvider - any object that supports the method NewQuery() and returns an EntityQuery object.
		
	returns - an object with a .next() method that can be called successively to return the next item.  The sequence
		terminates when next() returns null.
*/
function createInputSequence(queryProvider)
{
	var memo = null;
	return {
		next : function()
		{
			var query = queryProvider.NewQuery();
			query.OrderBy.OID.SortAsc(0);
			if(memo)
				query.Where.OID.MoreThan(memo);
			
			try
			{
				var item = query.First();
				memo = item.OID;
				return item;
			}
			catch(e)
			{
				// query returned no results - end of sequence
				return null;
			}
		}
	};
}

/*	Processes an input sequence created by the createInputSequence() function.
		args - an object with the following properties:
			inputSequence - an input sequence
			processFunc - a callback that is passed each item for processing. Must have the form
				function process(item)
				{
					// process the item
					return rows;
				}
				
				The function may optionally return an array where each element is an array representing a processing result.
				These rows will be written to the output file in CSV format.
			outputFileName - the path to the file to which the CSV output should be written.
			dryRun - optional boolean flag - if true, indicates that no changes should be written to the DB - useful for developement
*/
function processInputSequence(args)
{
	// create an output file where the anonymized relationships will be logged in CSV format
	var out = (args.outputFileName) ? File.CreateText(args.outputFileName) : null;
	
	try
	{
		for(var more = true; more && !CancelRequested; )
		{
			// if dryRun is true, use a read -scope so we don't actually modify the DB
			var persistenceScopeFunc = args.dryRun ? store.Read : store.Update;
		
			persistenceScopeFunc(
				function()
				{
					var item = args.inputSequence.next();
					if(!item)
					{
						more = false;
						return;
					}
					
					
					// process the item
					var rows = args.processFunc(item);
					
					// write the rows to the output file
					if(out && rows) 
						writeToStream(rows, out);
				});
		}
	}
	finally
	{
		if(out)
		{
			out.Flush();
			out.Dispose();
		}
	}
}

/*	Writes the rows array to the specified output stream.
		rows - an array of arrays, where each element array is a line in the CSV file
		out - a .NET StreamWriter object
*/
function writeToStream(rows, out)
{
	rows.map(function(row) { return row.join(","); }).each(
			function(line)
			{
				LogInfo(line);
				out.WriteLine(line);
			});
}

/*	Reads the entire contents of a text file.
		filePath - path of the file to read.
		returns - an array of the lines in the file
*/
function readLines(filePath)
{
	var fs = File.OpenText(filePath);

	var line;
	var lines = $A();
	while ((line = fs.ReadLine()) != null) 
	{
		lines.add(line);
	}
	
	fs.Dispose();
	
	return lines;
}

/*	Creates a sequence object that generates an integer seqence.
		startValue - an integer that is the first value in the sequence.
		returns - an object with a .next() method that can be called successively to return the next item.
*/
function createIdentifierSequence(startValue)
{
	var i = startValue;
	return {
		next: function()
		{
			return (i++).toString();
		}
	};
}

/*	Creates a sequence object that generates random names.  This is not a true sequence, since it is entirely random.
		familyNames - an array of family names
		maleNames - an array of male first names
		femaleNames - an array of female first names
		
		returns - an object with a .next(sex) method that can be called successively to return the next item as 
			a function of sex, where sex is either "M" or "F". A call to .next(...) returns a Healthcare.PersonName object.
*/
function createPersonNameSequence(familyNames, maleNames, femaleNames)
{
	function chooseRandom(items)
	{
		return items.length > 0 ? items[Random.Next(items.length)] : null;
	}

	return {
		next: function(sex)
		{
			var name = Healthcare.PersonName.New();
			name.FamilyName = chooseRandom(familyNames);
			name.GivenName = (sex == "M") ? chooseRandom(maleNames) : chooseRandom(femaleNames);
			return name;
		}
	};
}

/*	Initializes the Healthcare model.
*/
function initModel()
{
	// create the model
	var modelService = CreateService("PersistentDomainModelService");
	var response = modelService.CreateModel({ ModelName: "Healthcare"});
	Healthcare = response.Model;
	store = response.Store;
}

