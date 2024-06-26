﻿using ContentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Repositories.Contracts
{
    public interface IPersonRepository
    {
        Task<PersonModel> AddPerson(PersonModel person);
        Task<IEnumerable<PersonModel>> GetPeople();
        IEnumerable<PersonModel> GetPeople(string str = "");
        Task<bool> PersonExists(string username);
        Task<bool> UpdatePerson(PersonModel person);
        Task<PersonModel> GetPerson(string username);
        Task<int> GetPersonId(string username);
    }
}
