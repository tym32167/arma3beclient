using System;

namespace Arma3BEClient.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ShowInUiAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class EnableCopyAttribute : Attribute
    {
    }
}