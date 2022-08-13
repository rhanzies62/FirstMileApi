using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class GatewayResponse<T>
    {
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
    public class Gateway
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "resource")]
        public string Resource { get; set; }

        [JsonProperty(PropertyName = "serial")]
        public string Serial { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "station")]
        public GatewayStation Station { get; set; }

    }

    public class GatewayStation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "resource")]
        public string Resource { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "callsign")]
        public string CallSign { get; set; }
    }

    public class GatewayUsage
    {
        public GatewayUsage()
        {

        }
        public GatewayUsage(bool isError)
        {
            HasError = isError;
        }

        [JsonProperty(PropertyName = "total_usage")]
        public int TotalUsage { get; set; }

        [JsonProperty(PropertyName = "cell_usage")]
        public int CellUsage { get; set; }

        [JsonProperty(PropertyName = "start_time")]
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "finish_time")]
        public DateTime FinishTime { get; set; }

        public int OtherUsage { get; set; }
        public bool HasError { get; set; } = false;
    }

    public class SourceUsage
    {
        public SourceUsage()
        {

        }
        public SourceUsage(bool isError)
        {
            HasError = isError;
        }
        public bool HasError { get; set; } = false;
        [JsonProperty(PropertyName = "totalUsage")]
        public int TotalUsage { get; set; }

        [JsonProperty(PropertyName = "totalCellUsage")]
        public int TotalCellUsage { get; set; }

        [JsonProperty(PropertyName = "liveUsage")]
        public int LiveUsage { get; set; }

        [JsonProperty(PropertyName = "liveCellUsage")]
        public int LiveCellUsage { get; set; }

        [JsonProperty(PropertyName = "snfUsage")]
        public int SnfUsage { get; set; }

        [JsonProperty(PropertyName = "snfCellUsage")]
        public int SnfCellUsage { get; set; }

        [JsonProperty(PropertyName = "fileUsage")]
        public int FileUsage { get; set; }

        [JsonProperty(PropertyName = "fileCellUsage")]
        public int FileCellUsage { get; set; }

        [JsonProperty(PropertyName = "hotspotUsage")]
        public int HotspotUsage { get; set; }

        [JsonProperty(PropertyName = "hotspotCellUsage")]
        public int HotspotCellUsage { get; set; }

        [JsonProperty(PropertyName = "remoteControlUsage")]
        public int RemoteControlUsage { get; set; }

        [JsonProperty(PropertyName = "remoteControlCellUsage")]
        public int RemoteControlCellUsage { get; set; }

        [JsonProperty(PropertyName = "ifbUsage")]
        public int IfbUsage { get; set; }

        [JsonProperty(PropertyName = "ifbCellUsage")]
        public int IfbCellUsage { get; set; }

        [JsonProperty(PropertyName = "returnVideoUsage")]
        public int ReturnVideoUsage { get; set; }

        [JsonProperty(PropertyName = "returnVideoCellUsage")]
        public int ReturnVideoCellUsage { get; set; }

        [JsonProperty(PropertyName = "startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "finishTime")]
        public DateTime FinishTime { get; set; }
    }

    public class LocationData
    {
        public IEnumerable<Location> Locations { get; set; }
        public Bounds Bounds { get; set; }
    }

    public class Location
    {
        [JsonProperty(PropertyName = "latitude")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public float Longitude { get; set; }

        [JsonProperty(PropertyName = "accuracy")]
        public double Accuracy { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime TimeStamp { get; set; }

        public EquipmentModel Equipment { get; set; }
    }

    public class Bounds
    {
        [JsonProperty(PropertyName = "nw")]
        public BoundData NW { get; set; }

        [JsonProperty(PropertyName = "se")]
        public BoundData SE { get; set; }
    }

    public class BoundData
    {
        [JsonProperty(PropertyName = "lat")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Longitude { get; set; }
    }
}
