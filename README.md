# FoodHub Delivery Management System

This project is a Windows Forms Application developed using C# and Object-Oriented Programming concepts.  
The system is designed for FoodHub Company to manage customers, orders, food items, ingredients, riders, dependents, motorbikes, themes/colours, order items, and item ingredients.

## Project Type

- Windows Forms App
- C#
- Visual Studio
- SQL Server LocalDB / SQLite
- Object-Oriented Programming

## Main Features

- Login Form
- Main Menu / Dashboard
- Customer Management
- Order Management
- Food Item Management
- Ingredient Management
- Rider Management
- Dependent Management
- Motorbike Management
- Theme / Colour Management
- Order Item Management
- Item Ingredient Management
- CRUD Operations: Add, Update, Delete, Search, Clear, View

## OOP Concepts Used

### 1. Classes and Objects
Separate model classes are created for each main entity such as Customer, Order, FoodItem, Rider, Motorbike, and Ingredient.

### 2. Encapsulation
Data is protected using properties with get and set methods inside model classes.

### 3. Inheritance
Common person details are stored in a base `Person` class.  
`Customer` and `Rider` classes inherit from the `Person` class.

### 4. Abstraction
Repository interfaces are used to define common CRUD operations.

### 5. Polymorphism
Different repository classes implement the same CRUD interface with their own logic.

## Project Structure

```text
FoodHubProject/
 ├── Forms/
 ├── Models/
 ├── Repositories/
 ├── Services/
 ├── Database/
 ├── Helpers/
 ├── Program.cs
 ├── App.config
 └── DatabaseScript.sql# FOODHUB
FoodHub Delivery Management System developed using C# Windows Forms, OOP concepts, CRUD operations, and SQL database.
