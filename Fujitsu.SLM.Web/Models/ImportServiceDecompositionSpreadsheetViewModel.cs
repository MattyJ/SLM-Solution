﻿using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ImportServiceDecompositionSpreadsheetViewModelValidator))]
    public class ImportServiceDecompositionSpreadsheetViewModel : LevelViewModel
    {
        public ImportServiceDecompositionSpreadsheetViewModel()
        {
            ServiceDesks = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Display(Name = "Service Desk")]
        public int ServiceDeskId { get; set; }

        public string ServiceDeskName { get; set; }

        public bool HasServiceDeskContext { get; set; }

        [Display(Name = "Spreadsheet File")]
        public HttpPostedFileBase SpreadsheetFile { get; set; }

        public List<SelectListItem> ServiceDesks { get; private set; }
    }
}