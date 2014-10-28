<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="GroupEnd.aspx.cs" Inherits="CBStudy.GroupEnd" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
    .style3
    {
        font-family: Tahoma;
        font-size: medium;
    }
    .style4
    {
        width: 9px;
        height: 9px;
    }
        </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    
    <table style="height: 400px;width:98%">
    <tr><td>
    <table>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td><span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" /> 
    <strong>This is the end of the practice , please press the &quot;Next&quot; button to go the next treatment</strong></span></td></tr>
    <tr><td></td></tr>
    <tr><td><asp:Button ID="Button1" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button2_Click" /></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    </table>

    <table align="right" style="height:20;vertical-align:bottom;">
    
    <tr>
    <td valign="bottom">&nbsp;<asp:Label ID="lblUSERID" runat="server" Visible="False"></asp:Label>
        </td>
    <td valign="bottom">&nbsp;</td>
    <td valign="bottom">
        </td>
    </tr>
    </table>
    </td></tr>
    </table>
    <!--hidden field 영역 -->
    <span style='display:none'>
    <asp:TextBox ID='hdUSERID' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat="server"></asp:TextBox>
    <asp:TextBox ID="hdEX_SET" runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>

