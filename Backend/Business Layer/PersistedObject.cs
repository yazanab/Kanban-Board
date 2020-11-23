using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccess_Layer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    interface PersistedObject<T> where T : DataAccess_Layer.DTOs.DTO
    {
        T ToDalObject();

    }
}
