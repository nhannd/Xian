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
			theBirthdateThisYear.setYMD(endDate.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate());

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
			theBirthdateThisYear.setYMD(endDate.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate());

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
	return {
		formatProcedureSchedule: function(scheduledStartTime, schedulingRequestTime, showDescriptiveTime)
		{
			if (scheduledStartTime)
				return showDescriptiveTime ? Ris.formatDescriptiveDateTime(scheduledStartTime) : Ris.formatDateTime(scheduledStartTime); 
			else if (schedulingRequestTime)
				return "Requested for " + showDescriptiveTime ? Ris.formatDescriptiveDateTime(schedulingRequestTime) : Ris.formatDateTime(schedulingRequestTime);
			else
				return "Not scheduled";
		},
		
		formatProcedureStatus: function(status, scheduledStartTime, startTime, checkInTime, checkOutTime)
		{
			if (status.Code == 'CA')
				return status.Value;  // show status instead of "Unscheduled" for unscheduled procedure that are cancelled
				
			if (!scheduledStartTime && !startTime)
				return "Unscheduled";

			if (status.Code == 'SC' && checkInTime)
				return "Checked-In";
			
			if (status.Code == 'IP' && checkOutTime)
				return "Performed";
				
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
			
		addTable: function(parentElement, id, noTableHeading)
		{

			noTableHeading = !!noTableHeading;

			var htmlTableContainer = document.createElement("DIV");
			htmlTableContainer.className = "ProceduresTableContainer";
			var htmlTable = document.createElement("TABLE");
			htmlTableContainer.appendChild(htmlTable);
			parentElement.appendChild(htmlTableContainer);
			var body = document.createElement("TBODY");
			htmlTable.appendChild(body);

			if(noTableHeading)
			{
				var headingRow = document.createElement("TR");
				body.appendChild(headingRow);
			}

			return htmlTable;
		},
		
		addTableWithClass: function(parentElement, id, noTableHeading, cssClass)
		{

			noTableHeading = !!noTableHeading;

			var htmlTableContainer = document.createElement("DIV");
			htmlTableContainer.className = cssClass;
			var htmlTable = document.createElement("TABLE");
			htmlTableContainer.appendChild(htmlTable);
			parentElement.appendChild(htmlTableContainer);
			var body = document.createElement("TBODY");
			htmlTable.appendChild(body);

			if(noTableHeading)
			{
				var headingRow = document.createElement("TR");
				body.appendChild(headingRow);
			}

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
			
		return presentScheduledProcedures.concat(presentNotScheduledProceduress);
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

	var _formatPerformingFacility = function(item, memberName)
	{
		return item.ProcedurePerformingFacility ? item.ProcedurePerformingFacility[memberName] : "";
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

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);

		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
			 [
				{   label: "Procedure",
					cellType: "text",
					getValue: function(item) { return Ris.formatOrderListItemProcedureName(item); }
				},
				{   label: "Schedule",
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime, true); },
					getTooltip: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime, false); }
				},
				{   label: "Status",
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStatus(item.ProcedureStatus, item.ProcedureScheduledStartTime, item.ProcedureStartTime, item.ProcedureCheckInTime, item.ProcedureCheckOutTime); }
				},
				{   label: "Performing Facility",
					cellType: "text",
					getValue: function(item) { return _formatPerformingFacility(item, 'Code'); },
					getTooltip: function(item) { return _formatPerformingFacility(item, 'Name'); }
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
			Preview.SectionContainer.create(parentElement, "Active Imaging Services", { collapsible: true, initiallyCollapsed: true });						
		},
		
		createPast: function(parentElement, ordersList, options)
		{
			var pastProcedures = _getNonActiveProcedures(ordersList);
				
			_createHelper(parentElement, pastProcedures, "Past Imaging Services");
			Preview.SectionContainer.create(parentElement, "Past Imaging Services", { collapsible: true, initiallyCollapsed: true });						
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
		create: function(parentElement, procedures, options)
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
			
			if(options && options.AddSectionHeading)
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
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null, true); },
						getTooltip: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null, false); }
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
			
			Preview.SectionContainer.create(parentElement, "Procedures");
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
		if (!protocol || !protocol.Urgency)
			return "";
			
		return protocol.Urgency.Value;
	}
			
	return {
	
		// TODO: WTIS -> urgency and override in UHN-specific script.
		
		create: function(parentElement, procedures, notes)
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
			
			// group procedures according to their protocol ref(indicates linked protocolling),
			// or by procedureRef, if they don't have a protocol (effectively not grouping them)
			var procGroupings = procedures.groupBy(
				function(proc)
				{
					return proc.Protocol ? proc.Protocol.ProtocolRef : proc.ProcedureRef;
				});

			// transform the groupings into individual items that can be displayed in the table	
			procGroupings = procGroupings.map(
				function(grouping)
				{
					// reduce each procedure Grouping to a single item, where all procedure names are concatenated (all other fields should be identical)
					return grouping.reduce({},
						function(memo, item) 
						{
							return {
								Procedure : memo.Procedure ? [memo.Procedure, Ris.formatProcedureName(item)].join(', ') : Ris.formatProcedureName(item),
								// if the procedure was cancelled, leave the protocol status blank
								Status : memo.Status || ((item.Status.Code == "CA") ? "" : _formatProtocolStatus(item.Protocol)),
								Protocol: memo.Protocol || _formatProtocolCode(item.Protocol),
								Author : memo.Author || _formatProtocolAuthor(item.Protocol),
								Urgency: memo.Urgency || _formatProtocolUrgency(item.Protocol)
							};
						});
				});

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: "Procedure",
						cellType: "text",
						getValue: function(item) { return item.Procedure; }
					},
					{   label: "Status",
						cellType: "text",
						getValue: function(item) { return item.Status; }
					},
					{   label: "Protocol",
						cellType: "html",
						getValue: function(item) { return item.Protocol; }
					},
					{   label: "Author",
						cellType: "text",
						getValue: function(item) { return item.Author; }
					},
					{   label: "Urgency",
						cellType: "text",
						getValue: function(item) { return item.WTIS; }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procGroupings);
									
			if(notes.length > 0) {
				Preview.OrderNotesTable.createAsSubSection(parentElement, notes, "Notes");
			}

			Preview.SectionContainer.create(parentElement, "Protocols");
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
		var reportingStepNames = ["InterpretationStep", "TranscriptionStep", "TranscriptionReviewStep", "VerificationStep", "PublicationStep"];
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
		// bug #3470: there is no point drilling down into the "reporting" status unless the procedure is actually In Progress or Completed
		// if not, we just return the Procedure status
		if(["IP", "CM"].indexOf(procedure.Status.Code) == -1)
			return Preview.ProceduresTableHelper.formatProcedureStatus(procedure.Status, procedure.ScheduledStartTime, procedure.StartTime, procedure.CheckInTime, procedure.CheckOutTime);
	
		var activeReportingStep = _getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = _getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;
		
		// bug #3470: there may not be any reporting steps yet, or the procedure may have been linked to another - there is no way to tell
		// for now, just return "". The linked procedure problem should be fixed server-side in a future version
		if(!lastStep) return "";
		
		var isAddendum = activeReportingStep && lastCompletedPublicationStep;

		var stepName = lastStep.ProcedureStepName;
		var addendumPrefix = isAddendum ? "Addendum " : "";

		var formattedStatus;
		switch(lastStep.State.Code)
		{
			case "SC": formattedStatus = "Pending " + stepName;
				break;
			case "IP": formattedStatus = stepName + " In Progress"; 
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
	
	var _getImageAvailabilityIcon = function(imageAvailability)
	{
		switch (imageAvailability.Code)
		{
			case "X":
				statusIcon = "question.png";
			case "N":
				return "question.png";
			case "Z":
				return "shield_red.png";
			case "P":
				return "shield_yellow.png";
			case "C":
				return "shield_green.png";
			default:
				return "question.png";
		}
		
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
			
			// group procedures according to their active reporting step (indicates linked reporting),
			// or by procedureRef, if they don't have an active reporting step (effectively not grouping them)
			var procGroupings = procedures.groupBy(
				function(proc)
				{
					var ps = _getActiveReportingStep(proc);
					return ps ? ps.ProcedureStepRef : proc.ProcedureRef;
				});

			// transform the groupings into individual items that can be displayed in the table	
			procGroupings = procGroupings.map(
				function(grouping)
				{
					// reduce each procedure Grouping to a single item, where all procedure names are concatenated (all other fields should be identical)
					return grouping.reduce({},
						function(memo, item) 
						{
							return {
								Procedure : memo.Procedure ? [memo.Procedure, Ris.formatProcedureName(item)].join(', ') : Ris.formatProcedureName(item),
								Status : memo.Status || _formatProcedureReportingStatus(item),
								StartEndTime: memo.StartEndTime || Preview.ProceduresTableHelper.formatProcedureStartEndTime(item.StartTime, item.CheckOutTime),
								PerformingStaff : memo.PerformingStaff || Preview.ProceduresTableHelper.formatProcedurePerformingStaff(item),
								Owner: memo.Owner || _formatProcedureReportingOwner(item)
							};
						});
				});

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					// {   label: "Image Availability",
						// cellType: "html",
						// getValue: function(item) { return "<img class='alert' src='" + imagePath + "/" + _getImageAvailabilityIcon(item.ImageAvailability) + "' alt='" + item.ImageAvailability.Value + "'/>"; }
					// },
					{   label: "Procedure",
						cellType: "text",
						getValue: function(item) { return item.Procedure; }
					},
					{   label: "Status",
						cellType: "text",
						getValue: function(item) { return item.Status; }
					},
					{   label: "Start/End Time",
						cellType: "text",
						getValue: function(item) { return item.StartEndTime; }
					},
					{   label: "Performing Staff",
						cellType: "text",
						getValue: function(item) { return item.PerformingStaff; }
					},
					{   label: "Owner",
						cellType: "text",
						getValue: function(item) { return item.Owner; }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procGroupings);
			
			Preview.SectionContainer.create(parentElement, "Procedures");
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
		Preview.ReportPreview.create($("reportContent"), reportDetail, { hideSectionContainer: true });
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
			
			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, null);

			var reportContent = document.createElement("DIV");
			reportContent.id = "reportContent";
			parentElement.appendChild(reportContent);

			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, autoSelectFirstElement: true, addColumnHeadings:true },
			[
				{   label: "Procedure",
					cellType: "text",
					getValue: function(item) { return Ris.formatOrderListItemProcedureName(item); }
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
			
			Preview.SectionContainer.create(parentElement, "Reports");
		}
	};
}();

