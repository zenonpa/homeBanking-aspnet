﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BancaInternet.wsCuenta {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="wsCuenta.IwsCuenta")]
    public interface IwsCuenta {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerCuentasPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerCuentasPasivasClienteResponse")]
        BancaInternet.EN.enCuenta[] WSObtenerCuentasPasivasCliente(string sValor1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerCuentasPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerCuentasPasivasClienteResponse")]
        System.Threading.Tasks.Task<BancaInternet.EN.enCuenta[]> WSObtenerCuentasPasivasClienteAsync(string sValor1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerDetalleCuentasPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerDetalleCuentasPasivasClienteResponse")]
        BancaInternet.EN.enCuenta WSObtenerDetalleCuentasPasivasCliente(string valor1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerDetalleCuentasPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerDetalleCuentasPasivasClienteResponse")]
        System.Threading.Tasks.Task<BancaInternet.EN.enCuenta> WSObtenerDetalleCuentasPasivasClienteAsync(string valor1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerMovimientosCuentaPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerMovimientosCuentaPasivasClienteResponse")]
        BancaInternet.EN.enCuenta[] WSObtenerMovimientosCuentaPasivasCliente(string sValor1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IwsCuenta/WSObtenerMovimientosCuentaPasivasCliente", ReplyAction="http://tempuri.org/IwsCuenta/WSObtenerMovimientosCuentaPasivasClienteResponse")]
        System.Threading.Tasks.Task<BancaInternet.EN.enCuenta[]> WSObtenerMovimientosCuentaPasivasClienteAsync(string sValor1);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IwsCuentaChannel : BancaInternet.wsCuenta.IwsCuenta, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IwsCuentaClient : System.ServiceModel.ClientBase<BancaInternet.wsCuenta.IwsCuenta>, BancaInternet.wsCuenta.IwsCuenta {
        
        public IwsCuentaClient() {
        }
        
        public IwsCuentaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IwsCuentaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IwsCuentaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IwsCuentaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public BancaInternet.EN.enCuenta[] WSObtenerCuentasPasivasCliente(string sValor1) {
            return base.Channel.WSObtenerCuentasPasivasCliente(sValor1);
        }
        
        public System.Threading.Tasks.Task<BancaInternet.EN.enCuenta[]> WSObtenerCuentasPasivasClienteAsync(string sValor1) {
            return base.Channel.WSObtenerCuentasPasivasClienteAsync(sValor1);
        }
        
        public BancaInternet.EN.enCuenta WSObtenerDetalleCuentasPasivasCliente(string valor1) {
            return base.Channel.WSObtenerDetalleCuentasPasivasCliente(valor1);
        }
        
        public System.Threading.Tasks.Task<BancaInternet.EN.enCuenta> WSObtenerDetalleCuentasPasivasClienteAsync(string valor1) {
            return base.Channel.WSObtenerDetalleCuentasPasivasClienteAsync(valor1);
        }
        
        public BancaInternet.EN.enCuenta[] WSObtenerMovimientosCuentaPasivasCliente(string sValor1) {
            return base.Channel.WSObtenerMovimientosCuentaPasivasCliente(sValor1);
        }
        
        public System.Threading.Tasks.Task<BancaInternet.EN.enCuenta[]> WSObtenerMovimientosCuentaPasivasClienteAsync(string sValor1) {
            return base.Channel.WSObtenerMovimientosCuentaPasivasClienteAsync(sValor1);
        }
    }
}
