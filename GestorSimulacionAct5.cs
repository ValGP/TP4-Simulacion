using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP4_Final.Distribuciones;
using TP4_Final.Eventos;
using TP4_Final.Objetos;

namespace TP4_Final
{
    /// <summary>
    /// Gestiona la ejecución de la simulación, registra los estados intermedios
    /// y permite acceder al historial de estados para visualización.
    /// </summary>
    public class GestorSimulacionAct5
    {
        private readonly Dictionary<int, Distribucion> _distribuciones;
        private readonly int _numIteraciones;
        private readonly int _mostrarDesde;
        private readonly int _mostrarHasta;

        /// <summary>
        /// Estados guardados para las iteraciones solicitadas.
        /// </summary>
        public List<VectorEstadoAct5> Registros { get; } = new List<VectorEstadoAct5>();

        /// <summary>
        /// Constructor del gestor de simulación.
        /// </summary>
        /// <param name="numIteraciones">Cantidad total de eventos a procesar.</param>
        /// <param name="mostrarDesde">Índice de iteración desde el cual almacenar estados.</param>
        /// <param name="mostrarHasta">Índice de iteración hasta el cual almacenar estados.</param>
        /// <param name="distribuciones">Mapeo de distribuciones para generación de eventos.</param>
        public GestorSimulacionAct5(
            int numIteraciones,
            int mostrarDesde,
            int mostrarHasta,
            Dictionary<int, Distribucion> distribuciones)
        {
            _numIteraciones = numIteraciones;
            _mostrarDesde = mostrarDesde;
            _mostrarHasta = mostrarHasta;
            _distribuciones = distribuciones;
        }

        /// <summary>
        /// Ejecuta la simulación completa, almacenando los estados
        /// de las iteraciones entre mostrarDesde y mostrarHasta.
        /// </summary>
        public void Ejecutar()
        {
            var estado = new VectorEstadoAct5(_distribuciones);

            // 1) Inicializo el estado inicial
            Registros.Add(new VectorEstadoAct5(estado, _distribuciones, keep: true));

            for (int i = 0; i < _numIteraciones; i++)
            {
                // Siguiente Evento
                var (proximo, hora) = estado.DeterminarSiguienteEvento();

                // Guardo la Info
                estado.Evento = proximo.Nombre;
                estado.Reloj = hora;
                ProcesarEvento(estado, proximo);

                // 3) Programo la próxima vez que ocurra este mismo evento
                proximo.GenerarProxima(estado.Reloj);

                //if (proximo is EventoLlegada)
                //{
                //    // Si es una llegada, programo la próxima llegada de este tipo
                //    proximo.GenerarProxima(estado.Reloj);
                //}
                //else if (proximo is EventoFin fin)
                //{
                //    // Si es un fin, programo el próximo fin para el empleado correspondiente
                //    var empleadoId = fin.EmpleadoId;
                //    if (empleadoId >= 0 && _distribuciones.ContainsKey(empleadoId))
                //    {
                //        proximo.GenerarProxima(estado.Reloj);
                //    }

                // 4) Ahora sí guardo (clono) el estado si está en el rango pedido
                if (i >= _mostrarDesde && i <= _mostrarHasta)
                    Registros.Add(new VectorEstadoAct5(estado, _distribuciones, keep: true));
            
            }
        }


