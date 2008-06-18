/*
    preview
*/

function getAlertHtml(alertItem, patientName)
{
	//return "<img width='50' src='" + getAlertIcon(alertItem) + "' alt='" + getAlertTooltip(alertItem, patientName) + "' align='right'/>";
	return "<img class='alert' src='" + getAlertIcon(alertItem) + "' alt='" + getAlertTooltip(alertItem, patientName) + "'/>";
}

function getAlertIcon(alertItem)
{
	switch (alertItem.AlertClassName)
	{
		case "NoteAlert":
			return "Images/AlertNote.png";
		case "LanguageAlert":
			return "Images/AlertLanguage.png";
		case "ReconciliationAlert":
			return "Images/AlertReconcile.png";
		case "IncompleteDemographicDataAlert":
			return "Images/AlertIncompleteData.png";
		case "InvalidVisitAlert":
			return "Images/AlertVisit.png";
		default:
			return "Images/AlertGeneral.png";
	}
}

function getAlertTooltip(alertItem, patientName)
{
	var reasons = String.combine(alertItem.Reasons, ", ");

	switch (alertItem.AlertClassName)
	{
		case "NoteAlert":
			return patientName + " has high severity notes: " + reasons;
		case "LanguageAlert":
			return patientName + " speaks: " + reasons;
		case "ReconciliationAlert":
			return patientName + " has unreconciled records";
		case "IncompleteDemographicDataAlert":
			return patientName + " has incomplete demographic data: " + reasons;
		case "InvalidVisitAlert":
			return "This order has invalid visit: " + reasons;
		default:
			return reasons;
	}
}

function getDescriptiveTime(dateTime)
{
	if (dateTime == null)
		return "";
	
	var today = Date.today();
	var yesterday = today.addDays(-1);
	var tomorrow = today.addDays(1);
	var afterTomorrow = tomorrow.addDays(1);

	if (Date.compare(dateTime, yesterday) < 0)
	{
		var dayDiff = Math.ceil((Date.parse(today) - Date.parse(dateTime))/(1000*60*60*24));

		if (dayDiff < 31)
		{
			return dayDiff + " days ago";
		}
		else 
		{
			return Ris.formatDate(dateTime);
		}
		
	}
	else if (Date.compare(dateTime, yesterday) >= 0 && Date.compare(dateTime, today) < 0)
	{
		return "Yesterday " + Ris.formatTime(dateTime);
	}
	else if (Date.compare(dateTime, today) >= 0 && Date.compare(dateTime, tomorrow) < 0)
	{
		return "Today " + Ris.formatTime(dateTime);                
	}
	else if (Date.compare(dateTime, tomorrow) >= 0 && Date.compare(dateTime, afterTomorrow) < 0)
	{
		return "Tomorrow " + Ris.formatTime(dateTime);
	}
	else
	{
		return Ris.formatDateTime(dateTime);                
	}
}

function createImagingRequestsTable(htmlTable)
{
	var ordersTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return item.ProcedureType.Name; }
			},
			{   label: "Schedule",
				cellType: "text",
				getValue: function(item) 
				{ 
					return item.ProcedureScheduledStartTime == null 
					? "Requested for " + getDescriptiveTime(item.SchedulingRequestTime) 
					: getDescriptiveTime(item.ProcedureScheduledStartTime); 
				}
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) 
				{ 
					if (item.ProcedureScheduledStartTime == null)
						return "Unscheduled";

					if (item.ProcedureStatus.Code == 'SC' && item.ProcedureCheckInTime != null)
						return "Checked-In";
					
					if ((item.ProcedureStatus.Code == 'IP' || item.ProcedureStatus.Code == 'CM') 
						&& item.ProcedureCheckOutTime != null)
						return "Completed";

					return  item.ProcedureStatus.Value; 
				}
			},
			{   label: "Insurance",
				cellType: "text",
				getValue: function(item) { return ""; }
			},
			{   label: "Ordering Facility",
				cellType: "text",
				getValue: function(item) { return item.OrderingFacility.Code; }
			},
			{   label: "Ordering Physician",
				cellType: "html",
				getValue: function(item) 
				{ 
					// return "<a href=\"javascript:Ris.openPractitionerDetails('" + JSML.create(item.OrderingPractitioner, "Practitioner") + "')\">" 
						// + Ris.formatPersonName(item.OrderingPractitioner.Name)
						// + "</a>"; 
					return Ris.formatPersonName(item.OrderingPractitioner.Name);
				}
			},
			{
				label: "Ordering Physician Lookup",
				cellType: "html",
				getValue: function(item)
				{
					return "<a href=\"javascript:Ris.openPractitionerDetails('" + JSML.create(item.OrderingPractitioner, "Practitioner") + "')\">" 
						+ "<img src='images/PractitionerDetail.png' border=\"0\"/>"
						+ "</a>"; 
				}
			}
		 ]);
		 
	ordersTable.rowCycleClassNames = ["row1", "row0"];
	
	return ordersTable;
}

