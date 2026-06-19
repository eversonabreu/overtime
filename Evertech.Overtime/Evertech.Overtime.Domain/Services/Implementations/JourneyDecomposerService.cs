using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Enums;
using Evertech.Overtime.Domain.Helpers;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Domain.Services.Implementations;

internal sealed class JourneyDecomposerService(IHolidayService holidayService) : IJourneyDecomposerService
{
    private const int MaxFirstTierMinutes = 120;
    private const int NightStartHour = 22;
    private const int NightEndHour = 5;
    private const int NightBonusHour = 30;

    public async Task<IReadOnlyList<JourneyEntry>> DecomposeAsync(
        Guid journeyId,
        DateTimeOffset checkIn,
        DateTimeOffset checkOut,
        decimal hourlyRate,
        bool compensatoryTimeEnabled,
        Guid municipalityId,
        CancellationToken cancellationToken = default)
    {
        var entries = new List<JourneyEntry>();
        var cursor = checkIn;
        var accumulatedMinutes = 0;

        while (cursor < checkOut)
        {
            var breakpoint = await GetNextBreakpointAsync(cursor, checkOut, accumulatedMinutes, municipalityId, cancellationToken);
            var blockMinutes = (int)(breakpoint - cursor).TotalMinutes;

            var isNight = IsNightPeriod(cursor.Hour);
            var isFirstTier = accumulatedMinutes < MaxFirstTierMinutes;
            var dayType = await GetDayTypeAsync(cursor, municipalityId, cancellationToken);

            var baseRate = GetBaseRate(dayType, isFirstTier);
            var finalRate = baseRate + (isNight ? NightBonusHour : 0);

            var goesToCompensatory = IsEligibleForCompensatory(
                compensatoryTimeEnabled,
                dayType,
                cursor.Hour,
                isFirstTier);

            var entryType = goesToCompensatory ? EntryType.Compensatory : EntryType.Overtime;
            var grossAmount = entryType == EntryType.Overtime
                ? CalculateGrossAmount(hourlyRate, finalRate, blockMinutes)
                : 0m;

            entries.Add(new JourneyEntry
            {
                Id = Guid.NewGuid(),
                JourneyId = journeyId,
                CheckIn = cursor,
                CheckOut = breakpoint,
                Minutes = blockMinutes,
                BaseRate = (short)finalRate,
                GrossAmount = grossAmount,
                Type = entryType,
                CreatedAt = DateTimeOffset.UtcNow
            });

            accumulatedMinutes += blockMinutes;
            cursor = breakpoint;
        }

        return entries;
    }

    private async Task<DateTimeOffset> GetNextBreakpointAsync(
        DateTimeOffset cursor,
        DateTimeOffset checkOut,
        int accumulatedMinutes,
        Guid municipalityId,
        CancellationToken cancellationToken)
    {
        var candidates = new List<DateTimeOffset> { checkOut };

        var midnight = cursor.Date.AddDays(1);
        if (midnight > cursor && midnight < checkOut)
            candidates.Add(midnight);

        var nightStart = cursor.Date.AddHours(NightStartHour);
        if (nightStart > cursor && nightStart < checkOut)
            candidates.Add(nightStart);

        var nightEnd = cursor.Date.AddHours(NightEndHour).AddMinutes(1);
        if (nightEnd > cursor && nightEnd < checkOut)
            candidates.Add(nightEnd);

        var dayType = await GetDayTypeAsync(cursor, municipalityId, cancellationToken);
        if (dayType == DayType.Weekday && accumulatedMinutes < MaxFirstTierMinutes)
        {
            var minutesUntilTierChange = MaxFirstTierMinutes - accumulatedMinutes;
            var tierChange = cursor.AddMinutes(minutesUntilTierChange);
            if (tierChange > cursor && tierChange < checkOut)
                candidates.Add(tierChange);
        }

        return candidates.Min();
    }

    private static bool IsNightPeriod(int hour) =>
        hour >= NightStartHour || hour <= NightEndHour;

    private async Task<DayType> GetDayTypeAsync(DateTimeOffset moment, Guid municipalityId, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(moment.DateTime);

        var isHoliday = await holidayService.IsHolidayAsync(date, municipalityId, cancellationToken);
        if (isHoliday)
            return DayType.Holiday;

        return moment.DayOfWeek switch
        {
            DayOfWeek.Sunday => DayType.Sunday,
            DayOfWeek.Saturday => DayType.Saturday,
            _ => DayType.Weekday
        };
    }

    private static int GetBaseRate(DayType dayType, bool isFirstTier) => dayType switch
    {
        DayType.Holiday => 100,
        DayType.Sunday => 100,
        DayType.Saturday => 75,
        DayType.Weekday => isFirstTier ? 50 : 75,
        _ => throw new ArgumentOutOfRangeException(nameof(dayType))
    };

    private static bool IsEligibleForCompensatory(
        bool compensatoryTimeEnabled,
        DayType dayType,
        int hour,
        bool isFirstTier)
    {
        if (!compensatoryTimeEnabled)
            return false;

        if (dayType != DayType.Weekday)
            return false;

        var isDaytime = hour is > NightEndHour and < NightStartHour;
        if (!isDaytime)
            return false;

        return isFirstTier;
    }

    private static decimal CalculateGrossAmount(decimal hourlyRate, int ratePercentage, int minutes)
    {
        var rateDecimal = 1 + (ratePercentage / 100m);
        var value = (hourlyRate / 60m) * rateDecimal * minutes;
        return RoundingHelper.RoundMonetary(value);
    }
}