/*
 *	Create one or more tables of notes with a preformatted HTML column:
 *	The notes can be split into tables for specific note categories if desired.
 * 	
 *	Exposes one method: create(parentElement, notes, subsections, hideHeading, canAcknowledge)
 * 		parentElement - parent node for table(s)
 *		notes - the list of note objects
 *		subsections - optional - a list of objects of form { category: "SomeNoteCategory", subsectionHeading: "SomeHeading" }.  
 *			If no subsections are specified, all notes are shown in a single table.
 *		hideHeading - optional - hide heading and category headings
 *		checkBoxesProperties - optional - customize the checkbox behaviour, including callback when a checkbox is toggled and override when a checkbox is visible
 *  Also exposes defaultSubsections array which can be used as the subsections parameter in create(...)
 */
Preview.OrderNotesTable = function () {
	var _createNotesTable = function(parentElement, notes, title)
	{
		if (notes.length == 0)
			return;
			
		if(title)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, title, 'subsectionheading');
		}			

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, null, true);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: false },
		[
			{   
				cellType: "html",
				getValue: function(item) 
				{
					var divTag = document.createElement("div");
					Preview.OrderNoteSection.create(divTag, item);
					
					// retrieve the innerHTML of the template table
					return Field.getValue(divTag);
				}
			}
		]);

		// highlight the row that need to be acknowledged
		htmlTable.renderRow = function(sender, args)
		{
			if(args.item.CanAcknowledge)
			{
				args.htmlRow.className = "attention" + (args.rowIndex % 2);
			}
		};

//		htmlTable.rowCycleClassNames = ["row1", "row0"];

		htmlTable.bindItems(notes);
	};

	return {
		create: function(parentElement, notes, heading, collapsedByDefault)
		{
			if(notes.length == 0)
				return;

			_createNotesTable(parentElement, notes);

			Preview.SectionContainer.create(parentElement, heading, { collapsible: true, initiallyCollapsed: collapsedByDefault });
		},
		
		createAsSubSection: function(parentElement, notes, title) {
			if(notes.length == 0)
				return;

			_createNotesTable(parentElement, notes, title);			
		}
	};
}();

