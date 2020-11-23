using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccess_Layer.DTOs
{
    class UserDTO : DTO
    {

        public const string EmailColumnName = "Email";
        public const string NickNameColumnName = "Nickname";
        public const string PassColumnName = "Password";
        public const string boardIDColumnName = "boardID";

        /// <summary>
        /// Simple constructor creating a DAL-User to save it in the dataBase
        /// </summary>
        /// <param name="Email"> user's email</param>
        /// <param name="Nickname"> user's nickName</param>
        /// <param name="Password"> user's password </param>
        /// <param name="isOnline"> user's status </param>
        public UserDTO(string Email, string Nickname, string Password, int boardId) : base(new UserDalController())
        {
            _email = Email;
            _nickname = Nickname;
            _pass = Password;
            _boardId = boardId;
        }

        private string _email;
        public string Email { get => _email; set { _email = value; _controller.GetUserDalController().Update(_email, EmailColumnName, value); } }

        private string _nickname;
        public string Nickname { get => _nickname; set { _nickname = value; _controller.GetUserDalController().Update(_email, NickNameColumnName, value); } }

        private string _pass;
        public string Password { get => _pass; set { _pass = value; _controller.GetUserDalController().Update(_email, PassColumnName, value); } }


        private int _boardId;
        public int boardID { get => _boardId; set { _boardId = value; _controller.GetUserDalController().Update(_email, boardIDColumnName, value.ToString()); } }

        /// <summary>
        /// saving the user 
        /// </summary>
        public void Save()
        {
            Nickname = _nickname;
            Password = _pass;
            boardID = _boardId;
        }

    }
}
