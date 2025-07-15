//js que maneja todo el comportamiento de la vista de usuarios

//definir una clase JS, usando prototype

function UsersViewController() {
     
    this.ViewName = "Users";
    this.ApiEndPointName = "User";

    //Metodo constructor
    this.InitView = function () {

        console.log("User init view-- > ok");
        this.LoadTable();

        //Asociar el evento al boton
        $('#btnCreate').click(() => {
            var vc = new UsersViewController();
            vc.Create();
        });

        $('#btnUpdate').click(() => {
            var vc = new UsersViewController();
            vc.Update();
        });

        $('#btnDelete').click(() => {
            var vc = new UsersViewController();
            vc.Delete();
        });




    }

    //Metodo para la carga de una tabla
    this.LoadTable = function () {
        var ca = new ControlActions();
        var service = this.ApiEndPointName + "/RetrieveAll";

        var urlService = ca.GetUrlApiService(service);
        console.log("Calling API:", urlService);

        var columns = [];
        columns[0] = { 'data': "id" };
        columns[1] = { 'data': "userCode" };
        columns[2] = { 'data': "name" };
        columns[3] = { 'data': "email" };
        columns[4] = { 'data': "birthDate" };
        columns[5] = { 'data': "status" };

        $("#tblUsers").DataTable({
            "ajax": {
                url: urlService,
                "dataSrc": ""
            },
            columns: columns
        });

        $('#tblUsers tbody').on('click', 'tr', function () {
            var row = $(this).closest('tr');
            var userDTO = $('#tblUsers').DataTable().row(row).data();

            $('#txtId').val(userDTO.id);
            $('#txtUserCode').val(userDTO.userCode);
            $('#txtName').val(userDTO.name);
            $('#txtEmail').val(userDTO.email);
            $('#txtStatus').val(userDTO.status);

            var onlyDate = userDTO.birthDate.split("T");
            $('#txtBirthDate').val(onlyDate[0]);
        });
    }

    this.Create = function () {
        var userDTO = {};
        userDTO.id = 0;
        userDTO.created = "2025-01-01";
        userDTO.updated = "2025-01-01";

        userDTO.userCode = $('#txtUserCode').val();
        userDTO.name = $('#txtName').val();
        userDTO.email = $('#txtEmail').val();
        userDTO.status = $('#txtStatus').val();
        userDTO.birthDate = $('#txtBirthDate').val();
        userDTO.password = $('#txtPassword').val();

        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Create";

        ca.PostToAPI(urlService, userDTO, function () {
            $('#tblUsers').DataTable().ajax.reload();
        });
    }

    this.Update = function () {
        var userDTO = {};
        //Atributos con valores default, que son controlados por el API
        userDTO.id = $('#txtId').val();
        userDTO.created = "2025-01-01";
        userDTO.updated = "2025-01-01";

        //valores capturados en pantalla
        userDTO.userCode = $('#txtUserCode').val();
        userDTO.name = $('#txtName').val();
        userDTO.email = $('#txtEmail').val(); 
        userDTO.status = $('#txtStatus').val(); 
        userDTO.birthDate = $('#txtBirthDate').val();
        userDTO.password = $('#txtPassword').val();  

        //Enviar la data al API
        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Update";

        ca.PutToAPI(urlService, userDTO, function () {
            //recargo de la tabla
            $('#tblUsers').DataTable().ajax.reload();
        });
    }

    this.Delete = function () {
        var userDTO = {};
        userDTO.id = $('#txtId').val();
        userDTO.created = "2025-01-01";
        userDTO.updated = "2025-01-01";

        userDTO.userCode = $('#txtUserCode').val();
        userDTO.name = $('#txtName').val();  
        userDTO.email = $('#txtEmail').val();   
        userDTO.status = $('#txtStatus').val(); 
        userDTO.birthDate = $('#txtBirthDate').val(); 
        userDTO.password = $('#txtPassword').val();   

        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Delete"; 

        ca.DeleteToAPI(urlService, userDTO, function () {
            $('#tblUsers').DataTable().ajax.reload(); 
        });
    }

}

$(document).ready(function () {
    var vc = new UsersViewController();
    vc.InitView();
});
