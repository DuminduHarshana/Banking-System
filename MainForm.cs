using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankClient
{
    public partial class MainForm : Form
    {
        TcpClient tcpClient;
        String username = "";

        public MainForm(TcpClient client, string username)
        {
            InitializeComponent();
            this.tcpClient = client;
            this.username = username;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblUsername.Text = username;

            Thread readThread = new Thread(new ThreadStart(read));
            readThread.IsBackground = true;
            readThread.Start();
        }

        public void read() {

            try
            {
                while (true)
                {
                    Stream stm = tcpClient.GetStream();

                    byte[] receiveByte = new byte[10240];

                    int k = stm.Read(receiveByte, 0, receiveByte.Length);

                    string receiveMessage = Encoding.ASCII.GetString(receiveByte);


                    //balance
                    if (receiveMessage.Contains("@Bal"))
                    {
                        int startBal = receiveMessage.IndexOf("?")+1;
                        int endBal = receiveMessage.IndexOf("#");

                        char[] arr = receiveMessage.ToCharArray();

                        string balance = "";

                        for (int i = startBal; i < endBal; i++)
                        {
                            balance = balance + arr[i];
                        }

                        showMessageBox("Account Balance : Rs." + balance);
                    }

                    //@Dep@Success


                }
            }
            catch (Exception)
            {

           
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void btnBalance_Click(object sender, EventArgs e)
        {
            string message = "@Check@Bal@User?" + username +"#";

            byte[] send = Encoding.ASCII.GetBytes(message);

            Stream stm = tcpClient.GetStream();

            stm.Write(send, 0, send.Length);

            stm.Flush();
        }

        public void sendMessageToServer(string message) {


            byte[] send = Encoding.ASCII.GetBytes(message);

            Stream stm = tcpClient.GetStream();

            stm.Write(send, 0, send.Length);

            stm.Flush();
        }


        public void showMessageBox(string message)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(message);
                });
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            string value = "";

            if (InputBox("Deposit", "Enter Amount", ref value) == DialogResult.OK)
            {
                double amount = Double.Parse(value);

                //@Depo?100@User?dumindu#
                string message = "@Depo?"+ amount + "@User?" + username + "#";

                sendMessageToServer(message);

            }
        }



        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            string value = "";

            if (InputBox("Withdraw", "Enter Amount", ref value) == DialogResult.OK)
            {
                double amount = Double.Parse(value);

                //@With?100@User?dumindu#
                string message = "@With?" + amount + "@User?" + username + "#";


                byte[] send = Encoding.ASCII.GetBytes(message);

                Stream stm = tcpClient.GetStream();

                stm.Write(send, 0, send.Length);

                stm.Flush();

            }
        }
    }
}
