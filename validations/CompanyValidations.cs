using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace company_api.validations
{
    public static class CompanyValidations
    {
        public static StringBuilder ValidateCompanyInputs(Company company, List<Company> companyList)
        {
            var errorBuilder = new StringBuilder();

            if (!IsNumeric(company.Id.ToString()))
            {
                errorBuilder.Append("Company Id is not valid. Please provide an whole number.\n");
            }

            if (companyList.FindAll(x => x.Id == company.Id).Count > 0)
            {
                errorBuilder.Append("Company Id already exists.\n");
            }

            if (string.IsNullOrWhiteSpace(company.Name) || company.Name.Length < 3)
            {
                errorBuilder.Append("Company Name must be at least 3 characters long.\n");
            }

            if(companyList.FindAll(x => x.Name == company.Name).Count>0)
            {
                errorBuilder.Append("Company Name already exists.\n");
            }

            if (string.IsNullOrWhiteSpace(company.WebsiteUrl) || company.WebsiteUrl.Length < 3)
            {
                errorBuilder.Append("WebsiteUrl must be at least 3 characters long.\n");
            }

            if (companyList.FindAll(x => x.WebsiteUrl == company.WebsiteUrl).Count > 0)
            {
                errorBuilder.Append("WebsiteUrl already exists.\n");
            }

            if (string.IsNullOrWhiteSpace(company.WebsiteUrl) ||
                !Uri.TryCreate(company.WebsiteUrl, UriKind.Absolute, out Uri? result) ||
                (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps))
            {
                errorBuilder.Append("Website URL must be a valid URL starting with http:// or https://.\n");
            }

            if (!IsRelevant(company.Name!, company.WebsiteUrl!))
            {
                return errorBuilder.Append("Company Name does not seem relevant to the Website URL.\n");
            }

            return errorBuilder;

        }

        public static bool IsNumeric(string value)
        {
            return int.TryParse(value, out _); 
        }

        private static bool IsRelevant(string name, string websiteUrl)
        {
            var companyName = Regex.Replace(name, @"\s+", "").ToLower();
            return websiteUrl.ToLower().Contains(companyName);
        }
    }
}
