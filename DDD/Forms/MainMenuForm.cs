using System;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;

namespace DDD.Forms
{
    /// <summary>
    /// Dashboard shown after a successful login. Each tile opens one module
    /// form modally; the dashboard hides itself while a module is open and
    /// reappears once the module form is closed.
    /// </summary>
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "FoodHub Delivery Management System - Main Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 680);
            MinimumSize = new Size(820, 560);
            BackColor = UiHelper.BackgroundColor;
            Font = new Font("Segoe UI", 9F);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = UiHelper.HeaderColor };
            var lblTitle = new Label
            {
                Text = "FoodHub Delivery Management System",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };
            headerPanel.Controls.Add(lblTitle);

            var footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = UiHelper.PanelColor };
            var lblFooter = new Label
            {
                Text = "Select a module to manage its records",
                Dock = DockStyle.Fill,
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };
            footerPanel.Controls.Add(lblFooter);

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                Padding = new Padding(30),
                BackColor = UiHelper.BackgroundColor
            };
            for (int c = 0; c < 3; c++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            for (int r = 0; r < 4; r++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 4));

            AddTile(grid, "Customer\nManagement", "#2980B9", (s, e) => OpenModule(new CustomerForm()));
            AddTile(grid, "Order\nManagement", "#8E44AD", (s, e) => OpenModule(new OrderForm()));
            AddTile(grid, "Food Item\nManagement", "#D35400", (s, e) => OpenModule(new FoodItemForm()));
            AddTile(grid, "Ingredient\nManagement", "#16A085", (s, e) => OpenModule(new IngredientForm()));
            AddTile(grid, "Rider\nManagement", "#2C3E50", (s, e) => OpenModule(new RiderForm()));
            AddTile(grid, "Dependent\nManagement", "#C0392B", (s, e) => OpenModule(new DependentForm()));
            AddTile(grid, "Motorbike\nManagement", "#27AE60", (s, e) => OpenModule(new MotorbikeForm()));
            AddTile(grid, "Theme / Colour\nManagement", "#F39C12", (s, e) => OpenModule(new ThemeForm()));
            AddTile(grid, "Order Item\nManagement", "#7F8C8D", (s, e) => OpenModule(new OrderItemForm()));
            AddTile(grid, "Item Ingredient\nManagement", "#2980B9", (s, e) => OpenModule(new ItemIngredientForm()));
            AddTile(grid, "Reports", "#8E44AD", (s, e) => OpenModule(new ReportsForm()));
            AddTile(grid, "Exit", "#34495E", (s, e) => ConfirmExit());

            Controls.Add(grid);
            Controls.Add(footerPanel);
            Controls.Add(headerPanel);
        }

        private void AddTile(TableLayoutPanel grid, string text, string htmlColor, EventHandler onClick)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(12),
                BackColor = ColorTranslator.FromHtml(htmlColor),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += onClick;
            grid.Controls.Add(button);
        }

        /// <summary>Hides the dashboard, shows the requested module modally, then restores the dashboard.</summary>
        private void OpenModule(Form moduleForm)
        {
            using (moduleForm)
            {
                Hide();
                moduleForm.ShowDialog();
                Show();
            }
        }

        private void ConfirmExit()
        {
            var result = MessageBox.Show("Are you sure you want to exit FoodHub?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
