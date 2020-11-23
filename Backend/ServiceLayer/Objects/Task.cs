using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Task
    {
        public readonly int Id;
        public readonly DateTime CreationTime;
        public readonly DateTime DueDate;
        public readonly string Title;
        public readonly string Description;
        public readonly string emailAssignee;
        internal Task(int id, DateTime creationTime, DateTime DueDate, string title, string description, string emailAssignee)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.DueDate = DueDate;
            this.Title = title;
            this.Description = description;
            this.emailAssignee = emailAssignee;
        }
        // You can add code here
        internal Task(Business_Layer.BoardPackage.Task t)
        {
            this.CreationTime = t.getCreationDate();
            this.Title = t.getTitle();
            this.Description = t.getDescription();
            this.Id = t.getId();
            this.DueDate = t.getDueDate();
            this.emailAssignee = t.GetEmail();
        }
    }
}