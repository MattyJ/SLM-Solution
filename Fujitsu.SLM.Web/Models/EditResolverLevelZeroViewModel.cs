using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujitsu.SLM.Web.Models
{
    public class EditResolverLevelZeroViewModel : LevelViewModel
    {
        public int Id { get; set; }

        public string ServiceDeskName { get; set; }

        [UIHint("ServiceDeliveryOrganisationViewModel")]
        public EditResolverServiceDeliveryOrganisationViewModel ResolverServiceDeliveryOrganisation { get; set; }

        [UIHint("ServiceDeliveryUnitLevelZeroViewModel")]
        public EditResolverServiceDeliveryUnitLevelZeroViewModel ResolverServiceDeliveryUnit { get; set; }

        [UIHint("ResolverGroupViewModel")]
        public EditResolverResolverGroupViewModel ResolverGroup { get; set; }
    }
}