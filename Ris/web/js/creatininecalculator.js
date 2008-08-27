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
	}
}