using System;
using System.Drawing;
using System.Windows.Forms;
using DDD.Helpers;

namespace DDD.Forms
{
    /// <summary>
    /// Application entry screen. Validates a fixed operator username/password
    /// (this system does not model a Users table - the assignment only asks
    /// for simple login validation) and, on success, opens the main dashboard.
    /// </summary>
    public class LoginForm : Form
    {
        // Fixed demo credentials for the FoodHub staff login.
        private const string ValidUsername = "admin";
        private const string ValidPassword = "admin123";

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Label lblError;

        public LoginForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "FoodHub Delivery Management System - Login";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(420, 480);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = UiHelper.BackgroundColor;
            Font = new Font("Segoe UI", 9F);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = UiHelper.HeaderColor };
            var lblLogo = new Label
            {
                Text = "FoodHub",
                Dock = DockStyle.Top,
                Height = 55,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomCenter
            };
            var lblSubtitle = new Label
            {
                Text = "Delivery Management System",
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = ColorTranslator.FromHtml("#BDC3C7"),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.TopCenter
            };
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblLogo);

            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(340, 260),
                Location = new Point(40, 150)
            };

            var lblUsername = new Label { Text = "Username", Location = new Point(20, 20), Size = new Size(280, 20), Font = UiHelper.LabelFont };
            txtUsername = new TextBox { Location = new Point(20, 44), Size = new Size(280, 28), Font = UiHelper.InputFont };

            var lblPassword = new Label { Text = "Password", Location = new Point(20, 84), Size = new Size(280, 20), Font = UiHelper.LabelFont };
            txtPassword = new TextBox { Location = new Point(20, 108), Size = new Size(280, 28), Font = UiHelper.InputFont, PasswordChar = '●' };

            lblError = new Label
            {
                Text = "",
                Location = new Point(20, 142),
                Size = new Size(280, 36),
                ForeColor = UiHelper.DeleteColor,
                Font = new Font("Segoe UI", 8.5F)
            };

            var btnLogin = UiHelper.CreateButton("Login", UiHelper.AccentColor, BtnLogin_Click);
            btnLogin.Location = new Point(20, 182);
            btnLogin.Size = new Size(280, 38);

            var btnExit = UiHelper.CreateButton("Exit", UiHelper.ClearColor, (s, e) => Application.Exit());
            btnExit.Location = new Point(20, 224);
            btnExit.Size = new Size(280, 32);

            card.Controls.Add(lblUsername);
            card.Controls.Add(txtUsername);
            card.Controls.Add(lblPassword);
            card.Controls.Add(txtPassword);
            card.Controls.Add(lblError);
            card.Controls.Add(btnLogin);
            card.Controls.Add(btnExit);

            AcceptButton = btnLogin;
            Controls.Add(card);
            Controls.Add(headerPanel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter both username and password.";
                return;
            }

            if (username == ValidUsername && password == ValidPassword)
            {
                lblError.Text = "";
                Hide();
                using (var mainMenu = new MainMenuForm())
                {
                    mainMenu.ShowDialog();
                }
                Close();
            }
            else
            {
                lblError.Text = "Invalid username or password. Please try again.";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
