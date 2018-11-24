using System;

namespace IoCLiteContainer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectionRequiredAttribute : Attribute
    {
    }
}
