using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DDD.Database
{
    /// <summary>
    /// The ONLY class in the whole application that opens a SQLite
    /// connection. Every repository calls into this class instead of
    /// creating its own SqliteConnection, so connection handling and
    /// low-level error handling live in exactly one place.
    /// </summary>
    public static class DatabaseConnection
    {
        private static readonly string DbFileName = "FoodHub.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;Foreign Keys=True;";

        /// <summary>
        /// Creates the database file (if missing) and all tables (if missing).
        /// Called once on application startup.
        /// </summary>
        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            string createTablesSql = @"
                CREATE TABLE IF NOT EXISTS Customer (
                    CustomerID      TEXT PRIMARY KEY,
                    CustomerName    TEXT NOT NULL,
                    NIC             TEXT,
                    DateOfBirth     TEXT,
                    ContactNumber   TEXT,
                    LocationNo      TEXT,
                    Lane            TEXT,
                    Street          TEXT,
                    City            TEXT
                );

                CREATE TABLE IF NOT EXISTS Rider (
                    EmployeeNo      TEXT PRIMARY KEY,
                    FirstName       TEXT NOT NULL,
                    MiddleName      TEXT,
                    LastName        TEXT,
                    NIC             TEXT,
                    DateOfBirth     TEXT,
                    ContactNumber   TEXT,
                    LicenseNumber   TEXT,
                    Address         TEXT,
                    Age             INTEGER
                );

                CREATE TABLE IF NOT EXISTS Dependent (
                    DependentID     TEXT PRIMARY KEY,
                    EmployeeNo      TEXT NOT NULL,
                    DependentName   TEXT NOT NULL,
                    Relationship    TEXT,
                    DateOfBirth     TEXT,
                    FOREIGN KEY (EmployeeNo) REFERENCES Rider(EmployeeNo) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Motorbike (
                    VehicleRegNo    TEXT PRIMARY KEY,
                    Brand           TEXT NOT NULL,
                    Model           TEXT,
                    EngineNo        TEXT,
                    RegisteredDate  TEXT
                );

                CREATE TABLE IF NOT EXISTS Theme (
                    ThemeID         TEXT PRIMARY KEY,
                    VehicleRegNo    TEXT NOT NULL,
                    ColourName      TEXT,
                    FOREIGN KEY (VehicleRegNo) REFERENCES Motorbike(VehicleRegNo) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS FoodItem (
                    ItemNo          TEXT PRIMARY KEY,
                    ItemName        TEXT NOT NULL,
                    ItemCategory    TEXT,
                    Price           REAL
                );

                CREATE TABLE IF NOT EXISTS Ingredient (
                    IngredientID    TEXT PRIMARY KEY,
                    IngredientName  TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ItemIngredient (
                    ItemNo          TEXT NOT NULL,
                    IngredientID    TEXT NOT NULL,
                    Quantity        REAL,
                    PRIMARY KEY (ItemNo, IngredientID),
                    FOREIGN KEY (ItemNo) REFERENCES FoodItem(ItemNo) ON DELETE CASCADE,
                    FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Orders (
                    OrderNo         TEXT PRIMARY KEY,
                    CustomerID      TEXT NOT NULL,
                    OrderDate       TEXT,
                    OrderTime       TEXT,
                    OrderStatus     TEXT,
                    PaymentMethod   TEXT,
                    OrderAmount     REAL,
                    DispatchedTime  TEXT,
                    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS OrderItem (
                    OrderNo         TEXT NOT NULL,
                    ItemNo          TEXT NOT NULL,
                    Quantity        INTEGER,
                    SubTotal        REAL,
                    PRIMARY KEY (OrderNo, ItemNo),
                    FOREIGN KEY (OrderNo) REFERENCES Orders(OrderNo) ON DELETE CASCADE,
                    FOREIGN KEY (ItemNo) REFERENCES FoodItem(ItemNo) ON DELETE CASCADE
                );";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(createTablesSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Runs INSERT/UPDATE/DELETE statements and returns affected row count.</summary>
        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                        command.Parameters.AddRange(parameters);

                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Runs a SELECT statement and returns the results as a DataTable.</summary>
        public static DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            var table = new DataTable();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                        command.Parameters.AddRange(parameters);

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        /// <summary>Runs a SELECT that returns a single value (e.g. COUNT(*), SUM(...)).</summary>
        public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                        command.Parameters.AddRange(parameters);

                    return command.ExecuteScalar();
                }
            }
        }
    }
}
