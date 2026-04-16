using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb1_LINQ
{
    public class Menu
    {
        private readonly ShopDbContext _context;


        public Menu(ShopDbContext context)
        {
            _context = context;
        }

        public void ShowMainMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== SHOP MANAGEMENT SYSTEM ===");
                Console.WriteLine("1. Lista produkter i 'Electronics' (Dyrast först)");
                Console.WriteLine("2. Visa leverantörer med lågt lagersaldo (< 10)");
                Console.WriteLine("3. Beräkna totalt ordervärde (Mars 2026)");
                Console.WriteLine("4. Topp 3 mest sålda produkter");
                Console.WriteLine("5. Visa antal produkter per kategori");
                Console.WriteLine("6. Visa dyra ordrar (> 1000 kr) med kundnamn");
                Console.WriteLine("0. Avsluta");
                Console.Write("\nDitt val: ");

                var choice = Console.ReadLine();
                if (choice == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Avslutar programmet");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    break;
                }

                switch (choice)
                {
                    case "1":
                        ShowElectronics();
                        break;
                    case "2":
                        ShowLowStockSuppliers();
                        break;
                    case "3":
                        ShowTotalOrderValue();
                        break;
                    case "4":
                        ShowTop3Products();
                        break;
                    case "5":
                        ShowCategoryStats();
                        break;
                    case "6":
                        ShowExpensiveOrders();
                        break;
                }
            }
        }

        public void ShowTitle(string title)
        {
            Console.Clear();
            Console.WriteLine(title);
        }

        public void ShowElectronics()
        {
            //sortera efter pris dyrast först
            ShowTitle("ELECTRONICS");
            var products = _context.Products
                .Where(p => p.Category.Name == "Electronics")
                .OrderByDescending(p => p.Price);
                //.ToList();

            foreach (var p in products)
            {
                Console.WriteLine($"{p.Name}, {p.Price}kr");
            }

            Console.ReadKey();
        }
        public void ShowLowStockSuppliers()
        {
            //leverantörer med lågt lagersaldo <10
            ShowTitle("Leverantörer Med Lagersaldo Under 10");
            var suppliers = _context.Suppliers
                .Where(s => s.Products.Any(p => p.StockQuantity < 10))
                .ToList();

            foreach (var s in suppliers)
            {
                Console.WriteLine($"{s.Name}");
            }

            Console.ReadKey();
        }
        public void ShowTotalOrderValue()
        {
            using (var ctx = new ShopDbContext())
            {
                //beräkna totalt oredervärde mars 2026
                ShowTitle("Totalt Ordervärde Mars 2026");
                var total = ctx.Orders
                    .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-1))
                    .Sum(o => o.TotalAmount);

                Console.WriteLine($"Total Försäljning: {total:F2}kr");
            }
            Console.ReadKey();
        }
        public void ShowTop3Products()
        {
            //topp 3 mest sålda produkter
            ShowTitle("Topp 3 Sålda Produkter");
            var topProducts = _context.OrderDetails
                .GroupBy(od => od.Product.Name)
                .Select(g => new { Name = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.TotalSold)
                .Take(3)
                .ToList();

            foreach (var item in topProducts)
            {
                Console.WriteLine($"{item.Name}, Antal Sålda: {item.TotalSold}");
            }
            Console.ReadKey();
        }
        public void ShowCategoryStats()
        {
            //visa antal produkter per kategori
            ShowTitle("Antal Produkter Per Kategori");
            var stats = _context.Categories
                .Select(c => new { c.Name, c.Products.Count })
                .ToList();

            foreach (var s in stats)
            {
                Console.WriteLine($"{s.Name}, {s.Count} st produkter");
            }

            Console.ReadKey();
        }
        public void ShowExpensiveOrders()
        {
            //>1000kr med kundnamn
            ShowTitle("Ordrar Över 1000 kr");

            var orders = _context.Orders
                //.Include(o => o.Customer)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Product)
                .Where(o => o.TotalAmount > 1000)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Customer.Name,
                    o.Customer.Email,
                    o.Customer.Address,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.Product.Name,
                        od.Quantity,
                        od.UnitPrice
                    })
                })
                .OrderByDescending(o => o.TotalAmount)
                .ToList();

            foreach (var o in orders)
            {
                Console.WriteLine(new string('~', 60));

                Console.WriteLine($"\nOrder #{o.Id} | Datum: {o.OrderDate.ToShortDateString()} | Totalt: {o.TotalAmount}kr");

                // 2. Skriv ut Kunduppgifter
                Console.WriteLine($"Kund:  {o.Name} ({o.Email})");
                Console.WriteLine($"Adress: {o.Address}");

                Console.WriteLine("Produkter:");
                foreach (var detail in o.OrderDetails)
                {
                    Console.WriteLine($" - {detail.Name} {detail.Quantity} st x {detail.UnitPrice}kr\n");
                }

                Console.WriteLine(new string('~', 60));
            }
            Console.ReadKey();
        }
    }
}
