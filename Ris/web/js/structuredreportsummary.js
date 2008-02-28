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

	var biometrySummaryTable = Table.createTable($("biometrySummaryTable"+fetus), { editInPlace: true, flow: true, checkBoxes: false},
	[
		new readOnlyBiometryCell("CRL", "crl", function(item) { return item.crl ? "CALC" : ""; }),
		new readOnlyBiometryCell("BPD", "bpd", function(item) { return item.bpd ? "CALC" : ""; }),
		new readOnlyBiometryCell("OFD", "ofd", function(item) { return item.ofd ? "CALC" : ""; }),
		new readOnlyBiometryCell("Corrected BPD", "correctedBPD", function(item) { return item.correctedBPD ? "CALC" : ""; }),
		new readOnlyBiometryCell("HC", "hc", function(item) { return item.hc ? "CALC" : ""; }),
		new readOnlyBiometryCell("ABPD", "", function(item) { return "TODO"; }),// ABPD
		new readOnlyBiometryCell("FL", "fl", function(item) { return item.fl ? "CALC" : ""; }),
		new readOnlyBiometryCell("Average Size", "", function(item) { return "CALC"; }),
		new readOnlyBiometryCell("Nuchal Transparency", "nuchalTransparency", function(item) { return ""; })
	]);
		
	biometryAssessedTable.bindItems([data.biometry[fetus]]);
	biometrySummaryTable.bindItems([data.biometry[fetus]]);
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
		new readOnlyCell("Head Shape", "headShape", true)
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
		new readOnlyCell("Nuchal fold", "nuchalFold", true)
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
		new readOnlyCell("Spine", "spine", true)
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
		new readOnlyCell("Chest", "chest", true)
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
		new readOnlyCell("Genitalia", "genitalia", true)
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
		new readOnlyCell("Cord Vessels", "cordVessels", true)
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
		new readOnlyCell("PI - Right", "rightUterinePi"),
		new readOnlyCell("Notch - Left", "leftUterineNotch"),
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

function readOnlyCell(label, prop, formatLabelLikeTableHeading)
{
	this.label = formatLabelLikeTableHeading ? "<span class=\"tableheading\">" + label + "</span>" : label;
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

function readOnlyBiometryCell(label, prop, calcProp)
{
	this.label = label;
	this.prop = prop;
	this.calc = calcProp;
	this.cellType = "readonly";
	this.getValue = function(item) 
	{ 
		var value = "";
		value += (item[prop] != null && item[prop] != "") ? item[prop] + " mm" : "";
		value += (this.calc(item) != null && this.calc(item) != "") ? (value != "" ? " = " + this.calc(item) + " wks" : this.calc(item) + " wks") : "";
		return value; 
	}
	this.getVisible = function(item) { return this.getValue(item) != ""; }
}

function readonlyStructuredReportHtml(source)
{
	var html = "";
	var data = source || { general : { fetalNumber: 1}};
	var fetusCount = data.general.fetalNumber;

	html+= "<div class=\"ObsrSummary\">";
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
	html+= "</div>";

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
	html+= 	"			<table id=\"indicationsAndDatesTable\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"lmpTable\">";
	html+= 	"				<tr><td class=\"tableheading\">LMP</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"usTable\">";
	html+= 	"				<tr><td class=\"tableheading\">US</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"establishedEDCTable\">";
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
	html+= 	"			<table id=\"generalTable\">";
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
	html+= 	"			<table id=\"biometryAssessedTable" + fetus + "\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<table id=\"biometrySummaryTable" + fetus + "\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";

	return html;
}

function anatomyHtml(fetus, fetusCount)
{
	var html = "";
	var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";

	html+= 	"		<div id=\"Anatomy" + fetus + "\">";
	html+=	"			<div class=\"sectionheading\">Anatomy" + fetusLabel + "</div>";
	html+= 	"			<table id=\"anatomyTable" + fetus + "\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div style=\"{width:48%; float:left;}\">";
	html+= 	"				<table id=\"headShapeTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"headTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Head</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"nuchalFoldTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"faceTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Face</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"spineTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{width:48%; float:right;}\">";
	html+= 	"				<table id=\"heartTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Heart</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"chestTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"abdomenTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Abdomen</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"genitaliaTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table class=\"extremitiesTable\" id=\"extremitiesTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Extremities</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"cordVesselsTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
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
	html+= 	"			<table id=\"fourChamberViewTable" + fetus + "\">";
	html+= 	"				<tr><td class=\"tableheading\">Four-Chamber View</td></tr>";
	html+= 	"			</table>";
	html+= 	"			<br />";
	html+= 	"			<table id=\"outflowTractsTable" + fetus + "\">";
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
	html+= 	"			<table id=\"wellBeingTable" + fetus + "\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"			<div class=\"wellBeingBpsColumn\" style=\"{width:48%;float:left;}\">";
	html+= 	"				<table id=\"bpsTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">BPS</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div class=\"wellBeingDopplerColumn\"  style=\"{width:48%;float:right;}\">";
	html+= 	"				<div id=\"dopplerHeading" + fetus + "\" class=\"tableheading\">Doppler</div>";
	html+= 	"				<table id=\"dopplerUmbilicalArteryTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUmbilicalVeinTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Umbilical - Vein</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerUterineArteryTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Uterine Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"				<table id=\"dopplerMiddleCerebralArteryTable" + fetus + "\">";
	html+= 	"					<tr><td class=\"tableheading\">Middle Cerebral Artery</td></tr>";
	html+= 	"				</table>";
	html+= 	"			</div>";
	html+= 	"			<div style=\"{clear:both;}\">";
	html+= 	"				<table class=\"placentaTable\" id=\"placentaTable" + fetus + "\">";
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
	html+= 	"			<table id=\"commentsConclusionTable\">";
	html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
	html+= 	"			</table>";
	html+= 	"		</div>";
	
	return html;
}

