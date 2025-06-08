using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP4_Final.Eventos;
using TP4_Final.Objetos;
using TP4_Final.Distribuciones;
using System.Reflection;

namespace TP4_Final
{
    public class VectorEstado
    {
        // Tiempo y Evento
        public string Evento { get; set; }
        public double Reloj { get; set; }

        // Eventos de Llegada
        public EventoLlegada LlegadaMostrador { get; set; }
        public EventoLlegada LlegadaAutoservicio { get; set; }
        public EventoLlegada LlegadaOnline { get; set; }
        public EventoLlegada LlegadaDelivery { get; set; }
        public EventoLlegada LlegadaLlevar { get; set; }

        // Eventos de Fin
        public List<EventoFin> FinesMostrador { get; set; }
        public List<EventoFin> FinesAutoservicio { get; set; }
        public List<EventoFin> FinesOnline { get; set; }
        public List<EventoFin> FinesDelivery { get; set; }
        public List<EventoFin> FinesLlevar { get; set; }

        private readonly int cantidadEmpleadosMostrador = 5;
        private readonly int cantidadEmpleadosAutoservicio = 3;
        private readonly int cantidadEmpleadosOnline = 3;
        private readonly int cantidadEmpleadosDelivery = 3;
        private readonly int cantidadEmpleadosLlevar = 2;

        //Servicios
        //Cada servicio tiene su cola y su lista de empleados
        public Servicio Mostrador { get; set; }
        public Servicio Autoservicio { get; set; }
        public Servicio Online { get; set; }
        public Servicio Delivery { get; set; }
        public Servicio Llevar { get; set; }


        // Lista Clientes
        public List<Cliente> ListaClientes { get; set; }

        /// <summary>
        /// Constructor inicial: arma todo en tiempo 0
        /// </summary>

        public VectorEstado(Dictionary<int, Distribucion> distribuciones)
        {
            Evento = "Inicializacion";
            Reloj = 0;

            // Generacion Llegadas e inicializacion
            LlegadaMostrador = new EventoLlegada("Llegada Mostrador", distribuciones[1]);
            LlegadaAutoservicio = new EventoLlegada("Llegada Autoservicio", distribuciones[2]);
            LlegadaOnline = new EventoLlegada("Llegada Online", distribuciones[3]);
            LlegadaDelivery = new EventoLlegada("Llegada Delivery", distribuciones[5]);
            LlegadaLlevar = new EventoLlegada("Llegada Llevar", distribuciones[4]);

            LlegadaMostrador.GenerarProxima(Reloj);
            LlegadaAutoservicio.GenerarProxima(Reloj);
            LlegadaOnline.GenerarProxima(Reloj);
            LlegadaDelivery.GenerarProxima(Reloj);
            LlegadaLlevar.GenerarProxima(Reloj);

            // Generacion Fines de Atencion e inicializacion
            FinesMostrador = new List<EventoFin>();
            FinesAutoservicio = new List<EventoFin>();
            FinesOnline = new List<EventoFin>();
            FinesDelivery = new List<EventoFin>();
            FinesLlevar = new List<EventoFin>();
         
            for (int i = 0; i < cantidadEmpleadosMostrador; i++)
            {
                var fin = new EventoFin("Fin Mostrador", distribuciones[6], i)
                {
                    Hora = double.PositiveInfinity
                };
                FinesMostrador.Add(fin);
            }

            for (int i = 0; i < cantidadEmpleadosAutoservicio; i++)
            {
                var fin = new EventoFin("Fin Autoservicio", distribuciones[7], i)
                {
                    Hora = double.PositiveInfinity
                };
                FinesAutoservicio.Add(fin);
            }

            for (int i = 0; i < cantidadEmpleadosOnline; i++)
            {
                var fin = new EventoFin("Fin Online", distribuciones[8], i)
                {
                    Hora = double.PositiveInfinity
                };
                FinesOnline.Add(fin);
            }

            for (int i = 0; i < cantidadEmpleadosDelivery; i++)
            {
                var fin = new EventoFin("Fin Delivery", distribuciones[10], i)
                {
                    Hora = double.PositiveInfinity
                };
                FinesDelivery.Add(fin);
            }

            for (int i = 0; i < cantidadEmpleadosLlevar; i++)
            {
                var fin = new EventoFin("Fin Llevar", distribuciones[9], i)
                {
                    Hora = double.PositiveInfinity
                };
                FinesLlevar.Add(fin);
            }

            // Inicializacion Servicios

            Mostrador = new Servicio("Mostrador", cantidadEmpleadosMostrador);
            Autoservicio = new Servicio("Autoservicio", cantidadEmpleadosAutoservicio);
            Online = new Servicio("Online", cantidadEmpleadosOnline);
            Delivery = new Servicio("Delivery", cantidadEmpleadosDelivery);
            Llevar = new Servicio("Llevar", cantidadEmpleadosLlevar);

            // Lista de Clientes
            ListaClientes = new List<Cliente>();

        }

