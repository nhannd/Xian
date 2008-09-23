/*
    preview
*/

function getAlertHtml(alertItem, patientName)
{
	//return "<img width='50' src='" + getAlertIcon(alertItem) + "' alt='" + getAlertTooltip(alertItem, patientName) + "' align='right'/>";
	return "<img class='alert' src='" + imagePath + "/" + getAlertIcon(alertItem) + "' alt='" + getAlertTooltip(alertItem, patientName) + "'/>";
}

function getAlertIcon(alertItem)
{
	switch (alertItem.AlertClassName)
	{
		case "NoteAlert":
			return "AlertNote.png";
		case "LanguageAlert":
			return "AlertLanguage.png";
		case "ReconciliationAlert":
			return "AlertReconcile.png";
		case "IncompleteDemographicDataAlert":
			return "AlertIncompleteData.png";
		case "InvalidVisitAlert":
			return "AlertVisit.png";
		default:
			return "AlertGeneral.png";
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

function formatProcedureName(procedureType, portable, laterality)
{
	var procedureDecorator = portable ? "Portable" : null;
	if (laterality && laterality.Code != "N")
		procedureDecorator = procedureDecorator ? procedureDecorator + "/" + laterality.Value : laterality.Value;

	procedureDecorator = procedureDecorator ? " (" + procedureDecorator + ")" : "";
	return procedureType.Name + procedureDecorator;
}

function formatProcedureSchedule(scheduledStartTime, schedulingRequestTime)
{
	if (scheduledStartTime)
		return getDescriptiveTime(scheduledStartTime); 
	else if (schedulingRequestTime)
		return "Requested for " + getDescriptiveTime(schedulingRequestTime);
	else
		return "Not scheduled";
}

function formatProcedureStatus(status, scheduledStartTime, startTime, checkInTime, checkOutTime)
{
	if (!scheduledStartTime && !startTime)
		return "Unscheduled";

	if (status.Code == 'SC' && checkInTime)
		return "Checked-In";
	
	if ((status.Code == 'IP' || status.Code == 'CM') && checkOutTime)
		return "Completed";

	return status.Value; 
}

function formatProcedureStartEndTime(startTime, checkOutTime)
{
	if (!startTime)
		return "Not started";

	if (checkOutTime)
		return "Ended " + Ris.formatDateTime(checkOutTime);

	return "Started " + Ris.formatDateTime(startTime);
}

function formatProcedurePerformingStaff(procedure)
{
	var firstMps = procedure.ProcedureSteps.select(function(step) { return step.StepClassName == "ModalityProcedureStep"; }).firstElement();

	if (!firstMps)
		return "";
	else if (firstMps.Performer)
		return Ris.formatPersonName(firstMps.Performer.Name);
	else if (firstMps.ScheduledPerformer)
		return Ris.formatPersonName(firstMps.ScheduledPerformer.Name);
	else
		return "";
}

function formatPerformingFacilityList(procedures)
{
	var facilities = [];
	for(var i = 0; i < procedures.length; i++)
	{
		if (facilities.indexOf(procedures[i].PerformingFacility.Name) < 0)
			facilities.add(procedures[i].PerformingFacility.Name);
	}
	
	return String.combine(facilities, "<br>");
}

function formatVisitCurrentLocation(visit)
{
	if (!visit || !visit.CurrentLocation)
		return null;
	
	if (visit.CurrentLocation.Room || visit.CurrentLocation.Bed)
		return visit.CurrentLocation.Name + ", " + (visit.CurrentLocation.Room || "") + (visit.CurrentLocation.Bed ? "/" + visit.CurrentLocation.Bed : "");
	else
		return visit.CurrentLocation.Name;
}

function filterProcedureByModality(procedures, modalityIdFilter)
{
	var isStepInModality = function (step)
	{
		return modalityIdFilter.find(
			function(id) 
			{
				return step.Modality.Id == id; 
			}) != null;
	}
	
	var isProcedureInModality = function (p)
	{
		var mps = p.ProcedureSteps.select(function(step) { return step.StepClassName == "ModalityProcedureStep"; });
		return mps.find(isStepInModality) != null;
	}

	return procedures.select(isProcedureInModality);
}

function createImagingServiceTable(htmlTable, patientOrderData)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return formatProcedureName(item.ProcedureType, item.ProcedurePortable, item.ProcedureLaterality); }
			},
			{   label: "Schedule",
				cellType: "text",
				getValue: function(item) { return formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime); }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return formatProcedureStatus(item.ProcedureStatus, item.ProcedureScheduledStartTime, item.ProcedureStartTime, item.ProcedureCheckInTime, item.ProcedureCheckOutTime); }
			},
			{   label: "Performing Facility",
				cellType: "text",
				getValue: function(item) { return formatPerformingFacility(item); }
			},
			{   label: "Ordering Physician",
				cellType: "link",
				getValue: function(item)  { return Ris.formatPersonName(item.OrderingPractitioner.Name); },
				clickLink: function(item) { Ris.openPractitionerDetails(item.OrderingPractitioner); }
			}
		 ]);

	function formatPerformingFacility(item)
	{
		 return item.ProcedurePerformingFacility ? item.ProcedurePerformingFacility.Code : "";
	}
	
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(patientOrderData);
}

