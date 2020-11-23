using System;
using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.DataAccess_Layer
{
    class UserDalController : DalController
    {
        private const string UsersTableName = "Users";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public UserDalController() : base(UsersTableName)
        {

        }

        public List<UserDTO> SelectAllUsers()
        {
            List<UserDTO> result = Select().Cast<UserDTO>().ToList();
            return result;
        }


        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO result = new UserDTO(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
            return result;
        }

        public bool Insert(UserDTO user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {UsersTableName} ({UserDTO.EmailColumnName},{UserDTO.NickNameColumnName},{UserDTO.PassColumnName},{UserDTO.boardIDColumnName}) " +
                         $"VALUES (@Email,@Nickname,@Password,@boardID);";

                    SQLiteParameter EmailParam = new SQLiteParameter(@"Email", user.Email);
                    SQLiteParameter NicknameParam = new SQLiteParameter(@"Nickname", user.Nickname);
                    SQLiteParameter PasswordParam = new SQLiteParameter(@"Password", user.Password);
                    SQLiteParameter boardidParam = new SQLiteParameter(@"boardID", user.boardID);
                    command.Parameters.Add(EmailParam);
                    command.Parameters.Add(NicknameParam);
                    command.Parameters.Add(PasswordParam);
                    command.Parameters.Add(boardidParam);
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

        public bool Update(string id, string attributeName, string attributeValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"update {UsersTableName} set [{attributeName}]=@{attributeName} where Email=@Email";
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
