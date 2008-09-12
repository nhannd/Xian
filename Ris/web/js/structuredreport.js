

var reportTypes = ["T1 - Dating/Nuchal", "T2 - Anatomy", "T3 - Growth for obstetrical ultrasounds"];

var StructuredReportForm = {

	_data : null,
	_reportType : "unknown",
	_fetus : 0,
	
	create : function(source, element)
	{
		element.innerHTML = this._html();
		this._initializeData(source);
	},

	startOver : function()
	{
		if(confirm('Are you sure you would like to restart the current report?  All data will be lost.'))
		{
			this._data.indicationsAndDates = {};
			this._data.indicationsAndDates.lmp = {};
			this._data.indicationsAndDates.us = {};
			this._data.indicationsAndDates.establishedEDC = {};
			this._data.general = {};
			this._data.biometry = [];
			this._data.anatomy = [];
			this._data.cardiac = [];
			this._data.wellBeing = [];
			this._data.commentsConclusion = {};
			this._ensurePerFetusDataExists();

			this._data.obusReportType = null;

			this._initializeData(this._data);
			
			this.refreshPreview();
		}
	},

	onTypeSelected : function()
	{
		if(this._data.obusReportType != null)
		{
			this._showReportContent();
		}
	},

	selectFetus : function(number)
	{
		this._fetus = parseInt(number);
		
		BiometryForm.showFetus(this._fetus);
		AnatomyForm.showFetus(this._fetus);
		CardiacForm.showFetus(this._fetus);
		WellBeingForm.showFetus(this._fetus);
	},

	refreshPreview : function()
	{
		StructuredReportPreview.create(this._data, $("Preview"));
	},
	
	_initializeData : function(source)
	{	
		this._data = source || {};
		this._reportType = "unknown";
		this._fetus = 0;

		this._data.indicationsAndDates = this._data.indicationsAndDates || {};
		this._data.indicationsAndDates.referenceDate = this._data.indicationsAndDates.referenceDate || new Date();
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
		
		if(this._data.obusReportType == null)
		{
			this._showReportTypeSelector();
		}
		else
		{
			this._showReportContent();
		}
	},
	
	_showReportTypeSelector : function()
	{
		document.getElementById("TypeSelection").style.display = 'block';
		document.getElementById("ObsrForm").style.display = 'none';
		document.getElementById("fetusSelect").style.display = 'none';
		
		errorProvider.setError($("TypeSelection"), "Please confirm report type.");
		
		this._initTypeSelectionTable();
	},
	
	_initTypeSelectionTable : function()
	{
		Table.createTable(
			$("typeSelectionTable"),
			{ editInPlace: true, flow: true, checkBoxes: false},
			[
				{
					label: "Select report type and confirm",
					cellType: "choice",
					choices: reportTypes,
					getValue: function(item){ return item.obusReportType; },
					setValue: function(item, value){ item.obusReportType = value; },
					getError: function(item){ return null; }
				}
			]);

		$("typeSelectionTable").errorProvider = errorProvider;   // share errorProvider with the rest of the form
		$("typeSelectionTable").bindItems([this._data]);
	},

	_showReportContent : function()
	{
		if(document.getElementById("TypeSelection")) document.getElementById("TypeSelection").style.display = 'none';
		if(document.getElementById("ObsrForm")) document.getElementById("ObsrForm").style.display = 'block';
		if(document.getElementById("fetusSelect")) document.getElementById("fetusSelect").style.display = 'block';
		if(document.getElementById("reportType")) document.getElementById("reportType").innerHTML = this._data.obusReportType;
		
		errorProvider.setError($("TypeSelection"), null);
		
		this._initTabs();
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
	
	_initTabs : function()
	{
		var _me = this;
		this._reportType = this._data.obusReportType;

		IndicationsAndDatesForm.initialize(this._data);
		GeneralForm.initialize(this._data, function(count) { _me._updateFetalNumber(count); } );
		BiometryForm.initialize(this._data, this._fetus);
		AnatomyForm.initialize(this._data, this._fetus);
		CardiacForm.initialize(this._data, this._fetus);
		WellBeingForm.initialize(this._data, this._fetus);
		CommentsConclusionForm.initialize(this._data);
	},

	_updateFetalNumber : function(count)
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
			
			this._ensurePerFetusDataExists();
		}
	},

	_html : function()
	{
		var html = "";
		
		html+= 	"<div id=\"TypeSelection\">";
		html+= 	"	<p class=\"sectionheading\">Report Type</p>";
		html+= 	"	<table id=\"typeSelectionTable\">";
		html+= 	"		<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"	</table>";
		html+= 	"	<input type=\"button\" value=\"Confirm\" onclick=\"javascript: StructuredReportForm.onTypeSelected()\" style=\"margin-left:1.5em;\"/>";
		html+= 	"</div>";
		html+= 	"<div id=\"ObsrForm\" class=\"TabControl\">";
		html+= 	"	<div class=\"TabHeader\">";
		html+= 	"		<input type=\"button\" value=\"Start Over\" onclick=\"javascript: StructuredReportForm.startOver()\" style=\"{float:right;vertical-align:top;}\" />";
		html+= 	"		<select id=\"fetusSelect\" onChange=\"javascript: StructuredReportForm.selectFetus(this.options[this.selectedIndex].value)\" style=\"{float:right; vertical-align:top;}\"></select>";
		html+= 	"		<h3>Report Type:&nbsp;<span id=\"reportType\"></span></h3>";
		html+= 	"		<div class=\"TabList\">";
		html+= 	"			<label for=\"IndicationsAndDates\" class=\"Tab\">Indications & Dates</label>";
		html+= 	"			<label for=\"General\" class=\"Tab\">General</label>";
		html+= 	"			<label for=\"Biometry\" class=\"Tab\">Biometry</label>";
		html+= 	"			<label for=\"Anatomy\" class=\"Tab\">Anatomy</label>";
		html+= 	"			<label id=\"CardiacTab\" for=\"Cardiac\" class=\"Tab\">Cardiac</label>";
		html+= 	"			<label for=\"WellBeing\" class=\"Tab\">Well-being</label>";
		html+= 	"			<label for=\"CommentsConclusion\" class=\"Tab\">Comments/Conclusion</label>";
		html+= 	"			<label for=\"Preview\" class=\"Tab\" onclick=\"javascript: StructuredReportForm.refreshPreview();\">Report Preview</label>";
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
		html+= 	"			<div style=\"{float:left;width:42%;}\">";
		html+= 	"				<table id=\"crlTable\">";
		html+= 	"					<tr><td class=\"tableheading\">CRL</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"bpdTable\">";
		html+= 	"					<tr><td class=\"tableheading\">BPD</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"abdTable\">";
		html+= 	"					<tr><td class=\"tableheading\">ABD</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"flTable\">";
		html+= 	"					<tr><td class=\"tableheading\">FL</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"hcTable\">";
		html+= 	"					<tr><td class=\"tableheading\">HC</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{float:left;width:28%;}\">";
		html+= 	"				<table id=\"averageSizeTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Average Size</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"efwTable\">";
		html+= 	"					<tr><td class=\"tableheading\">EFW</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{float:right;width:28%;}\">";
		html+= 	"				<table id=\"nuchalTransparencyTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Nuchal Transparency</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{clear:both;}\">&nbsp;</div>";
		html+= 	"		</div>";
		html+= 	"		<div id=\"Anatomy\" class=\"TabPage\">";
		html+=	"			<input type=\"button\" value=\"Set all Normal\" onClick=\"javascript: AnatomyForm.setAnatomyDefaults()\" style=\"{float:right; margin-top:1em;}\"></input>";
		html+= 	"			<table id=\"anatomyTable\" style=\"{width:48%;}\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"			<div style=\"{width:48%; float:left;}\">";
		html+= 	"				<table id=\"headShapeTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";  // use cell label instead
		html+= 	"				</table>";
		html+= 	"				<table id=\"headTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Head</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"nuchalFoldTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";  // use cell label instead
		html+= 	"				</table>";
		html+= 	"				<table id=\"faceTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Face</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"spineTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";  // use cell label instead
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div style=\"{width:48%; float:right;}\">";
		html+= 	"				<table id=\"heartTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Heart</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"chestTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";  // use cell label instead
		html+= 	"				</table>";
		html+= 	"				<table id=\"abdomenTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Abdomen</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"genitaliaTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";   // use cell label instead
		html+= 	"				</table>";
		html+= 	"				<table class=\"extremitiesTable\" id=\"extremitiesTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Extremities</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"cordVesselsTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";   // use cell label instead
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
		html+= 	"			<div class=\"wellBeingBpsColumn\" style=\"{width:48%;float:left;}\">";
		html+= 	"				<table id=\"bpsTable\">";
		html+= 	"					<tr><td class=\"tableheading\">BPS</td></tr>";
		html+= 	"				</table>";
		html+= 	"				<table id=\"bpsAfiTable\">";
		html+= 	"					<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"			<div class=\"wellBeingDopplerColumn\" style=\"{width:48%;float:right;}\">";
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
		html+= 	"				<table class=\"placentaTable\" id=\"placentaTable\">";
		html+= 	"					<tr><td class=\"tableheading\">Placenta</td></tr>";
		html+= 	"				</table>";
		html+= 	"			</div>";
		html+= 	"		</div>";
		html+= 	"		<div id=\"CommentsConclusion\" class=\"TabPage\">";
		html+= 	"			<table id=\"commentsConclusionTable\" width=\"98%\">";
		html+= 	"				<tr><td class=\"tableheading\"></td></tr>";
		html+= 	"			</table>";
		html+= 	"		</div>";
		html+= 	"		<div id=\"Preview\" class=\"TabPage\">";
		html+= 	"		</div>";
		html+= 	"	</div>";
		html+= 	"</div>";
		
		return html;
	}
	
}

