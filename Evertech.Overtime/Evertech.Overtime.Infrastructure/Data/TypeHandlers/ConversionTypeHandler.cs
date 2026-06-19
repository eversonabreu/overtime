using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Infrastructure.Data.TypeHandlers;

[ExcludeFromCodeCoverage]
internal sealed class ConversionTypeHandler : SqlMapper.TypeHandler<ConversionType>
{
    public override void SetValue(IDbDataParameter parameter, ConversionType value) =>
        parameter.Value = value switch
        {
            ConversionType.Remunerated => "REMUNERATED",
            ConversionType.TimeOff => "TIME_OFF",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    public override ConversionType Parse(object value) =>
        value.ToString() switch
        {
            "REMUNERATED" => ConversionType.Remunerated,
            "TIME_OFF" => ConversionType.TimeOff,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
}