        /// <summary>
        /// Aplica la lógica de negocio correspondiente al evento actual,
        /// ya sea una llegada o un fin de atención en un servicio.
        /// </summary>
        /// <summary>
        /// Aplica la lógica de negocio correspondiente al evento actual,
        /// ya sea una llegada o un fin de atención en un servicio.
        /// </summary>
        private void ProcesarEvento(VectorEstado ve, Evento ev)
        {
            switch (ev)
            {
                // ===== Llegadas =====
                case EventoLlegada lleg when lleg.Nombre.Contains("Mostrador"):
                    var clienteM = new Cliente(EstadoCliente.Esperando, TipoCliente.Mostrador, ve.Reloj);
                    bool inicioM = ve.Mostrador.AtenderCliente(clienteM, ve.Reloj);
                    ve.ListaClientes.Add(clienteM);

                    if (inicioM)
                    {
                        // 🟢 Cambio: si entra directo a atención, programar su fin
                        var finEvM = ve.FinesMostrador[clienteM.EmpleadoId];
                        finEvM.GenerarProxima(ve.Reloj);
                    }
                    break;

                case EventoLlegada lleg when lleg.Nombre.Contains("Autoservicio"):
                    var clienteA = new Cliente(EstadoCliente.Esperando, TipoCliente.Autoservicio, ve.Reloj);
                    bool inicioA = ve.Autoservicio.AtenderCliente(clienteA, ve.Reloj);
                    ve.ListaClientes.Add(clienteA);

                    if (inicioA)
                    {
                        var finEvA = ve.FinesAutoservicio[clienteA.EmpleadoId];
                        finEvA.GenerarProxima(ve.Reloj);
                    }
                    break;

                case EventoLlegada lleg when lleg.Nombre.Contains("Online"):
                    var clienteO = new Cliente(EstadoCliente.Esperando, TipoCliente.Online, ve.Reloj);
                    bool inicioO = ve.Online.AtenderCliente(clienteO, ve.Reloj);
                    ve.ListaClientes.Add(clienteO);

                    if (inicioO)
                    {
                        var finEvO = ve.FinesOnline[clienteO.EmpleadoId];
                        finEvO.GenerarProxima(ve.Reloj);
                    }
                    break;

                case EventoLlegada lleg when lleg.Nombre.Contains("Delivery"):
                    var clienteD = new Cliente(EstadoCliente.Esperando, TipoCliente.Delivery, ve.Reloj);
                    bool inicioD = ve.Delivery.AtenderCliente(clienteD, ve.Reloj);
                    ve.ListaClientes.Add(clienteD);

                    if (inicioD)
                    {
                        var finEvD = ve.FinesDelivery[clienteD.EmpleadoId];
                        finEvD.GenerarProxima(ve.Reloj);
                    }
                    break;

                case EventoLlegada lleg when lleg.Nombre.Contains("Llevar"):
                    var clienteL = new Cliente(EstadoCliente.Esperando, TipoCliente.Llevar, ve.Reloj);
                    bool inicioL = ve.Llevar.AtenderCliente(clienteL, ve.Reloj);
                    ve.ListaClientes.Add(clienteL);

                    if (inicioL)
                    {
                        var finEvL = ve.FinesLlevar[clienteL.EmpleadoId];
                        finEvL.GenerarProxima(ve.Reloj);
                    }
                    break;


                // ===== Fines de atención =====
                case EventoFin fin when fin.Nombre.Contains("Mostrador"):
                    var siguienteM = ve.Mostrador.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
                    if (siguienteM != null)
                    {
                        // 🆕 Vuelve a programar fin para este mismo empleado
                        var finEvM2 = ve.FinesMostrador[fin.EmpleadoId];
                        finEvM2.GenerarProxima(ve.Reloj);
                        ve.ListaClientes.Add(siguienteM);
                    }
                    break;

                case EventoFin fin when fin.Nombre.Contains("Autoservicio"):
                    var siguienteA = ve.Autoservicio.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
                    if (siguienteA != null)
                    {
                        var finEvA2 = ve.FinesAutoservicio[fin.EmpleadoId];
                        finEvA2.GenerarProxima(ve.Reloj);
                        ve.ListaClientes.Add(siguienteA);
                    }
                    break;

                case EventoFin fin when fin.Nombre.Contains("Online"):
                    var siguienteO = ve.Online.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
                    if (siguienteO != null)
                    {
                        var finEvO2 = ve.FinesOnline[fin.EmpleadoId];
                        finEvO2.GenerarProxima(ve.Reloj);
                        ve.ListaClientes.Add(siguienteO);
                    }
                    break;

                case EventoFin fin when fin.Nombre.Contains("Delivery"):
                    var siguienteD = ve.Delivery.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
                    if (siguienteD != null)
                    {
                        var finEvD2 = ve.FinesDelivery[fin.EmpleadoId];
                        finEvD2.GenerarProxima(ve.Reloj);
                        ve.ListaClientes.Add(siguienteD);
                    }
                    break;

                case EventoFin fin when fin.Nombre.Contains("Llevar"):
                    var siguienteL = ve.Llevar.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
                    if (siguienteL != null)
                    {
                        var finEvL2 = ve.FinesLlevar[fin.EmpleadoId];
                        finEvL2.GenerarProxima(ve.Reloj);
                        ve.ListaClientes.Add(siguienteL);
                    }
                    break;

                default:
                    // Evento no contemplado
                    break;
            }
        }

