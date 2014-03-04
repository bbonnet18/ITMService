<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasicTest.aspx.cs" Inherits="ITMService.BasicTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">

        var id = "";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input id="username" type="hidden" value="@User.Identity.Name" />
    </div>
    </form>
</body>
</html>
