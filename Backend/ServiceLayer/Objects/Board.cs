using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Board
    {
        public readonly IReadOnlyCollection<string> ColumnsNames;
        public readonly string emailCreator;
        internal Board(IReadOnlyCollection<string> columnsNames, string emailCreator)
        {
            this.ColumnsNames = columnsNames;
            this.emailCreator = emailCreator;
        }

        internal Board(Business_Layer.BoardPackage.Board tempBoard)
        {
            List<string> t = new List<string>();
            foreach (string name in tempBoard.getBoardColumnsNames())
            {
                t.Add(name);
            }
            ColumnsNames = t;
            this.emailCreator = tempBoard.getEmail();
        }
        // You can add code here
    }
}