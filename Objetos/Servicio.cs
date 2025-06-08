using System;
using System.Collections.Generic;
using System.Linq;
using TP4_Final.Objetos;

namespace TP4_Final
{
    /// <summary>
    /// Representa un servicio (Mostrador, Autoservicio, Online, Delivery, Llevar).
    /// Gestiona su propia lista de empleados, cola de clientes y métricas de ocupación.
    /// </summary>
    public class Servicio
    {
        /// <summary>Nombre del servicio (por ejemplo: "Mostrador").</summary>
        public string Nombre { get; set; }

        /// <summary>Empleados asignados a este servicio.</summary>
        public List<Empleado> Empleados { get; set; }

        /// <summary>Cola de clientes esperando atención.</summary>
        public Queue<Cliente> Cola { get; set; }

        /// <summary>Total de clientes atendidos por este servicio.</summary>
        public int TotalClientesAtendidos { get; set; }

        // Marca de inicio de un período en que todos los empleados estuvieron ocupados.
        private double tiempoInicioOcupacionCompleta;

        /// <summary>
        /// Tiempo acumulado (en unidades de reloj) durante el cual todos los empleados
        /// de este servicio estuvieron ocupados.
        /// </summary>
        public double TiempoAcumuladoOcupacionCompleta { get; set; }

        /// <summary>
        /// Crea un servicio con un nombre y una cantidad fija de empleados.
        /// </summary>
        /// <param name="nombre">Nombre del servicio.</param>
        /// <param name="cantidadEmpleados">Número de empleados en este servicio.</param>
        public Servicio(string nombre, int cantidadEmpleados)
        {
            Nombre = nombre;
            Empleados = new List<Empleado>();
            for (int i = 0; i < cantidadEmpleados; i++)
            {
                Empleados.Add(new Empleado());
            }

            Cola = new Queue<Cliente>();
            TotalClientesAtendidos = 0;
            tiempoInicioOcupacionCompleta = -1;
            TiempoAcumuladoOcupacionCompleta = 0;
        }

        /// <summary>
        /// Indica si actualmente todos los empleados están ocupados.
        /// </summary>
        public bool TodosOcupados()
        {
            return Empleados.All(e => !e.EstaLibre());
        }

        /// <summary>
        /// Intenta asignar un cliente a un empleado libre. Si no hay ninguno,
        /// encola al cliente. Devuelve true si el cliente comenzó atención inmediatamente.
        /// </summary>
        public bool AtenderCliente(Cliente cliente, double relojActual)
        {
            var libre = Empleados.FirstOrDefault(e => e.EstaLibre());
            if (libre != null)
            {
                // El cliente es atendido de inmediato
                libre.Estado = EstadoEmpleado.Ocupado;
                libre.CantidadClientesAtendidos++;
                libre.HoraInicioOcupacion = relojActual;
                cliente.EmpleadoId = Empleados.IndexOf(libre);
                TotalClientesAtendidos++;
                // Si justo ahora todos quedaron ocupados, empezamos a contar ese periodo
                if (TodosOcupados() && tiempoInicioOcupacionCompleta < 0)
                    tiempoInicioOcupacionCompleta = relojActual;

                return true;
            }
            else
            {
                // Todos ocupados: cliente va a la cola del servicio
                Cola.Enqueue(cliente);
                Console.WriteLine($"{Cola.Count}, {Cola}");
                return false;
                
            }
        }

        /// <summary>
        /// Libera al empleado (por su índice), actualiza métricas de ocupación completa,
        /// y si hay clientes esperando en cola, devuelve el siguiente cliente para que
        /// sea atendido; en caso contrario, deja al empleado libre.
        /// </summary>
        public Cliente LiberarEmpleado(int empleadoIndex, double relojActual)
        {
            

            // 2) Si veníamos de un periodo de ocupación completa, cerrarlo

            if (Empleados[empleadoIndex].HoraInicioOcupacion >= 0)
            {
                Empleados[empleadoIndex].TiempoOcupado += relojActual - Empleados[empleadoIndex].HoraInicioOcupacion;
                Empleados[empleadoIndex].HoraInicioOcupacion = -1;
            }
            Empleados[empleadoIndex].Estado = EstadoEmpleado.Libre;


            if (tiempoInicioOcupacionCompleta >= 0)
            {
                TiempoAcumuladoOcupacionCompleta += relojActual - tiempoInicioOcupacionCompleta;
                tiempoInicioOcupacionCompleta = -1;
            }

            // 3) Si hay gente en cola, atender al siguiente
            if (Cola.Count > 0)
            {
                var siguiente = Cola.Dequeue();
                Empleados[empleadoIndex].Estado = EstadoEmpleado.Ocupado;
                Empleados[empleadoIndex].HoraInicioOcupacion = relojActual;
                Empleados[empleadoIndex].CantidadClientesAtendidos++;
                TotalClientesAtendidos++;
                siguiente.EmpleadoId = empleadoIndex;

                // Si al asignar este cliente vuelven a estar todos ocupados, iniciar periodo
                if (TodosOcupados())
                    tiempoInicioOcupacionCompleta = relojActual;

                return siguiente;
            }

            // 4) No hubo cola: el empleado permanece libre
            return null;
        }
    }
}

