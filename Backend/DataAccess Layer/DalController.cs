using IntroSE.Kanban.Backend.DataAccess_Layer.DTOs;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System;

namespace IntroSE.Kanban.Backend.DataAccess_Layer
{
    internal abstract class DalController
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;
        protected readonly string _db_name;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        /// <summary>
        /// SETTING UP THE CONNECTION.
        /// So the _db_name is the file name for the dataBase to save!
        /// </summary>
        /// <param name="tableName"></param>
        public DalController(string tableName)
        {
            _db_name = "KanbanDB.db";
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), _db_name));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;

            //checking if the file exists, if not create a new one!
            if (!File.Exists(path))
            {
                string creation_query = @"CREATE TABLE 'Users' (
                'Email' TEXT NOT NULL UNIQUE,
                'Nickname'  TEXT NOT NULL,
	            'Password'  TEXT,
                'boardID'   INTEGER,
	            PRIMARY KEY('Email')
                );
                CREATE TABLE 'Boards'(
                    'Email' TEXT NOT NULL,
                    'taskId'    INTEGER,
                    'boardId'   INTEGER,
                    PRIMARY KEY('Email'),
                    FOREIGN KEY('Email') REFERENCES 'Users'('Email') ON UPDATE NO ACTION ON DELETE CASCADE
                );
                CREATE TABLE 'Columns'(
                    'Email' TEXT NOT NULL,
                    'Name'  TEXT NOT NULL,
                    'lmt'   INTEGER DEFAULT 100,
                    'columnID'  INTEGER,
                    FOREIGN KEY('Email') REFERENCES 'Users'('Email') ON UPDATE NO ACTION ON DELETE CASCADE,
                    PRIMARY KEY('Email', 'Name', 'columnID')
                );
                CREATE TABLE 'Tasks'(
                    'taskID'    INTEGER,
                    'Assignee' TEXT,
                    'columnName'    TEXT,
                    'Title' TEXT NOT NULL,
                    'Description'   TEXT,
                    'CreationDate'  TEXT,
                    'DueDate'   TEXT,
                    'hostEmail' TEXT,
                    PRIMARY KEY('hostEmail', 'taskID'),
                    FOREIGN KEY('hostEmail') REFERENCES 'Users'('Email') ON UPDATE NO ACTION ON DELETE CASCADE
                ); ";
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    command.CommandText = creation_query;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message);
                        log.Debug("Error accured");
                    }
                    finally
                    {
                        // closing connection
                        command.Dispose();
                        connection.Close();
                    }

                }
            }

        }



        /// <summary>
        /// GENERIC SELECTOR FOR ALL DATA TYPES FROM DB.
        /// </summary>
        /// <returns></returns>
        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
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

                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }

        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        //DATA ACCESS LAYER Controllers.
        public UserDalController GetUserDalController()
        {
            return new UserDalController();
        }

        public TaskDalController GetTaskDalController()
        {
            return new TaskDalController();
        }

        public ColumnDalController GetColumnDalController()
        {
            return new ColumnDalController();
        }

        public BoardDalController GetBoardDalController()
        {
            return new BoardDalController();
        }
        /// <summary>
        /// Deleting all persisted data in the dataBaseFile
        /// </summary>
        /// <returns></returns>
        public bool DeleteData()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {

                    command.CommandText = $"DELETE  FROM {_tableName}";

                    command.Prepare();
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res >= 0;
            }
        }
    }
}