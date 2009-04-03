<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ApplicationLogPanel.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog.ApplicationLogSearchPanel" %>
<%@ Register Src="ApplicationLogGridView.ascx" TagName="ApplicationLogGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Table ID="Table1" runat="server" Width="100%">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
					    <ccAsp:JQuery runat="server" ID="JQuery1" MultiSelect="false" Effects="false" MaskedInput="true" />
		 
						<script type="text/javascript">
							jQuery(function($) {
								$.mask.definitions[';'] = '[01]';
								$("#<%=FromFilter.ClientID %>").mask("9999-99-99? 99:99:99.999");
								$("#<%=ToFilter.ClientID %>").mask("9999-99-99? 99:99:99.999");
							});</script>

						<table width="100%" cellpadding="0" cellspacing="0">
							<tr>
								<td>
									<table cellpadding="0" cellspacing="0">
										<tr>
											<td align="left">
												<asp:Label ID="Label2" runat="server" Text="Host" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="HostFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search for a hostname"></asp:TextBox></td>
											<td align="left">
												<asp:Label ID="Label4" runat="server" Text="From" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="FromFilter" runat="server" CssClass="SearchDateTimeTextBox" ToolTip="From Date/Time (YYYY-MM-DD HH:MM:SS.FFF)"></asp:TextBox></td>
									
											<td align="left">
												<asp:Label ID="Label5" runat="server" Text="To" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="ToFilter" runat="server" CssClass="SearchDateTimeTextBox" ToolTip="To Date/Time (YYYY-MM-DD HH:MM:SS.FFF)"></asp:TextBox></td>
											<td align="left">
												<asp:Label ID="Label3" runat="server" Text="Thread" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="ThreadFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search for a thread"></asp:TextBox></td>
											<td align="left">
												<asp:Label ID="Label6" runat="server" Text="Log Level" CssClass="SearchTextBoxLabel"
													EnableViewState="False" /><br />
												<asp:DropDownList runat="server" ID="LogLevelListBox" CssClass="SearchDropDownList">
													<asp:ListItem Value="ANY">- Any -</asp:ListItem>
													<asp:ListItem Value="INFO">INFO</asp:ListItem>
													<asp:ListItem Value="ERROR">ERROR</asp:ListItem>
													<asp:ListItem Value="WARN">WARN</asp:ListItem>
												</asp:DropDownList>
											</td>
											<td align="left">
												<asp:Label ID="Label1" runat="server" Text="Log Message" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="MessageFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the log messages"></asp:TextBox></td>
											<td valign="bottom">
												<asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
													<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click" /></asp:Panel>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow Height="100%">
				<asp:TableCell>
					<table width="100%" cellpadding="0" cellspacing="0" >
						<tr>
							<td>
								<asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
									<ContentTemplate>
										<asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
										</asp:Panel>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
						<tr>
							<td>
								<asp:Panel ID="Panel2" runat="server" Style="border: solid 1px #3d98d1;">
									<table width="100%" cellpadding="0" cellspacing="0">
										<tr>
											<td style="border-bottom: solid 1px #3d98d1">
												<ccAsp:GridPager ID="GridPagerTop" runat="server" />
											</td>
										</tr>
										<tr>
											<td style="background-color: white;">
												<localAsp:ApplicationLogGridView ID="ApplicationLogGridView" runat="server"/>
											</td>
										</tr>
									</table>
								</asp:Panel>
							</td>
						</tr>
					</table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</ContentTemplate>
</asp:UpdatePanel>
