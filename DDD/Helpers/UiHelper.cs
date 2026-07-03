using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DDD.Helpers
{
    /// <summary>
    /// Shared colours, fonts and control factory methods so every form in the
    /// application looks consistent without repeating style code everywhere.
    /// </summary>
    public static class UiHelper
    {
        public static readonly Color HeaderColor = ColorTranslator.FromHtml("#2C3E50");
        public static readonly Color AccentColor = ColorTranslator.FromHtml("#2980B9");
        public static readonly Color BackgroundColor = ColorTranslator.FromHtml("#F5F6FA");
        public static readonly Color PanelColor = Color.White;

        public static readonly Color AddColor = ColorTranslator.FromHtml("#27AE60");
        public static readonly Color UpdateColor = ColorTranslator.FromHtml("#E67E22");
        public static readonly Color DeleteColor = ColorTranslator.FromHtml("#C0392B");
        public static readonly Color ClearColor = ColorTranslator.FromHtml("#7F8C8D");
        public static readonly Color SearchColor = ColorTranslator.FromHtml("#2980B9");
        public static readonly Color BackButtonColor = ColorTranslator.FromHtml("#34495E");

        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font LabelFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font InputFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        /// <summary>Creates a flat, coloured push button with white text.</summary>
        public static Button CreateButton(string text, Color backColor, EventHandler onClick)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = ButtonFont,
                FlatStyle = FlatStyle.Flat,
                Width = 110,
                Height = 36,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            if (onClick != null) button.Click += onClick;
            return button;
        }

        /// <summary>Creates a left-aligned field label used in the "label left / input right" layout.</summary>
        public static Label CreateFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = LabelFont,
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Location = new Point(x, y),
                Size = new Size(140, 22),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        /// <summary>Styles a DataGridView consistently: read-only, full row select, alternating rows.</summary>
        public static void StyleGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Font = new Font("Segoe UI", 9F);
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.EnableHeadersVisualStyles = false;
            grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EAF0F6");
            grid.RowTemplate.Height = 26;
        }

        /// <summary>
        /// Turns PascalCase model property names ("CustomerID", "DateOfBirth")
        /// into readable column headers ("Customer ID", "Date Of Birth") once
        /// a grid has been bound to a List&lt;T&gt; of model objects.
        /// </summary>
        public static void ApplyFriendlyHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderText = Regex.Replace(column.HeaderText, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
            }
        }
    }
}
