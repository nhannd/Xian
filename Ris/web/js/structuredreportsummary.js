var StructuredReportPreview = {

	_data : null,

	create : function(source, element)
	{
		var data = source;
		data.general = data.general || { fetalNumber :1 };
		data.general.fetalNumber = data.general.fetalNumber || 1;

		var fetusCount = data.general.fetalNumber;

		element.innerHTML = this._html(fetusCount);
		this._initializeData(source);
	},
	
	_initializeData : function(source)
	{
		this._data = source || {};

		this._data.indicationsAndDates = this._data.indicationsAndDates || {};
		this._data.indicationsAndDates.lmp = this._data.indicationsAndDates.lmp || {};
		this._data.indicationsAndDates.us = this._data.indicationsAndDates.us || {};
		this._data.indicationsAndDates.establishedEDC = this._data.indicationsAndDates.establishedEDC || {};
		this._data.general = this._data.general || {};
		this._data.biometry = this._data.biometry || [];
		this._data.anatomy = this._data.anatomy || [];
		this._data.cardiac = this._data.cardiac || [];
		this._data.wellBeing = this._data.wellBeing || [];
		this._data.commentsConclusion = this._data.commentsConclusion || {};
		
		this._ensurePerFetusDataExists();
		
		this._initSections();
	},
	
	_ensurePerFetusDataExists : function()
	{
		// Ensure all per fetus objects are defined
		// If the fetalNumber was previously higher than it is now and data was saved, these extra objects
		// also need to be initialized
		var max = this._data.general.fetalNumber > this._data.biometry.length ? this._data.general.fetalNumber : this._data.biometry.length;
		
		for(var i = 0; i < max; i++)
		{
			this._data.biometry[i] = this._data.biometry[i] || {};
		}
		for(var i = 0; i < max; i++)
		{
			this._data.anatomy[i] = this._data.anatomy[i] || {};
		}
		for(var i = 0; i < max; i++)
		{
			this._data.cardiac[i] = this._data.cardiac[i] || {};
		}
		for(var i = 0; i < max; i++)
		{
			this._data.wellBeing[i] = this._data.wellBeing[i] || {};
		}
	},

	_initSections : function()
	{
		if(document.getElementById("reportTypePreview")) document.getElementById("reportTypePreview").innerHTML = this._data.obusReportType;

		IndicationsAndDatesPreview.initialize(this._data.indicationsAndDates);
		GeneralPreview.initialize(this._data.general);

		for(var i = 0; i < this._data.general.fetalNumber; i++)
		{
			try
			{
				BiometryPreview.initialize(this._data.biometry, i);
				AnatomyPreview.initialize(this._data.anatomy, i);
				CardiacPreview.initialize(this._data.cardiac, i);
				WellBeingPreview.initialize(this._data.wellBeing, i);
			}
			catch(e)
			{
				alert(e.message);
			}
		}
		
		CommentsConclusionPreview.initialize(this._data.commentsConclusion);
	},

	_html : function(fetusCount)
	{
		var html = "";

		html+= "<div class=\"ObsrSummary\">";
		html+= "		<div class=\"sectionheading\">Structured Report Summary</div>";
		html+= "		<div style=\"{margin-top:0.5em;margin-left:1em;}\">";
		html+= "			Report Type:&nbsp;<span id=\"reportTypePreview\"></span>";
		html+= "		</div>";
		html+= IndicationsAndDatesPreview.html();
		html+= GeneralPreview.html();
		for(var i = 0; i < fetusCount; i++)
		{
			html+= BiometryPreview.html(i, fetusCount);
			html+= AnatomyPreview.html(i, fetusCount);
			html+= CardiacPreview.html(i, fetusCount);
			html+= WellBeingPreview.html(i, fetusCount);
		}
		html+= CommentsConclusionPreview.html();
		html+= "</div>";

		return html;
	}
}