/*
 *	Create a list of conversation items. Each item contains the author, post date and a message.
 * 	
 *	Exposes one method: create(parentElement, notes, subsections, hideHeading, canAcknowledge)
 * 		parentElement - parent node for table(s)
 *		notes - the list of note objects
 *		subsections - optional - a list of objects of form { category: "SomeNoteCategory", subsectionHeading: "SomeHeading" }.  
 *			If no subsections are specified, all notes are shown in a single table.
 *		hideHeading - optional - hide heading and category headings
 *		checkBoxesProperties - optional - customize the checkbox behaviour, including callback when a checkbox is toggled and override when a checkbox is visible
 *  Also exposes defaultSubsections array which can be used as the subsections parameter in create(...)
 */
Preview.ConversationHistory = function () {
	var _createSubsection = function(parentElement, notes, categoryFilter, subsectionHeading, hideHeading, checkBoxesProperties)
	{
		var filteredNotes = categoryFilter ? notes.select(function(note) { return note.Category == categoryFilter; }) : notes;

		if (filteredNotes.length == 0)
			return;

		if(subsectionHeading && !hideHeading)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, subsectionHeading, 'subsectionheading');
		}

		var canAcknowledge = checkBoxesProperties && checkBoxesProperties.onItemChecked && filteredNotes.find(function(note) { return note.CanAcknowledge; });

		var htmlTable = Preview.ProceduresTableHelper.addTableWithClass(parentElement, "NoteEntryTable", noColumnHeadings=true, "ConversationHistoryTable");
		htmlTable = Table.createTable(htmlTable, { checkBoxes: false, checkBoxesProperties: checkBoxesProperties, editInPlace: false, flow: false, addColumnHeadings: false },
		[
			{   label: "Order Note",
				cellType: "html",
				getValue: function(item) 
				{
					var divTag = document.createElement("div");
					Preview.ConversationNote.create(divTag, item);
					
					// retrieve the innerHTML of the template table
					return Field.getValue(divTag);
				}
			}
		]);

		htmlTable.bindItems(filteredNotes);
		
		// after binding the items, need to hook up the check boxes manually
		notes.each(
			function(note)
			{			
				var checkBox = $(note.OrderNoteRef);	// the OrderNoteRef was used as the checkbox "id"
				// checkBox may be null if that note was not ack'able
				if(checkBox)
				{
					checkBox.onclick = function() { checkBoxesProperties.onItemChecked(note, checkBox.checked); };
				}
			});
	};

	return {
		create: function(parentElement, notes, subsections, hideHeading, checkBoxesProperties)
		{		
			if(notes.length == 0)
				return;

			if(subsections)
			{
				for(var i = 0; i < subsections.length; i++)
				{
					if(subsections[i])
					{
						_createSubsection(parentElement, notes, subsections[i].category, subsections[i].subsectionHeading, hideHeading, checkBoxesProperties);
					}
				}
			}
			else
			{
				_createSubsection(parentElement, notes);
			}

			if(!hideHeading)
				Preview.SectionContainer.create(parentElement, "Order Notes");
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
			return reportJsml || "";
		}

		// depending on how the report was captured, it may contain an Impression and Finding section (Default RIS report editor)
		var reportText;
		if(reportObject.Impression || reportObject.Finding)
			reportText = "<B>Impression:</B> " + reportObject.Impression + "<br>" + "<B>Finding:</B> " + reportObject.Finding + "<br>";
		else // or it may simply contain a ReportText section (UHN report editor)
			reportText = reportObject.ReportText;

		if (!reportText)
			return "";
			
		// #5423: ensure that the reportText object is actually a string (not a Number or Date or whatever)	
		reportText = String(reportText);		

		return reportText.replaceLineBreak();
	}

	var _formatReportStatus = function(report, isAddendum)
	{
		var timePropertyMap = {X: 'CancelledTime', D: 'CreationTime', P: 'PreliminaryTime', F: 'CompletedTime'};
		var timeText = Ris.formatDateTime(report[timePropertyMap[report.Status.Code]]);
		var warningText = " *** THIS " + (isAddendum ? "ADDENDUM" : "REPORT") + " HAS NOT BEEN VERIFIED ***";

		var statusText = report.Status.Value + " - " + timeText;

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

		if (reportPart.Supervisor)
			formattedReport += "<br> Supervised By: " + Ris.formatPersonName(reportPart.Supervisor.Name);

		return formattedReport;
	}

	return {
		create: function(element, report, options)
		{
			if (element == null || report == null || report.Parts == null || report.Parts.length == 0)
			{
				element.style.display = 'none';
				return;
			}

			// force options to boolean values
			options = options || {};
			options.hideSectionContainer = !!options.hideSectionContainer;

			var formattedReport = "<br>";

			formattedReport += '<div id="transcriptionErrorsDiv" style="{display:none; border: 1px solid black; text-align: center; color: red; margin-bottom:1em; font-weight:bold;}">Transcription has errors.</div>'

			if (report.Parts.length > 1)
			{
				for (var i = report.Parts.length-1; i > 0; i--)
				{
					var addendumPart = report.Parts[i];
					var addendumContent = addendumPart && addendumPart.ExtendedProperties && addendumPart.ExtendedProperties.ReportContent ? addendumPart.ExtendedProperties.ReportContent : "";
					var parsedReportContent = _parseReportContent(addendumContent);
					if (parsedReportContent)
					{
						formattedReport += "<div class='reportPreview'>";
						formattedReport += "<b>Addendum " + _formatReportStatus(addendumPart, true) + ": </b><br><br>";
						formattedReport += parsedReportContent;
						formattedReport += _formatReportPerformer(addendumPart);
						formattedReport += "<br><br>";
						formattedReport += "</div>";
					}
				}
			}

			var part0 = report.Parts[0];
			var reportContent = part0 && part0.ExtendedProperties && part0.ExtendedProperties.ReportContent ? part0.ExtendedProperties.ReportContent : "";
			formattedReport += "<div class='reportPreview'>";
			formattedReport += "<b>Report" + _formatReportStatus(part0) + "</b>";
			formattedReport += "<br><br>";
			formattedReport += _parseReportContent(reportContent);
			formattedReport += _formatReportPerformer(part0);
			formattedReport += "<br><br>";
			formattedReport += "</div>";

			element.innerHTML = formattedReport;
			
			if (!options.hideSectionContainer)
				Preview.SectionContainer.create(element, "Report");
		},
		
		toggleTranscriptionErrors: function(hasErrors)
		{
			Field.show($("transcriptionErrorsDiv"), hasErrors);
		}
	};
}();

