<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>

<%@ Import Namespace="ClearCanvas.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteStudyConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DeleteStudyConfirmDialog" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, DeleteStudyConfirmDialogTitle %>" Width="800px">
    <ContentTemplate>
    
        <script language="javascript" type="text/javascript">
            Sys.Application.add_load(page_load);
            function page_load()
            {            
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                if (document.all) //IE6
                {
                    listbox.attachEvent('onchange', studyReasonSelectionChanged);
                }
                else //Firefox
                {
                    listbox.addEventListener('onchange', studyReasonSelectionChanged, false);
                }
            }
            
            function studyReasonSelectionChanged()
            {
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                var textbox = $get('<%= Reason.ClientID %>');
                textbox.value = listbox.options[listbox.selectedIndex].value;
                
            }
        </script>
    
        <div class="ContentPanel">
        <div class="DialogPanelContent">
            <div id="StudyList">
                <table border="0" cellspacing="5" width="100%">
                    <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel" 
                                                    Text='<%$ Resources:Labels, DeleteStudyConfirmDialog_StudyListingLabel %>'></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="DeleteStudiesTableContainer" style="background: white">
                                    <asp:Repeater runat="server" ID="StudyListing">
                                        <HeaderTemplate>
                                            <table  cellspacing="0" width="100%" class="DeleteStudiesConfirmTable">
                                                <tr>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Patient's Name</th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Patient Id</th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Study Date</th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Study Description</th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Accession #</th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader">
                                                        Modality</th>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr class="GlobalGridViewRow">
                                                <td>
                                                    <%# Eval("PatientsName") %>
                                                </td>
                                                <td>
                                                    <%# Eval("PatientId") %>
                                                </td>
                                                <td>
                                                    <%# Eval("StudyDate") %>
                                                </td>
                                                <td>
                                                    <%# Eval("StudyDescription") %>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "AccessionNumber", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# Eval("Modalities")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                </table>
            </div>
            <div id="ReasonPanel">
                <table border="0">
                   <tr valign="top">
                        <td>
                            <asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel" Text="Reason:"></asp:Label>                            
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ReasonListBox" style="font-family: Arial, Sans-Serif; font-size: 14px;"/>                                        
                        </td>
                   </tr>
                   <tr>
                        <td valign="top">
                            <asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel" 
                                            Text='Comment:'></asp:Label> 
                             
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                                        <asp:TextBox  Width="400px" Rows="3" ID="Reason" runat="server" TextMode="MultiLine" style="font-family: Arial, Sans-Serif; font-size: 14px;" />                                            
                                    </td>
                                    <td>
                                        <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="ReasonSavePanel" runat="server">
                        <td>
                            <asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel" 
                                                Text="Save reason as:"></asp:Label> 
                                 
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="SaveReasonAsName" style="font-family: Arial, Sans-Serif; font-size: 14px;"/>
                        </td>
                </tr>
                </table>
            </div>                
        </div>
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="OKButton" 
                            OnClick="DeleteButton_Clicked" ValidationGroup='StudyGroup'/>
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton"
                            OnClick="CancelButton_Clicked" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </div>
       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="Reason" InvalidInputIndicatorID="InvalidReasonIndicator" 
                                                ValidationGroup='StudyGroup'
                                                Text="You must specify the reason for deleting the studies for future auditing purposes." Display="None" InvalidInputColor="#FAFFB5"></ccValidator:ConditionalRequiredFieldValidator>
           
    </ContentTemplate>
</ccAsp:ModalDialog>
