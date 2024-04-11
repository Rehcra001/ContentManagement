using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContentManagement.WPF.Views.Account
{
    /// <summary>
    /// Interaction logic for ChangePasswordView.xaml
    /// </summary>
    public partial class ChangePasswordView : UserControl
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordEntered(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                string name = ((PasswordBox)sender).Name;
                switch (name)
                {
                    case "oldPassword":
                        ((dynamic)this.DataContext).ChangePassword.OldPassword = ((PasswordBox)sender).Password;
                        break;
                    case "newPassword":
                        ((dynamic)this.DataContext).ChangePassword.NewPassword = ((PasswordBox)sender).Password;
                        break;
                    case "confirmPassword":
                        ((dynamic)this.DataContext).ChangePassword.ConfirmPassword = ((PasswordBox)sender).Password;
                        break;
                }
            }
        }
    }
}
