using System;

namespace TP4_Final.Objetos
{
    public enum EstadoEmpleado
    {
        Libre,
        Ocupado
    }
    /// <summary>
    /// Representa un empleado genérico que atiende clientes en un servicio.
    /// </summary>
    public class Empleado
    {
        /// <summary>
        /// Estado actual del empleado: "Libre" o "Ocupado".</summary>
        public EstadoEmpleado Estado { get; set; }

        /// <summary>
        /// Cantidad de clientes que este empleado ha atendido.</summary>
        public int CantidadClientesAtendidos { get; set; }

        public double TiempoOcupado { get; set; }
        public double HoraInicioOcupacion { get; set; }

        /// <summary>
        /// Crea un empleado inicialmente libre, sin clientes atendidos.</summary>
        public Empleado()
        {
            Estado = EstadoEmpleado.Libre;
            CantidadClientesAtendidos = 0;
            TiempoOcupado = 0.0;
            HoraInicioOcupacion = -1.0; // Indica que aún no ha comenzado a atender a nadie
        }

        /// <summary>
        /// Constructor de copia: clona el estado y el contador de otro empleado.</summary>
        public Empleado(Empleado other)
        {
            Estado = other.Estado;
            CantidadClientesAtendidos = other.CantidadClientesAtendidos;
            TiempoOcupado = other.TiempoOcupado;
            HoraInicioOcupacion = other.HoraInicioOcupacion;
        }

        /// <summary>
        /// Indica si el empleado está libre para atender a un nuevo cliente.</summary>
        public bool EstaLibre()
        {
            return Estado == EstadoEmpleado.Libre;
        }
    }
}
