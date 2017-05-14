using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Controle_Tranche
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Authentification authentifcation = new Authentification();
            authentifcation.Show();
            Application.Run();
        }

         
    }
}
