using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Repositorios
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        private readonly DbventaContext _dbventaContext;

        public GenericRepository(DbventaContext dbventaContext)
        {
            _dbventaContext = dbventaContext;
        }

        public async Task<TModel> Obtener(Expression<Func<TModel, bool>> filtro)
        {
            try
            {
                //se retorna el modelo encontrado deacuerdo al filtro
                TModel modelo = await _dbventaContext.Set<TModel>().FirstOrDefaultAsync(filtro);
                return modelo;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TModel> Crear(TModel modelo)
        {
            try
            {
                _dbventaContext.Set<TModel>().Add(modelo);
                await _dbventaContext.SaveChangesAsync();
                return modelo;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(TModel modelo)
        {
            try
            {
                _dbventaContext.Set<TModel>().Update(modelo);
                await _dbventaContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(TModel model)
        {
            try
            {
                _dbventaContext.Set<TModel>().Remove(model);
                await _dbventaContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IQueryable<TModel>> Consultar(Expression<Func<TModel, bool>> filtro = null)
        {
            try
            {
                IQueryable<TModel> queryModelo = filtro == null ? _dbventaContext.Set<TModel>() : _dbventaContext.Set<TModel>().Where(filtro);
                return queryModelo;
            }
            catch
            {
                throw;
            }
        }
    }
}
