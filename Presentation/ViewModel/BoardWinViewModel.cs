using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Presentation.Model;

namespace Presentation.ViewModel
{
    class BoardWinViewModel : NotifiableObject
    {
        public BoardModel board { get; private set; }
        public UserModel user { get; private set; }
        private ObservableCollection<ColumnModel> columnList;
        private ColumnModel _selectedColumn;
        private SoundPlayer sound_effect;
        private SoundPlayer sound_effect1;
        private SoundPlayer sound_effect2;
        private SoundPlayer sound_effect3;

        /// <summary>
        /// A board has a column list which is observable collection so it contains all the columns of the board!
        /// </summary>
        public ObservableCollection<ColumnModel> ColumnList
        {
            get => columnList;
            set
            {
                columnList = value;
                RaisePropertyChanged("ColumnList");
            }
        }

        /// <summary>
        /// When a user clicks on a specific column on the board, this field is activated so we know which column he choose!
        /// </summary>
        public ColumnModel SelectedColumn
        {
            get => _selectedColumn;
            set
            {
                _selectedColumn = value;
                RaisePropertyChanged("SelectedColumn");
            }
        }


        public string boardHeader { get; private set; }
        public string greeting { get; private set; }


        /// <summary>
        /// Fields for adding an new Task to the board! * task has new title, new description and a new dueDate *
        /// </summary>
        private string title = "";
        private string Description = "";
        private DateTime dueDate;

        public string taskTitle
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged("taskTitle");
            }
        }

        public string taskDesc
        {
            get => Description;
            set
            {
                Description = value;
                RaisePropertyChanged("taskDesc");
            }
        }

        public DateTime taskDueDate
        {
            get => dueDate;
            set
            {
                dueDate = value;
                RaisePropertyChanged("taskDueDate");
            }
        }


        /// <summary>
        /// Fields for adding a new column to the board! * position and name *
        /// </summary>
        private string colid = "";
        private string colname = "";

        public string colID
        {
            get => colid;
            set
            {
                colid = value;
                RaisePropertyChanged("colID");
            }
        }

        public string colName
        {
            get => colname;
            set
            {
                colname = value;
                RaisePropertyChanged("colName");
            }
        }

        /// <summary>
        /// Supporting filter tasks on the board by a custom search text!
        /// </summary>
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        /// <summary>
        /// Simple constructor which receives a UserModel as parameter and holds the board for him
        /// </summary>
        /// <param name="user"> the currently logged-in user! </param>
        public BoardWinViewModel(UserModel user)
        {
            board = user.GetBoard(user.Email);
            ColumnList = board.Columns;
            this.user = user;
            boardHeader = "Kanban Board for [" + board.emailCreator + "]";
            greeting = "Welcome back " + user.Nickname + " !";
            sound_effect = new SoundPlayer(Properties.Resources.sound);
            sound_effect1 = new SoundPlayer(Properties.Resources.sound1);
            sound_effect2 = new SoundPlayer(Properties.Resources.sound2);
            sound_effect3 = new SoundPlayer(Properties.Resources.sound3);
        }

        /// <summary>
        /// Supporting remove column from the board!
        /// </summary>
        public void removeColumn()
        {
            try
            {
                board.removeColumn(SelectedColumn);
                sound_effect2.Play();
                // System.Windows.Forms.MessageBox.Show("Column Removed Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Supporting move columns to the right in the board!
        /// </summary>
        public void MoveColumnRight()
        {
            try
            {
                board.MoveColumnRight(SelectedColumn);
                //System.Windows.Forms.MessageBox.Show("Column Moved To Right Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sound_effect1.Play();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Supporting move columns to the left in the board!
        /// </summary>
        public void MoveColumnLeft()
        {
            try
            {
                board.MoveColumnLeft(SelectedColumn);
                //System.Windows.Forms.MessageBox.Show("Column Moved To Left Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sound_effect1.Play();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Logging-out of the currently logged-in user!
        /// </summary>
        public void Logout()
        {
            user.Logout(user.Email);
        }

        /// <summary>
        /// Adding a new column to the board by the user!
        /// </summary>
        public void AddColumn()
        {
            try
            {
                board.AddColumn(colID.ToString(), colName);
                //System.Windows.Forms.MessageBox.Show("Column Added Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sound_effect.Play();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Adding a new Task to the board!
        /// </summary>
        public void addTask()
        {
            try
            {
                board.AddTask(taskTitle, taskDesc, taskDueDate);
                sound_effect.Play();
                //System.Windows.Forms.MessageBox.Show("Task has been added successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("AddTask Failed!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Supporting remove an exsisting task from the board!
        /// </summary>
        public void RemoveTask()
        {
            try
            {
                ColumnList[SelectedColumn.ColumnOrdinal].RemoveTask(SelectedColumn.SelectedTask);
                sound_effect2.Play();
                // System.Windows.Forms.MessageBox.Show("Task Removed Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Removing Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Supporting advancing task (or move task to the right) in the board!
        /// </summary>
        public void AdvanceTask()
        {
            try
            {
                SelectedColumn.AdvanceTask(SelectedColumn.SelectedTask);
                TaskModel temp = SelectedColumn.SelectedTask;
                SelectedColumn.Tasks.Remove(SelectedColumn.SelectedTask);
                ColumnList[SelectedColumn.ColumnOrdinal + 1].Tasks.Add(temp);
                temp.ColumnOrdinal = SelectedColumn.ColumnOrdinal + 1;
                sound_effect3.Play();
                //System.Windows.Forms.MessageBox.Show("Task Moved Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Moving Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Sorting all the tasks in the board by their dueDate.
        /// </summary>
        public void SortTasks()
        {
            foreach (ColumnModel c in board.Columns)
            {
                c.SortTasks();
            }
        }

        /// <summary>
        /// Filtering all the tasks in the board by the text written in the _searchText field. 
        /// </summary>
        /// <returns></returns>
        public bool filterAllTasks()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                foreach (ColumnModel column in board.Columns)
                {
                    ObservableCollection<TaskModel> list = new ObservableCollection<TaskModel>();
                    column.save_original = new ObservableCollection<TaskModel>();

                    foreach (TaskModel task in column.Tasks)
                    {
                        column.save_original.Add(task);
                        if (task.Description.Contains(SearchText) || task.Title.Contains(SearchText))
                        {
                            task.IsVisible = true;
                            list.Add(task);
                        }
                        else
                        {
                            task.IsVisible = true;
                        }
                    }
                    column.Tasks.Clear();
                    foreach (TaskModel t in list)
                    {
                        column.Tasks.Add(t);
                    }
                    list.Clear();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// After filtering the tasks in the board, we support reseting the filter so all tasks should be visible again to the user!
        /// </summary>
        public void ResetFilter()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                foreach (ColumnModel column in board.Columns)
                {
                    column.Tasks.Clear();
                    foreach (TaskModel task in column.save_original)
                    {
                        column.Tasks.Add(task);
                    }
                    column.save_original.Clear();
                }
                SearchText = "";

            }
        }

    }
}