var IndicationsAndDatesForm = {

	initialize : function(source)
	{
		var indicationChoices;
		var reportType = source.obusReportType;
		var referenceDate = source.indicationsAndDates.referenceDate;
		
		if(reportType == reportTypes[0])
		{
			indicationChoices = ["Dating", "NT", "Other"];
		}
		else if(reportType == reportTypes[1])
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
		table.bindItems([source.indicationsAndDates]);
		
		var lmpTable = Table.createTable($("lmpTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "Date of LMP",
				cellType: "date",
				getValue: function(item) { return item.LMP; },
				setValue: function(item, value) { item.LMP = value; },
				getError: function(item) { return null; }
			},
			{
				label: "EDC",
				cellType: "readonly",
				getVisible: function(item) { return !!item.LMP; },
				getValue: function(item)
                {
                    item.lmpEdc = EdcCalculator.edcFromLmp(item.LMP); 
                    return !!item.lmpEdc ? Ris.formatDate(item.lmpEdc) : "";
                },
                setValue: function(item, value) { return; }
			},
			{
				label: "Age on " + Ris.formatDate(referenceDate), 
				cellType: "readonly",
				getVisible: function(item) { return !!item.LMP; },
				getValue: function(item) 
                { 
                    item.lmpAge = EdcCalculator.differenceInWeeks(item.LMP, referenceDate) || null; 
					return item.lmpAge + " wks";
                },
                setValue: function(item, value) { return; }
			}
		]);
		
		lmpTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		lmpTable.bindItems([source.indicationsAndDates.lmp]);

		var usTable = Table.createTable($("usTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[						
			{
				label: "1st Ultrasound",
				cellType: "date",
				getValue: function(item) { return item.firstUltrasound; },
				setValue: function(item, value) { item.firstUltrasound = value; },
				getError: function(item) { return null; }
			},
			{
				label: "Age at 1st Ultrasound (wks)",
				cellType: "text",
				getValue: function(item) { return item.firstUltrasoundAge; },
				setValue: function(item, value) { item.firstUltrasoundAge = isNaN(value) ? null : Number(value).roundTo(1); },
				getError: function(item) { return null; }
			},
			{
				label: "Age on " + Ris.formatDate(referenceDate), 
				cellType: "readonly",
				getVisible: function(item) { return !!item.firstUltrasound && (!!item.firstUltrasoundAge || item.firstUltrasoundAge === 0); },
				getValue: function(item)
                { 
                    item.ultrasoundAge = EdcCalculator.differenceInWeeks(item.firstUltrasound, referenceDate) + item.firstUltrasoundAge; 
					return item.ultrasoundAge + " wks";
                },
                setValue: function(item, value) { return; }
			},
			{
				label: "EDC",
				cellType: "readonly",
				getValue: function(item) 
                { 
                    item.firstUltrasoundEdc = EdcCalculator.edcFromAge(item.ultrasoundAge, referenceDate);
                    return Ris.formatDate(item.firstUltrasoundEdc); 
                },
				getVisible: function(item) { return !!item.firstUltrasound && (!!item.firstUltrasoundAge || item.firstUltrasoundAge === 0); },
                setValue: function(item, value) { return; }
			}
		]);
		
		usTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		usTable.bindItems([source.indicationsAndDates.us]);

		var establishedEDCTable = Table.createTable($("establishedEDCTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "EDC",
				cellType: "date",
				getValue: function(item) { return item.establishedEDC; },
				setValue: function(item, value) { item.establishedEDC = value; },
				getError: function(item) { return null; }
			},
			{
				label: "Age on " + Ris.formatDate(referenceDate), 
				cellType: "readonly",
				getValue: function(item) 
                { 
                    item.establishedEDCAge = EdcCalculator.ageFromEdc(item.establishedEDC, referenceDate) || null;
					return item.establishedEDCAge + " wks";
                },
				setValue: function(item, value) { return; },
				getVisible: function(item) { return !!item.establishedEDC; }
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
				cellType: "date",
				getValue: function(item) { return item.transferredDate; },
				setValue: function(item, value) { item.transferredDate = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return item.edcMethod && item.edcMethod == "IVF"; }
			}
		]);

		establishedEDCTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		establishedEDCTable.bindItems([source.indicationsAndDates.establishedEDC]);
	}
}

