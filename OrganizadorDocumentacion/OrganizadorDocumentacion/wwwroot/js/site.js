// Escuchar el evento de envío del formulario de cierre de sesión
$(document).ready(function () {
    $("#logoutForm").on("submit", function (e) {
        e.preventDefault(); // Evitar el envío del formulario por defecto

        // Mostrar un mensaje de confirmación antes de cerrar sesión
        if (confirm("¿Estás seguro de que deseas cerrar sesión?")) {
            // Si el usuario confirma, enviar el formulario de cierre de sesión
           
        }
    });
});
