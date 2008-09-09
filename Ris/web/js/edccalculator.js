var EdcCalculator = {

    _daysToMilliseconds: function(days) {
        if (isNaN(days)) throw {name:"Invalid parameter days", message:"days must be a number"};
        
        days = Number(days);
        return days * 24 * 60 * 60 * 1000;
    },
    
    edcFromLmp: function(lmp) {
        if (isNaN(lmp)) return undefined;

        return new Date(Number(lmp) + this._daysToMilliseconds(280));
    },
    
    differenceInWeeks: function(startDate, endDate) {
        startDate = Number(startDate);
        endDate = Number(endDate);
        
        var differenceInMilliseconds = endDate - startDate;
        return (differenceInMilliseconds / (1000 * 60 * 60 * 24 * 7)).roundTo(1);
    },
    
    edcFromAge: function(ageInWeeks, referenceDate) {
        if (isNaN(ageInWeeks)) return undefined;
        if (isNaN(referenceDate)) return undefined;
    
        ageInWeeks = Number(ageInWeeks);
        
        var daysLeft = 280 - (ageInWeeks * 7);
        var millisecondsLeft = this._daysToMilliseconds(daysLeft);
        
        return new Date(Number(referenceDate) + millisecondsLeft);
    },
    
    ageFromEdc: function(edc, referenceDate) {
        if (isNaN(edc)) return undefined;
        if (isNaN(referenceDate)) return undefined;

        return (40 - this.differenceInWeeks(referenceDate, edc)).roundTo(1);
    }
}