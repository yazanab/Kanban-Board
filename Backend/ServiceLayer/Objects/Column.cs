using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Column
    {
        public readonly IReadOnlyCollection<Task> Tasks;
        public readonly string Name;
        public readonly int Limit;
        internal Column(IReadOnlyCollection<Task> tasks, string name, int limit)
        {
            this.Tasks = tasks;
            this.Name = name;
            this.Limit = limit;
        }

        internal Column(Business_Layer.BoardPackage.Column c)
        {
            Name = c.getName();
            Limit = c.getLimit();
            List<Task> list = new List<Task>();
            for (int i = 0; i < c.getNumberOfTasks(); i++)
            {
                list.Add(new Task(c.getTasks()[i]));
            }
            Tasks = list;
        }
        // You can add code here
    }
}
