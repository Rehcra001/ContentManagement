using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IProcessJWTTokenService
    {
        void ProcessJwtToken(string token);
        void ClearJwtToken();
    }
}