function createDiagnosticServiceBreakdownTable(htmlTable)
{
	var dsTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Diagnostic Service",
				cellType: "text",
				getValue: function(item) { return item.DiagnosticService; }
			},
			{   label: "Requested Procedure",
				cellType: "text",
				getValue: function(item) { return item.Procedure; }
			},
			{   label: "Step",
				cellType: "text",
				getValue: function(item) { return item.Step; }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return item.State; }
			}
		 ]);
		 
	dsTable.rowCycleClassNames = ["row0", "row1"];
	dsTable.renderRow = function(sender, args)
	{
		if(args.item.Active)
			args.htmlRow.className = "highlight";
	};
	
	return dsTable;
}

function createProceduresTable(htmlTable, procedures)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return item.Type.Name; }
			},
			{   label: "Protocol Status",
				cellType: "html",
				getValue: function(item) { return item.Protocol ? item.Protocol.Status.Value : "Not Protocolled"; }
			},
			{   label: "Protocol Codes",
				cellType: "html",
				getValue: function(item) { return item.Protocol ? String.combine(item.Protocol.Codes.map(function(code) { return code.Name; }), "<br>") : ""; }
			}
		 ]);
		 
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(procedures);
}

function createOrderNotesTable(htmlTable, notes)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Note",
				cellType: "readonly",
				getValue: function(item) { return item.NoteBody; }
			},
			{   label: "Time",
				cellType: "text",
				getValue: function(item) { return Ris.formatDateTime(item.CreationTime); }
			},
			{   label: "Author",
				cellType: "text",
				getValue: function(item) 
				{ 
					var from = Ris.formatPersonName(item.Author.Name);
					
					if(item.OnBehalfOfGroup != null)
						from = from + " on behalf of " + item.OnBehalfOfGroup.Name;
					
					return from;
				}
			}
		 ]);
		 
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(notes);
}


function orderRequestScheduledDateComparison(data1, data2)
{
	return Date.compareMoreRecent(data1.SchedulingRequestTime, data2.SchedulingRequestTime);
}

function procedureScheduledDateComparison(data1, data2)
{
	return Date.compareMoreRecent(data1.ProcedureScheduledStartTime, data2.ProcedureScheduledStartTime);
}

// group patientOrderData by AccessionNumber
function groupDataToOrders(listData)
{
	var orders = [];
	for(var i = 0; i < listData.length; i++)
	{
	    var thisAccessionNumber = listData[i].AccessionNumber;
		var thisOrder = orders.find(function(order) { return order.AccessionNumber == thisAccessionNumber; });

		if (thisOrder)
		{
			thisOrder.values.push(listData[i]);
		}
		else
		{
			thisOrder = { key: thisAccessionNumber, values:[] };
			thisOrder.values.push(listData[i]);
			orders.push(thisOrder);
		}
	}

	// set the common properties only at the end, since some properties in setCommonPropertiesFunc may be a composite value of each element
	for (var j = 0; j < orders.length; j++)
	{
		var thisOrder = orders[j];
		var firstData = thisOrder.values[0];

		var listProcedureName = thisOrder.values.map(function(item) { return item.ProcedureName; });

		thisOrder.AccessionNumber = firstData.AccessionNumber;
		thisOrder.CombineProcedureName = String.combine(listProcedureName, "/");
		thisOrder.OrderScheduledStartTime = firstData.OrderScheduledStartTime;
		thisOrder.OrderStatus = firstData.OrderStatus;
		thisOrder.Insurance = "";
		thisOrder.OrderingFacility = firstData.OrderingFacility;
		thisOrder.OrderingPractitioner = firstData.OrderingPractitioner;
	}

    return orders;
}

