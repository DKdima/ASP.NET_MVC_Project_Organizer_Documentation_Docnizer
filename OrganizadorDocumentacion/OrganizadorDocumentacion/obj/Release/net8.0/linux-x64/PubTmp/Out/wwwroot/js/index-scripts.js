// Espera a que el documento esté completamente cargado antes de ejecutar el script
document.addEventListener("DOMContentLoaded", function () {
    // Obtener referencias a los botones y elementos del formulario
    var botonIniciar = document.querySelector(".btn-iniciar");
    var botonArchivo = document.querySelector(".btn-subir");
    var botonLogin = document.querySelector(".btn-login");
    var formUsuario = document.getElementById("formUsuario");
    var cuadroSubir = document.getElementById("cuadroSubir");

    // Escuchar el evento click en el botón de Iniciar Sesión
    botonIniciar.addEventListener("click", function () {
        // Mostrar el formulario de usuario y contraseña
        formUsuario.style.display = "block";
        // Ocultar el cuadro de imagen si está visible
        cuadroSubir.style.display = "none";
    });

    // Escuchar el evento click en el botón de login
    botonLogin.addEventListener("click", function () {
        // Navegar a la página MisIniciativas.cshtml
        window.location.href = "/Home/MisIniciativas";
    });

    // Escuchar el evento click en el botón de Subir Archivo
    botonArchivo.addEventListener("click", function () {
        // Mostrar el cuadro de imagen
        cuadroSubir.style.display = "block";
        // Ocultar el formulario de usuario y contraseña si está visible
        formUsuario.style.display = "none";
    });

    // Escuchar el evento click en el botón de Subir Archivo
    botonArchivo.addEventListener("click", function () {
        // Mostrar el cuadro de imagen
        cuadroSubir.style.display = "block";
        // Ocultar el formulario de usuario y contraseña si está visible
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
            console.log("No se ha seleccionado ningún archivo.");
        }
    });
});
