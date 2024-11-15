using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.DLL.Data.Repository.Interfaces;
using TiendaPc.DLL.Models;

namespace TiendaPc.DLL.Data.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DB_TIENDA_PCContext _context;

        public ClienteRepository(DB_TIENDA_PCContext context)
        {
            _context = context;
        }

        public async Task<bool> BajaCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if(cliente == null)
            {
                throw new KeyNotFoundException($"No se encontró un cliente con el id {id}");
            }
            else if (cliente.Estado == false)
            {
                throw new Exception("El cliente ya se encuentra dado de baja");
            }
            else
            {
                cliente.Estado = false;
                _context.Clientes.Update(cliente);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Cliente>> GetAll()
        {
            return await _context.Clientes.ToListAsync();

        }

        public async Task<List<Cliente>> GetAllFiltro(string filtro)
        {
            var clientes = await _context.Clientes.
                Where(c => c.Nombre.ToLower().Contains(filtro.ToLower()) || c.Apellido.ToLower().Contains(filtro.ToLower())).ToListAsync();
            return clientes;
        }

        public async Task<Cliente> GetById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if(cliente == null)
            {
                throw new KeyNotFoundException($"No se encontró un cliente con el id {id}");
            }
            return cliente;
        }

        public async Task<bool> Save(Cliente cliente)
        {

            if(cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "El objeto cliente no puede ser nulo.");
            }
            // agarre Y HAGA UN SP;
            try
            {
                var parameters = new[]
                {
                            new SqlParameter("@nombre", cliente.Nombre),
                            new SqlParameter("@apellido", cliente.Apellido),
                            new SqlParameter("@email", cliente.Email),
                            new SqlParameter("@direccion", cliente.Direccion),
                            new SqlParameter("@nro_calle", cliente.NroCalle),
                            new SqlParameter("@id_tipo_doc", cliente.IdTipoDoc),
                            new SqlParameter("@id_barrio", cliente.IdBarrio),
                            new SqlParameter("@documento", cliente.Documento)    
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC SP_NUEVO_CLIENTE @nombre, @apellido, @email, @direccion," +
                                                            " @nro_calle, @id_tipo_doc, @id_barrio, @documento", parameters);
            }
            catch (SqlException ex) // Captura excepción SQL
            {
                // Lanza la excepción para que el controlador la maneje
                throw new Exception(ex.Message, ex);
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("Error inesperado al guardar el cliente.", ex);
            //}
            return true;
        }

        public async Task<bool> UpdateCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "El objeto cliente no puede ser nulo.");
            }
            _context.Clientes.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
