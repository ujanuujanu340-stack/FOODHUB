-- ============================================================================
-- FoodHub Delivery Management System - Database Schema (SQLite dialect)
-- This script is for reference only. The application automatically creates
-- these tables at startup (see DatabaseHelper.InitializeDatabase). You can
-- also run this script manually against FoodHub.db using any SQLite tool.
-- ============================================================================

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
);
