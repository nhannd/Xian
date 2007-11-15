<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.Devices.DeviceGridView" Codebehind="DeviceGridView.ascx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Panel ID="Panel1" runat="server"  >

<asp:Panel ID="Panel2" runat="server" BorderColor="Silver" BorderWidth="1px" Height="400px">
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                ForeColor="#333333"
                OnRowDataBound="GridView1_RowDataBound" 
                OnDataBound="GridView1_DataBound"
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                OnSelectedIndexChanging="GridView1_SelectedIndexChanging" 
                EmptyDataText="No devices available (Please check the filter settings!)" 
                OnPageIndexChanging="GridView1_PageIndexChanging" 
                PageSize="15" 
                AllowSorting="True" width="100%" CellPadding="2" AllowPaging="True" BackColor="#E0E0E0" CaptionAlign="Top" BorderWidth="1px"
                 >
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="AETitle" HeaderText="AE Title" >
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Description" >
                        
                    </asp:BoundField>
                    <asp:BoundField DataField="IPAddress" HeaderText="IP Address" >
                        
                    </asp:BoundField>
                    <asp:BoundField DataField="Port" HeaderText="Port">
                        
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Active">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Active") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" ImageUrl="~/images/unchecked_small.gif"/>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DHCP">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DHCPImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Partition">
                        <ItemTemplate>
                            <asp:Label ID="ServerParitionLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Features">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="FeaturePlaceHolder" runat="server"></asp:PlaceHolder>
                        </ItemTemplate>
                        
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="GridRowStyle"/>
                <EditRowStyle BackColor="#2461BF" Wrap="True" BorderColor="Yellow" />
                <SelectedRowStyle CssClass="GridSelectedRowStyle" />
                <PagerStyle BackColor="Silver" HorizontalAlign="Center" CssClass="TableHeaderRow" />
                <HeaderStyle CssClass="GridHeader" />
                
    <PagerTemplate>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Label ID="PagerDeviceCountLabel" runat="server" Text="Label"></asp:Label></td>
                <td>
                    <asp:Label ID="PagerPagingLabel" runat="server" Text="Label"></asp:Label></td>
                <td align="right">
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:ImageButton ID="PagerPrevImageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                                    ImageUrl="~/images/prev.gif" OnCommand="ImageButton_Command" />                    
                            </td>
                            <td>
                                <asp:ImageButton ID="PagerNextImageButton" runat="server" CommandArgument="Next" CommandName="Page"
                                     ImageUrl="~/images/next.gif" OnCommand="ImageButton_Command" /></td>
                          
                        </tr>
                        
                    </table>
                    
            </tr>
        </table>
    </PagerTemplate>
                
            </asp:GridView>
</asp:Panel>
</asp:Panel>
