﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Controle_Tranche.CWS.Authentication {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:Core.service.livelink.opentext.com", ConfigurationName="CWS.Authentication.Authentication")]
    public interface Authentication {
        
        // CODEGEN : La génération du contrat de message depuis le message AuthenticateApplicationRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/AuthenticateApplication", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/AuthenticateApplicationResp" +
            "onse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.AuthenticateApplicationResponse AuthenticateApplication(Controle_Tranche.CWS.Authentication.AuthenticateApplicationRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/AuthenticateUser", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/AuthenticateUserResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string AuthenticateUser(string userName, string userPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/AuthenticateUserWithApplicationToken", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/AuthenticateUserWithApplica" +
            "tionTokenResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string AuthenticateUserWithApplicationToken(string userName, string userPassword, string applicationToken);
        
        // CODEGEN : La génération du contrat de message depuis le message CombineApplicationTokenRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/CombineApplicationToken", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/CombineApplicationTokenResp" +
            "onse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.CombineApplicationTokenResponse CombineApplicationToken(Controle_Tranche.CWS.Authentication.CombineApplicationTokenRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/GetOTDSResourceID", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/GetOTDSResourceIDResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string GetOTDSResourceID();
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/GetOTDSServer", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/GetOTDSServerResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string GetOTDSServer();
        
        // CODEGEN : La génération du contrat de message depuis le message GetSessionExpirationDateRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/GetSessionExpirationDate", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/GetSessionExpirationDateRes" +
            "ponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.GetSessionExpirationDateResponse GetSessionExpirationDate(Controle_Tranche.CWS.Authentication.GetSessionExpirationDateRequest request);
        
        // CODEGEN : La génération du contrat de message depuis le message ImpersonateApplicationRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/ImpersonateApplication", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/ImpersonateApplicationRespo" +
            "nse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.ImpersonateApplicationResponse ImpersonateApplication(Controle_Tranche.CWS.Authentication.ImpersonateApplicationRequest request);
        
        // CODEGEN : La génération du contrat de message depuis le message ImpersonateUserRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/ImpersonateUser", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/ImpersonateUserResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.ImpersonateUserResponse ImpersonateUser(Controle_Tranche.CWS.Authentication.ImpersonateUserRequest request);
        
        // CODEGEN : La génération du contrat de message depuis le message RefreshTokenRequest contient des en-têtes
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/RefreshToken", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/RefreshTokenResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Controle_Tranche.CWS.Authentication.RefreshTokenResponse RefreshToken(Controle_Tranche.CWS.Authentication.RefreshTokenRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:Core.service.livelink.opentext.com/ValidateUser", ReplyAction="urn:Core.service.livelink.opentext.com/Authentication/ValidateUserResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string ValidateUser(string capToken);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.79.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:api.ecm.opentext.com")]
    public partial class OTAuthentication : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string authenticationTokenField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string AuthenticationToken {
            get {
                return this.authenticationTokenField;
            }
            set {
                this.authenticationTokenField = value;
                this.RaisePropertyChanged("AuthenticationToken");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AuthenticateApplication", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class AuthenticateApplicationRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string applicationID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=1)]
        public string password;
        
        public AuthenticateApplicationRequest() {
        }
        
        public AuthenticateApplicationRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationID, string password) {
            this.OTAuthentication = OTAuthentication;
            this.applicationID = applicationID;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AuthenticateApplicationResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class AuthenticateApplicationResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string AuthenticateApplicationResult;
        
        public AuthenticateApplicationResponse() {
        }
        
        public AuthenticateApplicationResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string AuthenticateApplicationResult) {
            this.OTAuthentication = OTAuthentication;
            this.AuthenticateApplicationResult = AuthenticateApplicationResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CombineApplicationToken", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class CombineApplicationTokenRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string applicationToken;
        
        public CombineApplicationTokenRequest() {
        }
        
        public CombineApplicationTokenRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationToken) {
            this.OTAuthentication = OTAuthentication;
            this.applicationToken = applicationToken;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CombineApplicationTokenResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class CombineApplicationTokenResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string CombineApplicationTokenResult;
        
        public CombineApplicationTokenResponse() {
        }
        
        public CombineApplicationTokenResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string CombineApplicationTokenResult) {
            this.OTAuthentication = OTAuthentication;
            this.CombineApplicationTokenResult = CombineApplicationTokenResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetSessionExpirationDate", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class GetSessionExpirationDateRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        public GetSessionExpirationDateRequest() {
        }
        
        public GetSessionExpirationDateRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication) {
            this.OTAuthentication = OTAuthentication;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetSessionExpirationDateResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class GetSessionExpirationDateResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> GetSessionExpirationDateResult;
        
        public GetSessionExpirationDateResponse() {
        }
        
        public GetSessionExpirationDateResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, System.Nullable<System.DateTime> GetSessionExpirationDateResult) {
            this.OTAuthentication = OTAuthentication;
            this.GetSessionExpirationDateResult = GetSessionExpirationDateResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ImpersonateApplication", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class ImpersonateApplicationRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string applicationID;
        
        public ImpersonateApplicationRequest() {
        }
        
        public ImpersonateApplicationRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationID) {
            this.OTAuthentication = OTAuthentication;
            this.applicationID = applicationID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ImpersonateApplicationResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class ImpersonateApplicationResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string ImpersonateApplicationResult;
        
        public ImpersonateApplicationResponse() {
        }
        
        public ImpersonateApplicationResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string ImpersonateApplicationResult) {
            this.OTAuthentication = OTAuthentication;
            this.ImpersonateApplicationResult = ImpersonateApplicationResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ImpersonateUser", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class ImpersonateUserRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string userName;
        
        public ImpersonateUserRequest() {
        }
        
        public ImpersonateUserRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string userName) {
            this.OTAuthentication = OTAuthentication;
            this.userName = userName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ImpersonateUserResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class ImpersonateUserResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string ImpersonateUserResult;
        
        public ImpersonateUserResponse() {
        }
        
        public ImpersonateUserResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string ImpersonateUserResult) {
            this.OTAuthentication = OTAuthentication;
            this.ImpersonateUserResult = ImpersonateUserResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RefreshToken", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class RefreshTokenRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        public RefreshTokenRequest() {
        }
        
        public RefreshTokenRequest(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication) {
            this.OTAuthentication = OTAuthentication;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RefreshTokenResponse", WrapperNamespace="urn:Core.service.livelink.opentext.com", IsWrapped=true)]
    public partial class RefreshTokenResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="urn:api.ecm.opentext.com")]
        public Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:Core.service.livelink.opentext.com", Order=0)]
        public string RefreshTokenResult;
        
        public RefreshTokenResponse() {
        }
        
        public RefreshTokenResponse(Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string RefreshTokenResult) {
            this.OTAuthentication = OTAuthentication;
            this.RefreshTokenResult = RefreshTokenResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AuthenticationChannel : Controle_Tranche.CWS.Authentication.Authentication, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationClient : System.ServiceModel.ClientBase<Controle_Tranche.CWS.Authentication.Authentication>, Controle_Tranche.CWS.Authentication.Authentication {
        
        public AuthenticationClient() {
        }
        
        public AuthenticationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.AuthenticateApplicationResponse Controle_Tranche.CWS.Authentication.Authentication.AuthenticateApplication(Controle_Tranche.CWS.Authentication.AuthenticateApplicationRequest request) {
            return base.Channel.AuthenticateApplication(request);
        }
        
        public string AuthenticateApplication(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationID, string password) {
            Controle_Tranche.CWS.Authentication.AuthenticateApplicationRequest inValue = new Controle_Tranche.CWS.Authentication.AuthenticateApplicationRequest();
            inValue.OTAuthentication = OTAuthentication;
            inValue.applicationID = applicationID;
            inValue.password = password;
            Controle_Tranche.CWS.Authentication.AuthenticateApplicationResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).AuthenticateApplication(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.AuthenticateApplicationResult;
        }
        
        public string AuthenticateUser(string userName, string userPassword) {
            return base.Channel.AuthenticateUser(userName, userPassword);
        }
        
        public string AuthenticateUserWithApplicationToken(string userName, string userPassword, string applicationToken) {
            return base.Channel.AuthenticateUserWithApplicationToken(userName, userPassword, applicationToken);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.CombineApplicationTokenResponse Controle_Tranche.CWS.Authentication.Authentication.CombineApplicationToken(Controle_Tranche.CWS.Authentication.CombineApplicationTokenRequest request) {
            return base.Channel.CombineApplicationToken(request);
        }
        
        public string CombineApplicationToken(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationToken) {
            Controle_Tranche.CWS.Authentication.CombineApplicationTokenRequest inValue = new Controle_Tranche.CWS.Authentication.CombineApplicationTokenRequest();
            inValue.OTAuthentication = OTAuthentication;
            inValue.applicationToken = applicationToken;
            Controle_Tranche.CWS.Authentication.CombineApplicationTokenResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).CombineApplicationToken(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.CombineApplicationTokenResult;
        }
        
        public string GetOTDSResourceID() {
            return base.Channel.GetOTDSResourceID();
        }
        
        public string GetOTDSServer() {
            return base.Channel.GetOTDSServer();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.GetSessionExpirationDateResponse Controle_Tranche.CWS.Authentication.Authentication.GetSessionExpirationDate(Controle_Tranche.CWS.Authentication.GetSessionExpirationDateRequest request) {
            return base.Channel.GetSessionExpirationDate(request);
        }
        
        public System.Nullable<System.DateTime> GetSessionExpirationDate(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication) {
            Controle_Tranche.CWS.Authentication.GetSessionExpirationDateRequest inValue = new Controle_Tranche.CWS.Authentication.GetSessionExpirationDateRequest();
            inValue.OTAuthentication = OTAuthentication;
            Controle_Tranche.CWS.Authentication.GetSessionExpirationDateResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).GetSessionExpirationDate(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.GetSessionExpirationDateResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.ImpersonateApplicationResponse Controle_Tranche.CWS.Authentication.Authentication.ImpersonateApplication(Controle_Tranche.CWS.Authentication.ImpersonateApplicationRequest request) {
            return base.Channel.ImpersonateApplication(request);
        }
        
        public string ImpersonateApplication(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string applicationID) {
            Controle_Tranche.CWS.Authentication.ImpersonateApplicationRequest inValue = new Controle_Tranche.CWS.Authentication.ImpersonateApplicationRequest();
            inValue.OTAuthentication = OTAuthentication;
            inValue.applicationID = applicationID;
            Controle_Tranche.CWS.Authentication.ImpersonateApplicationResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).ImpersonateApplication(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.ImpersonateApplicationResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.ImpersonateUserResponse Controle_Tranche.CWS.Authentication.Authentication.ImpersonateUser(Controle_Tranche.CWS.Authentication.ImpersonateUserRequest request) {
            return base.Channel.ImpersonateUser(request);
        }
        
        public string ImpersonateUser(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication, string userName) {
            Controle_Tranche.CWS.Authentication.ImpersonateUserRequest inValue = new Controle_Tranche.CWS.Authentication.ImpersonateUserRequest();
            inValue.OTAuthentication = OTAuthentication;
            inValue.userName = userName;
            Controle_Tranche.CWS.Authentication.ImpersonateUserResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).ImpersonateUser(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.ImpersonateUserResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Controle_Tranche.CWS.Authentication.RefreshTokenResponse Controle_Tranche.CWS.Authentication.Authentication.RefreshToken(Controle_Tranche.CWS.Authentication.RefreshTokenRequest request) {
            return base.Channel.RefreshToken(request);
        }
        
        public string RefreshToken(ref Controle_Tranche.CWS.Authentication.OTAuthentication OTAuthentication) {
            Controle_Tranche.CWS.Authentication.RefreshTokenRequest inValue = new Controle_Tranche.CWS.Authentication.RefreshTokenRequest();
            inValue.OTAuthentication = OTAuthentication;
            Controle_Tranche.CWS.Authentication.RefreshTokenResponse retVal = ((Controle_Tranche.CWS.Authentication.Authentication)(this)).RefreshToken(inValue);
            OTAuthentication = retVal.OTAuthentication;
            return retVal.RefreshTokenResult;
        }
        
        public string ValidateUser(string capToken) {
            return base.Channel.ValidateUser(capToken);
        }
    }
}
