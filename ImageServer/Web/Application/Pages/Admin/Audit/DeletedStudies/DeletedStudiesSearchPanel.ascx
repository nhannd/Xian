<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudiesSearchPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudiesSearchPanel" %>

<%@ Register Src="SearchResultGridView.ascx" TagName="SearchResultGridView" TagPrefix="localAsp" %>

<asp:ScriptManagerProxy runat="server" ID="ScriptManagerProxy" />

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
            <asp:Table ID="Table1" runat="server" BorderStyle="none">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom" >                    
                       <table cellpadding="0" cellspacing="0"  width="100%"  border="0" >
                            <tr>
                                <td align="left">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
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
                                                <asp:Label ID="Label6" runat="server" Text="Deleted By" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="DeletedBy" runat="server"  CssClass="SearchTextBox" ToolTip="Search the list by the person who deleted the study" />
                                            </td>                                            
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click" /></asp:Panel>
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

                        <ccUI:CalendarExtender ID="StudyDateCalendarExtender" runat="server" TargetControlID="StudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="ViewStudyDetailsButton" runat="server" SkinID="ViewDetailsButton" OnClick="ViewDetailsButtonClicked" />
                                        <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="DeleteButton" OnClick="DeleteButtonClicked" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server"  TargetControlID="SearchResultGridView1"/></td></tr>                        
                                <tr><td style="background-color: white;">
                                
                                    <localAsp:SearchResultGridView ID="SearchResultGridView1" runat="server" />
                                
                                </td></tr>
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
 



                                