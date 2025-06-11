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
        
        /// <summary>Capacidad máxima de la cola. Si se supera, los clientes se pierden.</summary>
        public int CapacidadCola { get; set; }
        /// <summary>Cantidad de clientes que abandonaron por cola llena.</summary>
        public int ClientesPerdidos { get; set; }
        /// <summary>Cliente actualmente en atención por cada empleado.</summary>
        public Cliente[] ClienteEnAtencion { get; set; }

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
        public Servicio(string nombre, int cantidadEmpleados, int capacidadCola = int.MaxValue)
        {
            Nombre = nombre;
            Empleados = new List<Empleado>();
            for (int i = 0; i < cantidadEmpleados; i++)
            {
                Empleados.Add(new Empleado());
            }

            Cola = new Queue<Cliente>();
            CapacidadCola = capacidadCola;
            ClientesPerdidos = 0;
            TotalClientesAtendidos = 0;
            tiempoInicioOcupacionCompleta = -1;
            TiempoAcumuladoOcupacionCompleta = 0;
            ClienteEnAtencion = new Cliente[cantidadEmpleados];
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
                cliente.Estado = EstadoCliente.SiendoAtendido;
                cliente.HoraInicioEspera = relojActual;
                cliente.HoraFinEspera = relojActual;
                ClienteEnAtencion[cliente.EmpleadoId] = cliente;
                TotalClientesAtendidos++;
                // Si justo ahora todos quedaron ocupados, empezamos a contar ese periodo
                if (TodosOcupados() && tiempoInicioOcupacionCompleta < 0)
                    tiempoInicioOcupacionCompleta = relojActual;

                return true;
            }
            else
            {
                // Todos ocupados: si la cola está llena, el cliente se pierde
                if (Cola.Count >= CapacidadCola)
                {
                    ClientesPerdidos++;
                    cliente.Estado = EstadoCliente.Finalizado;
                    return false;
                }

                // Cliente espera en cola
                cliente.HoraInicioEspera = relojActual;
                Cola.Enqueue(cliente);
                
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

            var actual = ClienteEnAtencion[empleadoIndex];
            if (actual != null)
            {
                actual.Estado = EstadoCliente.Finalizado;
                actual.HoraFinAtencion = relojActual;
                ClienteEnAtencion[empleadoIndex] = null;
            }


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
                siguiente.Estado = EstadoCliente.SiendoAtendido;
                siguiente.HoraFinEspera = relojActual;
                Empleados[empleadoIndex].Estado = EstadoEmpleado.Ocupado;
                Empleados[empleadoIndex].HoraInicioOcupacion = relojActual;
                Empleados[empleadoIndex].CantidadClientesAtendidos++;
                TotalClientesAtendidos++;
                siguiente.EmpleadoId = empleadoIndex;
                ClienteEnAtencion[empleadoIndex] = siguiente;

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

