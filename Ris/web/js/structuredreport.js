var data = {};
var reportType = "unknown";
var fetus = 0;

function initStructuredReport(source)
{
	data = source || {};

	data.indicationsAndDates = data.indicationsAndDates || {};
	data.indicationsAndDates.lmp = data.indicationsAndDates.lmp || {};
	data.indicationsAndDates.us = data.indicationsAndDates.us || {};
	data.indicationsAndDates.establishedEDC = data.indicationsAndDates.establishedEDC || {};
	data.general = data.general || {};
	data.biometry = data.biometry || [];
	data.anatomy = data.anatomy || [];
	data.cardiac = data.cardiac || [];
	data.wellBeing = data.wellBeing || [];
	data.commentsConclusion = data.commentsConclusion || {};
	EnsurePerFetusDataExists();
	
	if(data.obusReportType == null)
	{
		document.getElementById("TypeSelection").style.display = 'block';
		document.getElementById("T1T2T3Doc").style.display = 'none';
		document.getElementById("fetusSelect").style.display = 'none';
		initTypeSelectionTable();
	}
	else
	{
		if(document.getElementById("TypeSelection")) document.getElementById("TypeSelection").style.display = 'none';
		if(document.getElementById("T1T2T3Doc")) document.getElementById("T1T2T3Doc").style.display = 'block';
		if(document.getElementById("fetusSelect")) document.getElementById("fetusSelect").style.display = 'block';
		if(document.getElementById("reportType")) document.getElementById("reportType").innerHTML = data.obusReportType;
		initTabs();
	}
}

function EnsurePerFetusDataExists()
{
	// Ensure all per fetus objects are defined
	// If the fetalNumber was previously higher than it is now and data was saved, these extra objects
	// also need to be initialized
	var max = data.general.fetalNumber > data.biometry.length ? data.general.fetalNumber : data.biometry.length;
	
	for(var i = 0; i < max; i++)
	{
		data.biometry[i] = data.biometry[i] || {};
	}
	for(var i = 0; i < max; i++)
	{
		data.anatomy[i] = data.anatomy[i] || {};
	}
	for(var i = 0; i < max; i++)
	{
		data.cardiac[i] = data.cardiac[i] || {};
	}
	for(var i = 0; i < max; i++)
	{
		data.wellBeing[i] = data.wellBeing[i] || {};
	}
}

function initTypeSelectionTable()
{
	var table = Table.createTable(
		$("typeSelectionTable"),
		{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: "Select report type and confirm",
				cellType: "choice",
				choices: ["T1", "T2", "T3"],
				getValue: function(item){ return item.obusReportType; },
				setValue: function(item, value){ item.obusReportType = value; },
				getError: function(item){ return null; }
			}
		]);

	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data]);
}

