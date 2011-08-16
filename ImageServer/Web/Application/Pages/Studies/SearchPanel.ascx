<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel" %>

<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>



<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>

<script type="text/Javascript">

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

function MultiSelect() {

        $("#<%=ModalityListBox.ClientID %>").multiSelect({
            selectAllText: "<%= SR.All %>",
            noneSelected: '',
            oneOrMoreSelected: '*',
            dropdownStyle: 'width: 90px;',
            textboxStyle: 'width: 75px;'
        });   
        
        $("#<%=StatusListBox.ClientID %>").multiSelect({
            noneSelected: '',
            oneOrMoreSelected: '*',
            //dropdownStyle: 'width: 200px;',
            textboxStyle: 'width: 90px;'            
        });   

}

</script>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom" >                    
                       <table cellpadding="0" cellspacing="0"  width="100%">
                            <tr>
                                <td align="left">
                                <asp:Panel ID="Panel6" runat="server" style="border-bottom: solid 1px #3D98D1; padding: 6px 4px 6px 4px;" DefaultButton="SearchButton">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                        <td>
                                            <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>   
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels,PatientName %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientName %>" style="width: 95px"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, PatientID%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientID%>" style="width: 95px"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, AccessionNumber%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="AccessionNumber" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByAccessionNumber%>" style="width: 95px"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels,FromDate %>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearFromStudyDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 0px;"/><br />
                                                <ccUI:TextBox ID="FromStudyDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate%>" style="width: 95px" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels,ToDate %>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearToStudyDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 0px;"/><br />
                                                <ccUI:TextBox ID="ToStudyDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate%>" style="width: 95px" />                                                
                                            </td>                                                                                     
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels,Description%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="StudyDescription" runat="server"  CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByDescription%>" style="width: 95px"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label9" runat="server" Text="<%$Resources: SearchFieldLabels,ReferringPhysician%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="ReferringPhysiciansName" runat="server"  CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByRefPhysician%>" style="width: 95px"/>
                                            </td>                                                                                                  
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels,Modality%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="ModalityListBox" SelectionMode="Multiple">
                                                    <asp:ListItem Value="CR">CR</asp:ListItem>
                                                    <asp:ListItem Value="CT">CT</asp:ListItem>
                                                    <asp:ListItem Value="DX">DX</asp:ListItem>
                                                    <asp:ListItem Value="ES">ES</asp:ListItem>
                                                    <asp:ListItem Value="KO">KO</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="MG">MG</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="MR">MR</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="NM">NM</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="OT">OT</asp:ListItem>
                                                    <asp:ListItem Value="PR">PR</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="PT">PT</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="RF">RF</asp:ListItem>                                                                                                                                                            
                                                    <asp:ListItem Value="SC">SC</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="US">US</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="XA">XA</asp:ListItem>                                                                                                                                                            
                                                </asp:ListBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label8" runat="server" Text="<%$Resources: SearchFieldLabels,StudyStatus%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="StatusListBox" SelectionMode="Multiple">                                       
                                                </asp:ListBox>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" /></asp:Panel>
                                            </td>
                                            </tr>                                          
                                            </table>
                                        </td>
                                     </tr>
                                    </table>
                                </asp:Panel>
                                </td>
                            </tr>
                        </table>

                        <ccUI:CalendarExtender ID="FromStudyDateCalendarExtender" runat="server" TargetControlID="FromStudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                        <ccUI:CalendarExtender ID="ToStudyDateCalendarExtender" runat="server" TargetControlID="ToStudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="ViewImagesButton" runat="server" SkinID="<%$Image:ViewImagesButton%>" />
                                        <ccUI:ToolbarButton ID="ViewStudyDetailsButton" runat="server" SkinID="<%$Image:ViewDetailsButton%>" />
                                        <ccUI:ToolbarButton ID="MoveStudyButton" runat="server" SkinID="<%$Image:MoveButton%>" />
                                        <ccUI:ToolbarButton ID="DeleteStudyButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="DeleteStudyButton_Click" />
                                        <ccUI:ToolbarButton ID="RestoreStudyButton" runat="server" SkinID="<%$Image:RestoreButton%>" OnClick="RestoreStudyButton_Click" />
                                        <ccUI:ToolbarButton ID="AssignAuthorityGroupsButton" runat="server" SkinID="<%$Image:AddDataAccessButton%>" OnClick="AssignAuthorityGroupsButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;">
                                <localAsp:StudyListGridView id="StudyListGridView" runat="server" Height="500px"></localAsp:StudyListGridView></td></tr>
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
    </Triggers>

</asp:UpdatePanel>

<ccAsp:MessageBox ID="MessageBox" runat="server" />
<ccAsp:MessageBox ID="RestoreMessageBox" runat="server" />   
<ccAsp:MessageBox ID="ConfirmStudySearchMessageBox" runat="server" MessageType="YESNO" />

