namespace RendelesApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TermekKategoriaForm termekKategoriaForm = new TermekKategoriaForm();
            termekKategoriaForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UgyfelKezeloForm kezeloForm = new();
            kezeloForm.ShowDialog();
        }
    }
}
