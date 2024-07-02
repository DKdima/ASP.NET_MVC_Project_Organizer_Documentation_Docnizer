

document.addEventListener("DOMContentLoaded", function () {
    var botonCrear = document.querySelector(".btn-add");
    // Escuchar el evento click en el bot�n de a�adir
    botonCrear.addEventListener("click", function () {
        // Navegar a la p�gina LaIniciativa.cshtml
        window.location.href = "/Home/CrearIniciativa";
    });

// lista de objetos Iniciativa
var iniciativas = [
    { nombre: "Mudanza", estado: "Activo" },
    { nombre: "Iniciativa2", estado: "Finalizado" },
    { nombre: "Iniciativa3", estado: "Pendiente" },
    { nombre: "Iniciativa4", estado: "Activo" },
    { nombre: "Iniciativa5", estado: "Finalizado" },
    { nombre: "Iniciativa6", estado: "Pendiente" },
    { nombre: "Iniciativa7", estado: "Activo" },
    { nombre: "Iniciativa8", estado: "Finalizado" },
    { nombre: "Iniciativa9", estado: "Pendiente" },
    { nombre: "Iniciativa10", estado: "Activo" }
];

// Obt�n la referencia al cuerpo de la tabla
var tbody = document.querySelector("table tbody");

// Itera sobre la lista de iniciativas
iniciativas.forEach(function (iniciativa) {
    // Crea una nueva fila
    var fila = document.createElement("tr");

    // Crea las celdas y establece su contenido
    var celdaNombre = document.createElement("td");
    celdaNombre.textContent = iniciativa.nombre;

    var celdaEstado = document.createElement("td");
    celdaEstado.textContent = iniciativa.estado;
    celdaEstado.style.textAlign = "right";

    // Agrega las celdas a la fila
    fila.appendChild(celdaNombre);
    fila.appendChild(celdaEstado);

    // Agrega la fila al cuerpo de la tabla
    tbody.appendChild(fila);
});

// Obt�n todas las filas de la tabla
var filas = document.querySelectorAll("table tbody tr");

// Itera sobre cada fila
filas.forEach(function (fila) {
    // Agrega un evento click a cada fila
    fila.addEventListener("click", function () {
        // Aqu� puedes realizar la acci�n que desees cuando se hace clic en una iniciativa
        // Por ejemplo, mostrar m�s detalles o realizar una consulta


        console.log("Iniciativa seleccionada:", fila.textContent);

        // Navegar a la p�gina
        window.location.href = "/Home/LaIniciativa";
    });
});

});