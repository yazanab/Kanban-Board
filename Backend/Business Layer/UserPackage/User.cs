using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.Business_Layer.UserPackage
{
    class User : PersistedObject<DataAccess_Layer.DTOs.UserDTO>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string Email;
        private string Password;
        private string nickName;
        private bool IsOnline;
        public int boardID { get; set; }

        private const int MIN_LENGTH = 5;
        private const int MAX_LENGTH = 25;

        /// <summary>
        /// Simple constructor
        /// First we check validity of the user, then if all args are valid we create the user!
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="nickName"></param>
        public User(string Email, string Password, string nickName)
        {
            if (!IsValidEmail(Email))
            {
                log.Warn("The given email is inValid");
                throw new Exception("Invalid Email!");
            }
            validatePass(Password);
            checkValid(nickName);
            this.Email = Email;
            this.Password = Password;
            IsOnline = false;
            this.nickName = nickName;
        }

        /// <summary>
        /// user constractor convert from DAL user to a  BL user
        /// </summary>
        /// <param name="u">the DAL user</param>
        public User(DataAccess_Layer.DTOs.UserDTO u)
        {
            this.Password = u.Password;
            this.Email = u.Email;
            this.nickName = u.Nickname;
            IsOnline = false;
            boardID = u.boardID;
        }

        /// <summary>
        /// Checks if the given <Email,Pass> matches the fields so the user has the right to login.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Pass"></param>
        public void Login(string Email, string Pass)
        {
            if (!this.Email.Equals(Email))
            {
                log.Warn("Email is not correct!");
                throw new Exception("Email is not correct!");
            }
            if (!Password.Equals(Pass))
            {
                log.Warn("Password is not correct!");
                throw new Exception("Password is not correct!");
            }
            if (IsOnline)
            {
                log.Warn("You already logged-in!");
                throw new Exception("You already logged-in!");
            }
            IsOnline = true;
            Save();
        }

        /// <summary>
        /// Logout func
        /// User cannot logout if he is already logged out!
        /// </summary>
        public void Logout()
        {
            if (!IsOnline)
            {
                log.Warn("User has not logged in!");
                throw new Exception("User has not logged in!");
            }
            IsOnline = false;
            Save();
        }

        /// <summary>
        /// this method check if the password is legal
        /// </summary>
        /// <param name="Password">the password to check for legality</param>
        /// <returns></returns>
        public void validatePass(string Password)
        {
            bool Number, LowerCaseLetter, UpperCaseLetter;
            Number = LowerCaseLetter = UpperCaseLetter = false;
            if (Password.Length < MIN_LENGTH || Password.Length > MAX_LENGTH)
            {
                log.Warn("Error: Invalid password length!");
                throw new Exception("Invalid password length!");
            }
            foreach (char index in Password)
            {
                // Password validity check
                if (index >= '1' && index <= '9' && !Number)
                    Number = true;
                if (index >= 'a' && index <= 'z' && !LowerCaseLetter)
                    LowerCaseLetter = true;
                if (index >= 'A' && index <= 'Z' && !UpperCaseLetter)
                    UpperCaseLetter = true;
            }
            // Exception handling
            if (!Number)
            {
                log.Warn("Your password does not contain a Number!");
                throw new Exception("Password must has a number!");
            }
            if (!LowerCaseLetter)
            {
                log.Warn("Your password does not contain a LawCaseLetter!");
                throw new Exception("Password must has a LawCaseLetter!");
            }
            if (!UpperCaseLetter)
            {
                log.Warn("Your password does not contain an UpperCaseLetter!");
                throw new Exception("Password must has an UpperCaseLetter!");
            }
        }

        /// <summary>
        /// Checks the validity of the user's nickName
        /// </summary>
        /// <param name="nickName"></param>
        public void checkValid(string nickName)
        {
            if (nickName.Equals(""))
            {
                log.Warn("Nickname shouldnt be empty!");
                throw new Exception("Nickname shouldnt be empty!");
            }
            if (string.IsNullOrWhiteSpace(nickName))
            {
                log.Warn("Nickname is inValid!");
                throw new Exception("Invalid nickname!");
            }
        }

        /// <summary>
        /// convert the User from BL user to a new DAL User
        /// </summary>
        /// <returns></returns>
        public DataAccess_Layer.DTOs.UserDTO ToDalObject()
        {
            DataAccess_Layer.DTOs.UserDTO dal_user = new DataAccess_Layer.DTOs.UserDTO(Email, nickName, Password, boardID);
            return dal_user;
        }

        public void Save()
        {
            ToDalObject().Save();
        }


        // Checks the validity of given Email!
        // return true iff the Email is valid
        // This function is taken from Microsoft API
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public bool isLoggedIn()
        {
            return IsOnline;
        }
        public string getEmail()
        {
            return Email;
        }
        public string getNickname()
        {
            return nickName;
        }
    }
}
