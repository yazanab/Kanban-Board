using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Presentation.Model
{
    public class ColumnModel : NotifiableModelObject
    {
        private string UserEmail { get; set; }


        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get
            {
                return _selectedTask;
            }
            set
            {
                _selectedTask = value;
                RaisePropertyChanged("SelectedTask");
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                try
                {
                    controller.ChangeName(UserEmail, ColumnOrdinal, value);
                    name = value;
                    RaisePropertyChanged("Name");
                    System.Windows.Forms.MessageBox.Show("Column Name Changed Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Column Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int limit;
        public int Limit
        {
            get { return limit; }
            set
            {
                try
                {
                    controller.LimitColumnTasks(UserEmail, ColumnOrdinal, value);
                    limit = value;
                    RaisePropertyChanged("Limit");
                    System.Windows.Forms.MessageBox.Show("Column Limit Changed Successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Updating Column", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int colOrdinal;
        public int ColumnOrdinal
        {
            get { return colOrdinal; }
            set
            {
                colOrdinal = value;
                RaisePropertyChanged("ColumnOrdinal");
            }
        }

        public ObservableCollection<TaskModel> save_original;

        private ObservableCollection<TaskModel> tasks;
        public ObservableCollection<TaskModel> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                RaisePropertyChanged("Tasks");
            }
        }

        /// <summary>
        /// simple constructor 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="controller"></param>
        /// <param name="UserEmail"></param>
        /// <param name="columnOrdinal"></param>
        internal ColumnModel(Column c, BackendController controller, string UserEmail, int columnOrdinal) : base(controller)
        {
            this.name = c.Name;
            this.limit = c.Limit;
            this.UserEmail = UserEmail;
            this.ColumnOrdinal = columnOrdinal;
            tasks = new ObservableCollection<TaskModel>(c.Tasks.Select(t => new TaskModel(t.Id, t.CreationTime, t.Title, t.Description, t.DueDate, t.emailAssignee, controller, UserEmail, ColumnOrdinal)).ToList());
            OverDueDate();
            arrangeAssigneeTasks();
        }

        /// <summary>
        /// supporting add a new task to the borad
        /// </summary>
        /// <param name="title"> the new task title</param>
        /// <param name="desc"> the new task description </param>
        /// <param name="duedate"> the new task dueDate </param>
        public void addTask(string title, string desc, DateTime duedate)
        {
            TaskModel toAdd = controller.AddTask(UserEmail, title, desc, duedate);
            Tasks.Add(toAdd);
        }

        /// <summary>
        /// removing the given task from the borad!
        /// </summary>
        /// <param name="t"></param>
        public void RemoveTask(TaskModel t)
        {
            controller.DeleteTask(UserEmail, ColumnOrdinal, t.Id);
            Tasks.Remove(t);
        }

        /// <summary>
        /// advancing the given task to the next column
        /// </summary>
        /// <param name="t"></param>
        public void AdvanceTask(TaskModel t)
        {
            controller.AdvanceTask(UserEmail, ColumnOrdinal, t.Id);
        }

        /// <summary>
        /// sorting task by their dueDate
        /// </summary>
        public void SortTasks()
        {
            List<TaskModel> toSort = Tasks.OrderBy(x => x.DueDate).ToList();
            Tasks.Clear();
            foreach (TaskModel t in toSort)
            {
                Tasks.Add(t);
            }
        }

        /// <summary>
        /// colors the task according to its dueDate, if it passed 70% of the time then color is orange! 
        /// if 100% passed the color is red!
        /// the default color of the task it lightGreen
        /// </summary>
        internal void OverDueDate()
        {
            foreach (TaskModel t in this.Tasks)
            {
                if (t.Persentage() >= 100)
                {
                    t.TaskBackground = new SolidColorBrush(Colors.Red);
                }
                else if (t.Persentage() >= 75)
                {
                    t.TaskBackground = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    t.TaskBackground = new SolidColorBrush(Colors.LightGreen);
                }
            }
        }

        /// <summary>
        /// tells the user if the the task belongs to him or not! if yes, then change the border to blue
        /// </summary>
        internal void arrangeAssigneeTasks()
        {
            foreach (TaskModel t in this.Tasks)
            {
                if (UserEmail == t.Assignee)
                    t.TaskBorderColor = new SolidColorBrush(Colors.Blue);
            }
        }
    }
}
