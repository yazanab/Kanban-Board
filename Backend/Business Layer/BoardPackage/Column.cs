using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.Business_Layer.BoardPackage
{
    class Column : PersistedObject<DataAccess_Layer.DTOs.ColumnDTO>
    {
        private string name;
        private List<Task> Tasks;
        private int NumberOfTasks;
        private int limit;

        //Fields Below Are For Database Managment Purposes Only.
        public string Email { get; set; }
        public int columnID { get; set; }
        private DataAccess_Layer.TaskDalController taskDalController;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int DEFAULT_LIMIT = 100;
        private const int UNLIMITED = -1;


        /// <summary>
        /// Initializig a new column with a UNIQUE name!
        /// So we check the validity of the name in checkValid func
        /// </summary>
        /// <param name="name"> column name </param>
        public Column(string name)
        {
            checkValid(name);
            this.name = name;
            Tasks = new List<Task>();
            NumberOfTasks = 0;
            limit = DEFAULT_LIMIT;  //which means there is no limit by default
            taskDalController = new DataAccess_Layer.TaskDalController();
        }
        public Task getTask(int taskId)
        {
            int index = 0;
            bool temp = false;
            for (int i = 0; i < Tasks.Count && !temp; i++)
            {
                if (Tasks[i].getId() == taskId)
                {
                    temp = true;
                    index = i;
                }
            }
            if (!temp)
            {
                log.Warn("No such task with the given Id!");
                throw new Exception("No such task with the given Id!");
            }
            return Tasks[index];
        }

        public void SetName(string newName)
        {
            checkValid(newName);
            this.ToDalObject().UpdatedName = newName;
            name = newName;
            foreach (Task t in Tasks)
            {
                t.colName = newName;
                t.Save();
            }
            Save();
        }

        /// <summary>
        /// Adding a new Task to column!
        /// </summary>
        /// <param name="newTask"> Task to be added</param>
        /// <param name="taskId"> Task Id (unique) to be added </param>
        public void addTask(Task newTask, int taskId)
        {
            if (limit == Tasks.Count)
                throw new Exception("The first column reached its full number of tasks!");
            Tasks.Add(newTask);
            newTask.setId(taskId);
            newTask.colName = name;
            //Saving to database.
            taskDalController.Insert(newTask.ToDalObject());
            NumberOfTasks++;
            Save();//Saving Column due to task increment.
        }

        /// <summary>
        /// Helper function to achive an important requirement!
        /// Removing an existing task from the column!
        /// </summary>
        /// <param name="taskId"> Task unique Id to be removed! </param>
        public void RemoveTask(int taskId)
        {
            int index = 0;
            bool temp = false;
            for (int i = 0; i < Tasks.Count && !temp; i++)
            {
                if (Tasks[i].getId() == taskId)
                {
                    temp = true;
                    index = i;
                }
            }
            if (!temp)
            {
                log.Warn("No such task with the given Id!");
                throw new Exception("No such task with the given Id!");
            }
            taskDalController.Delete(Tasks.ElementAt(index).ToDalObject());//Deleting task.
            Tasks.RemoveAt(index);
            NumberOfTasks--;
            Save();//Saving column.
        }
        public IReadOnlyList<Task> getTasks()
        {
            return Tasks;
        }
        public string getName()
        {
            return name;
        }

        /// <summary>
        /// Limiting the column number of tasks!
        /// </summary>
        /// <param name="Limit"> the column limit to be </param>
        public void limitColumn(int Limit)
        {
            if (Limit < Tasks.Count && Limit != UNLIMITED)
            {
                log.Warn("Could not change column limit!");
                throw new Exception("Could not change column limit!");
            }
            limit = Limit;
            Save();
        }
        public int getLimit()
        {
            return limit;
        }
        public int getNumberOfTasks()
        {
            return Tasks.Count;
        }

        /// <summary>
        /// Converting BL-Column to DAL-Column so we save it in the dataBase
        /// </summary>
        /// <returns></returns>
        public DataAccess_Layer.DTOs.ColumnDTO ToDalObject()
        {
            DataAccess_Layer.DTOs.ColumnDTO newColumn = new DataAccess_Layer.DTOs.ColumnDTO(Email, name, limit, columnID);
            return newColumn;
        }

        /// <summary>
        /// Saving the changed so far in the dataBase
        /// </summary>
        public void Save()
        {
            ToDalObject().Save();
        }

        /// <summary>
        /// Simple construcotr that converts DAL-column to BL-Column
        /// </summary>
        /// <param name="c"></param>
        public Column(DataAccess_Layer.DTOs.ColumnDTO c)
        {
            this.name = c.Name;
            this.limit = c.Limit;
            columnID = c.ColID;
            Email = c.Email;
            List<Task> tasks = new List<Task>();
            List<TaskDTO> tasksDTO_list = new List<TaskDTO>();
            taskDalController = new DataAccess_Layer.TaskDalController();
            tasksDTO_list = taskDalController.SelectUserTasks(Email, name);
            foreach (DataAccess_Layer.DTOs.TaskDTO t in tasksDTO_list)
            {
                tasks.Add(new Task(t));
            }
            this.Tasks = tasks;
            this.NumberOfTasks = Tasks.Count;

        }

        /// <summary>
        /// column name validity!
        /// </summary>
        /// <param name="name"> not empty or null or consists of white spaces only! </param>
        public void checkValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length == 0)
                throw new Exception("Column name is not valid!");
        }
    }
}
