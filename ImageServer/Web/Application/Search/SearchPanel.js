Sys.Application.add_load( function(){
        studylistgrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
        if (studylistgrid!=null)
        {
            studylistgrid.add_onClientRowClick(OnStudyListRowSelected);
        }
    }
);

function OnStudyListRowSelected(sender, ev)
{
    studylistgrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
    
    if (studylistgrid!=null)
    {
        deletebutton = $find('@@DELETE_BUTTON_CLIENTID@@');
    
        rows = studylistgrid.getSelectedRowElements();
        if (rows.length>0)
        {
            enabledelete = true;
            for(i=0; i<rows.length; i++)
            {
                row = rows[i];
                deleted = row.getAttribute('deleted')=='true';
                if (deleted)
                {
                    enabledelete = false;
                    break;
                }
            }
            
            deletebutton.set_enable(enabledelete);
        }
        else
        {
            deletebutton.set_enable(false);
        }
        
        
        openbutton = $find('@@OPEN_STUDY_BUTTON_CLIENTID@@');
        openbutton.set_enable(rows.length>0);
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
    studylistgrid = $find('@@STUDYLIST_GRIDVIEW_CLIENTID@@');
    if (studylistgrid!=null)
    {
        var rows = studylistgrid.getSelectedRowElements();
        for(i=0; i<rows.length; i++)
        {
            var url = '@@OPEN_STUDY_BASE_URL@@?siuid=' + rows[i].getAttribute('instanceuid') +'&serverae=@@PARTITION_AE@@';
            window.open(url);
        }
    }
    
}