var GeneralForm = {

	initialize : function(data, fetalNumberChangeCallback)
	{
		var _me = this;
		this._reportType = data.obusReportType;
		this._onFetalNumberChanged = fetalNumberChangeCallback;

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
			{
				label: "Fetal Number",
				cellType: "text",
                size: 5,
				getValue: function(item) { return item.fetalNumber = item.fetalNumber || 1; },
				setValue: function(item, value) 
                { 
                    item.fetalNumber = isNaN(value) ? item.fetalNumber : Number(value);
                    if (item.fetalNumber === 0)
                    {
                        item.fetalNumber = 1;
                    }

                    // limit input to maximum of 25 to limit the amount of additional divs created.
                    // further validation should be put in the getError function
                    if (item.fetalNumber >= 25)
                    {
                        item.fetalNumber = 25;
                    }
                    
                    _me._onFetalNumberChanged(item.fetalNumber); 
                },
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
            new NewLineField(),
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
            new NewLineField(),
			{
				label: "Placenta",
				cellType: "choice",
				choices: ["Anterior", "Fundal", "Left Lateral", "Right Lateral", "Posterior"],
				getValue: function(item) { return item.placenta; },
				setValue: function(item, value) { item.placenta = value; },
				getError: function(item) { return null; }
			},
			{
				label: "Relationship to Placenta",
				cellType: "choice",
				choices: ["Clear of Cervix", "Close to Cervix", "Praevia"],
				getValue: function(item) { return item.relationshipToPlacenta; },
				setValue: function(item, value) { item.relationshipToPlacenta = value; },
				getError: function(item) { return null; }
			},
            new NewLineField(),
			{
				label: "Right Adnexa",
				cellType: "choice",
				choices: ["Normal", "Unseen", "See comments"],
				getValue: function(item) { return item.rightAdnexa; },
				setValue: function(item, value) { item.rightAdnexa = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[0]; }
			},
			{
				label: "Left Adnexa",
				cellType: "choice",
				choices: ["Normal", "Unseen", "See comments"],
				getValue: function(item) { return item.leftAdnexa; },
				setValue: function(item, value) { item.leftAdnexa = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[0]; }
			},
			{
				label: "Yolk Sac",
				cellType: "choice",
				choices: ["Normal", "Unseen", "See comments"],
				getValue: function(item) { return item.yolkSac; },
				setValue: function(item, value) { item.yolkSac = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[0]; }
			},
			{
				label: "Presentation",
				cellType: "choice",
				choices: ["Breech", "Cephalic", "Oblique", "Transverse"],
				getValue: function(item) { return item.presentation; },
				setValue: function(item, value) { item.presentation = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[2]; }
			},
			{
				label: "AFV",
				cellType: "choice",
				choices: ["Decreased", "Increased", "Normal", "Subjectively Decreased", "Subjectively Increased"],
				getValue: function(item) { return item.afv; },
				setValue: function(item, value) { item.afv = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1]; }
			},
			{
				label: "Cervix",
				cellType: "choice",
				choices: ["Funnelling", "Normal", "Not Assessed", "Open", "Unseen"],
				getValue: function(item) { return item.cervix; },
				setValue: function(item, value) { item.cervix = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }
			},
			{
				label: "Apposed Length",
				cellType: "text",
				getValue: function(item) { return item.apposedLength; },
				setValue: function(item, value) { item.apposedLength = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }
			},
			{
				label: "Cervix Assessed",
				cellType: "choice",
				choices: ["Abdominally", "Labially", "Vaginally"],
				getValue: function(item) { return item.cervixAssessed; },
				setValue: function(item, value) { item.cervixAssessed = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }
			}
		]);

		table.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		table.bindItems([data.general]);

		this._onFetalNumberChanged(parseInt(data.general.fetalNumber) || 1);
	}
}


