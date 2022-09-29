using Microsoft.AspNetCore.Mvc;
using MVCcrypto.Data;

namespace MVCcrypto.Controllers;

public class CurrencyController : Controller
{
    private ApiDbContext _dbContext;
    public CurrencyController(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // GET
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}