var data = {};
var reportType = "unknown";

function initStructuredReportSummary(source)
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
	
	initSections();
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


function initIndicationsAndDates()
{
	var table = Table.createTable($("indicationsAndDatesTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Indication", "indication"),
		new readOnlyCell("Clinical Details", "clinicalDetails")
	]);
	
	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.indicationsAndDates]);
	
	var lmpTable = Table.createTable($("lmpTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyDateCell("LMP", "LMP"),
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
		new readOnlyDateCell("1st Ultrasound", "firstUltrasound"),
		new readOnlyCell("Number of weeks", "numberOfWeeks"),
		new readOnlyCell("Today", "today"),
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
		new readOnlyDateCell("EDC", "establishedEDC"),
		{
			label: "Age from EDC",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getVisible: function(item) { return item.establishedEDC && item.establishedEDC != null; }
		},
		new readOnlyCell("How Determined?", "edcMethod"),
		new readOnlyDateCell("Transferred Date", "transferredDate")
	]);

	establishedEDCTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	establishedEDCTable.bindItems([data.indicationsAndDates.establishedEDC]);
}

function initGeneral()
{
	var table = Table.createTable($("generalTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Visibility", "visibility"),
		new readOnlyCell("Fetal Number", "fetalNumber"),
		new readOnlyCell("Twin Type", "twinType"),
		new readOnlyCell("FH Activity", "fhActivity"),
		new readOnlyCell("Are you sure FH activity is absent?", "fhActivityConfirmation"),
		new readOnlyCell("Presentation", "presentation"),
		new readOnlyCell("AFV", "afv"),
		new readOnlyCell("Placenta", "placenta"),
		new readOnlyCell("??", "cervixProximity"),
		new readOnlyCell("Right Adnexa", "rightAdnexa"),
		new readOnlyCell("Left Adnexa", "leftAdnexa"),
		new readOnlyCell("Yolk Sac", "yolkSac"),
		new readOnlyCell("Cervix", "cervix"),
		new readOnlyCell("Apposed Length", "apposedLength"),
		new readOnlyCell("Cervix Assessed", "cervixAssessed")
	]);

	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.general]);
}

