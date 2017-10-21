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
    public class CopyFunction : ICommand
    {
        public void Execute()
        {
            // HMRC
            // =====
            // xxx = HMRC FAST-DA ( * - Copied )

            // Domains
            // ========
            // xxx = Network Services (Source)
            // xxx = Security Services (Target)

            var sourceFunctionId = 978;
            var targetDomainId = 280;

            var serviceFunction = new ServiceFunction().GetSourceFunction(sourceFunctionId);

            if (serviceFunction != null)
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

                            var targetDomain = targetDbContext.ServiceDomains.Single(x => x.Id == targetDomainId);

                            var serviceFunctions = new List<ServiceFunction> { serviceFunction };

                            var serviceFunctionProcessor = new ServiceFunctionProcessor(targetDomain, serviceFunctions, targetDbContext, unitOfWork);
                            serviceFunctionProcessor.Execute();

                            var targetServiceFunction = targetDomain.ServiceFunctions.Single(x => x.FunctionType.FunctionName == serviceFunction.FunctionType.FunctionName);
                            // Add underlying Service Function hierarchy
                            foreach (var serviceComponent in serviceFunction.ServiceComponents.Where(x => x.ComponentLevel == 1))
                            {
                                var serviceComponentProcessor = new ServiceComponentProcessor(targetDomain.ServiceDesk, targetServiceFunction, serviceComponent, targetDbContext, unitOfWork);
                                serviceComponentProcessor.Execute();
                            }

                            repositoryTransaction?.Save();
                            System.Console.WriteLine($@"Successfully Copied Service Function [{serviceFunction.FunctionType.FunctionName}] To Service Domain [{targetDomain.DomainType.DomainName}]");

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
