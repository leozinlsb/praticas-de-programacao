using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Prova
{
    class DAL
    {
        private static String strConexao = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\unisanta\Desktop\Trabalho 1 - Consulta de CNPJ\PrimeiroExercicio\Prova\bin\Debug\BDFarinha.mdb";
        private static OleDbConnection conn = new OleDbConnection(strConexao);
        private static OleDbCommand strSQL;
        private static OleDbDataReader result;

        public static void conecta()
        {
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                Erro.setMsg("Problemas ao se conectar ao Banco de Dados");
            }

        }

        public static void desconecta()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public static void consultaUmCliente()
        {
            try
            {
                String query = "SELECT * FROM TabClientes WHERE CNPJ = '" + Cliente.getCNPJ() + "'";
                strSQL = new OleDbCommand(query, conn);
                result = strSQL.ExecuteReader();
                if (result.Read())
                {
                    Cliente.setNome(result["Nome"].ToString());
                }
                else
                {
                    Erro.setMsg("Cliente não encontrado.");
                }
                result.Close();
            }
            catch (Exception ex)
            {
                Erro.setMsg("Erro ao consultar cliente: " + ex.Message);
            }
        }

        public static OleDbDataReader consultaVendasCliente()
        {
            try
            {
                String query = "SELECT * FROM TabVendasCliente WHERE CNPJ = '" + Cliente.getCNPJ() + "'";
                strSQL = new OleDbCommand(query, conn);
                return strSQL.ExecuteReader();
            }
            catch (Exception ex)
            {
                Erro.setMsg("Erro ao consultar vendas do cliente: " + ex.Message);
                return null;
            }
        }
        
    }
}
