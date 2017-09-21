using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EddieFxCtrl.Classes
{
    public class EfcSoftPatch : INotifyPropertyChanged
    {
        protected UInt16 _In;
        protected UInt16 _Out;
        protected Boolean _Enabled = true;

        public UInt16 In {
            get { return _In; }
            set
            {
                _In = value;
                PropertyChanged(this, new PropertyChangedEventArgs("In"));
            }
        }
        public UInt16 Out
        {
            get { return _Out; }
            set
            {
                _Out = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Out"));
            }
        }

        public Boolean Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        
        public XElement ToXML(XNamespace ns)
        {
            XElement elm = new XElement(ns + "softpatch");

            elm.SetAttributeValue("in", _In);
            elm.SetAttributeValue("out", _Out);
            elm.SetAttributeValue("enabled", _Enabled);

            return elm;
        }
        public static EfcSoftPatch FromXML(XElement elm, EfcShow show, EfcUniverse universe, XNamespace ns)
        {
            EfcSoftPatch patch = new EfcSoftPatch()
            {
                _In = Convert.ToUInt16(elm.Attribute("in").Value),
                _Out = Convert.ToUInt16(elm.Attribute("out").Value),
                _Enabled = Convert.ToBoolean(elm.Attribute("enabled").Value)
            };

            return patch;
        }
    }
}