function createProceduresTable(htmlTable, procedures)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return formatProcedureName(item.Type, item.Portable, item.Laterality); }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return formatProcedureStatus(item.Status, item.ScheduledStartTime, item.StartTime, item.CheckInTime, item.CheckOutTime); }
			},
			{   label: "Schedule",
				cellType: "text",
				getValue: function(item) { return formatProcedureSchedule(item.ScheduledStartTime, null); }
			},
			{   label: "Start/End Time",
				cellType: "text",
				getValue: function(item) { return formatProcedureStartEndTime(item.StartTime, item.CheckOutTime); }
			},
			{   label: "Performing Staff",
				cellType: "text",
				getValue: function(item) { return formatProcedurePerformingStaff(item); }
			}
		 ]);
		
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(procedures);
}

function createProtocolProceduresTable(htmlTable, procedures)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return formatProcedureName(item.Type, item.Portable, item.Laterality); }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return formatProtocolStatus(item.Protocol); }
			},
			{   label: "Protocol",
				cellType: "html",
				getValue: function(item) { return formatProtocolCode(item.Protocol); }
			},
			{   label: "Author",
				cellType: "text",
				getValue: function(item) { return formatProtocolAuthor(item.Protocol); }
			},
			{   label: "WTIS",
				cellType: "text",
				getValue: function(item) { return formatProtocolUrgency(item.Protocol); }
			}
		 ]);
	
	function formatProtocolStatus(protocol)
	{
		if(!protocol)
			return "Not Protocolled";

		if(protocol.Status.Code == "RJ")
			return protocol.Status.Value + " - "+ protocol.RejectReason.Value;

		return protocol.Status.Value; 
	}
	
	function formatProtocolCode(protocol)
	{
		if (!protocol)
			return "";
			
		return String.combine(protocol.Codes.map(function(code) { return code.Name; }), "<br>");
	}
	
	function formatProtocolAuthor(protocol)
	{
		if (!protocol || !protocol.Author)
			return "";
		
		return Ris.formatPersonName(protocol.Author.Name);
	}
	
	function formatProtocolUrgency(protocol)
	{
		if (!protocol)
			return "";
			
		return protocol.Urgency.Value;
	}
	
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(procedures);
}

