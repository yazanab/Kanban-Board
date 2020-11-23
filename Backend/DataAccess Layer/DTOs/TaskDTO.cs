using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccess_Layer.DTOs
{
    class TaskDTO : DTO
    {
        /// <summary>
        /// All the fields bellow is going to appear in the dataBase file
        /// </TaskId> The task Id to be
        /// </myEmail> The user's email which this task belongs to
        /// </columnName> The name of the column which this task belongs to
        /// </myTitle> The task title to be
        /// </Description> The task description to be
        /// </myCreationDate> The task creationDate to be
        /// </myDueDate> The task dueDate to be
        /// </summary>
        public const string TaskId = "taskID";
        public const string myEmail = "hostEmail";
        public const string columnName = "columnName";
        public const string myTitle = "Title";
        public const string myDescription = "Description";
        public const string myCreationDate = "CreationDate";
        public const string myDueDate = "DueDate";
        public const string myAssignee = "Assignee";

        /// <summary>
        /// Updating the fields using getters/setter as shown in the class
        /// </summary>
        private int _task_id;
        public int taskId { get => _task_id; set { _task_id = value; _controller.GetTaskDalController().Update(_task_id, _email, TaskId, value.ToString()); } }

        private string _email;
        public string Email { get => _email; set { _email = value; _controller.GetTaskDalController().Update(_task_id, _email, myEmail, value); } }

        private string _colName;
        public string colName { get => _colName; set { _colName = value; _controller.GetTaskDalController().Update(_task_id, _email, columnName, value.ToString()); } }


        private string _title;
        public string Title { get => _title; set { _title = value; _controller.GetTaskDalController().Update(_task_id, _email, myTitle, value); } }

        private string _description;
        public string Description { get => _description; set { _description = value; _controller.GetTaskDalController().Update(_task_id, _email, myDescription, value); } }

        private string _creation_date;
        public string CreationDate { get => _creation_date; set { _creation_date = value; _controller.GetTaskDalController().Update(_task_id, _email, myCreationDate, value); } }

        private string _due_date;
        public string DueDate { get => _due_date; set { _due_date = value; _controller.GetTaskDalController().Update(_task_id, _email, myDueDate, value); } }

        private string _assignee;
        public string Assignee { get => _assignee; set { _assignee = value; _controller.GetTaskDalController().Update(_task_id, _email, myAssignee, value); } }

        /// <summary>
        /// simple constructor of the DAL-Task
        /// </summary>
        /// <param name="task_id"></param>
        /// <param name="Email"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="creation_date"></param>
        /// <param name="due_date"></param>
        public TaskDTO(int task_id, string Assignee, string colName, string title, string desc, string creation_date, string due_date, string hostEmail) : base(new TaskDalController())
        {
            _task_id = task_id;
            _email = hostEmail;
            _assignee = Assignee;
            _colName = colName;
            _title = title;
            _description = desc;
            _creation_date = creation_date;
            _due_date = due_date;
        }

        /// <summary>
        /// Saving the task details to the dataBase
        /// </summary>
        public void Save()
        {
            taskId = _task_id;
            colName = _colName;
            Title = _title;
            Description = _description;
            CreationDate = _creation_date;
            DueDate = _due_date;
            Assignee = _assignee;
        }
    }
}

