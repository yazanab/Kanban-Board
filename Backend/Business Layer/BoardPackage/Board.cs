using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.Business_Layer.BoardPackage
{
    class Board : PersistedObject<DataAccess_Layer.DTOs.BoardDTO>
    {
        private List<Column> boardColumns;
        private int taskId;
        private string Email;
        public int id { get; set; }
        //Field below for database purposes.
        private DataAccess_Layer.ColumnDalController columnDalController;
        private DataAccess_Layer.TaskDalController taskDalController;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Simple constructor of the board, so it need an email(unique) in order to create a new one for a new user!
        /// </summary>
        /// <param name="email"></param>
        public Board(string email)
        {
            taskId = 1;
            Email = email;
            boardColumns = new List<Column>();

            columnDalController = new DataAccess_Layer.ColumnDalController();
            taskDalController = new DataAccess_Layer.TaskDalController();

            Column c1 = new Column("backlog");
            c1.Email = email;
            c1.columnID = 0;

            columnDalController.Insert(c1.ToDalObject());
            Column c2 = new Column("in progress");
            c2.Email = email;
            c2.columnID = 1;

            columnDalController.Insert(c2.ToDalObject());
            Column c3 = new Column("done");
            c3.Email = email;
            c3.columnID = 2;
            columnDalController.Insert(c3.ToDalObject());

            boardColumns.Add(c1);
            boardColumns.Add(c2);
            boardColumns.Add(c3);

        }

        public IReadOnlyList<string> getBoardColumnsNames()
        {
            List<string> toReturn = new List<string>();
            foreach (Column c in boardColumns)
            {
                toReturn.Add(c.getName());
            }
            return toReturn;
        }
        public int getTaskId()
        {
            return taskId;
        }

        /// <summary>
        /// each task has unique Id, here we cover this point!
        /// </summary>
        /// <param name="id"></param>
        public void setTaskId(int id)
        {
            taskId = id;
            Save();
        }

        public Column getColumnByName(string name)
        {
            for (int i = 0; i < boardColumns.Count; i++)
            {
                if (boardColumns[i].getName().Equals(name))
                    return boardColumns[i];
            }
            log.Warn("No such column name!!");
            throw new Exception("No such column name!!");
        }

        /// <summary>
        /// A very important func, getting a board by his unique Id, if not exist throws an error!
        /// </summary>
        /// <param name="id"> Column unique id </param>
        /// <returns></returns>
        public Column getColumnById(int id)
        {
            if (id < 0 || id >= boardColumns.Count)
            {
                log.Warn("wrong column id");
                throw new Exception("wrong column id");
            }
            return boardColumns[id];
        }

        /// <summary>
        /// Advancing a task only if its not in the last column!!
        /// </summary>
        /// <param name="sourceId"> column id to be moved from </param>
        /// <param name="taskId"> task unique id to be moved </param>
        public void advanceTask(int sourceId, int taskId)
        {
            if (sourceId == boardColumns.Count - 1)
                throw new Exception("Task is done, cannot move it");
            if (getColumnById(sourceId + 1).getLimit() == getColumnById(sourceId + 1).getNumberOfTasks())
            {
                throw new Exception("The next column reached the full number of tasks!");
            }
            Task task = getColumnById(sourceId).getTask(taskId);
            getColumnById(sourceId).RemoveTask(taskId);
            getColumnById(sourceId + 1).addTask(task, taskId);

        }

        /// <summary>
        /// editing a task allowed only if its not in the last column!
        /// </summary>
        /// <param name="columnID"></param>
        /// <param name="taskId"></param>
        /// <param name="Title"></param>
        public void changeTaskTitle(int columnID, int taskId, string Title)
        {
            if (columnID == boardColumns.Count - 1)
                throw new Exception("Task is done, cannot change it!");
            getColumnById(columnID).getTask(taskId).changetTitle(Title);
        }

        /// <summary>
        /// editing a task allowed only if its not in the last column!
        /// </summary>
        /// <param name="columnID"></param>
        /// <param name="taskId"></param>
        /// <param name="Description"></param>
        public void changeTaskDesc(int columnID, int taskId, string Description)
        {
            if (columnID == boardColumns.Count - 1)
                throw new Exception("Task is done, cannot change it!");
            getColumnById(columnID).getTask(taskId).changeDesc(Description);
        }

        /// <summary>
        /// editing a task allowed only if its not in the last column!
        /// </summary>
        /// <param name="columnID"></param>
        /// <param name="taskId"></param>
        /// <param name="finalDate"></param>
        public void changeTaskDueDate(int columnID, int taskId, DateTime finalDate)
        {
            if (columnID == boardColumns.Count - 1)
                throw new Exception("Task is done, cannot change it!");
            getColumnById(columnID).getTask(taskId).changeFinalDate(finalDate);
        }

        /// <summary>
        /// adding a new task to the board!
        /// </summary>
        /// <param name="id"> a unique id for the task </param>
        /// <param name="title"> task's title </param>
        /// <param name="desc"> task's description </param>
        /// <param name="finalDate"> task's dueDate </param>
        public Task addTask(int id, string title, string desc, DateTime finalDate)
        {
            Task task = new Task(Email, title, desc, finalDate);
            task.colName = boardColumns.ElementAt(0).getName();
            boardColumns.ElementAt(0).addTask(task, id);
            return task;
        }

        /// <summary>
        /// Converting BL-Board to DAL-Board so we can save it in the dataBase
        /// </summary>
        /// <returns></returns>
        public DataAccess_Layer.DTOs.BoardDTO ToDalObject()
        {
            DataAccess_Layer.DTOs.BoardDTO newBoard = new DataAccess_Layer.DTOs.BoardDTO(Email, taskId, id);
            return newBoard;
        }

        /// <summary>
        /// Saving the all changed done to the board in the dataBase to be updated! 
        /// </summary>
        public void Save()
        {
            ToDalObject().Save();
        }

        /// <summary>
        /// Constructor converting DAL-Board to BL-Board - in order to load data
        /// </summary>
        /// <param name="b"></param>
        public Board(DataAccess_Layer.DTOs.BoardDTO b)
        {
            taskId = b.taskID;
            Email = b.Email;
            columnDalController = new DataAccess_Layer.ColumnDalController();
            List<ColumnDTO> colDTO_list = columnDalController.SelectUserColumns(Email);
            boardColumns = new List<Column>();
            foreach (ColumnDTO col in colDTO_list)
            {
                boardColumns.Add(new Column(col));
            }
            id = b.ID;
        }
        public int getNumOfColumns()
        {
            return boardColumns.Count;
        }

        /// <summary>
        /// Supporting moving column to the right only the column isnt in the most right side!
        /// </summary>
        /// <param name="columnId"> unique id for the column to be moved! </param>
        public void moveColumnRight(int columnId)
        {
            if (columnId < 0 || columnId > boardColumns.Count - 1)
                throw new Exception("Id is wrong");
            if (columnId == boardColumns.Count - 1)
                throw new Exception("Can't move the RightMost molumn right!");
            Column temp = getColumnById(columnId);
            boardColumns.RemoveAt(columnId);
            boardColumns.Insert(columnId + 1, temp);
            boardColumns[columnId].columnID--;
            boardColumns[columnId + 1].columnID++;
            boardColumns[columnId].Save();
            boardColumns[columnId + 1].Save();
        }

        /// <summary>
        /// Supporting moving column to the left only the column isnt in the most left side!
        /// </summary>
        /// <param name="columnId"> unique id for the column to be moved! </param>
        public void moveColumnLeft(int columnId)
        {
            if (columnId <= 0 || columnId > boardColumns.Count - 1)
                throw new Exception("Can't move the LeftMost molumn left!");
            Column temp = getColumnById(columnId);
            boardColumns.RemoveAt(columnId);
            boardColumns.Insert(columnId - 1, temp);
            boardColumns[columnId].columnID++;
            boardColumns[columnId - 1].columnID--;
            boardColumns[columnId].Save();
            boardColumns[columnId - 1].Save();
        }

        /// <summary>
        /// Removing an exists column from the board!
        /// WE musts have minimum 2 columns on the board! so if the NumberOfColumns equals to 2 throws an error!
        /// All the tasks are moved to the neighbor column but only if the neighbor coloum has limit > NumOfHisTasks + NumOfOtherTasks
        /// </summary>
        /// <param name="columnID"> unique id for the column to be moved! </param>
        public void removeColumn(int columnID) // 0 (1) 2
        {
            taskDalController = new DataAccess_Layer.TaskDalController();
            columnDalController = new DataAccess_Layer.ColumnDalController();
            if (columnID < 0 || columnID >= boardColumns.Count || boardColumns.Count == 2)
                throw new Exception("Board must have minmum of 2 columns!");
            if (columnID == 0)
            {
                if (!(getColumnById(1).getLimit() == -1) && getColumnById(1).getNumberOfTasks() + getColumnById(0).getNumberOfTasks() > getColumnById(1).getLimit())
                    throw new Exception("Couldnt remove the column!");
                foreach (Task t in getColumnById(columnID).getTasks())
                {
                    taskDalController.Delete(t.ToDalObject());
                    boardColumns[1].addTask(t, t.getId());
                }
            }
            else
            {
                if (!(getColumnById(columnID - 1).getLimit() == -1) && getColumnById(columnID - 1).getNumberOfTasks() + getColumnById(columnID).getNumberOfTasks() > getColumnById(columnID - 1).getLimit())
                    throw new Exception("Couldnt remove the column!");
                foreach (Task t in getColumnById(columnID).getTasks())
                {
                    taskDalController.Delete(t.ToDalObject());
                    boardColumns[columnID - 1].addTask(t, t.getId());
                }
            }
            columnDalController.Delete(getColumnById(columnID).ToDalObject());
            //Shifting columns back.
            for (int i = columnID + 1; i < boardColumns.Count; i++)
            {
                boardColumns.ElementAt(i).columnID--; // done id = 1
                boardColumns.ElementAt(i).Save();
            }
            boardColumns.RemoveAt(columnID);
            Save();//Saving board due to columns decrement.
        }

        /// <summary>
        /// Support adding a new column to the board!
        /// </summary>
        /// <param name="columnId"> a unique Id of column </param>
        /// <param name="name"> a unique name of the column </param>
        public void addColumn(int columnId, string name)
        {
            if (columnId < 0 || columnId > boardColumns.Count)
                throw new Exception("position must be between 0 and " + (boardColumns.Count - 1));
            for (int i = 0; i < boardColumns.Count; i++)
                if (boardColumns[i].getName().Equals(name))
                    throw new Exception("Column already named with this name.");
            if (name.Length > 15)
                throw new Exception("Column name is too long");
            Column col = new Column(name);
            boardColumns.Insert(columnId, col);

            //Shifting columns.
            for (int i = columnId; i < boardColumns.Count; i++)
            {
                boardColumns.ElementAt(i).columnID = i;
                boardColumns.ElementAt(i).Save();
            }

            //Insert new column to database.
            col.Email = Email;
            col.columnID = columnId;
            columnDalController.Insert(col.ToDalObject());
            Save();//Saving board due to columns increment.
        }

        public void ChangeColumnName(int id, string newName)
        {
            if (getBoardColumnsNames().Contains(newName))
                throw new Exception("Column name already exists");
            if (id < 0 || id > boardColumns.Count - 1)
                throw new Exception("Given ID is out of bounds");
            boardColumns[id].SetName(newName);
        }

        public List<Task> GetallTasks()
        {
            List<Task> all = new List<Task>();
            for (int i = 0; i < boardColumns.Count; i++)
            {
                Column c = boardColumns.ElementAt(i);
                foreach (Task t in c.getTasks())
                    all.Add(t);
            }
            return all;
        }

        public void DeleteTask(int ColumnID, int taskID)
        {
            if (ColumnID < 0 || ColumnID >= boardColumns.Count - 1)
                throw new Exception("Task is done, cannot remove it");
            boardColumns[ColumnID].RemoveTask(taskID);
        }

        public string getEmail()
        {
            return Email;
        }
    }
}
