<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="Cross.Modules.ChildPageList.List" %>
<asp:DataList ID="dlSubTabList" runat="server" RepeatColumns="<%#ColumnPerRow%>" >
    <ItemTemplate >
        &nbsp;&nbsp;
        <asp:HyperLink ID="hypTab" runat="server" Target="<%#LinkTarget%>"   ToolTip='<%# Convert.ToString(Eval("Description")) %>'
            Text='<%# Server.HtmlDecode(Convert.ToString(Eval("TabName"))) %>' NavigateUrl='<%#DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(Eval("TabID"))) %>' />
    </ItemTemplate>
    <ItemStyle Wrap="false" />
</asp:DataList>
