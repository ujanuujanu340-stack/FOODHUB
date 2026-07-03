using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Theme/Colour table. UI only - all data access goes through <see cref="ThemeService"/> and <see cref="MotorbikeService"/>.</summary>
    public class ThemeForm : BaseCrudForm
    {
        private readonly ThemeService _service = new ThemeService();
        private readonly MotorbikeService _motorbikeService = new MotorbikeService();

        private TextBox txtThemeId;
        private ComboBox cboVehicleRegNo;
        private ComboBox cboColourName;

        public ThemeForm() : base("Theme / Colour Management", 90)
        {
            BuildFields();
            WireEvents();
            LoadVehicleOptions();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10;

            FieldsPanel.Controls.Add(AddLabel("Theme ID:", leftX, row1));
            txtThemeId = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtThemeId);

            FieldsPanel.Controls.Add(AddLabel("Vehicle Reg No:", rightX, row1));
            cboVehicleRegNo = new ComboBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboVehicleRegNo);

            FieldsPanel.Controls.Add(AddLabel("Colour Name:", leftX, row1 + 38));
            cboColourName = new ComboBox { Location = new Point(leftX + 150, row1 + 38), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDown };
            cboColourName.Items.AddRange(new object[] { "Red", "Blue", "Green", "Black", "White", "Silver", "Yellow", "Orange" });
            FieldsPanel.Controls.Add(cboColourName);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddTheme();
            BtnUpdate.Click += (s, e) => UpdateTheme();
            BtnDelete.Click += (s, e) => DeleteTheme();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchTheme();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
        }

        /// <summary>Fills the Vehicle Reg No combo box with the current list of motorbikes (foreign key selection).</summary>
        private void LoadVehicleOptions()
        {
            try
            {
                cboVehicleRegNo.Items.Clear();
                foreach (Motorbike motorbike in _motorbikeService.GetAll())
                    cboVehicleRegNo.Items.Add(motorbike.VehicleRegNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load motorbike list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Failed to load themes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Theme theme)) return;

            txtThemeId.Text = theme.ThemeID;
            cboVehicleRegNo.Text = theme.VehicleRegNo;
            cboColourName.Text = theme.ColourName;
        }

        private Theme BuildThemeFromFields()
        {
            return new Theme
            {
                ThemeID = txtThemeId.Text.Trim(),
                VehicleRegNo = cboVehicleRegNo.Text.Trim(),
                ColourName = cboColourName.Text.Trim()
            };
        }

        private void AddTheme()
        {
            try
            {
                _service.Add(BuildThemeFromFields());
                MessageBox.Show("Theme added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A theme with this Theme ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("The selected Vehicle Reg No does not exist.", "Invalid Reference", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add theme: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTheme()
        {
            try
            {
                _service.Update(BuildThemeFromFields());
                MessageBox.Show("Theme updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update theme: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteTheme()
        {
            if (Validator.IsNullOrEmpty(txtThemeId.Text))
            {
                MessageBox.Show("Select a theme or enter a Theme ID to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this theme?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtThemeId.Text.Trim());
                MessageBox.Show("Theme deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete theme: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single theme by Theme ID and displays it in the fields.</summary>
        private void SearchTheme()
        {
            if (Validator.IsNullOrEmpty(txtThemeId.Text))
            {
                MessageBox.Show("Enter a Theme ID to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Theme theme = _service.Search(txtThemeId.Text.Trim());
                if (theme == null)
                {
                    MessageBox.Show("No theme found with that Theme ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cboVehicleRegNo.Text = theme.VehicleRegNo;
                cboColourName.Text = theme.ColourName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtThemeId.Clear();
            cboVehicleRegNo.SelectedIndex = -1;
            cboColourName.SelectedIndex = -1;
            cboColourName.Text = "";
            Grid.ClearSelection();
        }
    }
}
