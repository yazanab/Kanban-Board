using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Presentation.Model
{
    public class TaskModel : NotifiableModelObject
    {

        /// <summary>
        /// A task has the following fields.
        /// </summary>
        private string UserEmail; // the board's creater email
        private int columnOrdinal;
        private int id;
        private string title;
        private string description;
        private DateTime dueDate;
        private DateTime creationTime;
        private string assignee;  // the owner of this task


        public int ColumnOrdinal
        {
            get => columnOrdinal;
            set => columnOrdinal = value;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public DateTime CreationTime
        {
            get => creationTime;
            set
            {
                creationTime = value;
                RaisePropertyChanged("CreationTime");
            }
        }

        /// <summary>
        /// Updating task's title using the controller
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                try
                {
                    controller.UpdateTaskTitle(UserEmail, ColumnOrdinal, Id, value);
                    title = value;
                    RaisePropertyChanged("Title");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Updating Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Updating task's description using the controller
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                try
                {
                    controller.UpdateTaskDescription(UserEmail, ColumnOrdinal, Id, value);
                    description = value;
                    RaisePropertyChanged("Description");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Updating Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Updating task's dueDate using the controller
        /// if the dueDate passes the the real final date we color it as Red
        /// if the dueDate passes 70% of the read final date we color it as Orange
        /// otherwise the default is lightGreen
        /// </summary>
        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                try
                {
                    controller.UpdateTaskDueDate(UserEmail, ColumnOrdinal, Id, value);
                    dueDate = value;
                    if (Persentage() >= 100)
                    {
                        TaskBackground = new SolidColorBrush(Colors.Red);
                    }
                    else if (Persentage() >= 75)
                    {
                        TaskBackground = new SolidColorBrush(Colors.Orange);
                    }
                    else
                    {
                        TaskBackground = new SolidColorBrush(Colors.LightGreen);
                    }
                    RaisePropertyChanged("DueDate");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Updating Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Updating the owner of the task so we support AssignTask to other user!
        /// after updating the email, we must change the task border because the task no longer belongs to the currently logged-in user
        /// </summary>
        public string Assignee
        {
            get => assignee;
            set
            {
                try
                {
                    controller.AssignTask(UserEmail, ColumnOrdinal, Id, value);
                    assignee = value;
                    TaskBorderColor = new SolidColorBrush(Colors.Gray);
                    RaisePropertyChanged("Assignee");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Updating Task", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Updating task background according to the dueDate
        /// </summary>
        private SolidColorBrush taskBackground;
        public SolidColorBrush TaskBackground
        {
            get => taskBackground;
            set
            {
                taskBackground = value; RaisePropertyChanged("TaskBackground");
            }
        }

        /// <summary>
        /// Updating task border according to the assignee
        /// </summary>
        private SolidColorBrush taskBorderColor;
        public SolidColorBrush TaskBorderColor
        {
            get => taskBorderColor;
            set
            {
                taskBorderColor = value; RaisePropertyChanged("TaskBorderColor");
            }
        }

        /// <summary>
        /// This field determine whether the task is being filtered by the textBox or not
        /// </summary>
        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        /// <summary>
        /// Simple constructor
        /// checking if the conditions are true and updating the task color accordingly
        /// </summary>
        /// <param name="id"> the id of the Task </param>
        /// <param name="creationTime"> the task creation date </param>
        /// <param name="title">  task's title </param>
        /// <param name="description"> task's description </param>
        /// <param name="dueDate"></param>
        /// <param name="emailAssignee"></param>
        /// <param name="controller"></param>
        /// <param name="UserEmail"> the host of the board that the task included in </param>
        /// <param name="columnOrdinal"></param>
        internal TaskModel(int id, DateTime creationTime, string title, string description, DateTime dueDate, string emailAssignee, BackendController controller, string UserEmail, int columnOrdinal) : base(controller)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.title = title;
            this.description = description;
            this.dueDate = dueDate;
            this.assignee = emailAssignee;
            this.UserEmail = UserEmail;
            this.ColumnOrdinal = columnOrdinal;
            if (UserEmail == assignee)
                this.TaskBorderColor = new SolidColorBrush(Colors.Blue);
            else
                this.TaskBorderColor = new SolidColorBrush(Colors.Gray);

            if (Persentage() >= 100)
            {
                TaskBackground = new SolidColorBrush(Colors.Red);
            }
            else if (Persentage() >= 75)
            {
                TaskBackground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                TaskBackground = new SolidColorBrush(Colors.LightGreen);
            }
        }

        /// <summary>
        /// calculates the persentage of the dueDate to help us know when we need to color the task
        /// </summary>
        /// <returns> returns the persentage of the dueDate </returns>
        public int Persentage()
        {
            var total = (dueDate - CreationTime).TotalSeconds;
            var persentage = (int)(DateTime.Now - CreationTime).TotalSeconds * 100 / total;
            return (int)persentage;
        }
    }
}
