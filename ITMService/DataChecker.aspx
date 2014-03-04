<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataChecker.aspx.cs" Inherits="ITMService.DataChecker" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:BuildsConnectionString %>" SelectCommand="SELECT [buildItemID], [caption], [fileName], [orderNumber], [status], [thumbnailPath], [timeStamp], [title], [type], [buildID] FROM [BuildItems]"></asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
