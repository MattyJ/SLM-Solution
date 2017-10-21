using Fujitsu.SLM.CustomerMigration.Console.Extensions;
using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ContributorProcessor : IProcessor
    {
        private readonly Customer _customer;
        private readonly IEnumerable<Contributor> _customerContributors;
        private readonly IUnitOfWork _unitOfWork;

        public ContributorProcessor(Customer customer,
            IEnumerable<Contributor> customerContributors,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customerContributors = customerContributors;
            _customer = customer;
        }

        public void Execute()
        {
            var users = new Dictionary<string, string>().GetUsersDictionary();
            foreach (var contributor in _customerContributors)
            {
                var newContributor = new Contributor
                {
                    UserId = users.ContainsKey(contributor.EmailAddress) ? users[contributor.EmailAddress] : null,
                    EmailAddress = contributor.EmailAddress,
                    CustomerId = _customer.Id,
                    Customer = _customer,
                    InsertedBy = contributor.InsertedBy,
                    InsertedDate = contributor.InsertedDate,
                    UpdatedBy = contributor.UpdatedBy,
                    UpdatedDate = contributor.UpdatedDate
                };

                _customer.Contributors.Add(newContributor);
            }

            _unitOfWork.Save();
        }
    }
}
