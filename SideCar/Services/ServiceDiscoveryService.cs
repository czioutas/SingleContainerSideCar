using System;
using System.Collections.Generic;
using SideCar.Services.Contracts;

namespace SideCar.Services
{
    public class ServiceDiscoveryService : IServiceDiscoveryService
    {

        private Dictionary<string, string> _addressBook;

        public ServiceDiscoveryService()
        {
            _addressBook = new Dictionary<string, string>() {
                {"internal_app_0", "localhost:5000"},
                {"internal_app_1", "localhost:5100"},
                {"internal_app_3", "localhost:5300"},
                {"internal_app_4", "localhost:5400"},
                {"internal_app_5", "localhost:5500"},
            };
        }

        public string GetService(string key)
        {
            if (!_addressBook.ContainsKey(key))
            {
                return null;
            }

            return _addressBook[key];
        }
    }
}