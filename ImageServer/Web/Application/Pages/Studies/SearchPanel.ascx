<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel" %>

<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>

<ccAsp:JQuery ID="JQuery1" runat="server" MultiSelect="true" />

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>

<script type="text/Javascript">

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);

function MultiSelect() {
    $("#<%=ModalityListBox.ClientID %>").multiSelect({
        noneSelected: '',
        oneOrMoreSelected: '% Selected',
        style: 'width: 120px;'
    });
}
</script>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom" >                    
                       <table cellpadding="0" cellspacing="0"  width="100%">
                            <tr>
                                <td align="left">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                            <tr>

                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Patient Name" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Name" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Patient ID" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Id" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Accession#" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="AccessionNumber" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Accession Number" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="Study Date" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearStudyDateButton" runat="server" Text="X" CssClass="SmallLink"/><br />
                                                <ccUI:TextBox ID="StudyDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="Search the list by Study Date" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Description" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="StudyDescription" runat="server"  CssClass="SearchTextBox" ToolTip="Search the list by Study Description" />
                                            </td>                                            
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="Modality" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="ModalityListBox" SelectionMode="Multiple">
                                                    <asp:ListItem Value="CR">CR</asp:ListItem>
                                                    <asp:ListItem Value="CT">CT</asp:ListItem>
                                                    <asp:ListItem Value="DX">DX</asp:ListItem>
                                                    <asp:ListItem Value="ES">ES</asp:ListItem>
                                                    <asp:ListItem Value="MG">MG</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="MR">MR</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="NM">NM</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="OT">OT</asp:ListItem>
                                                    <asp:ListItem Value="PT">PT</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="RF">RF</asp:ListItem>                                                                                                                                                            
                                                    <asp:ListItem Value="SC">SC</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="US">US</asp:ListItem>                                                                                                        
                                                    <asp:ListItem Value="XA">XA</asp:ListItem>                                                                                                                                                            
                                                </asp:ListBox>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click" /></asp:Panel>
                                            </td>
                                            </tr>
                                            </table>
                                        </td>
                                         <td align="right" valign="bottom">

                                         </td>
                                     </tr>
                                    </table>
                                </asp:Panel>
                                </td>
                            </tr>
                        </table>

                        <ccUI:CalendarExtender ID="StudyDateCalendarExtender" runat="server" TargetControlID="StudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="0" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="ViewStudyDetailsButton" runat="server" SkinID="ViewDetailsButton" />
                                        <ccUI:ToolbarButton ID="MoveStudyButton" runat="server" SkinID="MoveButton" />
                                        <ccUI:ToolbarButton ID="DeleteStudyButton" runat="server" SkinID="DeleteButton" OnClick="DeleteStudyButton_Click" />
                                        <ccUI:ToolbarButton ID="RestoreStudyButton" runat="server" SkinID="RestoreButton" OnClick="RestoreStudyButton_Click" />
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
                                <tr><td style="border-top: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerBottom" runat="server" /></td></tr>                    
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

