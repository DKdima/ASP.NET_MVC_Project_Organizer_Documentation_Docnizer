// Objeto para almacenar la estructura de la iniciativa
const iniciativa = {
    nombreIniciativa: '',
    campos: {}
};

// Funci�n para crear un nuevo campo o subcampo
function crearCampo(contenedor) {
    const nuevoCampo = document.createElement('div');
    nuevoCampo.classList.add('campo');


    // Agregar un input para el nombre del campo o subcampo
    const nombreCampo = document.createElement('input');
    nombreCampo.setAttribute('type', 'text');
    nombreCampo.setAttribute('placeholder', 'Nombre del Campo');
    nuevoCampo.appendChild(nombreCampo);

    // Agregar un input para el n�mero de archivos
    const numArchivos = document.createElement('input');
    numArchivos.setAttribute('type', 'number');
    numArchivos.setAttribute('placeholder', 'Cantidad de Archivos');
    nuevoCampo.appendChild(numArchivos);

    // Agregar bot�n para agregar subcampo
    const btnAgregarSubcampo = document.createElement('button');
    btnAgregarSubcampo.textContent = 'Agregar';
    btnAgregarSubcampo.addEventListener('click', () => agregarSubcampo(nuevoCampo));
    nuevoCampo.appendChild(btnAgregarSubcampo);

    // Agregar bot�n para remover el campo o subcampo
    const btnRemoverCampo = document.createElement('button');
    btnRemoverCampo.textContent = 'Remover';
    btnRemoverCampo.addEventListener('click', () => {
        contenedor.removeChild(nuevoCampo);
    });
    nuevoCampo.appendChild(btnRemoverCampo);

    return { nuevoCampo }; // El elemento del campo o subcampo
}

// Funci�n para agregar un nuevo campo o subcampo a un campo existente
function agregarSubcampo(contenedorCampo) {
    const nuevoSubcampo = crearCampo(contenedorCampo);
    nuevoSubcampo.nuevoCampo.classList.add('subcampo');
    contenedorCampo.appendChild(nuevoSubcampo.nuevoCampo);
}

// Funci�n para agregar un nuevo campo
function agregarCampo() {
    const contenedorCampos = document.getElementById('contenedor-campos');
    const nuevoCampo = crearCampo(contenedorCampos);
    contenedorCampos.appendChild(nuevoCampo.nuevoCampo);
}

// Agregar evento para agregar campo al bot�n "Agregar Campo"
document.getElementById('btn-agregar-campo').addEventListener('click', agregarCampo);

// Agregar evento para guardar los campos y subcampos
document.getElementById('btn-guardar').addEventListener('click', () => {

        // Navegar a la p�gina MisIniciativas.cshtml
        window.location.href = "/Home/MisIniciativas";

});


