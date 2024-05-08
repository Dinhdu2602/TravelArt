using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace GUI.Views.MainWindow
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var userName = TbUserName.Text;
            var passWord = PbPassword.Password;
            if (string.IsNullOrEmpty(this.TbUserName.Text) || string.IsNullOrEmpty(this.PbPassword.Password))
            {
                MessageBox.Show("Provide User Name and Password");
                return;
            }
            if (userName == "admin" && passWord == "123")
            {
                var mainWindow = new GUI.MainWindow();
                mainWindow.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid UserName or Password");
            }
        }
    }
}