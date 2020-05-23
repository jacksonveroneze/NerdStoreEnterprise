﻿namespace NSE.Identidade.API.Extensions
{
    public class EmailSettings
    {
        public string Host { get; set; }
        
        public int Port { get; set; }
        
        public bool EnableSSL { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}