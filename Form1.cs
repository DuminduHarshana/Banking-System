using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BankClient
{
    public partial class Form1 : Form
    {
        string username;


        //instance of tcpclient
        TcpClient tcpClient = new TcpClient();

        string reg = "@Reg";
        string log = "@Log";
        string use = "@User?";
        string pas = "@Pass?";

        public Form1()
        {
            InitializeComponent();
        }


        public void authentication(string type, string ip, int port, string username, string password)
        {

            try
            {

                //connecting to the server
                tcpClient.Connect(ip, port);


                string message = "";

                if (type == "Login")
                {
                    message = log;

                }
                else if (type == "Register")
                {
                    message = reg;
                }

                message = message + use + username + pas + password;

                //convert json string into byte for stream
                byte[] send = Encoding.ASCII.GetBytes(message);


                //open stream
                Stream stm = tcpClient.GetStream();

                //send message to server
                stm.Write(send, 0, send.Length);

                //flush the stream
                stm.Flush();

                Thread thread = new Thread(new ThreadStart(serverReceive));
                thread.IsBackground = true;
                thread.Start();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }

        private void serverReceive()
        {
            try
            {
                while (true)
                {
                    Stream stm = tcpClient.GetStream();

                    byte[] receiveByte = new byte[10240];

                    int k = stm.Read(receiveByte, 0, receiveByte.Length);

                    string receiveMessage = Encoding.ASCII.GetString(receiveByte);


                    if (receiveMessage.Contains("@Reg@Success"))
                    {

                        MessageBox.Show("Successfully Registered, Restart App!");
                        Application.Restart();

                    }
                    else if (receiveMessage.Contains("@Log@Success"))
                    {


                        MessageBox.Show("User Login Completed!");
                        showMain(tcpClient, username);
                        
                        
                    }
                    else if (receiveMessage.Contains("@Log@Unsuccess"))
                    {
                        MessageBox.Show("User name or password incorrect!");
                        Application.Restart();
                    }


                }
            }
            catch (Exception)
            {
            }
        }

        public void showMain(TcpClient tcp, string username)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    new MainForm (tcp, username).Show();
                    Hide();
                });
            }
            else
            {
                new MainForm (tcp, username).Show();
                Hide();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //send login information to server
            validate("Login");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            //send register information to server
            validate("Register");
        }

        public void validate(string type)
        {

            //get ip from text box
            string ip = txtIP.Text.Trim();
            //get port from text box
            int port = 8080;
            //get username from text box
            username = txtUsername.Text.Trim();
            //get password from text box
            string password = txtPassword.Text.Trim();

            //pass data to server
            authentication(type, ip, port, username, password);
        }

    }
}
