
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteStudyConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DeleteStudyConfirmDialog" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, DeleteStudyConfirmDialogTitle %>" Width="800px">
    <ContentTemplate>
    
        <script language="javascript" type="text/javascript">
            Sys.Application.add_load(page_load);
            function page_load()
            {            
                $get('<%= ReasonSavePanel.ClientID %>').style.display = 'none';
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                if (document.all) //IE6
                {
                    listbox.attachEvent('onchange', reasonSelectionChanged);
                }
                else //Firefox
                {
                    listbox.addEventListener('onchange', reasonSelectionChanged, false);
                }
            }
            
            function reasonSelectionChanged()
            {
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                var textbox = $get('<%= Reason.ClientID %>');
                textbox.value = listbox.options[listbox.selectedIndex].value;
                if (listbox.selectedIndex==listbox.options.length-1)
                {
                    textbox.select();
                    $get('<%= ReasonSavePanel.ClientID %>').style.display = ''; // show
                }
                else
                {
                    $get('<%= ReasonSavePanel.ClientID %>').style.display = 'none';                
                }
            }
        </script>
    
        <div class="ContentPanel">
        <div class="DialogPanelContent">
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
                                <asp:Repeater runat="server" ID="StudyListing">
                                    <HeaderTemplate>
                                        <table  cellspacing="0" width="100%" class="GlobalGridView"
                                            style="border: solid 1px #3d98d1;">
                                            <tr class="GlobalGridViewHeader">
                                                <th style="white-space:nowrap">
                                                    Patient's Name</th>
                                                <th style="white-space:nowrap">
                                                    Patient Id</th>
                                                <th style="white-space:nowrap">
                                                    Study Date</th>
                                                <th style="white-space:nowrap">
                                                    Study Description</th>
                                                <th style="white-space:nowrap">
                                                    Accession #</th>
                                                <th style="white-space:nowrap">
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
                                                <%# Eval("AccessionNumber") %>
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
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server" CssClass="DialogTextBoxLabel" 
                                                Text='<%$ Resources:Labels, DeleteStudyConfirmReasonLabel %>'></asp:Label> 
                                            <asp:DropDownList runat="server" ID="ReasonListBox" />
                                        </td>
                                        
                                        <td>
                                            <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">
                                    <tr>                                        
                                        <td>
                                            <asp:TextBox  Width="100%" Rows="3" ID="Reason" runat="server" TextMode="MultiLine"   />                                            
                                        </td>
                                    </tr>
                                    </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel runat="server" ID="ReasonSavePanel">
                    Save this reason as <asp:TextBox runat="server" ID="SaveReasonAsName" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </div>
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="OKButton" 
                            ValidationGroup="<%= ClientID %>"
                            OnClick="DeleteButton_Clicked" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton"
                            OnClick="CancelButton_Clicked" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </div>
       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="Reason" InvalidInputIndicatorID="InvalidReasonIndicator" 
                                                ValidationGroup="<%= ClientID %>"
                                                Text="You must specify the reason for deleting the studies for future auditing purpose." Display="None" InvalidInputColor="#FAFFB5"></ccValidator:ConditionalRequiredFieldValidator>
       
       
    </ContentTemplate>
</ccAsp:ModalDialog>
