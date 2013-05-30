
namespace THOK.Wms.DbModel
{
    public class SRM
    {
        public int ID { get; set; }
        public string SRMName { get; set; }
        public string Description { get; set; }
        public string OPCServiceName { get; set; }
        public string GetRequest { get; set; }
        public string GetAllow { get; set; }
        public string GetComplete { get; set; }
        public string PutRequest { get; set; }
        public string PutAllow { get; set; }
        public string PutComplete { get; set; }
        public string State { get; set; }
    }
}

