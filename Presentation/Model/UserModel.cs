using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presentation.Model
{
    public class UserModel : NotifiableModelObject
    {
        private string email;
        public string Email { get { return email; } }
        private string nickname;
        public string Nickname { get { return nickname; } }

        /// <summary>
        /// Simple constructor 
        /// </summary>
        /// <param name="email"> email of the user </param>
        /// <param name="nickname"> the username of that user </param>
        /// <param name="controller"> the controller which connected the backend service to GUI </param>
        internal UserModel(string email, string nickname, BackendController controller) : base(controller)
        {
            this.email = email;
            this.nickname = nickname;
        }

        /// <summary>
        /// given a user email, return his board if exsit!
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <returns></returns>
        public BoardModel GetBoard(string email)
        {
            return controller.GetBoard(email);
        }

        /// <summary>
        /// logout the currently logged-in user
        /// </summary>
        /// <param name="email"> the user email </param>
        public void Logout(string email)
        {
            controller.Logout(email);
        }
    }
}