function createReportingProceduresTable(htmlTable, procedures)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return formatProcedureName(item.Type, item.Portable, item.Laterality); }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return formatProcedureReportingStatus(item); }
			},
			{   label: "Start/End Time",
				cellType: "text",
				getValue: function(item) { return formatProcedureStartEndTime(item.StartTime, item.CheckOutTime); }
			},
			{   label: "Performing Staff",
				cellType: "text",
				getValue: function(item) { return formatProcedurePerformingStaff(item); }
			},
			{   label: "Owner",
				cellType: "text",
				getValue: function(item) { return formatProcedureReportingOwner(item); }
			}
		 ]);

 
	function getActiveReportingStep(procedure)
	{
		var reportingStepNames = ["InterpretationStep", "TranscriptionStep", "VerificationStep", "PublicationStep"];
		var activeStatusCode = ["SC", "IP"];
		var isActiveReportingStep = function(step) { return reportingStepNames.indexOf(step.StepClassName) >= 0 && activeStatusCode.indexOf(step.State.Code) >= 0; };

		return procedure.ProcedureSteps.select(isActiveReportingStep).firstElement();
	}
	
	function getLastCompletedPublicationStep(procedure)
	{
		var isCompletedPublicationStep = function(step) { return step.StepClassName == "PublicationStep" && step.State.Code == "CM"; };
		var compreStepEndTime = function(step1, step2) { return Date.compare(step1.EndTime, step2.EndTime); };
		return procedure.ProcedureSteps.select(isCompletedPublicationStep).sort(compreStepEndTime).reverse().firstElement();
	}
		 
	function formatProcedureReportingStatus(procedure)
	{
		var activeReportingStep = getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;
		var isAddendum = activeReportingStep && lastCompletedPublicationStep;

		var stepName = lastStep.ProcedureStepName;
		var addendumPrefix = isAddendum ? "Addendum " : "";
		
		var formattedStatus;
		switch(lastStep.State.Code)
		{
			case "SC": formattedStatus = "Pending " + stepName;
				if (stepName == "Verification")
					formattedStatus = "To Be Revised";
					
				break;
			case "IP": formattedStatus = stepName + " In Progress"; 
				if (stepName == "Verification")
					formattedStatus = "Revising";

				break;
			case "SU": formattedStatus = stepName + " Suspended"; break;
			case "CM": formattedStatus = stepName + " Completed";
				// Exceptions to formatting
				if (stepName == "Verification")
					formattedStatus = "Verified";
				else if (stepName == "Publication")
					formattedStatus = "Published";
					
				break;
				
			case "DC": formattedStatus = stepName + " Cancelled"; break;
			default: break;
		}
		
		return addendumPrefix + formattedStatus;
	}
	
	function formatProcedureReportingOwner(procedure)
	{
		var activeReportingStep = getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;

		if (!lastStep)
			return "";

		if (lastStep.Performer)
			return Ris.formatPersonName(lastStep.Performer.Name)
		
		if (lastStep.ScheduledPerformer)
			return Ris.formatPersonName(lastStep.ScheduledPerformer.Name);
			
		return "";
	}
		 
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(procedures);
}

function createReportListTable(htmlTable, reportList, onLoadReport)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, autoSelectFirstElement: true },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return formatProcedureName(item.ProcedureType, item.ProcedurePortable, item.ProcedureLaterality); }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return item.ReportStatus.Value; }
			}
		 ]);
	
	htmlTable.onRowClick = function(sender, args)
		{
			onLoadReport(args.item);
		};
		 
	htmlTable.mouseOverClassName = "mouseover";
	htmlTable.highlightClassName = "highlight";
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(reportList);
}

