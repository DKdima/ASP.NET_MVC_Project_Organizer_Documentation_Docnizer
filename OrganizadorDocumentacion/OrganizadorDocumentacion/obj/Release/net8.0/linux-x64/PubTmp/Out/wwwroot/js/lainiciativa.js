// Supongamos que tienes los datos de la tabla en un array llamado 'datosDocumentacion'
// Cada objeto en el array representa una fila de la tabla

// Obtener referencia al tbody de la tabla
const tbody = document.querySelector('#tablaDocumentacion tbody');

// Limpiar cualquier contenido previo en el tbody
tbody.innerHTML = '';

// Iterar sobre los datos y crear filas para la tabla
datosDocumentacion.forEach(dato => {
    const fila = document.createElement('tr');

    // Crear celdas para cada campo y agregarlos a la fila
    const celdaFichero = document.createElement('td');
    celdaFichero.textContent = dato.fichero;
    fila.appendChild(celdaFichero);

    const celdaFechaModificacion = document.createElement('td');
    celdaFechaModificacion.textContent = dato.fechaModificacion;
    fila.appendChild(celdaFechaModificacion);

    const celdaAutor = document.createElement('td');
    celdaAutor.textContent = dato.autor;
    fila.appendChild(celdaAutor);

    const celdaTamaño = document.createElement('td');
    celdaTamaño.textContent = dato.tamaño;
    fila.appendChild(celdaTamaño);

    const celdaEliminar = document.createElement('td');
    // Si hay un fichero, mostrar el botón de eliminar
    if (dato.fichero) {
        const botonEliminar = document.createElement('button');
        botonEliminar.textContent = 'Eliminar';
        botonEliminar.addEventListener('click', () => {
            // Aquí puedes agregar la lógica para eliminar el archivo de la base de datos
            // y luego actualizar la tabla si es necesario
        });
        celdaEliminar.appendChild(botonEliminar);
    }
    fila.appendChild(celdaEliminar);

    // Agregar la fila al tbody de la tabla
    tbody.appendChild(fila);
});
