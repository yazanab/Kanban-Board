using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccess_Layer.DTOs
{
    class ColumnDTO : DTO
    {
        public const string myEmail = "Email";
        public const string columnName = "Name";
        public const string myLimit = "lmt";
        public const string columnId = "columnID";

        private string _email;
        private string _name;

        /// <summary>
        /// Down bellow we update all the fields using Geters/Seters!
        /// </summary>
        private int _col_id;
        public string Name { get => _name; set { _name = value; _controller.GetColumnDalController().Update(_name, _email, columnName, value); } }

        public string Email { get => _email; set { _email = value; _controller.GetColumnDalController().Update(_name, _email, myEmail, value); } }


        private int _limit;
        public int Limit { get => _limit; set { _limit = value; ; _controller.GetColumnDalController().Update(_name, _email, myLimit, value.ToString()); } }


        public int ColID { get => _col_id; set { _col_id = value; _controller.GetColumnDalController().Update(_name, _email, columnId, value.ToString()); } }

        private string updated_name;
        public string UpdatedName { set { updated_name = value; _controller.GetColumnDalController().Update(ColID, _email, columnName, value); } }

        /// <summary>
        /// Creating a new DAL-Column so we save it to dataBase
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="limit"></param>
        /// <param name="col_ID"></param>
        public ColumnDTO(string Email, string name, int limit, int col_ID) : base(new ColumnDalController())
        {
            _email = Email;
            _name = name;
            _limit = limit;
            _col_id = col_ID;
        }

        /// <summary>
        /// Saving our column in the dataBase
        /// </summary>
        public void Save()
        {
            Name = _name;
            Limit = _limit;
            ColID = _col_id;
        }



    }
}
