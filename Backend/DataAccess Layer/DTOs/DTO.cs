using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccess_Layer.DTOs
{
    internal abstract class DTO
    {
        public const string theID = "ID";

        protected DalController _controller;

        protected DTO(DalController controller)
        {
            _controller = controller;
        }

    }
}