function initIndicationsAndDates()
{
	var indicationChoices;
	if(reportType == "T1")
	{
		indicationChoices = ["Dating", "NT", "Other"];
	}
	else if(reportType == "T2")
	{
		indicationChoices = ["Anatomy", "Suspected Anomaly", "Other"];
	}
	else
	{
		indicationChoices = ["Growth/Well-being", "Other"];
	}

	var table = Table.createTable($("indicationsAndDatesTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Indication",
			cellType: "choice",
			choices: indicationChoices,
			getValue: function(item) {	return item.indication; },
			setValue: function(item, value) { item.indication = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Clinical Details",
			cellType: "textarea",
			cols: 110,
			rows: 5,
			getValue: function(item) { return item.clinicalDetails; },
			setValue: function(item, value) { item.clinicalDetails = value; },
			getError: function(item) { return null; }
		}
	]);
	
	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.indicationsAndDates]);
	
	var lmpTable = Table.createTable($("lmpTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "LMP",
			cellType: "datetime",
			getValue: function(item) { return item.LMP; },
			setValue: function(item, value) { item.LMP = value; },
			getError: function(item) { return null; }
		},
		{
			label: "EDC",
			cellType: "readonly",
			getVisible: function(item) { return item.LMP && item.LMP != null; },
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return; }
		},
		{
			label: "Age from dates",
			cellType: "readonly",
			getVisible: function(item) { return item.LMP && item.LMP != null; },
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return; }
		}
	]);
	
	lmpTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	lmpTable.bindItems([data.indicationsAndDates.lmp]);

	var usTable = Table.createTable($("usTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[						
		{
			label: "1st Ultrasound",
			cellType: "datetime",
			getValue: function(item) { return item.firstUltrasound; },
			setValue: function(item, value) { item.firstUltrasound = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Number of weeks",
			cellType: "text",
			getValue: function(item) { return item.numberOfWeeks; },
			setValue: function(item, value) { item.numberOfWeeks = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Today",
			cellType: "text",
			getValue: function(item) { return item.today; },
			setValue: function(item, value) { item.today = value; },
			getError: function(item) { return null; }
		},
		{
			label: "EDC",
			cellType: "readonly",
			getValue: function(item) { return "CALC"; }
		}
	]);
	
	usTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	usTable.bindItems([data.indicationsAndDates.us]);

	var establishedEDCTable = Table.createTable($("establishedEDCTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "EDC",
			cellType: "datetime",
			getValue: function(item) { return item.establishedEDC; },
			setValue: function(item, value) { item.establishedEDC = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Age from EDC",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getVisible: function(item) { return item.establishedEDC && item.establishedEDC != null; }
		},
		{
			label: "How Determined",
			cellType: "choice",
			choices: ["Dates", "Dates & US", "IVF", "US"],
			getValue: function(item) { return item.edcMethod; },
			setValue: function(item, value) { item.edcMethod = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Transferred Date",
			cellType: "datetime",
			getValue: function(item) { return item.transferredDate; },
			setValue: function(item, value) { item.transferredDate = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return item.edcMethod && item.edcMethod == "IVF"; }
		}
	]);

	establishedEDCTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	establishedEDCTable.bindItems([data.indicationsAndDates.establishedEDC]);
}

function initGeneral()
{
	var table = Table.createTable($("generalTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Visibility",
			cellType: "choice",
			choices: ["Satisfactory", "Sub-optimal", "Moderate"],
			getValue: function(item) { return item.visibility; },
			setValue: function(item, value) { item.visibility = value; },
			getError: function(item) { return null; }
		},
		// layout placeholder
		{
			label: "",
			cellType: "readonly",
			getValue: function(item) { return ""; },
			setValue: function(item, value) { return; },
			getError: function(item) { return null; }
		},
		{
			label: "Fetal Number",
			cellType: "text",
			getValue: function(item) { return item.fetalNumber = item.fetalNumber || "1"; },
			setValue: function(item, value) { item.fetalNumber = value; UpdateFetalNumber(parseInt(item.fetalNumber)); },
			getError: function(item) { return null; }
		},
		{
			label: "Twin Type",
			cellType: "choice",
			choices: ["Diamniotic", "dichorionic", "indeterminate", "monoamniotic", "monochorionic", "monochorionic dia", "see comment"],
			getValue: function(item) { return item.twinType; },
			setValue: function(item, value) { item.twinType = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return parseInt(item.fetalNumber) > 1; }
		},
		// layout placeholder
		{
			label: "",
			cellType: "readonly",
			getValue: function(item) { return ""; },
			setValue: function(item, value) { return; },
			getVisible: function(item) { return parseInt(item.fetalNumber) <= 1; },
			getError: function(item) { return null; }
		},
		{
			label: "FH Activity",
			cellType: "choice",
			choices: ["Absent", "Present"],
			getValue: function(item) { return item.fhActivity; },
			setValue: function(item, value) { item.fhActivity = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Are you sure FH activity is absent?",
			cellType: "choice",
			choices: ["Yes", "No"],
			getValue: function(item) { return item.fhActivityConfirmation; },
			setValue: function(item, value) { item.fhActivityConfirmation = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return item.fhActivity == "Absent"; }					
		},
		// layout placeholder
		{
			label: "",
			cellType: "readonly",
			getValue: function(item) { return ""; },
			setValue: function(item, value) { return; },
			getVisible: function(item) { return item.fhActivity != "Absent"; },
			getError: function(item) { return null; }
		},
		{
			label: "Placenta",
			cellType: "choice",
			choices: ["Anterior", "Fundal", "Left Lateral", "Right Lateral", "Posterior"],
			getValue: function(item) { return item.placenta; },
			setValue: function(item, value) { item.placenta = value; },
			getError: function(item) { return null; }
		},
		{
			label: "??",
			cellType: "choice",
			choices: ["Clear of Cervix", "Close to Cervix", "Praevia"],
			getValue: function(item) { return item.cervixProximity; },
			setValue: function(item, value) { item.cervixProximity = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Right Adnexa",
			cellType: "choice",
			choices: ["Normal", "Unseen", "See comments"],
			getValue: function(item) { return item.rightAdnexa; },
			setValue: function(item, value) { item.rightAdnexa = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T1"; }
		},
		{
			label: "Left Adnexa",
			cellType: "choice",
			choices: ["Normal", "Unseen", "See comments"],
			getValue: function(item) { return item.leftAdnexa; },
			setValue: function(item, value) { item.leftAdnexa = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T1"; }
		},
		{
			label: "Yolk Sac",
			cellType: "choice",
			choices: ["Normal", "Unseen", "See comments"],
			getValue: function(item) { return item.yolkSac; },
			setValue: function(item, value) { item.yolkSac = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T1"; }
		},
		{
			label: "Presentation",
			cellType: "choice",
			choices: ["Breech", "Cephalic", "Oblique", "Transverse"],
			getValue: function(item) { return item.presentation; },
			setValue: function(item, value) { item.presentation = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T3"; }
		},
		{
			label: "AFV",
			cellType: "choice",
			choices: ["Decreased", "Increased", "Normal", "Subjectively Decreased", "Subjectively Increased"],
			getValue: function(item) { return item.afv; },
			setValue: function(item, value) { item.afv = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2"; }
		},
		{
			label: "Cervix",
			cellType: "choice",
			choices: ["Funnelling", "Normal", "Not Assessed", "Open", "Unseen"],
			getValue: function(item) { return item.cervix; },
			setValue: function(item, value) { item.cervix = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "Apposed Length",
			cellType: "text",
			getValue: function(item) { return item.apposedLength; },
			setValue: function(item, value) { item.apposedLength = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "Cervix Assessed",
			cellType: "choice",
			choices: ["Abdominally", "Labially", "Vaginally"],
			getValue: function(item) { return item.cervixAssessed; },
			setValue: function(item, value) { item.cervixAssessed = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		}
	]);

	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.general]);

	UpdateFetalNumber(parseInt(data.general.fetalNumber) || 1);
}

function initBiometry()
{
	var biometryAssessedTable = Table.createTable($("biometryAssessedTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Biometry",
			cellType: "choice",
			choices: ["Assessed", "Not Assessed"],
			getValue: function(item) { return item.assessed; },
			setValue: function(item, value) { item.assessed = value; OnBiometryChanged(value == "Assessed"); },
			getError: function(item) { return null; }
		}
	]);
	
	biometryAssessedTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var crlTable = Table.createTable($("crlTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			size: 13,
			cellType: "text",
			getValue: function(item) { return item.crl; },
			setValue: function(item, value) { item.crl = value; },
			getVisible: function(item) { return reportType == "T1"; },
			getError: function(item) { return null; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getVisible: function(item) { return reportType == "T1"; },
			getError: function(item) { return null; }
		}
	]);
	
	crlTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var bpdTable = Table.createTable($("bpdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "text",
			size: 13,
			getValue: function(item) { return item.bpd; },
			setValue: function(item, value) { item.bpd = value; },
			getError: function(item) { return null; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; }					
		}
	]);

	bpdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var ofdTable = Table.createTable($("ofdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "text",
			size: 13,
			getValue: function(item) { return item.ofd; },
			setValue: function(item, value) { item.ofd = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		}
	]);

	ofdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var correctedBpdTable = Table.createTable($("correctedBpdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; },
			getError: function(item) { return null; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; },
			getError: function(item) { return null; }
		}
	]);

	correctedBpdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var hcTable = Table.createTable($("hcTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "text",
			size: 13,
			getValue: function(item) { return item.hc; },
			setValue: function(item, value) { item.hc = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC	"; },
			setValue: function(item, value) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; },
			getError: function(item) { return null; }
		}
	]);

	hcTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var abdTable = Table.createTable($("abdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "", 
			cellType: "text",
			size: 4,
			getValue: function(item) { return item.abdX; },
			setValue: function(item, value) { item.abdX = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "", 
			cellType: "readonly",
			getValue: function(item) { return "+"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "", 
			cellType: "text",
			size: 4,
			getValue: function(item) { return item.abdY; },
			setValue: function(item, value) { item.abdY = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "mm", 
			cellType: "text",
			size: 13,
			getValue: function(item) { return item.abdAC; },
			setValue: function(item, value) { item.abdAC = value; },
			getError: function(item) { return item.abdAC; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		}
	]);

	abdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var flTable = Table.createTable($("flTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "text",
			size: 13,
			getValue: function(item) { return item.fl; },
			setValue: function(item, value) { item.fl = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		}
	]);

	flTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var averageSizeTable = Table.createTable($("averageSizeTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; },
			getError: function(item) { return null; }
		}
	]);

	averageSizeTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var nuchalTransparencyTable = Table.createTable($("nuchalTransparencyTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "mm", 
			cellType: "text",
			getValue: function(item) { return item.nuchalTransparency; },
			setValue: function(item, value) { item.nuchalTransparency = value; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2"; }
		}
	]);

	nuchalTransparencyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	
	bindBiometry();
}

function bindBiometry()
{
	data.biometry[fetus] = data.biometry[fetus] || {};

	biometryAssessedTable.bindItems([data.biometry[fetus]]);
	OnBiometryChanged(data.biometry[fetus].assessed == "Assessed");
	
	crlTable.bindItems([data.biometry[fetus]]);
	bpdTable.bindItems([data.biometry[fetus]]);
	ofdTable.bindItems([data.biometry[fetus]]);
	correctedBpdTable.bindItems([data.biometry[fetus]]);
	hcTable.bindItems([data.biometry[fetus]]);
	abdTable.bindItems([data.biometry[fetus]]);
	flTable.bindItems([data.biometry[fetus]]);
	averageSizeTable.bindItems([data.biometry[fetus]]);
	nuchalTransparencyTable.bindItems([data.biometry[fetus]]);
}

function OnBiometryChanged(show)
{
	document.getElementById("crlTable").style.display = (show && reportType == "T1") ? "block" : "none";
	document.getElementById("bpdTable").style.display = (show) ? "block" : "none";
	document.getElementById("ofdTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("correctedBpdTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("hcTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("abdTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("flTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("averageSizeTable").style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("nuchalTransparencyTable").style.display = (show && reportType == "T2") ? "block" : "none";
}

function initAnatomy()
{
	var standardAnswers = ["Normal", "Not seen", "See comment"];
	var defaultAnswer = 0;

	var anatomyTable = Table.createTable($("anatomyTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Anatomy",
			cellType: "choice",
			choices: ["Assessed", "Not assessed", "Not done because previously assessed"],
			getValue: function(item) { return item.assessed; },
			setValue: function(item, value) { item.assessed = value; OnAnatomyChanged(value == "Assessed"); },
			getError: function(item) { return null; }
		}
	]);
	
	anatomyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var headShapeTable = Table.createTable($("headShapeTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Head shape",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.headShape; },
			setValue: function(item, value) { item.headShape = value; },
			getError: function(item) { return null; }
		}
	]);			
	
	headShapeTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var headTable = Table.createTable($("headTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		{
			label: "Posterior horns",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.posteriorHorns; },
			setValue: function(item, value) { item.posteriorHorns = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Choroid plexi",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.choroidPlexi; },
			setValue: function(item, value) { item.choroidPlexi = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Cavum septi",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.cavumSepti; },
			setValue: function(item, value) { item.cavumSepti = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Cerebellum",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.cerebellum; },
			setValue: function(item, value) { item.cerebellum = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Cisterna magna",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.cisternaMagna; },
			setValue: function(item, value) { item.cisternaMagna = value; },
			getError: function(item) { return null; }
		}
	]);
	
	headTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var nuchalFoldTable = Table.createTable($("nuchalFoldTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Nuchal fold",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.nuchalFold; },
			setValue: function(item, value) { item.nuchalFold = value; },
			getError: function(item) { return null; }
		}
	]);
	
	nuchalFoldTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var faceTable = Table.createTable($("faceTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Eyes",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.eyes; },
			setValue: function(item, value) { item.eyes = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Orbital ratio (i/o)",
			cellType: "text", 
			getValue: function(item) { return item.orbitalRatio },
			setValue: function(item, value) { item.orbitalRatio = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Mouth",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.mouth; },
			setValue: function(item, value) { item.mouth = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Profile",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.profile; },
			setValue: function(item, value) { item.profile = value; },
			getError: function(item) { return null; }
		}
	]);
	
	faceTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var spineTable = Table.createTable($("spineTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Spine",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.spine; },
			setValue: function(item, value) { item.spine = value; },
			getError: function(item) { return null; }
		}
	]);

	spineTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var heartTable = Table.createTable($("heartTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "4 Chambers",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.fourChambers; },
			setValue: function(item, value) { item.fourChambers = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Great vessels",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.greatVessels; },
			setValue: function(item, value) { item.greatVessels = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Detailed Cardiac",
			cellType: "choice", 
			choices: ["Yes", "No"],
			getValue: function(item) { return item.detailedCardiac; },
			setValue: function(item, value) { item.detailedCardiac = value; ShowDetailedCardiac(value == "Yes"); },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		}
	]);
	
	ShowDetailedCardiac(data.anatomy.detailedCardiac);

	heartTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var chestTable = Table.createTable($("chestTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Chest",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.chest; },
			setValue: function(item, value) { item.chest = value; },
			getError: function(item) { return null; }
		}
	]);
	
	chestTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var abdomenTable = Table.createTable($("abdomenTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Diaphragm",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.diaphragm; },
			setValue: function(item, value) { item.diaphragm = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Stomach",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.stomach; },
			setValue: function(item, value) { item.stomach = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Abdominal wall",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.abdominalWall; },
			setValue: function(item, value) { item.abdominalWall = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Kidneys - RT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.kidneysRt; },
			setValue: function(item, value) { item.kidneysRt = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Kidneys - LT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.kidneysLt; },
			setValue: function(item, value) { item.kidneysLt = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Bladder",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.bladder; },
			setValue: function(item, value) { item.bladder = value; },
			getError: function(item) { return null; }
		}
	]);
	
	abdomenTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var genitaliaTable = Table.createTable($("genitaliaTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Genitalia",
			cellType: "choice", 
			choices: ["Female", "Male", "Not seen", "See comment"],
			getValue: function(item) { return item.genitalia = item.genitalia || "Female"; },
			setValue: function(item, value) { item.genitalia = value; },
			getError: function(item) { return null; }
		}
	]);
	
	genitaliaTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var extremitiesTable = Table.createTable($("extremitiesTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Upper - RT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.upperRt; },
			setValue: function(item, value) { item.upperRt = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Upper - LT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.upperLt; },
			setValue: function(item, value) { item.upperLt = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Lower - RT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.lowerRt; },
			setValue: function(item, value) { item.lowerRt = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Lower - LT",
			cellType: "choice", 
			choices: standardAnswers,
			getValue: function(item) { return item.lowerLt; },
			setValue: function(item, value) { item.lowerLt = value; },
			getError: function(item) { return null; }
		}
	]);
	
	extremitiesTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var cordVesselsTable = Table.createTable($("cordVesselsTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Cord Vessels",
			cellType: "choice", 
			choices: ["2", "3"],
			getValue: function(item) { return item.cordVessels = item.cordVessels || "2"; },
			setValue: function(item, value) { item.cordVessels = value; },
			getError: function(item) { return null; }
		}
	]);

	cordVesselsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	bindAnatomy();
}

function bindAnatomy()
{
	data.anatomy[fetus] = data.anatomy[fetus] || {};

	anatomyTable.bindItems([data.anatomy[fetus]]);
	OnAnatomyChanged(data.anatomy[fetus].assessed == "Assessed");

	headShapeTable.bindItems([data.anatomy[fetus]]);
	headTable.bindItems([data.anatomy[fetus]]);
	nuchalFoldTable.bindItems([data.anatomy[fetus]]);
	faceTable.bindItems([data.anatomy[fetus]]);
	spineTable.bindItems([data.anatomy[fetus]]);
	heartTable.bindItems([data.anatomy[fetus]]);
	ShowDetailedCardiac(data.anatomy[fetus].detailedCardiac == "Yes");
	
	chestTable.bindItems([data.anatomy[fetus]]);
	abdomenTable.bindItems([data.anatomy[fetus]]);
	genitaliaTable.bindItems([data.anatomy[fetus]]);
	extremitiesTable.bindItems([data.anatomy[fetus]]);
	cordVesselsTable.bindItems([data.anatomy[fetus]]);
}

function OnAnatomyChanged(show)
{
	var display = show ? "block" : "none";

	document.getElementById("headShapeTable").style.display = display;
	document.getElementById("headTable").style.display = display;
	document.getElementById("nuchalFoldTable").style.display = display;
	document.getElementById("faceTable").style.display = display;
	document.getElementById("spineTable").style.display = display;
	document.getElementById("heartTable").style.display = display;
	document.getElementById("chestTable").style.display = display;
	document.getElementById("abdomenTable").style.display = display;
	document.getElementById("genitaliaTable").style.display = display;
	document.getElementById("extremitiesTable").style.display = display;
	document.getElementById("cordVesselsTable").style.display = display;
}

function ShowDetailedCardiac(show)
{
	if(document.getElementById("CardiacTab")) document.getElementById("CardiacTab").style.display = show ? "inline" : "none";
}

function initCardiac()
{
	var standardChoices = ["Normal", "Not seen", "See comment"];
	var defaultChoice = 0;

	var fourChamberViewTable = Table.createTable($("fourChamberViewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Heart/Thoracic Ratio",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.heartThoracicRatio ? item.heartThoracicRatio : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.heartThoracicRatio = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Ventricals symmetrical in size",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.ventricalsSymmetrical ? item.ventricalsSymmetrical : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.ventricalsSymmetrical = value; },
			getError: function(item) { return null; }
		},
		{
			label: "RV Diameter",
			cellType: "text",
			getValue: function(item) { return item.rvDiameter; },
			setValue: function(item, value) { item.rvDiameter = value; },
			getError: function(item) { return null; }
		},
		{
			label: "LV Diameter",
			cellType: "text",
			choices: standardChoices,
			getValue: function(item) { return item.lvDiameter; },
			setValue: function(item, value) { item.lvDiameter = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Thin and mobile valves",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.thinMobileValves ? item.thinMobileValves : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.thinMobileValves = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Function and rhythm",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.functionAndRhythm ? item.functionAndRhythm : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.functionAndRhythm = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Ventricular septum/crux",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.ventricularSeptumCrux ? item.ventricularSeptumCrux : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.ventricularSeptumCrux = value; },
			getError: function(item) { return null; }
		}
	]);

	fourChamberViewTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var outflowTractsTable = Table.createTable($("outflowTractsTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Two Outlets",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.twoOutlets ? item.twoOutlets : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.twoOutlets = value; },
			getError: function(item) { return null; }
		},
		{
			label: "PA \"crosses\" AO",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.paCrossesAO ? item.paCrossesAO : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.paCrossesAO = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Outlets symmetry",
			cellType: "choice",
			choices: standardChoices,
			getValue: function(item) { return item.outletsSymmetry ? item.outletsSymmetry : standardChoices[defaultChoice]; },
			setValue: function(item, value) { item.outletsSymmetry = value; },
			getError: function(item) { return null; }
		},
		{
			label: "AO diameter",
			cellType: "text",
			getValue: function(item) { return item.aoDiameter; },
			setValue: function(item, value) { item.aoDiameter = value; },
			getError: function(item) { return null; }
		},
		{
			label: "PO diameter",
			cellType: "text",
			getValue: function(item) { return item.poDiameter; },
			setValue: function(item, value) { item.poDiameter = value; },
			getError: function(item) { return null; }
		}
	]);

	outflowTractsTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

	bindCardiac();
}

function bindCardiac()
{
	data.cardiac[fetus] = data.cardiac[fetus] || {};

	fourChamberViewTable.bindItems([data.cardiac[fetus]]);
	outflowTractsTable.bindItems([data.cardiac[fetus]]);
}

function initWellBeing()
{
	var wellBeingTable = Table.createTable($("wellBeingTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Well-being",
			cellType: "choice",
			choices: ["Assessed", "Not Assessed"],
			getValue: function(item) { return item.assessed ? item.assessed : "Not Assessed"; },
			setValue: function(item, value) { item.assessed = value; OnWellBeingChanged(value == "Assessed"); },
			getError: function(item) { return null; }
		}
	]);

	wellBeingTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var bpsTable = Table.createTable($("bpsTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "AFV",
			cellType: "choice",
			choices: ["Decreased", "Increased", "Normal", "Subjectively decreased", "Subjectively increased"],
			getValue: function(item) { return item.afv; },
			setValue: function(item, value) { item.afv = value; },
			getError: function(item) { return null; }
		},
		{
			label: "??",
			cellType: "text",
			getValue: function(item) { return item.unknown; },
			setValue: function(item, value) { item.unknown = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Max vertical pocket",
			cellType: "text",
			getValue: function(item) { return item.maxVerticalPocket; },
			setValue: function(item, value) { item.maxVerticalPocket = value; },
			getError: function(item) { return null; }
		},
		{
			label: "AFI (A+B+C+D)",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getError: function(item) { return null; }
		},
		{
			label: "FM",
			cellType: "text",
			getValue: function(item) { return item.fm; },
			setValue: function(item, value) { item.fm = value; },
			getError: function(item) { return null; }
		},
		{
			label: "FT",
			cellType: "text",
			getValue: function(item) { return item.ft; },
			setValue: function(item, value) { item.ft = value; },
			getError: function(item) { return null; }
		},
		{
			label: "FBM",
			cellType: "text",
			getValue: function(item) { return item.fbm; },
			setValue: function(item, value) { item.fbm = value; },
			getError: function(item) { return null; }
		},
		{
			label: "NST",
			cellType: "text",
			getValue: function(item) { return item.nst; },
			setValue: function(item, value) { item.nst = value; },
			getError: function(item) { return null; }
		},
		{
			label: "BPS",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getError: function(item) { return null; }
		},
		{
			label: "??",
			cellType: "choice",
			choices: ["Abnormal", "Equivocal", "Normal"],
			getValue: function(item) { return item.unknown2; },
			setValue: function(item, value) { item.unknown2 = value; },
			getError: function(item) { return null; }
		}
	]);

	bpsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	
	var dopplerUmbilicalArteryTable = Table.createTable($("dopplerUmbilicalArteryTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "PI",
			cellType: "text",
			getValue: function(item) { return item.umbilicalPi; },
			setValue: function(item, value) { item.umbilicalPi = value; },
			getError: function(item) { return null; }
		},
		{
			label: "EDF",
			cellType: "choice",
			choices: ["Absent EDF", "Elevated", "Normal", "Reversed EDF"],
			getValue: function(item) { return item.edf; },
			setValue: function(item, value) { item.edf= value; },
			getError: function(item) { return null; }
		}
	]);

	dopplerUmbilicalArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerUmbilicalVeinTable = Table.createTable($("dopplerUmbilicalVeinTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "UVVmax",
			cellType: "text",
			getValue: function(item) { return item.uvvMax; },
			setValue: function(item, value) { item.uvvMax = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Pulsations",
			cellType: "choice",
			choices: ["Absent", "Present"],
			getValue: function(item) { return item.pulsations; },
			setValue: function(item, value) { item.pulsations = value; },
			getError: function(item) { return null; }
		}
	]);

	dopplerUmbilicalVeinTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerUterineArteryTable = Table.createTable($("dopplerUterineArteryTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "PI - Left",
			cellType: "text",
			getValue: function(item) { return item.leftUterinePi; },
			setValue: function(item, value) { item.leftUterinePi = value; },
			getError: function(item) { return null; }
		},
		{
			label: "PI - Right",
			cellType: "text",
			getValue: function(item) { return item.rightUterinePi; },
			setValue: function(item, value) { item.rightUterinePi = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Notch - Left",
			cellType: "choice",
			choices: ["Yes", "No"],
			getValue: function(item) { return item.leftUterinNotch; },
			setValue: function(item, value) { item.leftUterinNotch = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Notch - Right",
			cellType: "choice",
			choices: ["Yes", "No"],
			getValue: function(item) { return item.rightUterinNotch; },
			setValue: function(item, value) { item.rightUterinNotch = value; },
			getError: function(item) { return null; }
		}
	]);

	dopplerUterineArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerMiddleCerebralArteryTable = Table.createTable($("dopplerMiddleCerebralArteryTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "PI",
			cellType: "text",
			getValue: function(item) { return item.middleCerebralPi; },
			setValue: function(item, value) { item.middleCerebralPi = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Peak Systolic Velocity",
			cellType: "text",
			getValue: function(item) { return item.peakSystolicVelocity; },
			setValue: function(item, value) { item.peakSystolicVelocity = value; },
			getError: function(item) { return null; }
		}
	]);

	dopplerMiddleCerebralArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var placentaTable = Table.createTable($("placentaTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Max Length",
			cellType: "text",
			getValue: function(item) { return item.maxLength; },
			setValue: function(item, value) { item.maxLength = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Max Depth",
			cellType: "text",
			getValue: function(item) { return item.maxDepth; },
			setValue: function(item, value) { item.maxDepth = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Grannum grade",
			cellType: "choice",
			choices: ["0", "1", "2", "3"],
			getValue: function(item) { return item.grannumGrade; },
			setValue: function(item, value) { item.grannumGrade = value; },
			getError: function(item) { return null; }
		},
		{
			label: "Texture",
			cellType: "choice",
			choices: ["Abnormal", "Normal"],
			getValue: function(item) { return item.texture ? item.texture : "Normal"; },
			setValue: function(item, value) { item.texture = value; },
			getError: function(item) { return null; }
		}
	]);

	placentaTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	bindWellBeing();
}

function bindWellBeing()
{
	data.wellBeing[fetus] = data.wellBeing[fetus] || {};
	
	wellBeingTable.bindItems([data.wellBeing[fetus]]);
	OnWellBeingChanged(data.wellBeing[fetus].assessed == "Assessed");
	
	bpsTable.bindItems([data.wellBeing[fetus]]);
	dopplerUmbilicalArteryTable.bindItems([data.wellBeing[fetus]]);
	dopplerUmbilicalVeinTable.bindItems([data.wellBeing[fetus]]);
	dopplerUterineArteryTable.bindItems([data.wellBeing[fetus]]);
	dopplerMiddleCerebralArteryTable.bindItems([data.wellBeing[fetus]]);
	placentaTable.bindItems([data.wellBeing[fetus]]);
}

function OnWellBeingChanged(show)
{
	var display = show ? "block" : "none";

	document.getElementById("bpsTable").style.display = display;
	document.getElementById("dopplerHeading").style.display = display;
	document.getElementById("dopplerUmbilicalArteryTable").style.display = display;
	document.getElementById("dopplerUmbilicalVeinTable").style.display = display;
	document.getElementById("dopplerUterineArteryTable").style.display = display;
	document.getElementById("dopplerMiddleCerebralArteryTable").style.display = display;
	document.getElementById("placentaTable").style.display = display;			
}

function initCommentsConclusion()
{
	var table = Table.createTable($("commentsConclusionTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		{
			label: "Sonographer's Comments",
			cellType: "textarea",
			cols: 100,
			rows: 5,
			getValue: function(item) { return item.sonographersComments; },
			setValue: function(item, value) { item.sonographersComments = value; },
			getError: function(item) { return null; }
		},
		{
			label: "MD Opinion/Recommendation",
			cellType: "textarea",
			cols: 100,
			rows: 5,
			getValue: function(item) { return item.mdOpinion; },
			setValue: function(item, value) { item.mdOpinion = value; },
			getError: function(item) { return null; }
		}
	]);

	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.commentsConclusion]);
}

