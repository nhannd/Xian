var EdcCalculator = {

    daysToMilliseconds: function(days) {
        if (isNaN(days)) throw {name:"Invalid parameter days", message:"days must be a number"};
        
        days = Number(days);
        return days * 24 * 60 * 60 * 1000;
    },
    
    edcFromLmp: function(lmp) {
        if (isNaN(lmp)) return undefined;

        return new Date(Number(lmp) + this.daysToMilliseconds(280));
    },
    
    differenceInWeeks: function(startDate, endDate) {
        startDate = Number(startDate);
        endDate = Number(endDate);
        
        var differenceInMilliseconds = endDate - startDate;
        return differenceInMilliseconds / (1000 * 60 * 60 * 24 * 7);
    },
    
    edcFromTodaysAge: function(ageInWeeks) {
        if (isNaN(ageInWeeks)) return undefined;
    
        ageInWeeks = Number(ageInWeeks);
        
        var daysLeft = 280 - (ageInWeeks * 7);
        var millisecondsLeft = this.daysToMilliseconds(daysLeft);
        var today = new Date();
        
        return new Date(Number(today) + millisecondsLeft);
    },
    
    ageFromEdc: function(edc) {
        if (isNaN(edc)) return undefined;

        var today = new Date();
        return 40 - this.differenceInWeeks(today, edc);
    }
}