/// Register onload event listener
Sys.Application.add_load( function()
{
        studyListGrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
        if (studyListGrid!=null)
        {
            studyListGrid.add_onClientRowClick(OnStudyListRowSelected);
        }
    }
);


function OnStudyListRowSelected(sender, ev)
{
    var studyListGrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
    
    if (studyListGrid!=null)
    {
        // enable the delete study button if the selected study hasn't been scheduled for delete
        // If multiple studies are selected, only enable the button if none of them has been scheduled for delete.
        
        var deleteButton = $find('@@DELETE_BUTTON_CLIENTID@@');
    
        var rows = studyListGrid.getSelectedRowElements();
        if (rows.length>0)
        {
            var allowDelete = true;
            for(i=0; i<rows.length; i++)
            {
                var row = rows[i];
                deleted = row.getAttribute('deleted')=='true'; //'deleted' is a custom attribute injected by the server control
                if (deleted)
                {
                    allowDelete = false;
                    break;
                }
            }
            
            deleteButton.set_enable(allowDelete);
        }
        else
        {
            deleteButton.set_enable(false);
        }
        
        
        openButton = $find('@@OPEN_STUDY_BUTTON_CLIENTID@@');
        openButton.set_enable(rows.length>0);
    }
}

function OnStudyListRowDoubleClicked(sender, ev)
{
    if (ev.row.getAttribute('selected')=='true')
    {
        var url = '@@OPEN_STUDY_BASE_URL@@?siuid=' + ev.row.getAttribute('instanceuid') +'&serverae=@@PARTITION_AE@@';
        window.open(url);
    }
     
}

function OpenSelectedStudies()
{
    studyListGrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
    if (studyListGrid!=null)
    {
        var rows = studyListGrid.getSelectedRowElements();
        for(i=0; i<rows.length; i++)
        {
            var url = '@@OPEN_STUDY_BASE_URL@@?siuid=' + rows[i].getAttribute('instanceuid') +'&serverae=@@PARTITION_AE@@';
            window.open(url);
        }
    }
    
}