function initBiometry(fetus)
{
	data.biometry[fetus] = data.biometry[fetus] || {};

	var biometryAssessedTable = Table.createTable($("biometryAssessedTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyAssessedCell("Biometry", "assessed")
	]);

	biometryAssessedTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var crlTable = Table.createTable($("crlTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "crl"),
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

	var bpdTable = Table.createTable($("bpdTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "bpd"),
		{
			label: "wks", 
			cellType: "readonly",
			getValue: function(item) { return "CALC"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; }					
		}
	]);

	bpdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var ofdTable = Table.createTable($("ofdTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "ofd")
	]);

	ofdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var correctedBpdTable = Table.createTable($("correctedBpdTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
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

	var hcTable = Table.createTable($("hcTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "hc"),
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

	var abdTable = Table.createTable($("abdTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("", "abdX"),
		{
			label: "", 
			cellType: "readonly",
			getValue: function(item) { return "+"; },
			setValue: function(item, value) { return null; },
			getError: function(item) { return null; },
			getVisible: function(item) { return reportType == "T2" || reportType == "T3"; }
		},
		new readOnlyCell("", "abdY"),
		new readOnlyCell("mm", "abdAC"),
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

	var flTable = Table.createTable($("flTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "fl"),
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

	var averageSizeTable = Table.createTable($("averageSizeTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
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

	var nuchalTransparencyTable = Table.createTable($("nuchalTransparencyTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("mm", "nuchalTransparency")
	]);

	nuchalTransparencyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	biometryAssessedTable.bindItems([data.biometry[fetus]]);
	OnBiometryChanged(data.biometry[fetus].assessed == "Assessed", fetus);
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

function OnBiometryChanged(show, fetus)
{
	document.getElementById("crlTable"+fetus).style.display = (show && reportType == "T1") ? "block" : "none";
	document.getElementById("bpdTable"+fetus).style.display = (show) ? "block" : "none";
	document.getElementById("ofdTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("correctedBpdTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("hcTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("abdTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("flTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("averageSizeTable"+fetus).style.display = (show && (reportType == "T2" || reportType == "T3")) ? "block" : "none";
	document.getElementById("nuchalTransparencyTable"+fetus).style.display = (show && reportType == "T2") ? "block" : "none";
}

function initAnatomy(fetus)
{
	var anatomyTable = Table.createTable($("anatomyTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyAssessedCell("Anatomy", "assessed")
	]);
	
	anatomyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var headShapeTable = Table.createTable($("headShapeTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Head shape", "headShape")
	]);			
	
	headShapeTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var headTable = Table.createTable($("headTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[			
		new readOnlyCell("Posterior horns", "posteriorHorns"),
		new readOnlyCell("Choroid plexi", "choroidPlexi"),
		new readOnlyCell("Cavum septi", "cavumSepti"),
		new readOnlyCell("Cerebellum", "cerebellum"),
		new readOnlyCell("Cisterna magna", "cisternaMagna")
	]);
	
	headTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var nuchalFoldTable = Table.createTable($("nuchalFoldTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Nuchal fold", "nuchalFold")
	]);
	
	nuchalFoldTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var faceTable = Table.createTable($("faceTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Eyes", "eyes"),
		new readOnlyCell("Orbital ratio (i/o)", "orbitalRatio"),
		new readOnlyCell("Mouth", "mouth"),
		new readOnlyCell("Profile", "profile")
	]);
	
	faceTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var spineTable = Table.createTable($("spineTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Spine", "spine")
	]);

	spineTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var heartTable = Table.createTable($("heartTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("4 Chambers", "fourChambers"),
		new readOnlyCell("Great vessels", "greatVessels"),
		new readOnlyCell("Detail cardiac", "detailedCardiac")
	]);
	
	ShowDetailedCardiac(data.anatomy.detailedCardiac);

	heartTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var chestTable = Table.createTable($("chestTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Chest", "chest")
	]);
	
	chestTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var abdomenTable = Table.createTable($("abdomenTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Diaphragm", "diaphragm"),
		new readOnlyCell("Stomach", "stomach"),
		new readOnlyCell("Abdominal wall", "abdominalWall"),
		new readOnlyCell("Kidneys - RT", "kidneysRt"),
		new readOnlyCell("Kidneys - LT", "kidneysLt"),
		new readOnlyCell("Bladder", "bladder")
	]);
	
	abdomenTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var genitaliaTable = Table.createTable($("genitaliaTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Genitalia", "genitalia")
	]);
	
	genitaliaTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var extremitiesTable = Table.createTable($("extremitiesTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Upper - RT", "upperRt"),
		new readOnlyCell("Upper - LT", "upperLt"),
		new readOnlyCell("Lower - RT", "lowerRt"),
		new readOnlyCell("Lower - LT", "lowerLt")
	]);
	
	extremitiesTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var cordVesselsTable = Table.createTable($("cordVesselsTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Cord Vessels", "cordVessels")
	]);

	cordVesselsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	data.anatomy[fetus] = data.anatomy[fetus] || {};

	anatomyTable.bindItems([data.anatomy[fetus]]);
	OnAnatomyChanged(data.anatomy[fetus].assessed == "Assessed", fetus);

	headShapeTable.bindItems([data.anatomy[fetus]]);
	headTable.bindItems([data.anatomy[fetus]]);
	nuchalFoldTable.bindItems([data.anatomy[fetus]]);
	faceTable.bindItems([data.anatomy[fetus]]);
	spineTable.bindItems([data.anatomy[fetus]]);
	heartTable.bindItems([data.anatomy[fetus]]);
	ShowDetailedCardiac(data.anatomy[fetus].detailedCardiac == "Yes", fetus);
	
	chestTable.bindItems([data.anatomy[fetus]]);
	abdomenTable.bindItems([data.anatomy[fetus]]);
	genitaliaTable.bindItems([data.anatomy[fetus]]);
	extremitiesTable.bindItems([data.anatomy[fetus]]);
	cordVesselsTable.bindItems([data.anatomy[fetus]]);
}

function OnAnatomyChanged(show, fetus)
{
	var display = show ? "block" : "none";

	document.getElementById("headShapeTable"+fetus).style.display = display;
	document.getElementById("headTable"+fetus).style.display = display;
	document.getElementById("nuchalFoldTable"+fetus).style.display = display;
	document.getElementById("faceTable"+fetus).style.display = display;
	document.getElementById("spineTable"+fetus).style.display = display;
	document.getElementById("heartTable"+fetus).style.display = display;
	document.getElementById("chestTable"+fetus).style.display = display;
	document.getElementById("abdomenTable"+fetus).style.display = display;
	document.getElementById("genitaliaTable"+fetus).style.display = display;
	document.getElementById("extremitiesTable"+fetus).style.display = display;
	document.getElementById("cordVesselsTable"+fetus).style.display = display;
}

function ShowDetailedCardiac(show, fetus)
{
	if(document.getElementById("Cardiac"+fetus)) document.getElementById("Cardiac"+fetus).style.display = show ? "inline" : "none";
}

function initCardiac(fetus)
{
	var standardChoices = ["Normal", "Not seen", "See comment"];
	var defaultChoice = 0;

	var fourChamberViewTable = Table.createTable($("fourChamberViewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Heart/Thoracic Ratio", "heartThoracicRatio"),
		new readOnlyCell("Ventricals symmetrical in size", "ventricalsSymmetrical"),
		new readOnlyCell("RV Diameter", "rvDiameter"),
		new readOnlyCell("LV Diameter", "lvDiameter"),
		new readOnlyCell("Thin and mobile valves", "thinMobileValves"),
		new readOnlyCell("Function and rhythm", "functionAndRhythm"),
		new readOnlyCell("Ventricular septum/crux", "ventricularSeptumCrux")
	]);

	fourChamberViewTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var outflowTractsTable = Table.createTable($("outflowTractsTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Two Outlets", "twoOutlets"),
		new readOnlyCell("PA crosses AO", "paCrossesAO"),
		new readOnlyCell("Outlets symmetry", "outletsSymmetry"),
		new readOnlyCell("AO diameter", "aoDiameter"),
		new readOnlyCell("PO diameter", "poDiameter")
	]);

	outflowTractsTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

	data.cardiac[fetus] = data.cardiac[fetus] || {};

	fourChamberViewTable.bindItems([data.cardiac[fetus]]);
	outflowTractsTable.bindItems([data.cardiac[fetus]]);
}

function initWellBeing(fetus)
{
	var wellBeingTable = Table.createTable($("wellBeingTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyAssessedCell("Well-being", "assessed")
	]);

	wellBeingTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var bpsTable = Table.createTable($("bpsTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("AFV", "afv"),
		new readOnlyCell("??", "unknown"),
		new readOnlyCell("Max vertical pocket", "maxVerticalPocket"),
		{
			label: "AFI (A+B+C+D)",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getError: function(item) { return null; }
		},
		new readOnlyCell("FM", "fm"),
		new readOnlyCell("FT", "ft"),
		new readOnlyCell("FBM", "fbm"),
		new readOnlyCell("NST", "nst"),
		{
			label: "BPS",
			cellType: "readonly",
			getValue: function(item) { return "TODO"; },
			setValue: function(item, value) { return; },
			getError: function(item) { return null; }
		},
		new readOnlyCell("??", "unknown2")
	]);

	bpsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	
	var dopplerUmbilicalArteryTable = Table.createTable($("dopplerUmbilicalArteryTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("PI", "umbilicalPi"),
		new readOnlyCell("EDF", "edf")
	]);

	dopplerUmbilicalArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerUmbilicalVeinTable = Table.createTable($("dopplerUmbilicalVeinTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("UVVmax", "uvvMax"),
		new readOnlyCell("Pulsations", "pulsations")
	]);

	dopplerUmbilicalVeinTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerUterineArteryTable = Table.createTable($("dopplerUterineArteryTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("PI - Left", "leftUterinePi"),
		new readOnlyCell("Notch - Left", "leftUterineNotch"),
		new readOnlyCell("PI - Right", "rightUterinePi"),
		new readOnlyCell("Notch - Right", "rightUterineNotch")
	]);

	dopplerUterineArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var dopplerMiddleCerebralArteryTable = Table.createTable($("dopplerMiddleCerebralArteryTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("PI", "middleCerebralPi"),
		new readOnlyCell("Peak Systolic Velocity", "peakSystolicVelocity")
	]);

	dopplerMiddleCerebralArteryTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	var placentaTable = Table.createTable($("placentaTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Max Length", "maxLength"),
		new readOnlyCell("Max Depth", "maxDepth"),
		new readOnlyCell("Grannum grade", "grannumGrade"),
		new readOnlyCell("Texture", "texture")
	]);

	placentaTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

	data.wellBeing[fetus] = data.wellBeing[fetus] || {};
	
	wellBeingTable.bindItems([data.wellBeing[fetus]]);
	OnWellBeingChanged(data.wellBeing[fetus].assessed == "Assessed", fetus);
	
	bpsTable.bindItems([data.wellBeing[fetus]]);
	dopplerUmbilicalArteryTable.bindItems([data.wellBeing[fetus]]);
	dopplerUmbilicalVeinTable.bindItems([data.wellBeing[fetus]]);
	dopplerUterineArteryTable.bindItems([data.wellBeing[fetus]]);
	dopplerMiddleCerebralArteryTable.bindItems([data.wellBeing[fetus]]);
	placentaTable.bindItems([data.wellBeing[fetus]]);
}

function OnWellBeingChanged(show, fetus)
{
	var display = show ? "block" : "none";

	document.getElementById("bpsTable"+fetus).style.display = display;
	document.getElementById("dopplerHeading"+fetus).style.display = display;
	document.getElementById("dopplerUmbilicalArteryTable"+fetus).style.display = display;
	document.getElementById("dopplerUmbilicalVeinTable"+fetus).style.display = display;
	document.getElementById("dopplerUterineArteryTable"+fetus).style.display = display;
	document.getElementById("dopplerMiddleCerebralArteryTable"+fetus).style.display = display;
	document.getElementById("placentaTable"+fetus).style.display = display;			
}

function initCommentsConclusion()
{
	var table = Table.createTable($("commentsConclusionTable"),{ editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyCell("Sonographer's Comments", "sonographersComments"),
		new readOnlyCell("MD Opinion/Recommendation", "mdOpinion")
	]);

	table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
	table.bindItems([data.commentsConclusion]);
}

function initSections()
{
	reportType = data.obusReportType;
	if(document.getElementById("reportType")) document.getElementById("reportType").innerHTML = data.obusReportType;

	initIndicationsAndDates();
	initGeneral();

	for(var i = 0; i < data.general.fetalNumber; i++)
	{
		try
		{
			initBiometry(i);
			initAnatomy(i);
			initCardiac(i);
			initWellBeing(i);
		}
		catch(e)
		{
			alert(e.message);
		}
	}
	initCommentsConclusion();
}

function readOnlyCell(label, prop)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop]; };
	this.getVisible = function(item) { return item[prop] != null && item[prop] != ""; };
}

function readOnlyDateCell(label, prop)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return Ris.formatDate(item[prop]); };
	this.getVisible = function(item) { return item[prop] != null && item[prop] != ""; };
}

