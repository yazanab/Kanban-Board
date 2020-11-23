using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Presentation.Model
{
    public class BoardModel : NotifiableModelObject
    {
        public ObservableCollection<ColumnModel> Columns { get; set; }

        public string emailCreator { get; set; }
        private string UserEmail;

        private ObservableCollection<TaskModel> taskList;
        public ObservableCollection<TaskModel> TaskList
        {
            get { return taskList; }
            set { taskList = value; RaisePropertyChanged("TaskList"); }
        }

        /// <summary>
        /// simply constructor
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="emailCreator"></param>
        /// <param name="userEmail"></param>
        /// <param name="controller"></param>
        internal BoardModel(List<ColumnModel> columns, string emailCreator, string userEmail, BackendController controller) : base(controller)
        {
            Columns = new ObservableCollection<ColumnModel>(columns);
            this.emailCreator = emailCreator;
            this.UserEmail = userEmail;
        }


        /// <summary>
        /// adding a new task to the board by the user!
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        public void AddTask(string title, string description, DateTime dueDate)
        {
            Columns[0].addTask(title, description, dueDate);
        }

        /// <summary>
        /// adding a new column to the board by its name and position!
        /// </summary>
        /// <param name="columnOrdinal"> the new column position to be </param>
        /// <param name="name"> the new column name to be </param>
        public void AddColumn(string columnOrdinal, string name)
        {
            int num = Int32.Parse(columnOrdinal);
            ColumnModel added = controller.AddColumn(UserEmail, num, name);
            if (added != null)
            {
                Columns.Insert(num, added);
                for (int i = num; i < Columns.Count; i++)
                {
                    Columns[i].ColumnOrdinal = i;
                    foreach (TaskModel t in Columns[i].Tasks)
                    {
                        t.ColumnOrdinal = i + 1;
                    }
                }
            }
        }

        /// <summary>
        /// removig the given column from the board!
        /// </summary>
        /// <param name="columnModel"></param>
        public void removeColumn(ColumnModel columnModel)
        {
            controller.RemoveColumn(UserEmail, columnModel.ColumnOrdinal);
            if (columnModel.ColumnOrdinal == 0)
            {
                foreach (TaskModel t in columnModel.Tasks)
                {
                    Columns[columnModel.ColumnOrdinal + 1].Tasks.Add(t);
                    t.ColumnOrdinal = columnModel.ColumnOrdinal + 1;
                }
            }
            else
            {
                foreach (TaskModel t in columnModel.Tasks)
                {
                    Columns[columnModel.ColumnOrdinal - 1].Tasks.Add(t);
                    t.ColumnOrdinal = columnModel.ColumnOrdinal - 1;
                }
            }
            Columns.Remove(columnModel);
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].ColumnOrdinal = i;
            }
        }

        /// <summary>
        /// moving the given column to the left index
        /// </summary>
        /// <param name="columnModel"></param>
        public void MoveColumnRight(ColumnModel columnModel)
        {
            ColumnModel selected = controller.MoveColumnRight(UserEmail, columnModel.ColumnOrdinal);
            if (selected != null)
            {
                Columns.Move(selected.ColumnOrdinal, selected.ColumnOrdinal + 1);
                int temp = Columns[selected.ColumnOrdinal].ColumnOrdinal;
                Columns[selected.ColumnOrdinal].ColumnOrdinal = selected.ColumnOrdinal;
                Columns[selected.ColumnOrdinal + 1].ColumnOrdinal = temp;
                foreach (TaskModel t in columnModel.Tasks)
                {
                    t.ColumnOrdinal = columnModel.ColumnOrdinal;
                }
            }
        }

        /// <summary>
        /// moving the given column to the next index in the board!
        /// </summary>
        /// <param name="columnModel"></param>
        public void MoveColumnLeft(ColumnModel columnModel)
        {
            ColumnModel selected = controller.MoveColumnLeft(UserEmail, columnModel.ColumnOrdinal);
            if (selected != null)
            {
                Columns.Move(selected.ColumnOrdinal, selected.ColumnOrdinal - 1);
                int temp = Columns[selected.ColumnOrdinal].ColumnOrdinal;
                Columns[selected.ColumnOrdinal].ColumnOrdinal = selected.ColumnOrdinal;
                Columns[selected.ColumnOrdinal - 1].ColumnOrdinal = temp;
                foreach (TaskModel t in columnModel.Tasks)
                {
                    t.ColumnOrdinal = columnModel.ColumnOrdinal;
                }
            }
        }
    }
}
