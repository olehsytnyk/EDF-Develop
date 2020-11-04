using System;
using System.Collections.Generic;
using System.Linq;
using Databox.Libs.TestModule1;
using DevExpress.XtraEditors;
using WheelsScraper;

namespace TestModule1
{
    public partial class ucExtSettings : XtraUserControl
    {
        public ExtSettings ExtSett
        {
            get
            {
                return (ExtSettings)Sett.SpecialSettings;
            }
        }

        ScraperSettings _sett;
        public ScraperSettings Sett
        {
            get { return _sett; }
            set { _sett = value; if (_sett != null) RefreshBindings(); }
        }
        public ucExtSettings()
        {
            InitializeComponent();
        }

        protected void RefreshBindings()
        {
            bsSett.DataSource = ExtSett;
        }

        private void bsSett_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