function readOnlyAssessedCell(label, prop)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop]; };
	this.getVisible = function(item) { return item[prop] != null && item[prop] != "" && item[prop] != "Assessed"; };
}

function readonlyStructuredReportHtml(source)
{
	var html = "";
	var data = source || { general : { fetalNumber: 1}};
	var fetusCount = data.general.fetalNumber;

	html+= titleHtml();
	html+= indicationsAndDatesHtml();
	html+= generalHtml(i, fetusCount);
	for(var i = 0; i < fetusCount; i++)
	{
		html+= biometryHtml(i, fetusCount);
		html+= anatomyHtml(i, fetusCount);
		html+= cardiacHtml(i, fetusCount);
		html+= wellBeingHtml(i, fetusCount);
	}
	html+= commentsConclusionsHtml();

	return html;
}

function titleHtml()
{
	var html = "";

	html+= 	"		<div>";
	html+=	"			<div class=\"sectionheading\">Structured Report Summary</div>";
	html+= 	"			Report Type:&nbsp;<span id=\"reportType\"></span>";
	html+= 	"		</div>";

	return html;
}

function indicationsAndDatesHtml()
{
	var html = "";

	html+= 	"		<div id=\"IndicationsAndDates\">";
	html+=	"			<div class=\"sectionheading\">Indications and Dates</div>";
	html+= 	"			<table id=\"indicationsAndDatesTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"lmpTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\">LMP</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"usTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\">US</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"establishedEDCTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\">Established EDC</td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";

	return html;
}

