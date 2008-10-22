/*
 *	Provides several genaral methods for formatting data for preview pages.
*/
var Preview = function () {

	var _getAlertIcon = function(alertItem)
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
	};
	
	var _getAlertTooltip = function(alertItem, patientName)
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
	};
	
	return {
		getAlertHtml: function(alertItem, patientName)
		{
			return "<img class='alert' src='" + imagePath + "/" + _getAlertIcon(alertItem) + "' alt='" + _getAlertTooltip(alertItem, patientName) + "'/>";
		},

		getPatientAge: function(dateOfBirth, deathIndicator, timeOfDeath)
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
		},

		getPatientAgeInYears: function(dateOfBirth, deathIndicator, timeOfDeath)
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
		},

		formatPerformingFacilityList: function(procedures)
		{
			var facilities = [];
			for(var i = 0; i < procedures.length; i++)
			{
				if (facilities.indexOf(procedures[i].PerformingFacility.Name) < 0)
					facilities.add(procedures[i].PerformingFacility.Name);
			}
			
			return String.combine(facilities, "<br>");
		},
		
		filterProcedureByModality: function(procedures, modalityIdFilter)
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
		},
		
		formatVisitCurrentLocation: function(visit)
		{
			if (!visit || !visit.CurrentLocation)
				return null;
			
			if (visit.CurrentLocation.Room || visit.CurrentLocation.Bed)
				return visit.CurrentLocation.Name + ", " + (visit.CurrentLocation.Room || "") + (visit.CurrentLocation.Bed ? "/" + visit.CurrentLocation.Bed : "");
			else
				return visit.CurrentLocation.Name;
		}
	};
}();

/*
 *	Provides helper functions used by ProceduresTable, ProtocolProceduresTable and ReportingProceduresTable
 */
Preview.ProceduresTableHelper = function () {

	var _getDescriptiveTime = function(dateTime)
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

	return {
		formatProcedureSchedule: function(scheduledStartTime, schedulingRequestTime)
		{
			if (scheduledStartTime)
				return _getDescriptiveTime(scheduledStartTime); 
			else if (schedulingRequestTime)
				return "Requested for " + _getDescriptiveTime(schedulingRequestTime);
			else
				return "Not scheduled";
		},
		
		formatProcedureStatus: function(status, scheduledStartTime, startTime, checkInTime, checkOutTime)
		{
			if (!scheduledStartTime && !startTime)
				return "Unscheduled";

			if (status.Code == 'SC' && checkInTime)
				return "Checked-In";
			
			if ((status.Code == 'IP' || status.Code == 'CM') && checkOutTime)
				return "Completed";

			return status.Value; 
		},

		formatProcedureStartEndTime: function(startTime, checkOutTime)
		{
			if (!startTime)
				return "Not started";

			if (checkOutTime)
				return "Ended " + Ris.formatDateTime(checkOutTime);

			return "Started " + Ris.formatDateTime(startTime);
		},

		formatProcedurePerformingStaff: function(procedure)
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
		},
		
		addHeading: function(parentElement, text, className)
		{
			var heading = document.createElement("P");
			heading.className = className || 'sectionheading';
			heading.innerText = text;
			parentElement.appendChild(heading);
		},
		
		addTable: function(parentElement)
		{
			var htmlTable = document.createElement("TABLE");
			parentElement.appendChild(htmlTable);
			var body = document.createElement("TBODY");
			htmlTable.appendChild(body);
			return htmlTable;
		}
	};
}();

/*
 *	Create a table of imaging services with the following columns:
 *		- Procedure
 *		- Schedule
 *		- Status
 *		- Performing Facility
 *		- Ordering Physician
 *
 *	The imaging service of the selected procedure is excluded from the list
 *
 *	Exposes two methods
 *		- createActive:
 *		- createPast:
 */
