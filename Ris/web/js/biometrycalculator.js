var BiometryCalculator = {
	_precision : 1,

	crlWeeks : function(crl)
	{
		if (!crl) return 0;
		var result = (40.447+1.125*crl-0.0058*Math.pow(crl,2))/7;
		return result.roundTo(this._precision);
	},

	_bpdWeeksImpl : function(bpd)
	{
		if (!bpd) return 0;
		return 6.8954+0.26345*bpd+0.00000877*Math.pow(bpd, 3);
	},
	
	bpdWeeks : function(bpd)
	{
		var result = this._bpdWeeksImpl(bpd);
		return result.roundTo(this._precision);
	},

	_correctedBpdImpl : function(bpd, ofd)
	{
		if (!bpd) return 0;
		if (!ofd) return bpd*1;

		return Math.sqrt(bpd*ofd/1.265);
	},
	
	correctedBpd : function(bpd, ofd)
	{
		var result = this._correctedBpdImpl(bpd, ofd);
		return result.roundTo(this._precision);
	},
	
	correctedBpdWeeks : function(bpd, ofd)
	{
		var bpdc = this._correctedBpdImpl(bpd, ofd);
		var result = this._bpdWeeksImpl(bpdc);
		return result.roundTo(this._precision);
	},
	
	_abdomenCircumferenceImpl : function(a1, a2)
	{
		if (!a1) return 0;
		if (!a2) return 0;

		return (a1+a2)*1.57;
	},
	
	abdomenCircumference : function(a1, a2)
	{
		var result = this._abdomenCircumferenceImpl(a1, a2);
		return result.roundTo(this._precision);
	},
	
	abdomenCircumferenceWeeks : function(a1, a2)
	{
		var ac = this._abdomenCircumferenceImpl(a1, a2);
		if (!ac) return 0;

		var result =  7.607+0.07645*ac+0.0000393*Math.pow(ac, 2);
		return result.roundTo(this._precision);
	},
	
	femurWeeks : function(fl)
	{
		if (!fl) return 0;

		var result =  9.54+0.2977*fl+0.001039*Math.pow(fl, 2);
		return result.roundTo(this._precision);
	},
	
	hcWeeks : function(hc)
	{
		if (!hc) return 0;

		var result =  0;
		return result.roundTo(this._precision);
	},
	
	gsWeeks : function(gs1, gs2, gs3)
	{
		if (!gs1) return null;
		if (!gs2) return null;
		if (!gs3) return null;

		var result = (gs1+gs2+gs3)/3*0.132+4.299;  // Nyberg
		//var result = ((gs1+gs2+gs3)/30+2.543)/0.702;  // Hellman.AmJOG 103:789. 1969
		return result.roundTo(this._precision);
	},
	
	efw : function(useBpdcHc, useFl, acX, acY, fl, bpd, ofd)
	{
        useBpdcHc = !!useBpdcHc;
        useFl = !!useFl;
        acX = Number(acX);
        acY = Number(acY);
        fl = Number(fl);
        bpd = Number(bpd);
		ofd = Number(ofd);

        var exponent;
		var ac = this._abdomenCircumferenceImpl(acX, acY);
		var bpdc = this._correctedBpdImpl(bpd, ofd);

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
		
		var result = (corrBpdWks + acWks + flWks) / numOfMeasurements;
		return result.roundTo(this._precision);
	},
	
	ageEfwStatistics : 
	{
		25 : { average: 850, deviation: 100 },
		26 : { average: 933, deviation: 115 },
		27 : { average: 1016, deviation: 135 },
		28 : { average: 1113, deviation: 150 },
		29 : { average: 1228, deviation: 165 },
		30 : { average: 1373, deviation: 175 },
		31 : { average: 1540, deviation: 200 },
		32 : { average: 1727, deviation: 225 },
		33 : { average: 1900, deviation: 250 },
		34 : { average: 2113, deviation: 280 },
		35 : { average: 2347, deviation: 315 },
		36 : { average: 2589, deviation: 350 },
		37 : { average: 2868, deviation: 375 },
		38 : { average: 3133, deviation: 400 },
		39 : { average: 3360, deviation: 430 },
		40 : { average: 3480, deviation: 460 },
		41 : { average: 3567, deviation: 475 },
		42 : { average: 3513, deviation: 480 },
		43 : { average: 3416, deviation: 485 }
	},

	ageEfwPercentile : function(ageWks, efw)
	{
		ageWks = ageWks.roundTo(0);
		
		if (!efw)
			return null;
	
		if(ageWks < 25 || ageWks > 43)
			return null;
		
		var average = this.ageEfwStatistics[ageWks].average;
		var deviation = this.ageEfwStatistics[ageWks].deviation;
		var percentile = this.normalv(efw, average, deviation)
		
		return (100 * percentile).roundTo(0);
	},
	
	// Used courtasy of Dr. Achim Lewandowski
	// From:  http://www.alewand.de/stattab/tabstete.htm
	normalv : function(xwert,muewert,sigmawert)
	{
		x=eval(xwert);
        mue=eval(muewert);
        sigma=eval(sigmawert);
		
        if (eval(sigma)<=0) return "Error: Sigma<=0 !";

        if (eval(sigma)>0)
        {
			var  LTONE = 8;
            var  CON = 1.28;
            var  A1 = 0.398942280444;
			var  A2 = 0.399903438504;
		    var  A3 = 5.75885480458;
		    var  A4 = 29.8213557808;
		    var  A5 = 2.62433121679;
		    var  A6 = 48.6959930692;
		    var  A7 = 5.92885724438;
	 
		    var  B1 = 0.398942280385;
		    var  B2 = 0.8052E-8;
		    var  B3 = 1.00000615302;
		    var  B4 = 3.98064794E-4;
		    var  B5 = 1.98615381364;
		    var  B6 = 0.151679116635;
		    var  B7 = 5.29330324926;
		    var  B8 = 4.8385912808;
		    var  B9 = 15.1508972451;
		    var  B10= 0.742380924027;
		    var  B11= 30.789933034;
		    var  B12= 3.99019417011;
 
			var  UTZERO;
	        var  Z;
	        var  YY;
	        var  ALNORM;
	        var  DYN;
			var  UP;

			UTZERO = Math.pow(-2 * (Math.log(1.E-300)+1),0.5) - 0.3;
			UP= 0;

			Z = eval((x - mue)/sigma);
			if (Z < 0)
            {
				UP = 1;
                Z = -Z;
            }
			
			if ((Z > LTONE) && ((UP==1)|| (Z > UTZERO)))
				ALNORM = 0;

			if ((Z <= LTONE) || ((UP!=1)|| (Z <= UTZERO)))
		    {	
				yy = 0.5 * Z * Z;
				if (Z <=  CON)
                {
                    ALNORM = 0.5 - Z * (A1 - A2 * yy/(yy + A3 - A4/(yy + A5 + A6/(yy + A7))));
				}
				if (Z>CON)
                {
                    ALNORM = Math.exp(-yy) * B1;
					dummy = (Z + B4 + B5/(Z - B6 + B7/(Z + B8 - B9/(Z + B10 + B11/(Z +B12)))));
					ALNORM = ALNORM / (Z - B2 + B3/dummy);
				}
		    }
 
			if (UP==0) DYN = 1-ALNORM;
            if (UP!=0) DYN = ALNORM;

			return DYN
        }
    }
}
