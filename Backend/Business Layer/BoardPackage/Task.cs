using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.Business_Layer.BoardPackage
{
    class Task : PersistedObject<DataAccess_Layer.DTOs.TaskDTO>
    {
        private int taskId;
        private string assigneeEmail;
        private string Title;
        private string Description;
        private DateTime CreationDate;
        private DateTime finalDate;

        //Field below for database purposes.
        public string colName { get; set; }
        private string hostEmail;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int MAX_TITLE = 50;
        private const int MAX_DESC = 300;

        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="Title"></param>
        /// <param name="Desc"></param>
        /// <param name="finalDate"></param>
        public Task(string email, string Title, string Desc, DateTime finalDate)
        {
            validateDesc(Desc);
            validateDuoDate(finalDate);
            validateTitle(Title);
            CreationDate = DateTime.Now;
            assigneeEmail = email;
            hostEmail = email;
            this.Title = Title;
            Description = Desc;
            this.finalDate = finalDate;
            taskId = 0;
        }

        /// <summary>
        /// Each task has a unique id in the board, so here we set the Id.
        /// </summary>
        /// <param name="id"> Task id </param>
        public void setId(int id)
        {
            taskId = id;
            Save();
        }
        public int getId()
        {
            return taskId;
        }
        /// <summary>
        /// updating the task title!
        /// first we check the validity of the title and then change it..
        /// </summary>
        /// <param name="newTitle"></param>
        public void changetTitle(string newTitle)
        {
            validateTitle(newTitle);
            Title = newTitle;
            Save();
        }
        /// <summary>
        /// updating the task description!
        /// first we check the validity of the description and then change it..
        /// </summary>
        /// <param name="newTitle"></param>
        public void changeDesc(string newDesc)
        {
            validateDesc(newDesc);
            Description = newDesc;
            Save();
        }
        /// <summary>
        /// updating the task dueDate!
        /// first we check the validity of the DueDate and then change it..
        /// </summary>
        /// <param name="newTitle"></param>
        public void changeFinalDate(DateTime finalDate)
        {
            if (finalDate.CompareTo(DateTime.Now) < 0)
                throw new Exception("DueDate is invalid");
            this.finalDate = finalDate;
            Save();
        }
        public string getTitle()
        {
            return Title;
        }
        public string getDescription()
        {
            return Description;
        }
        public DateTime getCreationDate()
        {
            return CreationDate;
        }
        public DateTime getDueDate()
        {
            return finalDate;
        }

        /// <summary>
        /// checking validity of the task title!
        /// </summary>
        /// <param name="Title"> not empty and max 50 length! </param>
        public void validateTitle(string Title)
        {
            if (string.IsNullOrWhiteSpace(Title) || Title.Length > MAX_TITLE)
            {
                log.Warn("Task title length cannot be over 50!");
                throw new Exception("Title's length is not valid!");
            }
        }

        /// <summary>
        /// validity of the task description!
        /// </summary>
        /// <param name="Description"> optional, but if exists, must be at most 300 length! </param>
        public void validateDesc(string Description)
        {
            if (Description != null && Description.Length > MAX_DESC)
            {
                log.Warn("Task description length cannot be over 300!");
                throw new Exception("Task description length cannot be over 300!");
            }
        }

        /// <summary>
        /// validity of task DueDate!
        /// </summary>
        /// <param name="finalDate"> must be earlier of today so it makes sence! </param>
        public void validateDuoDate(DateTime finalDate)
        {
            if (finalDate.CompareTo(DateTime.Today) < 0)
            {
                log.Warn("Error: Can not set this finalDate because its over!");
                throw new Exception("Duedate is not valid!");
            }
        }

        /// <summary>
        /// Converting our BL-Task to DAL-Task in order to save in dataBase
        /// </summary>
        /// <returns></returns>
        public DataAccess_Layer.DTOs.TaskDTO ToDalObject()
        {
            DataAccess_Layer.DTOs.TaskDTO newTask = new DataAccess_Layer.DTOs.TaskDTO(taskId, assigneeEmail, colName, Title, Description, CreationDate.ToString(), finalDate.ToString(), hostEmail);
            return newTask;
        }

        /// <summary>
        /// Here we save the task into the DataBase
        /// </summary>
        public void Save()
        {
            ToDalObject().Save();
        }

        /// <summary>
        /// simple constructor that takes a DAL-Task and converts it to BL-Task
        /// </summary>
        /// <param name="t"> the DAL-Task </param>
        public Task(DataAccess_Layer.DTOs.TaskDTO t)
        {
            this.taskId = t.taskId;
            this.hostEmail = t.Email;
            this.colName = t.colName;
            this.Title = t.Title;
            this.Description = t.Description;
            this.CreationDate = DateTime.Parse(t.CreationDate);
            this.finalDate = DateTime.Parse(t.DueDate);
            this.assigneeEmail = t.Assignee;
        }

        public string getHostEamil()
        {
            return hostEmail;
        }

        public string GetEmail()
        {
            return assigneeEmail;
        }

        public void AssignEmail(string newEmail)
        {
            assigneeEmail = newEmail;
            Save();
        }
    }
}
