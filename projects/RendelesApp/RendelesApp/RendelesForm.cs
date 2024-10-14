using Microsoft.EntityFrameworkCore;
using RendelesApp.Data;
using RendelesApp.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RendelesApp
{
    public partial class RendelesForm : Form
    {
        private readonly RendelesDbContext _context;
        private const decimal AFA = .27m;

        private Ugyfel? KivalasztottUgyfel => lbUgyfelek.SelectedItem as Ugyfel;
        private Cim? KivalasztottCim => cbCimek.SelectedItem as Cim;
        private Rendeles? KivalasztottRendeles => lbRendeles.SelectedItem as Rendeles;
        private Termek? KivalasztottTermek => lbTermek.SelectedItem as Termek;

        public RendelesForm()
        {
            InitializeComponent();
            _context = new RendelesDbContext();
        }

        private void RendelesForm_Load(object sender, EventArgs e)
        {
            LoadData();
            SetupDataBindings();
        }

        private void SetupDataBindings()
        {
            Binding binding = txtKedvezmeny.DataBindings.Add("Text", rendelesBindingSource, "Kedvezmeny", true, DataSourceUpdateMode.OnPropertyChanged);
            binding.FormattingEnabled = true;
            binding.Format += Binding_Format;
            binding.Parse += Binding_Parse;

            binding.ReadValue();
        }

        private void LoadData()
        {
            LoadUgyfelek();
            cimBindingSource.DataSource = _context.Cim.ToList();
            termekBindingSource.DataSource = _context.Termek.ToList();
        }

        private void LoadUgyfelek()
        {
            var q = from x in _context.Ugyfel
                    where x.Nev.Contains(txtSzuro.Text) || x.Email.Contains(txtSzuro.Text)
                    orderby x.Nev
                    select x;

            ugyfelBindingSource.DataSource = q.ToList();

            ugyfelBindingSource.ResetCurrentItem();
        }

        private void LoadRendelesek()
        {
            if (KivalasztottUgyfel == null) return;

            dgvTetelek.DataSource = null;

            var rendeles = from x in _context.Rendeles
                           where x.UgyfelId == KivalasztottUgyfel.UgyfelId
                           select x;

            rendelesBindingSource.DataSource = rendeles.ToList();
            lbRendeles.DataSource = rendelesBindingSource;

            if (lbRendeles.Items.Count > 0)
            {
                lbRendeles.SelectedIndex = 0;
            }

            rendelesBindingSource.ResetCurrentItem();
        }

        private void txtSzuro_TextChanged(object sender, EventArgs e)
        {
            LoadUgyfelek();
        }

        private void lbUgyfelek_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRendelesek();
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
            if (KivalasztottUgyfel == null)
            {
                return;
            }

            var cim = KivalasztottUgyfel.Lakcim ?? _context.Cim.FirstOrDefault();

            if (cim == null)
            {
                MessageBox.Show("Nincs cím megadva.");
                return;
            }

            var ujRendeles = new Rendeles()
            {
                UgyfelId = KivalasztottUgyfel.UgyfelId,
                SzallitasiCimId = cim.CimId,
                RendelesDatum = DateTime.Now,
                Kedvezmeny = 0,
                Vegosszeg = 0,
                Statusz = "Feldolgozás alatt"
            };

            _context.Rendeles.Add(ujRendeles);
            Mentés();

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

            if (KivalasztottRendeles == null || KivalasztottTermek == null)
            {
                MessageBox.Show("Nincs kiválasztva rendelés vagy termék!");
                return;
            }

            decimal bruttoAr = KivalasztottTermek.AktualisAr * (1 + AFA);

            var ujTetel = new RendelesTetel
            {
                RendelesId = KivalasztottRendeles.RendelesId,
                TermekId = KivalasztottTermek.TermekId,
                Mennyiseg = mennyiseg,
                EgysegAr = KivalasztottTermek.AktualisAr,
                Afa = AFA,
                NettoAr = KivalasztottTermek.AktualisAr * mennyiseg,
                BruttoAr = bruttoAr
            };

            _context.RendelesTetel.Add(ujTetel);
            Mentés();

            LoadRendelesTetel();

            UpdateVegosszeg();
        }

        private void LoadRendelesTetel()
        {
            if (KivalasztottRendeles == null) return;

            var q = from rt in _context.RendelesTetel
                    where rt.RendelesId == KivalasztottRendeles.RendelesId
                    select new RendelesTetelDTO
                    {
                        TetelId = rt.TetelId,
                        TermekNev = rt.Termek.Nev,
                        Mennyiseg = rt.Mennyiseg,
                        EgysegAr = rt.EgysegAr,
                        Afa = rt.Afa,
                        NettoAr = rt.NettoAr,
                        BruttoAr = rt.BruttoAr
                    };

            dgvTetelek.DataSource = q.ToList();
        }

        private void UpdateVegosszeg()
        {
            if (KivalasztottRendeles == null) return;

            var vegosszeg = _context.RendelesTetel
                .Where(rt => rt.RendelesId == KivalasztottRendeles.RendelesId)
                .Sum(rt => rt.Mennyiseg * rt.BruttoAr);

            KivalasztottRendeles.Vegosszeg = vegosszeg * (1 - KivalasztottRendeles.Kedvezmeny);

            Mentés();

            rendelesBindingSource.ResetBindings(false);
        }

        private void btnMentes_Click(object sender, EventArgs e)
        {
            //if (KivalasztottCim == null || KivalasztottRendeles == null || cbStatusz.SelectedItem == null)
            //{
            //    return;
            //}

            //KivalasztottRendeles.SzallitasiCimId = KivalasztottCim.CimId;
            //KivalasztottRendeles.Statusz = cbStatusz.SelectedItem.ToString()!;

            Mentés();
        }

        private void btnTorol_Click(object sender, EventArgs e)
        {
            if (dgvTetelek.SelectedRows.Count == 0)
            {
                MessageBox.Show("Nincs kiválasztva tétel!");
                return;
            }

            var selectedTetel = dgvTetelek.SelectedRows[0].DataBoundItem as RendelesTetelDTO;

            var tetel = (from rt in _context.RendelesTetel
                         where rt.TetelId == selectedTetel!.TetelId
                         select rt).FirstOrDefault();

            if (tetel != null)
            {
                _context.RendelesTetel.Remove(tetel);
                Mentés();

                LoadRendelesTetel();
                UpdateVegosszeg();
            }
        }

        private void lbRendeles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRendelesTetel();

            UpdateVegosszeg();
        }

        void Mentés()
        {
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

    public class RendelesTetelDTO
    {
        public int TetelId { get; set; }
        public string? TermekNev { get; set; }
        public int Mennyiseg { get; set; }
        public decimal EgysegAr { get; set; }
        public decimal Afa { get; set; }
        public decimal NettoAr { get; set; }
        public decimal BruttoAr { get; set; }
    }
}
