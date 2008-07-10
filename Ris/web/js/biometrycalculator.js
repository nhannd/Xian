var BiometryCalculator = {

	crlWeeks : function(crl)
	{
		if(!crl) return 0;
		return (40.447+1.125*crl-0.0058*Math.pow(crl,2))/7;
	},

	bpdWeeks : function(bpd)
	{
		if(!bpd) return 0;		
		return 6.8954+0.26345*bpd+0.00000877*Math.pow(bpd, 3);
	},

	correctedBpd : function(bpd, ofd)
	{
		if(!bpd) return 0;
		if(!ofd) return bpd*1;
		return Math.sqrt(bpd*ofd/1.265);
	},
	
	abdomenCircumference : function(a1, a2)
	{
		return (a1+a2)*1.57;
	},
	
	abdomenCircumferenceWeeks : function(ac)
	{
		if(!ac) return 0;
		return 7.607+0.07645*ac+0.0000393*Math.pow(ac, 2);
	},
	
	femurWeeks : function(fl)
	{
		if(!fl) return 0;
		return 9.54+0.2977*fl+0.001039*Math.pow(fl, 2);
	},
	
	hcWeeks : function(hc)
	{
		if(!hc) return 0;
		return 0;
	},
	
	efw : function(useBpdcHc, useFl, Bpdc, Fl)
	{
		// TODO: compare RIS values with spreadsheet values to determine correct formula for different useX combinations
		return 0;
	},
	
	averageWeeks : function(corrBpdWks, acWks, flWks)
	{
		var numOfMeasurements = 3;
		
		if(!corrBpdWks)
		{
			corrBpdWks = 0;
			numOfMeasurements--;
		}
		if(!acWks)
		{
			acWks = 0;
			numOfMeasurements--;
		}
		if(!flWks)
		{
			flWks = 0;
			numOfMeasurements--;
		}

		if(!numOfMeasurements)
			return 0;
		
		return (corrBpdWks + acWks + flWks) / numOfMeasurements;
	}
}
