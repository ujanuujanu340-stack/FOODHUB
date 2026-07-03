using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Motorbike table. UI only - all data access goes through <see cref="MotorbikeService"/>.</summary>
    public class MotorbikeForm : BaseCrudForm
    {
        private readonly MotorbikeService _service = new MotorbikeService();

        private TextBox txtVehicleRegNo;
        private TextBox txtBrand;
        private TextBox txtModel;
        private TextBox txtEngineNo;
        private DateTimePicker dtpRegisteredDate;

        public MotorbikeForm() : base("Motorbike Management", 160)
        {
            BuildFields();
            WireEvents();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Vehicle Reg No:", leftX, row1));
            txtVehicleRegNo = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtVehicleRegNo);

            FieldsPanel.Controls.Add(AddLabel("Brand:", rightX, row1));
            txtBrand = new TextBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtBrand);

            FieldsPanel.Controls.Add(AddLabel("Model:", leftX, row1 + rowH));
            txtModel = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtModel);

            FieldsPanel.Controls.Add(AddLabel("Engine No:", rightX, row1 + rowH));
            txtEngineNo = new TextBox { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtEngineNo);

            FieldsPanel.Controls.Add(AddLabel("Registered Date:", leftX, row1 + rowH * 2));
            dtpRegisteredDate = new DateTimePicker { Location = new Point(leftX + 150, row1 + rowH * 2), Width = 200, Format = DateTimePickerFormat.Short };
            FieldsPanel.Controls.Add(dtpRegisteredDate);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddMotorbike();
            BtnUpdate.Click += (s, e) => UpdateMotorbike();
            BtnDelete.Click += (s, e) => DeleteMotorbike();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchMotorbike();
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
                MessageBox.Show("Failed to load motorbikes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Motorbike motorbike)) return;

            txtVehicleRegNo.Text = motorbike.VehicleRegNo;
            txtBrand.Text = motorbike.Brand;
            txtModel.Text = motorbike.Model;
            txtEngineNo.Text = motorbike.EngineNo;
            dtpRegisteredDate.Value = motorbike.RegisteredDate == default ? DateTime.Now : motorbike.RegisteredDate;
        }

        private Motorbike BuildMotorbikeFromFields()
        {
            return new Motorbike
            {
                VehicleRegNo = txtVehicleRegNo.Text.Trim(),
                Brand = txtBrand.Text.Trim(),
                Model = txtModel.Text.Trim(),
                EngineNo = txtEngineNo.Text.Trim(),
                RegisteredDate = dtpRegisteredDate.Value
            };
        }

        private void AddMotorbike()
        {
            try
            {
                _service.Add(BuildMotorbikeFromFields());
                MessageBox.Show("Motorbike added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A motorbike with this Vehicle Reg No already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add motorbike: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMotorbike()
        {
            try
            {
                _service.Update(BuildMotorbikeFromFields());
                MessageBox.Show("Motorbike updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update motorbike: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteMotorbike()
        {
            if (Validator.IsNullOrEmpty(txtVehicleRegNo.Text))
            {
                MessageBox.Show("Select a motorbike or enter a Vehicle Reg No to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this motorbike?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtVehicleRegNo.Text.Trim());
                MessageBox.Show("Motorbike deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This motorbike cannot be deleted because related theme/colour records exist.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete motorbike: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single motorbike by Vehicle Reg No and displays it in the fields.</summary>
        private void SearchMotorbike()
        {
            if (Validator.IsNullOrEmpty(txtVehicleRegNo.Text))
            {
                MessageBox.Show("Enter a Vehicle Reg No to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Motorbike motorbike = _service.Search(txtVehicleRegNo.Text.Trim());
                if (motorbike == null)
                {
                    MessageBox.Show("No motorbike found with that Vehicle Reg No.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtBrand.Text = motorbike.Brand;
                txtModel.Text = motorbike.Model;
                txtEngineNo.Text = motorbike.EngineNo;
                dtpRegisteredDate.Value = motorbike.RegisteredDate == default ? DateTime.Now : motorbike.RegisteredDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtVehicleRegNo.Clear();
            txtBrand.Clear();
            txtModel.Clear();
            txtEngineNo.Clear();
            dtpRegisteredDate.Value = DateTime.Now;
            Grid.ClearSelection();
        }
    }
}
