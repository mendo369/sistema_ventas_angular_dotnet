using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository: GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbContext;

        public VentaRepository(DbventaContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();

            //usamos transacciones
            //si dentro de la lógica ocurre un error se restablece la aplicación a como estaba antes
            using (var transaction = _dbContext.Database.BeginTransaction()){
                try
                {
                    foreach(DetalleVenta dv in modelo.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;
                        _dbContext.Productos.Update(productoEncontrado);
                    }

                    await _dbContext.SaveChangesAsync();

                    NumeroDocumento correlativo = _dbContext.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbContext.NumeroDocumentos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    int cantidadDigitos = 8;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidadDigitos, cantidadDigitos);

                    modelo.NumeroDocumento = numeroVenta;

                    await _dbContext.Venta.AddAsync(modelo);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = modelo;

                    //todas las acciones anteriores han sido guardadas temporalmente, con el commit se confirman las acciones
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return ventaGenerada;
            }
        }
    }
}
