<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="DeviceGridViewControl" Codebehind="DeviceGridViewControl.ascx.cs" %>
<%@ Register Src="DeviceFilterControl.ascx" TagName="DeviceFilterControl" TagPrefix="uc2" %>
<%@ Register Src="AddDeviceControl.ascx" TagName="AddDeviceControl" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%"
                AllowPaging="True" OnRowDataBound="GridView1_RowDataBound" 
                OnDataBound="GridView1_DataBound" OnPreRender="GridView1_PreRender" 
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                OnSelectedIndexChanging="GridView1_SelectedIndexChanging" EmptyDataText="No devices available"
                BorderColor="LightSteelBlue" BorderStyle="Outset"
                BorderWidth="2px" OnPageIndexChanging="GridView1_PageIndexChanging" style="position: relative; top: 0px;" PageSize="15">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="AETitle" HeaderText="AE Title" >
                        <HeaderStyle Width="300px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Description" >
                        <HeaderStyle Width="300px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="IPAddress" HeaderText="IP Address" >
                        <HeaderStyle Width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Port" HeaderText="Port" />
                    <asp:TemplateField HeaderText="Active">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Active") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" ImageUrl="~/images/tick.gif"
                                Style="position: relative; left: 0px;" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DHCP">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <HeaderStyle Width="80px" />
                        <ItemTemplate>
                            <asp:Image ID="DHCPImage" runat="server" ImageUrl="~/images/tick.gif"
                                Style="position: relative" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Server Partition">
                        <ItemTemplate>
                            <asp:Label ID="ServerParitionLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="300px" />
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="Solid" BorderWidth="1px" />
                <EditRowStyle BackColor="#999999" Wrap="True" BorderColor="Yellow" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" BorderColor="#FFFF80"
                    BorderStyle="Solid" BorderWidth="5px" />
                <PagerStyle BackColor="#0099FF" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle Font-Bold="True" CssClass="WindowTitleBar" Height="20px" />
                <AlternatingRowStyle ForeColor="#284775" BackColor="White" />
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="width: 309px">
                                <asp:Label ID="NumDeviceLabel" runat="server" Text="# device(s)"></asp:Label></td>
                            <td style="width: 165px">
                            </td>
                            <td align="right">
                                &nbsp;<asp:LinkButton ID="PrevButton" runat="server" CommandArgument="Prev" CommandName="Page"
                                    Font-Underline="False">Prev</asp:LinkButton>
                                <asp:Label ID="PageLabel" runat="server" Text="Label"></asp:Label>&nbsp;
                                <asp:LinkButton ID="NextButton" runat="server" CommandArgument="Next" CommandName="Page"
                                    Font-Underline="False">Next</asp:LinkButton></td>
                        </tr>
                    </table>
                </PagerTemplate>
            </asp:GridView>
