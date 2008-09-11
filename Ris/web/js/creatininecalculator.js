var CreatinineCalculator = 
{
	_standardFactors :
	{
		"Male": {a:140, b:1.2, c:1.0}, 
		"Female": {a:140, b:1.2, c:0.85},
		"Unknown": {a:0, b:0, c:0},
		"Other": {a:0, b:0, c:0}
	},
	
	calculateClearance : function(age, weight, sex, level)
	{
		age = parseFloat(age);
		weight = parseFloat(weight);
		level = parseFloat(level);

		if (["Male", "Female", "Unknown", "Other"].indexOf(sex) == -1)
		{
			throw "Invalid sex";
		}
		if (weight <= 0.0) 
		{
			throw "Weight must be greater than 0";
		}
		if (level <= 0.0) 
		{
			throw "Level must be greater than 0";
		}
		
		var result = ((this._standardFactors[sex]["a"] - age) * this._standardFactors[sex]["b"] * weight) / level * this._standardFactors[sex]["c"];
		return result.roundTo(2);
	},
	
	_gfrSexFactor :
	{
		"Male": 1.0, 
		"Female": 0.742,
		"Unknown": 1.0,
		"Other": 1.0
	},
	
	calculateGFR : function(creatinine, age, sex, black_race_p, recalibrated_p)
	{
		creatinine = parseFloat(creatinine);
		age = parseFloat(age);
		black_race_p = !!black_race_p;
		recalibrated_p = !!recalibrated_p;
		
		if (["Male", "Female", "Unknown", "Other"].indexOf(sex) == -1)
		{
			throw "Invalid sex";
		}
		
		if (age < 18.0 || age > 85.0)
		{
			throw "Invalid age.  The MDRD GFR formula is valid only for adults between 18 and 85.";
		}

        var cr = creatinine * 0.011312; // MW creatinine 113.12)
        if (cr <= 0.0003) cr = 0.0003;
        var multiplier = 186.0;
        if (recalibrated_p) {
            multiplier = 175.0;
        }

		var result = (
                multiplier
                * Math.pow(cr,-1.154)
                * Math.pow(Math.max(18,age), -0.203)
                * this._gfrSexFactor[sex]
                * (black_race_p ? 1.212 : 1.0)
                );
				
		return result.roundTo(0);
	},
	
	gfrUnits : function()
	{
		return "mL/min/1.73 m^2";
	}
}