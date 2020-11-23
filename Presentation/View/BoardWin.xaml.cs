using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Presentation.Model;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Presentation.View
{
    /// <summary>
    /// Interaction logic for BoardWin.xaml
    /// </summary>
    public partial class BoardWin : Window
    {
        BoardWinViewModel vm;
        public BoardWin(UserModel user)
        {
            InitializeComponent();
            vm = new BoardWinViewModel(user);
            this.DataContext = vm;
        }

        /// <summary>
        /// Logout from the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            vm.Logout();
            LoginWin View = new LoginWin();
            View.Show();
            this.Close();
        }

        /// <summary>
        /// A button for removing a column from the board!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            SetSender_Columns(sender);
            vm.removeColumn();
        }

        /// <summary>
        /// A button for moving the wanted column to the previous index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveColumnLeft_Click(object sender, RoutedEventArgs e)
        {
            SetSender_Columns(sender);
            vm.MoveColumnLeft();
        }

        /// <summary>
        /// A button for moving the wanted column to the next index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveColumnRight_Click(object sender, RoutedEventArgs e)
        {
            SetSender_Columns(sender);
            vm.MoveColumnRight();
        }

        /// <summary>
        /// A button for adding a new column to the board!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            vm.AddColumn();
            vm.colName = "";
            vm.colID = "";
        }

        /// <summary>
        /// A button for adding a new task to the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTask_Click(object sender, RoutedEventArgs e)
        {
            vm.addTask();
        }

        /// <summary>
        /// A button for removing task from specific column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            SetSender_Tasks(sender);
            vm.RemoveTask();
        }

        /// <summary>
        /// A button for advancing the task to the next column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advanceTask_Click(object sender, RoutedEventArgs e)
        {
            SetSender_Tasks(sender);
            vm.AdvanceTask();
        }

        /// <summary>
        /// Sorting tasks by their duedate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortTasks_Click(object sender, RoutedEventArgs e)
        {
            vm.SortTasks();
        }

        /// <summary>
        /// Filtering the board for tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTasks_Click(object sender, RoutedEventArgs e)
        {
            if (vm.filterAllTasks())
            {
                filterbtn.IsEnabled = false;
                resetbtn.IsEnabled = true;
                search.IsReadOnly = true;
            }
        }

        /// <summary>
        /// When the user filter the board for the tasks, he can reset to get all the tasks again in the board!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetFilterTasks_Click(object sender, RoutedEventArgs e)
        {
            filterbtn.IsEnabled = true;
            search.IsReadOnly = false;
            resetbtn.IsEnabled = false;
            vm.ResetFilter();
        }

        /// <summary>
        /// Used to arrange the click on the task, so the user doesnt need to click on the task in order to do some job on the task!
        /// </summary>
        /// <param name="sender"></param>
        private void SetSender_Tasks(object sender)
        {
            TaskModel selectedTask = (TaskModel)((System.Windows.Controls.Image)sender).DataContext;
            foreach (ColumnModel c in vm.board.Columns)
            {
                if (c.Tasks.Contains(selectedTask))
                {
                    vm.SelectedColumn = c;
                    vm.SelectedColumn.SelectedTask = selectedTask;
                    break;
                }
            }
        }

        /// <summary>
        /// Used to arrange the click on the column, so the user doesnt need to click on the column to do some job.
        /// </summary>
        /// <param name="sender"></param>
        private void SetSender_Columns(object sender)
        {
            ColumnModel selectedColumn = (ColumnModel)((Image)sender).DataContext;
            vm.SelectedColumn = selectedColumn;
        }

        /// <summary>
        /// A click that shows the full description to the user!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SowDescBtn(object sender, RoutedEventArgs e)
        {
            TaskModel selectedTask = (TaskModel)((System.Windows.Controls.Image)sender).DataContext;
            foreach (ColumnModel c in vm.board.Columns)
            {
                if (c.Tasks.Contains(selectedTask))
                {
                    vm.SelectedColumn = c;
                    vm.SelectedColumn.SelectedTask = selectedTask;
                    break;
                }
            }
            MessageBox.Show(selectedTask.Description, "Full Description of Task [" + selectedTask.Id + "]  ~  " + selectedTask.Description.Length + "/300");
        }
    }
}
