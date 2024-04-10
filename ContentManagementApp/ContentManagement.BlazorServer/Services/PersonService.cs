using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Models;
using ContentManagement.BlazorServer.ValidationClasses;
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

        /// <summary>
        /// Adds a person to the ContentManagementDB
        /// </summary>
        /// <param name="person"></param>
        /// <returns>PersonModel</returns>
        /// <exception cref="Exception"></exception>
        public async Task<PersonModel> AddPerson(PersonModel person)
        {
            try
            {
                //Validate person
                var validationErrors = ValidationHelper.Validate(person);
                if (validationErrors.Count > 0)
                {


                    foreach (ValidationMessage message in validationErrors)
                    {
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
                    _logger.Error(ex, ex.Message);
                    return new PersonModel();
                }

                await Log.CloseAndFlushAsync();
                return person;
            }
            catch (Exception ex)
            {
                //Log exception
                _logger.Error(ex, ex.Message);
                return new PersonModel();
            }
        }

        /// <summary>
        /// Check if a person already exists in ContentManagementDB
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Bool: True if a person exists. Otherwise false</returns>
        public async Task<bool> PersonExists(string username)
        {
            try
            {
                bool exists = await _personRepository.PersonExists(username);
                return exists;

            }
            catch (Exception ex)
            {
                //Log exception
                _logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates the person Display Name
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Bool: True if succeeded. False otherwise</returns>
        public async Task<bool> UpdatePerson(PersonModel person)
        {
            try
            {
                bool updated = await _personRepository.UpdatePerson(person);
                return updated;
            }
            catch (Exception ex)
            {
                //Log exception
                _logger.Error(ex, ex.Message);
                return false;
            }
        }
    }
}
