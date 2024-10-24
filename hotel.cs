using System;
using System.Collections.Generic;

// Singleton para el Hotel
namespace HotelBooking
{
    public class Hotel
    {
        private static Hotel _instance;
        public string Name { get; set; }
        public List<Room> Rooms { get; private set; } = new List<Room>();
        private List<IReservationObserver> observers = new List<IReservationObserver>();

        // Singleton: Método para obtener la única instancia del hotel
        public static Hotel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Hotel { Name = "Luxury Inn" };
                }
                return _instance;
            }
        }

        // Método para agregar habitaciones usando el Factory Method
        public void AddRoom(IRoomFactory roomFactory)
        {
            var room = roomFactory.CreateRoom();
            Rooms.Add(room);
        }

        // Observer: Método para suscribir observadores
        public void Subscribe(IReservationObserver observer)
        {
            observers.Add(observer);
        }

        // Observer: Método para notificar a los observadores
        public void NotifyObservers(Reservation reservation)
        {
            foreach (var observer in observers)
            {
                observer.Update(reservation);
            }
        }

        public void PrintAvailableRooms()
        {
            foreach (var room in Rooms)
            {
                Console.WriteLine($"Room: {room.RoomType}, Price: {room.Price}");
            }
        }
    }

    // Observer: Interfaz para notificar sobre los cambios de reserva
    public interface IReservationObserver
    {
        void Update(Reservation reservation);
    }

    // Observer: Clase que implementa el observador (Servicio de limpieza)
    public class CleaningService : IReservationObserver
    {
        public void Update(Reservation reservation)
        {
            Console.WriteLine($"Cleaning Service notified about the reservation for room: {reservation.Room.RoomType} from {reservation.StartDate} to {reservation.EndDate}.");
        }
    }

    // Clase Room
    public class Room
    {
        public string RoomType { get; set; }
        public double Price { get; set; }
    }

    // Factory Method: Interfaz para la fábrica de habitaciones
    public interface IRoomFactory
    {
        Room CreateRoom();
    }

    // Factories concretas para diferentes tipos de habitaciones
    public class SingleRoomFactory : IRoomFactory
    {
        public Room CreateRoom()
        {
            return new Room { RoomType = "Single", Price = 100 };
        }
    }

    public class DoubleRoomFactory : IRoomFactory
    {
        public Room CreateRoom()
        {
            return new Room { RoomType = "Double", Price = 200 };
        }
    }

    // Strategy: Interfaz para definir estrategias de reserva
    public interface IReservationStrategy
    {
        void Reserve(Hotel hotel, string roomType, DateTime startDate, DateTime endDate);
    }

    // Estrategia de reserva estándar
    public class StandardReservationStrategy : IReservationStrategy
    {
        public void Reserve(Hotel hotel, string roomType, DateTime startDate, DateTime endDate)
        {
            var room = hotel.Rooms.Find(r => r.RoomType == roomType);
            if (room != null)
            {
                var reservation = new Reservation { Hotel = hotel, Room = room, StartDate = startDate, EndDate = endDate };
                hotel.NotifyObservers(reservation); // Notificar a los observadores
                Console.WriteLine($"Standard Reservation made for {room.RoomType} from {startDate} to {endDate}.");
            }
            else
            {
                Console.WriteLine("Room not available.");
            }
        }
    }

    // Estrategia de reserva VIP
    public class VIPReservationStrategy : IReservationStrategy
    {
        public void Reserve(Hotel hotel, string roomType, DateTime startDate, DateTime endDate)
        {
            var room = hotel.Rooms.Find(r => r.RoomType == roomType);
            if (room != null)
            {
                var reservation = new Reservation { Hotel = hotel, Room = room, StartDate = startDate, EndDate = endDate };
                hotel.NotifyObservers(reservation); // Notificar a los observadores
                Console.WriteLine($"VIP Reservation made for {room.RoomType} with exclusive benefits from {startDate} to {endDate}.");
            }
            else
            {
                Console.WriteLine("VIP Room not available.");
            }
        }
    }

    // Clase Reservation
    public class Reservation
    {
        public Hotel Hotel { get; set; }
        public Room Room { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Singleton: Obtención de la instancia única del hotel
            var hotel = Hotel.Instance;

            // Factory Method: Crear habitaciones usando fábricas
            hotel.AddRoom(new SingleRoomFactory());
            hotel.AddRoom(new DoubleRoomFactory());

            // Observer: Crear servicio de limpieza y suscribirlo a las notificaciones
            var cleaningService = new CleaningService();
            hotel.Subscribe(cleaningService);

            // Strategy: Realizar una reserva estándar
            IReservationStrategy standardStrategy = new StandardReservationStrategy();
            standardStrategy.Reserve(hotel, "Single", DateTime.Now, DateTime.Now.AddDays(2));

            // Strategy: Realizar una reserva VIP
            IReservationStrategy vipStrategy = new VIPReservationStrategy();
            vipStrategy.Reserve(hotel, "Double", DateTime.Now, DateTime.Now.AddDays(3));

            // Imprimir habitaciones disponibles
            hotel.PrintAvailableRooms();
        }
    }
}
