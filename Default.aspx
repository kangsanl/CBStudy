<%@ Page Title="홈 페이지" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CBStudy._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Simulation Game : Instruction
    </h2>
    <p>
        In this session, you are required to play a computerized simulation game. You will get additional payment based on the result of the game. 
    </p>
    <p>
        Your role is to be a retailer who buys a product and sells it to a customer to make a profit. There are other retailers who are simulated by computer. Each retailer can buy one product in maximum. The number of retailer is more than the number of units for sale. Once you buy a product, you are guaranteed to resell this product to a customer. Therefore, you profit will be retail price minus your buying price.
    </p>
    <p>        
    </p>        
    <p>
        There are two periods when you can buy the product.         
    </p>
    <p>    
        In the first period, the price of product is higher but you can get the product undoubtedly. In the second period, you can get the product for a cheaper price but it is not guaranteed that the product will be available.
    </p>
    <p>
        In the beginning, you will see the number of units for sale, the number of buyers, retail price, the first period price, and the second period price.           
    </p>
    <p>
        Retail price is different for each player, from zero to one hundred. In other words, some players may have retail price lower than the second period price and some players may have retail price between the first and second period price. Of course, some can have more than the first period price. This will affect the possibility of getting the product in the second period. Remember that the number of players having retail price more than the first period price is less than the number of units for sale, therefore you will get the item for sure if you decide to buy in the first period.
    </p>
    <p>
    </p>
</asp:Content>