        /// <summary>
        /// Constructor de copia: duplica todo el estado, opcionalmente mantiene los últimos RND/Tiempo de eventos de llegada/fín.
        /// </summary>
        /// 

        public VectorEstado(VectorEstado ve, Dictionary<int, Distribucion> distribuciones, bool keep = false)
        {
            Evento = ve.Evento;
            Reloj = ve.Reloj;

            //Copia Llegadas
            LlegadaMostrador = new EventoLlegada(ve.LlegadaMostrador.Nombre, distribuciones[1]);
            LlegadaAutoservicio = new EventoLlegada(ve.LlegadaAutoservicio.Nombre, distribuciones[2]);
            LlegadaOnline = new EventoLlegada(ve.LlegadaOnline.Nombre, distribuciones[3]);
            LlegadaDelivery = new EventoLlegada(ve.LlegadaDelivery.Nombre, distribuciones[4]);
            LlegadaLlevar = new EventoLlegada(ve.LlegadaLlevar.Nombre, distribuciones[5]);

            if (keep) 
            {
                LlegadaMostrador.Rnd = ve.LlegadaMostrador.Rnd;
                LlegadaMostrador.Tiempo = ve.LlegadaMostrador.Tiempo;
                LlegadaAutoservicio.Rnd = ve.LlegadaAutoservicio.Rnd;
                LlegadaAutoservicio.Tiempo = ve.LlegadaAutoservicio.Tiempo;
                LlegadaOnline.Rnd = ve.LlegadaOnline.Rnd;
                LlegadaOnline.Tiempo = ve.LlegadaOnline.Tiempo;
                LlegadaDelivery.Rnd = ve.LlegadaDelivery.Rnd;
                LlegadaDelivery.Tiempo = ve.LlegadaDelivery.Tiempo;
                LlegadaLlevar.Rnd = ve.LlegadaLlevar.Rnd;
                LlegadaLlevar.Tiempo = ve.LlegadaLlevar.Tiempo;
            }

            LlegadaMostrador.Hora = ve.LlegadaMostrador.Hora;
            LlegadaAutoservicio.Hora = ve.LlegadaAutoservicio.Hora;
            LlegadaOnline.Hora = ve.LlegadaOnline.Hora;
            LlegadaDelivery.Hora = ve.LlegadaDelivery.Hora;
            LlegadaLlevar.Hora = ve.LlegadaLlevar.Hora;

            // Copia Fines de Atencion

            FinesMostrador = ve.FinesMostrador.Select(f => new EventoFin(f.Nombre, distribuciones[6], f.EmpleadoId)
            {
                Rnd = keep ? f.Rnd : 0,
                Tiempo = keep ? f.Tiempo : 0,
                Hora = f.Hora
            }).ToList();

            FinesAutoservicio = ve.FinesAutoservicio.Select(f => new EventoFin(f.Nombre, distribuciones[7], f.EmpleadoId)
            {
                Rnd = keep ? f.Rnd : 0,
                Tiempo = keep ? f.Tiempo : 0,
                Hora = f.Hora
            }).ToList();

            FinesOnline = ve.FinesOnline.Select(f => new EventoFin(f.Nombre, distribuciones[8], f.EmpleadoId)
            {
                Rnd = keep ? f.Rnd : 0,
                Tiempo = keep ? f.Tiempo : 0,
                Hora = f.Hora
            }).ToList();

            FinesDelivery = ve.FinesDelivery.Select(f => new EventoFin(f.Nombre, distribuciones[9], f.EmpleadoId)
            {
                Rnd = keep ? f.Rnd : 0,
                Tiempo = keep ? f.Tiempo : 0,
                Hora = f.Hora
            }).ToList();

            FinesLlevar = ve.FinesLlevar.Select(f => new EventoFin(f.Nombre, distribuciones[10], f.EmpleadoId)
            {
                Rnd = keep ? f.Rnd : 0,
                Tiempo = keep ? f.Tiempo : 0,
                Hora = f.Hora
            }).ToList();

            // Copia Empleados

            Mostrador = CopiaServicio(ve.Mostrador);
            Autoservicio = CopiaServicio(ve.Autoservicio);
            Online = CopiaServicio(ve.Online);
            Delivery = CopiaServicio(ve.Delivery);
            Llevar = CopiaServicio(ve.Llevar);

            // Copia Lista de Clientes
            ListaClientes = ve.ListaClientes
            .Select(c => new Cliente(c))
            .ToList();


        }

