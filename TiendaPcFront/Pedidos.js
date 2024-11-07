// FORMULARIO NUEVO PEDIDO

function ocultarFormulario() {
  const elementos = document.querySelectorAll('.nuevo-cliente');
  elementos.forEach(elemento => {
    elemento.style.display = elemento.style.display === 'none' ? 'block' : 'none';
  });
}

function guardarDatos() {
  const formulario = document.getElementById('formularioCliente');
  const formData = new FormData(formulario);
  const datos = {};
  formData.forEach((value, key) => {
    datos[key] = value;
  });

  console.log(datos); // Aquí puedes manejar los datos, por ejemplo, enviarlos a un servidor.
}
//////////////////////////////////////////////////////

// DROP DOWN FORMAS PAGO
async function cargarFormasPago() {
  const response = await fetch('https://localhost:7119/api/FormaPago/GetAll-FormasPago');
  const formasPago = await response.json();

  const menuFormasPago = document.getElementById('formas-pago');
  menuFormasPago.innerHTML = ''; // Limpiar opciones existentes
  const option = document.createElement('a');
    option.className = 'dropdown-item';
    option.href = '#';
    option.textContent = 'TODAS';
    option.setAttribute('data-id', 0); // Guardar el ID en el atributo data-id
    option.addEventListener('click', (event) => {
      event.preventDefault();
      handleOptionClick('formas-pago', option);
    });
  menuFormasPago.appendChild(option)
  formasPago.forEach(fp => {
    const option = document.createElement('a');
    option.className = 'dropdown-item';
    option.href = '#';
    option.textContent = fp.nombreFormaPago;
    option.setAttribute('data-id', fp.idFormaPago); // Guardar el ID en el atributo data-id
    option.addEventListener('click', (event) => {
      event.preventDefault();
      handleOptionClick('formas-pago', option);
    });
    
    menuFormasPago.appendChild(option);
  });
  
}

// Llama a la función para cargar las formas de pago al cargar la página
cargarFormasPago();

function handleOptionClick(menuId, selectedItem) {
  // Quita la clase 'active' de todos los elementos en el menú seleccionado
  const items = document.querySelectorAll(`#${menuId} .dropdown-item`);
  items.forEach(item => item.classList.remove('active'));
  
  // Agrega la clase 'active' al elemento seleccionado
  selectedItem.classList.add('active');
  
  // Solo ejecuta seleccionarFecha si es el menú de fechas
  if (menuId === 'fecha-options') {
    seleccionarFecha(selectedItem.textContent.trim());
  }
}

// Escuchar clic en opciones de los menús
document.querySelectorAll('.dropdown-menu .dropdown-item').forEach(item => {
  item.addEventListener('click', (event) => {
    event.preventDefault(); // Evita que el enlace recargue la página
    
    const menuId = event.target.closest('.dropdown-menu').id;
    handleOptionClick(menuId, event.target);
  });
});


// Función para obtener el valor seleccionado o el valor predeterminado
function getSelectedValue(menuId, defaultValue) {
  const selectedItem = document.querySelector(`#${menuId} .dropdown-item.active`);
  return selectedItem 
    ? { text: selectedItem.textContent, id: selectedItem.getAttribute('data-id') } 
    : { text: defaultValue, id: '' };
}

function seleccionarFecha(fechaSeleccionada) {
  const fechasManual = document.querySelectorAll('.fechas-manual');
  
  if (fechaSeleccionada === 'INGRESAR FECHAS') {
      // Muestra los inputs de fecha manual
      fechasManual.forEach(el => el.classList.remove('d-none'));
  } else {
      // Oculta los inputs de fecha manual si no es "INGRESAR FECHAS"
      fechasManual.forEach(el => el.classList.add('d-none'));
  }

  // Guardar la selección actual de fecha para usar en el botón de búsqueda
  document.getElementById('buscar-button').setAttribute('data-fecha-seleccionada', fechaSeleccionada);
}

