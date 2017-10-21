using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DiagramServiceTests
    {

        private Mock<IRepository<Diagram>> _mockDiagramRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IDiagramService _diagramService;
        private List<Diagram> _diagrams;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";
        private const string Data = "JVBERi0xLjQKJcLB2s/OCgoxIDAgb2JqIDw8CiAgL1R5cGUgL0NhdGFsb2cKICAvUGFnZXMgMiAwIFIKPj4gZW5kb2JqCgoyIDAgb2JqIDw8CiAgL1R5cGUgL1BhZ2VzCiAgL0tpZHMgWyA0IDAgUiBdCiAgL0NvdW50IDEKPj4gZW5kb2JqCgozIDAgb2JqIDw8CiAgL0xlbmd0aCA3MDI0Cj4+IHN0cmVhbQoxIDAgMCAtMSAwIDMyNS42OTI5MTM0IGNtCjEgMCAwIDEgMjguMzQ2NDU2NyAyOC4zNDY0NTY3IGNtCjAgMCAyMjAwIDI2OSByZQpXIG4KcQoxIDAgMCAxIC01MCAtNTAgY20KcQpxCnEKMSAwIDAgMSA1MCAyNTIgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjMgdwoxIDEgMSByZwowIDAgMjIwMCA2NyByZQpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTAzMiAyNC41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEZ1aml0c3UgU2VydmljZSBEZXNrKSBUagpFVApRClEKcQoxIDAgMCAxIDE2MyAxMTAgY20KcQpxCjAuMzQ1MDk4IDAuMzkyMTU2OSAwLjQ2NjY2NjcgUkcKMiB3CjEgMSAxIHJnCjI1IDUwIG0KMzguODA3MTE4NyA1MCA1MCAzOC44MDcxMTg3IDUwIDI1IGMKNTAgMTEuMTkyODgxMyAzOC44MDcxMTg3IDAgMjUgMCBjCjExLjE5Mjg4MTMgMCAwIDExLjE5Mjg4MTMgMCAyNSBjCjAgMzguODA3MTE4NyAxMS4xOTI4ODEzIDUwIDI1IDUwIGMKQgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDIxLjUgMTUuNSBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCigxKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKMTg5IDE5NyBtCjE4OSAxNzkuNSBsCjE4OSAxNzkuNSBsCjE4OSAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDE5NCAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDY0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA5Ni41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooSW5jaWRlbnQpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODkgMTEwIG0KMTg5IDkyLjUgbAoxODkgOTIuNSBsCjE4OSA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDQzOCAxMTAgY20KcQpxCjAuMzQ1MDk4IDAuMzkyMTU2OSAwLjQ2NjY2NjcgUkcKMiB3CjEgMSAxIHJnCjI1IDUwIG0KMzguODA3MTE4NyA1MCA1MCAzOC44MDcxMTg3IDUwIDI1IGMKNTAgMTEuMTkyODgxMyAzOC44MDcxMTg3IDAgMjUgMCBjCjExLjE5Mjg4MTMgMCAwIDExLjE5Mjg4MTMgMCAyNSBjCjAgMzguODA3MTE4NyAxMS4xOTI4ODEzIDUwIDI1IDUwIGMKQgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDIxLjUgMTUuNSBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCigyKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKNDY0IDE5NyBtCjQ2NCAxNzkuNSBsCjQ2NCAxNzkuNSBsCjQ2NCAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDQ2OSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDMzOSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTA1LjUgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihFdmVudCkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjQ2NCAxMTAgbQo0NjQgOTIuNSBsCjQ2NCA5Mi41IGwKNDY0IDc1IGwKUwpRClEKUQpxCjEgMCAwIDEgNzEzIDExMCBjbQpxCnEKMC4zNDUwOTggMC4zOTIxNTY5IDAuNDY2NjY2NyBSRwoyIHcKMSAxIDEgcmcKMjUgNTAgbQozOC44MDcxMTg3IDUwIDUwIDM4LjgwNzExODcgNTAgMjUgYwo1MCAxMS4xOTI4ODEzIDM4LjgwNzExODcgMCAyNSAwIGMKMTEuMTkyODgxMyAwIDAgMTEuMTkyODgxMyAwIDI1IGMKMCAzOC44MDcxMTg3IDExLjE5Mjg4MTMgNTAgMjUgNTAgYwpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMjEuNSAxNS41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKDMpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwo3MzkgMTk3IG0KNzM5IDE3OS41IGwKNzM5IDE3OS41IGwKNzM5IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgNzQ0IDE4NyBjbQowIDAgbQoxMCA1IGwKMCAxMCBsCjMgNSBsCmgKZgpRClEKUQpxCjEgMCAwIDEgNjE0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA1NS41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooQXV0aG9yaXplZCBSZXF1ZXN0KSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKNzM5IDExMCBtCjczOSA5Mi41IGwKNzM5IDkyLjUgbAo3MzkgNzUgbApTClEKUQpRCnEKMSAwIDAgMSA5ODggMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooNCkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEwMTQgMTk3IG0KMTAxNCAxNzkuNSBsCjEwMTQgMTc5LjUgbAoxMDE0IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTAxOSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDg4OSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgNTIgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihIb3cgZG8gSS4uIFF1ZXN0aW9ucykgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEwMTQgMTEwIG0KMTAxNCA5Mi41IGwKMTAxNCA5Mi41IGwKMTAxNCA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDEyNjMgMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooNikgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEyODkgMTk3IG0KMTI4OSAxNzkuNSBsCjEyODkgMTc5LjUgbAoxMjg5IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTI5NCAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDExNjQgNTAgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjEgMSAxIHJnCjAgMCAyNTAgMjUgcmUKZgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDQuNSAyIGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEF1dGhvcml6ZWQgTm9uLVN0YW5kYXJkIENoYW5nZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjEyODkgMTEwIG0KMTI4OSA5Mi41IGwKMTI4OSA5Mi41IGwKMTI4OSA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDE1MzggMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAyMS41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooOSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjE1NjQgMTk3IG0KMTU2NCAxNzkuNSBsCjE1NjQgMTc5LjUgbAoxNTY0IDE2MiBsClMKUQpxCjEgMSAxIFJHCi9HUzMgZ3MKMCAwIDAgcmcKMCAxIC0xIDAgMTU2OSAxODcgY20KMCAwIG0KMTAgNSBsCjAgMTAgbAozIDUgbApoCmYKUQpRClEKcQoxIDAgMCAxIDE0MzkgNTAgY20KcQpxCjAuNTAxOTYwOCAwLjUwMTk2MDggMC41MDE5NjA4IFJHCjEgMSAxIHJnCjAgMCAyNTAgMjUgcmUKZgpRClEKcQowLjE4MDM5MjIgMC4xODAzOTIyIDAuMTgwMzkyMiByZwoxIDAgMCAxIDU1LjUgMiBjbQoxIDAgMCAtMSAwIDE2IGNtCkJUCi9GMSAxNiBUZgowIFRyCihDb25maWcgTmV3IFVwZGF0ZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjE1NjQgMTEwIG0KMTU2NCA5Mi41IGwKMTU2NCA5Mi41IGwKMTU2NCA3NSBsClMKUQpRClEKcQoxIDAgMCAxIDE4MTMgMTEwIGNtCnEKcQowLjM0NTA5OCAwLjM5MjE1NjkgMC40NjY2NjY3IFJHCjIgdwoxIDEgMSByZwoyNSA1MCBtCjM4LjgwNzExODcgNTAgNTAgMzguODA3MTE4NyA1MCAyNSBjCjUwIDExLjE5Mjg4MTMgMzguODA3MTE4NyAwIDI1IDAgYwoxMS4xOTI4ODEzIDAgMCAxMS4xOTI4ODEzIDAgMjUgYwowIDM4LjgwNzExODcgMTEuMTkyODgxMyA1MCAyNSA1MCBjCkIKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSAxNy41IDE1LjUgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooMTIpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODM5IDE5NyBtCjE4MzkgMTc5LjUgbAoxODM5IDE3OS41IGwKMTgzOSAxNjIgbApTClEKcQoxIDEgMSBSRwovR1MzIGdzCjAgMCAwIHJnCjAgMSAtMSAwIDE4NDQgMTg3IGNtCjAgMCBtCjEwIDUgbAowIDEwIGwKMyA1IGwKaApmClEKUQpRCnEKMSAwIDAgMSAxNzE0IDUwIGNtCnEKcQowLjUwMTk2MDggMC41MDE5NjA4IDAuNTAxOTYwOCBSRwoxIDEgMSByZwowIDAgMjUwIDI1IHJlCmYKUQpRCnEKMC4xODAzOTIyIDAuMTgwMzkyMiAwLjE4MDM5MjIgcmcKMSAwIDAgMSA3Mi41IDIgY20KMSAwIDAgLTEgMCAxNiBjbQpCVAovRjEgMTYgVGYKMCBUcgooU3RhdHVzIFJlcXVlc3QpIFRqCkVUClEKUQpxCnEKcQowLjU5MjE1NjkgMC41OTIxNTY5IDAuNTkyMTU2OSBSRwoyIHcKMSAxIDEgcmcKL0dTMiBncwoxODM5IDExMCBtCjE4MzkgOTIuNSBsCjE4MzkgOTIuNSBsCjE4MzkgNzUgbApTClEKUQpRCnEKMSAwIDAgMSAyMDg4IDExMCBjbQpxCnEKMC4zNDUwOTggMC4zOTIxNTY5IDAuNDY2NjY2NyBSRwoyIHcKMSAxIDEgcmcKMjUgNTAgbQozOC44MDcxMTg3IDUwIDUwIDM4LjgwNzExODcgNTAgMjUgYwo1MCAxMS4xOTI4ODEzIDM4LjgwNzExODcgMCAyNSAwIGMKMTEuMTkyODgxMyAwIDAgMTEuMTkyODgxMyAwIDI1IGMKMCAzOC44MDcxMTg3IDExLjE5Mjg4MTMgNTAgMjUgNTAgYwpCClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgMTcuNSAxNS41IGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKDEzKSBUagpFVApRClEKcQpxCnEKMC41OTIxNTY5IDAuNTkyMTU2OSAwLjU5MjE1NjkgUkcKMiB3CjEgMSAxIHJnCi9HUzIgZ3MKMjExNCAxOTcgbQoyMTE0IDE3OS41IGwKMjExNCAxNzkuNSBsCjIxMTQgMTYyIGwKUwpRCnEKMSAxIDEgUkcKL0dTMyBncwowIDAgMCByZwowIDEgLTEgMCAyMTE5IDE4NyBjbQowIDAgbQoxMCA1IGwKMCAxMCBsCjMgNSBsCmgKZgpRClEKUQpxCjEgMCAwIDEgMTk4OSA1MCBjbQpxCnEKMC41MDE5NjA4IDAuNTAxOTYwOCAwLjUwMTk2MDggUkcKMSAxIDEgcmcKMCAwIDI1MCAyNSByZQpmClEKUQpxCjAuMTgwMzkyMiAwLjE4MDM5MjIgMC4xODAzOTIyIHJnCjEgMCAwIDEgNjguNSAyIGNtCjEgMCAwIC0xIDAgMTYgY20KQlQKL0YxIDE2IFRmCjAgVHIKKEluY2lkZW50IFVwZGF0ZSkgVGoKRVQKUQpRCnEKcQpxCjAuNTkyMTU2OSAwLjU5MjE1NjkgMC41OTIxNTY5IFJHCjIgdwoxIDEgMSByZwovR1MyIGdzCjIxMTQgMTEwIG0KMjExNCA5Mi41IGwKMjExNCA5Mi41IGwKMjExNCA3NSBsClMKUQpRClEKUQpRClEKCmVuZHN0cmVhbSBlbmRvYmoKCjQgMCBvYmogPDwKICAvQ29udGVudHMgMyAwIFIKICAvUGFyZW50IDIgMCBSCiAgL01lZGlhQm94IFsgMCAwIDIyNTYuNjkyOTEzNCAzMjUuNjkyOTEzNCBdCiAgL1R5cGUgL1BhZ2UKICAvUHJvY1NldCBbIC9QREYgL1RleHQgL0ltYWdlQiAvSW1hZ2VDIC9JbWFnZUkgXQogIC9SZXNvdXJjZXMgPDwKICAgIC9Gb250IDw8CiAgICAgIC9GMSA1IDAgUgogICAgPj4KICAgIC9FeHRHU3RhdGUgPDwKICAgICAgL0dTMiA2IDAgUgogICAgICAvR1MzIDcgMCBSCiAgICA+PgogICAgL1hPYmplY3QgPDw+PgogICAgL1BhdHRlcm4gPDw+PgogICAgL1NoYWRpbmcgPDw+PgogID4+Cj4+IGVuZG9iagoKNSAwIG9iaiA8PAogIC9UeXBlIC9Gb250CiAgL1N1YnR5cGUgL1R5cGUxCiAgL0Jhc2VGb250IC9UaW1lcy1Sb21hbgo+PiBlbmRvYmoKCjYgMCBvYmogPDwKICAvVHlwZSAvRXh0R1N0YXRlCiAgL2NhIDAKPj4gZW5kb2JqCgo3IDAgb2JqIDw8CiAgL1R5cGUgL0V4dEdTdGF0ZQogIC9DQSAwCj4+IGVuZG9iagoKeHJlZgowIDgKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDE3IDAwMDAwIG4gCjAwMDAwMDAwNzEgMDAwMDAgbiAKMDAwMDAwMDEzNyAwMDAwMCBuIAowMDAwMDA3MjE2IDAwMDAwIG4gCjAwMDAwMDc1NTEgMDAwMDAgbiAKMDAwMDAwNzYzMCAwMDAwMCBuIAowMDAwMDA3Njc5IDAwMDAwIG4gCgp0cmFpbGVyCjw8CiAgL1NpemUgOAogIC9Sb290IDEgMCBSCiAgL0luZm8gPDwKICAgIC9Qcm9kdWNlciAoS2VuZG8gVUkgUERGIEdlbmVyYXRvcikKICAgIC9UaXRsZSAoTWF0dCBUZXN0IEN1c3RvbWVyIFNlcnZpY2UgRGVzayBXaXRoIElucHV0cykKICAgIC9BdXRob3IgKG1hdHRoZXcuam9yZGFuQHVrLmZ1aml0c3UuY29tKQogICAgL1N1YmplY3QgKFNlcnZpY2UgRGVzayB3aXRoIElucHV0cyBEaWFncmFtKQogICAgL0tleXdvcmRzICgpCiAgICAvQ3JlYXRvciAoU2VydmljZSBEZWNvbXBvc2l0aW9uIFRvb2wgdiAwLjEpCiAgICAvQ3JlYXRpb25EYXRlIChEOjIwMTUwMzA0MTM0MzI0WikKICA+Pgo+PgoKc3RhcnR4cmVmCjc3MjgKJSVFT0YK";
        private const string MimeType = "application/pdf";

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _diagrams = new List<Diagram>
            {
                new Diagram
                {
                    Id =1,
                    Level = 0,
                    CustomerId = 1,
                    MimeType = MimeType,
                    Filename = "ServiceDeskWithInputs",
                    DiagramData = Convert.FromBase64String(Data),
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                new Diagram
                {
                    Id =2,
                    Level = 0,
                    Filename = "ServiceDeskWithInputs",
                    CustomerId = 1,
                    MimeType = MimeType,
                    DiagramData = Convert.FromBase64String(Data),
                    InsertedBy = UserNameTwo,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameTwo,
                    UpdatedDate = dateTimeNow,
                },
                new Diagram
                {
                    Id =3,
                    Level = 1,
                    Filename = "ServiceDeskWithDomains",
                    CustomerId = 2,
                    MimeType = MimeType,
                    DiagramData = Convert.FromBase64String(Data),
                    InsertedBy = UserNameTwo,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameTwo,
                    UpdatedDate = dateTimeNow,
                },
                new Diagram
                {
                    Id =4,
                    Level = 1,
                    Filename = "ServiceDeskWithDomains",
                    CustomerId = 1,
                    MimeType = MimeType,
                    DiagramData = Convert.FromBase64String(Data),
                    InsertedBy = UserNameTwo,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameTwo,
                    UpdatedDate = dateTimeNow,
                },
            };

            _mockDiagramRepository = MockRepositoryHelper.Create(_diagrams, (entity, id) => entity.Id == (int)id);

            _diagramService = new DiagramService(
                _mockDiagramRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramService_Constructor_NoDiagramRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new DiagramService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiagramService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new DiagramService(
                _mockDiagramRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void DiagramService_Create_CallInsertDiagramAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var diagram = new Diagram
            {
                Id = 5,
                Level = 1,
                CustomerId = 3,
                MimeType = MimeType,
                Filename = "ServiceDeskWithInputs",
                DiagramData = Convert.FromBase64String(Data),
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _diagramService.Create(diagram);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.Insert(It.IsAny<Diagram>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(_diagrams.Count, response);

            #endregion
        }

        [TestMethod]
        public void DiagramService_Update_CallUpdateDiagramAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var diagram = new Diagram
            {
                Id = 2,
                CustomerId = 2,
                DiagramNotes = "Some diagram notes added.",
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _diagramService.Update(diagram);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.Update(It.IsAny<Diagram>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void DiagramService_Delete_CallDeleteDiagramAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var diagram = new Diagram
            {
                Id = 3,
            };

            #endregion

            #region Act

            _diagramService.Delete(diagram);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.Delete(It.IsAny<Diagram>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }



        [TestMethod]
        public void DiagramService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _diagramService.All();

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DiagramService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _diagramService.GetById(1);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }



        [TestMethod]
        public void DiagramService_Diagrams_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _diagramService.Diagrams(1);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Diagram, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DiagramService_LevelDiagrams_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _diagramService.LevelDiagrams(0, 1);

            #endregion

            #region Assert

            _mockDiagramRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Diagram, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DiagramService_Diagrams_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var diagrams = _diagramService.Diagrams(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(diagrams, typeof(IQueryable<Diagram>));

            #endregion
        }

        [TestMethod]
        public void DiagramService_LevelDiagrams_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var diagrams = _diagramService.LevelDiagrams(0, 1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(diagrams, typeof(IQueryable<Diagram>));

            #endregion
        }

        [TestMethod]
        public void DiagramService_Diagrams_ReturnsThreeDiagramForTheCustomer()
        {
            #region Arrange

            #endregion

            #region Act

            var diagrams = _diagramService.Diagrams(1);

            #endregion

            #region Assert

            Assert.IsNotNull(diagrams);
            Assert.AreEqual(3, diagrams.Count());

            #endregion
        }

        [TestMethod]
        public void DiagramService_Diagrams_ReturnsOneDiagramForTheCustomerAtLevelOne()
        {
            #region Arrange

            #endregion

            #region Act

            var diagrams = _diagramService.LevelDiagrams(1, 1);

            #endregion

            #region Assert

            Assert.IsNotNull(diagrams);
            Assert.AreEqual(1, diagrams.Count());

            #endregion
        }

        [TestMethod]
        public void DiagramService_GetByCustomerId_CustomerDoesNotExist_NoDataReturned()
        {
            var result = _diagramService.GetByCustomerId(666);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void DiagramService_GetByCustomerId_CustomerExists_DataReturned()
        {
            var result = _diagramService.GetByCustomerId(1);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void DiagramService_GetByCustomerAndId_CustomerExists_DataReturned()
        {
            var result = _diagramService.GetByCustomerAndId(1, 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void DiagramService_GetByCustomerAndId_CustomerNotExists_DataReturned()
        {
            var result = _diagramService.GetByCustomerAndId(1111, 1);
            Assert.IsNull(result);
        }
    }
}