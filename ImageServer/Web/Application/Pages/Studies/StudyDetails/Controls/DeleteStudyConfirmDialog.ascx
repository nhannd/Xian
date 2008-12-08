
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteStudyConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DeleteStudyConfirmDialog" %>
    
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, DeleteStudyConfirmDialogTitle %>">
    <ContentTemplate>
        <div class="ContentPanel">
        <div class="DialogPanelContent">
        <table border="0" cellspacing="5">
            <tr>
                <td>
                    <table>
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
                                                <th>
                                                    Patient's Name</th>
                                                <th>
                                                    Patient Id</th>
                                                <th>
                                                    Study Date</th>
                                                <th>
                                                    Study Description</th>
                                                <th>
                                                    Accession #</th>
                                                <th>
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
                                        </td>
                                        <td>
                                            <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">                                
                                    
                                    <tr>
                                        
                                        <td>
                                                <asp:TextBox  Width="100%" Rows="3"
                                                    ID="Reason" runat="server" TextMode="MultiLine" ValidationGroup="<%= ModalDialog.ClientID %>" />
                                            
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
                </td>
            </tr>
        </table>
        </div>
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="YesButton" ValidationGroup="<%= ModalDialog.ClientID %>"
                            OnClick="DeleteButton_Clicked" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton"
                            OnClick="CancelButton_Clicked" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </div>
       
       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="Reason" InvalidInputIndicatorID="InvalidReasonIndicator" ValidationGroup="<%= ModalDialog.ClientID %>"
                                                Text="You must specify the reason for deleting the studies" Display="None" InvalidInputColor="#FAFFB5"></ccValidator:ConditionalRequiredFieldValidator>
        
    </ContentTemplate>
</ccAsp:ModalDialog>
