using firstmile.data;
using firstmile.domain;
using firstmile.domain.Model;
using firstmile.services.DejeroApi;
using firstmile.services.Interface;
using firstmile.services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.wj.console
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessUsageData().Wait();
        }

        static async Task ProcessUsageData()
        {
            var unitOfWork = new UnitOfWork(new FirstMileEntities());
            var equipmentService = new EquipmentService(unitOfWork);
            var usageService = new UsageService(unitOfWork);
            var equipments = await equipmentService.ListGateways();
            var gatewayAPI = new GatewayAPI();
            Console.WriteLine($"===Gateways Found: {equipments.Count()}===");
            foreach (var equipment in equipments.Where(i => i.GatewayId != 2662387 &&
                                                            i.GatewayId != 2662649 &&
                                                            i.GatewayId != 2771952 &&
                                                            i.GatewayId != 3220486 &&
                                                            i.GatewayId != 2829444 &&
                                                            i.GatewayId != 2980839))
            {
                var usage = await usageService.GetLastUsageDataOfGateway(equipment.GatewayId.ToString());
                var lastDate = usage == null ? DateTime.Parse("6/1/2020 00:00:00") : usage.DateFrom;
                Console.WriteLine($"===Gateway: {equipment.Name} - Last Pulled: {lastDate}");
                for (int day = 0; day <= 30; day++)
                {
                    var usages = new List<UsageModel>();
                    for (int hour = 0; hour <= 23; hour++)
                    {
                        var from = lastDate.AddDays(day).AddHours(hour);
                        var to = lastDate.AddDays(day).AddHours(hour + 1);
                        Console.WriteLine($"Downloading usage for {from} - {to}");
                        var usageResult = new GatewayUsage(true);
                        while (usageResult.HasError)
                        {
                            usageResult = await gatewayAPI.GetGatewayUsage(equipment.GatewayId.Value, from, to);
                            if (!usageResult.HasError)
                            {
                                usages.Add(new UsageModel
                                {
                                    CellUsage = usageResult.CellUsage,
                                    DateFrom = from,
                                    DateTo = to,
                                    GatewayId = equipment.GatewayId.ToString(),
                                    TotalUsage = usageResult.TotalUsage,
                                    OtherUsage = usageResult.TotalUsage - usageResult.CellUsage
                                });
                            }
                            else
                            {
                                Console.WriteLine("Retring to get usage.");
                            }
                        }
                        

                    }
                    var result = false;
                    while (!result)
                    {
                        var _result = await usageService.SaveGatewayUsages(usages);
                        result = _result.IsSuccess;
                        Console.WriteLine($"Result: {result}");
                        if (!result)
                        {
                            Console.WriteLine($"Retrying to save data..");
                        }
                    }
                }
            }
        }
    }
}
