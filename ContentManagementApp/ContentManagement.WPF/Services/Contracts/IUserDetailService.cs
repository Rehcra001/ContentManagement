using ContentManagement.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IUserDetailService
    {
        UserDetailModel UserDetailModel { get; set; }
    }
}
