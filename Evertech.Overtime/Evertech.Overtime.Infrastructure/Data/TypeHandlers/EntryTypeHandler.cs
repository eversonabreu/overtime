using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Infrastructure.Data.TypeHandlers;

[ExcludeFromCodeCoverage]
internal sealed class EntryTypeHandler : SqlMapper.TypeHandler<EntryType>
{
    public override void SetValue(IDbDataParameter parameter, EntryType value) =>
        parameter.Value = value switch
        {
            EntryType.Overtime => "OVERTIME",
            EntryType.Compensatory => "COMPENSATORY",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    public override EntryType Parse(object value) =>
        value.ToString() switch
        {
            "OVERTIME" => EntryType.Overtime,
            "COMPENSATORY" => EntryType.Compensatory,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
}