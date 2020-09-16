using System.Net.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace purr2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Create http client
            var client = new HttpClient();
            
            try
            {
                // Get all devices exposed by the maker api
                string devicesQueryResponseBody = await client.GetStringAsync("http://192.168.1.115/apps/api/2/devices?access_token=f6bb25f0-65f9-4413-ac03-6ed43628b5b3");
                var devices = JsonConvert.DeserializeObject<List<Device>>(devicesQueryResponseBody);

                /* foreach (var device in devices)
                {
                    this.logger.LogInformation(device.ToString());
                } */

                var device = devices.FirstOrDefault(d => d.Id == 2);
                IoHelper.PrintProperties(device, this.logger);
                string deviceEventsQueryResponse = await client.GetStringAsync($"http://192.168.1.115/apps/api/2/devices/{device?.Id}/events?access_token=f6bb25f0-65f9-4413-ac03-6ed43628b5b3");
                var deviceEvents = JsonConvert.DeserializeObject<List<DeviceEvent>>(deviceEventsQueryResponse);

                foreach (var deviceEvent in deviceEvents)
                {
                    IoHelper.PrintProperties(deviceEvent, this.logger);
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Unable to deserialize object.");
            }

            /* while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            } */
        }

        public static class IoHelper
        {
            /// <summary>
            /// Prints all properties of the target object using the provided logger.
            /// </summary>
            /// <param name="target">The target object.</param>
            /// <param name="logger">The logger for printing the properties.</param>
            public static void PrintProperties(object target, ILogger logger)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target))
                {
                    var name = descriptor.Name;
                    var value = descriptor.GetValue(target);
                    logger.LogInformation(string.Concat(name, "=", value));
                }

                logger.LogInformation("\n");
            }
        }

        

        public class Device
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Label { get; set; }
        }

        public class DeviceEvent
        {
            public int Id { get; set; }
            public string Label { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public DateTime Date { get; set; }
            public string Unit { get; set; }
            public string IsStateChange { get; set; }
            public string Source { get; set; }
        }
    }
}
