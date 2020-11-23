using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccess_Layer
{
    class TaskDalController : DalController
    {
        private const string TasksTableName = "Tasks";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskDalController() : base(TasksTableName)
        {

        }

        public List<TaskDTO> SelectAllTasks()
        {
            List<TaskDTO> result = Select().Cast<TaskDTO>().ToList();
            return result;
        }

        public List<TaskDTO> SelectUserTasks(string email, string colName)
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where hostEmail=@hostEmail and columnName=@columnName order by taskID;";
                SQLiteDataReader dataReader = null;
                try
                {
                    SQLiteParameter EmailParam = new SQLiteParameter(@"hostEmail", email);
                    SQLiteParameter colidParam = new SQLiteParameter(@"columnName", colName);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(colidParam);

                    command.Prepare();
                    connection.Open();

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));
                    }
                }
                catch (Exception e)
                {
                    log.Error("Error accured!");
                    log.Debug(e.Message);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            return results.Cast<TaskDTO>().ToList();
        }
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.IsDBNull(4) ? "" : reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7));
            return result;
        }

        public bool Insert(TaskDTO task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {TasksTableName} ({TaskDTO.TaskId},{TaskDTO.myAssignee},{TaskDTO.columnName},{TaskDTO.myTitle},{TaskDTO.myDescription},{TaskDTO.myCreationDate},{TaskDTO.myDueDate},{TaskDTO.myEmail}) " +
                         $"VALUES (@taskID,@Assignee,@columnName,@Title,@Description,@CreationDate,@DueDate,@hostEmail);";

                    SQLiteParameter taskParam = new SQLiteParameter(@"taskID", task.taskId);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Assignee", task.Assignee);
                    SQLiteParameter colnameParam = new SQLiteParameter(@"columnName", task.colName);
                    SQLiteParameter titleParam = new SQLiteParameter(@"Title", task.Title);
                    SQLiteParameter descParam = new SQLiteParameter(@"Description", task.Description);
                    SQLiteParameter creationParam = new SQLiteParameter(@"CreationDate", task.CreationDate);
                    SQLiteParameter dueParam = new SQLiteParameter(@"DueDate", task.DueDate);
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"hostEmail", task.Email);

                    command.Parameters.Add(taskParam);
                    command.Parameters.Add(assigneeParam);
                    command.Parameters.Add(colnameParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descParam);
                    command.Parameters.Add(creationParam);
                    command.Parameters.Add(dueParam);
                    command.Parameters.Add(EmailParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured!");
                    log.Debug(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        public bool Update(int id, string id1, string attributeName, string attributeValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"update {TasksTableName} set [{attributeName}]=@{attributeName} where taskID=@taskID and hostEmail=@hostEmail";

                    SQLiteParameter taskidParam = new SQLiteParameter(@"taskID", id);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"hostEmail", id1);
                    SQLiteParameter attribute_Param = new SQLiteParameter(attributeName, attributeValue);
                    command.Parameters.Add(taskidParam);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(attribute_Param);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured!");
                    log.Debug(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        public bool Delete(TaskDTO task)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where taskID={task.taskId} and hostEmail=@hostEmail and columnName=@columnName"
                };
                try
                {
                    SQLiteParameter EmailParam = new SQLiteParameter(@"hostEmail", task.Email);
                    SQLiteParameter colParam = new SQLiteParameter(@"columnName", task.colName);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(colParam);
                    command.Prepare();
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error("Error accured!");
                    log.Debug(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }
    }
}
