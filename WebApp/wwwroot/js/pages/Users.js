//js que maneja todo el comportamiento de la vista de usuarios

//definir una clase JS, usando prototype

function UsersViewControllers() {

    this.ViewName = "Users";
    this.ApiEndPointName = "User";

    //Metodo constructor
    this.InitView = function () {

        console.log("User init view-- > ok");
        this.LoadTable();

        //Asociar el evento al boton
        $('#btnCreate').click(function () {
            var vc = new UsersViewController();
            vc.Create();
        });

        $('#btnUpdate').click(function () {
            var vc = new UsersViewController();
            vc.Update();
        })

        $('#btnDelte').click(function () {
            var vc = new UsersViewController();
            vc.Delete();
        })


    }
        //Metodo para la carga de una tabla
    this.LoadTable = function() {
        //URL del API a invocar
        //https://localhost:7125/api/User/RetrieveAll


        var ca = new ControlActions();
        var service = this.ApiEndPointName + "/RetrieveAll";

        var urlService = ca.GetUrlApiService(service);

        /*{
    "userCode": "fgzumbado",
    "name": "Felix Zumbado",
    "email": "fgzumbado@gmail.com",
    "password": "Felix123!",
    "birthDate": "1988-11-29T00:00:00",
    "status": "Active",
    "id": 47,
    "created": "2025-06-29T21:24:37.907",
    "updated": "0001-01-01T00:00:00"
  }
                        <tr>
                        <th>Id</th>
                        <th>User Code</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Birth Date</th>
                        <th>Status</th>
                        </tr>


  */


        var columns = [];
        columns[0] = { 'data': "id" }
        columns[1] = { 'data': "userCode" }
        columns[2] = { 'data': "name" }
        columns[3] = { 'data': "email" }
        columns[4] = { 'data': "birthDate" }
        columns[5] = { 'data': "status" }

        $("#tblUsers.").dataTables({
            "ajax": {
                url: urlService,
                "dataSrc": ""

            },
            columns: columns
        });

        //asignar eventos de carga de datos o binding segun el click en la tabla
        $('tblUsers tbody').on('click', 'tr', function())

    
    }

}

$(document).ready(function () {
    var vc = new UsersViewController();
    vc.InitView();
})