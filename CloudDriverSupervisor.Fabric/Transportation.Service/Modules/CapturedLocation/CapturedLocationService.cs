namespace Transportation.Service.Modules.CapturedLocation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.Bootstrapper.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using Transportation.Interfaces;

    public class CapturedLocationService : ICapturedLocationService, ISingletonService
    {
        private const int MaxTransportationCountInTransaction = 200;

        private readonly ConcurrentDictionary<Guid, List<CapturedLocation>> _locationDictionary =
            new ConcurrentDictionary<Guid, List<CapturedLocation>>();

        private readonly ILoggingService _loggingService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        private readonly ITransportationRepository _transportationRepository;
        private readonly ITransportationService _transportationService;

        private bool _transactionInProgress;

        private Timer _transactionTimer;

        public CapturedLocationService(
            ITransportationRepository transportationRepository,
            ILoggingService loggingService,
            ITransportationService transportationService,
            IServiceBusCommunicationService serviceBusCommunicationService)
        {
            _transportationRepository = transportationRepository;
            _loggingService = loggingService;
            _transportationService = transportationService;
            _serviceBusCommunicationService = serviceBusCommunicationService;
        }

        public void ProcessCapturedLocations(Guid transportationId, List<CapturedLocation> locations)
        {
            while (_transactionInProgress)
            {
                Task.Delay(200);
            }

            _locationDictionary.AddOrUpdate(transportationId, guid => locations,
                (guid, list) =>
                {
                    list.AddRange(locations);
                    return list;
                });
        }

        public void StartTransactionCountdown()
        {
            if (_transactionTimer != null)
            {
                return;
            }

            _transactionTimer = new Timer(1000) { AutoReset = false };
            _transactionTimer.Elapsed += CallRepositoryOnElapsed;
            _transactionTimer.Start();
        }

        private async void CallRepositoryOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!_locationDictionary.Any())
            {
                _transactionTimer.Start();
                return;
            }

            try
            {
                _transactionInProgress = true;
                var locationDictionary = _locationDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);
                _locationDictionary.Clear();
                _transactionInProgress = false;
                var transactionDictionaries =
                    SplitLocationsInMultipleTransactionDictionaries(locationDictionary).ToList();
                foreach (var transactionDictionary in transactionDictionaries)
                {
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();

                    await _transportationRepository.SaveCapturedLocations(transactionDictionary);

                    stopwatch.Stop();
                    var count = transactionDictionary.Sum(pair => pair.Value.Count);
                    _loggingService.LogMessage(
                        $"Saved {count} captured locations in {stopwatch.ElapsedMilliseconds} ms");
                }

                var transportationListSasUri =
                    await _transportationService.GetTransportationListSasUri(transactionDictionaries);
                await SendSignalrProcessedLocationsMessage(transportationListSasUri);
            }
            catch (Exception e)
            {
                _loggingService.LogMessage(e.ToString());
                _transactionTimer.Start();
                return;
            }

            _transactionTimer.Start();
        }

        private IEnumerable<Dictionary<Guid, List<CapturedLocation>>> SplitLocationsInMultipleTransactionDictionaries(
            Dictionary<Guid, List<CapturedLocation>> locationDictionary)
        {
            var transactionDictionaries = new List<Dictionary<Guid, List<CapturedLocation>>>();
            var tempDictionary = new Dictionary<Guid, List<CapturedLocation>>();
            var tempAvailableStatementCount = MaxTransportationCountInTransaction;
            foreach (var transportation in locationDictionary)
            {
                if (tempAvailableStatementCount <= 0)
                {
                    SaveTransactionDictionary(transactionDictionaries, ref tempDictionary,
                        ref tempAvailableStatementCount);
                }

                HandleTransportationCapturedLocations(transactionDictionaries, ref tempDictionary,
                    ref tempAvailableStatementCount, transportation);
            }

            if (tempDictionary.Any())
            {
                SaveTransactionDictionary(transactionDictionaries, ref tempDictionary, ref tempAvailableStatementCount);
            }

            return transactionDictionaries;
        }

        private static void HandleTransportationCapturedLocations(
            List<Dictionary<Guid, List<CapturedLocation>>> transactionDictionaries,
            ref Dictionary<Guid, List<CapturedLocation>> tempDictionary,
            ref int tempAvailableStatementCount,
            KeyValuePair<Guid, List<CapturedLocation>> transportation)
        {
            while (true)
            {
                if (transportation.Value.Count <= tempAvailableStatementCount)
                {
                    tempDictionary.Add(transportation.Key, transportation.Value);
                    tempAvailableStatementCount -= transportation.Value.Count;
                }
                else if (transportation.Value.Count > tempAvailableStatementCount)
                {
                    var count = tempAvailableStatementCount;
                    var capturedLocations = transportation.Value.TakeWhile((location, i) => i < count).ToList();
                    transportation.Value.RemoveRange(0, count);
                    tempDictionary.Add(transportation.Key, capturedLocations);
                    SaveTransactionDictionary(transactionDictionaries, ref tempDictionary,
                        ref tempAvailableStatementCount);

                    continue;
                }

                break;
            }
        }

        private static void SaveTransactionDictionary(
            List<Dictionary<Guid, List<CapturedLocation>>> transactionDictionaries,
            ref Dictionary<Guid, List<CapturedLocation>> tempDictionary,
            ref int tempAvailableStatementCount)
        {
            transactionDictionaries.Add(tempDictionary);
            tempDictionary = new Dictionary<Guid, List<CapturedLocation>>();
            tempAvailableStatementCount = MaxTransportationCountInTransaction;
        }

        private async Task SendSignalrProcessedLocationsMessage(IEnumerable<string> sasUrilist)
        {
            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(sasUrilist),
                "Processed-Post-Captured-Locations-Queue");
        }
    }
}