Preview.ImagingServiceTable = function () {

	var _isProcedureStatusActive = function(procedureStatus)
	{
		return procedureStatus.Code == "SC" || 
				procedureStatus.Code == "IP";
	};

	var _orderRequestScheduledDateComparison = function(data1, data2)
	{
		return Date.compareMoreRecent(data1.SchedulingRequestTime, data2.SchedulingRequestTime);
	};

	var _procedureScheduledDateComparison = function(data1, data2)
	{
		return Date.compareMoreRecent(data1.ProcedureScheduledStartTime, data2.ProcedureScheduledStartTime);
	};

	var _getActiveProcedures = function(patientOrderData)
	{
		var today = Date.today();

		var presentScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime &&
						Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus);
			}).sort(_procedureScheduledDateComparison);

		var presentNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null &&
						item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus);
			}).sort(_orderRequestScheduledDateComparison);
			
		return presentScheduledProcedures .concat(presentNotScheduledProceduress);
	};

	var _getNonActiveProcedures = function(patientOrderData)
	{

		var today = Date.today();

		// List only the non-Active present procedures
		var presentScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus) == false;
			}).sort(_procedureScheduledDateComparison);

		// List only the non-Active present not-scheduled procedures
		var presentNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null &&
						item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus) == false;
			}).sort(_orderRequestScheduledDateComparison);

		var pastScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) < 0;
			}).sort(_procedureScheduledDateComparison);

		var pastNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null
				&& item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) < 0;
			}).sort(_orderRequestScheduledDateComparison);

		return presentScheduledProcedures.concat(
				presentNotScheduledProceduress.concat(
				pastScheduledProcedures.concat(pastNotScheduledProceduress)));
	};

	var _formatPerformingFacility = function(item)
	{
		 return item.ProcedurePerformingFacility ? item.ProcedurePerformingFacility.Code : "";
	};
	
	var _createHelper = function(parentElement, ordersList, sectionHeading)
	{
		if(ordersList.length == 0)
		{
			parentElement.style.display = 'none';
			return;
		}
		else
		{
			parentElement.style.display = 'block';
		}

		Preview.ProceduresTableHelper.addHeading(parentElement, sectionHeading);
		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);

		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
			 [
				{   label: "Procedure",
					cellType: "text",
					getValue: function(item) { return Ris.formatProcedureName(item); }
				},
				{   label: "Schedule",
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime); }
				},
				{   label: "Status",
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStatus(item.ProcedureStatus, item.ProcedureScheduledStartTime, item.ProcedureStartTime, item.ProcedureCheckInTime, item.ProcedureCheckOutTime); }
				},
				{   label: "Performing Facility",
					cellType: "text",
					getValue: function(item) { return _formatPerformingFacility(item); }
				},
				{   label: "Ordering Physician",
					cellType: "link",
					getValue: function(item)  { return Ris.formatPersonName(item.OrderingPractitioner.Name); },
					clickLink: function(item) { Ris.openPractitionerDetails(item.OrderingPractitioner); }
				}
			 ]);

		htmlTable.rowCycleClassNames = ["row0", "row1"];
		htmlTable.bindItems(ordersList);
	};

	return {
		createActive: function(parentElement, ordersList)
		{
			var activeProcedures = _getActiveProcedures(ordersList);
			_createHelper(parentElement, activeProcedures, "Active Imaging Services");
		},
		
		createPast: function(parentElement, ordersList)
		{
			var pastProcedures = _getNonActiveProcedures(ordersList);
			_createHelper(parentElement, pastProcedures, "Past Imaging Services");
		}
	};
}();

/*
 *	Create a table of procedures with the following columns:
 *		- Procedure
 *		- Status
 *		- Schedule
 *		- Start/End Time
 *		- Performing Staff
 *
 *	Exposes one method: create(...)
 */
Preview.ProceduresTable = function () {
	return {
		create: function(parentElement, procedures, addSectionHeading)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			if(!!addSectionHeading)
			{
				Preview.ProceduresTableHelper.addHeading(parentElement, 'Procedures');
			}

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: "Procedure",
						cellType: "text",
						getValue: function(item) { return Ris.formatProcedureName(item); }
					},
					{   label: "Status",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStatus(item.Status, item.ScheduledStartTime, item.StartTime, item.CheckInTime, item.CheckOutTime); }
					},
					{   label: "Schedule",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null); }
					},
					{   label: "Start/End Time",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStartEndTime(item.StartTime, item.CheckOutTime); }
					},
					{   label: "Performing Staff",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedurePerformingStaff(item); }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procedures);
		}
	};
}();