function CalculatedBiometryCell(label, prop, getVisible)
{
	this.label = label;
	this.prop = prop;
	this.cellType = "readonly";
	this.getValue = function(item) { return item[prop]; };
	this.getError = function(item) { return null; };
	this.setValue = function(item, value) { return; };  // required for hidden readonly fields
	if (getVisible) this.getVisible = getVisible;
}

var BiometryForm = {

	UpdateAvgSize : function()
	{
		averageSizeTable.bindItems([this._data[this._fetus]]);
	},

	UpdateEfw : function()
	{
		efwTable.bindItems([this._data[this._fetus]]);
	},

	initialize : function(data, fetus)
	{
		this._data = data.biometry;
		this._reportType = data.obusReportType;
		this._fetus = fetus;
		var _me = this;

		var biometryAssessedTable = Table.createTable($("biometryAssessedTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: "Biometry",
				cellType: "choice",
				choices: ["Assessed", "Not Assessed"],
				getValue: function(item) { return item.assessed; },
				setValue: function(item, value) { item.assessed = value; _me._onBiometryChanged(value == "Assessed"); },
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
				getValue: function(item) { return item.crl = item.crl || null; },
				setValue: function(item, value) 
				{ 
					item.crl = parseInt(value) || null;
					item.crlWks = BiometryCalculator.crlWeeks(item.crl);
				},
				getError: function(item) { return null; }
			},
			new CalculatedBiometryCell("wks", "crlWks")
		]);
		
		crlTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var bpdTable = Table.createTable($("bpdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				// biparietal diameter
				label: "BPD mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.bpd; },
				setValue: function(item, value) 
				{ 
					item.bpd = parseInt(value) || null;
					
					item.bpdWks = BiometryCalculator.bpdWeeks(item.bpd);
					item.correctedBpd = BiometryCalculator.correctedBpd(item.bpd, item.ofd);
					item.correctedBpdWks = BiometryCalculator.correctedBpdWeeks(item.bpd, item.ofd);
				
					item.avgWks = BiometryCalculator.averageWeeks(item.correctedBpdWks, item.abdCircumferenceWks, item.flWks);
					
					BiometryForm.UpdateAvgSize();
					BiometryForm.UpdateEfw();
				},
				getError: function(item) { return null; }
			},
			new CalculatedBiometryCell("wks", "bpdWks"),
			{
				label: "OFD mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.ofd; },
				setValue: function(item, value) 
				{ 
					item.ofd = parseInt(value) || null;
					
					item.correctedBpd = BiometryCalculator.correctedBpd(item.bpd, item.ofd);
					item.correctedBpdWks = BiometryCalculator.correctedBpdWeeks(item.bpd, item.ofd);

					item.avgWks = BiometryCalculator.averageWeeks(item.correctedBpdWks, item.abdCircumferenceWks, item.flWks);
					
					BiometryForm.UpdateAvgSize();
					BiometryForm.UpdateEfw();
				},
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }
			},
			new CalculatedBiometryCell("Corrected BPD mm", "correctedBpd", function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }),
			new CalculatedBiometryCell("wks", "correctedBpdWks", function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; })
		]);
		
		bpdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var abdTable = Table.createTable($("abdTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "A1 mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.abdX; },
				setValue: function(item, value) 
				{ 
					item.abdX = parseInt(value) || null; 
					
					item.abdCircumference = BiometryCalculator.abdomenCircumference(item.abdX, item.abdY);
					item.abdCircumferenceWks = BiometryCalculator.abdomenCircumferenceWeeks(item.abdX, item.abdY);

					item.avgWks = BiometryCalculator.averageWeeks(item.correctedBpdWks, item.abdCircumferenceWks, item.flWks);

					BiometryForm.UpdateAvgSize();
					BiometryForm.UpdateEfw();
				},
				getError: function(item) { return null; }
			},
			{
				label: "A2 mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.abdY; },
				setValue: function(item, value) 
				{ 
					item.abdY = parseInt(value) || null; 

					item.abdCircumference = BiometryCalculator.abdomenCircumference(item.abdX, item.abdY);
					item.abdCircumferenceWks = BiometryCalculator.abdomenCircumferenceWeeks(item.abdX, item.abdY);

					item.avgWks = BiometryCalculator.averageWeeks(item.correctedBpdWks, item.abdCircumferenceWks, item.flWks);

					BiometryForm.UpdateAvgSize();
					BiometryForm.UpdateEfw();
				},
				getError: function(item) { return null; }
			},
			new CalculatedBiometryCell("AC mm", "abdCircumference"),
			new CalculatedBiometryCell("wks", "abdCircumferenceWks")
		]);
		
		abdTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var flTable = Table.createTable($("flTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "FL mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.fl; },
				setValue: function(item, value) 
				{ 
					item.fl = parseInt(value) || null;
					
					item.flWks = BiometryCalculator.femurWeeks(item.fl);

					item.avgWks = BiometryCalculator.averageWeeks(item.correctedBpdWks, item.abdCircumferenceWks, item.flWks);

					BiometryForm.UpdateAvgSize();
					BiometryForm.UpdateEfw();
				},
				getError: function(item) { return null; }
			},
			new CalculatedBiometryCell("wks", "flWks")
		]);
		
		flTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var averageSizeTable = Table.createTable($("averageSizeTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			new CalculatedBiometryCell("wks", "avgWks")
		]);

		averageSizeTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var hcTable = Table.createTable($("hcTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "mm", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.hc; },
				setValue: function(item, value) 
				{ 
					item.hc = parseInt(value) || null; 
					item.hcWks = BiometryCalculator.hcWeeks(item.hc);
				},
				getError: function(item) { return null; }
			},
			new CalculatedBiometryCell("wks", "hcWks")
		]);

		hcTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var efwTable = Table.createTable($("efwTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "Use BPDc/HC", 
				cellType: "bool",
				getValue: function(item) { return item.useBpdcHc; },
				setValue: function(item, value) 
				{ 
					item.useBpdcHc = value; 
					item.efw = BiometryCalculator.efw(item.useBpdcHc, item.useFl, item.abdX, item.abdY, item.fl, item.bpd, item.ofd);
				},
				getError: function(item) { return null; }
			},
			{
				label: "Use FL", 
				cellType: "bool",
				getValue: function(item) { return item.useFl; },
				setValue: function(item, value) 
				{ 
					item.useFl = value; 
					item.efw = BiometryCalculator.efw(item.useBpdcHc, item.useFl, item.abdX, item.abdY, item.fl, item.bpd, item.ofd);
				},
				getError: function(item) { return null; }
			},
            new NewLineField(),
			new CalculatedBiometryCell("EFW (gm)", "efw"),
			{
				label: "Percentile", 
				cellType: "text",
				size: 5,
				getValue: function(item) { return item.efwCentile; },
				setValue: function(item, value) { item.efwCentile = parseInt(value) || null; },
				getError: function(item) { return null; }
			}
		]);

		efwTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var nuchalTransparencyTable = Table.createTable($("nuchalTransparencyTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			{
				label: "mm", 
				cellType: "text",
				getValue: function(item) { return item.nuchalTransparency; },
				setValue: function(item, value) { item.nuchalTransparency = value; },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1]; }
			}
		]);

		nuchalTransparencyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
		
		this.showFetus(fetus);
	},

	showFetus : function(fetus)
	{
		this._data[fetus] = this._data[fetus] || {};

		biometryAssessedTable.bindItems([this._data[fetus]]);
		this._onBiometryChanged(this._data[fetus].assessed == "Assessed");
		
		crlTable.bindItems([this._data[fetus]]);
		bpdTable.bindItems([this._data[fetus]]);
		abdTable.bindItems([this._data[fetus]]);
		flTable.bindItems([this._data[fetus]]);
		averageSizeTable.bindItems([this._data[fetus]]);
		hcTable.bindItems([this._data[fetus]]);
		efwTable.bindItems([this._data[fetus]]);
		nuchalTransparencyTable.bindItems([this._data[fetus]]);
	},

	_onBiometryChanged : function(show)
	{
		document.getElementById("crlTable").style.display = (show && this._reportType == reportTypes[0]) ? "block" : "none";
		document.getElementById("bpdTable").style.display = (show) ? "block" : "none";
		document.getElementById("abdTable").style.display = (show && (this._reportType == reportTypes[1] || this._reportType == reportTypes[2])) ? "block" : "none";
		document.getElementById("flTable").style.display = (show && (this._reportType == reportTypes[1] || this._reportType == reportTypes[2])) ? "block" : "none";
		document.getElementById("averageSizeTable").style.display = (show && (this._reportType == reportTypes[1] || this._reportType == reportTypes[2])) ? "block" : "none";
		document.getElementById("efwTable").style.display = (show && (this._reportType == reportTypes[2])) ? "block" : "none";
		document.getElementById("hcTable").style.display = (show && (this._reportType == reportTypes[1] || this._reportType == reportTypes[2])) ? "block" : "none";
		document.getElementById("nuchalTransparencyTable").style.display = (show && this._reportType == reportTypes[1]) ? "block" : "none";
	}
}