        private Servicio CopiaServicio(Servicio s)
        {
            var c = new Servicio(s.Nombre, s.Empleados.Count)
            {
                TotalClientesAtendidos = s.TotalClientesAtendidos,
                TiempoAcumuladoOcupacionCompleta = s.TiempoAcumuladoOcupacionCompleta
            };

            c.Empleados = s.Empleados
                .Select(e => new Empleado
                {
                    Estado = e.Estado,
                    HoraInicioOcupacion = e.HoraInicioOcupacion,
                    TiempoOcupado = e.TiempoOcupado
                })
                .ToList();

            foreach (var cli in s.Cola) c.Cola.Enqueue(new Cliente(cli));
            return c;
        }

        /// <summary>
        /// Metodo para Determinar el Siguiente Evento
        /// 

        /// <summary>
        /// Devuelve el próximo evento (llegada o fin) que ocurrirá después del Reloj actual.
        /// </summary>
        /// <returns>
        /// Tuple: 
        ///   - evento: la instancia de Evento (puede ser EventoLlegada o EventoFin)
        ///   - hora: la hora en que ocurrirá ese evento
        /// </returns>
        public (Evento evento, double hora) DeterminarSiguienteEvento()
        {
            Evento eventoMin = null;
            double horaMin = double.PositiveInfinity;

            // 1) Recorre las 5 llegadas
            var llegadas = new Evento[] {
                LlegadaMostrador,
                LlegadaAutoservicio,
                LlegadaOnline,
                LlegadaDelivery,
                LlegadaLlevar
            };

            foreach (var ev in llegadas)
            {
                if (ev.Hora > Reloj && ev.Hora < horaMin)
                {
                    horaMin = ev.Hora;
                    eventoMin = ev;
                }
            }

            // 2) Recorre cada fin de atención por servicio y por empleado
            foreach (var fin in FinesMostrador)
            {
                if (fin.Hora > Reloj && fin.Hora < horaMin)
                {
                    horaMin = fin.Hora;
                    eventoMin = fin;
                }
            }
            foreach (var fin in FinesAutoservicio)
            {
                if (fin.Hora > Reloj && fin.Hora < horaMin)
                {
                    horaMin = fin.Hora;
                    eventoMin = fin;
                }
            }
            foreach (var fin in FinesOnline)
            {
                if (fin.Hora > Reloj && fin.Hora < horaMin)
                {
                    horaMin = fin.Hora;
                    eventoMin = fin;
                }
            }
            foreach (var fin in FinesDelivery)
            {
                if (fin.Hora > Reloj && fin.Hora < horaMin)
                {
                    horaMin = fin.Hora;
                    eventoMin = fin;
                }
            }
            foreach (var fin in FinesLlevar)
            {
                if (fin.Hora > Reloj && fin.Hora < horaMin)
                {
                    horaMin = fin.Hora;
                    eventoMin = fin;
                }
            }

            return (eventoMin, horaMin);
        }



        /// </summary>
        /// Metodo TolIsta()
        /// 