/*
 *	Create a table of procedures with protocol-specific details contained in the following columns:
 *		- Procedure
 *		- Protocol
 *		- Code
 *		- Author
 *		- Urgency
 *
 *	Exposes one method: create(...)
 */
Preview.ProtocolProceduresTable = function () {
			
	var _formatProtocolStatus = function(protocol)
	{
		if(!protocol)
			return "Not Protocolled";

		if(protocol.Status.Code == "RJ")
			return protocol.Status.Value + " - "+ protocol.RejectReason.Value;

		return protocol.Status.Value; 
	}
	
	var _formatProtocolCode = function(protocol)
	{
		if (!protocol)
			return "";
			
		return String.combine(protocol.Codes.map(function(code) { return code.Name; }), "<br>");
	}
	
	var _formatProtocolAuthor = function(protocol)
	{
		if (!protocol || !protocol.Author)
			return "";
		
		return Ris.formatPersonName(protocol.Author.Name);
	}
	
	var _formatProtocolUrgency = function(protocol)
	{
		if (!protocol)
			return "";
			
		return protocol.Urgency.Value;
	}
			
	return {
	
		// TODO: WTIS -> urgency and override in UHN-specific script.
		
		create: function(parentElement, procedures)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			Preview.ProceduresTableHelper.addHeading(parentElement, 'Protocols');

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: "Procedure",
						cellType: "text",
						getValue: function(item) { return Ris.formatProcedureName(item); }
					},
					{   label: "Status",
						cellType: "text",
						getValue: function(item) { return _formatProtocolStatus(item.Protocol); }
					},
					{   label: "Protocol",
						cellType: "html",
						getValue: function(item) { return _formatProtocolCode(item.Protocol); }
					},
					{   label: "Author",
						cellType: "text",
						getValue: function(item) { return _formatProtocolAuthor(item.Protocol); }
					},
					{   label: "Urgency",
						cellType: "text",
						getValue: function(item) { return _formatProtocolUrgency(item.Protocol); }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procedures);
		}
	};
}();

/*
 *	Create a table of procedures with reporting-specific details contained in the following columns:
 *		- Procedure
 *		- Status
 *		- Start/End Time
 *		- Performing Staff
 *		- Owner
 *
 *	Exposes one method: create(...)
 */
Preview.ReportingProceduresTable = function () {
	var _getActiveReportingStep = function(procedure)
	{
		var reportingStepNames = ["InterpretationStep", "TranscriptionStep", "VerificationStep", "PublicationStep"];
		var activeStatusCode = ["SC", "IP"];
		var isActiveReportingStep = function(step) { return reportingStepNames.indexOf(step.StepClassName) >= 0 && activeStatusCode.indexOf(step.State.Code) >= 0; };

		return procedure.ProcedureSteps.select(isActiveReportingStep).firstElement();
	}
	
	var _getLastCompletedPublicationStep = function(procedure)
	{
		var isCompletedPublicationStep = function(step) { return step.StepClassName == "PublicationStep" && step.State.Code == "CM"; };
		var compreStepEndTime = function(step1, step2) { return Date.compare(step1.EndTime, step2.EndTime); };
		return procedure.ProcedureSteps.select(isCompletedPublicationStep).sort(compreStepEndTime).reverse().firstElement();
	}
		 
	var _formatProcedureReportingStatus = function(procedure)
	{
		var activeReportingStep = _getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = _getLastCompletedPublicationStep(procedure);

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
	
	var _formatProcedureReportingOwner = function(procedure)
	{
		var activeReportingStep = _getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = _getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;

		if (!lastStep)
			return "";

		if (lastStep.Performer)
			return Ris.formatPersonName(lastStep.Performer.Name)
		
		if (lastStep.ScheduledPerformer)
			return Ris.formatPersonName(lastStep.ScheduledPerformer.Name);
			
		return "";
	}
		 
	return {
		create: function(parentElement, procedures)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: "Procedure",
						cellType: "text",
						getValue: function(item) { return Ris.formatProcedureName(item); }
					},
					{   label: "Status",
						cellType: "text",
						getValue: function(item) { return _formatProcedureReportingStatus(item); }
					},
					{   label: "Start/End Time",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStartEndTime(item.StartTime, item.CheckOutTime); }
					},
					{   label: "Performing Staff",
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedurePerformingStaff(item); }
					},
					{   label: "Owner",
						cellType: "text",
						getValue: function(item) { return _formatProcedureReportingOwner(item); }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procedures);
		}
	};
}();

