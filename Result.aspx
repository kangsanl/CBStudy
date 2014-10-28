<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Result.aspx.cs" Inherits="CBStudy.Result" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
    .style4
    {
        width: 9px;
        height: 9px;
    }
        .style5
        {
            color: #FFFFFF;
        }
        .style6
        {
            font-family: Tahoma;
            font-size: large;
        }
        </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table style="height: 20px;width:98%">
    <tr><td align=right><strong>
    Session : <asp:Label ID="lblGroupID" runat="server"></asp:Label> &nbsp;Trial : <asp:Label ID="lblTrial" runat="server"></asp:Label>
    &nbsp;USERID : 
        <asp:Label ID="lblUSERID" runat="server"></asp:Label></strong></td></tr></table>
    <table style="height: 100px;width:98%">
<tr><td>
    <table style="width: 98%">
    <tr>
    <td><span class="style6"><img alt="" class="style4" src="Images/plus.gif" />&nbsp;<strong>Second Period</strong></span></td>
    </tr>
    </table>
    
    <table style="width:98% ; background-color:#4f81bd">
    <tr>
        <td class="style5" colspan="6" align=left>Number of Units for Sale&nbsp;:&nbsp;<asp:Label ID="lblK" 
                runat="server" Font-Bold="True" ForeColor="White"></asp:Label></td>
        
    </tr>
    <tr>
        <td class="style5" colspan="6" align=left>Number of Retailers Presenting From the First Period&nbsp;:&nbsp;<asp:Label ID="lblN1" 
                runat="server" Font-Bold="True" ForeColor="White"></asp:Label></td>
       
    </tr>
    <tr>
        <td class="style5" colspan="6" align=left>Number of Retailers Joining in the Second Period&nbsp;:&nbsp;<asp:Label ID="lblN2" 
                runat="server" Font-Bold="True" ForeColor="White"></asp:Label></td>
        <td colspan="3"></td>
    </tr>
    <tr><td style="height:5px;" colspan="6"></td></tr>
    <tr>
        <td bgcolor="#FFFFCC" colspan="2" style="width:33%;text-align:center;">Your Resale Price<br />
            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Medium" 
                ForeColor="#CC00CC">$ </asp:Label><asp:Label ID="lblV" runat="server" Font-Bold="True" Font-Size="Medium" 
                ForeColor="#CC00CC"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:33%;text-align:center;">First Period Price<br />
            <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">$ </asp:Label><asp:Label ID="lblP1" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:34%;text-align:center;">Second Period Price<br />
            <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Medium">$ </asp:Label><asp:Label ID="lblP2" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
        
    </tr>
    </table>
</td></tr>
</table>
<table style="height: 250px;width:98%">
<tr><td valign="top">
    <table>
    <tr><td></td></tr>
    <tr>
    <td><span class="style6"><img alt="" class="style4" src="Images/plus.gif" /> 
        <strong>Outcome</strong></span></td>
    </tr>
    <tr>
    <td><asp:Label ID="lblRESULT3" runat="server"></asp:Label></td>
    </tr>
    <tr>
    <td><asp:Label ID="lblRESULT4" runat="server"></asp:Label></td>
    </tr>
    </table>

    <table style="width: 98%">
    <tr><td></td></tr>
    <tr>
    <td><span class="style6"><img alt="" class="style4" src="Images/plus.gif" />&nbsp;<strong>Profit</strong></span></td>
    </tr>
    <tr>
    <td><asp:Label ID="lblRESULT1" runat="server"></asp:Label></td>
    </tr>
    <tr>
    <td><asp:Label ID="lblRESULT2" runat="server"></asp:Label></td>
    </tr>
        <tr><td></td></tr>
        <tr><td>

            <asp:Button ID="Button2" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button2_Click" />

            </td></tr>
    </table>    
 </td></tr>
 </table>   
 <table align="right" style="height:20;vertical-align:bottom;">
    <tr>
    <td valign="bottom"></td>
    <td valign="bottom">
        </td>
    <td valign="bottom">&nbsp;</td>
    <td valign="bottom"><asp:Label ID="lblTURN" runat="server" Visible="False"></asp:Label>
        </td>
        <td valign="bottom">
        
        </td>
    </tr>
</table>
    
    <asp:Label ID="lblResult" runat="server" Enabled="False" Visible="False"></asp:Label>
        <asp:Label ID="lblProfit" runat="server" Enabled="False" Visible="False"></asp:Label>
        <asp:Label ID="lblDecision" runat="server" Enabled="False" Visible="False"></asp:Label>
        <asp:Label ID="lblIsLast" runat="server" Enabled="False" Visible="False"></asp:Label>

    <!--hidden field 영역 -->
    <span style='display:none'>
    <asp:TextBox ID='hdPRODUCT' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdUSERID' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat="server"></asp:TextBox>
    <asp:TextBox ID="hdEX_SET" runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>

