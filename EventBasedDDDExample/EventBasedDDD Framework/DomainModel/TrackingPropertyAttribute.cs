using System;

namespace EventBasedDDD
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TrackingPropertyAttribute : Attribute
    {
    }
}
