<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"%>
<!-- Created by Nick Benedict at Coastal Web Development LLC CoastalWebDevelopment.com benni12@gmail.com-->
<!DOCTYPE html>
<!-- Created by Nick Benedict at Coastal Web Development LLC CoastalWebDevelopment.com benni12@gmail.com-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .widthFull
        {
            width: 100%;
        }

        .fontSize10
        {
            font-size: 10px;
        }
        .displayNone
        {
            display:none;
        }
    </style>
    <script src="Scripts/jquery-1.8.2.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.23.min.js"></script>
    <script src="Scripts/jquery.json-2.3.min.js"></script>
    <script src="Scripts/DataTables/jquery.dataTables.js"></script>
    <link href="Content/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="Content/themes/base/jquery.ui.all.css" rel="stylesheet" />
</head>
<body>
    
    <form id="form1" runat="server">
        <div>
            Enter Participant Name or Hit Search to Return All: 
            <asp:TextBox runat="server" ID="participant"></asp:TextBox><button id="get">Get UserNames</button>
        </div>
        <div>
            <table id="sampleTable" class="widthFull fontsize10 displayNone">
                <thead>
                    <tr>
                        <th>ID
                        </th>
                        <th>UserName
                        </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </form>
</body>
<script>
    $(document).ready(function () {
        $("#get").click(function (e) {
            e.preventDefault();
            getUserNames();

        });
    });


    var getUserNames = function () {
        $("#sampleTable").dataTable({
            "oLanguage": {
                "sZeroRecords": "No records to display",
                "sSearch": "Search on UserName"
            },
            "aLengthMenu": [[25, 50, 100, 150, 250, 500, -1], [25, 50, 100, 150, 250, 500, "All"]],
            "iDisplayLength": 150,
            "bSortClasses": false,
            "bStateSave": false,
            "bPaginate": true,
            "bAutoWidth": false,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "WebService1.asmx/GetItems",
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "bDeferRender": true,
            "fnServerParams": function (aoData) {

                aoData.push({ "name": "iParticipant", "value": $("#participant").val() });

            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {

                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#sampleTable").show();
                                }
                });
            }
        });
    }
</script>
</html>
