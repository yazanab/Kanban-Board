using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// The service for using the Kanban board.
    /// It allows executing all of the required behaviors by the Kanban board.
    /// You are not allowed (and can't due to the interfance) to change the signatures
    /// Do not add public methods\members! Your client expects to use specifically these functions.
    /// You may add private, non static fields (if needed).
    /// You are expected to implement all of the methods.
    /// Good luck.
    /// </summary>
    public class Service : IService
    {

        private userService userServe;
        private boardService boardServe;

        /// <summary>
        /// Simple public constructor.
        /// </summary>
        public Service()
        {
            userServe = new userService();
            boardServe = new boardService();
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LoadData()
        {
            Response myResponse = userServe.LoadData();
            if (myResponse.ErrorOccured)
            {
                return myResponse;
            }
            return boardServe.LoadData();
        }


        ///<summary>Remove all persistent data.</summary>
        public Response DeleteData()
        {
            Response rs = boardServe.DeleteData();
            if (rs.ErrorOccured)
                return rs;
            Response rs1 = userServe.DeleteData();
            if (!rs1.ErrorOccured)
            {
                userServe = new userService();
                boardServe = new boardService();
            }
            return rs1;
        }


        /// <summary>
        /// Registers a new user and creates a new board for him.
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string email, string password, string nickname)
        {
            Response response = userServe.Register(email, password, nickname);
            if (!response.ErrorOccured)
            {
                int id = boardServe.Register(email);
                userServe.SetId(email, id);
            }
            return response;
        }


        /// <summary>
        /// Registers a new user and joins the user to an existing board.
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <param name="emailHost">The email address of the host user which owns the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string email, string password, string nickname, string emailHost)
        {
            if (boardServe.foundHost(emailHost) == null)
            {
                return new Response("Host not found!");
            }
            Response rs = userServe.Register(email, password, nickname);
            if (rs.ErrorOccured)
                return rs;
            Response rs1 = boardServe.Register(email, emailHost);
            if (!rs1.ErrorOccured)
            {
                int Bid = boardServe.getId(email);
                userServe.SetId(email, Bid);
                return new Response();
            }
            else
                return rs1;
        }



        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
                return rs;
            return boardServe.AssignTask(email, columnOrdinal, taskId, emailAssignee);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        		
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response DeleteTask(string email, int columnOrdinal, int taskId)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
                return rs;
            return boardServe.DeleteTask(email, columnOrdinal, taskId);
        }



        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            if (!boardServe.setOnline(email))
            {
                return new Response<User>("Error: Login Failed");
            }
            Response<User> rs = userServe.Login(email, password);
            if (rs.ErrorOccured)
                boardServe.setOffline(email);
            return rs;
        }

        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            if (!boardServe.setOffline(email))
                return new Response("Error: Logout Failed");
            return userServe.Logout(email);
        }

        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<Board> GetBoard(string email)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Board>(rs.ErrorMessage);
            }
            return boardServe.GetBoard(email);
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.LimitColumnTasks(email, columnOrdinal, limit);
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string email, string title, string description, DateTime dueDate)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Task>(rs.ErrorMessage);
            }
            return boardServe.AddTask(email, title, description, dueDate);
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.UpdateTaskDueDate(email, columnOrdinal, taskId, dueDate);
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.UpdateTaskTitle(email, columnOrdinal, taskId, title);
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.UpdateTaskDescription(email, columnOrdinal, taskId, description);
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.AdvanceTask(email, columnOrdinal, taskId);
        }


        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<Column> GetColumn(string email, string columnName)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Column>(rs.ErrorMessage);
            }
            return boardServe.GetColumn(email, columnName);
        }

        /// <summary>
        /// Returns a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>

        public Response<Column> GetColumn(string email, int columnOrdinal)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Column>(rs.ErrorMessage);
            }
            return boardServe.GetColumn(email, columnOrdinal);
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return rs;
            }
            return boardServe.RemoveColumn(email, columnOrdinal);
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        /// <returns>A response object with a value set to the new Column, the response should contain a error message in case of an error</returns>
        public Response<Column> AddColumn(string email, int columnOrdinal, string Name)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Column>(rs.ErrorMessage);
            }
            return boardServe.AddColumn(email, columnOrdinal, Name);

        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnRight(string email, int columnOrdinal)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Column>(rs.ErrorMessage);
            }
            return boardServe.MoveColumnRight(email, columnOrdinal);

        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnLeft(string email, int columnOrdinal)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response<Column>(rs.ErrorMessage);
            }
            return boardServe.MoveColumnLeft(email, columnOrdinal);

        }

        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            Response rs = userServe.validateLoggedIn(email);
            if (rs.ErrorOccured)
            {
                return new Response(rs.ErrorMessage);
            }
            return boardServe.ChangeColumnName(email, columnOrdinal, newName);
        }

    }
}