function onTypeSelected()
{
	if(data.obusReportType != null)
	{
		document.getElementById("TypeSelection").style.display = 'none';
		document.getElementById("T1T2T3Doc").style.display = 'block';
		document.getElementById("reportType").innerHTML = data.obusReportType;
		initTabs();
	}
}

function initTabs()
{
	reportType = data.obusReportType;

	initIndicationsAndDates();
	initGeneral();
	initBiometry();
	initAnatomy();
	initCardiac();
	initWellBeing();
	initCommentsConclusion();
}

function UpdateFetalNumber(count)
{
	// update selection UI to show correct number of fetus selections
	var select = document.getElementById("fetusSelect");
	
	if(!select) return;

	if(count == 1)
	{
		select.style.display = "none";
	}
	else
	{
		select.style.display = "block";

		// clear existing drop-down values
		while (select.firstChild) 
		{
			 select.removeChild(select.firstChild);
		};

		for(var i = 0; i < count; i++)
		{
			var option = document.createElement("option");
			option.value = i;
			option.text = "Fetus " + (i+1).toString();
			try
			{
				select.add(option,null);
			}
			catch(ex)
			{
				select.add(option);
			}
		}
		
		EnsurePerFetusDataExists();
	}
		
}

function selectFetus(number)
{
	fetus = parseInt(number);
	
	// re-bind each fetus dependant page to the current fetus' data
	bindBiometry();
	bindAnatomy();
	bindCardiac();
	bindWellBeing();
}

