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
using System.Windows.Shapes;
using Presentation.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;
using Presentation.Model;

namespace Presentation.View
{
    /// <summary>
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        LoginWinViewModel vm;

        public LoginWin()
        {
            InitializeComponent();
            this.DataContext = new LoginWinViewModel();
            this.vm = (LoginWinViewModel)DataContext;
        }

        /// <summary>
        /// A button for logging in to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel u = vm.Login();
            if (u != null)
            {
                BoardWin kanbanView = new BoardWin(u);
                kanbanView.Show();
                this.Close();
            }
        }

        /// <summary>
        /// A button for registering a new user to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (vm.HostEmail == "")
            {
                vm.Register();
            }
            else
            {
                vm.Register(vm.HostEmail);
            }
        }
    }
}