var AnatomyForm = {

	initialize : function(data, fetus)
	{
		this._data = data.anatomy;
		this._reportType = data.obusReportType;
		var _me = this;
	
		var anatomyTable = Table.createTable($("anatomyTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: "Anatomy",
				cellType: "choice",
				choices: ["Assessed", "Not assessed", "Not done because previously assessed"],
				getValue: function(item) { return item.assessed; },
				setValue: function(item, value) { item.assessed = value; _me._onAnatomyChanged(value == "Assessed"); },
				getError: function(item) { return null; }
			}
		]);
		
		anatomyTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var headShapeTable = Table.createTable($("headShapeTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Head Shape", "headShape", true)
		]);			
		
		headShapeTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var headTable = Table.createTable($("headTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[			
			new StandardAnatomyCell("Posterior horns", "posteriorHorns"),
			new StandardAnatomyCell("Choroid plexi", "choroidPlexi"),
			new StandardAnatomyCell("Cavum septi", "cavumSepti"),
			new StandardAnatomyCell("Cerebellum", "cerebellum"),
			new StandardAnatomyCell("Cisterna magna", "cisternaMagna")
		]);
		
		headTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var nuchalFoldTable = Table.createTable($("nuchalFoldTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Nuchal Fold", "nuchalFold", true)
		]);
		
		nuchalFoldTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var faceTable = Table.createTable($("faceTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Eyes", "eyes"),
			{
				label: "Orbital ratio (i/o)",
				cellType: "text", 
				getValue: function(item) { return item.orbitalRatio },
				setValue: function(item, value) { item.orbitalRatio = value; },
				getError: function(item) { return null; }
			},
			new StandardAnatomyCell("Mouth", "mouth"),
			new StandardAnatomyCell("Profile", "profile")
		]);
		
		faceTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var spineTable = Table.createTable($("spineTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Spine", "spine", true)
		]);

		spineTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var heartTable = Table.createTable($("heartTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("4 Chambers", "fourChambers"),
			new StandardAnatomyCell("Great vessels", "greatVessels"),
			{
				label: "Detailed Cardiac",
				cellType: "choice", 
				choices: ["Yes", "No"],
				getValue: function(item) { return item.detailedCardiac; },
				setValue: function(item, value) { item.detailedCardiac = value; _me._onDetailedCardiacChanged(value == "Yes"); },
				getError: function(item) { return null; },
				getVisible: function(item) { return _me._reportType == reportTypes[1] || _me._reportType == reportTypes[2]; }
			}
		]);
		
		this._onDetailedCardiacChanged(this._data.detailedCardiac == "Yes");

		heartTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var chestTable = Table.createTable($("chestTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Chest", "chest", true)
		]);
		
		chestTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var abdomenTable = Table.createTable($("abdomenTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Diaphragm", "diaphragm"),
			new StandardAnatomyCell("Stomach", "stomach"),
			new StandardAnatomyCell("Abdominal wall", "abdominalWall"),
			new StandardAnatomyCell("Kidneys - RT", "kidneysRt"),
			new StandardAnatomyCell("Kidneys - LT", "kidneysLt"),
			new StandardAnatomyCell("Bladder", "bladder")
		]);
		
		abdomenTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var genitaliaTable = Table.createTable($("genitaliaTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: formatLikeTableHeading("Genitalia"),
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
			new StandardAnatomyCell("Upper - RT", "upperRt"),
			new StandardAnatomyCell("Upper - LT", "upperLt"),
			new StandardAnatomyCell("Lower - RT", "lowerRt"),
			new StandardAnatomyCell("Lower - LT", "lowerLt")
		]);
		
		extremitiesTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var cordVesselsTable = Table.createTable($("cordVesselsTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: formatLikeTableHeading("Cord Vessels"),
				cellType: "choice", 
				choices: ["2", "3"],
				getValue: function(item) { return item.cordVessels = item.cordVessels || "2"; },
				setValue: function(item, value) { item.cordVessels = value; },
				getError: function(item) { return null; }
			}
		]);

		cordVesselsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		this.showFetus(0);
	},
	
	showFetus : function(fetus)
	{
		this._fetus = fetus;
		
		this._data[fetus] = this._data[fetus] || {};

		anatomyTable.bindItems([this._data[fetus]]);
		this._onAnatomyChanged(this._data[fetus].assessed == "Assessed");

		headShapeTable.bindItems([this._data[fetus]]);
		headTable.bindItems([this._data[fetus]]);
		nuchalFoldTable.bindItems([this._data[fetus]]);
		faceTable.bindItems([this._data[fetus]]);
		spineTable.bindItems([this._data[fetus]]);
		heartTable.bindItems([this._data[fetus]]);
		this._onDetailedCardiacChanged(this._data[fetus].detailedCardiac == "Yes");
		
		chestTable.bindItems([this._data[fetus]]);
		abdomenTable.bindItems([this._data[fetus]]);
		genitaliaTable.bindItems([this._data[fetus]]);
		extremitiesTable.bindItems([this._data[fetus]]);
		cordVesselsTable.bindItems([this._data[fetus]]);
	},
	
	setAnatomyDefaults : function()
	{
		if(!this._data[this._fetus]) return;

		var anatomyProperties = ["headShape", "posteriorHorns", "choroidPlexi", "cavumSepti", "cerebellum", "cisternaMagna", "nuchalFold", "eyes", "mouth", 
				"profile", "spine", "fourChambers", "greatVessels", "chest", "diaphragm", "stomach", "abdominalWall", "kidneysRt", "kidneysLt", "bladder", "upperRt", "upperLt", 
				"lowerRt", "lowerLt", "cordVessels"];
				
		for(var i = 0; i < anatomyProperties.length; i++)
		{
			if(typeof(this._data[this._fetus][anatomyProperties[i]]) == "undefined" || this._data[this._fetus][anatomyProperties[i]] == null || this._data[this._fetus][anatomyProperties[i]] == "")
			{
				this._data[this._fetus][anatomyProperties[i]] = "Normal";
			}
		}
		
		this.showFetus(this._fetus);
	},

	_onAnatomyChanged : function(show)
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
	},

	_onDetailedCardiacChanged : function(show)
	{
		if(document.getElementById("CardiacTab")) document.getElementById("CardiacTab").style.display = show ? "inline" : "none";
	}
}

