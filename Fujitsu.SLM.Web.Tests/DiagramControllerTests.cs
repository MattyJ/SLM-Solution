using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Diagrams.Generators;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using KellermanSoftware.CompareNetObjects;
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;
using Diagram = Fujitsu.SLM.Constants.Diagram;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DiagramControllerTests
    {
        private Mock<IServiceDeskService> _mockServiceDeskService;
        private Mock<IDiagramService> _mockDiagramService;
        private Mock<IResolverService> _mockResolverGroupService;

        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<ICacheManager> _mockCacheManager;
        private Mock<IResponseManager> _mockResponseManager;

        private Mock<IObjectBuilder> _mockObjectBuilder;

        private DiagramController _controller;
        private DiagramController _controllerWithMockedServices;
        private AppContext _appContext;

        private Mock<IRepository<SLM.Model.Diagram>> _mockDiagramRepository;
        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<FujitsuDomains> _mockFujitsuDomains;
        private Mock<CustomerServices> _mockCustomerServices;

        private IDiagramService _diagramService;
        private IServiceDeskService _serviceDeskService;
        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _deskInputTypes;

        private List<SLM.Model.Diagram> _diagrams;
        private SLM.Model.Diagram _diagram;
        private SLM.Model.Diagram _diagramUpdated;
        private List<List<DotMatrixListItem>> _dotMatrixListItems;

        private const int CustomerId = 1;
        private const int CustomerIdWithOneServiceDesk = 2;
        private const int ServiceDeskId = 3;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private const string DataUri = "data:application/pdf;base64,JVBERi0xLjQKJcLB2s/OCgoxIDAgb2JqIDw8CiAgL1R5cGUgL0NhdGFsb2cKICAvUGFnZXMgMiAwIFIKPj4gZW5kb2JqCgoyIDAgb2JqIDw8CiAgL1R5cGUgL1BhZ2VzCiAgL0tpZHMgWyA0IDAgUiBdCiAgL0NvdW50IDEKPj4gZW5kb2JqCgozIDAgb2JqIDw8CiAgL0xlbmd0aCA3MDI0Cj4+IHN0cmVhbQoxIDAgMCAtMSAwIDMyNS42OTI5MTM0IGNtCjEgMCAwIDEgMjguMzQ2NDU2NyAyOC4zNDY0NTY3IGNtCjAgMCAyMjAwIDI2OSByZQpXIG4KcQoxIDAgMCAxIC01MCAtNTAgY20KcQpxCnEKMSAwIDAgMSA1MCAyNTIgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjMgdwoxIDEgMSByZwowIDAgMjIwMCA2NyByZQpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTAzMiAyNC41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEZ1aml0c3UgU2VydmljZSBEZXNrKSBUagpFVApRClEKcQoxIDAgMCAxIDE2MyAxMTAgY20KcQpxCjAuMzQ1MDk4IDAuMzkyMTU2OSAwLjQ2NjY2NjcgUkcKMiB3CjEgMSAxIHJnCjI1IDUwIG0KMzguODA3MTE4NyA1MCA1MCAzOC44MDcxMTg3IDUwIDI1IGMKNTAgMTEuMTkyODgxMyAzOC44MDcxMTg3IDAgMjUgMCBjCjExLjE5Mjg4MTMgMCAwIDExLjE5Mjg4MTMgMCAyNSBjCjAgMzguODA3MTE4NyAxMS4xOTI4ODEzIDUwIDI1IDUwIGMKQgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDIxLjUgMTUuNSBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCigxKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKMTg5IDE5NyBtCjE4OSAxNzkuNSBsCjE4OSAxNzkuNSBsCjE4OSAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDE5NCAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDY0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA5Ni41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooSW5jaWRlbnQpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODkgMTEwIG0KMTg5IDkyLjUgbAoxODkgOTIuNSBsCjE4OSA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDQzOCAxMTAgY20KcQpxCjAuMzQ1MDk4IDAuMzkyMTU2OSAwLjQ2NjY2NjcgUkcKMiB3CjEgMSAxIHJnCjI1IDUwIG0KMzguODA3MTE4NyA1MCA1MCAzOC44MDcxMTg3IDUwIDI1IGMKNTAgMTEuMTkyODgxMyAzOC44MDcxMTg3IDAgMjUgMCBjCjExLjE5Mjg4MTMgMCAwIDExLjE5Mjg4MTMgMCAyNSBjCjAgMzguODA3MTE4NyAxMS4xOTI4ODEzIDUwIDI1IDUwIGMKQgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDIxLjUgMTUuNSBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCigyKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKNDY0IDE5NyBtCjQ2NCAxNzkuNSBsCjQ2NCAxNzkuNSBsCjQ2NCAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDQ2OSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDMzOSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTA1LjUgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihFdmVudCkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjQ2NCAxMTAgbQo0NjQgOTIuNSBsCjQ2NCA5Mi41IGwKNDY0IDc1IGwKUwpRClEKUQpxCjEgMCAwIDEgNzEzIDExMCBjbQpxCnEKMC4zNDUwOTggMC4zOTIxNTY5IDAuNDY2NjY2NyBSRwoyIHcKMSAxIDEgcmcKMjUgNTAgbQozOC44MDcxMTg3IDUwIDUwIDM4LjgwNzExODcgNTAgMjUgYwo1MCAxMS4xOTI4ODEzIDM4LjgwNzExODcgMCAyNSAwIGMKMTEuMTkyODgxMyAwIDAgMTEuMTkyODgxMyAwIDI1IGMKMCAzOC44MDcxMTg3IDExLjE5Mjg4MTMgNTAgMjUgNTAgYwpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMjEuNSAxNS41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKDMpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwo3MzkgMTk3IG0KNzM5IDE3OS41IGwKNzM5IDE3OS41IGwKNzM5IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgNzQ0IDE4NyBjbQowIDAgbQoxMCA1IGwKMCAxMCBsCjMgNSBsCmgKZgpRClEKUQpxCjEgMCAwIDEgNjE0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA1NS41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooQXV0aG9yaXplZCBSZXF1ZXN0KSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKNzM5IDExMCBtCjczOSA5Mi41IGwKNzM5IDkyLjUgbAo3MzkgNzUgbApTClEKUQpRCnEKMSAwIDAgMSA5ODggMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooNCkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEwMTQgMTk3IG0KMTAxNCAxNzkuNSBsCjEwMTQgMTc5LjUgbAoxMDE0IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTAxOSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDg4OSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgNTIgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihIb3cgZG8gSS4uIFF1ZXN0aW9ucykgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEwMTQgMTEwIG0KMTAxNCA5Mi41IGwKMTAxNCA5Mi41IGwKMTAxNCA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDEyNjMgMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooNikgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEyODkgMTk3IG0KMTI4OSAxNzkuNSBsCjEyODkgMTc5LjUgbAoxMjg5IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTI5NCAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDExNjQgNTAgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjEgMSAxIHJnCjAgMCAyNTAgMjUgcmUKZgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDQuNSAyIGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEF1dGhvcml6ZWQgTm9uLVN0YW5kYXJkIENoYW5nZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEyODkgMTEwIG0KMTI4OSA5Mi41IGwKMTI4OSA5Mi41IGwKMTI4OSA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDE1MzggMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooOSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjE1NjQgMTk3IG0KMTU2NCAxNzkuNSBsCjE1NjQgMTc5LjUgbAoxNTY0IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTU2OSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDE0MzkgNTAgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjEgMSAxIHJnCjAgMCAyNTAgMjUgcmUKZgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDU1LjUgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihDb25maWcgTmV3IFVwZGF0ZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjE1NjQgMTEwIG0KMTU2NCA5Mi41IGwKMTU2NCA5Mi41IGwKMTU2NCA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDE4MTMgMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAxNy41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooMTIpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODM5IDE5NyBtCjE4MzkgMTc5LjUgbAoxODM5IDE3OS41IGwKMTgzOSAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDE4NDQgMTg3IGNtCjAgMCBtCjEwIDUgbAowIDEwIGwKMyA1IGwKaApmClEKUQpRCnEKMSAwIDAgMSAxNzE0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA3Mi41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooU3RhdHVzIFJlcXVlc3QpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODM5IDExMCBtCjE4MzkgOTIuNSBsCjE4MzkgOTIuNSBsCjE4MzkgNzUgbApTClEKUQpRCnEKMSAwIDAgMSAyMDg4IDExMCBjbQpxCnEKMC4zNDUwOTggMC4zOTIxNTY5IDAuNDY2NjY2NyBSRwoyIHcKMSAxIDEgcmcKMjUgNTAgbQozOC44MDcxMTg3IDUwIDUwIDM4LjgwNzExODcgNTAgMjUgYwo1MCAxMS4xOTI4ODEzIDM4LjgwNzExODcgMCAyNSAwIGMKMTEuMTkyODgxMyAwIDAgMTEuMTkyODgxMyAwIDI1IGMKMCAzOC44MDcxMTg3IDExLjE5Mjg4MTMgNTAgMjUgNTAgYwpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTcuNSAxNS41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKDEzKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKMjExNCAxOTcgbQoyMTE0IDE3OS41IGwKMjExNCAxNzkuNSBsCjIxMTQgMTYyIGwKUwpRCnEKMSAxIDEgUkcKL0dTMyBncwowIDAgMCByZwowIDEgLTEgMCAyMTE5IDE4NyBjbQowIDAgbQoxMCA1IGwKMCAxMCBsCjMgNSBsCmgKZgpRClEKUQpxCjEgMCAwIDEgMTk4OSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgNjguNSAyIGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEluY2lkZW50IFVwZGF0ZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjIxMTQgMTEwIG0KMjExNCA5Mi41IGwKMjExNCA5Mi41IGwKMjExNCA3NSBsClMKUQpRClEKUQpRClEKCmVuZHN0cmVhbSBlbmRvYmoKCjQgMCBvYmogPDwKICAvQ29udGVudHMgMyAwIFIKICAvUGFyZW50IDIgMCBSCiAgL01lZGlhQm94IFsgMCAwIDIyNTYuNjkyOTEzNCAzMjUuNjkyOTEzNCBdCiAgL1R5cGUgL1BhZ2UKICAvUHJvY1NldCBbIC9QREYgL1RleHQgL0ltYWdlQiAvSW1hZ2VDIC9JbWFnZUkgXQogIC9SZXNvdXJjZXMgPDwKICAgIC9Gb250IDw8CiAgICAgIC9GMSA1IDAgUgogICAgPj4KICAgIC9FeHRHU3RhdGUgPDwKICAgICAgL0dTMiA2IDAgUgogICAgICAvR1MzIDcgMCBSCiAgICA+PgogICAgL1hPYmplY3QgPDw+PgogICAgL1BhdHRlcm4gPDw+PgogICAgL1NoYWRpbmcgPDw+PgogID4+Cj4+IGVuZG9iagoKNSAwIG9iaiA8PAogIC9UeXBlIC9Gb250CiAgL1N1YnR5cGUgL1R5cGUxCiAgL0Jhc2VGb250IC9UaW1lcy1Sb21hbgo+PiBlbmRvYmoKCjYgMCBvYmogPDwKICAvVHlwZSAvRXh0R1N0YXRlCiAgL2NhIDAKPj4gZW5kb2JqCgo3IDAgb2JqIDw8CiAgL1R5cGUgL0V4dEdTdGF0ZQogIC9DQSAwCj4+IGVuZG9iagoKeHJlZgowIDgKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDE3IDAwMDAwIG4gCjAwMDAwMDAwNzEgMDAwMDAgbiAKMDAwMDAwMDEzNyAwMDAwMCBuIAowMDAwMDA3MjE2IDAwMDAwIG4gCjAwMDAwMDc1NTEgMDAwMDAgbiAKMDAwMDAwNzYzMCAwMDAwMCBuIAowMDAwMDA3Njc5IDAwMDAwIG4gCgp0cmFpbGVyCjw8CiAgL1NpemUgOAogIC9Sb290IDEgMCBSCiAgL0luZm8gPDwKICAgIC9Qcm9kdWNlciAoS2VuZG8gVUkgUERGIEdlbmVyYXRvcikKICAgIC9UaXRsZSAoTWF0dCBUZXN0IEN1c3RvbWVyIFNlcnZpY2UgRGVzayBXaXRoIElucHV0cykKICAgIC9BdXRob3IgKG1hdHRoZXcuam9yZGFuQHVrLmZ1aml0c3UuY29tKQogICAgL1N1YmplY3QgKFNlcnZpY2UgRGVzayB3aXRoIElucHV0cyBEaWFncmFtKQogICAgL0tleXdvcmRzICgpCiAgICAvQ3JlYXRvciAoU2VydmljZSBEZWNvbXBvc2l0aW9uIFRvb2wgdiAwLjEpCiAgICAvQ3JlYXRpb25EYXRlIChEOjIwMTUwMzA0MTM0MzI0WikKICA+Pgo+PgoKc3RhcnR4cmVmCjc3MjgKJSVFT0YK";
        private const string ResolverGroup1 = "ResolverGroup1";
        private const string ResolverGroup2 = "ResolverGroup2";
        private const string ResolverGroup3 = "ResolverGroup3";

        [TestInitialize]
        public void Initialize()
        {
            #region Unit Test Data

            #region Dot Matrix List Item

            _dotMatrixListItems = new List<List<DotMatrixListItem>>
            {
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup1;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = true;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = true;
                    })
                },
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup2;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = true;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = false;
                    })
                },
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup3;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = false;
                    })
                }
            };

            #endregion

            #region Desk Input Types

            _deskInputTypes = new List<DeskInputType>
            {
                new DeskInputType
                {
                    Id = 1,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 1, InputTypeNumber = 1, InputTypeName = "Incident", SortOrder = 5},
                },
                new DeskInputType
                {
                    Id = 2,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 2, InputTypeNumber = 2, InputTypeName = "Event", SortOrder = 5},
                },
                new DeskInputType
                {
                    Id = 3,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 3,
                            InputTypeNumber = 3,
                            InputTypeName = "Authorized Request",
                            SortOrder = 5
                        },
                },
                new DeskInputType
                {
                    Id = 4,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 4,
                            InputTypeNumber = 4,
                            InputTypeName = "How do I.. Questions",
                            SortOrder = 5
                        },
                },
                new DeskInputType
                {
                    Id = 5,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 5,
                            InputTypeNumber = 5,
                            InputTypeName = "Authorized Standard Change",
                            SortOrder = 5
                        },
                },
                new DeskInputType
                {
                    Id = 6,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 6,
                            InputTypeNumber = 6,
                            InputTypeName = "Authorized Non-Standard Change",
                            SortOrder = 5
                        },
                },
            };

            #endregion

            #region Service Desks

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id = 1,
                    DeskName = "Customer One Service Desk A",
                    CustomerId = CustomerId,
                    DeskInputTypes = new List<DeskInputType>
                    {
                        new DeskInputType
                        {
                            Id =1,
                            InputTypeRefData = new InputTypeRefData{Id = 1,InputTypeNumber = 1, InputTypeName= "Incident", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =2,
                            InputTypeRefData = new InputTypeRefData{Id = 2,InputTypeNumber = 2, InputTypeName= "Event", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =3,
                            InputTypeRefData = new InputTypeRefData{Id = 3,InputTypeNumber = 3, InputTypeName= "Authorized Request", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =4,
                            InputTypeRefData = new InputTypeRefData{Id = 4,InputTypeNumber = 4, InputTypeName= "How do I.. Questions", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =5,
                            InputTypeRefData = new InputTypeRefData{Id = 5,InputTypeNumber = 5, InputTypeName= "Authorized Standard Change", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =6,
                            InputTypeRefData = new InputTypeRefData{Id = 6,InputTypeNumber = 6, InputTypeName= "Authorized Non-Standard Change", SortOrder = 5},
                        },

                    },
                    ServiceDomains = new List<ServiceDomain>(),
                },
                new ServiceDesk
                {
                    Id =2,
                    DeskName = "Customer Two Service Desk B",
                    CustomerId = CustomerIdWithOneServiceDesk,
                    ServiceDomains = new List<ServiceDomain>
                    {
                        new ServiceDomain
                        {
                            Id =1,
                            AlternativeName = "Domain A",
                            DomainType = new DomainTypeRefData
                            {
                                Id =1,
                                DomainName = "A Domain",
                                SortOrder = 5,
                                Visible = true,
                            },
                            ServiceFunctions = new List<ServiceFunction>
                            {
                                new ServiceFunction
                                {
                                    Id=1,
                                    AlternativeName = "Function A",
                                    FunctionType = new FunctionTypeRefData
                                    {
                                        Id=1,
                                        FunctionName = "A Function",
                                        SortOrder = 5,
                                        Visible = true,
                                    },
                                    ServiceComponents = new List<ServiceComponent>
                                    {
                                        new ServiceComponent
                                        {
                                            Id =1,
                                            ComponentLevel = 1,
                                            ComponentName = "Component A",
                                            ServiceActivities = "Email\r\nPhone",
                                            Resolver = new Resolver
                                            {
                                                Id =1,
                                                ServiceDeskId = 2,
                                                ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData
                                                {
                                                    Id =1,
                                                    ServiceDeliveryOrganisationTypeName = "Fujitsu",
                                                    SortOrder = 5
                                                },
                                                ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData
                                                {
                                                    Id = 1,
                                                    ServiceDeliveryUnitTypeName = "Fujitsu",
                                                    SortOrder = 5
                                                },
                                                ResolverGroupType = new ResolverGroupTypeRefData
                                                {
                                                    Id =1,
                                                    ResolverGroupTypeName = "Wintel",
                                                    SortOrder = 5
                                                },
                                            }
                                        }
                                    }
                                },
                                new ServiceFunction
                                {
                                    Id=2,
                                    AlternativeName = "Function B",
                                    FunctionType = new FunctionTypeRefData
                                    {
                                        Id=2,
                                        FunctionName = "B Function",
                                        SortOrder = 5,
                                        Visible = true
                                    }
                                }
                            },

                        },
                        new ServiceDomain
                        {
                            Id =2,
                            DomainType = new DomainTypeRefData
                            {
                                Id =2,
                                DomainName = "Domain B",
                                SortOrder = 5,
                                Visible = true
                            }
                        },
                        new ServiceDomain
                        {
                            Id =3,
                            DomainType = new DomainTypeRefData
                            {
                                Id =3,
                                DomainName = "Domain C",
                                SortOrder = 5,
                                Visible = true
                            }
                        }
                    }
                },
                new ServiceDesk
                {
                    Id =3,
                    DeskName = "Customer One Service Desk B",
                    CustomerId = CustomerId,
                    DeskInputTypes = new List<DeskInputType>
                    {
                        new DeskInputType
                        {
                            Id =1,
                            InputTypeRefData = new InputTypeRefData{Id = 1,InputTypeNumber = 1, InputTypeName= "Incident", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =2,
                            InputTypeRefData = new InputTypeRefData{Id = 2,InputTypeNumber = 2, InputTypeName= "Event", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =3,
                            InputTypeRefData = new InputTypeRefData{Id = 3,InputTypeNumber = 3, InputTypeName= "Authorized Request", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =4,
                            InputTypeRefData = new InputTypeRefData{Id = 4,InputTypeNumber = 4, InputTypeName= "How do I.. Questions", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =5,
                            InputTypeRefData = new InputTypeRefData{Id = 5,InputTypeNumber = 5, InputTypeName= "Authorized Standard Change", SortOrder = 5},
                        },
                        new DeskInputType
                        {
                            Id =6,
                            InputTypeRefData = new InputTypeRefData{Id = 6,InputTypeNumber = 6, InputTypeName= "Authorized Non-Standard Change", SortOrder = 5},
                        }

                    },
                    ServiceDomains = new List<ServiceDomain>(),
                },
            };

            #endregion

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId,
                    CustomerName = "3663"
                }
            };

            _diagrams = new List<SLM.Model.Diagram>
            {
                UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>(x =>
                {
                    x.Level = 0;
                }),
                UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>(x =>
                {
                    x.Level = 2;
                }),
                UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>(x =>
                {
                    x.Level = 1;
                }),
                UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>(x =>
                {
                    x.Level = 0;
                }),
                UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>(x =>
                {
                    x.Level = 0;
                })
            };

            _diagram = UnitTestHelper.GenerateRandomData<SLM.Model.Diagram>();

            #endregion

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockContextManager = new Mock<IContextManager>();
            _mockResponseManager = new Mock<IResponseManager>();
            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);
            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _mockCacheManager = new Mock<ICacheManager>();
            _mockCacheManager.Setup(x => x.ExecuteAndCache(It.IsAny<string>(), It.IsAny<Func<string>>())).Returns("mailto:beardy@mustache-seeds.com");

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns("Matthew.Jordan@uk.fujitsu.com");

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockDiagramRepository = new Mock<IRepository<SLM.Model.Diagram>>();
            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);

            _mockServiceDeskService = new Mock<IServiceDeskService>();
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(CustomerIdWithOneServiceDesk, 2)).Returns(_serviceDesks[1]);

            _mockDiagramService = new Mock<IDiagramService>();
            _mockDiagramService.Setup(s => s.GetByCustomerId(It.Is<int>(m => m > 0))).Returns(_diagrams.AsQueryable());
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.Is<int>(m => m != 666))).Returns(_diagram.GetClone());
            _mockDiagramService.Setup(s => s.Update(It.IsAny<SLM.Model.Diagram>())).Callback<SLM.Model.Diagram>(c => _diagramUpdated = c);

            _mockResolverGroupService = new Mock<IResolverService>();
            _mockResolverGroupService.Setup(s => s.GetDotMatrix(CustomerId, true, ServiceDeskId))
                .Returns(_dotMatrixListItems);

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockFujitsuDomains = new Mock<FujitsuDomains>(_mockServiceDeskService.Object);
            _mockCustomerServices = new Mock<CustomerServices>(_mockServiceDeskService.Object);

            _diagramService = new DiagramService(_mockDiagramRepository.Object,
                _mockUnitOfWork.Object);

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            _controller = new DiagramController(
                _diagramService,
                _serviceDeskService,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object);

            _controllerWithMockedServices = new DiagramController(_mockDiagramService.Object,
                _mockServiceDeskService.Object,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoDiagramServiceThrowsException()
        {
            new DiagramController(null,
                _mockServiceDeskService.Object,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoServiceDeskServiceThrowsException()
        {
            new DiagramController(_mockDiagramService.Object,
                null,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoAppUserContextThrowsException()
        {
            new DiagramController(_mockDiagramService.Object,
                _mockServiceDeskService.Object,
                null,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoContextManagerThrowsException()
        {
            new DiagramController(_mockDiagramService.Object,
                _mockServiceDeskService.Object,
                _mockAppUserContext.Object,
                null,
                _mockUserIdentity.Object,
                _mockObjectBuilder.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoUserIdentityThrowsException()
        {
            new DiagramController(_mockDiagramService.Object,
                _mockServiceDeskService.Object,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                null,
                _mockObjectBuilder.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramController_Constructor_NoObjectManagerThrowsException()
        {
            new DiagramController(_mockDiagramService.Object,
                _mockServiceDeskService.Object,
                _mockAppUserContext.Object,
                _mockContextManager.Object,
                _mockUserIdentity.Object,
                null
                );
        }


        #endregion

        [TestMethod]
        public void DiagramController_Index_LevelZeroSupplied_ReturnsViewWithLevelAsName()
        {
            var result = _controller.Index(NavigationLevelNames.LevelZero) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(NavigationLevelNames.LevelZero, result.ViewName);
        }

        [TestMethod]
        public void DiagramController_Index_LevelZeroSupplied_LevelPlacedIntoViewModel()
        {
            var result = _controller.Index(NavigationLevelNames.LevelZero) as ViewResult;
            Assert.IsNotNull(result);
            var vm = result.Model as DiagramGridViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(NavigationLevelNames.LevelZero, vm.EditLevel);
        }

        [TestMethod]
        public void DiagramController_Index_LevelOneSupplied_ReturnsViewWithLevelAsName()
        {
            var result = _controller.Index(NavigationLevelNames.LevelOne) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void DiagramController_Index_LevelOneSupplied_LevelPlacedIntoViewModel()
        {
            var result = _controller.Index(NavigationLevelNames.LevelOne) as ViewResult;
            Assert.IsNotNull(result);
            var vm = result.Model as DiagramGridViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(NavigationLevelNames.LevelOne, vm.EditLevel);
        }

        [TestMethod]
        public void DiagramController_Index_LevelTwoSupplied_ReturnsViewWithLevelAsName()
        {
            var result = _controller.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(NavigationLevelNames.LevelTwo, result.ViewName);
        }

        [TestMethod]
        public void DiagramController_Index_LevelTwoSupplied_LevelPlacedIntoViewModel()
        {
            var result = _controller.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            Assert.IsNotNull(result);
            var vm = result.Model as DiagramGridViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(NavigationLevelNames.LevelTwo, vm.EditLevel);
        }

        [TestMethod]
        public void DiagramController_SaveDiagram_Update_CallsDiagramServiceCreate()
        {
            const string fileName = "xyz.pdf";
            _controllerWithMockedServices.SaveDiagram(DataUri, fileName, 0);
            _mockDiagramService.Verify(x => x.Create(It.IsAny<SLM.Model.Diagram>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_SaveDiagram_Update_CallsRepositoryInsertAndUnitOfWork()
        {
            const string fileName = "xyz.pdf";
            _controller.SaveDiagram(DataUri, fileName, 0);
            _mockDiagramRepository.Verify(x => x.Insert(It.IsAny<SLM.Model.Diagram>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);
        }

        [TestMethod]
        public void DiagramController_SaveDiagram_ExceptionAppendsErrorMessageToHeader()
        {
            const string fileName = "xyz.pdf";
            _mockDiagramService.Setup(s => s.Create(It.IsAny<SLM.Model.Diagram>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.SaveDiagram(DataUri, fileName, 0);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_ReadAjaxDiagramGrid_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _controllerWithMockedServices.GetMethodAttributeValue("ReadAjaxDiagramGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void DiagramController_ReadAjaxDiagramGrid_CurrentCustomerNotSet_ResultDataEmpty()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext() { CurrentCustomer = null });
            var result = _controllerWithMockedServices.ReadAjaxDiagramGrid(new DataSourceRequest(), NavigationLevelNames.LevelZero) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void DiagramController_ReadAjaxDiagramGrid_CurrentCustomerSet_ResultHasData()
        {
            var result = _controllerWithMockedServices.ReadAjaxDiagramGrid(new DataSourceRequest(), NavigationLevelNames.LevelZero) as JsonResult;
            var dataWrapper = result.Data as DataSourceResult;
            var data = dataWrapper.Data as List<DiagramViewModel>;
            Assert.AreEqual(3, data.Count);
            Assert.IsFalse(data.Any(x => x.Level > 0));
        }

        [TestMethod]
        public void DiagramController_ReadAjaxDiagramGrid_Exception_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerId(1)).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxDiagramGrid(new DataSourceRequest(), NavigationLevelNames.LevelZero);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void DiagramController_ReadAjaxDiagramGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerId(1)).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxDiagramGrid(new DataSourceRequest(), NavigationLevelNames.LevelZero);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_ModelStateIsNotValid_UpdateNotInvoked()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockDiagramService.Verify(v => v.Update(It.IsAny<SLM.Model.Diagram>()), Times.Never);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_ModelStateIsNotValid_ErrorIsIncludedInResponse()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var response = _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel()) as JsonResult;
            var dsr = response.Data as DataSourceResult;
            Assert.IsNotNull(dsr);
            var err = dsr.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.IsNotNull(err);
            Assert.IsTrue(err.ContainsKey("XXX"));
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_CurrentCustomerNotSet_NothingHappens()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            var jsonResult = _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel()) as JsonResult;
            _mockDiagramService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramCannotBeFound_ErrorAddedToModelState()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext() { CurrentCustomer = new CurrentCustomerViewModel() { Id = 999 } });
            var response = _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel { Id = 666 }) as JsonResult;
            var dsr = response.Data as DataSourceResult;
            Assert.IsNotNull(dsr);
            var err = dsr.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.IsNotNull(err);
            Assert.IsTrue(err.ContainsKey(ModelStateErrorNames.DiagramCannotBeFound));
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_DiagramIdUsedFromModel()
        {
            var response = _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(),
                    new DiagramViewModel() { Id = 666 }) as JsonResult;
            _mockDiagramService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), 666), Times.Once);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_OnlyAllowedFieldsUpdated()
        {
            var vm = UnitTestHelper.GenerateRandomData<DiagramViewModel>();
            var response = _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), vm) as JsonResult;
            var compare = new CompareLogic(
                new ComparisonConfig
                {
                    IgnoreObjectTypes = true,
                    MaxDifferences = 100,
                    MembersToIgnore = new List<string>
                    {
                        "Filename",
                        "DiagramNotes",
                        "UpdatedBy",
                        "UpdatedDate"
                    }
                });
            var same = compare.Compare(_diagram, _diagramUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_FilenameUpdated()
        {
            const string s = "XXX";
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(),
                    new DiagramViewModel { Filename = s });
            Assert.AreEqual(s, _diagramUpdated.Filename);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_DiagramNotesUpdated()
        {
            const string s = "XXX";
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(),
                    new DiagramViewModel { DiagramNotes = s });
            Assert.AreEqual(s, _diagramUpdated.DiagramNotes);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_UpdatedByUpdated()
        {
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(),
                    new DiagramViewModel());
            Assert.AreEqual(UserName, _diagramUpdated.UpdatedBy);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_DiagramUpdated_UpdatedDateUpdated()
        {
            var now = DateTime.Now;
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(),
                    new DiagramViewModel());
            Assert.AreEqual(now.Year, _diagramUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _diagramUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _diagramUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _diagramUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _diagramUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controllerWithMockedServices.GetMethodAttributeValue("UpdateAjaxDiagramGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_Exception_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void DiagramController_UpdateAjaxDiagramGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_ModelStateValid_CorrectServiceCalledToGetDiagram()
        {
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockDiagramService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_ServiceCalledWithCorrectParameters_IdIsUsedFromModel()
        {
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(),
                new DiagramViewModel() { Id = 666 });
            _mockDiagramService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), 666), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_ServiceCalledWithCorrectParameters_CustomerIdIsUsedFromContext()
        {
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(),
                new DiagramViewModel() { Id = 666 });
            _mockDiagramService.Verify(v => v.GetByCustomerAndId(1, It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_DiagramNotFound_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Returns(null as SLM.Model.Diagram);
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_DiagramNotFound_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Returns(null as SLM.Model.Diagram);
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_ModelStateIsNotValid_DeleteNotInvoked()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockDiagramService.Verify(v => v.Delete(It.IsAny<SLM.Model.Diagram>()), Times.Never);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_DiagramFound_DeleteInvoked()
        {
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockDiagramService.Verify(v => v.Delete(It.IsAny<SLM.Model.Diagram>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controllerWithMockedServices.GetMethodAttributeValue("DeleteAjaxDiagramGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_Exception_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void DiagramController_DeleteAjaxDiagramGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DeleteAjaxDiagramGrid(new DataSourceRequest(), new DiagramViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_DiagramNotFound_NullActionResultReturned()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as SLM.Model.Diagram);
            var result = _controllerWithMockedServices.DownloadDiagram(1);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_DiagramFound_ContentDispositionAddedToHeader()
        {
            _controllerWithMockedServices.DownloadDiagram(1);
            _mockResponseManager.Verify(v => v.AppendHeader("Content-Disposition", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_DiagramFound_FileStreamResultReturned()
        {
            var result = _controllerWithMockedServices.DownloadDiagram(1) as FileStreamResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _controllerWithMockedServices.GetMethodAttributeValue("DownloadDiagram", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_Exception_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DownloadDiagram(1);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void DiagramController_DownloadDiagram_Exception_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DownloadDiagram(1);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #region Fujitsu Domains

        [TestMethod]
        public void DiagramController_FujitsuDomains_Get_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.FujitsuDomains(0) as ViewResult;
            Assert.IsNotNull(result);
        }


        public void DiagramController_FujitsuDomainsDiagram_Get_ReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}";
            var result = _controller.FujitsuDomainsDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_Get_ReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}";
            var result = _controller.FujitsuDomainsDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }


        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_Get_ReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}";
            var result = _controller.FujitsuDomainsDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomains_Get_CustomerWithOneServiceDeskRedirectsToLevelZeroActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.FujitsuDomains(0) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("FujitsuDomainsDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.FujitsuDomainsDiagram(0, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_ReturnsChartViewModel()
        {
            var result = _controller.FujitsuDomainsDiagram(0, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_Level0_ReturnsChartViewModelWithLevel0()
        {
            var result = _controller.FujitsuDomainsDiagram(0, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(0, viewModel.Level);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_Level1_ReturnsChartViewModelWithLevel1()
        {
            var result = _controller.FujitsuDomainsDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
        }

        [TestMethod]
        public void DiagramController_FujitsuDomainsDiagram_DeskId2_ReturnsChartViewModelWithDeskId2()
        {
            var result = _controller.FujitsuDomainsDiagram(1, 2) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(2, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ReadFujitsuDomainsChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.FujitsuDomains))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadFujitsuDomainsChart(1);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.FujitsuDomains), Times.Once);

            #endregion
        }

        #endregion

        #region Customer Services

        [TestMethod]
        public void DiagramController_CustomerServices_Get_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.CustomerServices(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        public void DiagramController_CustomerServicesDiagram_Get_ReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelZero} {Diagram.CustomerServicesTitle}";
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_Get_ReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelZero} {Diagram.CustomerServicesTitle}";
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }


        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_Get_ReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelZero} {Diagram.CustomerServicesTitle}";
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_CustomerServices_Get_CustomerWithOneServiceDeskRedirectsToLevelZeroActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.CustomerServices(1) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("CustomerServicesDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_ReturnsChartViewModel()
        {
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_LevelOne_ReturnsChartViewModelWithLevel0()
        {
            var result = _controller.CustomerServicesDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_LevelTwo_ReturnsChartViewModelWithLevel1()
        {
            var result = _controller.CustomerServicesDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(2, viewModel.Level);
        }

        [TestMethod]
        public void DiagramController_CustomerServicesDiagram_DeskId2_ReturnsChartViewModelWithDeskId2()
        {
            var result = _controller.CustomerServicesDiagram(1, 2) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(2, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ReadCustomerServicesChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.CustomerServices))
                .Returns(_mockCustomerServices.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadCustomerServicesChart(1);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.CustomerServices), Times.Once);

            #endregion
        }

        #endregion

        #region Service Desk Structure

        [TestMethod]
        public void DiagramController_ServiceDeskStructure_Get_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.ServiceDeskStructure(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelOneReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelOne} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelTwoReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelTwo} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelOneReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelOne} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelTwoReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelTwo} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelOneReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelOne} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_LevelTwoReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelTwo} {Diagram.ServiceDeskStructureTitle}";
            var result = _controller.ServiceDeskStructureDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }


        [TestMethod]
        public void DiagramController_ServiceDeskStructure_Get_CustomerWithOneServiceDeskRedirectsToLevelOneActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceDeskStructure(1) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("ServiceDeskStructureDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructure_Get_CustomerWithOneServiceDeskRedirectsToLevelTwoActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceDeskStructure(2) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("ServiceDeskStructureDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelWithLevelAndIdSet()
        {
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
            Assert.AreEqual(1, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelWithInlineDomainDataOfCorrectType()
        {
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel.InlineDomainData, typeof(IEnumerable<TreeViewItemModel>));
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelWithCorrectRootInlineDomainData()
        {
            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerIdWithOneServiceDesk,
                    CustomerName = "Matt Test Customer"
                },
            };

            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            var result = _controller.ServiceDeskStructureDiagram(CustomerIdWithOneServiceDesk, 2) as ViewResult;
            var viewModel = result.Model as ChartViewModel;


            Assert.IsNotNull(viewModel);

            var inlineDomainData = viewModel.InlineDomainData.ToList();
            var root = inlineDomainData[0];

            Assert.AreEqual("0", root.Id);
            Assert.AreEqual("Service Domains", root.Text);
            Assert.IsTrue(root.Checked);
            Assert.IsTrue(root.Expanded);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelWithCorrectChildrenInlineDomainData()
        {
            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerIdWithOneServiceDesk,
                    CustomerName = "Matt Test Customer"
                },
            };

            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            var result = _controller.ServiceDeskStructureDiagram(CustomerIdWithOneServiceDesk, 2) as ViewResult;
            var viewModel = result.Model as ChartViewModel;


            Assert.IsNotNull(viewModel);

            var inlineDomainData = viewModel.InlineDomainData.ToList();
            var root = inlineDomainData[0];

            Assert.AreEqual("2", root.Items[1].Id);
            Assert.AreEqual("Domain B", root.Items[1].Text);
            Assert.IsTrue(root.Items[1].Checked);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelChildrenInlineDomainDataUsesAlternativeDomainName()
        {
            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerIdWithOneServiceDesk,
                    CustomerName = "Matt Test Customer"
                },
            };

            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            var result = _controller.ServiceDeskStructureDiagram(CustomerIdWithOneServiceDesk, 2) as ViewResult;

            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);

            var inlineDomainData = viewModel.InlineDomainData.ToList();
            var root = inlineDomainData[0];

            Assert.AreEqual("1", root.Items[0].Id);
            Assert.AreEqual("Domain A", root.Items[0].Text);
            Assert.IsTrue(root.Items[0].Checked);
        }

        [TestMethod]
        public void DiagramController_ServiceDeskStructureDiagram_ReturnsChartViewModelWithDefaultEntitiesSelected()
        {
            var result = _controller.ServiceDeskStructureDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.IsTrue(viewModel.ServiceDomains);
            Assert.IsTrue(viewModel.ServiceFunctions);
            Assert.IsTrue(viewModel.ServiceComponents);
            Assert.IsTrue(viewModel.Resolvers);
            Assert.IsFalse(viewModel.ServiceActivities);
            Assert.IsFalse(viewModel.OperationalProcesses);
        }

        [TestMethod]
        public void DiagramController_ReadServiceDeskStructureChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.ServiceDeskStructure))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadServiceDeskStructureChart(1, true, new string[0], true, true, false, false, false);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.ServiceDeskStructure), Times.Once);

            #endregion
        }

        #endregion

        #region Dot Matrix Diagram

        [TestMethod]
        public void DiagramController_DotMatrix_Get_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.DotMatrix(0) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_DotMatrix_Get_CustomerWithOneServiceDeskRedirectsToLevelZeroActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.DotMatrix(0) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("DotMatrixDiagram", result.RouteValues["action"]);
        }


        [TestMethod]
        public void DiagramController_DotMatrix_LevelPassed_ModelIsLevelValue()
        {
            var result = _controllerWithMockedServices.DotMatrix(1) as ViewResult;
            var model = result.Model;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelOneReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelOne} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelTwoReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelTwo} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelOneReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelOne} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelTwoReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelTwo} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelOneReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelOne} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_Get_LevelTwoReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelTwo} {Diagram.ServiceDeskDotMatrixTitle}";
            var result = _controller.DotMatrixDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_LevelPassed_ModelPopulatedWithLevel()
        {
            var result = _controllerWithMockedServices.DotMatrixDiagram(1, 2) as ViewResult;
            var model = result.Model as ChartViewModel;
            Assert.AreEqual(1, model.Level);
        }

        [TestMethod]
        public void DiagramController_DotMatrixDiagram_IdPassed_ModelPopulatedWithId()
        {
            var result = _controllerWithMockedServices.DotMatrixDiagram(1, 2) as ViewResult;
            var model = result.Model as ChartViewModel;
            Assert.AreEqual(2, model.Id);
        }

        [TestMethod]
        public void DiagramController_ReadServiceDeskDotMatrixChart_ContextNull_ResultIsEmptyList()
        {
            SetCustomerContextNull();
            var result = _controllerWithMockedServices.ReadServiceDeskDotMatrixChart(ServiceDeskId) as JsonResult;
            var data = result.Data as List<ChartDataViewModel>;
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void DiagramController_ReadServiceDeskDotMatrixChart_ContextZero_ResultIsEmptyList()
        {
            SetCustomerContextZero();
            var result = _controllerWithMockedServices.ReadServiceDeskDotMatrixChart(ServiceDeskId) as JsonResult;
            var data = result.Data as List<ChartDataViewModel>;
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void DiagramController_ReadServiceDeskDotMatrixChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.ServiceDeskDotMatrix))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadServiceDeskDotMatrixChart(1);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.ServiceDeskDotMatrix), Times.Once);

            #endregion
        }

        #endregion

        #region Service Organisation Diagrams

        #region Fujitsu Service Organisation

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Fujitsu_Get_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.FujitsuServiceOrganisation) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Fujitsu_Get_CustomerWithMoreThanOneServiceDeskReturnsCorrectViewModel()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.FujitsuServiceOrganisation) as ViewResult;
            var viewModel = result.Model as ServiceOrganisationViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ServiceOrganisationViewModel));
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelOneReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelOne} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelTwo} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelOneReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelOne} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelTwo} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelOneReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelOne} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelTwo} {Diagram.FujitsuServiceOrganisationTitle}";
            var result = _controller.FujitsuServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Fujitsu_Get_CustomerWithOneServiceDeskRedirectsToLevelOneActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(1, Diagram.FujitsuServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("FujitsuServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Fujitsu_Get_CustomerWithOneServiceDeskRedirectsToLevelTwoActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(2, Diagram.FujitsuServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("FujitsuServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.FujitsuServiceOrganisationDiagram(1, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_FujitsuServiceOrganisationDiagram_ReturnsChartViewModelWithLevelAndIdSet()
        {
            var result = _controller.FujitsuServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
            Assert.AreEqual(1, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ReadFujitsuServiceOrganisationChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.FujitsuServiceOrganisation))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadFujitsuServiceOrganisationChart(1, true, true, true);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.FujitsuServiceOrganisation), Times.Once);

            #endregion
        }

        #endregion

        #region Customer Service Organisation

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Get_Customer_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.CustomerServiceOrganisation) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Customer_Get_CustomerWithMoreThanOneServiceDeskReturnsCorrectViewModel()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.CustomerServiceOrganisation) as ViewResult;
            var viewModel = result.Model as ServiceOrganisationViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ServiceOrganisationViewModel));
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelOneReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelOne} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelTwo} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelOneReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelOne} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelTwo} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelOneReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelOne} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelTwo} {Diagram.CustomerServiceOrganisationTitle}";
            var result = _controller.CustomerServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_CustomerServiceOrganisation_Get_CustomerWithOneServiceDeskRedirectsToLevelOneActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(1, Diagram.CustomerServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("CustomerServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_CustomerServiceOrganisation_Get_CustomerWithOneServiceDeskRedirectsToLevelTwoActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(2, Diagram.CustomerServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("CustomerServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisationDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.CustomerServiceOrganisationDiagram(1, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_CustomerServiceOrganisation_ReturnsChartViewModelWithLevelAndIdSet()
        {
            var result = _controller.CustomerServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
            Assert.AreEqual(1, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ReadCustomerServiceOrganisationChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.CustomerServiceOrganisation))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadCustomerServiceOrganisationChart(1, true, true, true);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.CustomerServiceOrganisation), Times.Once);

            #endregion
        }


        #endregion

        #region Customer Third Party Service Organisation

        [TestMethod]
        public void DiagramController_ServiceOrganisation_Get_CustomerThirdParty_CustomerWithMoreThanOneServiceDeskReturnsViewResult()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.CustomerThirdPartyServiceOrganisation) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_CustomerThirdParty_Get_CustomerWithMoreThanOneServiceDeskReturnsCorrectViewModel()
        {
            var result = _controller.ServiceOrganisation(1, Diagram.CustomerThirdPartyServiceOrganisation) as ViewResult;
            var viewModel = result.Model as ServiceOrganisationViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ServiceOrganisationViewModel));
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelOneReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelOne} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectTitle()
        {
            var expectedTitle = $"3663 {LevelNames.LevelTwo} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedTitle, viewModel.Title);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelOneReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelOne} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectSubject()
        {
            var expectedSubject = $"{LevelNames.LevelTwo} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedSubject, viewModel.Subject);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelOneReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelOne} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_LevelTwoReturnsCorrectFilename()
        {
            var expectedFilename = $"{LevelNames.LevelTwo} {Diagram.CustomerThirdPartyServiceOrganisationTitle}";
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(2, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedFilename, viewModel.Filename);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_CustomerThirdPartyServiceOrganisation_Get_CustomerWithOneServiceDeskRedirectsToLevelOneActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(1, Diagram.CustomerThirdPartyServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("CustomerThirdPartyServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_ServiceOrganisation_CustomerThirdPartyServiceOrganisation_Get_CustomerWithOneServiceDeskRedirectsToLevelTwoActionResult()
        {
            _appContext.CurrentCustomer.Id = CustomerIdWithOneServiceDesk;

            var result = _controller.ServiceOrganisation(2, Diagram.CustomerThirdPartyServiceOrganisation) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["level"]);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("CustomerThirdPartyServiceOrganisationDiagram", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisationDiagram_Get_ReturnsViewResult()
        {
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(1, 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DiagramController_CustomerThirdPartyServiceOrganisation_ReturnsChartViewModelWithLevelAndIdSet()
        {
            var result = _controller.CustomerThirdPartyServiceOrganisationDiagram(1, 1) as ViewResult;
            var viewModel = result.Model as ChartViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(ChartViewModel));
            Assert.AreEqual(1, viewModel.Level);
            Assert.AreEqual(1, viewModel.Id);
        }

        [TestMethod]
        public void DiagramController_ReadCustomerThirdPartyServiceOrganisationChart_ExecutesCorrectGenerator()
        {
            #region

            _mockObjectBuilder.Setup(s => s.Resolve<IDiagramGenerator>(Diagram.CustomerThirdPartyServiceOrganisation))
                .Returns(_mockFujitsuDomains.Object);

            #endregion

            #region Act

            _controllerWithMockedServices.ReadCustomerThirdPartyServiceOrganisationChart(1, true, true, true);

            #endregion

            #region Assert

            _mockObjectBuilder.Verify(x => x.Resolve<IDiagramGenerator>(Diagram.CustomerThirdPartyServiceOrganisation), Times.Once);

            #endregion
        }

        #endregion

        #endregion

        #region Method Authorization Requirement Tests

        [TestMethod]
        public void DiagramController_SaveDiagram_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("SaveDiagram", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(string), typeof(string), typeof(int) }));
        }

        #endregion

        #region Helpers

        private void SetCustomerContextNull()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = null });
        }

        private void SetCustomerContextZero()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = new CurrentCustomerViewModel { Id = 0 } });
        }

        #endregion
    }
}
