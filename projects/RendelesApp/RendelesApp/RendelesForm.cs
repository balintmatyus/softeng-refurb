using Microsoft.EntityFrameworkCore;
using RendelesApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RendelesApp
{
    public partial class RendelesForm : Form
    {
        RendelesDbContext _context;

        public RendelesForm()
        {
            InitializeComponent();
            _context = new RendelesDbContext();
            LoadData();
        }

        public void LoadData()
        {
            // Ügyfelek

            _context.Ugyfel.Load();
            ugyfelBindingSource.DataSource = from x in _context.Ugyfel
                                             where textBox1.Text.Contains(x.Nev) || textBox1.Text.Contains(x.Email)
                                             select new { x, Nev_email = $"{x.Nev} ({x.Email})" };

            listBox1.DisplayMember = "Nev_email";
            listBox1.ValueMember = "UgyfelId";

            // Termékek

            _context.Termek.Load();
            termekBindingSource.DataSource = from x in _context.Termek
                                             where textBox2.Text.Contains(x.Nev) || textBox2.Text.Contains(x.Leiras ?? string.Empty)
                                             select x;

            listBox2.DisplayMember = "Nev";
            listBox2.ValueMember = "TermekId";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