var CardiacForm = {

	initialize : function(data, fetus)
	{
		this._data = data.cardiac;
		this._reportType = data.obusReportType;
		var _me = this;
	
		var fourChamberViewTable = Table.createTable($("fourChamberViewTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Heart/Thoracic Ratio", "heartThoracicRatio"),
			new StandardAnatomyCell("Ventricals symmetrical in size", "ventricalsSymmetrical"),
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
				getValue: function(item) { return item.lvDiameter; },
				setValue: function(item, value) { item.lvDiameter = value; },
				getError: function(item) { return null; }
			},
			new StandardAnatomyCell("Thin and mobile valves", "thinMobileValves"),
			new StandardAnatomyCell("Function and rhythm", "functionAndRhythm"),
			new StandardAnatomyCell("Ventricular septum/crux", "ventricularSeptumCrux")
		]);

		fourChamberViewTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var outflowTractsTable = Table.createTable($("outflowTractsTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			new StandardAnatomyCell("Two Outlets", "twoOutlets"),
			new StandardAnatomyCell("PA \"crosses\" AO", "paCrossesAO"),
			new StandardAnatomyCell("Outlets symmetry", "outletsSymmetry"),
			{
				label: "AO diameter",
				cellType: "text",
				getValue: function(item) { return item.aoDiameter; },
				setValue: function(item, value) { item.aoDiameter = value; },
				getError: function(item) { return null; }
			},
			{
				label: "PA diameter",
				cellType: "text",
				getValue: function(item) { return item.poDiameter; },
				setValue: function(item, value) { item.poDiameter = value; },
				getError: function(item) { return null; }
			}
		]);

		outflowTractsTable .errorProvider = errorProvider;   // share errorProvider with the rest of the form

		this.showFetus(fetus);
	},
	
	showFetus : function(fetus)
	{
		this._data[fetus] = this._data[fetus] || {};

		fourChamberViewTable.bindItems([this._data[fetus]]);
		outflowTractsTable.bindItems([this._data[fetus]]);
	}
}

