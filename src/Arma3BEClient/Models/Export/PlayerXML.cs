using System;
using System.Xml.Serialization;

namespace Arma3BEClient.Models.Export
{
    public class PlayerXML
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Guid { get; set; }

        [XmlAttribute]
        public string LastIP { get; set; }

        [XmlAttribute]
        public string Comment { get; set; }

        [XmlAttribute]
        public DateTime LastSeen { get; set; }
    }
}