var IndicationsAndDatesPreview = {

	initialize : function(source)
	{
		this._data = source;
		var referenceDate = this._data.referenceDate;
		
		var indicationsAndDatesPreviewTable = Table.createTable($("indicationsAndDatesPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Indication", "indication"),
			new readOnlyCell("Clinical Details", "clinicalDetails")
		]);
		
		indicationsAndDatesPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		indicationsAndDatesPreviewTable.bindItems([this._data]);
		
		var lmpPreviewTable = Table.createTable($("lmpPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			new readOnlyDateCell("Date of LMP", "LMP"),
			new readOnlyDateCell("EDC", "lmpEdc"),
			new readOnlyAgeCell("lmpAge", referenceDate)
		]);
		
		lmpPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		lmpPreviewTable.bindItems([this._data.lmp]);
		lmpPreviewTable.style.display = this._data.lmp.LMP && this._data.lmp.LMP != null ? "block" : "none";

		var usPreviewTable = Table.createTable($("usPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[						
			new readOnlyDateCell("1st Ultrasound", "firstUltrasound"),
			new readOnlyCell("Age at 1st Ultrasound (wks)", "firstUltrasoundAge"),
			new readOnlyAgeCell("ultrasoundAge", referenceDate),
			new readOnlyDateCell("EDC", "firstUltrasoundEdc")
		]);
		
		usPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		usPreviewTable.bindItems([this._data.us]);
		usPreviewTable.style.display = this._data.us.firstUltrasound && this._data.us.firstUltrasound != null ? "block" : "none";

		var establishedEDCPreviewTable = Table.createTable($("establishedEDCPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			new readOnlyDateCell("EDC", "establishedEDC"),
			new readOnlyAgeCell("establishedEDCAge", referenceDate),
			new readOnlyCell("How Determined?", "edcMethod"),
			new readOnlyDateCell("Transferred Date", "transferredDate")
		]);

		establishedEDCPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		establishedEDCPreviewTable.bindItems([this._data.establishedEDC]);
		establishedEDCPreviewTable.style.display = this._data.establishedEDC.establishedEDC && this._data.establishedEDC.establishedEDC != null ? "block" : "none";
	},
	
	html : function()
	{
		var html = "";

		html+= 	"<div id=\"IndicationsAndDatesPreview\">";
		html+=	"	<div class=\"sectionheading\">Indications and Dates</div>";
		html+= 	"	<table id=\"indicationsAndDatesPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"	</table>";
		html+= 	"	<table id=\"lmpPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\">LMP</td></tr>";
		html+= 	"	</table>";
		html+= 	"	<table id=\"usPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\">US</td></tr>";
		html+= 	"	</table>";
		html+= 	"	<table id=\"establishedEDCPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\">Established EDC</td></tr>";
		html+= 	"	</table>";
		html+= 	"</div>";

		return html;
	}
}

var GeneralPreview = {

	initialize : function(source)
	{
		var generalPreviewTable = Table.createTable($("generalPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Visibility", "visibility"),
			new readOnlyCell("Fetal Number", "fetalNumber"),
			new readOnlyCell("Twin Type", "twinType"),
			new readOnlyCell("FH Activity", "fhActivity"),
			new readOnlyCell("Are you sure FH activity is absent?", "fhActivityConfirmation"),
			new readOnlyCell("Presentation", "presentation"),
			new readOnlyCell("AFV", "afv"),
			new readOnlyCell("Placenta", "placenta"),
			new readOnlyCell("Relationship to Placenta", "relationshipToPlacenta"),
			new readOnlyCell("Right Adnexa", "rightAdnexa"),
			new readOnlyCell("Left Adnexa", "leftAdnexa"),
			new readOnlyCell("Yolk Sac", "yolkSac"),
			new readOnlyCell("Cervix", "cervix"),
			new readOnlyCell("Apposed Length", "apposedLength"),
			new readOnlyCell("Cervix Assessed", "cervixAssessed")
		]);

		generalPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		generalPreviewTable.bindItems([source]);
	},
	
	html : function()
	{
		var html = "";
		
		html+= 	"<div id=\"GeneralPreview\">";
		html+=	"	<div class=\"sectionheading\">General</div>";
		html+= 	"	<table id=\"generalPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"	</table>";
		html+= 	"</div>";

		return html;
	}
}

