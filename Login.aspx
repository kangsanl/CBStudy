<%@ Page Title="정보" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CBStudy.Login" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
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
            $("#MainContent_Button1").click(function () {
                if ($('#MainContent_txtUSERID').val() == "") {
                    alert('Please enter the USERID.');
                    $('#MainContent_txtUSERID').focus();
                    return false;
                }
            });
        });        
    </script>
    
    
    <table style="height: 400px">
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr>
    <td><span class="style6"><strong>Please input your USER ID and press "Next"</strong></span>
    </td>
    </tr>
    <tr>
    <td></td>
    </tr>
    <tr>
    <td></td>
    </tr>
    <tr>
        <td ><strong>USERID : </strong>
            <asp:TextBox ID="txtUSERID" runat="server" BorderStyle="Dashed" 
                BorderWidth="1px" MaxLength="20" Width="100px"></asp:TextBox>&nbsp;<asp:Button ID="Button1" runat="server" Text="Next" 
            BackColor="#3366FF" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="White" Width="80px" class="btn" onclick="Button1_Click" />
            
        </td>
        <td></td>
    </tr>
    <tr style="height: 50px"><td>&nbsp;</td></tr>
        <tr><td>There are two sessions: Session 0 and Session 1</td></tr>
        <tr><td>Session 0 is a practice session. You will play 5 trials of practice</td></tr>
        <tr><td>Session 1 is a real session. The performance in this session will be evaluated and will result in your performance compensation</td></tr>
        <tr><td>In this simulation game, you will not be able to navigate back to previous questions. Using "Back" button of browser will result in disqualification</td></tr>
        <tr style="height: 100px"><td>&nbsp;</td></tr>
    </table>
    <!--hidden field 영역 -->
    <span style="display:none">
    
    <asp:TextBox ID="hdEX_SET" runat='server'></asp:TextBox>
    </span>
    <!--//hidden field 영역-->
</asp:Content>
