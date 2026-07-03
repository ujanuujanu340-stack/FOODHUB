using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>
    /// CRUD screen for the OrderItem link table (many-to-many between Orders
    /// and FoodItem). UI only - all data access goes through
    /// <see cref="OrderItemService"/>, <see cref="OrderService"/> and
    /// <see cref="FoodItemService"/>.
    /// </summary>
    public class OrderItemForm : BaseCrudForm
    {
        private readonly OrderItemService _service = new OrderItemService();
        private readonly OrderService _orderService = new OrderService();
        private readonly FoodItemService _foodItemService = new FoodItemService();

        private ComboBox cboOrderNo;
        private ComboBox cboItemNo;
        private TextBox txtQuantity;
        private TextBox txtSubTotal;

        // Remembers the composite key of the currently-selected grid row so
        // Update/Delete still target the correct record even if the combo
        // boxes are changed before saving.
        private string _selectedOrderNo;
        private string _selectedItemNo;

        public OrderItemForm() : base("Order Item Management", 90)
        {
            BuildFields();
            WireEvents();
            LoadComboOptions();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Order No:", leftX, row1));
            cboOrderNo = new ComboBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboOrderNo);

            FieldsPanel.Controls.Add(AddLabel("Item No:", rightX, row1));
            cboItemNo = new ComboBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboItemNo);

            FieldsPanel.Controls.Add(AddLabel("Quantity:", leftX, row1 + rowH));
            txtQuantity = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtQuantity);

            FieldsPanel.Controls.Add(AddLabel("Sub Total:", rightX, row1 + rowH));
            txtSubTotal = new TextBox { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtSubTotal);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddOrderItem();
            BtnUpdate.Click += (s, e) => UpdateOrderItem();
            BtnDelete.Click += (s, e) => DeleteOrderItem();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchOrderItem();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
            cboItemNo.SelectedIndexChanged += (s, e) => RecalculateSubTotal();
            txtQuantity.TextChanged += (s, e) => RecalculateSubTotal();
        }

        private void LoadComboOptions()
        {
            try
            {
                cboOrderNo.Items.Clear();
                foreach (Order order in _orderService.GetAll())
                    cboOrderNo.Items.Add(order.OrderNo);

                cboItemNo.Items.Clear();
                foreach (FoodItem item in _foodItemService.GetAll())
                    cboItemNo.Items.Add(item.ItemNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load lookup lists: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Failed to load order items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Recomputes SubTotal via OrderItemService's business rule (Quantity x FoodItem.Price).</summary>
        private void RecalculateSubTotal()
        {
            if (cboItemNo.SelectedItem == null || !Validator.IsPositiveInt(txtQuantity.Text)) return;

            try
            {
                int quantity = int.Parse(txtQuantity.Text.Trim());
                txtSubTotal.Text = _service.CalculateSubTotal(cboItemNo.Text.Trim(), quantity).ToString("0.00");
            }
            catch
            {
                // Ignore auto-calculation errors; the user can still enter SubTotal manually.
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is OrderItem orderItem)) return;

            _selectedOrderNo = orderItem.OrderNo;
            _selectedItemNo = orderItem.ItemNo;

            cboOrderNo.Text = orderItem.OrderNo;
            cboItemNo.Text = orderItem.ItemNo;
            txtQuantity.Text = orderItem.Quantity.ToString();
            txtSubTotal.Text = orderItem.SubTotal.ToString("0.00");
        }

        private bool TryBuildOrderItemFromFields(out OrderItem orderItem)
        {
            orderItem = null;
            if (!Validator.IsPositiveInt(txtQuantity.Text))
            {
                MessageBox.Show("Quantity must be a valid whole number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Validator.IsPositiveDecimal(txtSubTotal.Text))
            {
                MessageBox.Show("Sub Total must be a valid non-negative number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            orderItem = new OrderItem
            {
                OrderNo = cboOrderNo.Text.Trim(),
                ItemNo = cboItemNo.Text.Trim(),
                Quantity = int.Parse(txtQuantity.Text.Trim()),
                SubTotal = decimal.Parse(txtSubTotal.Text.Trim())
            };
            return true;
        }

        private void AddOrderItem()
        {
            if (!TryBuildOrderItemFromFields(out OrderItem orderItem)) return;
            try
            {
                _service.Add(orderItem);
                MessageBox.Show("Order item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE") || ex.Message.Contains("PRIMARY KEY"))
            {
                MessageBox.Show("This Order No + Item No combination already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("The selected Order No or Item No does not exist.", "Invalid Reference", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add order item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateOrderItem()
        {
            if (!TryBuildOrderItemFromFields(out OrderItem orderItem)) return;
            if (Validator.IsNullOrEmpty(_selectedOrderNo) || Validator.IsNullOrEmpty(_selectedItemNo))
            {
                MessageBox.Show("Select a record from the grid to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _service.Update(_selectedOrderNo, _selectedItemNo, orderItem);
                MessageBox.Show("Order item updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update order item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteOrderItem()
        {
            string orderNo = !Validator.IsNullOrEmpty(_selectedOrderNo) ? _selectedOrderNo : cboOrderNo.Text.Trim();
            string itemNo = !Validator.IsNullOrEmpty(_selectedItemNo) ? _selectedItemNo : cboItemNo.Text.Trim();

            if (Validator.IsNullOrEmpty(orderNo) || Validator.IsNullOrEmpty(itemNo))
            {
                MessageBox.Show("Select a record or choose an Order No and Item No to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(orderNo, itemNo);
                MessageBox.Show("Order item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete order item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single order item by Order No + Item No and displays it in the fields.</summary>
        private void SearchOrderItem()
        {
            if (Validator.IsNullOrEmpty(cboOrderNo.Text) || Validator.IsNullOrEmpty(cboItemNo.Text))
            {
                MessageBox.Show("Select an Order No and Item No to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                OrderItem orderItem = _service.Search(cboOrderNo.Text.Trim(), cboItemNo.Text.Trim());
                if (orderItem == null)
                {
                    MessageBox.Show("No matching record found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedOrderNo = orderItem.OrderNo;
                _selectedItemNo = orderItem.ItemNo;
                txtQuantity.Text = orderItem.Quantity.ToString();
                txtSubTotal.Text = orderItem.SubTotal.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            cboOrderNo.SelectedIndex = -1;
            cboItemNo.SelectedIndex = -1;
            txtQuantity.Clear();
            txtSubTotal.Clear();
            _selectedOrderNo = null;
            _selectedItemNo = null;
            Grid.ClearSelection();
        }
    }
}
