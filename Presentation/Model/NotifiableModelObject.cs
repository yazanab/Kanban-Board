using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presentation.Model
{
    public class NotifiableModelObject : NotifiableObject
    {
        public BackendController controller { get; private set; }
        protected NotifiableModelObject(BackendController controller)
        {
            this.controller = controller;
        }
    }
}
