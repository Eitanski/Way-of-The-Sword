using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MLEM.Misc;
using Microsoft.Xna.Framework;

namespace Game1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            TextInputWrapper.Current = new TextInputWrapper.DesktopGl<TextInputEventArgs>((w, c) => w.TextInput += c);

            using (var startMenu = new StartMenu())
                startMenu.Run();

            if(StartMenu.champion == "none") System.Environment.Exit(0);

            using (var game = new Game1())
                game.Run();
        }
    }
}
