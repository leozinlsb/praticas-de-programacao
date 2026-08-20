using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Serializacao01
{
    class Program
    {
        static void Main(string[] args)
        {
            DAL.conecta();            
            List<Livro> Livros = DAL.BuscarLivros();

            foreach(Livro livro in Livros) 
            {
                Console.WriteLine($"Código: {livro.codigo}");
                Console.WriteLine($"Titulo: {livro.titulo}");
                Console.WriteLine($"Autor: {livro.autor}");
                Console.WriteLine($"Editora: {livro.editora}");
                Console.WriteLine($"Ano: {livro.ano}");
                Console.WriteLine();
            }

            TextWriter arquivo = new StreamWriter("teste.xml");
            XmlSerializer obj = new XmlSerializer(Livros.GetType());
            obj.Serialize(arquivo, Livros);

            String json = JsonConvert.SerializeObject(Livros);            
            File.WriteAllText("teste.json", json);            

            Console.ReadKey();
            DAL.desconecta();
        }
    }
}
