using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Auth.Model
{

    public class BaseClientInfo
    {
        public string EquipmentID { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentMac { get; set; }
        public string EquipmentIP { get; set; }
        public string EquipmentPort { get; set; }
        public string EquipmentCPU { get; set; }
        public string EquipmentWifiMac { get; set; }
        public string EquipmentMemory { get; set; }
        public string ClientChangeFlag { get; set; }
        public string EquipmentCpuTypeName { get; set; }
        public string OsName { get; set; }
        public string InstitutionName { get; set; }
        public string VersionNo { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
    }
}