function obtenerFechas(fechaSeleccionada) {
  let fechaDesde, fechaFin;
  const today = new Date();
  today.setHours(0, 0, 0, 0); // Aseguramos que la fecha actual solo tenga la parte de día

  switch (fechaSeleccionada) {
    case 'SEMANA ACTUAL':
      const startOfWeek = new Date(today);
      startOfWeek.setDate(today.getDate() - today.getDay());
      fechaDesde = startOfWeek;
      fechaFin = today;
      break;

    case 'MES ACTUAL':
      const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
      fechaDesde = startOfMonth;
      fechaFin = today;
      break;

    case 'AÑO ACTUAL':
      const startOfYear = new Date(today.getFullYear(), 0, 1);
      fechaDesde = startOfYear;
      fechaFin = today;
      break;

    case 'INGRESAR FECHAS':
      // Obtener las fechas desde los inputs
      const fechaInicioInput = document.getElementById('fechaInicio').value;
      const fechaFinInput = document.getElementById('fechaFin').value;

      if (fechaInicioInput && fechaFinInput) {
        fechaDesde = new Date(fechaInicioInput);
        fechaFin = new Date(fechaFinInput);

        // Validación: Verificar que fechaInicio no sea mayor que fechaFin
        if (fechaDesde > fechaFin) {
          swal("Atención!", "La fecha de inicio no puede ser mayor que la fecha de fin.", {
            icon: "warning",
            buttons: {
              confirm: {
                text: "Aceptar",
                className: "btn btn-warning",
              },
            },
          });
          return { fechaDesde: null, fechaFin: null };
        }

        // Validación: Verificar que fechaInicio y fechaFin no sean mayores a la fecha actual
        if (fechaDesde > today || fechaFin > today) {
          swal("Atención!", "Las fechas ingresadas no pueden ser mayores que la fecha actual.", {
            icon: "warning",
            buttons: {
              confirm: {
                text: "Aceptar",
                className: "btn btn-warning",
              },
            },
          });
          return { fechaDesde: null, fechaFin: null };
        }
      } else {
        swal("Atención!", "Por favor ingrese ambas fechas.", {
          icon: "warning",
          buttons: {
            confirm: {
              text: "Aceptar",
              className: "btn btn-warning",
            },
          },
        });
        return { fechaDesde: null, fechaFin: null };
      }
      break;

    default:
      fechaDesde = null;
      fechaFin = null;
  }

  return {
    fechaDesde: fechaDesde ? fechaDesde.toISOString() : null,
    fechaFin: fechaFin ? fechaFin.toISOString() : null
  };
}
document.getElementById('buscar-button').addEventListener('click', () => {
  const fechaSeleccionada = getSelectedValue('fecha-options', 'MES ACTUAL').text;
  const formaPagoSeleccionada = getSelectedValue('formas-pago', 'TODAS');
  const estadoSeleccionado = getSelectedValue('estado-options', 'TODOS').text;

  const { fechaDesde, fechaFin } = obtenerFechas(fechaSeleccionada);
  const idFormaPago = formaPagoSeleccionada.id ? parseInt(formaPagoSeleccionada.id) : null;

  if (fechaDesde && fechaFin) {
    getPedidos(fechaDesde, fechaFin, idFormaPago, estadoSeleccionado);
}
});
async function getPedidos(fechaDesde, fechaFin, idFormaPago, estado) {
  try {
    let url = `https://localhost:7119/api/Pedido/GetAll-Pedidos-Fitros?`;

    if (fechaDesde) {
      url += `fechaDesde=${fechaDesde}&`;
    }
    if (fechaFin) {
      url += `fechaFin=${fechaFin}&`;
    }
    if (idFormaPago) {
      url += `idFormaPago=${idFormaPago}&`;
    }
    if (estado && estado !== 'TODOS') {
      url += `estado=${estado}`;
    }
    const response = await fetch(url);
    if (response.status === 404) {
      // Mostrar la alerta si el código de respuesta es 404
      swal("!Atención!", "No se encontraron pedidos con los filtros seleccionados.", { 
          icon: "warning",
          buttons: {
              confirm: {
                  text: "Aceptar",
                  className: "btn btn-warning",
              },
          },
      });
      return;
    }
    if (!response.ok) {
      throw new Error("No se pudieron cargar los pedidos");
    }
    const pedidos = await response.json();
    cargarPedidos(pedidos);
  } catch (error) {
    console.error("Error:", error.message);
  }
}
// CARGAR PEDIDOS ///
function cargarPedidos(pedidos) {
  const tablaPedidos = document.getElementById('tabla-pedidos');
  tablaPedidos.innerHTML = ''; 

  pedidos.forEach((pedido) => {
    const row = `
      <tr>
        <td hidden>${pedido.idPedido}</td>
        <td>${pedido.nombreCliente}</td>
        <td>${pedido.nombreEmpleado}</td>
        <td>${new Date(pedido.fechaPedido).toLocaleDateString()}</td>
        <td>${pedido.total}</td>
        <td>${pedido.estado}</td>
        <td>${pedido.nombreFormaPago}</td>
        <td>
          <div class="d-flex justify-content-center">
            <button data-id="${pedido.idPedido}" id="detallePedido" type="button" class="btn btn-icon btn-round btn-primary me-2" >
              <i class="fas fa-eye"></i>
            </button>
            <button data-id="${pedido.idPedido}" id="cancelarPedido" type="button" class="btn btn-icon btn-round btn-danger">
              <i class="icon-close"></i>
            </button>
          </div>
        </td>
      </tr>`;
    tablaPedidos.insertAdjacentHTML("beforeend", row);
  });
       // Añadir event listeners a los botones de detalle
       document.querySelectorAll('.btn-primary').forEach(button => {
        button.addEventListener('click', function () {
            const id = this.dataset.id; // Obtener el ID del componente
            verDetallePedido(id); 
        });
    });

    document.querySelectorAll('.btn-danger').forEach(button => {
      button.addEventListener('click', function () {
          const id = this.dataset.id; // Obtener el ID del componente
          cancelarPedido(id); 
      });
  });

}

function verDetallePedido(id){
  console.log('detalle del pedido' , id)
}

function cancelarPedido(id){
  // IMPLEMENTAR UN SWAL PARA 
  console.log('se cancelo el pedido' , id)
}