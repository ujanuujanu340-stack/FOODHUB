using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Orders table. UI only - all data access goes through <see cref="OrderService"/> and <see cref="CustomerService"/>.</summary>
    public class OrderForm : BaseCrudForm
    {
        private readonly OrderService _service = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();

        private TextBox txtOrderNo;
        private ComboBox cboCustomerId;
        private DateTimePicker dtpOrderDate;
        private DateTimePicker dtpOrderTime;
        private ComboBox cboOrderStatus;
        private ComboBox cboPaymentMethod;
        private TextBox txtOrderAmount;
        private DateTimePicker dtpDispatchedTime;

        public OrderForm() : base("Order Management", 200)
        {
            BuildFields();
            WireEvents();
            LoadCustomerOptions();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Order No:", leftX, row1));
            txtOrderNo = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtOrderNo);

            FieldsPanel.Controls.Add(AddLabel("Customer ID:", rightX, row1));
            cboCustomerId = new ComboBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboCustomerId);

            FieldsPanel.Controls.Add(AddLabel("Order Date:", leftX, row1 + rowH));
            dtpOrderDate = new DateTimePicker { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Format = DateTimePickerFormat.Short };
            FieldsPanel.Controls.Add(dtpOrderDate);

            FieldsPanel.Controls.Add(AddLabel("Order Time:", rightX, row1 + rowH));
            dtpOrderTime = new DateTimePicker { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Format = DateTimePickerFormat.Time, ShowUpDown = true };
            FieldsPanel.Controls.Add(dtpOrderTime);

            FieldsPanel.Controls.Add(AddLabel("Order Status:", leftX, row1 + rowH * 2));
            cboOrderStatus = new ComboBox { Location = new Point(leftX + 150, row1 + rowH * 2), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboOrderStatus.Items.AddRange(new object[] { "Pending", "Preparing", "Dispatched", "Delivered", "Cancelled" });
            FieldsPanel.Controls.Add(cboOrderStatus);

            FieldsPanel.Controls.Add(AddLabel("Payment Method:", rightX, row1 + rowH * 2));
            cboPaymentMethod = new ComboBox { Location = new Point(rightX + 150, row1 + rowH * 2), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Online" });
            FieldsPanel.Controls.Add(cboPaymentMethod);

            FieldsPanel.Controls.Add(AddLabel("Order Amount:", leftX, row1 + rowH * 3));
            txtOrderAmount = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 3), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtOrderAmount);

            FieldsPanel.Controls.Add(AddLabel("Dispatched Time:", rightX, row1 + rowH * 3));
            dtpDispatchedTime = new DateTimePicker
            {
                Location = new Point(rightX + 150, row1 + rowH * 3),
                Width = 200,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                ShowCheckBox = true,
                Checked = false
            };
            FieldsPanel.Controls.Add(dtpDispatchedTime);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddOrder();
            BtnUpdate.Click += (s, e) => UpdateOrder();
            BtnDelete.Click += (s, e) => DeleteOrder();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchOrder();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
        }

        /// <summary>Fills the Customer ID combo box with the current list of customers (foreign key selection).</summary>
        private void LoadCustomerOptions()
        {
            try
            {
                cboCustomerId.Items.Clear();
                foreach (Customer customer in _customerService.GetAll())
                    cboCustomerId.Items.Add(customer.CustomerID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customer list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                Grid.DataSource = _service.GetAll();
                UiHelper.ApplyFriendlyHeaders(Grid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Order order)) return;

            txtOrderNo.Text = order.OrderNo;
            cboCustomerId.Text = order.CustomerID;
            cboOrderStatus.Text = order.OrderStatus;
            cboPaymentMethod.Text = order.PaymentMethod;
            txtOrderAmount.Text = order.OrderAmount.ToString("0.00");
            dtpOrderDate.Value = order.OrderDate == default ? DateTime.Now : order.OrderDate;
            dtpOrderTime.Value = DateTime.Today.Add(order.OrderTime);

            if (order.DispatchedTime.HasValue)
            {
                dtpDispatchedTime.Checked = true;
                dtpDispatchedTime.Value = DateTime.Today.Add(order.DispatchedTime.Value);
            }
            else
            {
                dtpDispatchedTime.Checked = false;
            }
        }

        private bool TryBuildOrderFromFields(out Order order)
        {
            order = null;
            if (!Validator.IsPositiveDecimal(txtOrderAmount.Text))
            {
                MessageBox.Show("Order Amount must be a valid non-negative number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            order = new Order
            {
                OrderNo = txtOrderNo.Text.Trim(),
                CustomerID = cboCustomerId.Text.Trim(),
                OrderDate = dtpOrderDate.Value,
                OrderTime = dtpOrderTime.Value.TimeOfDay,
                OrderStatus = cboOrderStatus.Text.Trim(),
                PaymentMethod = cboPaymentMethod.Text.Trim(),
                OrderAmount = decimal.Parse(txtOrderAmount.Text.Trim()),
                DispatchedTime = dtpDispatchedTime.Checked ? dtpDispatchedTime.Value.TimeOfDay : (TimeSpan?)null
            };
            return true;
        }

        private void AddOrder()
        {
            if (!TryBuildOrderFromFields(out Order order)) return;
            try
            {
                _service.Add(order);
                MessageBox.Show("Order added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("An order with this Order No already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("The selected Customer ID does not exist.", "Invalid Reference", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateOrder()
        {
            if (!TryBuildOrderFromFields(out Order order)) return;
            try
            {
                _service.Update(order);
                MessageBox.Show("Order updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteOrder()
        {
            if (Validator.IsNullOrEmpty(txtOrderNo.Text))
            {
                MessageBox.Show("Select an order or enter an Order No to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtOrderNo.Text.Trim());
                MessageBox.Show("Order deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This order cannot be deleted because related order items exist.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single order by Order No and displays it in the fields.</summary>
        private void SearchOrder()
        {
            if (Validator.IsNullOrEmpty(txtOrderNo.Text))
            {
                MessageBox.Show("Enter an Order No to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Order order = _service.Search(txtOrderNo.Text.Trim());
                if (order == null)
                {
                    MessageBox.Show("No order found with that Order No.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cboCustomerId.Text = order.CustomerID;
                cboOrderStatus.Text = order.OrderStatus;
                cboPaymentMethod.Text = order.PaymentMethod;
                txtOrderAmount.Text = order.OrderAmount.ToString("0.00");
                dtpOrderDate.Value = order.OrderDate == default ? DateTime.Now : order.OrderDate;
                dtpOrderTime.Value = DateTime.Today.Add(order.OrderTime);

                if (order.DispatchedTime.HasValue)
                {
                    dtpDispatchedTime.Checked = true;
                    dtpDispatchedTime.Value = DateTime.Today.Add(order.DispatchedTime.Value);
                }
                else
                {
                    dtpDispatchedTime.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtOrderNo.Clear();
            cboCustomerId.SelectedIndex = -1;
            cboOrderStatus.SelectedIndex = -1;
            cboPaymentMethod.SelectedIndex = -1;
            txtOrderAmount.Clear();
            dtpOrderDate.Value = DateTime.Now;
            dtpOrderTime.Value = DateTime.Now;
            dtpDispatchedTime.Checked = false;
            dtpDispatchedTime.Value = DateTime.Now;
            Grid.ClearSelection();
        }
    }
}