        public string[] ToLista(int maxClientesEnPantalla)
        {
            var salida = new List<string>();

            // 1) Datos generales
            salida.Add(Evento);
            salida.Add(Reloj.ToString("F4"));

            // 2) Llegadas
            foreach (var ev in new[] { LlegadaMostrador, LlegadaAutoservicio, LlegadaOnline, LlegadaDelivery, LlegadaLlevar })
            {
                salida.Add(ev.Rnd.ToString("F4"));
                salida.Add(ev.Tiempo.ToString("F4"));
                salida.Add(ev.Hora.ToString("F4"));
            }

            // 3) Fines de atención
            foreach (var fin in FinesMostrador)
            {
                salida.Add(fin.Rnd.ToString("F4"));
                salida.Add(fin.Tiempo.ToString("F4"));
                salida.Add(fin.Hora.ToString("F4"));
            }
            foreach (var fin in FinesAutoservicio) 
            {
                salida.Add(fin.Rnd.ToString("F4"));
                salida.Add(fin.Tiempo.ToString("F4"));
                salida.Add(fin.Hora.ToString("F4"));
            }
            foreach (var fin in FinesOnline) 
            {
                salida.Add(fin.Rnd.ToString("F4"));
                salida.Add(fin.Tiempo.ToString("F4"));
                salida.Add(fin.Hora.ToString("F4"));
            }
            foreach (var fin in FinesDelivery) 
            {
                salida.Add(fin.Rnd.ToString("F4"));
                salida.Add(fin.Tiempo.ToString("F4"));
                salida.Add(fin.Hora.ToString("F4"));
            }
            foreach (var fin in FinesLlevar) 
            {
                salida.Add(fin.Rnd.ToString("F4"));
                salida.Add(fin.Tiempo.ToString("F4"));
                salida.Add(fin.Hora.ToString("F4"));
            }

            // 4) Estados de empleados, hora inicio y tiempo ocupado, y colas generales
            //    * Mostrador *
            foreach (var emp in Mostrador.Empleados)
            {
                salida.Add(emp.Estado.ToString());                   
                salida.Add(emp.HoraInicioOcupacion.ToString("F4")); 
                salida.Add(emp.TiempoOcupado.ToString("F4"));       
            }
            salida.Add(Mostrador.Cola.Count.ToString());

            //    * Autoservicio *
            foreach (var emp in Autoservicio.Empleados)
            {
                salida.Add(emp.Estado.ToString());                  
                salida.Add(emp.HoraInicioOcupacion.ToString("F4")); 
                salida.Add(emp.TiempoOcupado.ToString("F4"));       
            }
            salida.Add(Autoservicio.Cola.Count.ToString());

            //    * Online *
            foreach (var emp in Online.Empleados)
            {
                salida.Add(emp.Estado.ToString());
                salida.Add(emp.HoraInicioOcupacion.ToString("F4"));
                salida.Add(emp.TiempoOcupado.ToString("F4"));
            }
            salida.Add(Online.Cola.Count.ToString());

            //    * Delivery *
            foreach (var emp in Delivery.Empleados)
            {
                salida.Add(emp.Estado.ToString());
                salida.Add(emp.HoraInicioOcupacion.ToString("F4"));
                salida.Add(emp.TiempoOcupado.ToString("F4"));
            }
            salida.Add(Delivery.Cola.Count.ToString());

            //    * Llevar *
            foreach (var emp in Llevar.Empleados)
            {
                salida.Add(emp.Estado.ToString());
                salida.Add(emp.HoraInicioOcupacion.ToString("F4"));
                salida.Add(emp.TiempoOcupado.ToString("F4"));
            }
            salida.Add(Llevar.Cola.Count.ToString());

            // 5) Tiempo total de espera por servicio
            double esperaMostrador = ListaClientes
                .Where(c => c.Tipo == TipoCliente.Mostrador && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            salida.Add(esperaMostrador.ToString("F4"));

            double esperaAutoservicio = ListaClientes
                .Where(c => c.Tipo == TipoCliente.Autoservicio && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            salida.Add(esperaAutoservicio.ToString("F4"));

            double esperaOnline = ListaClientes
                .Where(c => c.Tipo == TipoCliente.Online && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            salida.Add(esperaOnline.ToString("F4"));

            double esperaDelivery = ListaClientes
                .Where(c => c.Tipo == TipoCliente.Delivery && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            salida.Add(esperaDelivery.ToString("F4"));

            double esperaLlevar = ListaClientes
                .Where(c => c.Tipo == TipoCliente.Llevar && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            salida.Add(esperaLlevar.ToString("F4"));

            // 5) Métricas de servicio (ya las tienes en cada Servicio)

            salida.Add(Mostrador.TiempoAcumuladoOcupacionCompleta.ToString("F4"));
            salida.Add(Autoservicio.TiempoAcumuladoOcupacionCompleta.ToString("F4"));
            salida.Add(Online.TiempoAcumuladoOcupacionCompleta.ToString("F4"));
            salida.Add(Delivery.TiempoAcumuladoOcupacionCompleta.ToString("F4"));
            salida.Add(Llevar.TiempoAcumuladoOcupacionCompleta.ToString("F4"));

            
            salida.Add(Mostrador.TotalClientesAtendidos.ToString());
            salida.Add(Autoservicio.TotalClientesAtendidos.ToString());
            salida.Add(Online.TotalClientesAtendidos.ToString());
            salida.Add(Delivery.TotalClientesAtendidos.ToString());
            salida.Add(Llevar.TotalClientesAtendidos.ToString());

            


            // 6) Total general
            int total = Mostrador.TotalClientesAtendidos
                      + Autoservicio.TotalClientesAtendidos
                      + Online.TotalClientesAtendidos
                      + Delivery.TotalClientesAtendidos
                      + Llevar.TotalClientesAtendidos;
            salida.Add(total.ToString());

            // 7) Estado de clientes hasta límite
            int count = 0;
            foreach (var cli in ListaClientes)
            {
                if (count++ >= maxClientesEnPantalla) break;
                salida.Add(cli.Estado.ToString());
            }
            for (; count < maxClientesEnPantalla; count++)
                salida.Add("");

            return salida.ToArray();
        }




    }
}
