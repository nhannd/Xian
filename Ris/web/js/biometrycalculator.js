var BiometryCalculator = {

	crlWeeks : function(crl)
	{
		if (!crl) return 0;
		return (40.447+1.125*crl-0.0058*Math.pow(crl,2))/7;
	},

	bpdWeeks : function(bpd)
	{
		if (!bpd) return 0;		
		return 6.8954+0.26345*bpd+0.00000877*Math.pow(bpd, 3);
	},

	correctedBpd : function(bpd, ofd)
	{
		if (!bpd) return 0;
		if (!ofd) return bpd*1;
		return Math.sqrt(bpd*ofd/1.265);
	},
	
	abdomenCircumference : function(a1, a2)
	{
		return (a1+a2)*1.57;
	},
	
	abdomenCircumferenceWeeks : function(ac)
	{
		if (!ac) return 0;
		return 7.607+0.07645*ac+0.0000393*Math.pow(ac, 2);
	},
	
	femurWeeks : function(fl)
	{
		if (!fl) return 0;
		return 9.54+0.2977*fl+0.001039*Math.pow(fl, 2);
	},
	
	hcWeeks : function(hc)
	{
		if (!hc) return 0;
		return 0;
	},
	
	efw : function(useBpdcHc, useFl, ac, fl, bpdc)
	{
        useBpdcHc = !!useBpdcHc;
        useFl = !!useFl;
        ac = Number(ac);
        fl = Number(fl);
        bpdc = Number(bpdc);

        var exponent;

        if (useBpdcHc && useFl) {
            // Log10w=1.4787 - .003343ac*f +.001837b*b + .0458ac + .158f  (ac, f, b in cm)
            // Following ormula adjusted for mm
            exponent = 1.4787 - (0.00003343 * ac * fl) + (0.00001837 * bpdc * bpdc) + (0.00458 * ac) + (0.0158 * fl);
        }
        else if (useBpdcHc) {
            // log10w= 1.1134 + .05845ac - .000604 ac*ac - .007365b*b + .000595b*ac + .1694b   (ac, b in cm)
            // Following ormula adjusted for mm
            exponent = 1.1134 + (0.005845 * ac) - (0.00000604 * ac * ac) - (0.00007365 * bpdc * bpdc) + (0.00000595 * bpdc * ac) + (0.01694 * bpdc)
        }
        else if (useFl) {
            // log10w = 1.3598 + .051ac +.1844f - .0037ac*f   (ac, f in cm)
            // Following ormula adjusted for mm
            exponent = 1.3598 + (0.0051 * ac) + (0.01844 * fl) - (0.000037 * ac * fl);
        }
        else {
            return 0;
        }

        var weight = Math.pow(10, exponent);
        weight /= 10;
        weight = Math.ceil(weight);
        weight *= 10;
        return weight;
	},
	
	averageWeeks : function(corrBpdWks, acWks, flWks)
	{
		var numOfMeasurements = 3;
		
		if (!corrBpdWks) {
			corrBpdWks = 0;
			numOfMeasurements--;
		}
		if (!acWks) {
			acWks = 0;
			numOfMeasurements--;
		}
		if (!flWks) {
			flWks = 0;
			numOfMeasurements--;
		}

		if (!numOfMeasurements) return 0;
		
		return (corrBpdWks + acWks + flWks) / numOfMeasurements;
	}
}
