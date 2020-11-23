using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct User
    {
        public readonly string Email;
        public readonly string Nickname;
        internal User(string email, string nickname)
        {
            this.Email = email;
            this.Nickname = nickname;
        }

        internal User(Business_Layer.UserPackage.User user)
        {
            Email = user.getEmail();
            Nickname = user.getNickname();
        }
        // You can add code here
    }
}