namespace Alert.Service.Modules.Violation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.Contracts.Violation;
    using Domain.Entities;
    using Domain.Repositories.Interfaces;
    using Entities;
    using Infrastructure.Bootstrapper.Interfaces;
    using Interfaces;
    using Newtonsoft.Json;

    public class ViolationCalculationService : IViolationCalculationService, ITransientService
    {
        private readonly IViolationRepository _violationRepository;

        public ViolationCalculationService(IViolationRepository violationRepository)
        {
            _violationRepository = violationRepository;
        }


        public async Task<DrivingCalculationResult> CalculateDrivingLimit(string transportationListSasUri, Guid userId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(transportationListSasUri);
                var json = await response.Content.ReadAsStringAsync();
                var transportationList = JsonConvert.DeserializeObject<List<TransportationResult>>(json);
                var orderedTransportationList = transportationList.OrderBy(result =>
                    result.CapturedLocations.Min(location => location.CapturedDateTimeUtc));
                var currentDayTime =
                    DateTime.UtcNow.AddDays(-1).Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;

                double nineHourSecondLimit = 32400; // 9 hours
                double fourAndHalfHourSecondLimit = 16200; // 9 hours
                double weekSecondLimit = 201600; // 56 hours
                double breakTimeSeconds = 2700; // 45 minutes

                double nineHourLimitCounter = 0;
                double fourAndHalfHourLimitCounter = 0;
                double weekLimitCounter = 0;
                double currentDayLastTransportationTime = 0;
                double? currentDayFirstTransportationTime = null;
                double? currentWeekFirstTransportationTime = null;
                double restTimeLeftTillNextTransportationCanBeStarted = 0;
                foreach (var transportation in orderedTransportationList)
                {
                    var periodStart = transportation.CapturedLocations.Min(o => o.CapturedDateTimeUtc);
                    var periodEnd = transportation.CapturedLocations.Max(o => o.CapturedDateTimeUtc);
                    var drivenTime = periodEnd - periodStart;
                    weekLimitCounter += drivenTime;

                    if (currentWeekFirstTransportationTime == null)
                    {
                        currentWeekFirstTransportationTime = periodStart;
                    }

                    if (periodEnd > currentDayTime)
                    {
                        if (currentDayFirstTransportationTime == null)
                        {
                            currentDayFirstTransportationTime = periodStart;
                        }

                        if (periodStart - currentDayLastTransportationTime < breakTimeSeconds)
                        {
                            fourAndHalfHourLimitCounter += drivenTime;
                        }
                        else
                        {
                            fourAndHalfHourLimitCounter = drivenTime;
                        }

                        if (fourAndHalfHourLimitCounter > fourAndHalfHourSecondLimit)
                        {
                            // 4.5 hour violation
                            if (!await _violationRepository.CheckIfViolationCreatedPastHour(userId,
                                ViolationType.HalfDay))
                            {
                                var violation = new Violation
                                {
                                    Type = ViolationType.HalfDay,
                                    CreatedAt = (float) DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1699))
                                        .TotalSeconds
                                };
                                await _violationRepository.SaveViolation(userId, violation);
                            }

                            restTimeLeftTillNextTransportationCanBeStarted =
                                currentDayFirstTransportationTime.Value + breakTimeSeconds -
                                DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
                        }

                        nineHourLimitCounter += drivenTime;
                        if (nineHourLimitCounter > nineHourSecondLimit)
                        {
                            // 9 hour violation
                            if (!await _violationRepository.CheckIfViolationCreatedPastHour(userId, ViolationType.Day))
                            {
                                var violation = new Violation
                                {
                                    Type = ViolationType.Day,
                                    CreatedAt = (float) DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1699))
                                        .TotalSeconds
                                };
                                await _violationRepository.SaveViolation(userId, violation);
                            }

                            restTimeLeftTillNextTransportationCanBeStarted =
                                periodEnd + 86400 -
                                DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
                        }

                        currentDayLastTransportationTime = periodEnd;
                    }

                    if (weekLimitCounter > weekSecondLimit)
                    {
                        // week limit violation
                        if (!await _violationRepository.CheckIfViolationCreatedPastHour(userId, ViolationType.Week))
                        {
                            var violation = new Violation
                            {
                                Type = ViolationType.Week,
                                CreatedAt = (float) DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1699))
                                    .TotalSeconds
                            };
                            await _violationRepository.SaveViolation(userId, violation);
                        }

                        restTimeLeftTillNextTransportationCanBeStarted =
                            currentWeekFirstTransportationTime.Value + 604800 -
                            DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
                    }
                }

                var nineHourLimitSecondsLeft = nineHourSecondLimit - nineHourLimitCounter;
                var fourHourLimitSecondsLeft = fourAndHalfHourSecondLimit - fourAndHalfHourLimitCounter;
                var drivingTimeLeftTillNextRestRequired = nineHourLimitSecondsLeft > fourHourLimitSecondsLeft
                    ? fourHourLimitSecondsLeft
                    : nineHourLimitSecondsLeft;
                if (drivingTimeLeftTillNextRestRequired < 0)
                {
                    drivingTimeLeftTillNextRestRequired = 0;
                }

                if (restTimeLeftTillNextTransportationCanBeStarted < 0)
                {
                    restTimeLeftTillNextTransportationCanBeStarted = 0;
                }

                return new DrivingCalculationResult
                {
                    DrivingTimePastWeek = new TimeSpan(0, 0, 0, (int) weekLimitCounter),
                    DrivingTimePastDay = new TimeSpan(0, 0, 0, (int) nineHourLimitCounter),
                    DrivingTimeLeftTillNextRestRequired =
                        new TimeSpan(0, 0, 0, (int) drivingTimeLeftTillNextRestRequired),
                    RestTimeLeftTillNextTransportationCanBeStarted = new TimeSpan(0, 0, 0,
                        (int) (int?) restTimeLeftTillNextTransportationCanBeStarted)
                };
            }
        }
    }
}