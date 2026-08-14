using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prova
{
    class BLL
    {
        public static void conecta()
        {
            DAL.conecta();
        }

        public static void desconecta()
        {
            DAL.desconecta();
        }

        public static bool isValidoCNPJ(String _cnpj)
        {
            return true;
        }

        public static void validaCNPJ()
        {
            Erro.setErro(false);
            if (Cliente.getCNPJ().Equals(""))
            {
                Erro.setMsg("O código é de preenchimento obrigatório!");
                return;
            }
            if (!isValidoCNPJ(Cliente.getCNPJ()))
            {
                Erro.setMsg("O CNPJ digitado não é válido!");
                return;
            }

            //DAL.consultaUmCliente();
        }

        public static void validaDados()
        {
            Erro.setErro(false);
            if (Cliente.getCNPJ().Equals(""))
            {
                Erro.setMsg("O CNPJ é de preenchimento obrigatório!");
                return;
            }
            if (!isValidoCNPJ(Cliente.getCNPJ()))
            {
                Erro.setMsg("O CNPJ digitado não é válido!");
                return;
            }

            DAL.consultaUmCliente();
        }
    }
}
