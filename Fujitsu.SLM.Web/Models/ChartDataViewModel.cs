using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Models
{
    public class ChartDataViewModel
    {
        public int Id { get; set; }
        public string CenteredTitle { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string TitleTwo { get; set; } = string.Empty;
        public string TitleThree { get; set; } = string.Empty;
        public string Type { get; set; }
        public int Width { get; set; }
        public List<ChartDataViewModel> Inputs { get; set; }
        public List<ChartDataViewModel> Units { get; set; }
        public int NumberOfInputs => Inputs.Count;
        public int NumberOfUnits => Units.Count;

    }
}
