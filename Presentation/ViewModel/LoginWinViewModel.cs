using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Presentation.Model;
using System.Windows;
using System.Windows.Forms;

namespace Presentation.ViewModel
{
    class LoginWinViewModel : NotifiableObject
    {
        public BackendController controller { get; private set; }

        /// <summary>
        /// Simple constructor
        /// </summary>
        public LoginWinViewModel()
        {
            this.controller = new BackendController();
            this.personalEmail = "tariq@hot.com";
            this.pwd = "Tariq1";
        }

        /// <summary>
        /// all the fields that show up in the login/register window!
        /// </summary>
        private string personalEmail = "";
        private string pwd = "";
        private string hostEmail = "";
        private string nickName = "";

        public string PersonalEmail
        {
            get => personalEmail;
            set
            {
                personalEmail = value;
                RaisePropertyChanged("PersonalEmail");
            }
        }

        public string Pwd
        {
            get => pwd;
            set
            {
                pwd = value;
                RaisePropertyChanged("Pwd");
            }
        }

        public string HostEmail
        {
            get => hostEmail;
            set
            {
                hostEmail = value;
                RaisePropertyChanged("HostEmail");
            }
        }

        public string NickName
        {
            get => nickName;
            set
            {
                nickName = value;
                RaisePropertyChanged("Nickname");
            }
        }

        /// <summary>
        /// Trying to login of a user!
        /// </summary>
        /// <returns> returns object of UserModel if the operation succeeded! otherwise pop and error message </returns>
        public UserModel Login()
        {
            try
            {
                return controller.Login(personalEmail, pwd);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        /// <summary>
        /// Registering a new user to the system!
        /// </summary>
        /// <returns> returns true and pop a successfull message! otherwise returns false and pop an error message </returns>
        public bool Register()
        {
            try
            {
                controller.Register(personalEmail, pwd, nickName);
                System.Windows.Forms.MessageBox.Show("Registered successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        /// <summary>
        /// Registering a new user to the system!
        /// </summary>
        /// <param name="HostEmail"> registering a new user to join an exsisting board requests the host email of the board (board owner) </param>
        /// <returns> return true if the operation succeeded! otherwise returns false </returns>
        public bool Register(string HostEmail)
        {
            try
            {
                controller.Register(personalEmail, pwd, nickName, HostEmail);
                System.Windows.Forms.MessageBox.Show("Registered successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }
    }
}