function createReportPreview(element, report)
{
	if (element == null || report == null || report.Parts == null || report.Parts.length == 0)
		return "";

	var statusMap = {X: 'Cancelled', D: 'Draft', P: 'Preliminary', F: 'Final'};
	var formattedReport = "";

	if (report.Parts.length > 1)
	{
		for (var i = report.Parts.length-1; i > 0; i--)
		{
			var addendumPart = report.Parts[i];
			var addendumStatus = addendumPart.Status.Code;
			var addendumContent = addendumPart && addendumPart.ExtendedProperties && addendumPart.ExtendedProperties.ReportContent ? addendumPart.ExtendedProperties.ReportContent : "";
			
			if (addendumContent)
			{
				formattedReport += "<b>Addendum " + i + " (" + statusMap[addendumStatus] + "): </b><br>";
				formattedReport += addendumContent.replaceLineBreak();
				formattedReport += formatReportPerformer(addendumPart);

				if (['D', 'P'].indexOf(addendumStatus) > -1)
					formattedReport = "<font color='red'>" + formattedReport + "</font>";

				formattedReport += "<br><br>";
			}
		}

		if (formattedReport)
			formattedReport = "<h3>Addendum:</h3>" + formattedReport;
	}

	var mainReportText = "";
	var reportContent = report.Parts[0] && report.Parts[0].ExtendedProperties && report.Parts[0].ExtendedProperties.ReportContent ? report.Parts[0].ExtendedProperties.ReportContent : "";
	try
	{
		var mainReport = JSML.parse(reportContent);

		// depending on how the report was captured, it may contain an Impression and Finding section (Default RIS report editor)
		if(mainReport.Impression || mainReport.Finding)
		{
			mainReportText = "<B>Impression:</B> " + mainReport.Impression + "<br>" + "<B>Finding:</B> " + mainReport.Finding + "<br>";
		}
		else
		{
			// or it may simply contain a ReportText section (UHN report editor)
			mainReportText = mainReport.ReportText;
		}
	}
	catch(e)
	{
		// the Content was not JSML, but just plain text
		mainReportText = reportContent;
		if (mainReportText == null)
			mainReportText = "None";
	}

	var statusCode = report.Parts[0].Status.Code;

	formattedReport += "<h3>";
	formattedReport += "Main Report";
	formattedReport += " (" + statusMap[statusCode] + ")";
	formattedReport += "</h3>";
	formattedReport += "<div id=\"structuredReport\" style=\"{margin-bottom:1em;}\"></div>";
	if(mainReportText)
	{
		formattedReport += mainReportText.replaceLineBreak();
	}

	formattedReport += formatReportPerformer(report.Parts[0]);

	if(['D', 'P'].indexOf(statusCode) > -1)
		formattedReport = "<font color='red'>" + formattedReport + "</font>";
	element.innerHTML = formattedReport;
	 
	 // UHN report may contain a StructuredReport section
	if(mainReport && mainReport.StructuredReport)
	{
		createStructuredReportPreview(mainReport.StructuredReport.data);
	}
}

function createStructuredReportPreview(structuredReport)
{
	if(!structuredReport)
		return;

	StructuredReportPreview.create(structuredReport, $("structuredReport"));
}

function formatReportPerformer(reportPart)
{
	if (reportPart == null)
		return "";

	var formattedReport = "";
	
	if (reportPart.InterpretedBy)
		formattedReport += "<br> Interpreted By: " + Ris.formatPersonName(reportPart.InterpretedBy.Name);

	if (reportPart.TranscribedBy)
		formattedReport += "<br> Transcribed By: " + Ris.formatPersonName(reportPart.TranscribedBy.Name);

	if (reportPart.VerifiedBy)
		formattedReport += "<br> Verified By: " + Ris.formatPersonName(reportPart.VerifiedBy.Name);

	return formattedReport;
}

