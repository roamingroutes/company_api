using System.Reflection.Metadata.Ecma335;
using company_api.validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace company_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        // In-memory "database"
        private readonly AppDbContext _context;

        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "Company")]
        public IActionResult GetCompany()
        {
            return Ok(_context.Companies.ToList());
        }

        // GET: api/company/{id}
        [HttpGet("{id}")]
        public IActionResult GetCompany(int id)
        {

            if (!CompanyValidations.IsNumeric(id.ToString()))
            {
                return BadRequest("Company Id is not valid. Please provide an whole number.\n");
            }

            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound($"The company id provided ({id.ToString()}) does not exist.");
            }

            return Ok(company);
        }

        [HttpPost(Name = "Company")]
        public IActionResult AddCompany([FromBody] Company company)
        {
            var companyList = _context.Companies.ToList().FindAll(x=> x.Id == company.Id || x.Name == company.Name || x.WebsiteUrl == company.WebsiteUrl);

            var errorList = CompanyValidations.ValidateCompanyInputs(company, companyList).ToString();
            if (errorList.Length > 0)
            {
                return BadRequest(errorList);
            }

            _context.Companies.Add(company);
            _context.SaveChanges();

            return CreatedAtAction(nameof(AddCompany), new { id = company.Id }, company);
        }

    }
}
