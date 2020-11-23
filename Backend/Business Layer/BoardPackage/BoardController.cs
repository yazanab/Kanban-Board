using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.Business_Layer.BoardPackage
{
    /// <summary>
    /// This our board manager, it manges all the new/exists boards of the users!
    /// There are two important fields!
    /// 1. boardController dictionary which stores <Email,Board> in the system ...
    /// 2. userOnline dictionary which stores <Email,Online> of the current online user, so he cannot achive anything without being logged in.
    /// </summary>
    class BoardController
    {
        private Dictionary<string, List<string>> UsersOfBoard; // all users that has joined the board and the host
        private Dictionary<string, Board> boardController;
        private Dictionary<string, bool> userOnline;  // in order to check wheter a user is online or not!
        private DataAccess_Layer.BoardDalController DataOfBoard;
        private DataAccess_Layer.TaskDalController DataOfTask;
        private DataAccess_Layer.ColumnDalController DataOfColumn;
        private int id;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Simple constructor
        /// </summary>
        public BoardController()
        {
            boardController = new Dictionary<string, Board>();
            userOnline = new Dictionary<string, bool>();
            UsersOfBoard = new Dictionary<string, List<string>>();
            DataOfBoard = new DataAccess_Layer.BoardDalController();
            DataOfTask = new DataAccess_Layer.TaskDalController();
            DataOfColumn = new DataAccess_Layer.ColumnDalController();
            id = 0;
        }

        /// <summary>
        /// Each email in the system is unique, so getting a board by user's email is a good idea.
        /// </summary>
        /// <param name="email"> the user's email </param>
        /// <returns></returns>
        public Board getBoard(string email)
        {
            string host;
            if (UsersOfBoard.ContainsKey(email))
                host = email;
            else
                host = searchHost(email);

            log.Info("User followed with Email: " + email + " got the board successfully!");
            return boardController[host];
        }

        /// <summary>
        /// Adding a new Board to system (when registering a new user)
        /// </summary>
        /// <param name="email"> the user's email to add a board for him then inser to the dictionary </param>
        /// <returns></returns>
        public bool addBoard(string email)
        {
            Board board = new Board(email);
            boardController.Add(email, board);
            UsersOfBoard.Add(email, new List<string>());
            DataOfBoard.Insert(board.ToDalObject());
            board.id = id;
            id++;
            board.Save();
            return true;
        }

        /// <summary>
        /// User can create a new board or register it self for existing board(host)!
        /// </summary>
        /// <param name="email"> the new user's email to join </param>
        /// <param name="emailHost"> the exists user's email (host) that the new user want to join to! </param>
        public void JoinBoard(string email, string emailHost)
        {
            UsersOfBoard[emailHost].Add(email);
        }

        /// <summary>
        /// Advancing a task to the next column!
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="columnID"> Ordinal of the column(unique) that contains the task to be moved </param>
        /// <param name="taskId"> the unique task id to be moved </param>
        public void advanceTask(string myEmail, int columnID, int taskId)
        {
            string host = checkEmail(taskId, myEmail);
            if (host == null)
                throw new Exception("Email is invalid!");
            boardController[host].advanceTask(columnID, taskId);
            log.Info("User followed with Email: " + myEmail + " advanced task successfully!");
        }

        /// <summary>
        /// Allowing to updated a task title for each user(email)!
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="columnId"></param>
        /// <param name="taskId"></param>
        /// <param name="newTitle"> the new Title to be changed to </param>
        public void updateTaskTitle(string myEmail, int columnId, int taskId, string newTitle)
        {
            string host = checkEmail(taskId, myEmail);
            if (host == null || newTitle == null)
                throw new Exception("Invalid input!");
            boardController[host].changeTaskTitle(columnId, taskId, newTitle);
            log.Info("BoardController: Updating task title succeeded!");
        }

        /// <summary>
        /// Here we allow updating/changing the task description for all the users(emails)
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="columnId"></param>
        /// <param name="taskId"></param>
        /// <param name="newDesc"> the new Description to be changed to </param>
        public void updateTaskDesc(string myEmail, int columnId, int taskId, string newDesc)
        {
            string host = checkEmail(taskId, myEmail);
            if (host == null)
                throw new Exception("Email is invalid!");
            boardController[host].changeTaskDesc(columnId, taskId, newDesc);
            log.Info("Updating task description succeeded!");
        }

        /// <summary>
        /// Allowing to change Task dueDate for all the users in the system! 
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="columnId"></param>
        /// <param name="taskId"></param>
        /// <param name="finalDate"> the dueDate to be changed to </param>
        public void updateTaskDueDate(string myEmail, int columnId, int taskId, DateTime finalDate)
        {
            string host = checkEmail(taskId, myEmail);
            if (host == null || finalDate == null)
                throw new Exception("Invalid input!");
            boardController[host].changeTaskDueDate(columnId, taskId, finalDate);
            log.Info("BoardController: Updating task duedate succeeded!");
        }

        /// <summary>
        /// supporting limiting the column number of tasks
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="columnOrdinal"></param>
        /// <param name="limit"> the column new limit we want to be changed to </param>
        public void LimitColumnTasks(string myEmail, int columnOrdinal, int limit)
        {
            if (!boardController.ContainsKey(myEmail))
            {
                log.Warn("User is not exist!");
                throw new Exception("You are not the host!");
            }
            boardController[myEmail].getColumnById(columnOrdinal).limitColumn(limit);
            Board b = boardController[myEmail];
            b.getColumnById(columnOrdinal).ToDalObject().Limit = limit;
            log.Info("BoardController: Limiting column's number of tasks succeeded!");
        }

        /// <summary>
        /// Adding a new Task to the Board!
        /// </summary>
        /// <param name="myEmail"> user's email so we know which board to play with </param>
        /// <param name="title"> Task title shouldnt be empty or more than 50 in length</param>
        /// <param name="desc"> task Description shouldnt be more 300 chars in length! </param></optional>
        /// <param name="finalDate"></param>
        public Task addTask(string myEmail, string title, string desc, DateTime finalDate)
        {
            string host = myEmail;
            if (!boardController.ContainsKey(host))
                host = searchHost(myEmail);
            if (host == null || title == null)
                throw new Exception("Invalid input!");
            Board b = boardController[host];
            int id = b.getTaskId();
            Task t = boardController[host].addTask(id, title, desc, finalDate);
            AssignTask(host, 0, id, myEmail);
            b.setTaskId(id + 1);
            b.Save();
            log.Info("BoardController: Adding a new task to the board succeeded!");
            return t;
        }
        public Column GetColumnByName(string myEmail, string columnName)
        {
            string host = myEmail;
            if (!boardController.ContainsKey(myEmail))
                host = searchHost(myEmail);
            if (host == null || columnName == null)
                throw new Exception("Invalid input!");
            return boardController[host].getColumnByName(columnName);
        }
        public Column GetColumnById(string myEmail, int columnId)
        {
            string host = myEmail;
            if (!boardController.ContainsKey(myEmail))
                host = searchHost(myEmail);
            if (host == null)
                throw new Exception("Invalid input!");
            return boardController[host].getColumnById(columnId);
        }

        /// <summary>
        /// We are allowed to assume that only one user could be online in the same time!
        /// so here we update our dictionary(size of 1) so it contains the user's email and the status(online/offline)
        /// </summary>
        /// <param name="myEmail"></param>
        /// <returns></returns>
        public bool setOnline(string myEmail)
        {
            if (string.IsNullOrEmpty(myEmail))
                return false;
            if ((!boardController.ContainsKey(myEmail) && !searchAssignee(myEmail)) || userOnline.Count != 0)
                // which means someone is online so you cant login before he logs out
                return false;
            userOnline.Add(myEmail, true);
            return true;

        }
        public bool setOffline(string myEmail)
        {
            if (myEmail == null)
                return false;
            if ((!boardController.ContainsKey(myEmail) && !searchAssignee(myEmail)) || userOnline.Count == 0 || !userOnline.ContainsKey(myEmail))
                // which means no one is online so you cant logout before logging in
                return false;
            userOnline.Remove(myEmail);
            return true;
        }

        /// <summary>
        /// Loading all the boards in the dataBase to the system(Ram/Code)!
        /// </summary>
        public void LoadAllBoards()
        {
            List<DataAccess_Layer.DTOs.BoardDTO> boards = DataOfBoard.SelectAllBoards();
            foreach (DataAccess_Layer.DTOs.BoardDTO b in boards)
            {
                Board bordddd = new Board(b);
                boardController.Add(b.Email, bordddd);
                UsersOfBoard.Add(b.Email, new List<string>());
                List<string> participants = DataOfBoard.SelectAllParticipants(b.ID);
                UsersOfBoard[b.Email] = participants;
                UsersOfBoard[b.Email].Remove(b.Email);
            }
            id = boardController.Count;
        }

        /// <summary>
        /// Removing an exists column, otherwise throws an error!
        /// WE musts have minimum 2 columns on the board! so if the NumberOfColumns equals to 2 throws an error!
        /// </summary>
        /// <param name="myEmail"> </param>
        /// <param name="columnOrdinal"> unique ordinal foreach column so we know which one to deal with </param>
        public void removeColumn(string myEmail, int columnOrdinal)
        {
            string host = myEmail;
            if (!boardController.ContainsKey(myEmail))
                throw new Exception("Only the host can remove columns!");
            boardController[host].removeColumn(columnOrdinal);
        }

        /// <summary>
        /// adding a new column to the system!
        /// </summary>
        /// <param name="myEmail"> user's unique email</param>
        /// <param name="columnOrdinal"> a unique ordinal of the column </param>
        /// <param name="Name"> a unique name of column</param>
        public void addColumn(string myEmail, int columnOrdinal, string Name)
        {
            if (Name == null)
            {
                log.Warn("Getting a null!");
                throw new Exception("Please enter a name!");
            }
            if (!boardController.ContainsKey(myEmail))
                throw new Exception("Only the host can add columns!");
            boardController[myEmail].addColumn(columnOrdinal, Name);
        }

        /// <summary>
        /// Supporting moving an existed column to the right, otherwise throws an error!
        /// </summary>
        /// <param name="myEmail"></param>
        /// <param name="columnOrdinal"></param>
        public void moveColumnRight(string myEmail, int columnOrdinal)
        {
            if (!boardController.ContainsKey(myEmail))
            {
                log.Warn("User is not exist!");
                throw new Exception("Only the host can move columns!");
            }
            boardController[myEmail].moveColumnRight(columnOrdinal);
        }

        /// <summary>
        /// Supporting moving an existed column to the left, otherwise throws an error!
        /// </summary>
        /// <param name="myEmail"></param>
        /// <param name="columnOrdinal"></param>
        public void moveColumnLeft(string myEmail, int columnOrdinal)
        {
            if (!boardController.ContainsKey(myEmail))
            {
                log.Warn("User is not exist!");
                throw new Exception("Only the host can move columns!");
            }
            boardController[myEmail].moveColumnLeft(columnOrdinal);
        }

        /// <summary>
        /// Deleting all the boards/columns/tasks stored in the data base!
        /// </summary>
        public void DeleteData()
        {

            bool rs1 = DataOfTask.DeleteData();
            if (!rs1)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
            bool rs2 = DataOfColumn.DeleteData();
            if (!rs2)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
            bool rs3 = DataOfBoard.DeleteData();
            if (!rs3)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
        }

        /// <summary>
        /// A very usefull function, given an assignee email, it searches for the host.
        /// </summary>
        /// <param name="email"> the assignee email </param>
        /// <returns> the the email found, return the host's email, otherwise returns null </returns>
        public string searchHost(string email)
        {
            for (int i = 0; i < UsersOfBoard.Count; i++)
            {
                if (UsersOfBoard.ElementAt(i).Value.Contains(email))
                {
                    return UsersOfBoard.ElementAt(i).Key;
                }
            }
            return null;
        }

        /// <summary>
        /// checks whether the email is assigned for a host user!
        /// </summary>
        /// <param name="host"> the email we want to search for </param>
        /// <returns> if the host's email found we turn it, else return null </returns>
        public string foundHost(string host)
        {
            if (host == null)
                return null;
            if (boardController.ContainsKey(host))
                return host;
            return null;
        }

        /// <summary>
        /// Helper function checks if the given taskId assigned to the user whos email is @assignee 
        /// This functions helps us to valid updating Task details 
        /// </summary>
        /// <param name="taskId"> the task id </param>
        /// <param name="assignee"> the email of the user that we want to know if the task assigned to him </param>
        /// <returns> returns the host of the board that the task with the given taskId assigned to! </returns>
        public string checkEmail(int taskId, string assignee)
        {
            string theHost = null;
            if (boardController.ContainsKey(assignee))
                theHost = assignee;
            else
                theHost = searchHost(assignee);
            Board b = boardController[theHost];
            List<Task> all = b.GetallTasks();
            foreach (Task t in all)
            {
                if (t.getId() == taskId && assignee.Equals(t.GetEmail()))
                    return theHost;
            }
            return null;
        }

        /// <summary>
        /// Assigning Task to the given user's emailAssignee
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="emailAssignee"></param>
        public void AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            if (email == null || emailAssignee == null)
                throw new Exception("Invalid email!");
            string host = null;
            if (boardController.ContainsKey(email))
            {
                host = email;
                if (!boardController.ContainsKey(emailAssignee) && !host.Equals(searchHost(emailAssignee)))
                    throw new Exception("Email not found!");
            }
            else if (boardController.ContainsKey(emailAssignee))
            {
                host = emailAssignee;
                if (!host.Equals(searchHost(email)))
                    throw new Exception("Email not found!");
            }
            else
            {
                if (!inTheSameBoard(email, emailAssignee))
                    throw new Exception("Email not found!");
                host = searchHost(email);
            }
            Board b = boardController[host];
            if (columnOrdinal == b.getNumOfColumns() - 1)
                throw new Exception("Task in done, cannt change it!");
            Column c = b.getColumnById(columnOrdinal);
            if (!c.getTask(taskId).GetEmail().Equals(email) || !c.getTask(taskId).getHostEamil().Equals(host))
                throw new Exception("You are not the owner!");
            c.getTask(taskId).AssignEmail(emailAssignee);
            c.Save();
        }

        public bool inTheSameBoard(string email1, string email2)
        {
            foreach (List<string> emails in UsersOfBoard.Values)
            {
                if (emails.Contains(email1) && emails.Contains(email2))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// checks whether the given @email is assigned for a board!
        /// return true if so, otherwise return false!
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool searchAssignee(string email)
        {
            foreach (List<string> emails in UsersOfBoard.Values)
            {
                if (email.Contains(email))
                    return true;
            }
            return false;
        }

        public void ChangeColumnName(string email, int id, string newName)
        {
            if (!boardController.ContainsKey(email))
                throw new Exception("You are not the host!");
            boardController[email].ChangeColumnName(id, newName);
        }

        /// <summary>
        /// Deleteing an existed task in a column!
        /// </summary>
        /// <param name="email"> the email of the user! </param>
        /// <param name="columnOrdinal"> the unique columnId that task assigned to! </param>
        /// <param name="taskId"> the task unique id we want to remove from the board! </param>
        public void DeleteTask(string email, int columnOrdinal, int taskId)
        {
            string host = checkEmail(taskId, email);
            if (host == null)
                throw new Exception("Email is invalid!");
            Board b = boardController[host];
            b.DeleteTask(columnOrdinal, taskId);
        }

        public int getIdbyEmail(string email)
        {
            if (email == null)
                throw new Exception("Invalid");
            foreach (string e in boardController.Keys)
            {
                if (e.Equals(email))
                    return boardController[e].id;
            }
            string host = searchHost(email);
            if (host == null)
                return -1;
            else
            {
                return boardController[host].id;
            }
        }
    }
}
