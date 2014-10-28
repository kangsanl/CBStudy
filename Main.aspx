<%@ Page Title="정보" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Main.aspx.cs" Inherits="CBStudy.Main" %>
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
    <script type="text/javascript">
        $(document).ready(function () {
            $("#MainContent_Button1").click(function () {                
                if ($('#MainContent_txtUSERID').val() == "") {
                    alert('Please enter the USERID.');
                    $('#MainContent_txtUSERID').focus();
                    return false;
                }
            });
        });        
    </script>
    <p>
        <span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" /> 
        Inventory / Demand Informaiton</span><table style="width: 100%;">
            <tr>
                <td style="width: 180px">
                    <strong>Number of Products</strong></td>
                <td>
                    :
                    <asp:Label ID="lblK" runat="server" Font-Bold="True"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <strong>Number of Retailers</strong></td>
                <td>
                    :
                    <asp:Label ID="lblN" runat="server" Font-Bold="True"></asp:Label>
                </td>
            </tr>
        </table>
    </p>
<span class="style3">
<br />
&nbsp;<img alt="" class="style4" src="Images/plus.gif" /> Price Informaiton</span><table 
    style="width: 100%;">
    <tr>
        <td>
            <strong>Your Retail Price</strong></td>
        <td>
            :
            <asp:Label ID="lblV" runat="server" Font-Bold="True"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 180px">
            <strong>First Period Price</strong></td>
        <td>
            :
            <asp:Label ID="lblP1" runat="server" Font-Bold="True"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <strong>Second Period Price</strong></td>
        <td>
            :
            <asp:Label ID="lblP2" runat="server" Font-Bold="True"></asp:Label>
        </td>
    </tr>
    
</table>
      <br />
    <table align="right" style="height:130px;vertical-align:bottom;">
    <colgroup>
    <col />
    <col />
    </colgroup>
    <tr>
        <td valign="bottom"><strong>USERID : </strong>
            <asp:TextBox ID="txtUSERID" runat="server" BorderStyle="Dashed" 
                BorderWidth="1px" MaxLength="20" Width="100px"></asp:TextBox>
        </td>
    <td valign="bottom"><asp:Button ID="Button1" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button1_Click" /></td>
    <td valign="bottom"><asp:Label ID="lblTURN" runat="server" Visible="False"></asp:Label></td>
    </tr>
    </table>    
    <!--hidden field 영역 -->
    <span style='display:none'>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>