using System;

namespace Fujitsu.Aspose.Spreadsheets.Types.Definition
{
    /// <summary>
    /// Used to hold the dynamically generated definition of a report column.
    /// </summary>
    internal class ColumnDefinition<T>
    {
        internal int ColumnIndex { get; set; }
        internal int Ordinal { get; set; }
        internal string Name { get; set; }
        internal double? Width { get; set; }
        internal Func<T, object> Value { get; set; }
        internal StyleDefinition Style { get; set; }
        internal StyleDefinition HeaderStyle { get; set; }
    }
}