function generalHtml()
{
	var html = "";
	
	html+= 	"		<div id=\"General\">";
	html+=	"			<div class=\"sectionheading\">General</div>";
	html+= 	"			<table id=\"generalTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";

	return html;
}

function biometryHtml(fetus, fetusCount)
{
	var html = "";
	var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";

	html+= 	"		<div id=\"Biometry" + fetus + "\">";
	html+=	"			<div class=\"sectionheading\">Biometry" + fetusLabel + "</div>";
	html+= 	"			<table id=\"biometryAssessedTable" + fetus + "\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{float:left;width:48%;}\">";
	html+= 	"				<table id=\"crlTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">CRL</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"bpdTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">BPD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"ofdTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">OFD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"correctedBpdTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Corrected BPD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"hcTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">HC</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"abdTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">ABD</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"flTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">FL</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"averageSizeTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Average Size</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{float:right;width:48%;}\">";
	html+= 	"				<table id=\"nuchalTransparencyTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Nuchal Transparency</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
	html+= 	"		</div>";

	return html;
}

function anatomyHtml(fetus, fetusCount)
{
	var html = "";
	var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";

	html+= 	"		<div id=\"Anatomy" + fetus + "\">";
	html+=	"			<div class=\"sectionheading\">Anatomy" + fetusLabel + "</div>";
	html+= 	"			<table id=\"anatomyTable" + fetus + "\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{width:48%; float:left;}\">";
	html+= 	"				<table id=\"headShapeTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Head Shape</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"headTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Head</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"nuchalFoldTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Nuchal Fold</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"faceTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Face</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"spineTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Spine</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{width:48%; float:right;}\">";
	html+= 	"				<table id=\"heartTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Heart</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"chestTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Chest</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"abdomenTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Abdomen</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"genitaliaTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Genitalia</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"extremitiesTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Extremities</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"cordVesselsTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Cord Vessels</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
	html+= 	"		</div>";

	return html;
}