/*
 *	Create a summary of a single imaging service.
 */
Preview.ImagingServiceSection = function () {
	var _html = 
		'<div class="SectionTableContainer">' +
		'<table width="100%" border="0" cellspacing="5">'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Accession Number</td>'+
		'		<td width="200"><div id="AccessionNumber"/></td>'+
		'		<td width="120" class="propertyname">Priority</td>'+
		'		<td width="200"><div id="OrderPriority"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">Ordering Physician</td>'+
		'		<td width="200">'+
		'			<div id="OrderingPhysician"></div>'+
		'			<div id="OrderingPhysicianContactPointDetails" style="{font-size:75%;}">'+
		'				<div id="OrderingPhysicianAddress"/></div>'+
		'				<div id="OrderingPhysicianPhone"></div>'+
		'				<div id="OrderingPhysicianFax"></div>'+
		'				<div id="OrderingPhysicianEmail"></div>'+
		'			</div>'+
		'		</td>'+
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
		'	<tr id="EnteredBySection">'+
		'		<td width="120" class="propertyname">Entered By</td>'+
		'		<td width="200" colspan="3"><div id="EnteredBy"/></td>'+
		'	</tr>'+
		'	<tr id="AlertsSection">'+
		'		<td width="120" class="propertyname">Alerts</td>'+
		'		<td width="200" colspan="3"><div id="Alerts"/></td>'+
		'	</tr>'+
		'	<tr id="CancelSection">'+
		'		<td colspan="4">'+
		'			<p class="subsectionheading">Order Cancelled</p>'+
		'			<table width="100%" border="0">'+
		'				<tr id="CancelledBySection">'+
		'					<td width="150" class="propertyname">Cancelled By</td>'+
		'					<td><div id="CancelledBy"/></td>'+
		'				</tr>'+
		'				<tr>'+
		'					<td width="150" class="propertyname">Cancel Reason</td>'+
		'					<td><div id="CancelReason"/></td>'+
		'				</tr>'+
		'			</table>'+
		'		</td>'+
		'	</tr>'+
		'</table></div>';
		
	var GetPractitionerContactPoint = function(practitioner, recipients)
	{
		var contactPoint = null;
		recipients.each(function(recipient) 
		{
			if(recipient && recipient.Practitioner && recipient.Practitioner.PractitionerRef == practitioner.PractitionerRef)
			{
				contactPoint = recipient.ContactPoint;
			}
		});
		return contactPoint;
	}
		
	return {
		create: function (element, orderDetail, options)
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
			Field.setValue($("EnteredBy"), orderDetail.EnteredBy ? (Ris.formatPersonName(orderDetail.EnteredBy.Name) + ' (' + orderDetail.EnteredBy.StaffType.Value + ')') : "");
			if (orderDetail.CancelReason)
			{
				Field.setValue($("CancelledBy"), Ris.formatPersonName(orderDetail.CancelledBy.Name) + ' (' + orderDetail.CancelledBy.StaffType.Value + ')');
				Field.setValue($("CancelReason"), orderDetail.CancelReason.Value);
			}
			else
			{
				Field.show($("CancelSection"), false);
			}

			Field.show($("EnteredBySection"), false);
			Field.show($("CancelledBySection"), false);
			Field.show($("AlertsSection"), false);
			Field.show($("OrderingPhysicianContactPointDetails"), false);
			
			if (options)
			{
				Field.show($("EnteredBySection"), options.ShowEnterCancelByStaff);
				Field.show($("CancelledBySection"), options.ShowEnterCancelByStaff);
				
				if (options.Alerts && options.Alerts.length > 0)
				{
					Field.show($("AlertsSection"), true);
					var alertHtml = "";
					options.Alerts.each(function(item) { alertHtml += Preview.getAlertHtml(item); });
					Field.setPreFormattedValue($("Alerts"), alertHtml);
				}

				var contactPoint = GetPractitionerContactPoint(orderDetail.OrderingPractitioner, orderDetail.ResultRecipients);
				if (options.ShowOrderingPhysicianContactPointDetails && contactPoint)
				{
					Field.show($("OrderingPhysicianContactPointDetails"), true);

					Field.show($("OrderingPhysicianAddress"), contactPoint.CurrentAddress != null);
					Field.show($("OrderingPhysicianPhone"), contactPoint.CurrentPhoneNumber != null);
					Field.show($("OrderingPhysicianFax"), contactPoint.CurrentFaxNumber != null);
					Field.show($("OrderingPhysicianEmail"), contactPoint.CurrentEmailAddress != null);

					Field.setValue($("OrderingPhysicianAddress"), Ris.formatAddress(contactPoint.CurrentAddress));
					Field.setValue($("OrderingPhysicianPhone"), "Phone: " + Ris.formatTelephone(contactPoint.CurrentPhoneNumber));
					Field.setValue($("OrderingPhysicianFax"), "Fax: " + Ris.formatTelephone(contactPoint.CurrentFaxNumber));
					Field.setValue($("OrderingPhysicianEmail"), contactPoint.CurrentEmailAddress ? contactPoint.CurrentEmailAddress.Address : "");
				}
			}
			
			Preview.SectionContainer.create(element, "Imaging Service");
		}
	};

}();


/*
 *	Create a contrainer for the element with the specified title.
 */
