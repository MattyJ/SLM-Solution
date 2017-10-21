using System.Collections.Generic;

namespace Fujitsu.SLM.Web.Models
{
    public class ViewProcessDotMatrixViewModel : LevelViewModel
    {
        public ViewProcessDotMatrixViewModel()
        {
            Columns = new List<DynamicGridColumn>();
        }

        public List<DynamicGridColumn> Columns { get; private set; }
    }
}