function createOrderNotesTable(htmlTable, notes, categoryFilter)
{
	var filteredNotes = categoryFilter ? notes.select(function(note) { return note.Category == categoryFilter; }) : notes;

	if (filteredNotes.length == 0)
	{
		Field.show(htmlTable, false);
	}
	
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Comment",
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
					var from = Ris.formatStaffNameAndRole(item.Author);
					
					if(item.OnBehalfOfGroup != null)
						from = from + " on behalf of " + item.OnBehalfOfGroup.Name;
					
					return from;
				}
			}
		 ]);

	htmlTable.rowCycleClassNames = ["row1", "row0"];
	htmlTable.bindItems(filteredNotes);
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

	function parseReportObject(reportJsml)
	{
		try
		{
			return JSML.parse(reportJsml);
		}
		catch(e)
		{
			return null;
		}
	}
	
	function parseReportContent(reportJsml)
	{
		var reportObject = parseReportObject(reportJsml);
		if (!reportObject)
		{
			// the Content was not JSML, but just plain text
			return reportJsml || "None";
		}
		
		// depending on how the report was captured, it may contain an Impression and Finding section (Default RIS report editor)
		var reportText;
		if(reportObject.Impression || reportObject.Finding)
			reportText = "<B>Impression:</B> " + reportObject.Impression + "<br>" + "<B>Finding:</B> " + reportObject.Finding + "<br>";
		else // or it may simply contain a ReportText section (UHN report editor)
			reportText = reportObject.ReportText;

		if (!reportText)
			reportText = "None";

		return reportText.replaceLineBreak();
	}
	
	function formatReportStatus(report, isAddendum)
	{
		var statusMap = {X: 'Cancelled', D: 'Draft', P: 'Preliminary', F: 'Final'};
		var timePropertyMap = {X: 'CancelledTime', D: 'CreationTime', P: 'PreliminaryTime', F: 'CompletedTime'};
		var timeText = Ris.formatDateTime(report[timePropertyMap[report.Status.Code]]);
		var warningText = " *** THIS " + (isAddendum ? "ADDENDUM" : "REPORT") + " HAS NOT BEEN VERIFIED ***";

		var statusText = statusMap[report.Status.Code] + " - " + timeText;

		if (['D', 'P'].indexOf(report.Status.Code) > -1)
			statusText = "<font color='red'>" + statusText + warningText + "</font>";

		return " (" + statusText + ")";
	}
	
	var formattedReport = "<br>";
	if (report.Parts.length > 1)
	{
		for (var i = report.Parts.length-1; i > 0; i--)
		{
			var addendumPart = report.Parts[i];
			var addendumContent = addendumPart && addendumPart.ExtendedProperties && addendumPart.ExtendedProperties.ReportContent ? addendumPart.ExtendedProperties.ReportContent : "";
			formattedReport += "<b>Addendum " + formatReportStatus(addendumPart, true) + ": </b><br><br>";
			formattedReport += parseReportContent(addendumContent);
			formattedReport += formatReportPerformer(addendumPart);
			formattedReport += "<br><br>";
		}
	}

	var part0 = report.Parts[0];
	var reportContent = part0 && part0.ExtendedProperties && part0.ExtendedProperties.ReportContent ? part0.ExtendedProperties.ReportContent : "";
	formattedReport += "<b>Report" + formatReportStatus(part0) + "</b>";
	formattedReport += "<div id=\"structuredReport\" style=\"{color:black;margin-bottom:1em;}\"></div>";
	formattedReport += "<div class=\"sectionheading\" id=\"radiologistsCommentsHeader\" style=\"{"; 
	formattedReport += "display:none;margin-bottom:1em;}\">Radiologist's Comments</div>";
	formattedReport += parseReportContent(reportContent);
	formattedReport += formatReportPerformer(part0);
	
	element.innerHTML = formattedReport;
	
	 // UHN report may contain a StructuredReport section
	var mainReport = parseReportObject(reportContent);
	if(mainReport && mainReport.StructuredReport)
		createStructuredReportPreview(mainReport.StructuredReport.data);
}

function createStructuredReportPreview(structuredReport)
{
	if(!structuredReport)
		return;

	Field.show($("radiologistsCommentsHeader"), true);
	StructuredReportPreview.create(structuredReport, $("structuredReport"));
}

function formatReportPerformer(reportPart)
{
	if (reportPart == null)
		return "";

	var formattedReport = "<br>";
	
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
	if (dateOfBirth == null)
		return "Unknown";
		
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

function getPatientAgeInYears(dateOfBirth, deathIndicator, timeOfDeath)
{
	if (dateOfBirth == null)
		return 0;
		
	var endDate = (deathIndicator == true ? timeOfDeath : new Date());

	//Define a variable to hold the anniversary of theBirthdate in the endDate year
	var theBirthdateThisYear = new Date(endDate);
	theBirthdateThisYear.setDate(dateOfBirth.getDate());
	theBirthdateThisYear.setMonth(dateOfBirth.getMonth());

	// calculate the age at endDate
	var age = endDate.getFullYear() - dateOfBirth.getFullYear();
	if (endDate < theBirthdateThisYear) 
		age--;

	return age;
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
