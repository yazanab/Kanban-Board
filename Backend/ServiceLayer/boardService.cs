using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    class boardService
    {
        private Business_Layer.BoardPackage.BoardController boardController;

        public boardService()
        {
            boardController = new Business_Layer.BoardPackage.BoardController();
        }

        public Response LoadData()
        {
            try
            {
                boardController.LoadAllBoards();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public int Register(string email)
        {
            bool toReturn = boardController.addBoard(email);
            int id;
            if (toReturn)
                id = boardController.getIdbyEmail(email);
            else
                id = -1;
            return id;
        }
        public Response<Board> GetBoard(string email)
        {
            try
            {
                Board b = new Board(boardController.getBoard(email));
                return new Response<Board>(b);
            }
            catch (Exception ex)
            {
                return new Response<Board>(ex.Message);
            }
        }
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            try
            {
                boardController.LimitColumnTasks(email, columnOrdinal, limit);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response<Task> AddTask(string email, string title, string description, DateTime dueDate)
        {
            try
            {
                boardController.addTask(email, title, description, dueDate);
                Task t = new Task(boardController.GetColumnById(email, 0).getTasks().Last());
                return new Response<Task>(t);
            }
            catch (Exception ex)
            {
                return new Response<Task>(ex.Message);
            }
        }
        public Response UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                boardController.updateTaskDueDate(email, columnOrdinal, taskId, dueDate);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {

            try
            {
                boardController.updateTaskTitle(email, columnOrdinal, taskId, title);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            try
            {
                boardController.updateTaskDesc(email, columnOrdinal, taskId, description);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                boardController.advanceTask(email, columnOrdinal, taskId);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }

        public Response Register(string email, string emailHost)
        {
            try
            {
                boardController.JoinBoard(email, emailHost);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }

        public Response<Column> GetColumn(string email, string columnName)
        {
            try
            {
                Column c = new Column(boardController.GetColumnByName(email, columnName));
                return new Response<Column>(c);
            }
            catch (Exception ex)
            {
                return new Response<Column>(ex.Message);
            }
        }

        public Response AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                boardController.AssignTask(email, columnOrdinal, taskId, emailAssignee);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }

        public Response<Column> GetColumn(string email, int columnOrdinal)
        {
            try
            {
                Column c = new Column(boardController.GetColumnById(email, columnOrdinal));
                return new Response<Column>(c);
            }
            catch (Exception ex)
            {
                return new Response<Column>(ex.Message);
            }
        }
        public bool setOnline(string email)
        {
            return boardController.setOnline(email);
        }
        public bool setOffline(string email)
        {
            return boardController.setOffline(email);
        }
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            try
            {
                boardController.removeColumn(email, columnOrdinal);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response<Column> AddColumn(string email, int columnOrdinal, string Name)
        {
            try
            {
                boardController.addColumn(email, columnOrdinal, Name);
                Column c = new Column(boardController.GetColumnById(email, columnOrdinal));
                return new Response<Column>(c);
            }
            catch (Exception ex)
            {
                return new Response<Column>(ex.Message);
            }
        }
        public Response<Column> MoveColumnRight(string email, int columnOrdinal)
        {
            try
            {
                boardController.moveColumnRight(email, columnOrdinal);
                Column c = new Column(boardController.GetColumnById(email, columnOrdinal));
                return new Response<Column>(c);
            }
            catch (Exception ex)
            {
                return new Response<Column>(ex.Message);
            }
        }
        public Response<Column> MoveColumnLeft(string email, int columnOrdinal)
        {
            try
            {
                boardController.moveColumnLeft(email, columnOrdinal);
                Column c = new Column(boardController.GetColumnById(email, columnOrdinal));
                return new Response<Column>(c);
            }
            catch (Exception ex)
            {
                return new Response<Column>(ex.Message);
            }
        }
        public Response DeleteData()
        {
            try
            {
                boardController.DeleteData();
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }

        public Response ChangeColumnName(string email, int id, string newName)
        {
            try
            {
                boardController.ChangeColumnName(email, id, newName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response DeleteTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                boardController.DeleteTask(email, columnOrdinal, taskId);
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public string foundHost(string host)
        {
            return boardController.foundHost(host);
        }
        public int getId(string email)
        {
            return boardController.getIdbyEmail(email);
        }
    }
}
