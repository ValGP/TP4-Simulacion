using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP4_Final.Distribuciones;

namespace TP4_Final.Eventos
{
    public abstract class Evento
    {
        public string Nombre { get; }
        public double Rnd { get; set; }
        public double Tiempo { get; set; }
        public double Hora { get; set; }
        protected Distribucion Distribucion { get; }

        protected Evento(string nombre, Distribucion distribucion)
        {
            Nombre = nombre;
            Distribucion = distribucion;
        }

        /// <summary>
        /// Lanza un nuevo RND, genera Tiempo y calcula Hora = reloj + Tiempo.
        /// </summary>
        public void GenerarProxima(double reloj)
        {
            Rnd = new Random().NextDouble();
            Tiempo = Distribucion.generarValor(Rnd);
            Hora = reloj + Tiempo;
        }
    }

    // Para T O D A S las llegadas:
    public class EventoLlegada : Evento
    {
        public EventoLlegada(string nombre, Distribucion dist)
            : base(nombre, dist) { }
    }

    // Para T O D O S los fines de atención (sabe qué empleado libera):
    public class EventoFin : Evento
    {
        public int EmpleadoId { get; set; }
        public EventoFin(string nombre, Distribucion dist, int empleadoId)
            : base(nombre, dist)
        {
            EmpleadoId = empleadoId;
        }
    }
}
