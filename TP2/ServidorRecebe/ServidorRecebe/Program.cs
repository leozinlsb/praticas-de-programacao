using System;
using System.Windows.Forms;

namespace ServidorRecebe
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia a aplicação exibindo o formulário do servidor
            Application.Run(new FormServidor());
        }
    }
}
