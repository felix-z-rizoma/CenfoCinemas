function MoviesViewController() {
    this.ViewName = "Movies";
    this.ApiEndPointName = "Movie";

    this.InitView = function () {
        console.log("MoviesViewController init");
        this.LoadTable();

        // Initialize buttons
        this.UpdateButtonStates();

        // Bind events
        $('#btnCreate').click(() => this.Create());
        $('#btnUpdate').click(() => this.Update());
        $('#btnDelete').click(() => this.Delete());

        // Update button states when form changes
        $('.form-control').on('input', () => this.UpdateButtonStates());
    }

    this.UpdateButtonStates = function () {
        const hasId = $('#txtId').val() !== '';
        const formFilled = $('#txtTitle').val() &&
            $('#txtDescription').val() &&
            $('#txtReleaseDate').val() &&
            $('#txtGenre').val() &&
            $('#txtDirector').val();

        $('#btnCreate').prop('disabled', hasId || !formFilled);
        $('#btnUpdate').prop('disabled', !hasId || !formFilled);
        $('#btnDelete').prop('disabled', !hasId);
    }

    this.LoadTable = function () {
        var ca = new ControlActions();
        var service = this.ApiEndPointName + "/RetrieveAll";
        var urlService = ca.GetUrlApiService(service);
        console.log("Calling API:", urlService);

        var columns = [
            { data: "id" },
            { data: "title", render: data => data ? data.trim() : "" },
            { data: "description" },
            {
                data: "releaseDate",
                render: data => data ? data.split('T')[0] : ""
            },
            { data: "genre" },
            { data: "director", render: data => data ? data.trim() : "" },
            {
                data: "created",
                render: data => data ? data.split('T')[0] : ""
            },
            {
                data: "updated",
                render: data => {
                    if (!data || data.startsWith("0001-01-01")) return "";
                    return data.split('T')[0];
                }
            }
        ];

        // Destroy previous table if exists
        if ($.fn.DataTable.isDataTable("#tblMovies")) {
            $("#tblMovies").DataTable().clear().destroy();
        }

        // Initialize DataTable
        var dataTable = $("#tblMovies").DataTable({
            ajax: {
                url: urlService,
                dataSrc: "movies"
            },
            columns: columns
        });

        // Store controller reference for click handler
        var controller = this;

        // Row click handler
        $('#tblMovies tbody').on('click', 'tr', function () {
            var rowData = dataTable.row(this).data();
            if (!rowData) return;

            $('#txtId').val(rowData.id);
            $('#txtTitle').val(rowData.title ? rowData.title.trim() : "");
            $('#txtDescription').val(rowData.description || "");

            // Format date for date input
            var releaseDate = rowData.releaseDate ?
                rowData.releaseDate.split('T')[0] : '';
            $('#txtReleaseDate').val(releaseDate);

            $('#txtGenre').val(rowData.genre || "");
            $('#txtDirector').val(rowData.director ? rowData.director.trim() : "");

            controller.UpdateButtonStates();
        });
    }

    this.Create = function () {
        if (!this.ValidateForm()) return;

        var movie = this.getMovieFromForm();
        movie.id = 0; // ID 0 for new records

        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Create";

        this.SetButtonsDisabled(true);

        ca.PostToAPI(urlService, movie, (response) => {
            this.ShowSuccess("Movie created successfully!");
            $('#tblMovies').DataTable().ajax.reload(null, false); // Keep pagination
            this.clearMovieForm();
            this.SetButtonsDisabled(false);
        }, (error) => {
            this.ShowError("Error creating movie: " + error);
            this.SetButtonsDisabled(false);
        });
    }

    this.Update = function () {
        if (!this.ValidateForm()) return;

        var movie = this.getMovieFromForm();
        if (!movie.id) {
            this.ShowError("Please select a movie to update");
            return;
        }

        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Update";

        this.SetButtonsDisabled(true);

        ca.PutToAPI(urlService, movie, (response) => {
            this.ShowSuccess("Movie updated successfully!");
            $('#tblMovies').DataTable().ajax.reload(null, false);
            this.clearMovieForm();
            this.SetButtonsDisabled(false);
        }, (error) => {
            this.ShowError("Error updating movie: " + error);
            this.SetButtonsDisabled(false);
        });
    }

    this.Delete = function () {
        var id = $('#txtId').val();
        if (!id) {
            this.ShowError("Please select a movie to delete");
            return;
        }

        if (!confirm("Are you sure you want to delete this movie?")) {
            return;
        }

        var ca = new ControlActions();
        var urlService = this.ApiEndPointName + "/Delete/" + id;

        this.SetButtonsDisabled(true);

        ca.DeleteToAPI(urlService, null, (response) => {
            this.ShowSuccess("Movie deleted successfully!");
            $('#tblMovies').DataTable().ajax.reload(null, false);
            this.clearMovieForm();
            this.SetButtonsDisabled(false);
        }, (error) => {
            this.ShowError("Error deleting movie: " + error);
            this.SetButtonsDisabled(false);
        });
    }

    this.ValidateForm = function () {
        if (!$('#txtTitle').val()) {
            this.ShowError("Title is required");
            return false;
        }
        if (!$('#txtDescription').val()) {
            this.ShowError("Description is required");
            return false;
        }
        if (!$('#txtReleaseDate').val()) {
            this.ShowError("Release date is required");
            return false;
        }
        if (!$('#txtGenre').val()) {
            this.ShowError("Genre is required");
            return false;
        }
        if (!$('#txtDirector').val()) {
            this.ShowError("Director is required");
            return false;
        }
        return true;
    }

    this.ShowSuccess = function (message) {
        alert("Success: " + message);
    }

    this.ShowError = function (message) {
        alert("Error: " + message);
    }

    this.SetButtonsDisabled = function (disabled) {
        $('#btnCreate').prop('disabled', disabled);
        $('#btnUpdate').prop('disabled', disabled);
        $('#btnDelete').prop('disabled', disabled);
    }

    this.clearMovieForm = function () {
        $('#txtId').val('');
        $('#txtTitle').val('');
        $('#txtDescription').val('');
        $('#txtReleaseDate').val('');
        $('#txtGenre').val('');
        $('#txtDirector').val('');
        this.UpdateButtonStates();
    }

    this.getMovieFromForm = function () {
        return {
            id: $('#txtId').val(),
            title: $('#txtTitle').val().trim(),
            description: $('#txtDescription').val().trim(),
            releaseDate: $('#txtReleaseDate').val(),
            genre: $('#txtGenre').val().trim(),
            director: $('#txtDirector').val().trim()
        };
    }
}

$(document).ready(function () {
    var vc = new MoviesViewController();
    vc.InitView();
});