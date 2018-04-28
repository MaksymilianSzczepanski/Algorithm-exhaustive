using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kw.Combinatorics;
using DaneZPliku;

namespace DaneZPlikuOkienko
{

    public partial class DaneZPliku : Form
    {
        private string[][] systemDecyzyjny;
        public string r = "";
        public DaneZPliku()
        {
            InitializeComponent();
        }

        private void btnWybierzPlik_Click(object sender, EventArgs e)
        {
            DialogResult wynikWyboruPliku = ofd.ShowDialog(); // wybieramy plik
            if (wynikWyboruPliku != DialogResult.OK)
                return;

            tbSciezkaDoSystemuDecyzyjnego.Text = ofd.FileName;
            string trescPliku = System.IO.File.ReadAllText(ofd.FileName); // wczytujemy treść pliku do zmiennej
            string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' }); // treść pliku dzielimy wg znaku końca linii, dzięki czemu otrzymamy każdy wiersz w oddzielnej komórce tablicy
            systemDecyzyjny = new string[wiersze.Length][];   // Tworzymy zmienną, która będzie przechowywała wczytane dane. Tablica będzie miała tyle wierszy ile wierszy było z wczytanego poliku

            for (int i = 0; i < wiersze.Length; i++)
            {
                string wiersz = wiersze[i].Trim();     // przypisuję i-ty element tablicy do zmiennej wiersz
                string[] cyfry = wiersz.Split(new char[] { ' ' });   // dzielimy wiersz po znaku spacji, dzięki czemu otrzymamy tablicę cyfry, w której każda oddzielna komórka to czyfra z wiersza
                systemDecyzyjny[i] = new string[cyfry.Length];    // Do tablicy w której będą dane finalne dokładamy wiersz w postaci tablicy integerów tak długą jak długa jest tablica cyfry, czyli tyle ile było cyfr w jednym wierszu
                for (int j = 0; j < cyfry.Length; j++)
                {
                    string cyfra = cyfry[j].Trim(); // przypisuję j-tą cyfrę do zmiennej cyfra
                    systemDecyzyjny[i][j] = cyfra;  
                }
            }

            tbSystemDecyzyjny.Text = TablicaDoString(systemDecyzyjny);
        }

        public string TablicaDoString<T>(T[][] tab)
        {
            string wynik = "";
            for (int i = 0; i < tab.Length; i++)
            {
                for (int j = 0; j < tab[i].Length; j++)
                {
                    wynik += tab[i][j].ToString() + " ";
                }
                wynik = wynik.Trim() + Environment.NewLine;
            }

            return wynik;
        }


        public double StringToDouble(string liczba)
        {
            double wynik; liczba = liczba.Trim();
            if (!double.TryParse(liczba.Replace(',', '.'), out wynik) && !double.TryParse(liczba.Replace('.', ','), out wynik))
                throw new Exception("Nie udało się skonwertować liczby do double");

            return wynik;
        }


        public int StringToInt(string liczba)
        {
            int wynik;
            if (!int.TryParse(liczba.Trim(), out wynik))
                throw new Exception("Nie udało się skonwertować liczby do int");

            return wynik;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Tablica z wczytanymi danymi dostępna poniżej
            // this.systemDecyzyjny;

            // 
            // Przykład konwersji string to double 
            string sLiczbaDouble = "1.5";
            double dsLiczbaDouble = StringToDouble(sLiczbaDouble);

            // Przykład konwersji string to int 
            string sLiczbaInt = "1";
            int iLiczbaInt = StringToInt(sLiczbaInt);

            /****************** Miejsce na rozwiązanie *********************************/
            List<Regula> reguly = new List<Regula>();
            int[] kombinacja;
            
            for (int rzad = 1; rzad < systemDecyzyjny[0].Length - 1; rzad++)
            {
                int pom = 0;
                foreach (var macierz in macierzNieodruznialnosci(systemDecyzyjny))
                {                   
                        foreach (Combination combo in new Combination(systemDecyzyjny[0].Length - 1, rzad).GetRows())
                        {
                            kombinacja = combo.ToArray();                 
                            if (!czyKombinacjaWWierszu(macierz, kombinacja))
                            {
                                Regula regula = new Regula(obiekt(pom), kombinacja);
                                                               

                                if (!regula.czyRegulaZawieraReguleZListy(reguly))
                                {                                  
                                    regula.SupportReguly(systemDecyzyjny);                                  
                                    reguly.Add(regula);
                                }
                            }                                                
                    }
                    pom++;                    
                }
            }


            //wyswietl(reguly);
            foreach (var reg in reguly)
            {
                wynik.Text += reg.ToString();
            }

            /****************** Koniec miejsca na rozwiązanie ********************************/
        }

        int[][][] macierzNieodruznialnosci(string[][] system)
        {
            int[][][] macierz = new int[system.Length][][];
            for (int i = 0; i < macierz.Length; i++)
            {
                macierz[i] = new int[system.Length][];
                for (int j = 0; j < system.Length; j++)
                {
                    macierz[i][j] = KomorkaMacierzy(system[i], system[j]);
                }
            }
            return macierz;
        }
        int[] KomorkaMacierzy(string[] ob1, string[] ob2)
        {
            List<int> komorka = new List<int>();
            if (ob1.Last() == ob2.Last())
                return komorka.ToArray();
            for (int i = 0; i < ob1.Length - 1; i++)
            {
                if (ob1[i] == ob2[i])
                    komorka.Add(i);
            }
            return komorka.ToArray();

        }

        private void wynik_TextChanged(object sender, EventArgs e)
        {
            
        }

        bool czyKombinacjaWKomorce(int[] komorka, int[] kombinacja)
        {
            
            for (int i = 0; i < kombinacja.Length; i++)
            {
                if (!komorka.Contains(kombinacja[i]))
                {
                    return false;
                }
            }
            return true;
        }
        bool czyKombinacjaWWierszu(int[][] wiersz,int[] kombinacje)
        {
            for (int i = 0; i < wiersz.Length; i++)
            {
                if(czyKombinacjaWKomorce(wiersz[i],kombinacje))
                {
                    return true;
                }
            }
            return false;
        }
                 
       public string[] obiekt(int nr)
       {
           int pom = 0;
           string[] ob = new string[systemDecyzyjny[0].Length-1];
           foreach (var sys in systemDecyzyjny)
           {
               if(pom==nr)
               {
                   ob = sys;
               }
               pom++;
           }
           return ob;
       }
                    
    }
}
