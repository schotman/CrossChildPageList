<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Cross.Modules.ChildPageList.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table id="tblGerneral" cellspacing="0" cellpadding="2" border="0" runat="server" width="100%"
    class="dnnFormItem">
    <tr>
        <td  style="width: 200px">
            <dnn:Label ID="plParentTab" runat="server" Text="Parent Tab"></dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="ddlParentTab" DataValueField="TabId" DataTextField="TabName"
                 Width="600px" runat="server" >
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plIncludeSelf" runat="server" Suffix=":" Text="Include Self" ControlName="chkIncludeSelf">
            </dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkIncludeSelf" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plIncludeInvisible" runat="server" Suffix=":" Text="Include Hidden Tab"
                ControlName="chkIncludeInvisible"></dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkIncludeInvisible" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <dnn:Label ID="plRecursive" runat="server" Suffix=":" ControlName="chkRecursive">
            </dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkRecursive" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <dnn:Label ID="plDisplayIcon" runat="server" Suffix=":" ControlName="chkDisplayIcon">
            </dnn:Label>
        </td>
        <td>
            <asp:CheckBox ID="chkDisplayIcon" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <dnn:Label ID="plListTemplate" runat="server" Suffix=":" ControlName="ddlListTemplate">
            </dnn:Label>
        </td>
        <td >
            <asp:DropDownList ID="ddlListTemplate"  runat="server" Width="600px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td >
            <dnn:Label ID="plColumnCount" runat="server" Suffix=":" ControlName="txtColumnCount">
            </dnn:Label>
        </td>
        <td>
            <asp:TextBox ID="txtColumnCount" runat="server" Width="200px" Text="1"></asp:TextBox>
            <asp:RegularExpressionValidator ID="Regularexpressionvalidator4" resourcekey="MustBeInteger"
                runat="server" ValidationExpression="\d*" ControlToValidate="txtColumnCount"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td>
            <dnn:Label ID="plLinkTarget" ControlName="ddlLinkTarget" Suffix=":" runat="server">
            </dnn:Label>
        </td>
        <td>
            <asp:DropDownList ID="ddlLinkTarget" runat="server" Width="600px" >
                <asp:ListItem Value="_self" resourcekey="liLinkTarget_Self"></asp:ListItem>
                <asp:ListItem Value="_blank" resourcekey="liLinkTarget_Blank"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
<p align="center">
    <asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdUpdate" OnClick="cmdUpdate_Click"
        resourcekey="cmdUpdate" runat="server" BorderStyle="none" Text="Update" CausesValidation="true"></asp:LinkButton>&nbsp;
  <asp:LinkButton ID="cmdReturn" OnClick="cmdReturn_Click" CssClass="dnnSecondaryAction"
        resourcekey="cmdReturn" runat="server" BorderStyle="none" Text="Return" CausesValidation="False"></asp:LinkButton>

</p>
