using System;
using System.Windows.Forms;
using DDD.Database;
using DDD.Forms;

namespace DDD
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Create the SQLite database file and tables the first time the app runs.
                DatabaseConnection.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to initialize the database:\n" + ex.Message,
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}
