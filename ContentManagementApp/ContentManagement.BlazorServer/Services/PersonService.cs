using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Models;
using ContentManagement.Models.ValidationClasses;
using ContentManagement.Repositories.Contracts;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ContentManagement.BlazorServer.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogger _logger;


        public PersonService(IPersonRepository personRepository, ILogger logger)
        {
            _personRepository = personRepository;
            _logger = logger;
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
                        _logger.Error("Validation Error: " + message.ToString());

                    }

                    throw new Exception("Validation Errors");
                }

                //Add person
                try
                {
                    person = await _personRepository.AddPerson(person);
                    _logger.Information("Person Service: Person Added");

                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    throw;
                }

                await Log.CloseAndFlushAsync();
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