function setAnatomyDefaults()
{
	if(!data.anatomy[fetus]) return;

	var anatomyProperties = ["headShape", "posteriorHorns", "choroidPlexi", "cavumSepti", "cerebellum", "cisternaMagna", "nuchalFold", "eyes", "mouth", 
			"profile", "spine", "fourChambers", "greatVessels", "chest", "diaphragm", "stomach", "abdominalWall", "kidneysRt", "kidneysLt", "bladder", "upperRt", "upperLt", 
			"lowerRt", "lowerLt", "cordVessels"];
			
	for(var i = 0; i < anatomyProperties.length; i++)
	{
		if(typeof(data.anatomy[fetus][anatomyProperties[i]]) == "undefined" || data.anatomy[fetus][anatomyProperties[i]] == null || data.anatomy[fetus][anatomyProperties[i]] == "")
		{
			data.anatomy[fetus][anatomyProperties[i]] = "Normal";
		}
	}
	
	bindAnatomy();
}

function structuredReportHtml()
{
	var html = "";
	
	html+= 	"<div id=\"TypeSelection\">";
	html+= 	"		<p class=\"sectionheading\">Report Type</p>";
	html+= 	"		<table id=\"typeSelectionTable\">";
	html+= 	"			<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"		</table>";
	html+= 	"		<input type=\"button\" value=\"Confirm\" onclick=\"javascript: onTypeSelected()\"/>";
	html+= 	"	</div>";
	html+= 	"	<div id=\"T1T2T3Doc\" class=\"TabControl\">";
	html+= 	"		<div class=\"TabList\">";
	html+= 	"			<label for=\"IndicationsAndDates\" class=\"Tab\">Indications & Dates</label>";
	html+= 	"			<label for=\"General\" class=\"Tab\">General</label>";
	html+= 	"			<label for=\"Biometry\" class=\"Tab\">Biometry</label>";
	html+= 	"			<label for=\"Anatomy\" class=\"Tab\">Anatomy</label>";
	html+= 	"			<label id=\"CardiacTab\" for=\"Cardiac\" class=\"Tab\">Cardiac</label>";
	html+= 	"			<label for=\"WellBeing\" class=\"Tab\">Well-being</label>";
	html+= 	"			<label for=\"CommentsConclusion\" class=\"Tab\">Comments/Conclusion</label>";
	html+= 	"		</div>";
	html+= 	"		<div class=\"TabHeader\">";
	html+= 	"			<select id=\"fetusSelect\" onChange=\"javascript: selectFetus(this.options[this.selectedIndex].value)\" style=\"{float:right;}\"></select>";
	html+= 	"			Report Type:&nbsp;<span id=\"reportType\"></span>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"IndicationsAndDates\" class=\"TabPage\">";
	html+= 	"			<table id=\"indicationsAndDatesTable\" width=\"95%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<table id=\"lmpTable\">";
	html+= 	"				<tr><td class=\"tableheading\">LMP</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<table id=\"usTable\">";
	html+= 	"				<tr><td class=\"tableheading\">US</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<table id=\"establishedEDCTable\">";
	html+= 	"				<tr><td class=\"tableheading\">Established EDC</td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"General\" class=\"TabPage\">";
	html+= 	"			<table id=\"generalTable\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"Biometry\" class=\"TabPage\">";
	html+= 	"			<table id=\"biometryAssessedTable\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{float:left;width:48%;}\">";
	html+= 	"				<table id=\"crlTable\">";
	html+= 	"					<tr><td class=\"tableheading\">CRL</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"bpdTable\">";
	html+= 	"					<tr><td class=\"tableheading\">BPD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"ofdTable\">";
	html+= 	"					<tr><td class=\"tableheading\">OFD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"correctedBpdTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Corrected BPD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"hcTable\">";
	html+= 	"					<tr><td class=\"tableheading\">HC</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"abdTable\">";
	html+= 	"					<tr><td class=\"tableheading\">ABD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"flTable\">";
	html+= 	"					<tr><td class=\"tableheading\">FL</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"averageSizeTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Average Size</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{float:right;width:48%;}\">";
	html+= 	"				<table id=\"nuchalTransparencyTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Nuchal Transparency</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"Anatomy\" class=\"TabPage\">";
	html+=	"			<input type=\"button\" value=\"Set all Normal\" onClick=\"javascript: setAnatomyDefaults()\" style=\"{float:right; margin-top:1em;}\"></input>";
	html+= 	"			<table id=\"anatomyTable\" style=\"{width:48%;}\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{width:48%; float:left;}\">";
	html+= 	"				<table id=\"headShapeTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Head Shape</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"headTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Head</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"nuchalFoldTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Nuchal Fold</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"faceTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Face</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"spineTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Spine</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{width:48%; float:right;}\">";
	html+= 	"				<table id=\"heartTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Heart</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"chestTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Chest</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"abdomenTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Abdomen</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"genitaliaTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Genitalia</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"extremitiesTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Extremities</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"cordVesselsTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Cord Vessels</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"Cardiac\" class=\"TabPage\">";
	html+= 	"			<table id=\"fourChamberViewTable\">";
	html+= 	"				<tr><td class=\"tableheading\">Four-Chamber View</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"outflowTractsTable\">";
	html+= 	"				<tr><td class=\"tableheading\">Outflow Tracts</td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"WellBeing\" class=\"TabPage\">";
	html+= 	"			<table id=\"wellBeingTable\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div  id=\"wellBeingBpsColumn\" style=\"{width:48%;float:left;}\">";
	html+= 	"				<table id=\"bpsTable\">";
	html+= 	"					<tr><td class=\"tableheading\">BPS</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div id=\"wellBeingDopplerColumn\" style=\"{width:48%;float:right;}\">";
	html+= 	"				<div id=\"dopplerHeading\" class=\"tableheading\">Doppler</div>";
	html+= 	"				<table id=\"dopplerUmbilicalArteryTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUmbilicalVeinTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Vein</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUterineArteryTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Uterine Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerMiddleCerebralArteryTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Middle Cerebral Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">";
	html+= 	"				<table id=\"placentaTable\">";
	html+= 	"					<tr><td class=\"tableheading\">Placenta</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"		</div>";
	html+= 	"		<div id=\"CommentsConclusion\" class=\"TabPage\">";
	html+= 	"			<table id=\"commentsConclusionTable\" width=\"98%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	html+= 	"	</div>";
	
	return html;
}