using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcSka.Models;

namespace MvcSka.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MemoryCacheEntryOptions? _cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));
    private IMemoryCache Cache { get; set; }
    public HomeController(ILogger<HomeController> logger,IMemoryCache memoryCache)
    {
        _logger = logger;
        Cache = memoryCache;
    }
    public IActionResult Index()
    {
        var customerTable = InitCustomerTable();
        return View(customerTable);
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

    public CustomerTableModel InitCustomerTable()
    {
        Cache.TryGetValue("CacheKey", out CustomerTableModel model);
        if (model == null)
        {
            var customerData = new CustomerModel(0, "David", 10000);
            var customerDataList = new List<CustomerModel> { customerData };
            var customerTable = new CustomerTableModel
            {
                CustomerData = customerDataList
            };

            Cache.Set("CacheKey", customerTable, _cacheEntryOptions);
            return customerTable;
        }
        else
        {
            return model;
        }
    }

    public ActionResult CreateCustomer(int id, string name, int balance)
    {
        Cache.TryGetValue("CacheKey", out CustomerTableModel model);
        if (model == null)
        {
            var customerData = new CustomerModel(id, name, balance);
            var customerDataList = new List<CustomerModel> { customerData };
            var customerTable = new CustomerTableModel();
        
            customerTable.CustomerData = customerDataList;
            Cache.Set("CacheKey", customerTable, _cacheEntryOptions);
        }
        else
        {
            if (model.CustomerData.Any(a => a.id == id))
            {
                TempData["Message"] = "ID不可重複";
                return RedirectToAction("Index");
            }
            var customerData = new CustomerModel(id, name, balance);
            model.CustomerData.Add(customerData);
            Cache.Set("CacheKey", model, _cacheEntryOptions);
        }
        return RedirectToAction("Index");
    }
    
    public ActionResult DeleteCustomer(int deleteId)
    {
        Cache.TryGetValue("CacheKey", out CustomerTableModel model);
        if (model == null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var itemToRemove = model.CustomerData.Single(r => r.id == deleteId);
            model.CustomerData.Remove(itemToRemove);
            Cache.Set("CacheKey", model, _cacheEntryOptions);
        }
        return RedirectToAction("Index");
    }
    public ActionResult ModifyCustomer(int modifyId, string modifyName, int modifyBalance)
    {
        Cache.TryGetValue("CacheKey", out CustomerTableModel model);
        if (model == null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var itemToModify = model.CustomerData.Single(r => r.id == modifyId);
            model.CustomerData[model.CustomerData.IndexOf(itemToModify)] = new CustomerModel(modifyId, modifyName, modifyBalance);
            Cache.Set("CacheKey", model, _cacheEntryOptions);
        }
        return RedirectToAction("Index");
    }
    
}