Preview.SectionContainer = function () {

	var _createContainer = function(element, title, collapsible, collapsedByDefault)
	{
		var _createContentContainer = function(contentElement)
		{
			var divElement = document.createElement("div");
			divElement.className = "ContentContainer";
			divElement.appendChild(contentElement);
			return divElement;
		};
		
		var _createCell = function(row, className, text)
		{
			var cell = row.insertCell();
			cell.className = className;
			cell.innerText = text || "";
			return cell;
		};
		
		var _createHeadingCell = function(row, className, text, collapsible, collapsedByDefault)
		{
			var cell = row.insertCell();
			cell.className = className;
			
			if (collapsible) {
				var imageSrc = imagePath + "/";
				imageSrc += collapsedByDefault ? "Expand.png" : "Collapse.png";
				title = "<a href='javascript:void(0)' class='collapsibleSectionHeading' onclick='Collapse.toggleCollapsedSection(this)' style='{text-decoration: none; color: white;}'>" +
				 		"<img src='" + imageSrc + "' border='0' style='{margin-right: 5px; margin-left: -8px; background: #1b4066;}'/>" + text + "</a>";
				cell.innerHTML = title;
			}
			else {
				cell.innerText = text || "";
			}
			
			return cell;
		};		

		var content = _createContentContainer(element);

		var table = document.createElement("table");
		var body = document.createElement("tbody");
		table.className = "SectionContainer";
		table.appendChild(body);

		var row, cell;
		row = body.insertRow();
		_createCell(row, "SectionHeadingLeft");
		_createHeadingCell(row, "SectionHeadingBackground", title, collapsible, collapsedByDefault);
		_createCell(row, "SectionHeadingRight");

		row = body.insertRow();
		cell = _createCell(row, "ContentCell");
		cell.colSpan = "3";
		cell.appendChild(content);
		
		if(collapsible) Collapse.collapseSection(table, collapsedByDefault);
		
		return table;
	};

	return {
		
		/*
			options:
				collapsible: true/false - indicates whether the section is collapsible or not
				initiallyCollapsed: true/false - indicates whether a collapsible section is initially collapsed
		*/
		create: function (element, title, options)
		{			
			// no need to create a contrainer if the element is hidden
			if (element.style.display == 'none')
				return;
			
			// default value for options if not supplied
			options = options || {};

			// Replace the element with the new element that is encased in the container.
			// We cannot simply use innerHTML because all the event handler of the element will not
			// be propagated to the new element.  Hence the DOM manipulation to preserve the handlers.
			var parent = element.parentNode;
			var nextSibling = element.nextSibling;
			var newElement = _createContainer(element, title, options.collapsible, options.initiallyCollapsed);
			if (nextSibling)
				parent.insertBefore(newElement, nextSibling);
			else
				parent.appendChild(newElement);
		}
	};

}();

/*
 *	Create a banner section of the pages with the content (i.e. patient demographics).
 */
Preview.PatientBannner = function () {

	var _createContentContainer = function(contentElement)
	{
		var divElement = document.createElement("div");
		divElement.appendChild(contentElement);
		return divElement;
	};

	var _createContainer = function(element)
	{
		var _createCell = function(row, className, text)
		{
			var cell = row.insertCell();
			cell.className = className;
			cell.innerText = text || "";
			return cell;
		};

		var content = _createContentContainer(element);

		var table = document.createElement("table");
		var body = document.createElement("tbody");
		table.style.borderSpacing = 0;
		table.style.padding = 0;
		table.appendChild(body);

		var row, cell;
		row = body.insertRow();
		_createCell(row, "PatientBanner_topleft");
		_createCell(row, "PatientBanner_top");
		_createCell(row, "PatientBanner_topright");

		row = body.insertRow();
		_createCell(row, "PatientBanner_left");
		cell = _createCell(row, "PatientBanner_content");
		cell.appendChild(content);
		_createCell(row, "PatientBanner_right");

		row = body.insertRow();
		_createCell(row, "PatientBanner_bottomleft");
		_createCell(row, "PatientBanner_bottom");
		_createCell(row, "PatientBanner_bottomright");

		return table;
	};

	return {
		create: function (element)
		{
			// no need to create a contrainer if the element is hidden
			if (element.style.display == 'none')
				return;

			// Replace the element with the new element that is encased in the container.
			// We cannot simply use innerHTML because all the event handler of the element will not
			// be propagated to the new element.  Hence the DOM manipulation to preserve the handlers.
			var parent = element.parentNode;
			var nextSibling = element.nextSibling;
			var newElement = _createContainer(element);
			if (nextSibling)
				parent.insertBefore(newElement, nextSibling);
			else
				parent.appendChild(newElement);
		}
	};

}();

/*
 *	Create a summary of demographic information of a single patient profile.
 */
Preview.PatientDemographicsSection = function () {
	var _html = 
		'<table border="0" cellspacing="0" cellpadding="0" class="PatientDemographicsTable">'+
		'	<tr>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">Healthcard #: </td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="healthcard"/></td>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">Date of Birth:</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="dateOfBirth"/></td>'+
		'   </tr><tr>' +
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">Age:</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="age"/></td>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">Sex:</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="sex"/></td>'+
		'	</tr>'+
		'	<tr id="HomePhoneRow">'+
		'		<td class="ContactInfoDemographicsLabel" nowrap="nowrap">Home Phone:</td>'+
		'		<td colspan="3" class="ContactInfoDemographicsCell"><div id="currentHomePhone"/></td>'+
		'	</tr>'+
		'	<tr id="HomeAddressRow">'+
		'		<td class="ContactInfoDemographicsLabel" nowrap="nowrap">Home Address:</td>'+
		'		<td colspan="3" class="ContactInfoDemographicsCell" nowrap="nowrap"><div id="currentHomeAddress"/></td>'+
		'	</tr>'+
		'	<tr><td colspan="4"><img src="../images/blank.gif" height="10" /></td></tr>'
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
		'<table width="100%" border="0" cellspacing="0" cellpadding="0" class="PatientBannerTable">'+
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
			if(alerts)
			{
				alerts.each(function(item) { alertHtml += Preview.getAlertHtml(item, patientName); });
			}
			$("alerts").innerHTML = alertHtml;
			
			Preview.PatientBannner.create(element.parentNode);
		}
	};
}();

/*
 *	Create a order note view shoing author, post date, urgency, receipients and note body.
 *	Exposes:
 *		create(element, note)
 *			element - parent node for the order note
 *			note - the order note object
 */
