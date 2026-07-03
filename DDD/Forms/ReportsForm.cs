using System;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>
    /// Simple management reports: headline totals plus a few pre-built
    /// queries the user can run and view in a grid. UI only - all data
    /// access goes through <see cref="ReportService"/>.
    /// </summary>
    public class ReportsForm : Form
    {
        private readonly ReportService _service = new ReportService();

        private Label lblTotalCustomers;
        private Label lblTotalOrders;
        private Label lblTotalRevenue;
        private Label lblTotalRiders;
        private ComboBox cboReportType;
        private DataGridView grid;

        public ReportsForm()
        {
            BuildUi();
            LoadSummary();
        }

        private void BuildUi()
        {
            Text = "Reports";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 680);
            MinimumSize = new Size(820, 560);
            BackColor = UiHelper.BackgroundColor;
            Font = new Font("Segoe UI", 9F);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = UiHelper.HeaderColor };
            var lblTitle = new Label
            {
                Text = "Reports",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = UiHelper.TitleFont,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            headerPanel.Controls.Add(lblTitle);

            var statsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                ColumnCount = 4,
                Padding = new Padding(20, 10, 20, 10),
                BackColor = UiHelper.BackgroundColor
            };
            for (int i = 0; i < 4; i++) statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            lblTotalCustomers = CreateStatTile(statsPanel, "Total Customers", "#2980B9");
            lblTotalOrders = CreateStatTile(statsPanel, "Total Orders", "#8E44AD");
            lblTotalRevenue = CreateStatTile(statsPanel, "Total Revenue", "#27AE60");
            lblTotalRiders = CreateStatTile(statsPanel, "Total Riders", "#D35400");

            var toolPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = UiHelper.PanelColor };
            var lblReport = new Label { Text = "Report:", Location = new Point(20, 18), Size = new Size(60, 24), Font = UiHelper.LabelFont };
            cboReportType = new ComboBox
            {
                Location = new Point(85, 15),
                Width = 320,
                Font = UiHelper.InputFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboReportType.Items.AddRange(new object[]
            {
                "All Orders (with Customer Name)",
                "Orders by Status",
                "Revenue by Payment Method",
                "Food Item Sales Summary"
            });
            cboReportType.SelectedIndex = 0;

            var btnRun = UiHelper.CreateButton("Run Report", UiHelper.AccentColor, (s, e) => RunReport());
            btnRun.Location = new Point(420, 12);

            var btnBack = UiHelper.CreateButton("Back", UiHelper.BackButtonColor, (s, e) => Close());
            btnBack.Location = new Point(540, 12);

            toolPanel.Controls.Add(lblReport);
            toolPanel.Controls.Add(cboReportType);
            toolPanel.Controls.Add(btnRun);
            toolPanel.Controls.Add(btnBack);

            grid = new DataGridView();
            UiHelper.StyleGrid(grid);
            var gridWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 10, 20, 20), BackColor = UiHelper.BackgroundColor };
            gridWrapper.Controls.Add(grid);

            Controls.Add(gridWrapper);
            Controls.Add(toolPanel);
            Controls.Add(statsPanel);
            Controls.Add(headerPanel);
        }

        private Label CreateStatTile(TableLayoutPanel parent, string caption, string htmlColor)
        {
            var tile = new Panel { Dock = DockStyle.Fill, Margin = new Padding(6), BackColor = ColorTranslator.FromHtml(htmlColor) };
            var lblValue = new Label
            {
                Text = "0",
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomCenter
            };
            var lblCaption = new Label
            {
                Text = caption,
                Dock = DockStyle.Top,
                Height = 24,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.TopCenter
            };
            tile.Controls.Add(lblCaption);
            tile.Controls.Add(lblValue);
            parent.Controls.Add(tile);
            return lblValue;
        }

        private void LoadSummary()
        {
            try
            {
                lblTotalCustomers.Text = _service.GetTotalCustomers().ToString();
                lblTotalOrders.Text = _service.GetTotalOrders().ToString();
                lblTotalRiders.Text = _service.GetTotalRiders().ToString();
                lblTotalRevenue.Text = _service.GetTotalRevenue().ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load report summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunReport()
        {
            try
            {
                switch (cboReportType.SelectedIndex)
                {
                    case 0:
                        grid.DataSource = _service.GetAllOrdersWithCustomerName();
                        break;

                    case 1:
                        grid.DataSource = _service.GetOrdersByStatus();
                        break;

                    case 2:
                        grid.DataSource = _service.GetRevenueByPaymentMethod();
                        break;

                    case 3:
                        grid.DataSource = _service.GetFoodItemSalesSummary();
                        break;
                }

                LoadSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