/*
 *	Create a table of reports with the following columns:
 *		- Procedure
 *		- Report Status
 *
 *	Exposes one method: create(...)
 */
Preview.ReportListTable = function () {
	var _onReportListSelectionChanged = function(reportListItem)
	{
		if(!Ris) return;
	
		var request = 
		{
			GetReportDetailRequest: { ReportRef: reportListItem.ReportRef }
		};

		var service = Ris.getPatientDataService();
		var data = service.getData(request);

		if (data == null || data.length == 0)
		{
			Field.show($("reportContent"), false);
			return;
		}

		var reportDetail = data.GetReportDetailResponse ? data.GetReportDetailResponse.Report : null;
		if (reportDetail)
			Preview.ReportPreview.create($("reportContent"), reportDetail);
		else
			Field.show($("reportContent"), false);
	};	
		

	return {
		create: function(parentElement, reportList)
		{
			if(reportList.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			Preview.ProceduresTableHelper.addHeading(parentElement, 'Reports');

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);

			var reportContent = document.createElement("DIV");
			reportContent.id = "reportContent";
			parentElement.appendChild(reportContent);

			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, autoSelectFirstElement: true, addColumnHeadings:true },
			[
				{   label: "Procedure",
					cellType: "text",
					getValue: function(item) { return Ris.formatProcedureName(item); }
				},
				{   label: "Status",
					cellType: "text",
					getValue: function(item) { return item.ReportStatus.Value; }
				}
			]);

			htmlTable.onRowClick = function(sender, args)
			{
				_onReportListSelectionChanged(args.item);
			};

			htmlTable.mouseOverClassName = "mouseover";
			htmlTable.highlightClassName = "highlight";
			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(reportList);
		}
	};
}();

/*
 *	Create one or more tables of notes with the following columns:
 *		- Comment
 *		- Time
 *		- Author
 *	The notes can be split into tables for specific note categories if desired.
 * 	
 *	Exposes one method: create(parentElement, notes, subsections)
 * 		parentElement - parent node for table(s)
 *		notes - the list of note objects
 *		subsections - optional - a list of objects of form { category: "SomeNoteCategory", subsectionHeading: "SomeHeading" }.  
 *			If no subsections are specified, all notes are shown in a single table.
 *  Also exposes defaultSubsections array which can be used as the subsections parameter in create(...)
 */
Preview.OrderNotesTable = function () {
	var _createSubsection = function(parentElement, notes, categoryFilter, subsectionHeading)
	{
		var filteredNotes = categoryFilter ? notes.select(function(note) { return note.Category == categoryFilter; }) : notes;

		if (filteredNotes.length == 0)
			return;

		if(subsectionHeading)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, subsectionHeading, 'subsectionheading');
		}

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
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
	};

	return {
		create: function(parentElement, notes, subsections)
		{
			if(notes.length == 0)
				return;

			Preview.ProceduresTableHelper.addHeading(parentElement, 'Order Notes');

			if(subsections)
			{
				for(var i = 0; i < subsections.length; i++)
				{
					if(subsections[i])
					{
						_createSubsection(parentElement, notes, subsections[i].category, subsections[i].subsectionHeading);
					}
				}
			}
			else
			{
				_createSubsection(parentElement, notes);
			}
		},
		
		defaultSubsections:
		[
			{category:"General", subsectionHeading:"General"}, 
			{category:"Protocol", subsectionHeading:"Protocol"}, 
			{category:"PrelimDiagnosis", subsectionHeading:"Preliminary Diagnosis"}
		]
	};
}();

/*
 *	Create a report preview.
 * 	
 *	Exposes one method: create(...)
 */
