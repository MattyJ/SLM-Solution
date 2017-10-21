using System.Collections.Generic;

namespace Fujitsu.Aspose.Spreadsheets.Types.Definition
{
    internal class RowDefinition
    {
        public RowDefinition()
        {
            this.RowContents = new List<RowContentDefinition>();
        }

        internal int Row { get; set; }
        internal List<RowContentDefinition> RowContents { get; private set; }
    }
}
