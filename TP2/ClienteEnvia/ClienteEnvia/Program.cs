using System;
using System.Windows.Forms;

namespace ClienteEnvia
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Habilita estilos visuais modernos do Windows e compatibilidade de texto
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia a aplicação exibindo o formulário do cliente
            Application.Run(new FormCliente());
        }
    }
}
