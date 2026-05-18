using SistemaPolleria.Modelos;
using SistemaPolleria.Repositorios;
using SistemaPolleria.Servicios;
using SistemaPolleria.Eventos;
using SistemaPolleria.Interceptores;

var publicador = new PublicadorEventos();

publicador.PedidoCreado += (sender, e) =>
    File.AppendAllText("Datos/log.txt",
        $"[EVENTO] Pedido #{e.PedidoId} creado para {e.NombreCliente} por ${e.Total:F0}\n");

publicador.EstadoCambiado += (sender, e) =>
    File.AppendAllText("Datos/log.txt",
        $"[EVENTO] Pedido #{e.PedidoId}: {e.EstadoAnterior} → {e.EstadoNuevo}\n");

var repoClientes = new RepositorioClientes();
var repoProductos = new RepositorioProductos();
var repoPedidos = new RepositorioPedidos();

var servicioClientes = FabricaServicios.Crear<IServicio<Cliente>>(
    new ServicioClientes(repoClientes));

var servicioProductos = FabricaServicios.Crear<IServicio<Producto>>(
    new ServicioProductos(repoProductos));

var servicioPedidosBase = new ServicioPedidos(repoPedidos, repoClientes, publicador);
var servicioPedidos = FabricaServicios.Crear<IServicio<Pedido>>(servicioPedidosBase);

bool salir = false;

while (!salir)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║      POLLERÍA EL SABOR               ║");
    Console.WriteLine("║     Sistema de Gestión               ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine("\n  1. Gestión de Productos");
    Console.WriteLine("  2. Gestión de Clientes");
    Console.WriteLine("  3. Gestión de Pedidos");
    Console.WriteLine("  4. Reportes");
    Console.WriteLine("  0. Salir");
    Console.Write("\n  Seleccione una opción: ");

    switch (Console.ReadLine())
    {
        case "1": MenuProductos(servicioProductos, repoProductos); break;
        case "2": MenuClientes(servicioClientes); break;
        case "3": MenuPedidos(servicioPedidosBase, servicioClientes, servicioProductos, repoProductos); break;
        case "4": MenuReportes(servicioPedidosBase, servicioClientes, servicioProductos, repoProductos); break;
        case "0": salir = true; break;
        default: Console.WriteLine("\n Opción inválida."); Pausar(); break;
    }
}

Console.WriteLine("\n ¡Hasta luego!");


static void MenuProductos(IServicio<Producto> servicio, RepositorioProductos repo)
{
    bool volver = false;
    while (!volver)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("══════════════════════════════");
        Console.WriteLine("   GESTIÓN DE PRODUCTOS");
        Console.WriteLine("══════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("  1. Listar productos");
        Console.WriteLine("  2. Agregar producto asado");
        Console.WriteLine("  3. Agregar acompañamiento");
        Console.WriteLine("  4. Actualizar precio");
        Console.WriteLine("  5. Eliminar producto");
        Console.WriteLine("  0. Volver");
        Console.Write("\n  Opción: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Clear();
                var productos = servicio.ObtenerTodos();
                if (productos.Count == 0)
                    Console.WriteLine("\n  No hay productos registrados.");
                else
                    productos.ForEach(p => Console.WriteLine($"  {p}"));
                Pausar();
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("── NUEVO PRODUCTO ASADO ──");
                Console.Write("  Nombre: ");
                var nombreA = Console.ReadLine() ?? "";
                Console.Write("  Precio: $");
                decimal.TryParse(Console.ReadLine(), out decimal precioA);
                Console.Write("  Tamaño (Entero/Medio/Presa): ");
                var tamano = Console.ReadLine() ?? "";

                servicio.Crear(new ProductoAsado
                {
                    Nombre = nombreA,
                    Precio = precioA,
                    Tamano = tamano
                });
                Pausar();
                break;

            case "3":
                Console.Clear();
                Console.WriteLine("── NUEVO ACOMPAÑAMIENTO ──");
                Console.Write("  Nombre: ");
                var nombreAc = Console.ReadLine() ?? "";
                Console.Write("  Precio: $");
                decimal.TryParse(Console.ReadLine(), out decimal precioAc);

                servicio.Crear(new Acompanamiento
                {
                    Nombre = nombreAc,
                    Precio = precioAc
                });
                Pausar();
                break;

            case "4":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(p => Console.WriteLine($"  {p}"));
                Console.Write("\n  ID del producto a actualizar: ");
                int.TryParse(Console.ReadLine(), out int idActP);
                var prodActualizar = servicio.ObtenerPorId(idActP);
                if (prodActualizar == null) { Console.WriteLine("❌ No encontrado."); Pausar(); break; }
                Console.Write("  Nuevo precio: $");
                decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio);
                prodActualizar.Precio = nuevoPrecio;
                servicio.Actualizar(prodActualizar);
                Pausar();
                break;

            case "5":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(p => Console.WriteLine($"  {p}"));
                Console.Write("\n  ID del producto a eliminar: ");
                int.TryParse(Console.ReadLine(), out int idElimP);
                servicio.Eliminar(idElimP);
                Pausar();
                break;

            case "0": volver = true; break;
        }
    }
}


