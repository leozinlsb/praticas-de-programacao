using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prova
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BLL.conecta();
            if (Erro.getErro())
                MessageBox.Show(Erro.getMsg());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            BLL.desconecta();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Limpar os campos antes da consulta
            textBox2.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox3.Clear();
            textBox4.Clear();

            Cliente.setCNPJ(textBox1.Text);
            BLL.validaDados();

            if (Erro.getErro())
            {
                MessageBox.Show(Erro.getMsg());
            }
            else
            {
                textBox2.Text = Cliente.getNome();
                
                System.Data.OleDb.OleDbDataReader reader = BLL.consultaVendasCliente();
                double totalToneladas = 0;
                double totalValor = 0;

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        string dataCompra = reader["Data"].ToString();
                        string toneladasStr = reader["Toneladas"].ToString();
                        string valorStr = reader["Valor"].ToString();

                        // Opcional: tentar formatar a data caso venha com horas, etc.
                        if (DateTime.TryParse(dataCompra, out DateTime dt))
                        {
                            dataCompra = dt.ToString("dd/MM/yyyy");
                        }

                        listBox1.Items.Add(dataCompra);
                        listBox2.Items.Add(toneladasStr);
                        listBox3.Items.Add(valorStr);

                        if (double.TryParse(toneladasStr, out double tons))
                        {
                            totalToneladas += tons;
                        }
                        if (double.TryParse(valorStr, out double val))
                        {
                            totalValor += val;
                        }
                    }
                    reader.Close();
                }

                textBox3.Text = totalToneladas.ToString("N2");
                textBox4.Text = totalValor.ToString("N2");
            }
        }
    }
}
