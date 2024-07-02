document.addEventListener("DOMContentLoaded", function () {
    // Obtener referencias a los botones y elementos del formulario
    var botonIniciar = document.querySelector(".btn-iniciar");
    var botonArchivo = document.querySelector(".btn-subir");
    var botonLogin = document.querySelector(".btn-login");
    var formUsuario = document.getElementById("formUsuario");
    var cuadroSubir = document.getElementById("cuadroSubir");

    // Función para cambiar la visibilidad de los elementos
    function cambiarVisibilidad(elemento1, elemento2) {
        elemento1.style.display = "block";
        elemento2.style.display = "none";
        $('#mensajeNoVerificado').text('');
        $('#mensajeVerificado').text('');
    }

    // Escuchar el evento click en los botones
    botonIniciar.addEventListener("click", function () {
        cambiarVisibilidad(formUsuario, cuadroSubir);
    });

    botonLogin.addEventListener("click", function () {
        cambiarVisibilidad(formUsuario, cuadroSubir);
    });

    botonArchivo.addEventListener("click", function () {
        cambiarVisibilidad(cuadroSubir, formUsuario);
    });

    // Seleccionar el input de tipo file
    var inputArchivo = document.getElementById("archivo");

    // Escuchar el evento change en el input de tipo file
    inputArchivo.addEventListener("change", function () {
        if (inputArchivo.files.length > 0) {
            console.log("Archivo cargado:", inputArchivo.files[0]);
        } else {
            console.log("No se ha seleccionado ningún archivo.");
        }
    });

    // Manejar el envío del formulario
    document.getElementById('archivoForm').addEventListener('submit', function (event) {
        event.preventDefault();
        console.log('Formulario de subida enviado');
        $('#mensajeVerificado').text('');
        $('#mensajeNoVerificado').text('');
        var formData = new FormData();
        var inputArchivo = document.getElementById("archivo");
        var ref = $('#usuario').val(); // Obtén el valor del campo de referencia

        if (inputArchivo.files.length > 0) {
            formData.append('archivo', inputArchivo.files[0]);
            formData.append('ref', ref); // Añade la referencia al formData
            console.log('Archivo agregado a formData:', inputArchivo.files[0]);

            fetch('../SubirArchivoIndex', {
                method: 'POST',
                body: formData
            })
                .then(response => {
                    console.log('Respuesta recibida:', response);
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('Success:', data);
                    $('#mensajeVerificado').text('Archivo subido correctamente');
                    // Limpiar el campo de archivo
                    $('#archivo').val('');
                })
                .catch((error) => {
                    console.error('Error:', error);
                    $('#mensajeNoVerificado').text('Error al subir el archivo, Ref no valida o Limite de archivos alcanzados.');
                });
        } else {
            $('#mensajeNoVerificado').text('No se ha seleccionado ningun archivo.');
        }
    });

});
