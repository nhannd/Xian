/*
    preview
*/

function getAlertHtml(alertItem, patientName)
{
	return "<img width='50' src='" + getAlertIcon(alertItem) + "' alt='" + getAlertTooltip(alertItem, patientName) + "' align='right'/>";
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
		case "Schedule Alert":
			return "Images/AlertClock.png";
		case "Incomplete demographic data alert":
			return "Images/AlertIncompleteData.png";
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
		case "Incomplete demographic data alert":
			return patientName + " has incomplete demographic data: " + reasons;
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
		var dateDiff = new Date(Date.parse(today) - Date.parse(dateTime));
		return dateDiff.getDate() + " days ago";
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

function createOrdersTable(htmlTable)
{
	var ordersTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Requested Procedures",
				cellType: "text",
				getValue: function(item) { return item.CombineRequestedProcedureName; }
			},
			{   label: "Scheduled For",
				cellType: "text",
				getValue: function(item) { return getDescriptiveTime(item.EarliestScheduledMPSDateTime); }
			},
			{   label: "Order Status",
				cellType: "text",
				getValue: function(item) { return item.OrderStatus; }
			},
			{   label: "Insurance",
				cellType: "text",
				getValue: function(item) { return ""; }
			},
			{   label: "Ordering Facility",
				cellType: "text",
				getValue: function(item) { return item.OrderingFacilityName; }
			},
			{   label: "Ordering Physician",
				cellType: "text",
				getValue: function(item) { return Ris.formatPersonName(item.OrderingPractitionerName); }
			}
		 ]);
		 
	ordersTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	ordersTable.rowCycleClassNames = ["row1", "row0"];
	
	return ordersTable;
}

function createDiagnosticServiceBreakdownTable(htmlTable)
{
	var dsTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
		 [
			{   label: "Diagnostic Service",
				cellType: "text",
				getValue: function(item) { return item.DiagnosticServiceName; }
			},
			{   label: "Requested Procedure",
				cellType: "text",
				getValue: function(item) { return item.RequestedProcedureName; }
			},
			{   label: "Step",
				cellType: "text",
				getValue: function(item) { return item.ModalityProcedureStepName; }
			},
			{   label: "Status",
				cellType: "text",
				getValue: function(item) { return item.ModalityProcedureStepStatus; }
			},
			{   label: "Active",
				cellType: "text",
				getValue: function(item) { return item.Active ? "*" : ""; }
			}
		 ]);
		 
	dsTable.rowCycleClassNames = ["row0", "row1"];
	
	return dsTable;
}

function orderDataComparison(data1, data2)
{
	return Date.compareMoreRecent(data1.EarliestScheduledMPSDateTime, data2.EarliestScheduledMPSDateTime);
}
