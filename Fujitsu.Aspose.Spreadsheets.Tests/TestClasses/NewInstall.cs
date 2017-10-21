using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(6, 5, 4)]
    public class NewInstall
    {
        /// <summary>
        /// The Unique Identified stored within the Asset Tag
        /// </summary>
        [WorksheetRowItemValue("Unique ID")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Stored as “Environment”
        /// </summary>
        [WorksheetRowItemValue("Environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Class and Sub-Class will form the new Asset Type (e.g. Class / Sub Class)
        /// </summary>
        [WorksheetRowItemValue("Class")]
        public string Class { get; set; }

        /// <summary>
        /// Class and Sub-Class will form the new Asset Type (e.g. Class / Sub Class)
        /// </summary>
        [WorksheetRowItemValue("Sub-Class")]
        public string SubClass { get; set; }

        /// <summary>
        /// Stored as “Operating System”
        /// </summary>
        [WorksheetRowItemValue("O/S")]
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Stored as "Physical Processes"
        /// </summary>
        [WorksheetRowItemValue("CPU/Cores")]
        public string CpuCores { get; set; }

        /// <summary>
        /// Memory
        /// </summary>
        [WorksheetRowItemValue("Memory (GB)")]
        public string Memory { get; set; }


        /// <summary>
        ///  Stored as "Ready For Use Date"
        /// Input is of form year and quarter and needs mapping to a date dd/mm/yyyy.
        /// E.g. “13Q2” would become date 01/07/2013
        /// </summary>
        [WorksheetRowItemValue("Install Date")]
        public string InstallDate { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        [WorksheetRowItemValue("Location")]
        public string Location { get; set; }


        /// <summary>
        /// Charge Quantity for Component with Column Name
        /// </summary>
        [WorksheetRowMultipleRegExItemValueAttribute("^[A-Z]{4}$")]
        public List<NamedValue<string>> ChargeQuantities { get; set; }
    }
}
