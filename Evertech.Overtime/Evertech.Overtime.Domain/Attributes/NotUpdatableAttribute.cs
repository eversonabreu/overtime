namespace Evertech.Overtime.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class NotUpdatableAttribute : Attribute;