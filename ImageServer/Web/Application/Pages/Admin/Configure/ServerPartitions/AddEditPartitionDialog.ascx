<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>
    
<script type="text/javascript">

</script>

<ccAsp:ModalDialog ID="ModalDialog" runat="server">
    <ContentTemplate>
            <asp:ValidationSummary ID="EditPartitionValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="vg1" CssClass="EditStudyDialogErrorMessage" />   			
        <asp:Panel ID="Panel3" runat="server" DefaultButton="OKButton">
            <aspAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
                <aspAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel" CssClass="DialogTabControl">
                    <ContentTemplate>
                            <table id="GeneralTabTable" runat="server">
                                <tr id="Tr1" runat="server" align="left">
                                    <td id="Td1" runat="server">
                                        <table width="100%">
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="AE Title" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="AETitleTextBox" runat="server" MaxLength="16" ValidationGroup="vg1" CssClass="DialogTextBox"
                                                        ToolTip="The DICOM Application Entity Title for the partition."></asp:TextBox>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="AETitleHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                    <ccValidator:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="true" Text="AE Title is required"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="AETitleHelp" />
                                                    <ccValidator:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator2"
                                                        runat="server" ControlToValidate="AETitleTextBox" Display="None" Text="The AE Title is not valid."
                                                        InvalidInputColor="#FAFFB5" ValidationExpression="^([^\\]){1,16}$" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="AETitleHelp" />
                                                    <ccValidator:ServerPartitionValidator ID="ServerPartitionValidator" runat="server"
                                                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="false"
                                                        Text="The AE Title is not valid." InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="AETitleHelp" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td2" runat="server" align="left">
                                        <table width="100%">
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Description" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="DescriptionTextBox" runat="server" ToolTip="A textual description of the partition." CssClass="DialogTextBox"></asp:TextBox>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr id="Tr2" runat="server" align="left">
                                    <td id="Td3" runat="server">
                                        <table>
                                            <tr align="left">
                                                <td >
                                                    <asp:Label ID="Label2" runat="server" Text="Port" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="PortTextBox" runat="server" CssClass="DialogTextBox"></asp:TextBox>
                                                    <ccValidator:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                        Text="Partition Port must be between 1 and 65535" Display="None" InvalidInputIndicatorID="PortHelp"></ccValidator:RangeValidator>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="PortHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td4" runat="server">
                                        <table>
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="Folder Name" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="PartitionFolderTextBox" runat="server" CausesValidation="true" ValidationGroup="vg1"
                                                        ToolTip="A unique folder name to store images within for the partition." CssClass="DialogTextBox"/>
                                                    <ccValidator:ServerPartitionFolderValidator ID="PartitionFolderValidator"
                                                        runat="server" ControlToValidate="PartitionFolderTextBox" Display="None" EnableClientScript="false"
                                                        Text="Folder Name is not valid" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="FolderHelp"/>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="FolderHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="Tr3" runat="server" align="left">
                                    <td id="Td5" runat="server">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="True" Text="Enabled" ToolTip="Enable or Disable DICOM connections to the partition." CssClass="DialogCheckBox" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td6" runat="server" valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="DuplicateSopLabel" runat="server" Text="Duplicate Object Policy" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:DropDownList ID="DuplicateSopDropDownList" runat="server" CssClass="DialogDropDownList" ToolTip="A policy for dealing with duplication DICOM objects received by the partition." />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                    </ContentTemplate>
                    <HeaderTemplate>
                        General
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" CssClass="DialogTabPanelContent" >
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="AcceptAnyDeviceCheckBox" runat="server" Text="Accept Any Device" CssClass="DialogCheckBox"
                                                        ToolTip="Accept DICOM Associations from any device to this partition." />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="AutoInsertDeviceCheckBox" runat="server" Text="Auto Insert Devices" CssClass="DialogCheckBox"
                                                        ToolTip="Automatically add devices when they connect to this partition." />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label5" runat="server" Text="Default Remote Port" CssClass="DialogTextBoxLabel" /><asp:TextBox ID="DefaultRemotePortTextBox" CssClass="DialogTextBox" runat="server"></asp:TextBox>
                                                    <td valign="bottom">
                                                        <ccAsp:InvalidInputIndicator ID="DefaultPortHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                        <ccValidator:RangeValidator ID="DefaultRemotePortRangeValidator" runat="server"
                                                            ControlToValidate="DefaultRemotePortTextBox" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                            MinValue="1" MaxValue="65535" Text="Remote device default port must be between 1 and 65535"
                                                            Display="None" InvalidInputIndicatorID="DefaultPortHelp" />
                                                    </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <HeaderTemplate>
                        Remote Devices
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                 <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="GeneralTabPanel" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" CssClass="DialogTabPanelContent" >
                            <div class="DialogMessagePanel"  style="width: 460px;">Incoming studies will be placed in the Study Integrity Queue if the Study Instance UID matches an existing study and the data differs from any of the selected criteria below.</div>
                            
                            <table width="100%">
                            <tr>
                                <td><asp:CheckBox ID="MatchPatientName" runat="server" Text="Patient Name" CssClass="DialogCheckBox"/></td>
                                <td><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="20px" Height="1px"/></td>                                                        
                                <td><asp:CheckBox ID="MatchPatientID" runat="server" Text="Patient ID" CssClass="DialogCheckBox" /></td>

                            </tr>
                            <tr>
                                <td><asp:CheckBox ID="MatchPatientBirthDate" runat="server" Text="Patient Birth Date" CssClass="DialogCheckBox"/></td>
                                <td></td>                                                        
                                <td><asp:CheckBox ID="MatchPatientSex" runat="server" Text="Patient Sex" CssClass="DialogCheckBox"/></td>
                                                                                        
                            </tr>
                            <tr>
                                <td><asp:CheckBox ID="MatchAccessionNumber" runat="server" Text="Accession Number" CssClass="DialogCheckBox"/></td>
                                <td></td>                                                        
                                <td><asp:CheckBox ID="MatchIssuer" runat="server" Text="Issuer of Patient ID" CssClass="DialogCheckBox"/></td>
                            </tr>
                        </table>
                        </asp:Panel>
                    
                    </ContentTemplate>
                    <HeaderTemplate>
                        Study Matching
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel3" runat="server" HeaderText="<%$ Resources:Titles, DeleteManagement %>" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel5" runat="server" CssClass="DialogTabPanelContent" >
                            <table width="100%">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="AuditDeleteStudyCheckBox" runat="server" Text="Study Deletion" CssClass="DialogCheckBox" 
                                            ToolTip="<%$ Resources:Tooltips, ServerPartitionAddEditDialog_AuditDeleteStudy %>"/>
                                </td>
                            </tr>
                        </table>
                        </asp:Panel>
                    
                    </ContentTemplate>
                    
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
        </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" OnClick="OKButton_Click" ValidationGroup="vg1" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>                                                        
    </ContentTemplate>
</ccAsp:ModalDialog>