var BiometryPreview = {
	
	initialize : function(source, fetus)
	{
		var biometryAssessedPreviewTable = Table.createTable($("biometryAssessedPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyAssessedCell("Biometry", "assessed")
		]);

		biometryAssessedPreviewTable.bindItems([source[fetus]]);

		var biometryPreviewTable = Table.createTable($("biometryPreviewTable"+fetus), { editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyBiometryCell("CRL", "crl", "crlWks"),
			new readOnlyBiometryCell("BPD", "bpd", "bpdWks"),
			new readOnlyBiometryCell("OFD", "ofd", ""),
			new readOnlyBiometryCell("Corrected BPD", "correctedBpd", "correctedBpdWks"),
			new readOnlyBiometryCell("ABD", "abdCircumference", "abdCircumferenceWks"),
			new readOnlyBiometryCell("FL", "fl", "flWks"),
			new readOnlyBiometryCell("Average Size", "", "avgWks"),
			new readOnlyBiometryCell("HC", "hc", "hcWks"),
			new readOnlyBiometryCell("Nuchal Transparency", "nuchalTransparency", ""),
			new readOnlyCell("EFW (gm)", "efw"),
			new readOnlyCell("Centile", "efwCentile")
		]);
			
		biometryPreviewTable.bindItems([source[fetus]]);
	},
	
	html : function(fetus, fetusCount)
	{
		var html = "";
		var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";

		html+= 	"		<div id=\"BiometryPreview" + fetus + "\">";
		html+=	"			<div class=\"sectionheading\">Biometry" + fetusLabel + "</div>";
		html+= 	"			<table id=\"biometryAssessedPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"			<table id=\"biometryPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"		</div>";

		return html;
	}
}

var AnatomyPreview = {
	
	initialize : function(source, fetus)
	{
		source[fetus] = source[fetus] || {};

		var anatomyAssessedPreviewTable = Table.createTable($("anatomyAssessedPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyAssessedCell("Anatomy", "assessed")
		]);
		
		var headShapePreviewTable = Table.createTable($("headShapePreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Head Shape", "headShape", true)
		]);			
		
		var headPreviewTable = Table.createTable($("headPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			new readOnlyCell("Posterior horns", "posteriorHorns"),
			new readOnlyCell("Choroid plexi", "choroidPlexi"),
			new readOnlyCell("Cavum septi", "cavumSepti"),
			new readOnlyCell("Cerebellum", "cerebellum"),
			new readOnlyCell("Cisterna magna", "cisternaMagna")
		]);
		
		var nuchalFoldPreviewTable = Table.createTable($("nuchalFoldPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Nuchal fold", "nuchalFold", true)
		]);
		
		var facePreviewTable = Table.createTable($("facePreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Eyes", "eyes"),
			new readOnlyCell("Orbital ratio (i/o)", "orbitalRatio"),
			new readOnlyCell("Mouth", "mouth"),
			new readOnlyCell("Profile", "profile")
		]);
		
		var spinePreviewTable = Table.createTable($("spinePreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Spine", "spine", true)
		]);

		var heartPreviewTable = Table.createTable($("heartPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("4 Chambers", "fourChambers"),
			new readOnlyCell("Great vessels", "greatVessels"),
			new readOnlyCell("Detail cardiac", "detailedCardiac")
		]);
		
		var chestPreviewTable = Table.createTable($("chestPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Chest", "chest", true)
		]);
		
		var abdomenPreviewTable = Table.createTable($("abdomenPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Diaphragm", "diaphragm"),
			new readOnlyCell("Stomach", "stomach"),
			new readOnlyCell("Abdominal wall", "abdominalWall"),
			new readOnlyCell("Kidneys - RT", "kidneysRt"),
			new readOnlyCell("Kidneys - LT", "kidneysLt"),
			new readOnlyCell("Bladder", "bladder")
		]);
		
		var genitaliaPreviewTable = Table.createTable($("genitaliaPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Genitalia", "genitalia", true)
		]);
		
		var extremitiesPreviewTable = Table.createTable($("extremitiesPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Upper - RT", "upperRt"),
			new readOnlyCell("Upper - LT", "upperLt"),
			new readOnlyCell("Lower - RT", "lowerRt"),
			new readOnlyCell("Lower - LT", "lowerLt")
		]);
		
		var cordVesselsPreviewTable = Table.createTable($("cordVesselsPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Cord Vessels", "cordVessels", true)
		]);

		anatomyAssessedPreviewTable.bindItems([source[fetus]]);
		this._onAnatomyChanged(source[fetus].assessed == "Assessed", fetus);

		headShapePreviewTable.bindItems([source[fetus]]);
		headPreviewTable.bindItems([source[fetus]]);
		nuchalFoldPreviewTable.bindItems([source[fetus]]);
		facePreviewTable.bindItems([source[fetus]]);
		spinePreviewTable.bindItems([source[fetus]]);
		heartPreviewTable.bindItems([source[fetus]]);
		this._showDetailedCardiac(source[fetus].detailedCardiac == "Yes", fetus);
		
		chestPreviewTable.bindItems([source[fetus]]);
		abdomenPreviewTable.bindItems([source[fetus]]);
		genitaliaPreviewTable.bindItems([source[fetus]]);
		extremitiesPreviewTable.bindItems([source[fetus]]);
		cordVesselsPreviewTable.bindItems([source[fetus]]);
	},
	
	_onAnatomyChanged : function(show, fetus)
	{
		var display = show ? "block" : "none";

		document.getElementById("headShapePreviewTable"+fetus).style.display = display;
		document.getElementById("headPreviewTable"+fetus).style.display = display;
		document.getElementById("nuchalFoldPreviewTable"+fetus).style.display = display;
		document.getElementById("facePreviewTable"+fetus).style.display = display;
		document.getElementById("spinePreviewTable"+fetus).style.display = display;
		document.getElementById("heartPreviewTable"+fetus).style.display = display;
		document.getElementById("chestPreviewTable"+fetus).style.display = display;
		document.getElementById("abdomenPreviewTable"+fetus).style.display = display;
		document.getElementById("genitaliaPreviewTable"+fetus).style.display = display;
		document.getElementById("extremitiesPreviewTable"+fetus).style.display = display;
		document.getElementById("cordVesselsPreviewTable"+fetus).style.display = display;
	},
	
	_showDetailedCardiac : function(show, fetus)
	{
		if(document.getElementById("CardiacPreview"+fetus)) 
			document.getElementById("CardiacPreview"+fetus).style.display = show ? "inline" : "none";
	},
	
	html : function(fetus, fetusCount)
	{
		var html = "";
		var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";

		html+= 	"		<div id=\"AnatomyPreview" + fetus + "\">";
		html+=	"			<div class=\"sectionheading\">Anatomy" + fetusLabel + "</div>";
		html+= 	"			<table id=\"anatomyAssessedPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"			<div style=\"{width:48%; float:left;}\">";
		html+= 	"				<table id=\"headShapePreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"headPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Head</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"nuchalFoldPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"facePreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Face</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"spinePreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{width:48%; float:right;}\">";
		html+= 	"				<table id=\"heartPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Heart</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"chestPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"abdomenPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Abdomen</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"genitaliaPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table class=\"extremitiesTable\" id=\"extremitiesPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Extremities</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"cordVesselsPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
		html+= 	"		</div>";

		return html;
	}
}

