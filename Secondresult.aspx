<%@ Page Title="정보" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Secondresult.aspx.cs" Inherits="CBStudy.Secondresult" %>
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
        .style5
        {
            color: #FFFFFF;
        }
        </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" />&nbsp;Informaiton</span>
    <table style="width: 98%; background-color:#4f81bd">
    <tr>
        <td class="style5" colspan="3">Number of Products&nbsp;:&nbsp;<asp:Label ID="lblK" 
                runat="server" Font-Bold="False" ForeColor="White"></asp:Label></td>
        <td class="style5" colspan="3">Number of Retailers&nbsp;:&nbsp;<asp:Label ID="lblN" 
                runat="server" Font-Bold="False" ForeColor="White"></asp:Label></td>
    </tr>
    <tr><td style="height:5px;" colspan="6"></td></tr>
    <tr>
        <td bgcolor="#FFFFCC" colspan="2" style="width:100px;text-align:center;">Your Retail Price<br />
            <asp:Label ID="lblV" runat="server" Font-Bold="False"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:100px;text-align:center;">First Period Price<br />
            <asp:Label ID="lblP1" runat="server" Font-Bold="False"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:100px;text-align:center;">Second Period Price<br />
            <asp:Label ID="lblP2" runat="server" Font-Bold="False"></asp:Label></td>
    </tr>
    </table>
    <br />
    <br />
        <span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" /> 
        <strong>Second Period</strong><br /><br />
    &nbsp;&nbsp;&nbsp;
    <asp:Label ID="lblRESULT" runat="server"></asp:Label>
    <br />
    &nbsp;&nbsp;</span><table align="right" style="height:130px;vertical-align:bottom;">
    <colgroup>
    <col />
    <col />
    </colgroup>
    <tr>
    <td valign="bottom"><asp:Button ID="Button2" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button2_Click" /></td>
    <td valign="bottom"><asp:Label ID="lblTURN" runat="server" Visible="False"></asp:Label>&nbsp;Totalprofit 
        :
        <asp:Label ID="lblTPROFIT" runat="server" Font-Bold="True" Font-Italic="True" 
            ForeColor="Blue"></asp:Label>
        </td>
    </tr>
    </table>
    <!--hidden field 영역-->
    <span style='display:none'>
    <asp:TextBox ID='hdUSERID' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat="server"></asp:TextBox>
    <asp:TextBox ID='hdPROFIT' runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>

