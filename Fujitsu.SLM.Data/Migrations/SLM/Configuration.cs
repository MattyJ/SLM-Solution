using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Fujitsu.SLM.Data.SLMDataContext>
    {
        private const bool CreateCustomer = false;


        #region Customer Sizing

        // Small

        private const int InputsPerDesk = 10;
        private const int ServiceDomainsPerDesk = 3;
        private const int ServiceFunctionsPerDomain = 3;
        private const int LevelOneComponentsPerFunction = 3;
        private const int LevelTwoComponentsPerLevelOne = 1;
        private const int OperationalProcessesPerResolver = 6;

        // Medium

        //private const int InputsPerDesk = 13;
        //private const int ServiceDomainsPerDesk = 7;
        //private const int ServiceFunctionsPerDomain = 3;
        //private const int LevelOneComponentsPerFunction = 3;
        //private const int LevelTwoComponentsPerLevelOne = 3;
        //private const int OperationalProcessesPerResolver = 6;

        // Large

        //private const int InputsPerDesk = 15;
        //private const int ServiceDomainsPerDesk = 15;
        //private const int ServiceFunctionsPerDomain = 3;
        //private const int LevelOneComponentsPerFunction = 3;
        //private const int LevelTwoComponentsPerLevelOne = 4;
        //private const int OperationalProcessesPerResolver = 8;

        #endregion

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\SLM";
        }

        #region Test Data

        // Component Names are purely for test data

        static readonly List<string> ComponentLevel1Names = new List<string>
            {
                "Capacity Management",
                "Monitoring",
                "WAN",
                "2nd & 3rd Line Break - Fix",
                "Core distribution layer",
                "Core Network Services (DHCP, NTP, DNS and AAA)",
                "Firewall Management",
                "Internet Edge",
                "LAN Management minor sites",
                "Network Security",
                "VPN/RAS",
                "2nd & 3rd Line Break - Fix",
                "Centralised Wi-Fi Management",
                "Inventory Services",
                "LAN access layer",
                "Unified Communications and telephony",
            };

        static readonly List<string> ComponentLevel2Names = new List<string>
            {
                "Remote Support",
                "Campus networks core and distribution layer",
                "Management and support (including server infrastructure)",
                "Management and support of firewall system platform/s",
                "Provisioning of Firewall Services",
                "Provisioning of Firewall Services - secure application access",
                "Provisioning of Firewall Services - secure web browsing",
                "Manage internet connectivity to local internet providers",
                "Management and monitoring of the internet edge environment",
                "Managed LAN service",
                "Manage secure network services into all service elements",
                "Managed Environment",
                "Manage the VPN/RAS solution",
                "On-site Support",
                "Remote Support",
                "Management of the central Wi-Fi infrastructure (Cisco prime and Cisco WLC)",
                "Stock Management of network devices, spares and peripherals",
                "Configuration of LAN and Wi-Fi devices",
                "End to end management and support of the LAN Access layer",
                "Management and support of the LAN infrastructure",
                "Support of access switches",
                "IP enabled legacy PBX systems",
                "Legacy PBX systems",
                "Local Cisco IPT solutions",
                "Local non Cisco IPT solutions",
                "Standardised and centrally managed Cisco / Tandberg video conferencing solution",

            };
        #endregion

        protected override void Seed(Fujitsu.SLM.Data.SLMDataContext context)
        {

            //  This method will be called after migrating to the latest version.
            var customerNames = new List<string> { "ING", "3663", "FCA", "M&S" };
            const string insertUserName = "matthew.jordan@uk.fujitsu.com";
            var userNames = new List<string> { insertUserName };
            //const string userNameTwo = "patrick.williams@uk.fujitsu.com";
            var dateTime = DateTime.Now;
            var random = new Random();

            #region Parameters

            var existingParameters = context.Parameters
                .AsQueryable()
                .Select(s => s.ParameterName)
                .ToList();
            var newParameters = new List<Parameter>();

            if (!existingParameters.Contains(ParameterNames.UserInactivityLockoutDays))
            {
                newParameters.Add(
                new Parameter
                {
                    Id = 1,
                    ParameterName = ParameterNames.UserInactivityLockoutDays,
                    ParameterValue = "30",
                    Type = ParameterType.Admin,
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime,
                });
            }

            if (!existingParameters.Contains(ParameterNames.UserResetPassword))
            {
                newParameters.Add(
                    new Parameter
                    {
                        Id = 1,
                        ParameterName = ParameterNames.UserResetPassword,
                        ParameterValue = "Pa$$w0rd",
                        Type = ParameterType.Admin,
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingParameters.Contains(ParameterNames.ContactEmailAddress))
            {
                newParameters.Add(
                new Parameter
                {
                    Id = 1,
                    ParameterName = ParameterNames.ContactEmailAddress,
                    ParameterValue = "GlobalServiceArchitecture@uk.fujitsu.com",
                    Type = ParameterType.Admin,
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime,
                });
            }

            newParameters.ForEach(p => context.Parameters.AddOrUpdate(p));
            context.SaveChanges();

            #endregion

            var existingCustomersCount = context.Customers.Count();
            if (existingCustomersCount > 0)
            {
                return;
            }

            #region Reference Data

            // Reference Data
            var inputTypeRefData = new List<InputTypeRefData>
            {
                new InputTypeRefData{Id = 1,InputTypeNumber = 1, InputTypeName= "Authorized Request", SortOrder = 5},
                new InputTypeRefData{Id = 2,InputTypeNumber = 2, InputTypeName= "How do I.. Questions", SortOrder = 5},
                new InputTypeRefData{Id = 3,InputTypeNumber = 3, InputTypeName= "Authorized Standard Change", SortOrder = 5},
                new InputTypeRefData{Id = 4,InputTypeNumber = 4, InputTypeName= "Authorized Non-Standard Change", SortOrder = 5},
                new InputTypeRefData{Id = 5,InputTypeNumber = 5, InputTypeName= "Emergency Change", SortOrder = 5},
                new InputTypeRefData{Id = 6,InputTypeNumber = 6, InputTypeName= "Asset Update", SortOrder = 5},
                new InputTypeRefData{Id = 7,InputTypeNumber = 7, InputTypeName= "Config New Update", SortOrder = 5},
                new InputTypeRefData{Id = 8,InputTypeNumber = 8, InputTypeName= "Problem Information", SortOrder = 5},
                new InputTypeRefData{Id = 9,InputTypeNumber = 9, InputTypeName= "Reporting Request", SortOrder = 5},
                new InputTypeRefData{Id = 10,InputTypeNumber = 10, InputTypeName= "Status Request", SortOrder = 5},
                new InputTypeRefData{Id = 11,InputTypeNumber = 11, InputTypeName= "Additional Report", SortOrder = 5},
                new InputTypeRefData{Id = 12,InputTypeNumber = 12, InputTypeName= "Change Update", SortOrder = 5},
                new InputTypeRefData{Id = 13,InputTypeNumber = 13, InputTypeName= "Complaint", SortOrder = 5},
                new InputTypeRefData{Id = 14,InputTypeNumber = 14, InputTypeName= "Incident Management", SortOrder = 5},
                new InputTypeRefData{Id = 15,InputTypeNumber = 15, InputTypeName= "Self Service Portal", SortOrder = 5},
                new InputTypeRefData{Id = 16,InputTypeNumber = 16, InputTypeName= "Knowledge Article", SortOrder = 5},
                new InputTypeRefData{Id = 17,InputTypeNumber = 17, InputTypeName= "Email", SortOrder = 5},
                new InputTypeRefData{Id = 18,InputTypeNumber = 18, InputTypeName= "Telephone Call", SortOrder = 5},
            };

            inputTypeRefData.ForEach(f => context.InputTypeRefData.AddOrUpdate(f));
            context.SaveChanges();

            var operationalProcessTypeRefData = new List<OperationalProcessTypeRefData>
            {
                new OperationalProcessTypeRefData{Id=1,OperationalProcessTypeName= "Service Strategy", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=2,OperationalProcessTypeName= "Service Transition", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=3,OperationalProcessTypeName= "Service Operation", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=4,OperationalProcessTypeName= "Change Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=5,OperationalProcessTypeName= "Event Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=6,OperationalProcessTypeName= "Service Portfolio Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=7,OperationalProcessTypeName= "Change Evaluation", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=8,OperationalProcessTypeName= "Incident Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=9,OperationalProcessTypeName= "Financial Management for IT Services", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=10,OperationalProcessTypeName= "Project Management (Transition Planning and Support)", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=11,OperationalProcessTypeName= "Request Fulfilment", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=12,OperationalProcessTypeName= "Demand Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=13,OperationalProcessTypeName= "Application Development", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=14,OperationalProcessTypeName= "Access Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=15,OperationalProcessTypeName= "Business Relationship Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=16,OperationalProcessTypeName= "Problem Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=17,OperationalProcessTypeName= "Service Design", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=18,OperationalProcessTypeName= "Service Validation and Testing", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=19,OperationalProcessTypeName= "IT Operations Control", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=20,OperationalProcessTypeName= "Design Coordination", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=21,OperationalProcessTypeName= "Service Asset and Configuration Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=22,OperationalProcessTypeName= "Facilities Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=23,OperationalProcessTypeName= "Service Catalogue Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=24,OperationalProcessTypeName= "Knowledge Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=25,OperationalProcessTypeName= "Application Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=26,OperationalProcessTypeName= "Service Level Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=27,OperationalProcessTypeName= "Technical Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=28,OperationalProcessTypeName= "Risk Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=29,OperationalProcessTypeName= "Continual Service Improvement(CSI)", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=30,OperationalProcessTypeName= "Capacity Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=31,OperationalProcessTypeName= "Service Review", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=32,OperationalProcessTypeName= "Availability Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=33,OperationalProcessTypeName= "Process Evaluation", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=34,OperationalProcessTypeName= "IT Service Continuity Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=35,OperationalProcessTypeName= "Definition of CSI Initiatives", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=36,OperationalProcessTypeName= "Information Security Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=37,OperationalProcessTypeName= "Monitoring of CSI Initiatives", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=38,OperationalProcessTypeName= "Architecture Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=39,OperationalProcessTypeName= "Supplier Management", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=40,OperationalProcessTypeName= "Release and Deployment Management", SortOrder = 5, Visible=true}

            };

            operationalProcessTypeRefData.ForEach(o => context.OperationalProcessTypeRefData.AddOrUpdate(o));
            context.SaveChanges();

            var domainTypeRefData = new List<DomainTypeRefData>
            {
                new DomainTypeRefData{Id=1,DomainName= "Service Desk", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=2,DomainName= "Infrastructure as a Service", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=3,DomainName= "Service Delivery Management", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=4,DomainName= "Infrastructure Management", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=5,DomainName= "End User Device Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=6,DomainName= "Engineering Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=7,DomainName= "Programme Management Office", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=8,DomainName= "Consulting Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=9,DomainName= "Bill of material components", SortOrder = 5, Visible = true},
            };

            domainTypeRefData.ForEach(d => context.DomainTypeRefData.AddOrUpdate(d));
            context.SaveChanges();

            var functionTypeRefData = new List<FunctionTypeRefData>
            {
                new FunctionTypeRefData{Id=1,FunctionName= "System Management Infrastructure", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=2,FunctionName= "Desktop Virtualisation", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=3,FunctionName= "Secure Remote Access", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=4,FunctionName= "Computing Management", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=5,FunctionName= "General Services", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=6,FunctionName= "Network Services Tower A", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=7,FunctionName= "Network Services Tower B", SortOrder = 5,Visible = true},
            };

            // Vanda will confirm if the items will need to be changed or added to

            functionTypeRefData.ForEach(f => context.FunctionTypeRefData.AddOrUpdate(f));
            context.SaveChanges();

            var serviceDeliveryOrganisationTypeRefData = new List<ServiceDeliveryOrganisationTypeRefData>
            {
                new ServiceDeliveryOrganisationTypeRefData{Id=1,ServiceDeliveryOrganisationTypeName= ServiceDeliveryOrganisationNames.Fujitsu, SortOrder = 5},
                new ServiceDeliveryOrganisationTypeRefData{Id=2,ServiceDeliveryOrganisationTypeName= ServiceDeliveryOrganisationNames.Customer, SortOrder = 10},
                new ServiceDeliveryOrganisationTypeRefData{Id=3,ServiceDeliveryOrganisationTypeName= ServiceDeliveryOrganisationNames.CustomerThirdParty, SortOrder = 15},
            };

            serviceDeliveryOrganisationTypeRefData.ForEach(r => context.ServiceDeliveryOrganisationTypeRefData.AddOrUpdate(r));
            context.SaveChanges();

            var serviceDeliveryUnitTypeRefData = new List<ServiceDeliveryUnitTypeRefData>
            {
                new ServiceDeliveryUnitTypeRefData{Id=1,ServiceDeliveryUnitTypeName= "Verizon", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=2,ServiceDeliveryUnitTypeName= "HP", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=3,ServiceDeliveryUnitTypeName= "Cisco", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=4,ServiceDeliveryUnitTypeName= "SAP", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=5,ServiceDeliveryUnitTypeName= "Business Applications", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=6,ServiceDeliveryUnitTypeName= "Facilities", SortOrder = 5, Visible = true}
            };

            serviceDeliveryUnitTypeRefData.ForEach(s => context.ServiceDeliveryUnitTypeRefData.AddOrUpdate(s));
            context.SaveChanges();

            var resolverGroupTypeRefData = new List<ResolverGroupTypeRefData>
            {
                new ResolverGroupTypeRefData{Id=1,ResolverGroupTypeName= "Wintel Team", SortOrder = 5, Visible = true},
                new ResolverGroupTypeRefData{Id=2,ResolverGroupTypeName= "Oracle Team", SortOrder = 10, Visible = true},
                new ResolverGroupTypeRefData{Id=3,ResolverGroupTypeName= "On-Site Support Dispatch", SortOrder = 15, Visible = true},
                new ResolverGroupTypeRefData{Id=4,ResolverGroupTypeName= "On-Site Support Permanent", SortOrder = 15, Visible = true},
                new ResolverGroupTypeRefData{Id=5,ResolverGroupTypeName= "Off-Site Support Dispatch", SortOrder = 15, Visible = true},
                new ResolverGroupTypeRefData{Id=6,ResolverGroupTypeName= "Off-Site Support Permanent", SortOrder = 15, Visible = true},
            };

            resolverGroupTypeRefData.ForEach(r => context.ResolverGroupTypeRefData.AddOrUpdate(r));
            context.SaveChanges();

            #endregion


            #region Test Customer

            if (CreateCustomer)
            {
                var customerId = 1;
                var customer = new List<Customer>();

                userNames.ForEach(f =>
                {
                    customer.Add(new Customer
                    {
                        Id = customerId,
                        CustomerName = customerNames[customerId],
                        Active = true,
                        AssignedArchitect = f,
                        InsertedBy = f,
                        InsertedDate = dateTime,
                        UpdatedBy = f,
                        UpdatedDate = dateTime
                    });
                    customerId++;
                });

                customer.ForEach(c => context.Customers.AddOrUpdate(c));
                context.SaveChanges();

                var serviceDesks = new List<ServiceDesk>();
                var serviceDeskId = 1;
                customer.ForEach(f => serviceDesks.Add(new ServiceDesk
                {
                    Id = serviceDeskId++,
                    DeskName = string.Format("Fujitsu {0} Shared Service Desk", f.CustomerName),
                    InsertedBy = f.AssignedArchitect,
                    InsertedDate = dateTime,
                    UpdatedBy = f.AssignedArchitect,
                    UpdatedDate = dateTime,
                    CustomerId = f.Id
                }));
                serviceDesks.ForEach(s => context.ServiceDesks.AddOrUpdate(s));
                context.SaveChanges();


                var deskInputId = 1;
                var serviceFunctionId = 1;
                var serviceComponentId = 1;

                serviceDesks.ForEach(f =>
                {
                    var deskInputTypes = new List<DeskInputType>();
                    for (var i = 0; i < InputsPerDesk; i++)
                    {
                        deskInputTypes.Add(new DeskInputType
                        {
                            Id = deskInputId++,
                            InputTypeRefData = inputTypeRefData.Find(x => x.Id == i + 1),
                            ServiceDeskId = f.Id,
                        });
                    }

                    deskInputTypes.ForEach(d => context.DeskInputTypes.AddOrUpdate(d));

                    var serviceDomains = new List<ServiceDomain>();
                    for (var i = 0; i < ServiceDomainsPerDesk; i++)
                    {
                        var number = random.Next(1, 7);
                        var domain = new ServiceDomain
                        {
                            Id = i + 1,
                            InsertedBy = f.InsertedBy,
                            InsertedDate = dateTime,
                            UpdatedBy = f.InsertedBy,
                            UpdatedDate = dateTime,
                            ServiceDeskId = f.Id,
                            DomainType = domainTypeRefData.Find(x => x.Id == number),
                        };

                        serviceFunctionId = AddServiceFunctionsToDomain(context, serviceFunctionId, functionTypeRefData,
                            f.InsertedBy, dateTime, i + 1, ref serviceComponentId);

                        serviceDomains.Add(domain);
                    }

                    serviceDomains.ForEach(d => context.ServiceDomains.AddOrUpdate(d));

                });

                context.SaveChanges();
            }

            #endregion

        }

        private static int AddServiceFunctionsToDomain(ISLMDataContext context, int serviceFunctionId, List<FunctionTypeRefData> functionTypeRefData,
            string userName, DateTime dateTime, int serviceDomainId, ref int serviceComponentId)
        {
            var serviceFunctions = new List<ServiceFunction>();

            for (var i = 0; i < ServiceFunctionsPerDomain; i++)
            {
                serviceFunctions.Add(new ServiceFunction
                {
                    Id = serviceFunctionId++,
                    FunctionType = functionTypeRefData.Find(x => x.Id == 1),
                    InsertedBy = userName,
                    InsertedDate = dateTime,
                    UpdatedBy = userName,
                    UpdatedDate = dateTime,
                    ServiceDomainId = serviceDomainId,
                });
            }

            foreach (var function in serviceFunctions)
            {
                serviceComponentId = AddServiceComponentsToFunction(context, serviceComponentId, userName, dateTime, function.Id);
            }

            serviceFunctions.ForEach(x => context.ServiceFunctions.AddOrUpdate(x));

            return serviceFunctionId;
        }

        private static int AddServiceComponentsToFunction(ISLMDataContext context, int serviceComponentId, string userName, DateTime dateTime, int serviceFunctionId)
        {
            var r = new Random();

            var serviceComponent = new List<ServiceComponent>();

            for (var i = 0; i < LevelOneComponentsPerFunction; i++)
            {
                var parentServiceComponentId = serviceComponentId++;
                var component = new ServiceComponent
                {
                    Id = parentServiceComponentId,
                    ComponentLevel = 1,
                    ComponentName = ComponentLevel1Names[r.Next(0, ComponentLevel1Names.Count)],
                    InsertedBy = userName,
                    InsertedDate = dateTime,
                    UpdatedBy = userName,
                    UpdatedDate = dateTime,
                    ServiceFunctionId = serviceFunctionId,
                    ChildServiceComponents = new List<ServiceComponent>(),
                };

                for (var j = 0; j < LevelTwoComponentsPerLevelOne; j++)
                {
                    var sdoId = r.Next(1, 4);
                    var sduId = r.Next(1, 4);
                    var rgId = r.Next(1, 7);

                    var operationalProcessTypes = new List<OperationalProcessType>();

                    for (var k = 0; k < OperationalProcessesPerResolver; k++)
                    {
                        operationalProcessTypes.Add(new OperationalProcessType
                        {
                            OperationalProcessTypeRefData = context.OperationalProcessTypeRefData.First(x => x.Id == k + 1)
                        });
                    }

                    var resolver = new Resolver
                    {
                        ServiceDeliveryOrganisationType = context.ServiceDeliveryOrganisationTypeRefData.First(y => y.Id == sdoId),
                        ServiceDeliveryUnitType = context.ServiceDeliveryUnitTypeRefData.First(y => y.Id == sduId),
                        ResolverGroupType = context.ResolverGroupTypeRefData.First(y => y.Id == rgId),
                        ServiceDesk = context.ServiceDesks.First(y => y.Id == 1),
                        InsertedBy = userName,
                        InsertedDate = dateTime,
                        UpdatedBy = userName,
                        UpdatedDate = dateTime,
                        OperationalProcessTypes = operationalProcessTypes
                    };

                    component.ChildServiceComponents.Add(new ServiceComponent
                    {
                        Id = 1000 + parentServiceComponentId,
                        ComponentLevel = 2,
                        ComponentName = ComponentLevel2Names[r.Next(0, ComponentLevel2Names.Count)],
                        InsertedBy = userName,
                        InsertedDate = dateTime,
                        UpdatedBy = userName,
                        UpdatedDate = dateTime,
                        ServiceFunctionId = serviceFunctionId,
                        ParentServiceComponentId = parentServiceComponentId,
                        Resolver = resolver
                    });
                }
                serviceComponent.Add(component);
            }

            serviceComponent.ForEach(x => context.ServiceComponents.AddOrUpdate(x));

            return serviceComponentId;
        }

    }
}
