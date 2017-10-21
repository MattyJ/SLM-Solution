using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class ServiceDeliveryOrganisationTypeRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string ServiceDeliveryOrganisationTypeName { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}