using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Presentation.Model;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Presentation
{
    public class BackendController
    {
        public Service service { get; private set; }
        public BackendController()
        {
            service = new Service();
            service.LoadData();
        }

        public void Register(string email, string password, string nickname)
        {
            Response rs = service.Register(email, password, nickname);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void Register(string email, string password, string nickname, string hostemail)
        {
            Response rs = service.Register(email, password, nickname, hostemail);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public UserModel Login(string email, string password)
        {
            Response<User> rs = service.Login(email, password);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new UserModel(rs.Value.Email, rs.Value.Nickname, this);
        }

        public void Logout(string email)
        {
            Response rs = service.Logout(email);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public BoardModel GetBoard(string email)
        {
            Response<Board> rs = service.GetBoard(email);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            List<ColumnModel> columns = new List<ColumnModel>();
            int i = 0;
            foreach (string c in rs.Value.ColumnsNames)
            {
                Response<Column> temp = service.GetColumn(email, c);
                ColumnModel col = new ColumnModel(temp.Value, this, email, i);
                columns.Add(col);
                i++;
            }
            return new BoardModel(columns, rs.Value.emailCreator, email, this);
        }

        public ColumnModel GetColumn(string email, int columnOrdinal)
        {
            Response<Column> rs = service.GetColumn(email, columnOrdinal);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new ColumnModel(rs.Value, this, email, columnOrdinal);
        }

        public ColumnModel AddColumn(string email, int columnOrdinal, string name)
        {
            Response<Column> rs = service.AddColumn(email, columnOrdinal, name);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new ColumnModel(rs.Value, this, email, columnOrdinal);
        }

        public ColumnModel MoveColumnLeft(string email, int columnOrdinal)
        {
            Response<Column> rs = service.MoveColumnLeft(email, columnOrdinal);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new ColumnModel(rs.Value, this, email, columnOrdinal);
        }

        public ColumnModel MoveColumnRight(string email, int columnOrdinal)
        {
            Response<Column> rs = service.MoveColumnRight(email, columnOrdinal);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new ColumnModel(rs.Value, this, email, columnOrdinal);
        }

        public void RemoveColumn(string email, int columnOrdinal)
        {
            Response rs = service.RemoveColumn(email, columnOrdinal);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public TaskModel AddTask(string email, string title, string description, DateTime dueDate)
        {
            Response<Task> rs = service.AddTask(email, title, description, dueDate);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
            return new TaskModel(rs.Value.Id, rs.Value.CreationTime, rs.Value.Title, rs.Value.Description, rs.Value.DueDate, rs.Value.emailAssignee, this, email, 0);
        }

        public void AssignTask(string email, int columnOrdinal, int taskid, string emailAssignee)
        {
            Response rs = service.AssignTask(email, columnOrdinal, taskid, emailAssignee);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void AdvanceTask(string email, int columnOrdinal, int taskid)
        {
            Response rs = service.AdvanceTask(email, columnOrdinal, taskid);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void DeleteTask(string email, int columnOrdinal, int taskid)
        {
            Response rs = service.DeleteTask(email, columnOrdinal, taskid);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            Response rs = service.LimitColumnTasks(email, columnOrdinal, limit);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void UpdateTaskTitle(string email, int columnOrdinal, int taskid, string title)
        {
            Response rs = service.UpdateTaskTitle(email, columnOrdinal, taskid, title);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void UpdateTaskDescription(string email, int columnOrdinal, int taskid, string description)
        {
            Response rs = service.UpdateTaskDescription(email, columnOrdinal, taskid, description);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }

        public void UpdateTaskDueDate(string email, int columnOrdinal, int taskid, DateTime dueDate)
        {
            Response rs = service.UpdateTaskDueDate(email, columnOrdinal, taskid, dueDate);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }
        public void ChangeName(string email, int id, string newName)
        {
            Response rs = service.ChangeColumnName(email, id, newName);
            if (rs.ErrorOccured)
                throw new Exception(rs.ErrorMessage);
        }
    }
}
