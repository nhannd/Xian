/*
	This file contains a server-side Oto script that timeshifts all order in a RIS database to the present day.
*/
import ClearCanvas.Healthcare;

var store;	// persistent store

function main()
{
	// init Healthcare store
	initStore();
	
	var today = DateTime.Today;
	LogInfo("Current date is " + today);
	processInputSequence(
		{
			inputSequence: createInputSequence(store.GetQuery("Order")),
			processFunc: function(item)
			{
				return processOrder(item, today);
			},
			dryRun: false
		}
	);
}

function processOrder(order, today)
{
	LogInfo("Processing order A# " + order.AccessionNumber);
	

	var schedDate = order.ScheduledStartTime ? order.ScheduledStartTime.Date : 
		(order.SchedulingRequestTime ? order.SchedulingRequestTime.Date : null);
	if(schedDate == null)
	{
		LogError("Order has no scheduled start time or scheduling request time");
		return;
	}
	
	var shift = today - schedDate;
	
	LogInfo("Shifting order by " + shift.TotalDays + " days");

	order.TimeShift(shift.TotalMinutes);
}

/* Creates an object that represents a sequence of input items for processing
	queryProvider - an EntityQuery object.
		
	returns - an object with a .next() method that can be called successively to return the next item.  The sequence
		terminates when next() returns null.
*/
function createInputSequence(query)
{
	var memo = null;
	return {
		next : function()
		{
			query.OrderBy.OID.SortAsc(0);
			if(memo)
				query.Where.OID.MoreThan(memo);
			
			try
			{
				var item = query.First();
				memo = item.OID;
				return item;
			}
			catch(e)
			{
				// query returned no results - end of sequence
				return null;
			}
		}
	};
}


/*	Processes an input sequence created by the createInputSequence() function.
		args - an object with the following properties:
			inputSequence - an input sequence
			processFunc - a callback that is passed each item for processing. Must have the form
				function process(item)
				{
					// process the item
					return rows;
				}
				
				The function may optionally return an array where each element is an array representing a processing result.
				These rows will be written to the output file in CSV format.
			outputFileName - the path to the file to which the CSV output should be written.
			dryRun - optional boolean flag - if true, indicates that no changes should be written to the DB - useful for developement
*/
function processInputSequence(args)
{
	// create an output file where the anonymized relationships will be logged in CSV format
	var out = (args.outputFileName) ? File.CreateText(args.outputFileName) : null;
	
	try
	{
		for(var more = true; more && !CancelRequested; )
		{
			// if dryRun is true, use a read -scope so we don't actually modify the DB
			var persistenceScopeFunc = args.dryRun ? store.Read : store.Update;
		
			persistenceScopeFunc(
				function()
				{
					var item = args.inputSequence.next();
					if(!item)
					{
						more = false;
						return;
					}
					
					
					// process the item
					var rows = args.processFunc(item);
					
					// write the rows to the output file
					if(out && rows) 
						writeToStream(rows, out);
				});
		}
	}
	finally
	{
		if(out)
		{
			out.Flush();
			out.Dispose();
		}
	}
}

/*	Writes the rows array to the specified output stream.
		rows - an array of arrays, where each element array is a line in the CSV file
		out - a .NET StreamWriter object
*/
function writeToStream(rows, out)
{
	rows.map(function(row) { return row.join(","); }).each(
			function(line)
			{
				LogInfo(line);
				out.WriteLine(line);
			});
}



/*	Initializes the Healthcare persistent store.
*/
function initStore()
{
	// create the model
	var storeService = CreateService("PersistentStoreService");
	var response = storeService.GetStore({ StoreName: "Healthcare"});
	store = response.Store;
}
