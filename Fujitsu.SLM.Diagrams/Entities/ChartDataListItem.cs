using System.Collections.Generic;

namespace Fujitsu.SLM.Diagrams.Entities
{
    public class ChartDataListItem
    {
        public int Id { get; set; }
        public string CenteredTitle { get; set; }
        public string Title { get; set; }
        public string TitleTwo { get; set; }
        public string TitleThree { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public List<ChartDataListItem> Inputs { get; set; }
        public List<ChartDataListItem> Units { get; set; }

        public ChartDataListItem()
        {
            Inputs = new List<ChartDataListItem>();
            Units = new List<ChartDataListItem>();
        }
    }
}
