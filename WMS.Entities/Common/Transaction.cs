using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WMS.Entities.Common
{
    public class Transaction
    {
        public TypeTransaction type { get; set; }
        public string message { get; set; }
    }

    public enum TypeTransaction
    {
        OK, ERR
    }   
}
