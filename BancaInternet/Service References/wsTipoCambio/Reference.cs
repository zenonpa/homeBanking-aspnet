﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BancaInternet.wsTipoCambio {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="wsTipoCambio.IwsTipoCambio")]
    public interface IwsTipoCambio {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsTipoCambio/WSObtenerTipoCambioDelDia", ReplyAction="http://tempuri.org/IwsTipoCambio/WSObtenerTipoCambioDelDiaResponse")]
        BancaInternet.EN.enTipoCambio WSObtenerTipoCambioDelDia();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsTipoCambio/WSObtenerTipoCambioDelDia", ReplyAction="http://tempuri.org/IwsTipoCambio/WSObtenerTipoCambioDelDiaResponse")]
        System.Threading.Tasks.Task<BancaInternet.EN.enTipoCambio> WSObtenerTipoCambioDelDiaAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IwsTipoCambioChannel : BancaInternet.wsTipoCambio.IwsTipoCambio, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IwsTipoCambioClient : System.ServiceModel.ClientBase<BancaInternet.wsTipoCambio.IwsTipoCambio>, BancaInternet.wsTipoCambio.IwsTipoCambio {
        
        public IwsTipoCambioClient() {
        }
        
        public IwsTipoCambioClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IwsTipoCambioClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IwsTipoCambioClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IwsTipoCambioClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public BancaInternet.EN.enTipoCambio WSObtenerTipoCambioDelDia() {
            return base.Channel.WSObtenerTipoCambioDelDia();
        }
        
        public System.Threading.Tasks.Task<BancaInternet.EN.enTipoCambio> WSObtenerTipoCambioDelDiaAsync() {
            return base.Channel.WSObtenerTipoCambioDelDiaAsync();
        }
    }
}
