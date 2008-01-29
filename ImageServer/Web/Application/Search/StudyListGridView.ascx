<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudyListGridView"
    Codebehind="StudyListGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="CSSGridViewPanelContainer">
    <asp:Panel ID="Panel3" runat="server" CssClass="CSSGridViewPanelBorder">
        <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelContent">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                CssClass="CSSGridView"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" 
                OnDataBound="GridView1_DataBound"                
                EmptyDataText="" 
                CellPadding="0"
                PageSize="20" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="0px">
                <Columns>
                    <asp:BoundField DataField="PatientsName" HeaderText="Patient's Name"></asp:BoundField>
                    <asp:BoundField DataField="PatientID" HeaderText="Patient ID"></asp:BoundField>
                    <asp:BoundField DataField="AccessionNumber" HeaderText="Accession #"></asp:BoundField>
                    <asp:BoundField DataField="StudyDate" HeaderText="Study Date"></asp:BoundField>
                    <asp:BoundField DataField="StudyDescription" HeaderText="Description"></asp:BoundField>
                    <asp:BoundField DataField="NumberOfRelatedSeries" HeaderText="Series"></asp:BoundField>
                    <asp:BoundField DataField="NumberOfRelatedInstances" HeaderText="Instances"></asp:BoundField>
                    <%--Placeholder for the hover menu--%>
                </Columns>
                <RowStyle CssClass="CSSGridRowStyle" />
                <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
                <HeaderStyle CssClass="CSSGridHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </asp:GridView>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
