using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the FoodItem table. UI only - all data access goes through <see cref="FoodItemService"/>.</summary>
    public class FoodItemForm : BaseCrudForm
    {
        private readonly FoodItemService _service = new FoodItemService();

        private TextBox txtItemNo;
        private TextBox txtItemName;
        private ComboBox cboItemCategory;
        private TextBox txtPrice;

        public FoodItemForm() : base("Food Item Management", 120)
        {
            BuildFields();
            WireEvents();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Item No:", leftX, row1));
            txtItemNo = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtItemNo);

            FieldsPanel.Controls.Add(AddLabel("Item Name:", rightX, row1));
            txtItemName = new TextBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtItemName);

            FieldsPanel.Controls.Add(AddLabel("Item Category:", leftX, row1 + rowH));
            cboItemCategory = new ComboBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDown };
            cboItemCategory.Items.AddRange(new object[] { "Rice & Curry", "Short Eats", "Beverages", "Dessert", "Fast Food", "Other" });
            FieldsPanel.Controls.Add(cboItemCategory);

            FieldsPanel.Controls.Add(AddLabel("Price:", rightX, row1 + rowH));
            txtPrice = new TextBox { Location = new Point(rightX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtPrice);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddItem();
            BtnUpdate.Click += (s, e) => UpdateItem();
            BtnDelete.Click += (s, e) => DeleteItem();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchItem();
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
                MessageBox.Show("Failed to load food items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is FoodItem item)) return;

            txtItemNo.Text = item.ItemNo;
            txtItemName.Text = item.ItemName;
            cboItemCategory.Text = item.ItemCategory;
            txtPrice.Text = item.Price.ToString("0.00");
        }

        private bool TryBuildItemFromFields(out FoodItem item)
        {
            item = null;
            if (!Validator.IsPositiveDecimal(txtPrice.Text))
            {
                MessageBox.Show("Price must be a valid non-negative number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            item = new FoodItem
            {
                ItemNo = txtItemNo.Text.Trim(),
                ItemName = txtItemName.Text.Trim(),
                ItemCategory = cboItemCategory.Text.Trim(),
                Price = decimal.Parse(txtPrice.Text.Trim())
            };
            return true;
        }

        private void AddItem()
        {
            if (!TryBuildItemFromFields(out FoodItem item)) return;
            try
            {
                _service.Add(item);
                MessageBox.Show("Food item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A food item with this Item No already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add food item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateItem()
        {
            if (!TryBuildItemFromFields(out FoodItem item)) return;
            try
            {
                _service.Update(item);
                MessageBox.Show("Food item updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update food item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteItem()
        {
            if (Validator.IsNullOrEmpty(txtItemNo.Text))
            {
                MessageBox.Show("Select a food item or enter an Item No to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this food item?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtItemNo.Text.Trim());
                MessageBox.Show("Food item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This food item cannot be deleted because related orders or ingredients exist.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete food item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single food item by Item No and displays it in the fields.</summary>
        private void SearchItem()
        {
            if (Validator.IsNullOrEmpty(txtItemNo.Text))
            {
                MessageBox.Show("Enter an Item No to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                FoodItem item = _service.Search(txtItemNo.Text.Trim());
                if (item == null)
                {
                    MessageBox.Show("No food item found with that Item No.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtItemName.Text = item.ItemName;
                cboItemCategory.Text = item.ItemCategory;
                txtPrice.Text = item.Price.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtItemNo.Clear();
            txtItemName.Clear();
            cboItemCategory.SelectedIndex = -1;
            cboItemCategory.Text = "";
            txtPrice.Clear();
            Grid.ClearSelection();
        }
    }
}