Preview.ReportPreview = function () {
	var _parseReportObject = function(reportJsml)
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

	var _parseReportContent = function(reportJsml)
	{
		var reportObject = _parseReportObject(reportJsml);
		if (!reportObject)
		{
			// the Content was not JSML, but just plain text
			return reportJsml || null;
		}

		// depending on how the report was captured, it may contain an Impression and Finding section (Default RIS report editor)
		var reportText;
		if(reportObject.Impression || reportObject.Finding)
			reportText = "<B>Impression:</B> " + reportObject.Impression + "<br>" + "<B>Finding:</B> " + reportObject.Finding + "<br>";
		else // or it may simply contain a ReportText section (UHN report editor)
			reportText = reportObject.ReportText;

		if (!reportText)
			return null;

		return reportText.replaceLineBreak();
	}

	var _formatReportStatus = function(report, isAddendum)
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

	var _formatReportPerformer = function(reportPart)
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

	return {
		create: function(element, report)
		{
			if (element == null || report == null || report.Parts == null || report.Parts.length == 0)
				return "";

			element.className = 'reportPreview';

			var formattedReport = "<br>";
			if (report.Parts.length > 1)
			{
				for (var i = report.Parts.length-1; i > 0; i--)
				{
					var addendumPart = report.Parts[i];
					var addendumContent = addendumPart && addendumPart.ExtendedProperties && addendumPart.ExtendedProperties.ReportContent ? addendumPart.ExtendedProperties.ReportContent : "";
					var parsedReportContent = _parseReportContent(addendumContent);
					if (parsedReportContent)
					{
						formattedReport += "<b>Addendum " + _formatReportStatus(addendumPart, true) + ": </b><br><br>";
						formattedReport += parsedReportContent;
						formattedReport += _formatReportPerformer(addendumPart);
						formattedReport += "<br><br>";
					}
				}
			}

			var part0 = report.Parts[0];
			var reportContent = part0 && part0.ExtendedProperties && part0.ExtendedProperties.ReportContent ? part0.ExtendedProperties.ReportContent : "";
			formattedReport += "<b>Report" + _formatReportStatus(part0) + "</b>";
			formattedReport += "<br><br>";
			formattedReport += _parseReportContent(reportContent);
			formattedReport += _formatReportPerformer(part0);

			element.innerHTML = formattedReport;
		}
	};
}();

/*
 *	Create a summary of a single imaging service.
 */
Preview.ImagingServiceSection = function () {
	var _html = 
		'<p class="sectionheading">Imaging Service</p>'+
		'<table width="100%" border="0">'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Accession Number</td>'+
		'		<td width="200"><div id="AccessionNumber"/></td>'+
		'		<td width="120" class="propertyname">Priority</td>'+
		'		<td width="200"><div id="OrderPriority"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Ordering Physician</td>'+
		'		<td width="200"><div id="OrderingPhysician"/></td>'+
		'		<td width="120" class="propertyname">Performing Facility</td>'+
		'		<td width="200"><div id="PerformingFacility"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Patient Class</td>'+
		'		<td width="200"><div id="PatientClass"/></td>'+
		'		<td width="120" class="propertyname">Location, Room/Bed</td>'+
		'		<td width="200"><div id="LocationRoomBed"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Indication</td>'+
		'		<td colspan="4"><div id="ReasonForStudy"/></td>'+
		'	</tr>'+
		'	<tr id="CancelReasonSection">'+
		'		<td width="120" class="propertyname">Cancel Reason</td>'+
		'		<td width="200" colspan="4"><div id="CancelReason"/></td>'+
		'	</tr>'+
		'</table>';
		
	return {
		create: function (element, orderDetail)
		{
			if(orderDetail == null)
				return;
				
			element.innerHTML = _html;
		
			Field.setValue($("AccessionNumber"), Ris.formatAccessionNumber(orderDetail.AccessionNumber));
			Field.setValue($("OrderPriority"), orderDetail.OrderPriority.Value);
			Field.setLink($("OrderingPhysician"), Ris.formatPersonName(orderDetail.OrderingPractitioner.Name), function() { Ris.openPractitionerDetails(orderDetail.OrderingPractitioner); });
			Field.setValue($("PerformingFacility"), Preview.formatPerformingFacilityList(orderDetail.Procedures));
			Field.setValue($("PatientClass"), orderDetail.Visit.PatientClass.Value);
			Field.setValue($("LocationRoomBed"), Preview.formatVisitCurrentLocation(orderDetail.Visit));
			Field.setValue($("ReasonForStudy"), orderDetail.ReasonForStudy);
			if (orderDetail.CancelReason)
				Field.setValue($("CancelReason"), orderDetail.CancelReason.Value);
			else
				Field.show($("CancelReasonSection"), false);
		}
	};

}();

