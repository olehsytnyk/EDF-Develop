using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Scraper.Shared;
using System.Web;
using HtmlAgilityPack;
using TestModule1;
using Databox.Libs.TestModule1;

namespace WheelsScraper
{
    public class TestModule1 : BaseScraper
	{
		public TestModule1()
		{
			Name = "Zieglers";
			Url = "https://www.zieglers.com/";
			PageRetriever.Referer = Url;
			WareInfoList = new List<ExtWareInfo>();
			Wares.Clear();
			BrandItemType = 2;

			SpecialSettings = new ExtSettings();
		}

		private ExtSettings extSett
		{
			get
			{
				return (ExtSettings)Settings.SpecialSettings;
			}
		}

		public override Type[] GetTypesForXmlSerialization()
		{
			return new Type[] { typeof(ExtSettings) };
		}

        public override System.Windows.Forms.Control SettingsTab
        {
            get
            {
                var frm = new ucExtSettings();
                frm.Sett = Settings;
                return frm;
            }
        }

        public override WareInfo WareInfoType
		{
			get
			{
				return new ExtWareInfo();
			}
		}

		protected override bool Login()
		{
			return true;
		}

		protected override void RealStartProcess()
		{
			lstProcessQueue.Add(new ProcessQueueItem { URL = Url, ItemType = 1 });
			StartOrPushPropertiesThread();
		}

		protected void ProcessBrandsListPage(ProcessQueueItem pqi)
		{
			if (cancel)
				return;
			AddWareInfo(new ExtWareInfo { Brand = "Test Brand", Name = "Test product Name" });

			var html = PageRetriever.ReadFromServer(extSett.CategoryToScrap);
			var doc = CreateDoc(html);

			pqi.Processed = true;
			var product = doc.DocumentNode.SelectNodes("//*[@id='productGrid']/li");
			if (product == null)
				return;
			foreach (var prod in product)
			{
				var name = prod.SelectSingleNode("//a[@href]").InnerTextOrNull();
				if (string.IsNullOrEmpty(name))
					continue;
				var url = prod.SelectSingleNode("//a").AttributeOrNull("href");
				var partTit = prod.SelectSingleNode(".//div[@class='card-title'][1]").InnerTextOrNull();

				var wi = new ExtWareInfo { Name = name, URL = url, PartTitle = partTit };
				AddWareInfo(wi);

				lock (this)
					lstProcessQueue.Add(new ProcessQueueItem { ItemType = 10, Item = wi, URL = url });
			}

			OnItemLoaded(null);


			pqi.Processed = true;
			MessagePrinter.PrintMessage("Brands list processed");
			StartOrPushPropertiesThread();
		}

		private void ProcessProductPage(ProcessQueueItem pqi)
		{
			var wi = (ExtWareInfo)pqi.Item;
			MessagePrinter.PrintMessage("Processing: " + wi.Name);
			var html = PageRetriever.ReadFromServer(pqi.URL);

			var doc = CreateDoc(html);
			pqi.Processed = true;

			wi.Description = doc.DocumentNode.SelectSingleNode("//div[class='tab-content']/div").InnerTextOrNull();
			wi.Msrp = ParsePrice(doc.DocumentNode.SelectSingleNode("//*[contains(@id, 'price')]").InnerTextOrNull());
			wi.Sku = doc.DocumentNode.SelectSingleNode("//div[contains(@id, 'productView-info-name')]").InnerTextOrNull();
			wi.Weight = ParseDouble(doc.DocumentNode.SelectSingleNode("//div[contains(@id, 'productView-info-value')]").InnerTextOrNull());



			MessagePrinter.PrintMessage("Product processed: " + wi.Name);
		}

		protected override Action<ProcessQueueItem> GetItemProcessor(ProcessQueueItem item)
		{
			Action<ProcessQueueItem> act;
			if (item.ItemType == 1)
				act = ProcessBrandsListPage;
			else if (item.ItemType == 1)
				act = ProcessProductPage;
			else act = null;

			return act;
		}
    }
}
