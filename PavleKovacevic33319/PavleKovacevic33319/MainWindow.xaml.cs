using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PavleKovacevic33319
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext SL = new DataClasses1DataContext();
        public MainWindow()
        {
            InitializeComponent();
            PuniGrid();
            gbBaksis.IsEnabled = false;
            gbIzmenaTretmana.IsEnabled = false;
        }
        void PuniGrid()
        {
            try
            {
                var podaci = from p in SL.Tretmans
                             join k in SL.Klijents on p.KlijentID equals k.KlijentID
                             join r in SL.Radniks on p.RadnikID equals r.RadnikID

                             select new { p.TretmanID, p.Datum, p.Cena, p.Opis, p.KlijentID, p.RadnikID, RadnikIme = r.Ime + "" + r.Prezime, KlijentIme = k.Ime + "" + k.Prezime };
                DataGrid1.ItemsSource = podaci;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska" + ex.Message);
            }
        }


        private void btnIzracunaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbCenaTretmana.Text))
                {
                    MessageBox.Show("Morate odabrati tretman iz tabele");
                    return;
                }

                if (CmbProcenat.SelectedItem == null)
                {
                    MessageBox.Show("Morate izabrati procenat");
                    return;
                }

                // Najjednostavniji način - koristite Text property
                string selectedValue = CmbProcenat.Text;
                decimal procenat = decimal.Parse(selectedValue);
                decimal cena = decimal.Parse(tbCenaTretmana.Text);
                decimal baksis = cena * procenat / 100;

                tbBaksis.Text = baksis.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }
        private void MenuItemIzracunajBaksis_Click(object sender, RoutedEventArgs e)
        {
            gbBaksis.IsEnabled = true;
            if (DataGrid1.SelectedItem == null)
            {
                MessageBox.Show("Izaberi tretman iz tabele");
                return;
            }
        }

        private void DataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid1.SelectedItem != null)
            {
                dynamic selektovaniTretman = DataGrid1.SelectedItem;

                lbRadnikID.Content = "RadnikID: " + selektovaniTretman.RadnikID;
                lbKlijentID.Content = "KlijentID: " + selektovaniTretman.KlijentID;
            }
        }
        private void btnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            //Validacija
            if (string.IsNullOrWhiteSpace(tbOpis.Text))
            {
                MessageBox.Show("Morate uneti opis tretmana");
                return;
            }
            if (string.IsNullOrWhiteSpace(tbCena.Text))
            {
                MessageBox.Show("Unesi cenu");
                return;
            }
            if (!int.TryParse(tbCena.Text, out int cena))
            {
                MessageBox.Show("Cena mora biti broj");
                return;
            }
            //Kraj validacije

            Tretman gg = (from s in SL.Tretmans
                          where s.TretmanID == int.Parse(tbTretmanID.Text)
                          select s).Single();
            gg.Opis = tbOpis.Text;
            gg.Cena = int.Parse(tbCena.Text);
            try
            {
                SL.SubmitChanges();
                MessageBox.Show("Uspesno ste sacuvali izmene");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska" + ex.Message);
            }
        }

        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            gbIzmenaTretmana.IsEnabled = true;
        }

        private void btnNajduziStaz_Click(object sender, RoutedEventArgs e)
        {
            var max = SL.Radniks.Max(s => s.Staz);
            var radnik = SL.Radniks.FirstOrDefault(a => a.Staz == max);
            if (radnik != null)
            {
                lbRadnikID.Content = $"{radnik.Ime}{radnik.Prezime}";
                lbKlijentID.Content = $"Staž: {radnik.Staz} Godina";
                gbDetaljiTretmana.Visibility = Visibility.Visible;
            }
            else
            {
                lbRadnikID.Content = "Nema radnika";
                lbKlijentID.Content = "";
                gbDetaljiTretmana.Visibility = Visibility.Visible;
            }
        }

        private void btnNajkraciStaz_Click(object sender, RoutedEventArgs e)
        {
            var min = SL.Radniks.Min(s => s.Staz);
            var radnik = SL.Radniks.FirstOrDefault(a => a.Staz == min);
            if (radnik != null)
            {
                lbRadnikID.Content = $"{radnik.Ime}{radnik.Prezime}";
                lbKlijentID.Content = $"Staž{radnik.Staz}Godina";
                gbDetaljiTretmana.Visibility = Visibility.Visible;
            }
            else
            {
                lbRadnikID.Content = "Nema radnika";
                lbKlijentID.Content = "";
                gbDetaljiTretmana.Visibility = Visibility.Visible;
            }
        }

        private void MenuItemOtkazi_Click(object sender, RoutedEventArgs e)
        {
            Otkazi();
        }
        void Otkazi()
        {
            tbBaksis.Text = "";
            tbCena.Text = "";
            tbCenaTretmana.Text = "";
            tbOpis.Text = "";
            tbTretmanID.Text = "";
            CmbProcenat.SelectedItem = null;
        }

      
    }
}

