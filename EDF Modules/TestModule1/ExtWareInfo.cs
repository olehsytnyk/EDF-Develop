using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WheelsScraper;

namespace TestModule1
{
	public class ExtWareInfo : WareInfo
	{
		public string Category { get; set; }
		public double Msrp { get; set; }
        public string PartTitle { get; set; }
		public string Description { get; set; }
		public string Sku { get; set; }
		public double Weight { get; set; }
    }
}
