// Objeto para almacenar la estructura de la iniciativa
const iniciativa = {
    nombreIniciativa: '',
    campos: []
};

// Funci�n para crear un nuevo campo o subcampo
function crearCampo(contenedor, isSubcampo = false) {
    const nuevoCampo = document.createElement('div');
    nuevoCampo.classList.add(isSubcampo ? 'subcampo' : 'campo');

    // Agregar un input para el nombre del campo o subcampo
    const nombreCampo = document.createElement('input');
    nombreCampo.setAttribute('type', 'text');
    nombreCampo.setAttribute('placeholder', 'Nombre del ' + (isSubcampo ? 'Subcampo' : 'Campo'));
    nuevoCampo.appendChild(nombreCampo);

    // Agregar un input para el n�mero de archivos
    const numArchivos = document.createElement('input');
    numArchivos.setAttribute('type', 'number');
    numArchivos.setAttribute('placeholder', 'Cantidad de Archivos');
    // Validar que el valor ingresado no sea negativo y establecerlo como 0 si est� vac�o
    numArchivos.addEventListener('input', function () {
        if (parseInt(this.value) < 0) {
            this.value = 0;
        } else if (this.value.trim() === '') {
            this.value = 0;
        }
    });
    nuevoCampo.appendChild(numArchivos);

    // Agregar bot�n para agregar subcampo solo si no es un subcampo
    if (!isSubcampo) {
        const btnAgregarSubcampo = document.createElement('button');
        btnAgregarSubcampo.type = 'button'; // Evitar que el bot�n act�e como submit
        btnAgregarSubcampo.textContent = 'Agregar Subcampo';
        btnAgregarSubcampo.addEventListener('click', () => agregarSubcampo(nuevoCampo));
        nuevoCampo.appendChild(btnAgregarSubcampo);
    }

    // Agregar bot�n para remover el campo o subcampo
    const btnRemoverCampo = document.createElement('button');
    btnRemoverCampo.type = 'button'; // Evitar que el bot�n act�e como submit
    btnRemoverCampo.textContent = 'Remover';
    btnRemoverCampo.addEventListener('click', () => {
        contenedor.removeChild(nuevoCampo);
    });
    nuevoCampo.appendChild(btnRemoverCampo);

    // Contenedor de subcampos solo si no es un subcampo
    if (!isSubcampo) {
        const subcamposContenedor = document.createElement('div');
        subcamposContenedor.classList.add('subcampos-contenedor');
        nuevoCampo.appendChild(subcamposContenedor);
    }

    return { nuevoCampo, nombreCampo, numArchivos };
}


// Funci�n para agregar un nuevo subcampo a un campo existente
function agregarSubcampo(contenedorCampo) {
    const subcamposContenedor = contenedorCampo.querySelector('.subcampos-contenedor');
    const nuevoSubcampo = crearCampo(subcamposContenedor, true);
    subcamposContenedor.appendChild(nuevoSubcampo.nuevoCampo);
}

// Funci�n para agregar un nuevo campo
function agregarCampo() {
    const contenedorCampos = document.getElementById('contenedor-campos');
    const nuevoCampo = crearCampo(contenedorCampos);
    contenedorCampos.appendChild(nuevoCampo.nuevoCampo);
}

// Agregar evento para agregar campo al bot�n "Agregar Campo"
document.getElementById('btn-agregar-campo').addEventListener('click', agregarCampo);

// Funci�n para construir el objeto iniciativa desde el DOM
function construirIniciativa() {
    iniciativa.nombreIniciativa = document.getElementById('nombreIniciativa').value;
    const campos = Array.from(document.querySelectorAll('#contenedor-campos > .campo')).map(campo => {
        const nombreCampo = campo.querySelector('input[type="text"]').value;
        const numArchivos = campo.querySelector('input[type="number"]').value;
        const subcampos = construirSubcampos(campo.querySelector('.subcampos-contenedor'));
        return {
            nombreCampo,
            cantidadArchivos: numArchivos,
            subcampos
        };
    });
    iniciativa.campos = campos;
}

// Funci�n para construir los subcampos
function construirSubcampos(contenedorSubcampos) {
    return Array.from(contenedorSubcampos.children).map(subcampo => {
        const nombreSubcampo = subcampo.querySelector('input[type="text"]').value;
        const numArchivos = subcampo.querySelector('input[type="number"]').value;
        return {
            nombreSubcampo,
            cantidadArchivos: numArchivos,
            subcampos: []
        };
    });
}

// Manejar el env�o del formulario
document.getElementById('iniciativaForm').addEventListener('submit', function (event) {
    // Evitar el env�o del formulario hasta que hayamos construido el objeto
    event.preventDefault();

    // Construir el objeto iniciativa desde el DOM
    construirIniciativa();

    // Convertir el objeto a formato JSON y establecerlo en un campo oculto
    const iniciativaJson = JSON.stringify(iniciativa, null, 2);
    console.log("JSON a enviar:", iniciativaJson);

    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'iniciativaJson';
    input.value = iniciativaJson;
    this.appendChild(input);

    // Ahora, enviar el formulario
   this.submit();
});
