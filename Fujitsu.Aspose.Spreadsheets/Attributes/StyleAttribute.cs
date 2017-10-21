using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    public abstract class StyleAttribute : Attribute
    {
        public bool IsTextWrapped { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.TextAlignmentTypeValues"/> to populate this field.
        /// </summary>
        public string HorizontalAlign { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.BackgroundTypeValues"/> to populate this field.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The foreground color of the cell. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// The background color of the cell. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Set the custom style of the cell e.g. "£#,##0.00";
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// Are these cells numbers?
        /// </summary>
        public bool NumberFormat { get; set; }

        /// <summary>
        /// Set font bold.
        /// </summary>
        public bool FontBold { get; set; }

        /// <summary>
        /// Set font italic.
        /// </summary>
        public bool FontItalic { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.FontUnderlineTypeValues"/> to populate this field.
        /// </summary>
        public string FontUnderline { get; set; }

        /// <summary>
        /// The color of the font. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.CellBorderTypeValues"/> to populate this field.
        /// </summary>
        public string TopBorderLineStyle { get; set; }

        /// <summary>
        /// The color of the font. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string TopBorderColor { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.CellBorderTypeValues"/> to populate this field.
        /// </summary>
        public string BottomBorderLineStyle { get; set; }

        /// <summary>
        /// The color of the font. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string BottomBorderColor { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.CellBorderTypeValues"/> to populate this field.
        /// </summary>
        public string LeftBorderLineStyle { get; set; }

        /// <summary>
        /// The color of the font. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string LeftBorderColor { get; set; }

        /// <summary>
        /// Use <see cref="Fujitsu.Aspose.Spreadsheets.Constants.CellBorderTypeValues"/> to populate this field.
        /// </summary>
        public string RightBorderLineStyle { get; set; }

        /// <summary>
        /// The color of the font. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string RightBorderColor { get; set; }
    }
}
