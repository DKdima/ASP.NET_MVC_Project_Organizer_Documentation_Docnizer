function validarFormulario() {

    var clave = document.getElementById("Clave").value;


    // Realizar la validaci�n del campo Clave
    if (clave === "") {
        alert("El campo Clave es obligatorio.");
        return false; // Evitar que se env�e el formulario si la validaci�n falla
    } else if (!contieneTresNumeros(clave)) {
        alert("La clave necesita al menos 3 n�meros.");
        return false; // Evitar que se env�e el formulario si la validaci�n falla
    }

    // Si todas las validaciones pasan, se env�a el formulario
    return true;
}

// Funci�n auxiliar para verificar si la clave contiene al menos 3 n�meros
function contieneTresNumeros(clave) {
    var conteoNumeros = 0;
    for (var i = 0; i < clave.length; i++) {
        if (!isNaN(clave[i])) {
            conteoNumeros++;
        }
    }
    return conteoNumeros >= 3;
}