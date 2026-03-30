using System.Data;
using Microsoft.Data.Sqlite;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string dbPath = "stadium.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    FullName TEXT,
                    IsActive INTEGER DEFAULT 1,
                    Notes TEXT
                );

                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Phone TEXT UNIQUE,
                    Email TEXT,
                    Address TEXT,
                    Type TEXT DEFAULT 'Regular',
                    RegistrationDate TEXT,
                    TotalBookings INTEGER DEFAULT 0,
                    TotalSpent REAL DEFAULT 0,
                    LastBooking TEXT,
                    Notes TEXT,
                    IsActive INTEGER DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Bookings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookingNumber TEXT UNIQUE,
                    BookingDate TEXT,
                    Stadium TEXT,
                    StartHour INTEGER,
                    EndHour INTEGER,
                    Duration INTEGER,
                    TimeSlot TEXT,
                    CustomerId INTEGER,
                    CustomerName TEXT,
                    CustomerPhone TEXT,
                    Status TEXT,
                    TotalPrice REAL,
                    Deposit REAL,
                    Balance REAL,
                    PaymentMethod TEXT,
                    PaymentStatus TEXT,
                    Notes TEXT,
                    CreatedAt TEXT,
                    FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
                );

                CREATE TABLE IF NOT EXISTS Settings (
                    Key TEXT PRIMARY KEY,
                    Value TEXT
                );

                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Category TEXT,
                    Amount REAL NOT NULL,
                    Date TEXT NOT NULL,
                    Notes TEXT
                );

                CREATE TABLE IF NOT EXISTS Stadiums (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    HourlyPrice REAL NOT NULL,
                    IsActive INTEGER DEFAULT 1,
                    Notes TEXT
                );

                INSERT OR IGNORE INTO Users (Username, Password, Role, FullName) 
                VALUES ('admin', '1234', 'Admin', 'System Administrator');

                -- Default settings
                INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('OrganizationName', 'Jeel Al Bena Association');
                INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('Stadium1Price', '8000');
                INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('Stadium2Price', '8000');
                INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('ManagerName', 'Bilal Al Salami');
                INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('Location', 'Sana''a, Yemen');

                -- Default Stadiums
                INSERT OR IGNORE INTO Stadiums (Name, HourlyPrice) VALUES ('Stadium 1', 8000);
                INSERT OR IGNORE INTO Stadiums (Name, HourlyPrice) VALUES ('Stadium 2', 8000);
            ";
            command.ExecuteNonQuery();
        }

        public Settings GetSettings()
        {
            var settings = new Settings();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Key, Value FROM Settings";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var key = reader.GetString(0);
                var val = reader.GetString(1);
                switch (key)
                {
                    case "OrganizationName": settings.OrganizationName = val; break;
                    case "ManagerName": settings.ManagerName = val; break;
                    case "Location": settings.Location = val; break;
                    case "Stadium1Price": settings.Stadium1Price = decimal.Parse(val); break;
                    case "Stadium2Price": settings.Stadium2Price = decimal.Parse(val); break;
                    case "LogoPath": settings.LogoPath = val; break;
                }
            }
            return settings;
        }

        public void SaveSettings(Settings settings)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('OrganizationName', @org);
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('ManagerName', @man);
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('Location', @loc);
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('Stadium1Price', @s1);
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('Stadium2Price', @s2);
                INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('LogoPath', @logo);
            ";
            command.Parameters.AddWithValue("@org", settings.OrganizationName);
            command.Parameters.AddWithValue("@man", settings.ManagerName);
            command.Parameters.AddWithValue("@loc", settings.Location);
            command.Parameters.AddWithValue("@s1", settings.Stadium1Price.ToString());
            command.Parameters.AddWithValue("@s2", settings.Stadium2Price.ToString());
            command.Parameters.AddWithValue("@logo", settings.LogoPath ?? "");
            command.ExecuteNonQuery();
        }

        public bool CheckAvailability(string stadium, DateTime date, int start, int end)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) FROM Bookings 
                WHERE Stadium = @s AND BookingDate = @d
                AND StartHour <= @end AND EndHour >= @start
            ";
            command.Parameters.AddWithValue("@s", stadium);
            command.Parameters.AddWithValue("@d", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            return Convert.ToInt32(command.ExecuteScalar()) == 0;
        }

        public User? Authenticate(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Role, FullName FROM Users WHERE Username = @u AND Password = @p AND IsActive = 1";
            command.Parameters.AddWithValue("@u", username);
            command.Parameters.AddWithValue("@p", password);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Role = reader.GetString(2),
                    FullName = reader.IsDBNull(3) ? "" : reader.GetString(3)
                };
            }
            return null;
        }

        public void SaveBooking(Booking booking)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Ensure customer exists or update
                var cmdCust = connection.CreateCommand();
                cmdCust.Transaction = transaction;
                cmdCust.CommandText = @"
                    INSERT INTO Customers (Name, Phone, RegistrationDate, TotalBookings, TotalSpent, LastBooking)
                    VALUES (@name, @phone, @date, 1, @spent, @date)
                    ON CONFLICT(Phone) DO UPDATE SET
                        TotalBookings = TotalBookings + 1,
                        TotalSpent = TotalSpent + excluded.TotalSpent,
                        LastBooking = excluded.LastBooking;
                    SELECT Id FROM Customers WHERE Phone = @phone;
                ";
                cmdCust.Parameters.AddWithValue("@name", booking.CustomerName);
                cmdCust.Parameters.AddWithValue("@phone", booking.CustomerPhone);
                cmdCust.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                cmdCust.Parameters.AddWithValue("@spent", booking.TotalPrice);
                var customerId = cmdCust.ExecuteScalar();
                booking.CustomerId = Convert.ToInt32(customerId);

                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO Bookings (BookingNumber, BookingDate, Stadium, StartHour, EndHour, Duration, TimeSlot, 
                                        CustomerId, CustomerName, CustomerPhone, Status, TotalPrice, Deposit, Balance, 
                                        PaymentMethod, PaymentStatus, Notes, CreatedAt)
                    VALUES (@bn, @bd, @s, @sh, @eh, @d, @ts, @cid, @cn, @cp, @status, @tp, @dep, @bal, @pm, @ps, @n, @ca)
                ";
                command.Parameters.AddWithValue("@bn", booking.BookingNumber);
                command.Parameters.AddWithValue("@bd", booking.BookingDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@s", booking.Stadium);
                command.Parameters.AddWithValue("@sh", booking.StartHour);
                command.Parameters.AddWithValue("@eh", booking.EndHour);
                command.Parameters.AddWithValue("@d", booking.Duration);
                command.Parameters.AddWithValue("@ts", booking.TimeSlot);
                command.Parameters.AddWithValue("@cid", booking.CustomerId);
                command.Parameters.AddWithValue("@cn", booking.CustomerName);
                command.Parameters.AddWithValue("@cp", booking.CustomerPhone);
                command.Parameters.AddWithValue("@status", booking.Status);
                command.Parameters.AddWithValue("@tp", booking.TotalPrice);
                command.Parameters.AddWithValue("@dep", booking.Deposit);
                command.Parameters.AddWithValue("@bal", booking.Balance);
                command.Parameters.AddWithValue("@pm", booking.PaymentMethod);
                command.Parameters.AddWithValue("@ps", booking.PaymentStatus);
                command.Parameters.AddWithValue("@n", booking.Notes);
                command.Parameters.AddWithValue("@ca", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Booking> GetBookings()
        {
            var list = new List<Booking>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, BookingNumber, BookingDate, Stadium, StartHour, EndHour, Duration, TimeSlot,
                                          CustomerId, CustomerName, CustomerPhone, Status, TotalPrice, Deposit, Balance,
                                          PaymentMethod, PaymentStatus, Notes, CreatedAt
                                   FROM Bookings ORDER BY CreatedAt DESC";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Booking
                {
                    Id = reader.GetInt32(0),
                    BookingNumber = reader.GetString(1),
                    BookingDate = DateTime.Parse(reader.GetString(2)),
                    Stadium = reader.GetString(3),
                    StartHour = reader.GetInt32(4),
                    EndHour = reader.GetInt32(5),
                    Duration = reader.GetInt32(6),
                    TimeSlot = reader.GetString(7),
                    CustomerId = reader.GetInt32(8),
                    CustomerName = reader.GetString(9),
                    CustomerPhone = reader.GetString(10),
                    Status = reader.GetString(11),
                    TotalPrice = reader.GetDecimal(12),
                    Deposit = reader.GetDecimal(13),
                    Balance = reader.GetDecimal(14),
                    PaymentMethod = reader.GetString(15),
                    PaymentStatus = reader.GetString(16),
                    Notes = reader.IsDBNull(17) ? "" : reader.GetString(17),
                    CreatedAt = DateTime.Parse(reader.GetString(18))
                });
            }
            return list;
        }

        public List<Customer> GetCustomers()
        {
            var list = new List<Customer>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Phone, Email, Address, Type, RegistrationDate, TotalBookings, TotalSpent, LastBooking, Notes, IsActive FROM Customers ORDER BY TotalSpent DESC";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Customer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Phone = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Address = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Type = reader.GetString(5),
                    RegistrationDate = DateTime.Parse(reader.GetString(6)),
                    TotalBookings = reader.GetInt32(7),
                    TotalSpent = reader.GetDecimal(8),
                    LastBooking = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9)),
                    Notes = reader.IsDBNull(10) ? "" : reader.GetString(10),
                    IsActive = reader.GetInt32(11) == 1
                });
            }
            return list;
        }

        public void SaveExpense(Expense expense)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Expenses (Description, Category, Amount, Date, Notes)
                VALUES (@desc, @cat, @amt, @date, @notes)
            ";
            command.Parameters.AddWithValue("@desc", expense.Description);
            command.Parameters.AddWithValue("@cat", expense.Category);
            command.Parameters.AddWithValue("@amt", expense.Amount);
            command.Parameters.AddWithValue("@date", expense.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@notes", expense.Notes);
            command.ExecuteNonQuery();
        }

        public List<Expense> GetExpenses(DateTime start, DateTime end)
        {
            var list = new List<Expense>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Description, Category, Amount, Date, Notes FROM Expenses WHERE Date BETWEEN @s AND @e ORDER BY Date DESC";
            command.Parameters.AddWithValue("@s", start.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@e", end.ToString("yyyy-MM-dd"));
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Expense
                {
                    Id = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    Category = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    Date = DateTime.Parse(reader.GetString(4)),
                    Notes = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }
            return list;
        }

        public void DeleteExpense(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Expenses WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<Stadium> GetStadiums()
        {
            var list = new List<Stadium>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, HourlyPrice, IsActive, Notes FROM Stadiums";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Stadium
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    HourlyPrice = reader.GetDecimal(2),
                    IsActive = reader.GetInt32(3) == 1,
                    Notes = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }
            return list;
        }

        public void SaveStadium(Stadium stadium)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Stadiums (Id, Name, HourlyPrice, IsActive, Notes)
                VALUES (@id, @name, @price, @active, @notes)
                ON CONFLICT(Id) DO UPDATE SET
                    Name = excluded.Name,
                    HourlyPrice = excluded.HourlyPrice,
                    IsActive = excluded.IsActive,
                    Notes = excluded.Notes;
            ";
            command.Parameters.AddWithValue("@id", stadium.Id == 0 ? (object)DBNull.Value : stadium.Id);
            command.Parameters.AddWithValue("@name", stadium.Name);
            command.Parameters.AddWithValue("@price", stadium.HourlyPrice);
            command.Parameters.AddWithValue("@active", stadium.IsActive ? 1 : 0);
            command.Parameters.AddWithValue("@notes", stadium.Notes ?? "");
            command.ExecuteNonQuery();
        }

        public void DeleteStadium(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Stadiums WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<User> GetUsers()
        {
            var list = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Role, FullName, IsActive, Notes FROM Users";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Role = reader.GetString(2),
                    FullName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    IsActive = reader.GetInt32(4) == 1,
                    Notes = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }
            return list;
        }

        public void SaveUser(User user, string? password = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            if (user.Id == 0)
            {
                command.CommandText = @"
                    INSERT INTO Users (Username, Password, Role, FullName, IsActive, Notes)
                    VALUES (@u, @p, @r, @f, @a, @n)
                ";
                command.Parameters.AddWithValue("@p", password ?? "1234");
            }
            else
            {
                command.CommandText = @"
                    UPDATE Users SET Username = @u, Role = @r, FullName = @f, IsActive = @a, Notes = @n
                    " + (password != null ? ", Password = @p" : "") + @"
                    WHERE Id = @id
                ";
                command.Parameters.AddWithValue("@id", user.Id);
                if (password != null) command.Parameters.AddWithValue("@p", password);
            }
            command.Parameters.AddWithValue("@u", user.Username);
            command.Parameters.AddWithValue("@r", user.Role);
            command.Parameters.AddWithValue("@f", user.FullName ?? "");
            command.Parameters.AddWithValue("@a", user.IsActive ? 1 : 0);
            command.Parameters.AddWithValue("@n", user.Notes ?? "");
            command.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @id AND Username != 'admin'";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public void UpdateBookingPayment(int bookingId, decimal deposit, decimal balance, string status)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE Bookings SET Deposit = @d, Balance = @b, PaymentStatus = @s WHERE Id = @id";
            command.Parameters.AddWithValue("@d", deposit);
            command.Parameters.AddWithValue("@b", balance);
            command.Parameters.AddWithValue("@s", status);
            command.Parameters.AddWithValue("@id", bookingId);
            command.ExecuteNonQuery();
        }
    }
}
