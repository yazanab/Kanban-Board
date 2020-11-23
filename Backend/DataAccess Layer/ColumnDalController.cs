using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccess_Layer
{
    class ColumnDalController : DalController
    {
        /// <summary>
        /// the Table Columns contains all the columns in the system!
        /// </summary>
        private const string ColumnsTableName = "Columns";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// simple constructor
        /// </summary>
        public ColumnDalController() : base(ColumnsTableName) { }

        /// <summary>
        /// Selecting all columns to a specifit user(email) from the dataBase and then convert then to list of DTO in order to load the data!
        /// </summary>
        /// <param name="email"> user's email </param>
        /// <returns></returns>
        public List<ColumnDTO> SelectUserColumns(string email)
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where Email=@Email order by columnID;";
                SQLiteDataReader dataReader = null;
                try
                {
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", email);
                    command.Parameters.Add(EmailParam);
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
                    log.Error("Error accured");
                    log.Debug(e.Message);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }
                    // closing the connection
                    command.Dispose();
                    connection.Close();
                }

            }
            return results.Cast<ColumnDTO>().ToList();
        }

        /// <summary>
        /// Converting SQLiteReader to DTO object so we handle it and deal with it!
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
            return result;
        }

        /// <summary>
        /// Inserting Column in the table 
        /// </summary>
        /// <param name="column"> the column to be inserted </param>
        /// <returns></returns>
        public bool Insert(ColumnDTO column)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {ColumnsTableName} ({ColumnDTO.myEmail},{ColumnDTO.columnName},{ColumnDTO.myLimit},{ColumnDTO.columnId}) " +
                         $"VALUES (@Email,@Name,@lmt,@columnID);";

                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", column.Email);
                    SQLiteParameter nameParam = new SQLiteParameter(@"Name", column.Name);
                    SQLiteParameter limitParam = new SQLiteParameter(@"lmt", column.Limit);
                    SQLiteParameter colidParam = new SQLiteParameter(@"columnID", column.ColID);

                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(limitParam);
                    command.Parameters.Add(colidParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured");
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

        /// <summary>
        /// Updating the table of columns to save changed!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id1"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public bool Update(string id, string id1, string attributeName, string attributeValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"update {ColumnsTableName} set [{attributeName}]=@{attributeName} where Name=@Name and Email=@Email";
                    SQLiteParameter colidParam = new SQLiteParameter(@"Name", id);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", id1);
                    SQLiteParameter attribute_Param = new SQLiteParameter(attributeName, attributeValue);
                    command.Parameters.Add(colidParam);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(attribute_Param);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured");
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
                    command.CommandText = $"update {ColumnsTableName} set [{attributeName}]=@{attributeName} where columnID=@columnID and Email=@Email";
                    SQLiteParameter colidParam = new SQLiteParameter(@"columnID", id);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", id1);
                    SQLiteParameter attribute_Param = new SQLiteParameter(attributeName, attributeValue);
                    command.Parameters.Add(colidParam);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(attribute_Param);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured");
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

        public bool UpdateName(string id, string id1, string attributeName, string attributeValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"update {ColumnsTableName} set [{attributeName}]=@{attributeName} where columnID=@columnID and Email=@Email";
                    SQLiteParameter colidParam = new SQLiteParameter(@"columnID", id);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", id1);
                    SQLiteParameter attribute_Param = new SQLiteParameter(attributeName, attributeValue);
                    command.Parameters.Add(colidParam);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(attribute_Param);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("Error accured");
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

        /// <summary>
        /// Deleting column from the table!
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool Delete(ColumnDTO column)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where Name=@Name and Email=@Email"
                };
                try
                {
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", column.Email);
                    SQLiteParameter NameParam = new SQLiteParameter(@"Name", column.Name);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(NameParam);
                    command.Prepare();
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error("Error accured");
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