var WellBeingForm = {

	initialize : function(data, fetus)
	{
		this._data = data.wellBeing;
		this._reportType = data.obusReportType;
		var _me = this;
	
		var wellBeingTable = Table.createTable($("wellBeingTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: "Well-being",
				cellType: "choice",
				choices: ["Assessed", "Not Assessed"],
				getValue: function(item) { return item.assessed = item.assessed || "Not Assessed"; },
				setValue: function(item, value) { item.assessed = value; _me._onWellBeingChanged(value == "Assessed"); },
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
				label: "Amount",
				cellType: "choice",
                choices: ["0", "2"],
				getValue: function(item) { return item.afvAmount = item.afvAmount || 0; },
				setValue: function(item, value) { item.afvAmount = Number(value); },
				getError: function(item) { return null; }
			},
			{
				label: "Max vertical pocket",
				cellType: "text",
				getValue: function(item) { return item.maxVerticalPocket; },
				setValue: function(item, value) { item.maxVerticalPocket = value; },
				getError: function(item) { return null; }
			},
            new NewLineField(),
			{
				label: "FM",
				cellType: "choice",
                choices: ["0", "2"],
				getValue: function(item) { return item.fm = item.fm || 0; },
				setValue: function(item, value) { item.fm = Number(value); },
				getError: function(item) { return null; }
			},
			{
				label: "FT",
				cellType: "choice",
                choices: ["0", "2"],
				getValue: function(item) { return item.ft = item.ft || 0; },
				setValue: function(item, value) { item.ft = Number(value); },
				getError: function(item) { return null; }
			},
			{
				label: "FBM",
				cellType: "choice",
                choices: ["0", "2"],
				getValue: function(item) { return item.fbm = item.fbm || 0; },
				setValue: function(item, value) { item.fbm = Number(value); },
				getError: function(item) { return null; }
			},
			{
				label: "NST",
				cellType: "choice",
                choices: ["N/A", "0", "2"],
				getValue: function(item) { return item.nst = (item.nst || item.nst === 0) ? item.nst : "N/A"; },
				setValue: function(item, value) { item.nst = Number(value); },
				getError: function(item) { return null; }
			},
            new NewLineField(),
			{
				label: "BPS",
				cellType: "readonly",
				getValue: function(item) 
                { 
                    var of = "/8";
                    item.bpsTotal = item.afvAmount + item.fm + item.ft + item.fbm;
                    if(!isNaN(item.nst))
                    {
                        item.bpsTotal += item.nst;
                        of = "/10";
                    }
                    return item.bpsTotal + of; 
                }
			},
			{
				label: "BPS Score",
				cellType: "choice",
				choices: ["Abnormal", "Equivocal", "Normal"],
				getValue: function(item) { return item.bpsScore; },
				setValue: function(item, value) { item.bpsScore = value; },
				getError: function(item) { return null; }
			}
		]);

		bpsTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		var bpsAfiTable = Table.createTable($("bpsAfiTable"),{ editInPlace: true, flow: true, checkBoxes: false},
		[
			{
				label: "AFI",
				cellType: "text",
                size: 5,
				getValue: function(item) { return item.afiA = item.afiA || 0; },
				setValue: function(item, value) { item.afiA = !isNaN(value) ? Number(value) : 0; },
				getError: function(item) { return null; }
			},
			{
				label: "",
				cellType: "readonly",
				getValue: function(item) { return "+"; }
			},
			{
				label: "",
				cellType: "text",
                size: 5,
				getValue: function(item) { return item.afiB = item.afiB || 0; },
				setValue: function(item, value) { item.afiB = !isNaN(value) ? Number(value) : 0; },
				getError: function(item) { return null; }
			},
			{
				label: "",
				cellType: "readonly",
				getValue: function(item) { return "+"; }
			},
			{
				label: "",
				cellType: "text",
                size: 5,
				getValue: function(item) { return item.afiC = item.afiC || 0; },
				setValue: function(item, value) { item.afiC = !isNaN(value) ? Number(value) : 0; },
				getError: function(item) { return null; }
			},
			{
				label: "",
				cellType: "readonly",
				getValue: function(item) { return "+"; }
			},
			{
				label: "",
				cellType: "text",
                size: 5,
				getValue: function(item) { return item.afiD = item.afiD || 0; },
				setValue: function(item, value) { item.afiD = !isNaN(value) ? Number(value) : 0; },
				getError: function(item) { return null; }
			},
			{
				label: "",
				cellType: "readonly",
				getValue: function(item) { return "="; }
			},
			{
				label: "AFI Total",
				cellType: "readonly",
				getValue: function(item) { return item.afiTotal = item.afiA + item.afiB + item.afiC + item.afiD; },
				getError: function(item) { return null; }
			}
		]);

		bpsAfiTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

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
				label: "Status",
				cellType: "choice",
				choices: ["Absent EDF", "Elevated", "Normal", "Reversed EDF"],
				getValue: function(item) { return item.umbilicalStatus; },
				setValue: function(item, value) { item.umbilicalStatus = value; },
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
				getValue: function(item) { return item.leftUterineNotch; },
				setValue: function(item, value) { item.leftUterineNotch = value; },
				getError: function(item) { return null; }
			},
			{
				label: "Notch - Right",
				cellType: "choice",
				choices: ["Yes", "No"],
				getValue: function(item) { return item.rightUterineNotch; },
				setValue: function(item, value) { item.rightUterineNotch = value; },
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
				getValue: function(item) { return item.texture = item.texture || "Normal"; },
				setValue: function(item, value) { item.texture = value; },
				getError: function(item) { return null; }
			}
		]);

		placentaTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form

		this.showFetus(fetus);
	},
	
	showFetus : function(fetus)
	{
		this._data[fetus] = this._data[fetus] || {};
		
		wellBeingTable.bindItems([this._data[fetus]]);
		this._onWellBeingChanged(this._data[fetus].assessed == "Assessed");
		
		bpsTable.bindItems([this._data[fetus]]);
		bpsAfiTable.bindItems([this._data[fetus]]);
		dopplerUmbilicalArteryTable.bindItems([this._data[fetus]]);
		dopplerUmbilicalVeinTable.bindItems([this._data[fetus]]);
		dopplerUterineArteryTable.bindItems([this._data[fetus]]);
		dopplerMiddleCerebralArteryTable.bindItems([this._data[fetus]]);
		placentaTable.bindItems([this._data[fetus]]);
	},
	
	_onWellBeingChanged : function(show)
	{
		var display = show ? "block" : "none";

		document.getElementById("bpsTable").style.display = display;
		document.getElementById("bpsAfiTable").style.display = display;
		document.getElementById("dopplerHeading").style.display = display;
		document.getElementById("dopplerUmbilicalArteryTable").style.display = display;
		document.getElementById("dopplerUmbilicalVeinTable").style.display = display;
		document.getElementById("dopplerUterineArteryTable").style.display = display;
		document.getElementById("dopplerMiddleCerebralArteryTable").style.display = display;
		document.getElementById("placentaTable").style.display = display;			
	}
}

var CommentsConclusionForm = {

	initialize : function(data)
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
}

function StandardAnatomyCell(label, prop, formatLabelLikeTableHeading)
{
	this.label = formatLabelLikeTableHeading ? formatLikeTableHeading(label) : label;
	this.prop = prop;
	this.cellType = "choice";
	this.choices = ["Normal", "Not seen", "See comment"];
	this.getValue = function(item) { return item[prop]; };
	this.setValue = function(item, value) { item[prop] = value; };
	this.getError = function(item) { return null; };
}

function formatLikeTableHeading(label)
{
	return "<span class=\"tableheading\">" + label + "</span>";
}
