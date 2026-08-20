using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClienteEnvia
{
    public class FormCliente : Form
    {
        // ─── Componentes da interface ────────────────────────────────────────
        private TextBox txtMensagem;
        private Button  btnEnviar;
        private ListBox lstHistorico;
        private Label   lblMensagem;
        private Label   lblHistorico;

        // ─── Socket UDP ──────────────────────────────────────────────────────
        private Socket    socketEnviar;
        private IPEndPoint enderecoServidor;

        // ====================================================================
        // Construtor — inicializa socket e monta a tela
        // ====================================================================
        public FormCliente()
        {
            // Configura o socket UDP uma única vez
            socketEnviar      = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Dgram,
                                           ProtocolType.IP);
            enderecoServidor  = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9060);

            InitializeComponent();
        }

        // ====================================================================
        // Monta todos os controles da janela manualmente (sem designer .resx)
        // ====================================================================
        private void InitializeComponent()
        {
            // ── Janela principal ────────────────────────────────────────────
            this.Text            = "Cliente UDP — Chat";
            this.Size            = new System.Drawing.Size(480, 420);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(30, 30, 46);
            this.ForeColor       = System.Drawing.Color.White;

            // ── Label: Histórico ────────────────────────────────────────────
            lblHistorico           = new Label();
            lblHistorico.Text      = "Mensagens enviadas:";
            lblHistorico.Location  = new System.Drawing.Point(15, 10);
            lblHistorico.Size      = new System.Drawing.Size(200, 20);
            lblHistorico.ForeColor = System.Drawing.Color.FromArgb(180, 180, 255);
            lblHistorico.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);

            // ── ListBox: acumula histórico das mensagens ────────────────────
            lstHistorico               = new ListBox();
            lstHistorico.Location      = new System.Drawing.Point(15, 35);
            lstHistorico.Size          = new System.Drawing.Size(435, 270);
            lstHistorico.BackColor     = System.Drawing.Color.FromArgb(45, 45, 65);
            lstHistorico.ForeColor     = System.Drawing.Color.FromArgb(220, 220, 255);
            lstHistorico.Font          = new System.Drawing.Font("Segoe UI", 10f);
            lstHistorico.BorderStyle   = BorderStyle.FixedSingle;
            lstHistorico.SelectionMode = SelectionMode.None;

            // ── Label: Mensagem ─────────────────────────────────────────────
            lblMensagem           = new Label();
            lblMensagem.Text      = "Digite a mensagem:";
            lblMensagem.Location  = new System.Drawing.Point(15, 320);
            lblMensagem.Size      = new System.Drawing.Size(150, 20);
            lblMensagem.ForeColor = System.Drawing.Color.FromArgb(180, 180, 255);
            lblMensagem.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);

            // ── TextBox: campo de digitação ─────────────────────────────────
            txtMensagem            = new TextBox();
            txtMensagem.Location   = new System.Drawing.Point(15, 345);
            txtMensagem.Size       = new System.Drawing.Size(320, 30);
            txtMensagem.BackColor  = System.Drawing.Color.FromArgb(45, 45, 65);
            txtMensagem.ForeColor  = System.Drawing.Color.White;
            txtMensagem.Font       = new System.Drawing.Font("Segoe UI", 11f);
            txtMensagem.BorderStyle= BorderStyle.FixedSingle;
            // Pressionar Enter também envia a mensagem
            txtMensagem.KeyDown   += new KeyEventHandler(txtMensagem_KeyDown);

            // ── Botão Enviar ────────────────────────────────────────────────
            btnEnviar              = new Button();
            btnEnviar.Text         = "Enviar";
            btnEnviar.Location     = new System.Drawing.Point(345, 343);
            btnEnviar.Size         = new System.Drawing.Size(105, 34);
            btnEnviar.BackColor    = System.Drawing.Color.FromArgb(100, 100, 220);
            btnEnviar.ForeColor    = System.Drawing.Color.White;
            btnEnviar.Font         = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            btnEnviar.FlatStyle    = FlatStyle.Flat;
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.Cursor       = Cursors.Hand;
            btnEnviar.Click       += new EventHandler(btnEnviar_Click);

            // ── Adiciona controles à janela ─────────────────────────────────
            this.Controls.Add(lblHistorico);
            this.Controls.Add(lstHistorico);
            this.Controls.Add(lblMensagem);
            this.Controls.Add(txtMensagem);
            this.Controls.Add(btnEnviar);

            // ── Garante que o socket seja fechado ao fechar a janela ────────
            this.FormClosing += new FormClosingEventHandler(FormCliente_FormClosing);
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarMensagem();
        }

        // ====================================================================
        // Evento: pressionar Enter no TextBox também envia
        // ====================================================================
        private void txtMensagem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EnviarMensagem();
                e.SuppressKeyPress = true; // evita o "bip" do sistema
            }
        }

        // ====================================================================
        // Lógica central: lê o TextBox, envia via UDP e atualiza o ListBox
        // ====================================================================
        private void EnviarMensagem()
        {
            string mensagem = txtMensagem.Text.Trim();

            if (string.IsNullOrEmpty(mensagem))
            {
                MessageBox.Show("Digite uma mensagem antes de enviar.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Converte a string para bytes ASCII e envia via UDP
                byte[] dados = Encoding.ASCII.GetBytes(mensagem);
                socketEnviar.SendTo(dados, enderecoServidor);

                // Adiciona a mensagem ao histórico com data/hora
                string registro = string.Format("[{0}] {1}",
                    DateTime.Now.ToString("HH:mm:ss"), mensagem);
                lstHistorico.Items.Add(registro);

                // Faz scroll automático para o item mais recente
                lstHistorico.TopIndex = lstHistorico.Items.Count - 1;

                // Limpa o campo de digitação e recoloca o foco nele
                txtMensagem.Clear();
                txtMensagem.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar: " + ex.Message,
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // Evento: fecha o socket ao encerrar a aplicação
        // ====================================================================
        private void FormCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (socketEnviar != null)
            {
                socketEnviar.Close();
            }
        }
    }
}