static void MenuClientes(IServicio<Cliente> servicio)
{
    bool volver = false;
    while (!volver)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("══════════════════════════════");
        Console.WriteLine("   GESTIÓN DE CLIENTES");
        Console.WriteLine("══════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("  1. Listar clientes");
        Console.WriteLine("  2. Agregar cliente");
        Console.WriteLine("  3. Actualizar cliente");
        Console.WriteLine("  4. Eliminar cliente");
        Console.WriteLine("  0. Volver");
        Console.Write("\n  Opción: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Clear();
                var clientes = servicio.ObtenerTodos();
                if (clientes.Count == 0)
                    Console.WriteLine("\n  No hay clientes registrados.");
                else
                    clientes.ForEach(c => Console.WriteLine($"  {c}"));
                Pausar();
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("── NUEVO CLIENTE ──");
                Console.Write("  Nombre: ");
                var nombre = Console.ReadLine() ?? "";
                Console.Write("  Teléfono: ");
                var telefono = Console.ReadLine() ?? "";
                Console.Write("  Dirección: ");
                var direccion = Console.ReadLine() ?? "";

                servicio.Crear(new Cliente
                {
                    Nombre = nombre,
                    Telefono = telefono,
                    Direccion = direccion
                });
                Pausar();
                break;

            case "3":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(c => Console.WriteLine($"  {c}"));
                Console.Write("\n  ID del cliente a actualizar: ");
                int.TryParse(Console.ReadLine(), out int idAct);
                var clienteAct = servicio.ObtenerPorId(idAct);
                if (clienteAct == null) { Console.WriteLine("❌ No encontrado."); Pausar(); break; }
                Console.Write($"  Nuevo nombre ({clienteAct.Nombre}): ");
                var nuevoNombre = Console.ReadLine();
                Console.Write($"  Nuevo teléfono ({clienteAct.Telefono}): ");
                var nuevoTel = Console.ReadLine();
                Console.Write($"  Nueva dirección ({clienteAct.Direccion}): ");
                var nuevaDir = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nuevoNombre)) clienteAct.Nombre = nuevoNombre;
                if (!string.IsNullOrWhiteSpace(nuevoTel)) clienteAct.Telefono = nuevoTel;
                if (!string.IsNullOrWhiteSpace(nuevaDir)) clienteAct.Direccion = nuevaDir;

                servicio.Actualizar(clienteAct);
                Pausar();
                break;

            case "4":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(c => Console.WriteLine($"  {c}"));
                Console.Write("\n  ID del cliente a eliminar: ");
                int.TryParse(Console.ReadLine(), out int idElim);
                servicio.Eliminar(idElim);
                Pausar();
                break;

            case "0": volver = true; break;
        }
    }
}


