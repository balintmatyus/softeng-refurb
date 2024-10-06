using Microsoft.EntityFrameworkCore;
using RendelesApp.Data;
using RendelesApp.Models;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RendelesApp
{
    public partial class RendelesForm : Form
    {
        private RendelesDbContext _context;

        private Ugyfel? _kivalasztottUgyfel => lbUgyfelek.SelectedItem as Ugyfel;

        private TermekKategoria? _kivalasztottTermekKategoria => termekKategoriaTreeView1.KivalasztottKategoria;

        private Cim? _kivalasztottCim
        {
            get
            {
                if (cbCimek.SelectedValue != null)
                {
                    int selectedCimId = (int)cbCimek.SelectedValue;
                    return _context.Cim.FirstOrDefault(x => x.CimId == selectedCimId);
                }
                return null;
            }
        }

        private Rendeles? _kivalasztottRendeles => lbRendeles.SelectedItem as Rendeles;

        private Termek? _kivalasztottTermek => lbTermek.SelectedItem as Termek;

        private decimal _afa = .27m;

        public RendelesForm()
        {
            InitializeComponent();
            _context = new RendelesDbContext();
        }

        private void LoadData()
        {
            LoadUgyfelek();
            LoadTermekKategoriak();
            LoadCimek();
        }

        private void LoadTermekKategoriak()
        {
            var termekKategoriak = _context.TermekKategoria.ToList();
            termekKategoriaTreeView1.LoadData(termekKategoriak);
        }

        private void LoadUgyfelek()
        {
            var q = from x in _context.Ugyfel
                    where x.Nev.Contains(txtSzuro.Text) || x.Email.Contains(txtSzuro.Text)
                    orderby x.Nev
                    select x;

            ugyfelBindingSource.DataSource = q.ToList();

            lbUgyfelek.ValueMember = "UgyfelId";
            lbUgyfelek.DisplayMember = "Nev";
        }

        private void LoadCimek()
        {
            var cimek = _context.Cim.ToList();

            cimBindingSource.DataSource = cimek;

            cbCimek.ValueMember = "CimId";
            cbCimek.DisplayMember = "CimEgyben";
        }

        private void txtSzuro_TextChanged(object sender, EventArgs e)
        {
            LoadUgyfelek();
        }

        private void lbUgyfelek_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRendelesek();
        }

        private void LoadRendelesek()
        {
            if (_kivalasztottUgyfel == null) return;

            var q = from x in _context.Rendeles
                    where x.UgyfelId == _kivalasztottUgyfel.UgyfelId
                    select x;

            rendelesBindingSource.DataSource = q.ToList();

            lbRendeles.ValueMember = "RendelesId";
            lbRendeles.DisplayMember = "RendelesDatum";

            if (lbRendeles.Items.Count > 0)
            {
                lbRendeles.SelectedIndex = 0;
            }

            dgvTetelek.DataSource = null;
        }

        private void termekKategoriaTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_kivalasztottTermekKategoria == null) return;

            var kivalasztottKategoriak = termekKategoriaTreeView1
                .KivalasztottKategoriak(_kivalasztottTermekKategoria)
                .Select(k => k.KategoriaId)
                .ToList();

            var q = from x in _context.Termek
                    where x.KategoriaId != null && kivalasztottKategoriak.Contains((int)x.KategoriaId)
                    orderby x.Nev
                    select x;

            termekBindingSource.DataSource = q.ToList();

            lbTermek.DisplayMember = "Nev";
            lbTermek.ValueMember = "TermekId";
        }

        private void RendelesForm_Load(object sender, EventArgs e)
        {
            LoadData();

            txtKedvezmeny.DataBindings.Add("Text", rendelesBindingSource, "Kedvezmeny", true, DataSourceUpdateMode.OnPropertyChanged);
            Binding binding = txtKedvezmeny.DataBindings["Text"];
            binding.FormattingEnabled = true;
            binding.Format += Binding_Format;
            binding.Parse += Binding_Parse;

            binding.ReadValue();
        }

        private void Binding_Parse(object? sender, ConvertEventArgs e)
        {
            if (e.DesiredType != typeof(decimal)) return;

            string stringValue = e.Value?.ToString()?.Replace("%", "").Trim() ?? "0";
            if (decimal.TryParse(stringValue, out decimal result))
            {
                e.Value = result / 100m;
            }
        }

        private void Binding_Format(object? sender, ConvertEventArgs e)
        {
            if (e.DesiredType != typeof(string)) return;

            if (e.Value is decimal value)
            {
                e.Value = value.ToString("P");
            }
        }

        private void btnUjRendeles_Click(object sender, EventArgs e)
        {
            if (_kivalasztottUgyfel == null)
            {
                return;
            }

            var cim = _kivalasztottUgyfel.Lakcim ?? _context.Cim.FirstOrDefault();

            if (cim == null)
            {
                MessageBox.Show("Nincs cím megadva.");
                return;
            }

            var ujRendeles = new Rendeles()
            {
                UgyfelId = _kivalasztottUgyfel.UgyfelId,
                SzallitasiCimId = cim.CimId,
                RendelesDatum = DateTime.Now,
                Kedvezmeny = 0,
                Vegosszeg = 0,
                Statusz = "Feldolgozás alatt"
            };

            _context.Rendeles.Add(ujRendeles);
            _context.SaveChanges();

            lblRendelesLabel.Text = $"A rendelés teljes értéke: {ujRendeles.Vegosszeg} Ft";

            LoadRendelesek();

            lbRendeles.SelectedItem = ujRendeles;
        }

        private void btnHozzaad_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMennyiseg.Text, out int mennyiseg) || mennyiseg <= 0)
            {
                MessageBox.Show("Rossz mennyiség!");
                return;
            }

            if (_kivalasztottRendeles == null || _kivalasztottTermek == null)
            {
                MessageBox.Show("Nincs kiválasztva rendelés vagy termék!");
                return;
            }

            decimal bruttoAr = _kivalasztottTermek.AktualisAr * (1 + _afa);

            var ujTetel = new RendelesTetel
            {
                RendelesId = _kivalasztottRendeles.RendelesId,
                TermekId = _kivalasztottTermek.TermekId,
                Mennyiseg = mennyiseg,
                EgysegAr = _kivalasztottTermek.AktualisAr,
                Afa = _afa,
                NettoAr = _kivalasztottTermek.AktualisAr * mennyiseg,
                BruttoAr = bruttoAr
            };

            _context.RendelesTetel.Add(ujTetel);
            _context.SaveChanges();

            LoadRendelesTetel();

            UpdateVegosszeg();
        }

        private void LoadRendelesTetel()
        {
            if (_kivalasztottRendeles == null) return;

            var q = from rt in _context.RendelesTetel
                    where rt.RendelesId == _kivalasztottRendeles.RendelesId
                    select rt;

            dgvTetelek.DataSource = q.ToList();
        }

        private void UpdateVegosszeg()
        {
            if (_kivalasztottRendeles == null) return;

            var vegosszeg = _context.RendelesTetel
                .Where(rt => rt.RendelesId == _kivalasztottRendeles.RendelesId)
                .Sum(rt => rt.Mennyiseg * rt.BruttoAr);

            _kivalasztottRendeles.Vegosszeg = vegosszeg * (1 - _kivalasztottRendeles.Kedvezmeny);

            _context.SaveChanges();

            rendelesBindingSource.ResetBindings(false);
        }

        private void btnMentes_Click(object sender, EventArgs e)
        {
            if (_kivalasztottCim == null || _kivalasztottRendeles == null || cbStatusz.SelectedItem == null)
            {
                return;
            }

            _kivalasztottRendeles.SzallitasiCimId = _kivalasztottCim.CimId;
            _kivalasztottRendeles.Statusz = cbStatusz.SelectedItem.ToString()!;

            _context.SaveChanges();

            MessageBox.Show("Rendelés sikeresen mentve.");
        }

        private void btnTorol_Click(object sender, EventArgs e)
        {
            if (dgvTetelek.SelectedRows.Count == 0)
            {
                MessageBox.Show("Nincs kiválasztva tétel!");
                return;
            }

            var selectedTetel = dgvTetelek.SelectedRows[0].DataBoundItem as RendelesTetel;

            var tetel = (from rt in _context.RendelesTetel
                         where rt.TetelId == selectedTetel!.TetelId
                         select rt).FirstOrDefault();

            if (tetel != null)
            {
                _context.RendelesTetel.Remove(tetel);
                _context.SaveChanges();

                LoadRendelesTetel();
                UpdateVegosszeg();
            }
        }

        private void lbRendeles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRendelesTetel();

            UpdateVegosszeg();
        }
    }
}
