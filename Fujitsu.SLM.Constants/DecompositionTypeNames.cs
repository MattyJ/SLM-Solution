using Fujitsu.SLM.Enumerations;

namespace Fujitsu.SLM.Constants
{
    public struct DecompositionTypeNames
    {
        public static readonly string Activity;
        public static readonly string Component;
        public static readonly string Customer;
        public static readonly string Desk;
        public static readonly string Domain;
        public static readonly string EmptyForLayout;
        public static readonly string EmptyForLayoutRoot;
        public static readonly string LineForDummyChildComponent;
        public static readonly string Function;
        public static readonly string InputTypeName;
        public static readonly string InputTypeNumber;
        public static readonly string OperationalProcess;
        public static readonly string Resolver;
        public static readonly string ServiceDeliveryOrganisation;
        public static readonly string ServiceDeliveryUnit;
        public static readonly string CustomerServices;
        public static readonly string ResolverGroupOperationalProcess;
        public static readonly string ResolverGroupOperationalProcessSelected;

        static DecompositionTypeNames()
        {
            Activity = DecompositionType.Activity.ToString();
            Component = DecompositionType.Component.ToString();
            Customer = DecompositionType.Customer.ToString();
            Desk = DecompositionType.Desk.ToString();
            Domain = DecompositionType.Domain.ToString();
            EmptyForLayout = DecompositionType.EmptyForLayout.ToString();
            EmptyForLayoutRoot = DecompositionType.EmptyForLayoutRoot.ToString();
            LineForDummyChildComponent = DecompositionType.LineForDummyChildComponent.ToString();
            Function = DecompositionType.Function.ToString();
            InputTypeName = DecompositionType.InputTypeName.ToString();
            InputTypeNumber = DecompositionType.InputTypeNumber.ToString();
            OperationalProcess = DecompositionType.OperationalProcess.ToString();
            Resolver = DecompositionType.Resolver.ToString();
            ServiceDeliveryOrganisation = DecompositionType.ServiceDeliveryOrganisation.ToString();
            ServiceDeliveryUnit = DecompositionType.ServiceDeliveryUnit.ToString();
            CustomerServices = DecompositionType.CustomerServices.ToString();
            ResolverGroupOperationalProcess = DecompositionType.ResolverGroupOperationalProcess.ToString();
            ResolverGroupOperationalProcessSelected = DecompositionType.ResolverGroupOperationalProcessSelected.ToString();
        }
    }
}