using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Rider table. UI only - all data access goes through <see cref="RiderService"/>.</summary>
    public class RiderForm : BaseCrudForm
    {
        private readonly RiderService _service = new RiderService();

        private TextBox txtEmployeeNo;
        private TextBox txtFirstName;
        private TextBox txtMiddleName;
        private TextBox txtLastName;
        private TextBox txtNic;
        private DateTimePicker dtpDob;
        private TextBox txtContact;
        private TextBox txtLicenseNo;
        private TextBox txtAddress;
        private TextBox txtAge;

        public RiderForm() : base("Rider Management", 270)
        {
            BuildFields();
            WireEvents();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Employee No:", leftX, row1));
            txtEmployeeNo = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtEmployeeNo);

            FieldsPanel.Controls.Add(AddLabel("First Name:", rightX, row1));
            txtFirstName = new TextBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtFirstName);

            FieldsPanel.Controls.Add(AddLabel("Middle Name:", leftX, row1 + rowH));
            txtMiddleName = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtMiddleName);

            FieldsPanel.Controls.Add(AddLabel("Last Name:", rightX, row1 + rowH));
            txtLastName = new TextBox { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtLastName);

            FieldsPanel.Controls.Add(AddLabel("NIC:", leftX, row1 + rowH * 2));
            txtNic = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 2), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtNic);

            FieldsPanel.Controls.Add(AddLabel("Date Of Birth:", rightX, row1 + rowH * 2));
            dtpDob = new DateTimePicker { Location = new Point(rightX + 150, row1 + rowH * 2), Width = 200, Format = DateTimePickerFormat.Short };
            FieldsPanel.Controls.Add(dtpDob);

            FieldsPanel.Controls.Add(AddLabel("Contact Number:", leftX, row1 + rowH * 3));
            txtContact = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 3), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtContact);

            FieldsPanel.Controls.Add(AddLabel("License Number:", rightX, row1 + rowH * 3));
            txtLicenseNo = new TextBox { Location = new Point(rightX + 150, row1 + rowH * 3), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtLicenseNo);

            FieldsPanel.Controls.Add(AddLabel("Address:", leftX, row1 + rowH * 4));
            txtAddress = new TextBox { Location = new Point(leftX + 150, row1 + rowH * 4), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtAddress);

            FieldsPanel.Controls.Add(AddLabel("Age:", rightX, row1 + rowH * 4));
            txtAge = new TextBox { Location = new Point(rightX + 150, row1 + rowH * 4), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtAge);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddRider();
            BtnUpdate.Click += (s, e) => UpdateRider();
            BtnDelete.Click += (s, e) => DeleteRider();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchRider();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
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
                MessageBox.Show("Failed to load riders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Rider rider)) return;

            txtEmployeeNo.Text = rider.EmployeeNo;
            txtFirstName.Text = rider.FirstName;
            txtMiddleName.Text = rider.MiddleName;
            txtLastName.Text = rider.LastName;
            txtNic.Text = rider.NIC;
            txtContact.Text = rider.ContactNumber;
            txtLicenseNo.Text = rider.LicenseNumber;
            txtAddress.Text = rider.Address;
            txtAge.Text = rider.Age?.ToString() ?? "";
            dtpDob.Value = rider.DateOfBirth == default ? DateTime.Now : rider.DateOfBirth;
        }

        private Rider BuildRiderFromFields()
        {
            return new Rider
            {
                EmployeeNo = txtEmployeeNo.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                MiddleName = txtMiddleName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                NIC = txtNic.Text.Trim(),
                DateOfBirth = dtpDob.Value,
                ContactNumber = txtContact.Text.Trim(),
                LicenseNumber = txtLicenseNo.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Age = int.TryParse(txtAge.Text.Trim(), out int age) ? age : (int?)null
            };
        }

        private void AddRider()
        {
            try
            {
                _service.Add(BuildRiderFromFields());
                MessageBox.Show("Rider added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A rider with this Employee No already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add rider: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateRider()
        {
            try
            {
                _service.Update(BuildRiderFromFields());
                MessageBox.Show("Rider updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update rider: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRider()
        {
            if (Validator.IsNullOrEmpty(txtEmployeeNo.Text))
            {
                MessageBox.Show("Select a rider or enter an Employee No to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this rider?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtEmployeeNo.Text.Trim());
                MessageBox.Show("Rider deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This rider cannot be deleted because related dependents exist.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete rider: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single rider by Employee No and displays it in the fields.</summary>
        private void SearchRider()
        {
            if (Validator.IsNullOrEmpty(txtEmployeeNo.Text))
            {
                MessageBox.Show("Enter an Employee No to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Rider rider = _service.Search(txtEmployeeNo.Text.Trim());
                if (rider == null)
                {
                    MessageBox.Show("No rider found with that Employee No.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtFirstName.Text = rider.FirstName;
                txtMiddleName.Text = rider.MiddleName;
                txtLastName.Text = rider.LastName;
                txtNic.Text = rider.NIC;
                txtContact.Text = rider.ContactNumber;
                txtLicenseNo.Text = rider.LicenseNumber;
                txtAddress.Text = rider.Address;
                txtAge.Text = rider.Age?.ToString() ?? "";
                dtpDob.Value = rider.DateOfBirth == default ? DateTime.Now : rider.DateOfBirth;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtEmployeeNo.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();
            txtNic.Clear();
            txtContact.Clear();
            txtLicenseNo.Clear();
            txtAddress.Clear();
            txtAge.Clear();
            dtpDob.Value = DateTime.Now;
            Grid.ClearSelection();
        }
    }
}