function cardiacHtml(fetus, fetusCount)
{
	var html = "";
	var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";
	
	html+= 	"		<div id=\"Cardiac" + fetus + "\">";
	html+=	"			<div class=\"sectionheading\">Cardiac" + fetusLabel + "</div>";
	html+= 	"			<table id=\"cardiacDummy" + fetus + "\" width=\"100%\" style=\"{display:none;}\">";
	html+= 	"			</table>";
	html+= 	"			<table id=\"fourChamberViewTable" + fetus + "\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\">Four-Chamber View</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"outflowTractsTable" + fetus + "\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\">Outflow Tracts</td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";

	return html;
}

function wellBeingHtml(fetus, fetusCount)
{
	var html = "";
	var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";
	html+= 	"		<div id=\"WellBeing" + fetus + "\">";
	html+=	"			<div class=\"sectionheading\">Well-Being" + fetusLabel + "</div>";
	html+= 	"			<table id=\"wellBeingTable" + fetus + "\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{width:48%;float:left;}\">";
	html+= 	"				<table id=\"bpsTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">BPS</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{width:48%;float:right;}\">";
	html+= 	"				<div id=\"dopplerHeading" + fetus + "\" class=\"tableheading\">Doppler</div>";
	html+= 	"				<table id=\"dopplerUmbilicalArteryTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUmbilicalVeinTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Vein</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUterineArteryTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Uterine Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerMiddleCerebralArteryTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Middle Cerebral Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">";
	html+= 	"				<table id=\"placentaTable" + fetus + "\" width=\"100%\">";
	html+= 	"					<tr><td class=\"tableheading\">Placenta</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"		</div>";

	return html;
}

function commentsConclusionsHtml()
{
	var html = "";

	html+= 	"		<div id=\"CommentsConclusion\">";
	html+=	"			<div class=\"sectionheading\">Comments and Conclusions</div>";
	html+= 	"			<table id=\"commentsConclusionTable\" width=\"100%\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	
	return html;
}

