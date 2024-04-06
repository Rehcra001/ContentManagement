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

namespace ContentManagement.WPF.Views.Administration
{
    /// <summary>
    /// Interaction logic for NewUserView.xaml
    /// </summary>
    public partial class NewUserView : UserControl
    {
        public NewUserView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordEntered(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                string name = ((PasswordBox)sender).Name;
                if (name.Equals(this.pwbPassword.Name))
                {
                    ((dynamic)this.DataContext).NewUser.Password = ((PasswordBox)sender).Password;
                }
                else
                {
                    ((dynamic)this.DataContext).NewUser.ConfirmPassword = ((PasswordBox)sender).Password;
                }
                
            }
        }
    }
}
