using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinhaBiblioteca;

namespace Serializacao01
{
    public class DAL
    {
        private static String strConexao = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=BDLivros.mdb";
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
                Erro.setMensagem("Problemas ao se conectar ao Banco de Dados");
            }
        }

        public static void desconecta() 
        {
            conn.Close();
        }

        public static List<Livro> BuscarLivros()
        {
            String comando = "SELECT * FROM TabLivro ORDER BY codigo ASC";
            strSQL = new OleDbCommand(comando, conn);
            result = strSQL.ExecuteReader();
            Erro.setErro(false);

            List<Livro> livros = new List<Livro>();

            while (result.Read())
            {
                Livro livro = new Livro();

                livro.setCodigo(result.GetString(0));
                livro.setTitulo(result.GetString(1));
                livro.setAutor(result.GetString(2));
                livro.setEditora(result.GetString(3));
                livro.setAno(result.GetString(4));

                livros.Add(livro);
            }

            return livros;
        }
    }
}
