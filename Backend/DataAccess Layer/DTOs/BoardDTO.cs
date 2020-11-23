using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccess_Layer.DTOs
{
    class BoardDTO : DTO
    {
        public const string myEmail = "Email";
        public const string myTaskId = "taskId";
        public const string id = "boardId";

        /// <summary>
        /// Here we update all the fields as shown in the class!
        /// </summary>
        private string _email;
        public string Email { get => _email; set { _email = value; _controller.GetBoardDalController().Update(_email, myEmail, value); } }

        private int _task_id;
        public int taskID { get => _task_id; set { _task_id = value; _controller.GetBoardDalController().Update(_email, myTaskId, value.ToString()); } }

        private int _id;
        public int ID { get => _id; set { _id = value; _controller.GetBoardDalController().Update(_email, id, value.ToString()); } }


        public BoardDTO(string Email, int task_id, int id) : base(new BoardDalController())
        {
            _email = Email;
            _task_id = task_id;
            _id = id;
        }

        public void Save()
        {
            taskID = _task_id;
            ID = _id;
        }
    }
}
