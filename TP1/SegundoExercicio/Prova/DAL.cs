using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace Prova
{
    class DAL
    {
        private static String strConexao = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\unisanta\Desktop\Trabalho 1 - Consulta de CNPJ\SegundoExercicio\Prova\bin\Debug\BDFarinha.mdb";
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
            conn.Close();
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

        public static double consultaTotalToneladas()
        {
            try
            {
                String query = "SELECT SUM(Toneladas) FROM TabVendasCliente WHERE CNPJ = '" + Cliente.getCNPJ() + "'";
                strSQL = new OleDbCommand(query, conn);
                object result = strSQL.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToDouble(result);
                }
            }
            catch (Exception ex)
            {
                Erro.setMsg("Erro ao consultar total de toneladas: " + ex.Message);
            }
            return 0;
        }

        public static double consultaTotalValor()
        {
            try
            {
                String query = "SELECT SUM(Valor) FROM TabVendasCliente WHERE CNPJ = '" + Cliente.getCNPJ() + "'";
                strSQL = new OleDbCommand(query, conn);
                object result = strSQL.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToDouble(result);
                }
            }
            catch (Exception ex)
            {
                Erro.setMsg("Erro ao consultar total do valor: " + ex.Message);
            }
            return 0;
        }

        public static DataTable consultaDadosGrafico()
        {
            try
            {
                String query = "SELECT Data, Toneladas, Valor FROM TabVendasCliente WHERE CNPJ = '" + Cliente.getCNPJ() + "' ORDER BY Data";
                strSQL = new OleDbCommand(query, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(strSQL);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Erro.setMsg("Erro ao consultar dados para o gráfico: " + ex.Message);
                return null;
            }
        }
        
    }
}
