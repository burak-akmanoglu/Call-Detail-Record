using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{ 
        public class ClientData
        {
            [Key]
            public int ClientId { get; set; }
            public string TelephoneNumber { get; set; }
            public string TargetTelephoneNumber { get; set; }
            public string Date { get; set; }
            public string RingTime { get; set; }
            public string CallTime { get; set; }
            public string StartTime { get; set; }
            public string FinishTime { get; set; }
        }
    
}
