$(document).ready(function () {
    let table = $('#tblMovies').DataTable({
        ajax: {
            url: '/api/Movie/RetrieveAll',
            dataSrc: 'Movies'
        },
        columns: [
            { data: 'id' },
            { data: 'title' },
            { data: 'description' },
            { data: 'releaseDate' },
            { data: 'genre' },
            { data: 'director' },
            { data: 'created' },
            { data: 'updated' }
        ]
    });

    // Helper to get form data
    function getMovieFromForm() {
        return {
            Id: parseInt($('#txtId').val()) || 0,
            Title: $('#txtTitle').val(),
            Description: $('#txtDescription').val(),
            ReleaseDate: $('#txtReleaseDate').val(),
            Genre: $('#txtGenre').val(),
            Director: $('#txtDirector').val()
        };
    }

    // Fill form on row click
    $('#tblMovies tbody').on('click', 'tr', function () {
        const data = table.row(this).data();
        if (!data) return;

        $('#txtId').val(data.id);
        $('#txtTitle').val(data.title);
        $('#txtDescription').val(data.description);
        $('#txtReleaseDate').val(data.releaseDate?.split('T')[0]);
        $('#txtGenre').val(data.genre);
        $('#txtDirector').val(data.director);
    });

    // Create
    $('#btnCreate').click(function () {
        const movie = getMovieFromForm();

        $.ajax({
            url: '/api/Movie/Create',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(movie),
            success: function () {
                table.ajax.reload();
                clearForm();
                alert('Movie created successfully');
            },
            error: function (xhr) {
                alert('Error creating movie: ' + xhr.responseText);
            }
        });
    });

    // Update
    $('#btnUpdate').click(function () {
        const movie = getMovieFromForm();

        $.ajax({
            url: '/api/Movie/Update',
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(movie),
            success: function () {
                table.ajax.reload();
                clearForm();
                alert('Movie updated successfully');
            },
            error: function (xhr) {
                alert('Error updating movie: ' + xhr.responseText);
            }
        });
    });

    // Delete
    $('#btnDelete').click(function () {
        const id = $('#txtId').val();

        if (!id) {
            alert('Please select a movie to delete.');
            return;
        }

        $.ajax({
            url: '/api/Movie/Delete/' + id,
            method: 'DELETE',
            success: function () {
                table.ajax.reload();
                clearForm();
                alert('Movie deleted successfully');
            },
            error: function (xhr) {
                alert('Error deleting movie: ' + xhr.responseText);
            }
        });
    });

    // Clear form
    function clearForm() {
        $('#txtId').val('');
        $('#txtTitle').val('');
        $('#txtDescription').val('');
        $('#txtReleaseDate').val('');
        $('#txtGenre').val('');
        $('#txtDirector').val('');
    }
});
