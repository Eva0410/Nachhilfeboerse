﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     //
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoginAuthentication
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "0.4.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://svs.htl-leonding.ac.at/svsauthentication/", ConfigurationName="LoginAuthentication.SVSAuthenticationSoap")]
    public interface SVSAuthenticationSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetLehrer", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerAsync(string jahr);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetLehrerVonSchueler", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerVonSchuelerAsync(string matrikelnummer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetSprechstundeVonLehrer", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetSprechstundeVonLehrerAsync(int lehrerid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetLehrerDaten", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerDatenAsync(int lehrerid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetDatenVonSchueler", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetDatenVonSchuelerAsync(string matrikelnummer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/GetLehrerID", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> GetLehrerIDAsync(string benutzername, string kennwort);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/CheckLdapSchuelerLoginElektronik", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> CheckLdapSchuelerLoginElektronikAsync(string matrikelnummer, string kennwort);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://svs.htl-leonding.ac.at/svsauthentication/CheckLdapSchuelerLoginEdvo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> CheckLdapSchuelerLoginEdvoAsync(string matrikelnummer, string kennwort);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "0.4.0.0")]
    public interface SVSAuthenticationSoapChannel : LoginAuthentication.SVSAuthenticationSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "0.4.0.0")]
    public partial class SVSAuthenticationSoapClient : System.ServiceModel.ClientBase<LoginAuthentication.SVSAuthenticationSoap>, LoginAuthentication.SVSAuthenticationSoap
    {
        
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public SVSAuthenticationSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(SVSAuthenticationSoapClient.GetBindingForEndpoint(endpointConfiguration), SVSAuthenticationSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SVSAuthenticationSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(SVSAuthenticationSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SVSAuthenticationSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(SVSAuthenticationSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SVSAuthenticationSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerAsync(string jahr)
        {
            return base.Channel.GetLehrerAsync(jahr);
        }
        
        public System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerVonSchuelerAsync(string matrikelnummer)
        {
            return base.Channel.GetLehrerVonSchuelerAsync(matrikelnummer);
        }
        
        public System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetSprechstundeVonLehrerAsync(int lehrerid)
        {
            return base.Channel.GetSprechstundeVonLehrerAsync(lehrerid);
        }
        
        public System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetLehrerDatenAsync(int lehrerid)
        {
            return base.Channel.GetLehrerDatenAsync(lehrerid);
        }
        
        public System.Threading.Tasks.Task<LoginAuthentication.ArrayOfXElement> GetDatenVonSchuelerAsync(string matrikelnummer)
        {
            return base.Channel.GetDatenVonSchuelerAsync(matrikelnummer);
        }
        
        public System.Threading.Tasks.Task<int> GetLehrerIDAsync(string benutzername, string kennwort)
        {
            return base.Channel.GetLehrerIDAsync(benutzername, kennwort);
        }
        
        public System.Threading.Tasks.Task<int> CheckLdapSchuelerLoginElektronikAsync(string matrikelnummer, string kennwort)
        {
            return base.Channel.CheckLdapSchuelerLoginElektronikAsync(matrikelnummer, kennwort);
        }
        
        public System.Threading.Tasks.Task<int> CheckLdapSchuelerLoginEdvoAsync(string matrikelnummer, string kennwort)
        {
            return base.Channel.CheckLdapSchuelerLoginEdvoAsync(matrikelnummer, kennwort);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SVSAuthenticationSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.SVSAuthenticationSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SVSAuthenticationSoap))
            {
                return new System.ServiceModel.EndpointAddress("https://svs.htl-leonding.ac.at/authentication/authentication.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.SVSAuthenticationSoap12))
            {
                return new System.ServiceModel.EndpointAddress("https://svs.htl-leonding.ac.at/authentication/authentication.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            SVSAuthenticationSoap,
            
            SVSAuthenticationSoap12,
        }
    }
    
    [System.Xml.Serialization.XmlSchemaProviderAttribute(null, IsAny=true)]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "0.4.0.0")]
    public partial class ArrayOfXElement : object, System.Xml.Serialization.IXmlSerializable
    {
        
        private System.Collections.Generic.List<System.Xml.Linq.XElement> nodesList = new System.Collections.Generic.List<System.Xml.Linq.XElement>();
        
        public ArrayOfXElement()
        {
        }
        
        public virtual System.Collections.Generic.List<System.Xml.Linq.XElement> Nodes
        {
            get
            {
                return this.nodesList;
            }
        }
        
        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            System.Collections.Generic.IEnumerator<System.Xml.Linq.XElement> e = nodesList.GetEnumerator();
            for (
            ; e.MoveNext(); 
            )
            {
                ((System.Xml.Serialization.IXmlSerializable)(e.Current)).WriteXml(writer);
            }
        }
        
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            for (
            ; (reader.NodeType != System.Xml.XmlNodeType.EndElement); 
            )
            {
                if ((reader.NodeType == System.Xml.XmlNodeType.Element))
                {
                    System.Xml.Linq.XElement elem = new System.Xml.Linq.XElement("default");
                    ((System.Xml.Serialization.IXmlSerializable)(elem)).ReadXml(reader);
                    Nodes.Add(elem);
                }
                else
                {
                    reader.Skip();
                }
            }
        }
    }
}