Preview.OrderNoteSection = function() {
	var _formatStaffNameAndRoleAndOnBehalf = function(author, onBehalfOfGroup)
		{
			return Ris.formatStaffNameAndRole(author) + ((onBehalfOfGroup != null) ? (" on behalf of " + onBehalfOfGroup.Name) : "");
		};

	var _formatAcknowledgedTime = function(acknowledgedTime)
		{
			return acknowledgedTime ? (" at " + Ris.formatDateTime(acknowledgedTime)) : "";
		};
		
	var _formatAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "<br>";

			// create string of group acknowledgements
			var formattedGroups = String.combine(
				groups.map(function(r) 
					{
						return _formatStaffNameAndRoleAndOnBehalf(r.AcknowledgedByStaff, r.Group) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			// create string of staff acknowledgements
			// if staff already acknowledged for a group, no need to list it the second time in the staff recipients
			var groupAckStaffIds = groups.map(function(r) { return r.AcknowledgedByStaff.StaffId; }).unique();
			var formattedStaff = String.combine(
				staffs.map(function(r) 
					{ 
						var staffIdAlreadyExist = groupAckStaffIds.find(function(id) { return id == r.Staff.StaffId; });
						return staffIdAlreadyExist ? "" : Ris.formatStaffNameAndRole(r.Staff) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	var _formatNotAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "; ";

			var formattedGroups = String.combine(
				groups.map(function(r) { return r.Group.Name; }), 
				recipientSeparator);

			var formattedStaff = String.combine(
				staffs.map(function(r) { return Ris.formatStaffNameAndRole(r.Staff); }), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	return {
		create: function(element, note)
		{
			if(note == null)
				return;

			note.GroupRecipients = note.GroupRecipients || [];
			note.StaffRecipients = note.StaffRecipients || [];
			var acknowledgedGroups = note.GroupRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var acknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var notAcknowledgedGroups = note.GroupRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var notAcknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });

			var html = "";
			html += '<table style="{width:98%; margin-top: 4px;}" border="0" cellspacing="0" cellpadding="0">';
			html += '	<tr class="orderNoteHeading">';
			html += '		<td style="{width:100%;}"><span style="{padding-right: 5px;}" class="orderNoteLabel">From:</span>';
			html += '		' + _formatStaffNameAndRoleAndOnBehalf(note.Author, note.OnBehalfOfGroup) + '</td>';
			//html += '		<td>' + (note.Urgent ? "<img alt='Urgent' src='" + imagePath + "/urgent.gif'/>" : "") + '</td>';
			html += '		<td style="{width:5em; text-align:right; padding-right: 5px;}">' + (note.Urgent ? '<span class="urgentTextMark">URGENT</span>' : "")  + '</td>';
			html += '		<td style="{width:9.5em;text-align:right; padding-right: 5px;}" class="orderNoteLabel" NOWRAP title="' +  Ris.formatDateTime(note.PostTime) + '">' + Ris.formatDateTime(note.PostTime) + '</td>';
			html += '	</tr>';
			if (acknowledgedGroups.length > 0 || acknowledgedStaffs.length > 0)
			{
				html += '	<tr id="acknowledgedRow" class="orderNoteHeading">';
				html += '		<td colspan="3" NOWRAP valign="top"><span style="{padding-right: 5px;}" class="orderNoteLabel">Acknowledged By:</span>';
				html += '		' + _formatAcknowledged(acknowledgedGroups, acknowledgedStaffs).replaceLineBreak() + '<div id="acknowledged"></td>';
				html += '	</tr>';
			}
			if (notAcknowledgedGroups.length > 0 || notAcknowledgedStaffs.length > 0)
			{
				html += '	<tr id="notAcknowledgedRow" class="orderNoteHeading">';
				html += '		<td valign="top" colspan="3"><span style="{padding-right: 5px;}" class="orderNoteLabel">Waiting For Acknowledgement:</span>';
				html += '		<B>' + _formatNotAcknowledged(notAcknowledgedGroups, notAcknowledgedStaffs).replaceLineBreak() + '</B></td>';
				html += '	</tr>';
			}
			html += '	<tr>';
			html += '		<td colspan="3" class="orderNote">' +  note.NoteBody.replaceLineBreak() + '</td>';
			html += '	</tr>';
			html += '</table>';
			
			element.innerHTML = html;
		}
	};
}();

/*
 *	Create a conversation note that shows author, post date, urgency, receipients and note body.
 *	Exposes:
 *		create(element, note)
 *			element - parent node for the order note
 *			note - the order note object
 */
Preview.ConversationNote = function() {
	var _formatStaffNameAndRoleAndOnBehalf = function(author, onBehalfOfGroup)
		{
			return Ris.formatStaffNameAndRole(author) + ((onBehalfOfGroup != null) ? (" on behalf of " + onBehalfOfGroup.Name) : "");
		};

	var _formatAcknowledgedTime = function(acknowledgedTime)
		{
			return acknowledgedTime ? (" at " + Ris.formatDateTime(acknowledgedTime)) : "";
		};
		
	var _formatAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "<br>";

			// create string of group acknowledgements
			var formattedGroups = String.combine(
				groups.map(function(r) 
					{
						return _formatStaffNameAndRoleAndOnBehalf(r.AcknowledgedByStaff, r.Group) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			// create string of staff acknowledgements
			// if staff already acknowledged for a group, no need to list it the second time in the staff recipients
			var groupAckStaffIds = groups.map(function(r) { return r.AcknowledgedByStaff.StaffId; }).unique();
			var formattedStaff = String.combine(
				staffs.map(function(r) 
					{ 
						var staffIdAlreadyExist = groupAckStaffIds.find(function(id) { return id == r.Staff.StaffId; });
						return staffIdAlreadyExist ? "" : Ris.formatStaffNameAndRole(r.Staff) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	var _formatNotAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "; ";

			var formattedGroups = String.combine(
				groups.map(function(r) { return r.Group.Name; }), 
				recipientSeparator);

			var formattedStaff = String.combine(
				staffs.map(function(r) { return Ris.formatStaffNameAndRole(r.Staff); }), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	return {
		create: function(element, note)
		{			
			if(note == null)
				return;

			note.GroupRecipients = note.GroupRecipients || [];
			note.StaffRecipients = note.StaffRecipients || [];
			var acknowledgedGroups = note.GroupRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var acknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var notAcknowledgedGroups = note.GroupRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var notAcknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var checkBoxId = note.OrderNoteRef;

			var html = "";
			html += '<table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="ConversationNote_topleft"></td><td class="ConversationNote_top"></td><td class="ConversationNote_topright"></td></tr>';
			html += '<tr><td class="ConversationNote_left_upper"></td><td class="ConversationNote_content_upper">';
			html += '<table width="100%" class="ConversationNoteDetails" border="0" cellspacing="0" cellpadding="0">';
			html += '	<tr>';
			html += '		<td><span style="{color: #205F87; font-weight: bold; padding-right: 10px;}">From:</span> ' 
				+  _formatStaffNameAndRoleAndOnBehalf(note.Author, note.OnBehalfOfGroup) 
				//+ '<span style="{padding-left: 20px;}">' 
				//+ (note.Urgent ? "<img alt='Urgent' src='" + imagePath + "/urgent.gif'/>" : "") 
				+ (note.Urgent ? '<span class="urgentTextMark" style="{margin-left: 20px;}">URGENT</span>' : "") + "</td>";
				//+ '</span></td>';
			html += '		<td style="{padding-right: 10px; text-align:right; color: #205F87; font-weight: bold;}" NOWRAP title="' +  Ris.formatDateTime(note.PostTime) + '">' + Ris.formatDateTime(note.PostTime) + '</td>';
			html += '	</tr>';
			if (acknowledgedGroups.length > 0 || acknowledgedStaffs.length > 0) {
				html += '	<tr id="acknowledgedRow">';
				html += '		<td colspan="2" NOWRAP valign="top"><span style="{color: #205F87; font-weight: bold; padding-right: 10px;}">Acknowledged By:</span>';
				html += '		' + _formatAcknowledged(acknowledgedGroups, acknowledgedStaffs).replaceLineBreak() + '<div id="acknowledged"></td>';
				html += '	</tr>';
			}
			if (notAcknowledgedGroups.length > 0 || notAcknowledgedStaffs.length > 0) {
				html += '	<tr id="notAcknowledgedRow">';
				html += note.CanAcknowledge
						? '		<td valign="middle" colspan="2" ><input type="checkbox" id="' + checkBoxId + '"/><span style="{margin-left: 5px; margin-right: 10px;}">Waiting For Acknowledgement:</span>'
						: '		<td colspan="2" NOWRAP valign="top"><span style="{padding-right: 10px;}">Waiting For Acknowledgement:</span>';
				html += '		' + _formatNotAcknowledged(notAcknowledgedGroups, notAcknowledgedStaffs).replaceLineBreak() + '</td>';
				html += '	</tr>';
			}
			html += '   </table>';
			html += '   </td><td class="ConversationNote_right_upper"></td></tr>';
			html += '   <tr><td class="ConversationNote_left_lower"></td><td class="ConversationNote_content_lower"><table>'
			html += '	<tr>';
			html += '		<td colspan="4" style="{text-align:justify;}"><div class="ConversationNoteMessage">' +  note.NoteBody.replaceLineBreak() + '</div></td>';
			html += '	</tr>';
			html += '   </table>';
			html += '</td><td class="ConversationNote_right_lower"></td></tr>';
			html += '<tr><td class="ConversationNote_bottomleft"></td><td class="ConversationNote_bottom"></td><td class="ConversationNote_bottomright"></td></tr></table>';
			
			element.innerHTML = html;
		}
	};
}();

Preview.OrderedProceduresTable = function() {

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

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, "ProceduresTable");
			var htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
				[
					{	label: "Procedure",
						cellType: "html",
						getValue: function(item) 
						{
							var html = "";
							var formattedProcedureStatus = Preview.ProceduresTableHelper.formatProcedureStatus(item.Status, item.ScheduledStartTime, item.StartTime, item.CheckInTime, item.CheckOutTime);
							
							html += "<p class='sectionheading'>";
							html += "	<a href='javascript:void(0)' class='collapsibleHeading' onclick='Collapse.toggleCollapsed(this)'><span class='plusMinus'>+</span>" + Ris.formatProcedureName(item) + "</a>";
							html += "</p>";
							html += "<div class='collapsibleContent'>";
							html += "<table>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Status</td>";
							html += "	<td width='200'>" + formattedProcedureStatus + "</td>";
							html += "	<td width='120' class='propertyname'>Performing Facility</td>";
							html += "	<td width='200'>" + item.PerformingFacility.Name + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Scheduled Start Time</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.ScheduledStartTime) + "</td>";
							html += "	<td width='120' class='propertyname'>Check-In Time</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.CheckInTime) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Performing Start Time</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.StartTime) + "</td>";
							html += "	<td width='120' class='propertyname'>Performing End Time</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.CheckOutTime) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Report Published Time</td>";
							if(item.Status.Code == 'CA' || item.Status.Code == 'DC')
							{
								html += "	<td width='200'></td>";
							}
							else
							{
								html += "	<td width='200'>" + Ris.formatDateTime(item.EndTime) + "</td>";
							}
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Cancelled Time</td>";
							if(item.Status.Code == 'CA' || item.Status.Code == 'DC')
							{
								html += "	<td width='200'>" + Ris.formatDateTime(item.EndTime) + "</td>";
							}
							else
							{
								html += "	<td width='200'></td>";
							}
							html += "</tr>";
							html += "</table>";
							html += "</div>";

							return html;
						}
					}
				]);

			htmlTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
			htmlTable.bindItems(procedures);

			Preview.SectionContainer.create(parentElement, "Ordered Procedures");
		}
	}
}();

/*
 *	Create a table of details for a single visit.
 */ 
Preview.VisitDetailsSection = function () {

	var _html = 
		'<div class="SectionTableContainer">'+
		'	<table cellspacing="5">'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Visit Number</td>'+
		'			<td width="200"><div id="VisitNumber"/></td>'+
		'			<td width="120" class="propertyname">Facility</td>'+
		'			<td width="200"><div id="Facility"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Visit Status</td>'+
		'			<td width="200"><div id="VisitStatus"/></td>'+
		'			<td width="120" class="propertyname">Current Location</td>'+
		'			<td width="200"><div id="CurrentLocation"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Patient Class</td>'+
		'			<td width="200"><div id="PatientClass"/></td>'+
		'			<td width="120" class="propertyname">Patient Type</td>'+
		'			<td width="200"><div id="PatientType"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Admission Type</td>'+
		'			<td width="200"><div id="AdmissionType"/></td>'+
		'			<td width="120" class="propertyname">Discharge Disposition</td>'+
		'			<td width="200"><div id="DischargeDisposition"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Admit Date/Time</td>'+
		'			<td width="200"><div id="AdmitTime"/></td>'+
		'			<td width="120" class="propertyname">Discharge Date/Time</td>'+
		'			<td width="200"><div id="DischargeTime"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Pre-Admit Number</td>'+
		'			<td><div id="PreAdmitNumber"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">VIP?</td>'+
		'			<td><div id="VipFlag"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Ambulatory Statuses</td>'+
		'			<td><div id="AmbulatoryStatuses"/></td>'+
		'		</tr>'+
		'	</table>'+
		'</div>';

	return 	{
		create: function(element, visitDetail) {

			if(visitDetail == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("VisitNumber"), Ris.formatMrn(visitDetail.VisitNumber));
			Field.setValue($("Facility"), visitDetail.Facility.Name);
			Field.setValue($("VisitStatus"), visitDetail.Status.Value);
			Field.setValue($("AdmitTime"), Ris.formatDateTime(visitDetail.AdmitTime));
			Field.setValue($("DischargeTime"), Ris.formatDateTime(visitDetail.DischargeTime));
			Field.setValue($("PatientClass"), visitDetail.PatientClass.Value);
			Field.setValue($("PatientType"), visitDetail.PatientType.Value);
			Field.setValue($("AdmissionType"), visitDetail.AdmissionType.Value);
			Field.setValue($("DischargeDisposition"), visitDetail.DischargeDisposition);
			Field.setValue($("CurrentLocation"), visitDetail.CurrentLocation ? visitDetail.CurrentLocation.Name : null);
			Field.setValue($("PreAdmitNumber"), visitDetail.PreadmitNumber);
			Field.setValue($("VipFlag"), visitDetail.VipIndicator ? "Yes" : "No");
			
			var ambulatoryStatuses = String.combine(visitDetail.AmbulatoryStatuses.map(function(status) { return status.Value; }), ", ");
			Field.setValue($("AmbulatoryStatuses"), ambulatoryStatuses);

			Preview.SectionContainer.create(element, "Visit Details");
		}
	};
}();

/*
 *	Create a summary of the physician details of a single visit.
 */ 
Preview.PhysiciansSection = function () {

	var _html = 
		'<div class="SectionTableContainer">'+
		'	<table cellspacing="5">'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Attending Physician</td>'+
		'			<td><div id="AttendingPhysician"/></td>'+
		'			<td width="120" class="propertyname">Referring Physician</td>'+
		'			<td><div id="ReferringPhysician"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">Consulting Physician</td>'+
		'			<td><div id="ConsultingPhysician"/></td>'+
		'			<td width="120" class="propertyname">Admitting Physician</td>'+
		'			<td><div id="AdmittingPhysician"/></td>'+
		'		</tr>'+
		'	</table>'+
		'</div>';

	return 	{
		create: function(element, visitDetail) {

			if(visitDetail == null || visitDetail.ExtendedProperties == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("AttendingPhysician"), visitDetail.ExtendedProperties.AttendingPhysician);
			Field.setValue($("ReferringPhysician"), visitDetail.ExtendedProperties.ReferringPhysician);
			Field.setValue($("ConsultingPhysician"), visitDetail.ExtendedProperties.ConsultingPhysician);
			Field.setValue($("AdmittingPhysician"), visitDetail.ExtendedProperties.AdmittingPhysician);

			Preview.SectionContainer.create(element, "Physicians");
		}
	};
}();

Preview.ExternalPractitionerSummary = function() {

	var _detailsHtml = 
		'<table cellspacing="5" class="PatientDemographicsTable">'+
		'	<tr>'+
		'		<td width="150" class="DemographicsLabel">License Number</td>'+
		'		<td><div id="LicenseNumber"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="150" class="DemographicsLabel">Billing Number</td>'+
		'		<td><div id="BillingNumber"/></td>'+
		'	</tr>'+
		'	</table>';

	var _createBanner = function(element, externalPractitionerSummary)
		{
			var bannerContainer = document.createElement("div");
			element.appendChild(bannerContainer);

			var bannerName = document.createElement("div");
			bannerContainer.appendChild(bannerName);
			Preview.BannerSection.create(bannerName, Ris.formatPersonName(externalPractitionerSummary.Name), "");

			var bannerDetails = document.createElement("div");
			bannerDetails.innerHTML = _detailsHtml;
			bannerContainer.appendChild(bannerDetails);

			Field.setValue($("LicenseNumber"), externalPractitionerSummary.LicenseNumber);
			Field.setValue($("BillingNumber"), externalPractitionerSummary.BillingNumber);
		};

	var _createContactPointTable = function(element, externalPractitionerSummary)
		{
			if(!externalPractitionerSummary.ContactPoints)
				return;

			var activeContactPoints = externalPractitionerSummary.ContactPoints.select(function(item) { return item.Deactivated == false; });

			var htmlTable = Preview.ProceduresTableHelper.addTable(element, "ContactPointsTable");
			var htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
				[
					{
						label: "Name",
						cellType: "html",
						getValue: function(item)
						{
							var name;
							if(item.IsDefaultContactPoint)
								name = item.Name + " [Default]";
							else
								name = item.Name;
							return "<div class='DemographicsLabel'>" + name + "</div>"
						}
					},
					{	label: "Contact Point",
						cellType: "html",
						getValue: function(item) 
						{
							var html = "";

							html += "<table>";
							if(item.Description)
							{
								html += "<tr>";
								html += "	<td width='120' class='propertyname'>Description</td>";
								html += "	<td >" + item.Description + "</td>";
								html += "</tr>";
							}
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Phone Number</td>";
							html += "	<td>" + (Ris.formatTelephone(item.CurrentPhoneNumber) || "Not entered") + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Fax Number</td>";
							html += "	<td>" + (Ris.formatTelephone(item.CurrentFaxNumber) || "Not entered") + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Address</td>";
							html += "	<td>" + (Ris.formatAddress(item.CurrentAddress) || "Not entered") + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>Email Address</td>";
							html += "	<td>" + (item.CurrentEmailAddress && item.CurrentEmailAddress.Address ? item.CurrentEmailAddress.Address : "Not entered") + "</td>";
							html += "</tr>";

							html += "</table>";

							return html;
						}
					}
				]);
			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(activeContactPoints);

			Preview.SectionContainer.create(htmlTable, "Contact Points");
		};

	return {
		create: function(element, externalPractitionerSummary) {

			if(externalPractitionerSummary == null)
				return;

			_createBanner(element, externalPractitionerSummary);
			_createContactPointTable(element, externalPractitionerSummary);
		}
	};
}();