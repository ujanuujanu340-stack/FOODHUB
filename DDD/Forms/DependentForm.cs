using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Dependent table. UI only - all data access goes through <see cref="DependentService"/> and <see cref="RiderService"/>.</summary>
    public class DependentForm : BaseCrudForm
    {
        private readonly DependentService _service = new DependentService();
        private readonly RiderService _riderService = new RiderService();

        private TextBox txtDependentId;
        private ComboBox cboEmployeeNo;
        private TextBox txtDependentName;
        private ComboBox cboRelationship;
        private DateTimePicker dtpDob;

        public DependentForm() : base("Dependent Management", 160)
        {
            BuildFields();
            WireEvents();
            LoadEmployeeOptions();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Dependent ID:", leftX, row1));
            txtDependentId = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtDependentId);

            FieldsPanel.Controls.Add(AddLabel("Employee No:", rightX, row1));
            cboEmployeeNo = new ComboBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboEmployeeNo);

            FieldsPanel.Controls.Add(AddLabel("Dependent Name:", leftX, row1 + rowH));
            txtDependentName = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtDependentName);

            FieldsPanel.Controls.Add(AddLabel("Relationship:", rightX, row1 + rowH));
            cboRelationship = new ComboBox { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDown };
            cboRelationship.Items.AddRange(new object[] { "Spouse", "Child", "Parent", "Sibling", "Other" });
            FieldsPanel.Controls.Add(cboRelationship);

            FieldsPanel.Controls.Add(AddLabel("Date Of Birth:", leftX, row1 + rowH * 2));
            dtpDob = new DateTimePicker { Location = new Point(leftX + 150, row1 + rowH * 2), Width = 200, Format = DateTimePickerFormat.Short };
            FieldsPanel.Controls.Add(dtpDob);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddDependent();
            BtnUpdate.Click += (s, e) => UpdateDependent();
            BtnDelete.Click += (s, e) => DeleteDependent();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchDependent();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
        }

        /// <summary>Fills the Employee No combo box with the current list of riders (foreign key selection).</summary>
        private void LoadEmployeeOptions()
        {
            try
            {
                cboEmployeeNo.Items.Clear();
                foreach (Rider rider in _riderService.GetAll())
                    cboEmployeeNo.Items.Add(rider.EmployeeNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load rider list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Failed to load dependents: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Dependent dependent)) return;

            txtDependentId.Text = dependent.DependentID;
            cboEmployeeNo.Text = dependent.EmployeeNo;
            txtDependentName.Text = dependent.DependentName;
            cboRelationship.Text = dependent.Relationship;
            dtpDob.Value = dependent.DateOfBirth == default ? DateTime.Now : dependent.DateOfBirth;
        }

        private Dependent BuildDependentFromFields()
        {
            return new Dependent
            {
                DependentID = txtDependentId.Text.Trim(),
                EmployeeNo = cboEmployeeNo.Text.Trim(),
                DependentName = txtDependentName.Text.Trim(),
                Relationship = cboRelationship.Text.Trim(),
                DateOfBirth = dtpDob.Value
            };
        }

        private void AddDependent()
        {
            try
            {
                _service.Add(BuildDependentFromFields());
                MessageBox.Show("Dependent added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A dependent with this Dependent ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("The selected Employee No does not exist.", "Invalid Reference", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add dependent: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDependent()
        {
            try
            {
                _service.Update(BuildDependentFromFields());
                MessageBox.Show("Dependent updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update dependent: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteDependent()
        {
            if (Validator.IsNullOrEmpty(txtDependentId.Text))
            {
                MessageBox.Show("Select a dependent or enter a Dependent ID to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this dependent?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtDependentId.Text.Trim());
                MessageBox.Show("Dependent deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete dependent: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single dependent by Dependent ID and displays it in the fields.</summary>
        private void SearchDependent()
        {
            if (Validator.IsNullOrEmpty(txtDependentId.Text))
            {
                MessageBox.Show("Enter a Dependent ID to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Dependent dependent = _service.Search(txtDependentId.Text.Trim());
                if (dependent == null)
                {
                    MessageBox.Show("No dependent found with that Dependent ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cboEmployeeNo.Text = dependent.EmployeeNo;
                txtDependentName.Text = dependent.DependentName;
                cboRelationship.Text = dependent.Relationship;
                dtpDob.Value = dependent.DateOfBirth == default ? DateTime.Now : dependent.DateOfBirth;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtDependentId.Clear();
            cboEmployeeNo.SelectedIndex = -1;
            txtDependentName.Clear();
            cboRelationship.SelectedIndex = -1;
            cboRelationship.Text = "";
            dtpDob.Value = DateTime.Now;
            Grid.ClearSelection();
        }
    }
}
