using System.Web.Mvc;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Web.Models
{
    public class SaveServiceComponent
    {
        public ActionResult Result { get; set; }
        public bool IsValid { get; set; }
    }
}