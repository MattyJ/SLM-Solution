using System;

namespace Fujitsu.SLM.Web.Models
{
    public class DynamicGridColumn
    {
        public DynamicGridColumn()
        {
            Visible = true;
            Editable = true;
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public Type Type { get; set; }
        public bool Visible { get; set; }
        public bool Editable { get; set; }

        public string TypeFullName => Type.FullName;
    }
}