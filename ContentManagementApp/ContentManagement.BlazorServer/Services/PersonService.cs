using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Models;
using ContentManagement.Models.ValidationClasses;
using ContentManagement.Repositories.Contracts;

namespace ContentManagement.BlazorServer.Services
{
    public class PersonService : IPersonService
    {
        private IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<PersonModel> AddPerson(PersonModel person)
        {
            try
            {
                //Validate person
                var validationErrors = ValidationHelper.Validate(person);
                if (validationErrors.Count > 0)
                {


                    foreach(ValidationMessage message in validationErrors)
                    {
                        // TODO - Log validation errors
                    }

                    throw new Exception("Error saving person");
                    // TODO - Add logging functionality
                }

                //Add person
                person = await _personRepository.AddPerson(person);

                return person;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> PersonExists(string username)
        {
            try
            {
                bool exists = await _personRepository.PersonExists(username);
                return exists;

            }
            catch (Exception)
            {
                //Log exception
                // TODO - Add logging functionality
                throw;
            }
        }
    }
}
