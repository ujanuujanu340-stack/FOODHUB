using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;
using DDD.Models;
using DDD.Services;

namespace DDD.Forms
{
    /// <summary>CRUD screen for the Ingredient table. UI only - all data access goes through <see cref="IngredientService"/>.</summary>
    public class IngredientForm : BaseCrudForm
    {
        private readonly IngredientService _service = new IngredientService();

        private TextBox txtIngredientId;
        private TextBox txtIngredientName;

        public IngredientForm() : base("Ingredient Management", 60)
        {
            BuildFields();
            WireEvents();
            LoadData();
        }

        private void BuildFields()
        {
            int leftX = 20, rightX = 400, row1 = 10;

            FieldsPanel.Controls.Add(AddLabel("Ingredient ID:", leftX, row1));
            txtIngredientId = new TextBox { Location = new Point(leftX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtIngredientId);

            FieldsPanel.Controls.Add(AddLabel("Ingredient Name:", rightX, row1));
            txtIngredientName = new TextBox { Location = new Point(rightX + 150, row1), Width = 200, Font = UiHelper.InputFont };
            FieldsPanel.Controls.Add(txtIngredientName);
        }

        private void WireEvents()
        {
            BtnAdd.Click += (s, e) => AddIngredient();
            BtnUpdate.Click += (s, e) => UpdateIngredient();
            BtnDelete.Click += (s, e) => DeleteIngredient();
            BtnClear.Click += (s, e) => ClearFields();
            BtnSearch.Click += (s, e) => SearchIngredient();
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
                MessageBox.Show("Failed to load ingredients: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromSelectedRow()
        {
            if (!(Grid.CurrentRow?.DataBoundItem is Ingredient ingredient)) return;

            txtIngredientId.Text = ingredient.IngredientID;
            txtIngredientName.Text = ingredient.IngredientName;
        }

        private Ingredient BuildIngredientFromFields()
        {
            return new Ingredient
            {
                IngredientID = txtIngredientId.Text.Trim(),
                IngredientName = txtIngredientName.Text.Trim()
            };
        }

        private void AddIngredient()
        {
            try
            {
                _service.Add(BuildIngredientFromFields());
                MessageBox.Show("Ingredient added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("An ingredient with this Ingredient ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateIngredient()
        {
            try
            {
                _service.Update(BuildIngredientFromFields());
                MessageBox.Show("Ingredient updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteIngredient()
        {
            if (Validator.IsNullOrEmpty(txtIngredientId.Text))
            {
                MessageBox.Show("Select an ingredient or enter an Ingredient ID to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this ingredient?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _service.Delete(txtIngredientId.Text.Trim());
                MessageBox.Show("Ingredient deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("This ingredient cannot be deleted because it is used by a food item.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete ingredient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Looks up a single ingredient by Ingredient ID and displays it in the fields.</summary>
        private void SearchIngredient()
        {
            if (Validator.IsNullOrEmpty(txtIngredientId.Text))
            {
                MessageBox.Show("Enter an Ingredient ID to search for.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Ingredient ingredient = _service.Search(txtIngredientId.Text.Trim());
                if (ingredient == null)
                {
                    MessageBox.Show("No ingredient found with that Ingredient ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtIngredientName.Text = ingredient.IngredientName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtIngredientId.Clear();
            txtIngredientName.Clear();
            Grid.ClearSelection();
        }
    }
}
