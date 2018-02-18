using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Filarkiv.Models
{
    public class Filemodel
    {
        [Required(ErrorMessage = "Please select file.")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.doc|.docx|.xls|.xlsx|.pdf|.png|.jpg|.gif|.txt)$", ErrorMessage = "Only files with following extensions allowed: .pdf .doc .docx .txt .xls .xlsx .png .jpg .gif")]
        public HttpPostedFileBase[] PostedFiles { get; set; }

        public string SelectedCategory { get; set; }

        [Display(Name = "Category")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string NewCategory { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}