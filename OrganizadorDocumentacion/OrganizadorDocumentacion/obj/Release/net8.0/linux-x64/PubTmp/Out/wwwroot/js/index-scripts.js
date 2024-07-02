// Espera a que el documento est� completamente cargado antes de ejecutar el script
document.addEventListener("DOMContentLoaded", function () {
    // Obtener referencias a los botones y elementos del formulario
    var botonIniciar = document.querySelector(".btn-iniciar");
    var botonArchivo = document.querySelector(".btn-subir");
    var botonLogin = document.querySelector(".btn-login");
    var formUsuario = document.getElementById("formUsuario");
    var cuadroSubir = document.getElementById("cuadroSubir");

    // Escuchar el evento click en el bot�n de Iniciar Sesi�n
    botonIniciar.addEventListener("click", function () {
        // Mostrar el formulario de usuario y contrase�a
        formUsuario.style.display = "block";
        // Ocultar el cuadro de imagen si est� visible
        cuadroSubir.style.display = "none";
    });

    // Escuchar el evento click en el bot�n de login
    botonLogin.addEventListener("click", function () {
        // Navegar a la p�gina MisIniciativas.cshtml
        window.location.href = "/Home/MisIniciativas";
    });

    // Escuchar el evento click en el bot�n de Subir Archivo
    botonArchivo.addEventListener("click", function () {
        // Mostrar el cuadro de imagen
        cuadroSubir.style.display = "block";
        // Ocultar el formulario de usuario y contrase�a si est� visible
        formUsuario.style.display = "none";
    });

    // Escuchar el evento click en el bot�n de Subir Archivo
    botonArchivo.addEventListener("click", function () {
        // Mostrar el cuadro de imagen
        cuadroSubir.style.display = "block";
        // Ocultar el formulario de usuario y contrase�a si est� visible
        formUsuario.style.display = "none";
    });

    // Seleccionar el input de tipo file
    var inputArchivo = document.getElementById("archivo");

    // Escuchar el evento change en el input de tipo file
    inputArchivo.addEventListener("change", function () {
        // Verificar si se ha cargado un archivo
        if (inputArchivo.files.length > 0) {
            console.log("Archivo cargado:", inputArchivo.files[0]);
        } else {
            console.log("No se ha seleccionado ning�n archivo.");
        }
    });
});