var CardiacPreview = {
	
	initialize : function(source, fetus)
	{
		source[fetus] = source[fetus] || {};

		var fourChamberViewPreviewTable = Table.createTable($("fourChamberViewPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Heart/Thoracic Ratio", "heartThoracicRatio"),
			new readOnlyCell("Ventricals symmetrical in size", "ventricalsSymmetrical"),
			new readOnlyCell("RV Diameter", "rvDiameter"),
			new readOnlyCell("LV Diameter", "lvDiameter"),
			new readOnlyCell("Thin and mobile valves", "thinMobileValves"),
			new readOnlyCell("Function and rhythm", "functionAndRhythm"),
			new readOnlyCell("Ventricular septum/crux", "ventricularSeptumCrux")
		]);

		var outflowTractsPreviewTable = Table.createTable($("outflowTractsPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Two Outlets", "twoOutlets"),
			new readOnlyCell("PA crosses AO", "paCrossesAO"),
			new readOnlyCell("Outlets symmetry", "outletsSymmetry"),
			new readOnlyCell("AO diameter", "aoDiameter"),
			new readOnlyCell("PA diameter", "poDiameter")
		]);

		fourChamberViewPreviewTable.bindItems([source[fetus]]);
		outflowTractsPreviewTable.bindItems([source[fetus]]);
	},
	
	html : function(fetus, fetusCount)
	{
		var html = "";
		var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";
		
		html+= 	"		<div id=\"CardiacPreview" + fetus + "\">";
		html+=	"			<div class=\"sectionheading\">Cardiac" + fetusLabel + "</div>";
		html+= 	"			<table id=\"fourChamberViewPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\">Four-Chamber View</td></tr>";
		html+= 	"			</table>";
		html+= 	"			<br />";
		html+= 	"			<table id=\"outflowTractsPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\">Outflow Tracts</td></tr>";
		html+= 	"			</table>";
		html+= 	"		</div>";

		return html;
	}
}

