using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Statistics
{
    [DataContract]
    public class DataPoint
    {
        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "label")]
        public string Label { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.
        //[DataMember(Name = "x")]
        //public Nullable<double> X { get; set; }
    }
}
