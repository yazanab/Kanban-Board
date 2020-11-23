using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccess_Layer
{
    class BoardDalController : DalController
    {
        /// <summary>
        /// the name of the Table in the data base to save!
        /// </summary>
        private const string BoardsTableName = "Boards";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// simple constructor
        /// </summary>
        public BoardDalController() : base(BoardsTableName) { }

        /// <summary>
        /// Selecting all the boards in the dataBase file and converting them to list of BoardDTO
        /// </summary>
        /// <returns></returns>
        public List<BoardDTO> SelectAllBoards()
        {
            List<BoardDTO> result = Select().Cast<BoardDTO>().ToList();
            return result;
        }

        public List<string> SelectAllParticipants(int id)
        {
            List<string> results = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from Users where boardID=@boardID;";
                SQLiteDataReader dataReader = null;
                try
                {
                    SQLiteParameter EmailParam = new SQLiteParameter(@"boardID", id);
                    command.Parameters.Add(EmailParam);
                    command.Prepare();
                    connection.Open();

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(dataReader.GetString(0));
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
            return results;
        }

        /// <summary>
        /// converts SQLiteReader to DTO object so we can handle it and deal with it!
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardDTO result = new BoardDTO(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2));
            return result;
        }

        /// <summary>
        /// Inserting a new Board to the table in the dataBase!
        /// </summary>
        /// <param name="board"> the board to be inserted! </param>
        /// <returns></returns>
        public bool Insert(BoardDTO board)
        {
            // creatng a new connection!
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {BoardsTableName} ({BoardDTO.myEmail},{BoardDTO.myTaskId},{BoardDTO.id}) " +
                         $"VALUES (@Email,@taskId,@boardId);";

                    SQLiteParameter colParam = new SQLiteParameter(@"Email", board.Email);
                    SQLiteParameter EmailParam = new SQLiteParameter(@"taskId", board.taskID);
                    SQLiteParameter idParam = new SQLiteParameter(@"boardId", board.ID);
                    command.Parameters.Add(colParam);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(idParam);
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
                    //closing the connection when ending!
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        /// <summary>
        /// Updating the board details in the dataBase file!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public bool Update(string id, string attributeName, string attributeValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"update {BoardsTableName} set [{attributeName}]=@{attributeName} where Email=@Email";
                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", id);
                    SQLiteParameter attribute_Param = new SQLiteParameter(attributeName, attributeValue);
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
    }
}
