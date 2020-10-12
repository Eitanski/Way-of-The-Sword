using System;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Communicator.Setup();
            using (var game = new Game1())
                game.Run();
        }
    }
}
