using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCcrypto.Data;
using MVCcrypto.Models;
using MVCcrypto.Models.Models;

namespace MVCcrypto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApiDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IActionResult Index()
    {
        ViewBag.Bitcoin = _context.Currencies.Where(c => c.Name == "bitcoin").ToList()!;
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
