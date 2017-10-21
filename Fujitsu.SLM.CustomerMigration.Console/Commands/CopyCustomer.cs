using Fujitsu.SLM.CustomerMigration.Console.Extensions;
using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.CustomerMigration.Console.Processors;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Data.Repository;
using Fujitsu.SLM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Commands
{
    public class CopyCustomer : ICommand
    {
        public void Execute()
        {
            // HMRC
            // =====
            // 45 = HMRC (Live Services) ( * - Copied )
            // 69 = HMRC ( * - Copied )
            // 84 = HMRC Service Management ( * - Copied )
            // 118 = HMRC FAST-DA ( * - Copied )
            // 180 = HMRC FAST-DA_COPY ( * - Copied )


            // MATT TEST
            // ==========
            // 184 = Matt Test

            // VANDA
            // ======
            // 185 = TestingMagic

            var customer = new Customer().GetSourceCustomer(84);

            if (customer != null)
            {
                using (var targetDbContext = new SLMDataContext("Name=SLMTargetDataContext"))
                {
                    var unitOfWork = new UnitOfWork(targetDbContext);
                    IRepositoryTransaction repositoryTransaction = null;

                    using (var dbConnection = unitOfWork.CreateConnection())
                    {
                        try
                        {
                            // Open the connection and begin a transaction
                            dbConnection.Open();
                            repositoryTransaction = unitOfWork.BeginTransaction();

                            if (!targetDbContext.Customers.Any(x => x.CustomerName == customer.CustomerName))
                            {
                                var targetCustomer = new Customer
                                {
                                    Active = customer.Active,
                                    CustomerName = customer.CustomerName,
                                    CustomerNotes = customer.CustomerNotes,
                                    AssignedArchitect = customer.AssignedArchitect,
                                    Contributors = new List<Contributor>(),
                                    ServiceDesks = new List<ServiceDesk>(),
                                    InsertedBy = customer.InsertedBy,
                                    InsertedDate = customer.InsertedDate,
                                    UpdatedBy = customer.UpdatedBy,
                                    UpdatedDate = customer.UpdatedDate
                                };

                                targetDbContext.Customers.Add(targetCustomer);
                                unitOfWork.Save();

                                if (customer.Contributors.Any())
                                {
                                    var contributorProcessor = new ContributorProcessor(targetCustomer, customer.Contributors.ToList(), unitOfWork);
                                    contributorProcessor.Execute();
                                }

                                if (customer.ServiceDesks.Any())
                                {
                                    foreach (var serviceDesk in customer.ServiceDesks.ToList())
                                    {
                                        var serviceDeskProcessor = new ServiceDeskProcessor(targetCustomer, serviceDesk, unitOfWork);
                                        serviceDeskProcessor.Execute();

                                        var targetServiceDesk = targetDbContext.ServiceDesks.Single(x => x.DeskName == serviceDesk.DeskName && x.CustomerId == targetCustomer.Id);
                                        var deskInputTypeProcessor = new DeskInputTypeProcessor(targetServiceDesk, serviceDesk.DeskInputTypes.ToList(), targetDbContext, unitOfWork);
                                        deskInputTypeProcessor.Execute();

                                        var serviceDeskResolverProcessor = new ServiceDeskResolverProcessor(targetServiceDesk, serviceDesk.Resolvers, targetDbContext, unitOfWork);
                                        serviceDeskResolverProcessor.Execute();

                                        var serviceDomainProcessor = new ServiceDomainProcessor(targetServiceDesk, serviceDesk.ServiceDomains.ToList(), targetDbContext, unitOfWork);
                                        serviceDomainProcessor.Execute();
                                    }
                                }
                                repositoryTransaction?.Save();
                                System.Console.WriteLine($@"Successfully Migrated Customer > {customer.CustomerName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // If we have a transaction then roll it back
                            repositoryTransaction?.Rollback();

                            System.Console.WriteLine($@"Exception => {ex.Message}");
                        }

                    }
                }
            }
        }
    }
}