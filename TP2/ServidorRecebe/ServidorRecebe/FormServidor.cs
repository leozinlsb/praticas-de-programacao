using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServidorRecebe
{
    public class FormServidor : Form
    {
        // ─── Componentes da interface ────────────────────────────────────────
        private ListBox lstMensagens;
        private Label   lblTitulo;
        private Label   lblStatus;

        // ─── Socket e threading ──────────────────────────────────────────────
        private Socket  socketReceber;
        private Thread  threadEscuta;
        private bool    escutando = false;

        // ====================================================================
        // Construtor
        // ====================================================================
        public FormServidor()
        {
            InitializeComponent();
        }

        // ====================================================================
        // Monta todos os controles da janela
        // ====================================================================
        private void InitializeComponent()
        {
            // ── Janela principal ────────────────────────────────────────────
            this.Text            = "Servidor UDP — Mensagens Recebidas";
            this.Size            = new System.Drawing.Size(500, 450);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(20, 28, 40);
            this.ForeColor       = System.Drawing.Color.White;

            // ── Label: título ───────────────────────────────────────────────
            lblTitulo           = new Label();
            lblTitulo.Text      = "Servidor UDP — porta 9060";
            lblTitulo.Location  = new System.Drawing.Point(15, 10);
            lblTitulo.Size      = new System.Drawing.Size(460, 26);
            lblTitulo.ForeColor = System.Drawing.Color.FromArgb(130, 200, 255);
            lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 13f, System.Drawing.FontStyle.Bold);

            // ── Label: status ───────────────────────────────────────────────
            lblStatus           = new Label();
            lblStatus.Text      = "⏳ Aguardando mensagens...";
            lblStatus.Location  = new System.Drawing.Point(15, 40);
            lblStatus.Size      = new System.Drawing.Size(460, 20);
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(100, 200, 120);
            lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9f);

            // ── ListBox: exibe as mensagens recebidas ───────────────────────
            lstMensagens               = new ListBox();
            lstMensagens.Location      = new System.Drawing.Point(15, 70);
            lstMensagens.Size          = new System.Drawing.Size(455, 340);
            lstMensagens.BackColor     = System.Drawing.Color.FromArgb(28, 38, 55);
            lstMensagens.ForeColor     = System.Drawing.Color.FromArgb(190, 235, 255);
            lstMensagens.Font          = new System.Drawing.Font("Consolas", 10f);
            lstMensagens.BorderStyle   = BorderStyle.FixedSingle;
            lstMensagens.SelectionMode = SelectionMode.None;

            // ── Adiciona controles à janela ─────────────────────────────────
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lstMensagens);

            // ── Eventos de ciclo de vida do formulário ──────────────────────
            this.Load        += new EventHandler(FormServidor_Load);
            this.FormClosing += new FormClosingEventHandler(FormServidor_FormClosing);
        }

        // ====================================================================
        // Evento: ao abrir o formulário, inicia o socket e a thread de escuta
        // ====================================================================
        private void FormServidor_Load(object sender, EventArgs e)
        {
            try
            {
                // Cria e vincula o socket UDP à porta 9060
                socketReceber = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Dgram,
                                           ProtocolType.IP);

                EndPoint endereco = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9060);
                socketReceber.Bind(endereco);

                escutando = true;

                // Inicia a escuta em uma thread separada para não travar a UI
                // IsBackground = true faz a thread encerrar automaticamente
                // quando a aplicação fechar
                threadEscuta = new Thread(LoopEscuta);
                threadEscuta.IsBackground = true;
                threadEscuta.Start();

                lblStatus.Text = "✅ Servidor ativo — aguardando mensagens na porta 9060";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao iniciar servidor: " + ex.Message,
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // Loop de escuta — roda em thread separada (NUNCA na thread da UI)
        //
        // Problema: ReceiveFrom() é BLOQUEANTE. Se rodasse na thread da UI,
        // a janela travaria completamente enquanto aguarda dados.
        //
        // Solução: rodar em background thread e usar Invoke() para comunicar
        // com a UI de forma thread-safe.
        // ====================================================================
        private void LoopEscuta()
        {
            byte[]    buffer   = new byte[1024];
            EndPoint  remetente = new IPEndPoint(IPAddress.Any, 0);

            while (escutando)
            {
                try
                {
                    // Bloqueia aqui até receber um datagrama UDP
                    int qtdBytes = socketReceber.ReceiveFrom(buffer, ref remetente);

                    // Converte os bytes recebidos para string
                    string mensagem = Encoding.ASCII.GetString(buffer, 0, qtdBytes);

                    // Monta a linha de exibição com horário e endereço de quem enviou
                    string linha = string.Format("[{0}] {1}  ← {2}",
                        DateTime.Now.ToString("HH:mm:ss"),
                        mensagem,
                        remetente.ToString());

                    // ──────────────────────────────────────────────────────
                    // REGRA FUNDAMENTAL DO WINDOWS FORMS:
                    // Controles visuais só podem ser modificados pela thread
                    // que os criou (a thread da UI). Como estamos numa thread
                    // diferente, usamos Invoke() para executar o delegate
                    // de forma segura na thread correta.
                    // ──────────────────────────────────────────────────────
                    this.Invoke(new Action(() =>
                    {
                        lstMensagens.Items.Add(linha);

                        // Scroll automático para o item mais recente
                        lstMensagens.TopIndex = lstMensagens.Items.Count - 1;

                        // Refresh() garante que o ListBox redesenhe imediatamente
                        // (recomendado pelo enunciado para visualização em tempo real)
                        lstMensagens.Refresh();

                        lblStatus.Text = string.Format(
                            "✅ {0} mensagem(ns) recebida(s)", lstMensagens.Items.Count);
                    }));
                }
                catch (SocketException)
                {
                    // O socket foi fechado (ao encerrar o app) — encerra o loop
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Idem — socket descartado ao fechar
                    break;
                }
            }
        }

        // ====================================================================
        // Evento: ao fechar o formulário, sinaliza o loop e fecha o socket
        // ====================================================================
        private void FormServidor_FormClosing(object sender, FormClosingEventArgs e)
        {
            escutando = false;

            if (socketReceber != null)
            {
                // Fechar o socket faz o ReceiveFrom() na outra thread
                // lançar SocketException, saindo do loop de escuta
                socketReceber.Close();
            }
        }
    }
}
