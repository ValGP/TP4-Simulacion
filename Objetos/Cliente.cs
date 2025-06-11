using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP4_Final.Objetos
{
    public enum EstadoCliente
    {
        Esperando,
        SiendoAtendido,
        Finalizado
    }

    public enum TipoCliente
    {
        Mostrador,
        Autoservicio,
        Delivery,
        Online,
        Llevar,
        Encuesta,
        Indefinido
    }

    public class Cliente
    {
        private static int _contadorGlobal = 0;

        // Identificación del cliente
        public int Id { get; private set; }

        // Estado y tipo con enums
        public EstadoCliente Estado { get; set; }
        public TipoCliente Tipo { get; set; }

        public int EmpleadoId { get; set; }

        // Tiempos del cliente
        public double HoraLlegada { get; set; }
        public double HoraInicioEspera { get; set; }
        public double HoraFinEspera { get; set; }
        public double HoraFinAtencion { get; set; }

        public double TiempoEnCola => HoraFinEspera - HoraInicioEspera;
        public double TiempoEnSistema => HoraFinAtencion - HoraLlegada;

        // Constructor por defecto (puede no ser necesario si siempre usás el otro)
        public Cliente()
        {
            Id = _contadorGlobal++;
            Estado = EstadoCliente.Esperando;
            Tipo = TipoCliente.Indefinido;
            EmpleadoId = -1;
            HoraLlegada = HoraInicioEspera = HoraFinEspera = HoraFinAtencion = -1;
        }

        public Cliente(EstadoCliente estado, TipoCliente tipo, double horaLlegada = -1)
        {
            Id = _contadorGlobal++;
            Estado = estado;
            Tipo = tipo;
            EmpleadoId = -1;
            HoraLlegada = horaLlegada;
            HoraInicioEspera = HoraFinEspera = HoraFinAtencion = -1;
        }

        // Constructor para copiar un cliente
        public Cliente(Cliente c)
        {
            Id = c.Id;
            Estado = c.Estado;
            Tipo = c.Tipo;
            EmpleadoId = c.EmpleadoId;
            HoraLlegada = c.HoraLlegada;
            HoraInicioEspera = c.HoraInicioEspera;
            HoraFinEspera = c.HoraFinEspera;
            HoraFinAtencion = c.HoraFinAtencion;
        }
    }


}
