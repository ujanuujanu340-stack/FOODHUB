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
    /// CRUD screen for the ItemIngredient link table (many-to-many between
    /// FoodItem and Ingredient). UI only - all data access goes through
    /// <see cref="ItemIngredientService"/>, <see cref="FoodItemService"/> and
    /// <see cref="IngredientService"/>.
    /// </summary>
    public class ItemIngredientForm : BaseCrudForm
    {
        private readonly ItemIngredientService _service = new ItemIngredientService();
        private readonly FoodItemService _foodItemService = new FoodItemService();
        private readonly IngredientService _ingredientService = new IngredientService();

        private ComboBox cboItemNo;
        private ComboBox cboIngredientId;
        private TextBox txtQuantity;

        // Remembers the composite key of the currently-selected grid row so
        // Update/Delete still target the correct record even if the combo
        // boxes are changed before saving.
        private string _selectedItemNo;
        private string _selectedIngredientId;

        public ItemIngredientForm() : base("Item Ingredient Management", 90)
        {
            BuildFields();
            WireEvents();
            LoadComboOptions();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10, rowH = 38;

            FieldsPanel.Controls.Add(AddLabel("Item No:", leftX, row1));
            cboItemNo = new ComboBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboItemNo);

            FieldsPanel.Controls.Add(AddLabel("Ingredient ID:", rightX, row1));
            cboIngredientId = new ComboBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            FieldsPanel.Controls.Add(cboIngredientId);

            FieldsPanel.Controls.Add(AddLabel("Quantity:", leftX, row1 + rowH));
            txtQuantity = new TextBox { Location = new Point(leftX + 150, row1 + rowH), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtQuantity);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddLink();
            BtnUpdate.Click += (s, e) => UpdateLink();
            BtnDelete.Click += (s, e) => DeleteLink();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchLink();
            Grid.SelectionChanged += (s, e) => PopulateFieldsFromSelectedRow();
        }

        private void LoadComboOptions()
        {
            try
            {
                cboItemNo.Items.Clear();
                foreach (FoodItem item in _foodItemService.GetAll())
                    cboItemNo.Items.Add(item.ItemNo);

                cboIngredientId.Items.Clear();
                foreach (Ingredient ingredient in _ingredientService.GetAll())
                    cboIngredientId.Items.Add(ingredient.IngredientID);
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
                MessageBox.Show("Failed to load item ingredients: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is ItemIngredient itemIngredient)) return;

            _selectedItemNo = itemIngredient.ItemNo;
            _selectedIngredientId = itemIngredient.IngredientID;

            cboItemNo.Text = itemIngredient.ItemNo;
            cboIngredientId.Text = itemIngredient.IngredientID;
            txtQuantity.Text = itemIngredient.Quantity.ToString();
        }

        private bool TryBuildItemIngredientFromFields(out ItemIngredient itemIngredient)
        {
            itemIngredient = null;
            if (!Validator.IsPositiveDecimal(txtQuantity.Text))
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            itemIngredient = new ItemIngredient
            {
                ItemNo = cboItemNo.Text.Trim(),
                IngredientID = cboIngredientId.Text.Trim(),
                Quantity = decimal.Parse(txtQuantity.Text.Trim())
            };
            return true;
        }

        private void AddLink()
        {
            if (!TryBuildItemIngredientFromFields(out ItemIngredient itemIngredient)) return;
            try
            {
                _service.Add(itemIngredient);
                MessageBox.Show("Item ingredient added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE") || ex.Message.Contains("PRIMARY KEY"))
            {
                MessageBox.Show("This Item No + Ingredient ID combination already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("The selected Item No or Ingredient ID does not exist.", "Invalid Reference", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add item ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLink()
        {
            if (!TryBuildItemIngredientFromFields(out ItemIngredient itemIngredient)) return;
            if (Validator.IsNullOrEmpty(_selectedItemNo) || Validator.IsNullOrEmpty(_selectedIngredientId))
            {
                MessageBox.Show("Select a record from the grid to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _service.Update(_selectedItemNo, _selectedIngredientId, itemIngredient);
                MessageBox.Show("Item ingredient updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update item ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteLink()
        {
            string itemNo = !Validator.IsNullOrEmpty(_selectedItemNo) ? _selectedItemNo : cboItemNo.Text.Trim();
            string ingredientId = !Validator.IsNullOrEmpty(_selectedIngredientId) ? _selectedIngredientId : cboIngredientId.Text.Trim();

            if (Validator.IsNullOrEmpty(itemNo) || Validator.IsNullOrEmpty(ingredientId))
            {
                MessageBox.Show("Select a record or choose an Item No and Ingredient ID to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(itemNo, ingredientId);
                MessageBox.Show("Item ingredient deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete item ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single item ingredient by Item No + Ingredient ID and displays it in the fields.</summary>
        private void SearchLink()
        {
            if (Validator.IsNullOrEmpty(cboItemNo.Text) || Validator.IsNullOrEmpty(cboIngredientId.Text))
            {
                MessageBox.Show("Select an Item No and Ingredient ID to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ItemIngredient itemIngredient = _service.Search(cboItemNo.Text.Trim(), cboIngredientId.Text.Trim());
                if (itemIngredient == null)
                {
                    MessageBox.Show("No matching record found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedItemNo = itemIngredient.ItemNo;
                _selectedIngredientId = itemIngredient.IngredientID;
                txtQuantity.Text = itemIngredient.Quantity.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            cboItemNo.SelectedIndex = -1;
            cboIngredientId.SelectedIndex = -1;
            txtQuantity.Clear();
            _selectedItemNo = null;
            _selectedIngredientId = null;
            Grid.ClearSelection();
        }
    }
}
