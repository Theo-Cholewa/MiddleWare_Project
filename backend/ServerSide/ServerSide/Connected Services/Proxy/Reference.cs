﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServerSide.Proxy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Proxy.ISoapServer")]
    public interface ISoapServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISoapServer/Add", ReplyAction="http://tempuri.org/ISoapServer/AddResponse")]
        int Add(int num1, int num2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISoapServer/Add", ReplyAction="http://tempuri.org/ISoapServer/AddResponse")]
        System.Threading.Tasks.Task<int> AddAsync(int num1, int num2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISoapServerChannel : ServerSide.Proxy.ISoapServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SoapServerClient : System.ServiceModel.ClientBase<ServerSide.Proxy.ISoapServer>, ServerSide.Proxy.ISoapServer {
        
        public SoapServerClient() {
        }
        
        public SoapServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SoapServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SoapServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SoapServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int Add(int num1, int num2) {
            return base.Channel.Add(num1, num2);
        }
        
        public System.Threading.Tasks.Task<int> AddAsync(int num1, int num2) {
            return base.Channel.AddAsync(num1, num2);
        }
    }
}
