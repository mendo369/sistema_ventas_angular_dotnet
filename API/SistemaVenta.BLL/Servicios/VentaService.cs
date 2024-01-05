﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepositorio, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepository.Registrar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                {
                    throw new TaskCanceledException("No se pudo crear");
                }

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar();
            var listaResultado = new List<Venta>();

            try
            {
                if (buscarPor == "fecha")
                {
                    DateTime fecha_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));

                    DateTime fecha_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                    listaResultado = await query.Where(v => v.FechaRegistro.Value.Date >= fecha_Inicio.Date && v.FechaRegistro.Value.Date <= fecha_Fin.Date).Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation)
                        .ToListAsync();

                }
                else
                {
                    listaResultado = await query.Where(v => v.NumeroDocumento == numeroVenta)
                        .Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation)
                        .ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<VentaDTO>>(listaResultado);
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var listaResultados = new List<DetalleVenta>();
            try
            {
                DateTime fecha_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));

                DateTime fecha_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                listaResultados = await query
                    .Include(p => p.IdVentaNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdVentaNavigation.FechaRegistro.Value.Date >= fecha_Inicio.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= fecha_Fin.Date
                    )
                    .ToListAsync();
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(listaResultados);
        }
    }
}
