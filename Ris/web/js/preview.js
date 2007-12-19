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
	switch (alertItem.Type)
	{
		case "Note Alert":
			return "Images/AlertPen.png";
		case "Language Alert":
			return "Images/AlertWorld.png";
		case "Reconciliation Alert":
			return "Images/AlertMessenger.png";
		case "Incomplete Demographic Data Alert":
			return "Images/AlertIncompleteData.png";
		case "Visit Alert":
			return "Images/AlertClock.png";
		default:
			return "Images/AlertGeneral.png";
	}
}

function getAlertTooltip(alertItem, patientName)
{
	var reasons = String.combine(alertItem.Reasons, ", ");

	switch (alertItem.Type)
	{
		case "Note Alert":
			return patientName + " has high severity notes: " + reasons;
		case "Language Alert":
			return patientName + " speaks: " + reasons;
		case "Reconciliation Alert":
			return patientName + " has unreconciled records";
		case "Incomplete Demographic Data Alert":
			return patientName + " has incomplete demographic data: " + reasons;
		case "Visit Alert":
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

function createProceduresTable(htmlTable)
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
				cellType: "text",
				getValue: function(item) { return Ris.formatPersonName(item.OrderingPractitioner.Name); }
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

function createRequestedProceduresTable(htmlTable, procedures)
{
	htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Procedure",
				cellType: "text",
				getValue: function(item) { return item.Type.Name; }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return item.Status.Value; }
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
			{   label: "Comment",
				cellType: "text",
				getValue: function(item) { return item.Comment; }
			},
			{   label: "Time",
				cellType: "text",
				getValue: function(item) { return Ris.formatDateTime(item.CreationTime); }
			},
			{   label: "Author",
				cellType: "text",
				getValue: function(item) { return Ris.formatPersonName(item.Author.Name); }
			}
		 ]);
		 
	htmlTable.rowCycleClassNames = ["row0", "row1"];
	htmlTable.bindItems(notes);
}


function orderRequestScheduledDateComparison(data1, data2)
{
	return Date.compareMoreRecent(data1.SchedulingRequestDateTime, data2.SchedulingRequestDateTime);
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

        var listRequestedProcedureName = thisOrder.values.map(function(item) { return item.ProcedureName; });
        
        thisOrder.AccessionNumber = firstData.AccessionNumber;
        thisOrder.CombineRequestedProcedureName = String.combine(listRequestedProcedureName, "/");
        thisOrder.OrderScheduledStartTime = firstData.OrderScheduledStartTime;
        thisOrder.OrderStatus = firstData.OrderStatus;
        thisOrder.Insurance = "";
        thisOrder.OrderingFacility = firstData.OrderingFacility;
        thisOrder.OrderingPractitioner = firstData.OrderingPractitioner;
	}

    return orders;
}

function formatReport(report)
{
    if (report == null || report.Parts == null || report.Parts.length == 0)
        return "";
        
    var formattedReport = "";
 
    if (report.Parts.length > 1)
    {
        for (var i = report.Parts.length-1; i > 0; i--)
        {
            var addendumPart = report.Parts[i];
            var addendumContent = addendumPart && addendumPart.Content ? addendumPart.Content : "";
            
            if (addendumContent)
            {
                var isAddendumDraft = new Boolean(addendumPart.Status.Code == 'P');
                var isCancelled = new Boolean(addendumPart.Status.Code == 'X');

				if (isAddendumDraft == true)
					formattedReport += "<font color='red'><b>Addendum " + i + " (Draft): </b><br>";
				else if (isCancelled == true)
					formattedReport += "<b>Addendum " + i + " (cancelled): </b><br>";
				else
					formattedReport += "<b>Addendum " + i + ": </b><br>";

                formattedReport += addendumContent;
			
				formattedReport += formatReportPerformer(addendumPart);

				if (isAddendumDraft == true)
					formattedReport += "</font>";

				formattedReport += "<br><br>";
            }
        }

        if (formattedReport)
            formattedReport = "<h3>Addendum:</h3>" + formattedReport;
    }
        
    var mainReport = JSML.parse(report.Parts[0].Content);
	if (mainReport)
	{
	    var isDraft = new Boolean(report.Parts[0].Status.Code == 'P');
	   
	    formattedReport += isDraft == true ? "<font color='red'>" : ""; 
	    formattedReport += "<h3>";
	    formattedReport += "Main Report";
	    formattedReport += isDraft == true ? " (Draft)" : "";
	    formattedReport += "</h3>";
	    formattedReport += "<B>Impression:</B> " + mainReport.Impression + "<br>";    
	    formattedReport += "<B>Finding:</B> " + mainReport.Finding + "<br>";

		formattedReport += formatReportPerformer(report.Parts[0]);

	    formattedReport += isDraft == true ? "</font>" : ""; 
	}
	
    return formattedReport;
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