var WellBeingPreview = {
	
	initialize : function(source, fetus)
	{
		source[fetus] = source[fetus] || {};
		
		var wellBeingAssessedPreviewTable = Table.createTable($("wellBeingAssessedPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyAssessedCell("Well-being", "assessed")
		]);

		var bpsPreviewTable = Table.createTable($("bpsPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("AFV", "afv"),
			new readOnlyCell("Amount", "afvAmount"),
			new readOnlyCell("Max vertical pocket", "maxVerticalPocket"),
            new NewLineField(),
			new readOnlyCell("FM", "fm"),
			new readOnlyCell("FT", "ft"),
			new readOnlyCell("FBM", "fbm"),
			new readOnlyCell("NST", "nst"),
            new NewLineField(),
			{
				label: "BPS",
				cellType: "readonly",
				getValue: function(item) { return item.bpsTotal + (!isNaN(item.nst) ? "/10" : "/8"); },
				setValue: function(item, value) { return; },
				getError: function(item) { return null; }
			},
			new readOnlyCell("BPS Score", "bpsScore"),
            new NewLineField(),
			{
				label: "AFI",
				cellType: "readonly",
				getValue: function(item) { return item.afiA + " + " + item.afiB + " + " + item.afiC + " + " + item.afiD + " = " + item.afiTotal + "cm" ; },
				setValue: function(item, value) { return; },
				getError: function(item) { return null; }
			}
		]);

		var dopplerUmbilicalArteryPreviewTable = Table.createTable($("dopplerUmbilicalArteryPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("PI", "umbilicalPi"),
			new readOnlyCell("Status", "umbilicalStatus")
		]);

		var dopplerUmbilicalVeinPreviewTable = Table.createTable($("dopplerUmbilicalVeinPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("UVVmax", "uvvMax"),
			new readOnlyCell("Pulsations", "pulsations")
		]);

		var dopplerUterineArteryPreviewTable = Table.createTable($("dopplerUterineArteryPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("PI - Left", "leftUterinePi"),
			new readOnlyCell("PI - Right", "rightUterinePi"),
			new readOnlyCell("Notch - Left", "leftUterineNotch"),
			new readOnlyCell("Notch - Right", "rightUterineNotch")
		]);

		var dopplerMiddleCerebralArteryPreviewTable = Table.createTable($("dopplerMiddleCerebralArteryPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("PI", "middleCerebralPi"),
			new readOnlyCell("Peak Systolic Velocity", "peakSystolicVelocity")
		]);

		var placentaPreviewTable = Table.createTable($("placentaPreviewTable"+fetus),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Max Length", "maxLength"),
			new readOnlyCell("Max Depth", "maxDepth"),
			new readOnlyCell("Grannum grade", "grannumGrade"),
			new readOnlyCell("Texture", "texture")
		]);

		wellBeingAssessedPreviewTable.bindItems([source[fetus]]);
		this._onWellBeingChanged(source[fetus].assessed == "Assessed", fetus);
		
		bpsPreviewTable.bindItems([source[fetus]]);
		dopplerUmbilicalArteryPreviewTable.bindItems([source[fetus]]);
		dopplerUmbilicalVeinPreviewTable.bindItems([source[fetus]]);
		dopplerUterineArteryPreviewTable.bindItems([source[fetus]]);
		dopplerMiddleCerebralArteryPreviewTable.bindItems([source[fetus]]);
		placentaPreviewTable.bindItems([source[fetus]]);
	},
	
	_onWellBeingChanged : function(show, fetus)
	{
		var display = show ? "block" : "none";

		document.getElementById("bpsPreviewTable"+fetus).style.display = display;
		document.getElementById("dopplerHeading"+fetus).style.display = display;
		document.getElementById("dopplerUmbilicalArteryPreviewTable"+fetus).style.display = display;
		document.getElementById("dopplerUmbilicalVeinPreviewTable"+fetus).style.display = display;
		document.getElementById("dopplerUterineArteryPreviewTable"+fetus).style.display = display;
		document.getElementById("dopplerMiddleCerebralArteryPreviewTable"+fetus).style.display = display;
		document.getElementById("placentaPreviewTable"+fetus).style.display = display;			
	},
	
	html : function(fetus, fetusCount)
	{
		var html = "";
		var fetusLabel = fetusCount > 1 ? " - Fetus " + (fetus+1) : "";
		
		html+= 	"		<div id=\"WellBeingPreview" + fetus + "\">";
		html+=	"			<div class=\"sectionheading\">Well-Being" + fetusLabel + "</div>";
		html+= 	"			<table id=\"wellBeingAssessedPreviewTable" + fetus + "\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"			<div class=\"wellBeingBpsColumn\" style=\"{width:48%;float:left;}\">";
		html+= 	"				<table id=\"bpsPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">BPS</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div class=\"wellBeingDopplerColumn\"  style=\"{width:48%;float:right;}\">";
		html+= 	"				<div id=\"dopplerHeading" + fetus + "\" class=\"tableheading\">Doppler</div>";
		html+= 	"				<table id=\"dopplerUmbilicalArteryPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Umbilical - Artery</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"dopplerUmbilicalVeinPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Umbilical - Vein</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"dopplerUterineArteryPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Uterine Artery</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"dopplerMiddleCerebralArteryPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Middle Cerebral Artery</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{clear:both;}\">";
		html+= 	"				<table class=\"placentaTable\" id=\"placentaPreviewTable" + fetus + "\">";
		html+= 	"					<tr><td class=\"tableheading\">Placenta</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"		</div>";

		return html;
	}
}

