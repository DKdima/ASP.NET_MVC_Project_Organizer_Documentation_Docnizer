function validarFormulario() {

    var clave = document.getElementById("Clave").value;


    // Realizar la validación del campo Clave
    if (clave === "") {
        alert("El campo Clave es obligatorio.");
        return false; // Evitar que se envíe el formulario si la validación falla
    } else if (!contieneTresNumeros(clave)) {
        alert("La clave necesita al menos 3 números.");
        return false; // Evitar que se envíe el formulario si la validación falla
    }

    // Si todas las validaciones pasan, se envía el formulario
    return true;
}

// Función auxiliar para verificar si la clave contiene al menos 3 números
function contieneTresNumeros(clave) {
    var conteoNumeros = 0;
    for (var i = 0; i < clave.length; i++) {
        if (!isNaN(clave[i])) {
            conteoNumeros++;
        }
    }
    return conteoNumeros >= 3;
}