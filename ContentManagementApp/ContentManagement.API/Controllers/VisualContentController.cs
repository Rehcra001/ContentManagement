using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisualContentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VisualContentController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/<VisualContentController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // try save file
            string path = _config.GetSection("ContentRootFileSystem").Value!;
            string textToSave = "This text to be saved";
            string filename = "TextToSave.txt";

            using (StreamWriter sw = new StreamWriter(Path.Combine(path, filename)))
            {
                sw.WriteLine(textToSave);
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/<VisualContentController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<VisualContentController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<VisualContentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<VisualContentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