/*
// Useful test code for calculating patient age
function testPatientAge()
{
	var threeYearsAgoToday = new Date();
	var testDate = new Date(threeYearsAgoToday);
	threeYearsAgoToday.setYear(threeYearsAgoToday.getFullYear() - 3);

	var testString = "";
	
	while (testDate >= threeYearsAgoToday)
	{
		testString += testDate.getFullYear() + "-" + eval(testDate.getMonth()+1) + "-" + testDate.getDate() + " is " + getPatientAge(testDate, false, false);
		testDate.setDate(testDate.getDate() - 1);
	}
	
	return testString;
}
*/

function getPatientAge(dateOfBirth, deathIndicator, timeOfDeath)
{
	var endDate = (deathIndicator == true ? timeOfDeath : new Date());

	//Define a variable to hold the anniversary of theBirthdate in the endDate year
	var theBirthdateThisYear = new Date(endDate);
	theBirthdateThisYear.setDate(dateOfBirth.getDate());
	theBirthdateThisYear.setMonth(dateOfBirth.getMonth());

	// calculate the age at endDate
	var age = endDate.getFullYear() - dateOfBirth.getFullYear();
	if (endDate < theBirthdateThisYear) 
		age--;

	var ageString = age;
	if (age >= 2)
		ageString = age;
	else
	{
		// display number of month if less than 2 years old
		// of number of days if less than or equal to 31 days
		var days = Math.floor((endDate - dateOfBirth)/(1000*60*60*24));
		if (days < 0)
			return "undefined";
		else if (days < 1)
			ageString = "0";
		else if (days <= 31)
			ageString = days + " days";
		else
		{
			// Calculate the number of month
			var yearDiff = endDate.getFullYear() - dateOfBirth.getFullYear();
			var month = endDate.getMonth() - dateOfBirth.getMonth();
			if (endDate.getDate() < dateOfBirth.getDate())
				month -= 1;

			// no month should be negative
			month += (month < 0 ? 12 : 0);
			
			// add 12 month if already 1 year old
			month += (age == 1 ? 12 : 0);

			// special case for exactly 12 month 
			month += (month == 0 && yearDiff == 0 ? 12 : 0);

			ageString = month + " months";	
		}
	}

	if (deathIndicator == true)
		ageString += " (deceased)";
		
	return ageString;
}

function isProcedureStatusActive(procedureStatus)
{
	return procedureStatus.Code == "SC" || 
			procedureStatus.Code == "IP";
}

function getActiveProcedures(patientOrderData)
{
	var today = Date.today();

	var presentScheduledProcedures = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime &&
					Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
					isProcedureStatusActive(item.ProcedureStatus);
		}).sort(procedureScheduledDateComparison);

	var presentNotScheduledProceduress = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime == null &&
					item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
					isProcedureStatusActive(item.ProcedureStatus);
		}).sort(orderRequestScheduledDateComparison);
		
	return presentScheduledProcedures .concat(presentNotScheduledProceduress);
}

function getNonActiveProcedures(patientOrderData)
{

	var today = Date.today();

	// List only the non-Active present procedures
	var presentScheduledProcedures = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
					isProcedureStatusActive(item.ProcedureStatus) == false;
		}).sort(procedureScheduledDateComparison);

	// List only the non-Active present not-scheduled procedures
	var presentNotScheduledProceduress = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime == null &&
					item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
					isProcedureStatusActive(item.ProcedureStatus) == false;
		}).sort(orderRequestScheduledDateComparison);

	var pastScheduledProcedures = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) < 0;
		}).sort(procedureScheduledDateComparison);

	var pastNotScheduledProceduress = patientOrderData.select(
		function(item) 
		{ 
			return item.ProcedureScheduledStartTime == null
			&& item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) < 0;
		}).sort(orderRequestScheduledDateComparison);

	return presentScheduledProcedures.concat(
			presentNotScheduledProceduress.concat(
			pastScheduledProcedures.concat(pastNotScheduledProceduress)));
}