using Microsoft.EntityFrameworkCore;
using RendelesApp.Data;
using RendelesApp.Models;
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
    public partial class UgyfelKezeloForm : Form
    {
        private RendelesDbContext _context;
        private BindingList<Ugyfel>? ugyfelBindingList;

        public UgyfelKezeloForm()
        {
            InitializeComponent();
            _context = new RendelesDbContext();
            _context.Ugyfel.Load();
        }

        private void UgyfelKezeloForm_Load(object sender, EventArgs e)
        {
            ugyfelBindingList = _context.Ugyfel.Local.ToBindingList();
            ugyfelBindingSource.DataSource = ugyfelBindingList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UgyfelSzerkesztesForm ujUgyfelForm = new UgyfelSzerkesztesForm();
            if (ujUgyfelForm.ShowDialog() == DialogResult.OK) {
                _context.Ugyfel.Load();
                dataGridView1.Update();
                dataGridView1.Refresh();
            };

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filterString = textBox1.Text.ToLower();

            ugyfelBindingSource.DataSource = from u in ugyfelBindingList
                                             where u.Nev.ToLower().Contains(filterString) ||
                                             u.Email.ToLower().Contains(filterString) ||
                                             (u.Telefonszam != null && u.Telefonszam.Contains(filterString))
                                             orderby u.UgyfelId
                                             select u;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].DataBoundItem is Ugyfel ugyfel)
            {
                UgyfelSzerkesztesForm szerkesztesForm = new UgyfelSzerkesztesForm(ugyfel);
                if (szerkesztesForm.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Refresh();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows[0].DataBoundItem is Ugyfel ugyfel)
            {
                _context.Ugyfel.Remove(ugyfel);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