		private void ProcesarEvento(VectorEstadoAct5 ve, Evento ev)
		{
			switch (ev)
			{
				// ===== Llegadas =====
				case EventoLlegada lleg when lleg.Nombre.Contains("Mostrador"):
					var clienteM = new Cliente(EstadoCliente.Esperando, TipoCliente.Mostrador, ve.Reloj);
					bool inicioM = ve.Mostrador.AtenderCliente(clienteM, ve.Reloj);
					ve.ListaClientes.Add(clienteM);

					if (inicioM)
					{
						// 🟢 Cambio: si entra directo a atención, programar su fin
						var finEvM = ve.FinesMostrador[clienteM.EmpleadoId];
						finEvM.GenerarProxima(ve.Reloj);
					}
					break;

				case EventoLlegada lleg when lleg.Nombre.Contains("Autoservicio"):
					var clienteA = new Cliente(EstadoCliente.Esperando, TipoCliente.Autoservicio, ve.Reloj);
					bool inicioA = ve.Autoservicio.AtenderCliente(clienteA, ve.Reloj);
					ve.ListaClientes.Add(clienteA);

					if (inicioA)
					{
						var finEvA = ve.FinesAutoservicio[clienteA.EmpleadoId];
						finEvA.GenerarProxima(ve.Reloj);
					}
					break;

				case EventoLlegada lleg when lleg.Nombre.Contains("Online"):
					var clienteO = new Cliente(EstadoCliente.Esperando, TipoCliente.Online, ve.Reloj);
					bool inicioO = ve.Online.AtenderCliente(clienteO, ve.Reloj);
					ve.ListaClientes.Add(clienteO);

					if (inicioO)
					{
						var finEvO = ve.FinesOnline[clienteO.EmpleadoId];
						finEvO.GenerarProxima(ve.Reloj);
					}
					break;

				case EventoLlegada lleg when lleg.Nombre.Contains("Delivery"):
					var clienteD = new Cliente(EstadoCliente.Esperando, TipoCliente.Delivery, ve.Reloj);
					bool inicioD = ve.Delivery.AtenderCliente(clienteD, ve.Reloj);
					ve.ListaClientes.Add(clienteD);

					if (inicioD)
					{
						var finEvD = ve.FinesDelivery[clienteD.EmpleadoId];
						finEvD.GenerarProxima(ve.Reloj);
					}
					break;

				case EventoLlegada lleg when lleg.Nombre.Contains("Llevar"):
					var clienteL = new Cliente(EstadoCliente.Esperando, TipoCliente.Llevar, ve.Reloj);
					bool inicioL = ve.Llevar.AtenderCliente(clienteL, ve.Reloj);
					ve.ListaClientes.Add(clienteL);

					if (inicioL)
					{
						var finEvL = ve.FinesLlevar[clienteL.EmpleadoId];
						finEvL.GenerarProxima(ve.Reloj);
					}
					break;


				// ===== Fines de atención =====
				case EventoFin fin when fin.Nombre.Contains("Mostrador"):
					var siguienteM = ve.Mostrador.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
					if (siguienteM != null)
					{
						// 🆕 Vuelve a programar fin para este mismo empleado
						var finEvM2 = ve.FinesMostrador[fin.EmpleadoId];
						finEvM2.GenerarProxima(ve.Reloj);
						ve.ListaClientes.Add(siguienteM);
					}
					break;

				case EventoFin fin when fin.Nombre.Contains("Autoservicio"):
					var siguienteA = ve.Autoservicio.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
					if (siguienteA != null)
					{
						var finEvA2 = ve.FinesAutoservicio[fin.EmpleadoId];
						finEvA2.GenerarProxima(ve.Reloj);
						ve.ListaClientes.Add(siguienteA);
					}
					break;

				case EventoFin fin when fin.Nombre.Contains("Online"):
					var siguienteO = ve.Online.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
					if (siguienteO != null)
					{
						var finEvO2 = ve.FinesOnline[fin.EmpleadoId];
						finEvO2.GenerarProxima(ve.Reloj);
						ve.ListaClientes.Add(siguienteO);
					}
					break;

				case EventoFin fin when fin.Nombre.Contains("Delivery"):
					var siguienteD = ve.Delivery.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
					if (siguienteD != null)
					{
						var finEvD2 = ve.FinesDelivery[fin.EmpleadoId];
						finEvD2.GenerarProxima(ve.Reloj);
						ve.ListaClientes.Add(siguienteD);
					}
					break;

				case EventoFin fin when fin.Nombre.Contains("Llevar"):
					var siguienteL = ve.Llevar.LiberarEmpleado(fin.EmpleadoId, ve.Reloj);
					if (siguienteL != null)
					{
						var finEvL2 = ve.FinesLlevar[fin.EmpleadoId];
						finEvL2.GenerarProxima(ve.Reloj);
						ve.ListaClientes.Add(siguienteL);
					}
					break;

				default:
					// Evento no contemplado
					break;
			}
		}

	}
}
