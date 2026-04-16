namespace Labb1_LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new ShopDbContext();
            var menu = new Menu(context);

            menu.ShowMainMenu();
            Console.Write("Hejdå!");
        }
    }
}