/*
 *	Create a summary of demographic information of a single patient profile.
 */
Preview.PatientDemographicsSection = function () {
	var _html = 
		'<table width="100%" border="0">'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Date of Birth</td>'+
		'		<td width="200"><div id="dateOfBirth"/></td>'+
		'		<td width="63" class="propertyname">Age</td>'+
		'		<td width="229"><div id="age"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="propertyname">Healthcard # </td>'+
		'		<td><div id="healthcard"/></td>'+
		'		<td width="63" class="propertyname">Sex</td>'+
		'		<td width="229"><div id="sex"/></td>'+
		'	</tr>'+
		'	<tr id="HomePhoneRow">'+
		'		<td class="propertyname">Home Phone</td>'+
		'		<td colspan="4"><div id="currentHomePhone"/></td>'+
		'	</tr>'+
		'	<tr id="HomeAddressRow">'+
		'		<td class="propertyname">Home Address</td>'+
		'		<td colspan="4"><div id="currentHomeAddress"/></td>'+
		'	</tr>'+
		'</table>';

	return {
		create: function(element, patientProfile)
		{
			if(patientProfile == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("age"), Preview.getPatientAge(patientProfile.DateOfBirth, patientProfile.DeathIndicator, patientProfile.TimeOfDeath));
			Field.setValue($("sex"), patientProfile.Sex.Value);
			Field.setValue($("dateOfBirth"), Ris.formatDate(patientProfile.DateOfBirth));
			Field.setValue($("healthcard"), Ris.formatHealthcard(patientProfile.Healthcard));
			if (patientProfile.CurrentHomePhone)
				Field.setValue($("currentHomePhone"), Ris.formatTelephone(patientProfile.CurrentHomePhone));
			else
				Field.show($("HomePhoneRow"), false);

			if (patientProfile.CurrentHomeAddress)
				Field.setValue($("currentHomeAddress"), Ris.formatAddress(patientProfile.CurrentHomeAddress));
			else
				Field.show($("HomeAddressRow"), false);
		}
	};
}();

/*
 *	Create a banner showing a patient name, MRN and any provided alerts.
 */
Preview.PatientBannerSection = function() {
	return {
		create: function(element, patientProfile, alerts)
		{
			if(patientProfile == null)
				return;

			var patientName = Ris.formatPersonName(patientProfile.Name);
			var patientMRN = Ris.formatMrn(patientProfile.Mrn);
			Preview.BannerSection.create(element, patientName, patientMRN, patientName, alerts);
		}
	};
}();

/*
 *	Create a two line banner section with alerts
 *	Exposes:
 *		create(element, line1, line2, patientName, alerts)
 *			element - parent node for the banner
 *			line1 - first line text
 *			line2 - second line text
 *			patientName - patient name, used in alert hover text
 *			alerts - a list of alerts (from Ris.getPatientDataService) to display
 */
Preview.BannerSection = function() {
	var _html =
		'<table width="100%" border="0">'+
		'	<tr>'+
		'		<td class="patientnameheading"><div id="line1" /></td>'+
		'		<td rowspan="2" align="right"><div id="alerts"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="patientmrnheading"><div id="line2"/></td>'+
		'	</tr>'+
		'</table>';

	return {
		create: function(element, line1, line2, patientName, alerts)
		{
			element.innerHTML = _html;

			Field.setValue($("line1"), line1);
			Field.setValue($("line2"), line2);

			var alertHtml = "";
			alerts.each(function(item) { alertHtml += Preview.getAlertHtml(item, patientName); });
			$("alerts").innerHTML = alertHtml;
		}
	};
}();