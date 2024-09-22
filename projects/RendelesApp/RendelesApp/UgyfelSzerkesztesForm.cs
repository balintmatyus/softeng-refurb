﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RendelesApp.Data;
using RendelesApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RendelesApp
{
    public partial class UgyfelSzerkesztesForm : Form
    {
        private RendelesDbContext _context;
        private readonly Ugyfel? _ugyfel;

        private bool ujUgyfel { get { return _ugyfel == null; } }


        public UgyfelSzerkesztesForm(Ugyfel? ugyfel = null)
        {
            InitializeComponent();
            this._context = new RendelesDbContext();
            this._ugyfel = ugyfel;
        }

        private void rbLetezoCim_CheckedChanged(object sender, EventArgs e)
        {
            cbCimek.Enabled = rbLetezoCim.Checked;
        }

        private void rbUjCim_CheckedChanged(object sender, EventArgs e)
        {
            tbOrszag.Enabled =
                tbIranyitoszam.Enabled =
                tbVaros.Enabled =
                tbUtca.Enabled =
                tbHazszam.Enabled =
                rbUjCim.Checked;

            tbOrszag.CausesValidation =
                tbIranyitoszam.CausesValidation =
                tbVaros.CausesValidation =
                tbUtca.CausesValidation =
                tbHazszam.CausesValidation =
                rbUjCim.Checked;
        }

        private void UgyfelSzerkesztesForm_Load(object sender, EventArgs e)
        {
            cbCimek.DataSource = (from x in _context.Cim
                                  orderby x.Varos
                                  select new
                                  {
                                      CimId = x.CimId,
                                      CimEgyben = $"{x.Iranyitoszam}-{x.Varos}, {x.Orszag}: {x.Utca} {x.Hazszam}"
                                  }).ToList();

            cbCimek.DisplayMember = "CimEgyben";
            cbCimek.ValueMember = "CimId";

            if (_ugyfel != null)
            {
                tbNev.Text = _ugyfel.Nev;
                tbEmail.Text = _ugyfel.Email;
                tbTelefonszam.Text = _ugyfel.Telefonszam;
                cbCimek.SelectedValue = _ugyfel.LakcimId ?? _context.Cim.FirstOrDefault().CimId;
                btnMentes.Text = "Mentés";
            }
        }

        private void btnMentes_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren()) return;

            Cim? cim;

            if (rbUjCim.Checked)
            {
                // ha új cím, akkor először hozzáadjuk a címet az adatbázishoz
                cim = new Cim()
                {
                    Orszag = tbOrszag.Text,
                    Iranyitoszam = tbIranyitoszam.Text,
                    Varos = tbVaros.Text,
                    Utca = tbUtca.Text,
                    Hazszam = tbHazszam.Text
                };

                _context.Cim.Add(cim);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                int kivalasztottCimId = (int)cbCimek.SelectedValue!;
                cim = _context.Cim.Where(x => x.CimId == kivalasztottCimId).First();
            }

            if (cim != null)
            {
                if (ujUgyfel)
                {
                    Ugyfel ugyfel = new Ugyfel()
                    {
                        Nev = tbNev.Text,
                        Telefonszam = tbTelefonszam.Text,
                        Email = tbEmail.Text,
                        LakcimId = cim.CimId
                    };

                    _context.Ugyfel.Add(ugyfel);
                }

                else
                {
                    _ugyfel.Nev = tbNev.Text;
                    _ugyfel.Email = tbEmail.Text;
                    _ugyfel.Telefonszam = tbTelefonszam.Text;
                    _ugyfel.LakcimId = cim.CimId;
                    _context.Ugyfel.Update(_ugyfel);
                }

                try
                {
                    _context.SaveChanges();
                    MessageBox.Show("Sikeres mentés!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void tbNev_Validating(object sender, CancelEventArgs e)
        {
            Regex rgxNev = new Regex(@"^[a-zA-Z\s]+$");

            if (!rgxNev.IsMatch(tbNev.Text))
            {
                errorProvider1.SetError(tbNev, "A név csak kis- és nagybetűket jeleníthet meg.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbNev, "");
            }
        }

        private void tbEmail_Validating(object sender, CancelEventArgs e)
        {
            Regex rgxEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

            if (!rgxEmail.IsMatch(tbEmail.Text))
            {
                errorProvider1.SetError(tbEmail, "Nem megfelelő e-mail formátum.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbEmail, "");
            }
        }

        private void tbTelefonszam_Validating(object sender, CancelEventArgs e)
        {
            Regex rgxTelefonszam = new Regex(@"^\+36(?:20|30|31|50|70)(\d{7})$");

            if (!rgxTelefonszam.IsMatch(tbTelefonszam.Text))
            {
                errorProvider1.SetError(tbTelefonszam, "Helyes formátum: +36201234567");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbTelefonszam, "");
            }
        }

        private void tbCim_Validating(object sender, CancelEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    errorProvider1.SetError(textBox, "Nem lehet üres string!");
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1.SetError(textBox, "");
                }
            }
            
        }
    }
}
