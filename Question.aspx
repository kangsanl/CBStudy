<%@ Page Title="정보" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Question.aspx.cs" Inherits="CBStudy.Question" %>
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
        .style6
        {
            font-family: Tahoma;
            font-size: large;
        }
        </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#MainContent_Button2").click(function () {
                if ($('#MainContent_rbDecision1').is(':checked') == false && $('#MainContent_rbDecision2').is(':checked') == false) {
                    alert('Please choose the option/선택된 항목이 없습니다.');
                    return false;
                }
                if ($('#MainContent_txtUSERID').val() == "") {
                    alert('Please enter the USERID.');
                    $('#MainContent_txtUSERID').focus();
                    return false;
                }
                /*else {
                if ($('#rbDecision1').is(':checked') == true) {
                location.href = "Firstresult.aspx"; //
                } else {
                location.href = "Secondresult.aspx";
                }

                return;
                }
                */
            });
        });        
    </script>
    <span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" />&nbsp;Information</span>
    <table style="width: 98%; background-color:#4f81bd">
    <tr>
        <td class="style5" colspan="3">Number of Products&nbsp;:&nbsp;<asp:Label ID="lblK" 
                runat="server" Font-Bold="False" ForeColor="White"></asp:Label></td>
        <td class="style5" colspan="3">Number of Retailers&nbsp;:&nbsp;<asp:Label ID="lblN" 
                runat="server" Font-Bold="False" ForeColor="White"></asp:Label></td>
    </tr>
    <tr><td style="height:5px;" colspan="6"></td></tr>
    <tr>
        <td bgcolor="#FFFFCC" colspan="2" style="width:33%;text-align:center;">Your Retail Price<br />
            <asp:Label ID="lblV" runat="server" Font-Bold="False"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:33%;text-align:center;">First Period Price<br />
            <asp:Label ID="lblP1" runat="server" Font-Bold="False"></asp:Label></td>
        <td bgcolor="#FFFFCC" colspan="2" style="width:34%;text-align:center;">Second Period Price<br />
            <asp:Label ID="lblP2" runat="server" Font-Bold="False"></asp:Label></td>
        
    </tr>
    </table>
    <br />
    <br />
        <span class="style3">&nbsp;<img alt="" class="style4" src="Images/plus.gif" /> 
        </span>
        <span class="style6"> 
        <strong>First Period</strong></span><span class="style3"><br /><br />
    &nbsp;&nbsp;&nbsp; <strong>What is your decision?</strong><br />
    &nbsp;&nbsp;&nbsp;
    <asp:RadioButton ID="rbDecision1" runat="server" GroupName="period" 
        Text="Buy in the first period" />
&nbsp;<br />
    &nbsp;&nbsp;&nbsp;
    <asp:RadioButton ID="rbDecision2" runat="server" GroupName="period"
        Text="Wait until the second period" />
    </span>
    <br />
    <table align="right" style="height:130px;vertical-align:bottom;">
    <colgroup>
    <col />
    <col />
    <col />
    </colgroup>
    <tr>
    <td valign="bottom"><strong>USERID : </strong>
        <asp:Label ID="lblUSERID" runat="server"></asp:Label>
        </td>
    <td valign="bottom"><asp:Button ID="Button2" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button2_Click" /></td>
    <td valign="bottom" style="margin-left: 320px"><asp:Label ID="lblTURN" 
            runat="server" Visible="False"></asp:Label></td>
    </tr>
    </table>
    <!--hidden field 영역 -->
    <span style="display:none">
    <asp:Label ID="lblY" runat="server"></asp:Label>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat="server"></asp:TextBox>
    <asp:TextBox ID="hdGROUPID" runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>