var CommentsConclusionPreview = {

	initialize : function(source)
	{
		var commentsConclusionPreviewTable = Table.createTable($("commentsConclusionPreviewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new readOnlyCell("Sonographer's Comments", "sonographersComments"),
			new readOnlyCell("MD Opinion/Recommendation", "mdOpinion")
		]);

		commentsConclusionPreviewTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		commentsConclusionPreviewTable.bindItems([source]);
	},
	
	html : function()
	{
		var html = "";

		html+= 	"<div id=\"CommentsConclusionPreview\">";
		html+=	"	<div class=\"sectionheading\">Comments and Conclusions</div>";
		html+= 	"	<table id=\"commentsConclusionPreviewTable\">";
		html+= 	"		<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"	</table>";
		html+= 	"</div>";
		
		return html;
	}
}

/*
	Cells where the value of the specified property is rendered as read-only text.
*/
function readOnlyCell(label, prop, formatLabelLikeTableHeading)
{
	this.label = formatLabelLikeTableHeading ? "<span class=\"tableheading\">" + label + "</span>" : label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop]; };
	this.getVisible = function(item) { return item[prop] !== null && item[prop] !== undefined && item[prop] !== ""; };
}

/*
	Cells with value formatted as a date/time
*/
function readOnlyDateCell(label, prop)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	//this.getValue = function(item) { return !isNaN(item[prop]) ? Ris.formatDate(item[prop]) : ""; };
	this.getValue = function(item) { return Ris.formatDate(item[prop]); };
	this.getVisible = function(item) { return item[prop] !== null && item[prop] !== undefined && item[prop] !== ""; };
}

/*
	Cells with value formatted as an age
*/
function readOnlyAgeCell(prop, referenceDate)
{
	this.label = "Age on " + Ris.formatDate(referenceDate);
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop] + " wks"; };
	this.getVisible = function(item) { return item[prop] !== null && item[prop] !== undefined && item[prop] !== ""; };
}

/*
	Cell that is displayed only if its value is not "Assessed" 
	("assessed" sections have the section data shown)
*/
function readOnlyAssessedCell(label, prop)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop]; };
	this.getVisible = function(item) { return item[prop] && item[prop] !== "Assessed"; };
}

/*
	Cells where the value should be rendered like "XX mm = YY wks"
*/
function readOnlyBiometryCell(label, prop, calcProp)
{
	this.label = label;
	this.prop = prop;
	this.calc = calcProp;
	this.cellType = "readonly";
	this.getValue = function(item) 
	{ 
		var value = "";
		value += (item[prop] !== null && item[prop] !== undefined && item[prop] !== "") ? item[prop] + " mm" : "";
		value += (item[calcProp] !== null && item[calcProp] !== undefined && item[calcProp] !== "") ? (value != "" ? " = " + item[calcProp] + " wks" : item[calcProp] + " wks") : "";
		return value; 
	}
	this.getVisible = function(item) { return this.getValue(item) != ""; }
}
