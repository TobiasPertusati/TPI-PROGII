document.getElementById("cargarComponentesBtn").addEventListener("click", cargarComponentes);

function cargarComponentes() {
    fetch('https://localhost:7119/api/Componente/GetAll-Componentes')
        .then(response => {
            if(!response.ok){
                throw new Error('Error en la red: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            const tbody = document.querySelector(".table tbody");
            tbody.innerHTML = ""; // Limpia la tabla antes de agregar nuevos datos
            data.forEach(componente => {
                const estado = componente.estado ? "Activo" : "Inactivo";

                // Accede a nombreMarca y tipo, en caso de ser null asigna el valor 'Desconocido'
                const marcaNombre = componente.idMarcaNavigation ? componente.idMarcaNavigation.nombreMarca : 'Desconocido';
                const tipoComponente = componente.idTipoComponenteNavigation ? componente.idTipoComponenteNavigation.tipo : 'Desconocido';

                const row = `
                    <tr>
                        <td class="idCompOculto">${componente.idComponente}</td>
                        <td>${componente.nombre}</td>
                        <td>${marcaNombre}</td>
                        <td>${tipoComponente}</td>
                        <td>$${componente.precio.toFixed(2)}</td>
                        <td>${componente.stock}</td>
                        <td>${componente.stockMinimo}</td>
                        <td>${estado}</td>
                        <td class="sorting_1">
                            <div class="form-button-action">
                                <button type="button" data-id="${componente.idComponente}" data-bs-toggle="tooltip" title="Edit Task" class="btn btn-link btn-primary btn-lg">
                                    <i class="fa fa-edit"></i>
                                </button>
                                <button type="button" data-id="${componente.idComponente}" data-bs-toggle="tooltip" title="Remove" class="btn btn-link btn-danger">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                `;
                tbody.insertAdjacentHTML("beforeend", row);
            });

            // Añadir event listeners a los botones de editar
            document.querySelectorAll('.btn-primary').forEach(button => {
                button.addEventListener('click', function() {
                    const idComponente = this.dataset.id; // Obtener el ID del componente
                    cargarModalComponente(idComponente); // Cargar los datos en el modal
                    const modal = new bootstrap.Modal(document.getElementById('modalEditarComponente'));
                    modal.show();

                });
            });

            // Logica para los botones eliminar:


        })
        .catch(error => console.error('Error al cargar los componentes:', error));
}

//funcion para recargar el modal con el componente elegido
function cargarModalComponente(id) {
    fetch(`https://localhost:7119/api/Componente/GetById-Componente?id=${id}`)
        .then(response => response.json())
        .then(componente => {
            document.getElementById("idComponente").value = componente.idComponente;
            document.getElementById("nombre").value = componente.nombre;
            document.getElementById("precio").value = componente.precio;
            document.getElementById("stock").value = componente.stock;
            document.getElementById("stockMinimo").value = componente.stockMinimo;
            document.getElementById("estado").checked = componente.estado; // Cargar estado en el checkbox

            // Cargar los combos de marcas y tipos
            cargarDatosCombos().then(() => {
                // Ajustar los valores seleccionados en los combos
                document.getElementById("marca").value = componente.idMarca; // Establecer la marca seleccionada
                document.getElementById("tipoComponente").value = componente.idTipoComponente; // Establecer el tipo de componente seleccionado
            });
        })
        .catch(error => console.error('Error al cargar los datos del componente:', error));
}

//funcion en el boton guardar del Modal para guardar los cambios y hacer put 

document.getElementById("guardarCambiosBtn").addEventListener("click", function(){
     const idComponente = document.getElementById("idComponente").value;
     const datos = {
        idComponente: idComponente,
        nombre: document.getElementById("nombre").value,
        precio: parseFloat(document.getElementById("precio").value),
        stock: parseInt(document.getElementById("stock").value),
        stockMinimo: parseInt(document.getElementById("stockMinimo").value),
        idMarca: document.getElementById("marca").value,
        idTipoComponente: document.getElementById("tipoComponente").value,
        estado: document.getElementById("estado").checked
     };

     fetch('https://localhost:7119/api/Componente/Update-Componente',{
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(datos)
     })
     .then(response => {
        if(!response.ok){
            throw new Error('Error al guardar los cambios');
        }
        swal("Éxito", "Componente actualizado exitosamente.", "success");
        return response.json();
        
     })
     .then(data => {
        console.log('Componente actualizado:', data);
       
        cargarComponentes() // y recargamos la tabla con el nuevo componente
     })
     .catch(error => console.error('Error al guardar los cambios:', error))
     swal("Error", "No se pudieron guardar los cambios.", "error")

})

//Funcion en el boton Cerrar del modal
document.getElementById("cerrarModal").addEventListener("click", function() {
    const modal = new bootstrap.Modal(document.getElementById('modalEditarComponente'));
    modal.hide();
});

//funcion para cargar los combos de marcas y tipos de  componentes en el modal

function cargarDatosCombos() {
    return Promise.all([
        fetch('https://localhost:7119/api/Marca/GetAll-Marcas')
            .then(response => response.json())
            .then(marcas => {
                console.log(marcas);
                const selectMarca = document.getElementById("marca");
                selectMarca.innerHTML = ""; // Limpia el combo antes de agregar nuevas opciones
                marcas.forEach(marca => {
                    const option = document.createElement("option");
                    option.value = marca.idMarca; // valor que seleccionamos 
                    option.textContent = marca.nombreMarca; // y el nombre visible
                    selectMarca.appendChild(option);
                });
            })
            .catch(error => console.error('Error al cargar marcas:', error)),

        fetch('https://localhost:7119/api/TipoComponente/GetAll-TiposComponente')
            .then(response => response.json())
            .then(tipos => {
                console.log(tipos)
                const selectTipo = document.getElementById("tipoComponente");
                selectTipo.innerHTML = ""; // Limpia el combo antes de agregar nuevas opciones
                tipos.forEach(tipo => {
                    const option = document.createElement("option");
                    option.value = tipo.idTipoComponente;
                    option.textContent = tipo.tipo;
                    selectTipo.appendChild(option);
                });
            })
            .catch(error => console.error('Error al cargar tipos:', error))
    ]);
}

document.addEventListener("DOMContentLoaded", async function() {
    // Función para consumir el endpoint y crear los elementos en el menú desplegable
    async function cargarDatos(endpoint, idElementoDropdown, campo) {
        try {
            const response = await fetch(endpoint);
            if (!response.ok) {
                console.error("Error al cargar los datos:", response.statusText);
                return;
            }

            const data = await response.json();
            const dropdown = document.getElementById(idElementoDropdown);

            // Limpiar dropdown de elementos previos
            dropdown.innerHTML = '<li><a class="dropdown-item" href="#">TODOS</a></li>';

            // Crear elementos en el dropdown con los datos obtenidos
            data.forEach(item => {
                const li = document.createElement("li");
                const a = document.createElement("a");
                a.className = "dropdown-item";
                a.href = "#";
                a.textContent = item[campo];

                // Añadir evento click para asignar clase "active"
                a.addEventListener("click", function(event) {
                    event.preventDefault();
                    // Quitar la clase "active" de otros elementos
                    const items = dropdown.querySelectorAll(".dropdown-item");
                    items.forEach(i => i.classList.remove("active"));
                    // Asignar clase "active" al elemento seleccionado
                    a.classList.add("active");
                });

                li.appendChild(a);
                dropdown.appendChild(li);
            });
        } catch (error) {
            console.error("Error al consumir el endpoint:", error);
        }
    }

    // Cargar datos para tipos de componente y marcas
    await cargarDatos("https://localhost:7119/api/TipoComponente/GetAll-TiposComponente", "tipo-componente", "tipo");
    await cargarDatos("https://localhost:7119/api/Marca/GetAll-Marcas", "marcas", "nombreMarca");

    // Función para obtener las opciones seleccionadas cuando se presiona "Buscar"
    document.getElementById("cargarComponentesBtn").addEventListener("click", function() {
        const tipoSeleccionado = document.querySelector("#tipo-componente .dropdown-item.active")?.textContent || "TODOS";
        const marcaSeleccionada = document.querySelector("#marcas .dropdown-item.active")?.textContent || "TODAS";

        console.log("Tipo de componente seleccionado:", tipoSeleccionado);
        console.log("Marca seleccionada:", marcaSeleccionada);

        // Aquí puedes hacer cualquier procesamiento adicional con las opciones seleccionadas
    });
});
