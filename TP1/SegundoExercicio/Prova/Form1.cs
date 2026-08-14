using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
                
            radioButton1.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
            radioButton2.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarGrafico();
        }

        private void AtualizarGrafico()
        {
            if (chart1.DataSource != null && chart1.Series.Count > 0)
            {
                if (radioButton1.Checked)
                {
                    chart1.Series[0].YValueMembers = "Toneladas";
                    chart1.Series[0].Name = "Toneladas";
                }
                else if (radioButton2.Checked)
                {
                    chart1.Series[0].YValueMembers = "Valor";
                    chart1.Series[0].Name = "Valor";
                }
                chart1.DataBind();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            BLL.desconecta();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Limpar os campos antes da consulta
            textBox2.Clear();
            textBox3.Clear();
            if (chart1.Series.Count > 0)
                chart1.Series.Clear();

            Cliente.setCNPJ(textBox1.Text);
            BLL.validaDados();

            if (Erro.getErro())
            {
                MessageBox.Show(Erro.getMsg());
            }
            else
            {
                textBox2.Text = Cliente.getNome();
                
                double totalToneladas = DAL.consultaTotalToneladas();
                double totalValor = DAL.consultaTotalValor();
                
                // Exibe as duas informações na textBox3 como solicitado (ou formatado)
                textBox3.Text = $"Ton: {totalToneladas:N2} | R$: {totalValor:N2}";

                System.Data.DataTable dt = DAL.consultaDadosGrafico();
                if (dt != null)
                {
                    chart1.Series.Add("Vendas");
                    chart1.DataSource = dt;
                    chart1.Series[0].XValueMember = "Data";
                    
                    AtualizarGrafico();
                }
            }
        }
    }
}
