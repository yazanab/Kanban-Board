using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.Business_Layer.UserPackage
{
    class UserController
    {
        //A dictionary <Email, User> with all thee user's currectly resighted to the system.
        private Dictionary<string, User> allUsers;
        private DataAccess_Layer.UserDalController DataOfUser;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Simple constructor 
        /// </summary>
        public UserController()
        {
            allUsers = new Dictionary<string, User>();
            DataOfUser = new DataAccess_Layer.UserDalController();
        }

        /// <summary>
        /// A very used func that checks if the user is registered by checking the dictionary of all registered user.
        /// </summary>
        /// <param name="myEmail"></param>
        /// <returns> if the user hsa not found it returns NULL, otherwise returns the User.
        public User getUser(string myEmail)
        {
            if (!allUsers.ContainsKey(myEmail))
                // user has not registered yet!
                return null;
            return allUsers[myEmail];
        }

        /// <summary>
        /// login of an existed user, otherwise throw an error!
        /// </summary>
        /// <param name="myEmail">the email of the user </param>
        /// <param name="Password">the password of the user</param>
        /// <returns></returns>
        public void Login(string myEmail, string Password)
        {
            if (myEmail == null)
            {
                log.Warn("Email cannot be null!");
                throw new Exception("Invalid Email!");
            }
            if (getUser(myEmail) == null)
            {
                log.Warn("User not registered!");
                throw new Exception("Please register to login!");
            }
            getUser(myEmail).Login(myEmail, Password);
            log.Info("User followed with Email: " + myEmail + " logged-in successfully!");
        }

        /// <summary>
        /// Here we allowed user to logout of the system!
        /// </summary>
        /// <param name="myEmail"> must be in the system, otherwise the user is not registered so throw an error! </param>
        public void Logout(string myEmail)
        {
            if (myEmail == null)
            {
                log.Warn("Email cannot be null!");
                throw new Exception("Email cannot be null!");
            }
            if (getUser(myEmail) == null)
            {
                log.Warn("User not registered!");
                throw new Exception("User not registered!");
            }
            allUsers[myEmail].Logout();
            log.Info("User followed with Email: " + myEmail + " logged-out successfully!");
        }

        /// <summary>
        /// Registering new users to the system.
        /// </summary>
        /// <param name="myEmail"> must be valid and there is no user registered with this email before </param>
        /// <param name="Password"> user's password also must be valid (not null and so on ..) </param>
        /// <param name="nickName"></param>
        public void Register(string myEmail, string Password, string nickName)
        {
            if (myEmail == null || Password == null || nickName == null)
            {
                log.Warn("Email or Password or nickName cannot be null!");
                throw new Exception("One of the fields is invalid!");
            }
            if (getUser(myEmail) != null)
            {
                log.Warn("User already registered!");
                throw new Exception("This email is in-use!");
            }
            User newUser = new User(myEmail, Password, nickName);
            allUsers.Add(myEmail, newUser);
            /// after creating a user, we insert it to dataBase system to be updated!
            DataOfUser.Insert(new DataAccess_Layer.DTOs.UserDTO(myEmail, nickName, Password, 0));
            log.Info("User followed with Email: " + myEmail + " registered successfully!");

        }
        /// <summary>
        /// load all the users from the dataBase to Ram/Code.
        /// </summary>
        public void LoadAllUsers()
        {
            List<DataAccess_Layer.DTOs.UserDTO> list = DataOfUser.SelectAllUsers();
            foreach (DataAccess_Layer.DTOs.UserDTO user in list)
            {
                allUsers.Add(user.Email, new User(user));
            }
        }

        /// <summary>
        /// Checks if the user which has the givem email is logged-in
        /// </summary>
        /// <param name="email"></param>
        public void ValidateLoggedIn(string email)
        {
            if (email == null)
            {
                log.Warn("email is null");
                throw new Exception("email is null");
            }
            if (!allUsers.ContainsKey(email))
            {
                log.Warn("UserController: the user with the email:" + email + "is not logged in because he hasn't been registered yet.");
                throw new Exception("Register in order to login!");
            }
            if (!allUsers[email].isLoggedIn())
            {
                log.Warn("UserController: the user with the email:" + email + "is not logged in at this moment.");
                throw new Exception("User is not logged in");
            }
        }
        /// <summary>
        /// delete all the users from the dataBase.
        /// </summary>
        public void DeleteUserData()
        {
            bool rs = DataOfUser.DeleteData();
            if (!rs)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
        }
        public void SetId(string email, int id)
        {
            allUsers[email].boardID = id;
            allUsers[email].Save();
        }
    }
}
