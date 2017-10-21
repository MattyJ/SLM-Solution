using System;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.Aspose.Spreadsheets
{
    [Serializable]
    public class MissingValueAttributeException<T, TA> : Exception
    {
        public MissingValueAttributeException()
        {
            var typeName = typeof(T).Name;
            var attributeTypeName = typeof(TA).Name;
            Message = String.Format("Type '{0}' does not have any properties decorated with the '{1}'.  Unable to continue", typeName, attributeTypeName);
        }

        public new String Message { get; private set; }

        // This method is required only to keep static code analysis happy. We're not serialising, not we're not interested in testing this bit.
        [ExcludeFromCodeCoverage]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}