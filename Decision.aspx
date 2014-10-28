<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Decision.aspx.cs" Inherits="CBStudy.Decision" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
    .style3
    {
        width: 11px;
        height: 11px;
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
                    alert('Please choose the option.');
                    return false;
                }
                if ($('#MainContent_txtUSERID').val() == "") {
                    alert('Please enter the USERID.');
                    $('#MainContent_txtUSERID').focus();
                    return false;
                }
                if ($('#MainContent_rbConfidence1').is(':visible') == true && $('#MainContent_rbConfidence1').is(':checked') == false && $('#MainContent_rbConfidence2').is(':checked') == false && $('#MainContent_rbConfidence3').is(':checked') == false && $('#MainContent_rbConfidence4').is(':checked') == false && $('#MainContent_rbConfidence5').is(':checked') == false) {
                    alert('Please choose the option.');
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
    <table style="height: 20px;width:98%">
    <tr><td align=right><strong>
    Session : <asp:Label ID="lblGroupID" runat="server"></asp:Label> &nbsp;Trial : <asp:Label ID="lblTrial" runat="server"></asp:Label>
    &nbsp;USERID : 
        <asp:Label ID="lblUSERID" runat="server"></asp:Label></strong></td></tr></table>
    <table style="height: 100px;width:98%">
    <tr><td>
    <table style="width: 98%; vertical-align:top">
    <tr>
    <td><span class="style6"><img alt="" class="style3" src="Images/plus.gif" />&nbsp;<strong>First Period</strong></span></td>
    </tr>
    </table>
    
    <table style="width:98% ;vertical-align:top; background-color:#4f81bd">
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
    </td></tr></table>
    <table style="height: 250px;width:98%"><tr><td valign="top">
    <table style="width: 98%;vertical-align:top">
    <tr><td></td></tr>
    <tr><td><img alt="" class="style4" src="Images/plus.gif" /> 
        <span class="style6"> 
        <strong>Decision</strong></span></td></tr>
    
    </table>
    <table style="vertical-align:top">
    <tr><td><strong>What is your decision?</strong></td></tr>
    <tr><td><asp:RadioButton ID="rbDecision1" runat="server" GroupName="period" 
        Text="Buy in the first period" />
        <br />

    <asp:RadioButton ID="rbDecision2" runat="server" GroupName="period"
        Text="Wait until the second period" /></td></tr>
        <tr><td>&nbsp; &nbsp;</td></tr>
        <tr><td><asp:Button ID="Button2" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button2_Click" /></td></tr>
        <tr><td>
        <br />
        <br />
        <br />
        <br />
        <br />
            </td></tr>
            
    </table>
    </td></tr></table>
    <table align="right" style="height:20;vertical-align:bottom;">
    
    <tr>
    <td valign="bottom"></td>
    <td valign="bottom">
        </td>
    <td valign="bottom">&nbsp;</td>
    <td valign="bottom">
        <asp:Label ID="lblResult" runat="server" Enabled="False" Visible="False"></asp:Label>
        </td>
    </tr>
    </table>
    
    <!--hidden field 영역 -->
    <span style="display:none">
    <asp:Label ID="lblY" runat="server"></asp:Label>
    <asp:TextBox ID='hdMAX' runat='server'></asp:TextBox>
    <asp:TextBox ID='hdTURN' runat="server"></asp:TextBox>
    <asp:TextBox ID="hdGROUPID" runat='server'></asp:TextBox>
    <asp:TextBox ID="hdEX_SET" runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>


