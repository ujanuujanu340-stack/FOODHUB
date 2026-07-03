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
    /// CRUD screen for the Customer table. This form only builds the UI and
    /// reacts to button clicks - all data access and validation is delegated
    /// to <see cref="CustomerService"/>, keeping "Form logic" and "business
    /// logic" cleanly separated (see requirement 8: Forms = UI only).
    /// </summary>
    public class CustomerForm : BaseCrudForm
    {
        // The service is the only object this form talks to for data access.
        private readonly CustomerService _service = new CustomerService();

        private TextBox txtCustomerId;
        private TextBox txtCustomerName;
        private TextBox txtNic;
        private DateTimePicker dtpDob;
        private TextBox txtContact;
        private TextBox txtLocationNo;
        private TextBox txtLane;
        private TextBox txtStreet;
        private TextBox txtCity;

        public CustomerForm() : base("Customer Management", 230)
        {
            BuildFields();
            WireEvents();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Customer ID:", leftX, row1));
            txtCustomerId = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtCustomerId);

            FieldsPanel.Controls.Add(AddLabel("Customer Name:", rightX, row1));
            txtCustomerName = new TextBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtCustomerName);

            FieldsPanel.Controls.Add(AddLabel("NIC:", leftX, row1 + rowH));
            txtNic = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtNic);

            FieldsPanel.Controls.Add(AddLabel("Date Of Birth:", rightX, row1 + rowH));
            dtpDob = new DateTimePicker { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Format = DateTimePickerFormat.Short };
            FieldsPanel.Controls.Add(dtpDob);

            FieldsPanel.Controls.Add(AddLabel("Contact Number:", leftX, row1 + rowH * 2));
            txtContact = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 2), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtContact);

            FieldsPanel.Controls.Add(AddLabel("Location No:", rightX, row1 + rowH * 2));
            txtLocationNo = new TextBox { Location = new Point(rightX + 150, row1 + rowH * 2), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtLocationNo);

            FieldsPanel.Controls.Add(AddLabel("Lane:", leftX, row1 + rowH * 3));
            txtLane = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 3), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtLane);

            FieldsPanel.Controls.Add(AddLabel("Street:", rightX, row1 + rowH * 3));
            txtStreet = new TextBox { Location = new Point(rightX + 150, row1 + rowH * 3), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtStreet);

            FieldsPanel.Controls.Add(AddLabel("City:", leftX, row1 + rowH * 4));
            txtCity = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 4), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtCity);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddCustomer();
            BtnUpdate.Click += (s, e) => UpdateCustomer();
            BtnDelete.Click += (s, e) => DeleteCustomer();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchCustomer();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
        }

        /// <summary>Loads every customer via the service and binds the grid to the resulting list of Customer objects.</summary>
        private void LoadData()
        {
            try
            {
                Grid.DataSource = _service.GetAll();
                UiHelper.ApplyFriendlyHeaders(Grid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Because the grid is bound to List&lt;Customer&gt;, the selected row's DataBoundItem IS a Customer object - no manual parsing needed.</summary>
        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Customer customer)) return;

            txtCustomerId.Text = customer.CustomerID;
            txtCustomerName.Text = customer.CustomerName;
            txtNic.Text = customer.NIC;
            txtContact.Text = customer.ContactNumber;
            txtLocationNo.Text = customer.LocationNo;
            txtLane.Text = customer.Lane;
            txtStreet.Text = customer.Street;
            txtCity.Text = customer.City;
            dtpDob.Value = customer.DateOfBirth == default ? DateTime.Now : customer.DateOfBirth;
        }

        /// <summary>Builds a Customer object from the current field values.</summary>
        private Customer BuildCustomerFromFields()
        {
            return new Customer
            {
                CustomerID = txtCustomerId.Text.Trim(),
                CustomerName = txtCustomerName.Text.Trim(),
                NIC = txtNic.Text.Trim(),
                DateOfBirth = dtpDob.Value,
                ContactNumber = txtContact.Text.Trim(),
                LocationNo = txtLocationNo.Text.Trim(),
                Lane = txtLane.Text.Trim(),
                Street = txtStreet.Text.Trim(),
                City = txtCity.Text.Trim()
            };
        }

        private void AddCustomer()
        {
            try
            {
                _service.Add(BuildCustomerFromFields());
                MessageBox.Show("Customer added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                // Validation failures raised by CustomerService.
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A customer with this Customer ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCustomer()
        {
            try
            {
                _service.Update(BuildCustomerFromFields());
                MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteCustomer()
        {
            if (Validator.IsNullOrEmpty(txtCustomerId.Text))
            {
                MessageBox.Show("Select a customer or enter a Customer ID to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtCustomerId.Text.Trim());
                MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This customer cannot be deleted because related orders exist.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single customer by Customer ID and displays it in the fields (does not filter the grid).</summary>
        private void SearchCustomer()
        {
            if (Validator.IsNullOrEmpty(txtCustomerId.Text))
            {
                MessageBox.Show("Enter a Customer ID to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Customer customer = _service.Search(txtCustomerId.Text.Trim());
                if (customer == null)
                {
                    MessageBox.Show("No customer found with that Customer ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtCustomerName.Text = customer.CustomerName;
                txtNic.Text = customer.NIC;
                txtContact.Text = customer.ContactNumber;
                txtLocationNo.Text = customer.LocationNo;
                txtLane.Text = customer.Lane;
                txtStreet.Text = customer.Street;
                txtCity.Text = customer.City;
                dtpDob.Value = customer.DateOfBirth == default ? DateTime.Now : customer.DateOfBirth;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtNic.Clear();
            txtContact.Clear();
            txtLocationNo.Clear();
            txtLane.Clear();
            txtStreet.Clear();
            txtCity.Clear();
            dtpDob.Value = DateTime.Now;
            Grid.ClearSelection();
        }
    }
}
