using System;

namespace IoCLite.Container.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectionRequiredAttribute : Attribute
    {
    }
}