static void MenuPedidos(ServicioPedidos servicio, IServicio<Cliente> svcClientes,
    IServicio<Producto> svcProductos, RepositorioProductos repoProductos)
{
    bool volver = false;
    while (!volver)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("══════════════════════════════");
        Console.WriteLine("   GESTIÓN DE PEDIDOS");
        Console.WriteLine("══════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("  1. Listar pedidos");
        Console.WriteLine("  2. Crear pedido");
        Console.WriteLine("  3. Cambiar estado");
        Console.WriteLine("  4. Ver detalle de pedido");
        Console.WriteLine("  5. Eliminar pedido");
        Console.WriteLine("  0. Volver");
        Console.Write("\n  Opción: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Clear();
                var pedidos = servicio.ObtenerTodos();
                if (pedidos.Count == 0)
                    Console.WriteLine("\n  No hay pedidos registrados.");
                else
                    pedidos.ForEach(p => Console.WriteLine($"  {p}"));
                Pausar();
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("── NUEVO PEDIDO ──");
                var clientesDisp = svcClientes.ObtenerTodos();
                if (clientesDisp.Count == 0)
                {
                    Console.WriteLine(" No hay clientes. Registre uno primero.");
                    Pausar(); break;
                }
                Console.WriteLine("\n  Clientes disponibles:");
                clientesDisp.ForEach(c => Console.WriteLine($"  {c}"));
                Console.Write("\n  ID del cliente: ");
                int.TryParse(Console.ReadLine(), out int clienteId);

                var productosDisp = svcProductos.ObtenerTodos().Where(p => p.Disponible).ToList();
                if (productosDisp.Count == 0)
                {
                    Console.WriteLine(" No hay productos disponibles.");
                    Pausar(); break;
                }

                var detalles = new List<DetallePedido>();
                bool agregarMas = true;

                while (agregarMas)
                {
                    Console.Clear();
                    Console.WriteLine("  Productos disponibles:");
                    productosDisp.ForEach(p => Console.WriteLine($"  {p}"));
                    Console.Write("\n  ID del producto: ");
                    int.TryParse(Console.ReadLine(), out int prodId);
                    var prod = productosDisp.FirstOrDefault(p => p.Id == prodId);
                    if (prod == null) { Console.WriteLine(" Producto no encontrado."); Pausar(); continue; }

                    Console.Write("  Cantidad: ");
                    int.TryParse(Console.ReadLine(), out int cantidad);

                    detalles.Add(new DetallePedido
                    {
                        ProductoId = prod.Id,
                        NombreProducto = prod.Nombre,
                        Cantidad = cantidad,
                        PrecioUnitario = prod.Precio
                    });

                    Console.Write("\n  ¿Agregar otro producto? (s/n): ");
                    agregarMas = Console.ReadLine()?.ToLower() == "s";
                }

                servicio.Crear(new Pedido
                {
                    ClienteId = clienteId,
                    EmpleadoId = 1,
                    Detalles = detalles
                });
                Pausar();
                break;

            case "3":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(p => Console.WriteLine($"  {p}"));
                Console.Write("\n  ID del pedido: ");
                int.TryParse(Console.ReadLine(), out int idEstado);
                Console.WriteLine("\n  Estados: 0=Recibido 1=EnPreparacion 2=Listo 3=Entregado 4=Cancelado");
                Console.Write("  Nuevo estado: ");
                int.TryParse(Console.ReadLine(), out int nuevoEstado);
                servicio.CambiarEstado(idEstado, (EstadoPedido)nuevoEstado);
                Pausar();
                break;

            case "4":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(p => Console.WriteLine($"  {p}"));
                Console.Write("\n  ID del pedido: ");
                int.TryParse(Console.ReadLine(), out int idDetalle);
                var pedidoDetalle = servicio.ObtenerPorId(idDetalle);
                if (pedidoDetalle == null) { Console.WriteLine(" No encontrado."); Pausar(); break; }
                Console.WriteLine($"\n  {pedidoDetalle}");
                Console.WriteLine("  ── Productos ──");
                pedidoDetalle.Detalles.ForEach(d =>
                    Console.WriteLine($"  {d.NombreProducto} x{d.Cantidad} = ${d.Subtotal:F0}"));
                Console.WriteLine($"\n  TOTAL: ${pedidoDetalle.Total:F0}");
                Pausar();
                break;

            case "5":
                Console.Clear();
                servicio.ObtenerTodos().ForEach(p => Console.WriteLine($"  {p}"));
                Console.Write("\n  ID del pedido a eliminar: ");
                int.TryParse(Console.ReadLine(), out int idElimPed);
                servicio.Eliminar(idElimPed);
                Pausar();
                break;

            case "0": volver = true; break;
        }
    }
}


static void MenuReportes(ServicioPedidos servicioPedidos, IServicio<Cliente> svcClientes,
    IServicio<Producto> svcProductos, RepositorioProductos repoProductos)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("══════════════════════════════");
    Console.WriteLine("   REPORTES DEL DÍA");
    Console.WriteLine("══════════════════════════════");
    Console.ResetColor();

    Console.WriteLine($"\n   Total ventas del día: ${servicioPedidos.TotalVentasDelDia():F0}");
    Console.WriteLine($"   Total clientes registrados: {svcClientes.ObtenerTodos().Count}");
    Console.WriteLine($"   Total productos registrados: {svcProductos.ObtenerTodos().Count}");
    Console.WriteLine("\n   Productos más vendidos:");
    var masVendidos = servicioPedidos.ProductosMasVendidos();
    if (masVendidos.Count == 0)
        Console.WriteLine("  (Sin ventas aún)");
    else
        masVendidos.ForEach(c =>
            Console.WriteLine($"  {c.NombreProducto}: {c.CantidadVendida} unidades - ${c.TotalGenerado:F0}"));

    Console.WriteLine("\n   Pedidos por estado:");
    foreach (EstadoPedido estado in Enum.GetValues<EstadoPedido>())
    {
        var cantidad = servicioPedidos.ObtenerPorEstado(estado).Count;
        Console.WriteLine($"  {estado}: {cantidad}");
    }

    Pausar();
}


static void Pausar()
{
    Console.Write("\n  Presione cualquier tecla para continuar...");
    Console.ReadKey();
}