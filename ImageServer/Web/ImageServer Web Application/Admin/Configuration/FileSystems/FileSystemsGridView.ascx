<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.FileSystems.FileSystemsGridView" Codebehind="FileSystemsGridView.ascx.cs" %>
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
                    <asp:BoundField DataField="Description" HeaderText="Description" >
                        
                    </asp:BoundField>
                    
                    <asp:TemplateField HeaderText="Read">
                        <ItemTemplate>
                            <asp:Image ID="ReadImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Write">
                       <ItemTemplate>
                            <asp:Image ID="WriteImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>       
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tier">
                        <ItemTemplate>
                            <asp:Label ID="FilesystemTierDescription" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="FileSystemPath" HeaderText="Path" >
                    </asp:BoundField>
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
