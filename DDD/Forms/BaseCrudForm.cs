using System;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;

namespace DDD.Forms
{
    /// <summary>
    /// Common chrome shared by every module form: a title header, an area for
    /// labelled input fields, a DataGridView that fills the remaining space,
    /// and a row of Add / Update / Delete / Clear / Search / Back buttons.
    /// Concrete forms (CustomerForm, OrderForm, ...) inherit this class, add
    /// their own field controls into <see cref="FieldsPanel"/> and wire up
    /// the button click handlers. The forms themselves only build UI and
    /// call into a Service class - they never talk to the database directly.
    /// </summary>
    public abstract class BaseCrudForm : Form
    {
        protected Panel HeaderPanel { get; }
        protected Label TitleLabel { get; }
        protected Panel FieldsPanel { get; }
        protected DataGridView Grid { get; }
        protected Panel ButtonPanel { get; }

        protected Button BtnAdd { get; }
        protected Button BtnUpdate { get; }
        protected Button BtnDelete { get; }
        protected Button BtnClear { get; }
        protected Button BtnSearch { get; }
        protected Button BtnBack { get; }

        protected BaseCrudForm(string title, int fieldsPanelHeight)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 680);
            MinimumSize = new Size(900, 560);
            BackColor = UiHelper.BackgroundColor;
            Font = new Font("Segoe UI", 9F);

            // --- DataGridView (fills the space between the fields panel and the buttons) ---
            Grid = new DataGridView();
            UiHelper.StyleGrid(Grid);
            var gridWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 10, 20, 10), BackColor = UiHelper.BackgroundColor };
            gridWrapper.Controls.Add(Grid);

            // --- Fields panel (labels on the left, inputs on the right) ---
            FieldsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = fieldsPanelHeight,
                BackColor = UiHelper.PanelColor,
                Padding = new Padding(20)
            };

            // --- Button bar ---
            ButtonPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = UiHelper.PanelColor };
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20, 12, 20, 12),
                WrapContents = false
            };
            BtnAdd = UiHelper.CreateButton("Add", UiHelper.AddColor, null);
            BtnUpdate = UiHelper.CreateButton("Update", UiHelper.UpdateColor, null);
            BtnDelete = UiHelper.CreateButton("Delete", UiHelper.DeleteColor, null);
            BtnClear = UiHelper.CreateButton("Clear", UiHelper.ClearColor, null);
            BtnSearch = UiHelper.CreateButton("Search", UiHelper.SearchColor, null);
            BtnBack = UiHelper.CreateButton("Back", UiHelper.BackButtonColor, (s, e) => Close());
            foreach (var b in new[] { BtnAdd, BtnUpdate, BtnDelete, BtnClear, BtnSearch, BtnBack })
            {
                b.Margin = new Padding(0, 0, 10, 0);
                flow.Controls.Add(b);
            }
            ButtonPanel.Controls.Add(flow);

            // --- Header ---
            HeaderPanel = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = UiHelper.HeaderColor };
            TitleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = UiHelper.TitleFont,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            HeaderPanel.Controls.Add(TitleLabel);

            // Docking is applied in reverse of add-order, so add Fill first, then
            // Top/Bottom pieces, finishing with the outermost header last.
            Controls.Add(gridWrapper);
            Controls.Add(FieldsPanel);
            Controls.Add(ButtonPanel);
            Controls.Add(HeaderPanel);
        }

        /// <summary>Convenience overload used by subclasses to lay out a "Label | Input" row.</summary>
        protected Label AddLabel(string text, int x, int y) => UiHelper.CreateFieldLabel(text, x, y);
    }
}
