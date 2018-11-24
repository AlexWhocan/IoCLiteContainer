using System;

namespace Simple_IoC_Container.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectionRequiredAttribute : Attribute
    {
    }
}
