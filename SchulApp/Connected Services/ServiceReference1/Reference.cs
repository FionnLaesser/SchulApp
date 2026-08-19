namespace ServiceReference1
{
    using System.Runtime.Serialization;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "8.0.0")]
    [DataContractAttribute(Name="SchuelerModel", Namespace="http://schemas.datacontract.org/2004/07/SchulAppSOAP.Models")]
    public partial class SchuelerModel : object
    {
        private string AuditUserNameField;
        private string AuditUserRoleField;
        private int KlasseIdField;
        private string NameField;
        private int SchuelerIdField;

        [DataMemberAttribute()]
        public string AuditUserName { get => AuditUserNameField; set => AuditUserNameField = value; }

        [DataMemberAttribute()]
        public string AuditUserRole { get => AuditUserRoleField; set => AuditUserRoleField = value; }

        [DataMemberAttribute()]
        public int KlasseId { get => KlasseIdField; set => KlasseIdField = value; }

        [DataMemberAttribute()]
        public string Name { get => NameField; set => NameField = value; }

        [DataMemberAttribute()]
        public int SchuelerId { get => SchuelerIdField; set => SchuelerIdField = value; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "8.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.ISchuelerService")]
    public interface ISchuelerService
    {
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/GetSchueler", ReplyAction="http://tempuri.org/ISchuelerService/GetSchuelerResponse")]
        System.Threading.Tasks.Task<ServiceReference1.SchuelerModel[]> GetSchuelerAsync();

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/GetSchuelerById", ReplyAction="http://tempuri.org/ISchuelerService/GetSchuelerByIdResponse")]
        System.Threading.Tasks.Task<ServiceReference1.SchuelerModel> GetSchuelerByIdAsync(int id);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/AddSchueler", ReplyAction="http://tempuri.org/ISchuelerService/AddSchuelerResponse")]
        System.Threading.Tasks.Task<bool> AddSchuelerAsync(ServiceReference1.SchuelerModel schueler);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/UpdateSchueler", ReplyAction="http://tempuri.org/ISchuelerService/UpdateSchuelerResponse")]
        System.Threading.Tasks.Task<bool> UpdateSchuelerAsync(ServiceReference1.SchuelerModel schueler);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/DeleteSchueler", ReplyAction="http://tempuri.org/ISchuelerService/DeleteSchuelerResponse")]
        System.Threading.Tasks.Task<bool> DeleteSchuelerAsync(int id);

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchuelerService/DeleteSchuelerMitAudit", ReplyAction="http://tempuri.org/ISchuelerService/DeleteSchuelerMitAuditResponse")]
        System.Threading.Tasks.Task<bool> DeleteSchuelerMitAuditAsync(int id, string auditUserName, string auditUserRole);
    }

    public interface ISchuelerServiceChannel : ISchuelerService, System.ServiceModel.IClientChannel { }

    public partial class SchuelerServiceClient : System.ServiceModel.ClientBase<ISchuelerService>, ISchuelerService
    {
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public SchuelerServiceClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {
            Endpoint.Name = EndpointConfiguration.BasicHttpBinding_ISchuelerService.ToString();
            ConfigureEndpoint(Endpoint, ClientCredentials);
        }

        public SchuelerServiceClient(EndpointConfiguration endpointConfiguration) : base(GetBindingForEndpoint(endpointConfiguration), GetEndpointAddress(endpointConfiguration))
        {
            Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(Endpoint, ClientCredentials);
        }

        public SchuelerServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(Endpoint, ClientCredentials);
        }

        public SchuelerServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : base(GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(Endpoint, ClientCredentials);
        }

        public SchuelerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) { }

        public System.Threading.Tasks.Task<SchuelerModel[]> GetSchuelerAsync() => Channel.GetSchuelerAsync();
        public System.Threading.Tasks.Task<SchuelerModel> GetSchuelerByIdAsync(int id) => Channel.GetSchuelerByIdAsync(id);
        public System.Threading.Tasks.Task<bool> AddSchuelerAsync(SchuelerModel schueler) => Channel.AddSchuelerAsync(schueler);
        public System.Threading.Tasks.Task<bool> UpdateSchuelerAsync(SchuelerModel schueler) => Channel.UpdateSchuelerAsync(schueler);
        public System.Threading.Tasks.Task<bool> DeleteSchuelerAsync(int id) => Channel.DeleteSchuelerAsync(id);
        public System.Threading.Tasks.Task<bool> DeleteSchuelerMitAuditAsync(int id, string auditUserName, string auditUserRole) => Channel.DeleteSchuelerMitAuditAsync(id, auditUserName, auditUserRole);

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)this).BeginOpen(null, null), ((System.ServiceModel.ICommunicationObject)this).EndOpen);
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if (endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISchuelerService)
            {
                var result = new System.ServiceModel.BasicHttpBinding
                {
                    MaxBufferSize = int.MaxValue,
                    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true
                };
                return result;
            }
            throw new System.InvalidOperationException($"Es wurde kein Endpunkt mit dem Namen \"{endpointConfiguration}\" gefunden.");
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if (endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISchuelerService)
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:5210/SchuelerService.svc");
            }
            throw new System.InvalidOperationException($"Es wurde kein Endpunkt mit dem Namen \"{endpointConfiguration}\" gefunden.");
        }

        private static System.ServiceModel.Channels.Binding GetDefaultBinding() => GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_ISchuelerService);
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() => GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_ISchuelerService);

        public enum EndpointConfiguration
        {
            BasicHttpBinding_ISchuelerService
        }
    }
}
