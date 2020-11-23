using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    class userService
    {
        private Business_Layer.UserPackage.UserController userController;

        public userService()
        {
            userController = new Business_Layer.UserPackage.UserController();
        }

        public Response LoadData()
        {
            try
            {
                userController.LoadAllUsers();
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }

        public Response Register(string email, string password, string nickname)
        {
            try
            {
                userController.Register(email, password, nickname);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response<User> Login(string email, string password)
        {
            try
            {
                userController.Login(email, password);
                User user = new User(userController.getUser(email));
                return new Response<User>(user);
            }
            catch (Exception ex)
            {
                return new Response<User>(ex.Message);
            }
        }
        public Response Logout(string email)
        {
            try
            {
                userController.Logout(email);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response validateLoggedIn(string email)
        {
            try
            {
                userController.ValidateLoggedIn(email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        public Response DeleteData()
        {
            try
            {
                userController.DeleteUserData();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        public void SetId(string email, int id)
        {
            userController.SetId(email, id);
        }
    }
}
