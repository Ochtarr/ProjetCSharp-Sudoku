using Ivan.Projet.Sudoku.Configuration;
using SudokuApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Ivan.Projet.Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Grille> listGrille = new List<Grille>();

        public MainWindow()
        {
            InitializeComponent();
            getAllGames(@"C:\Users\Florian\Documents\Visual Studio 2013\Projects\Ivan.Projet.Sudoku\Ivan.Projet.Sudoku\Configuration\Sudokus à Résoudre.sud");

        }
        public void getAllGames(string _path)
        {
            StreamReader sr = new StreamReader(_path);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                if ("//---------------------------".Equals(line) || "//------------------------------------------".Equals(line))
                {
                    Grille oG = new Grille();
                    // name
                    line = sr.ReadLine();
                    oG.Name = line;
                    // Affichage de la liste des grilles
                    ComboBox cbListGrille = (ComboBox)this.FindName("cb_liste_grille");
                    cbListGrille.Items.Add(oG.Name);
                    listGrille.Add(oG);
                    // date
                    line = sr.ReadLine();
                    oG.Date = line;
                    // values
                    line = sr.ReadLine();
                    oG.Values = line;

                    // grille de jeux
                    line = sr.ReadLine();
                    int sizeSudoku = line.Length;
                    oG.Cases = new Case[sizeSudoku][];
                    for (int i = 0; i < sizeSudoku; i++)
                    {

                        oG.Cases[i] = new Case[sizeSudoku];
                        for (int j = 0; j < sizeSudoku; j++)
                        {
                            oG.Cases[i][j] = new Case();
                            oG.Cases[i][j].Valeur = line[j];
                        }

                            if (i != sizeSudoku - 1)
                            {
                                line = sr.ReadLine();
                            }
                    }

                }
                //Console.WriteLine(line);
            }

        }
        private void bt_resolve_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(cb_liste_grille.SelectedItem.ToString());
            // Chercher un jeux avec son nom
            Grille tmpGrille = getJeux(cb_liste_grille.SelectedItem.ToString());
            // Afficher le jeux trouvé
            displayGrilleOnWindow(tmpGrille);
        }
        /// <summary>
        /// Get Grille by name
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public Grille getJeux(string _name)
        {
            return this.listGrille.FirstOrDefault(g => g.Name == _name);
        }

        public void displayGrilleOnWindow(Grille g)
        {
            Grid tmpDG = (Grid)this.FindName("dg_grille");
            tmpDG.Children.Clear();
            int size = g.Values.Length;
            for (int i = 0; i < size + 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Auto);
                tmpDG.ColumnDefinitions.Add(cd);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Auto);
                tmpDG.RowDefinitions.Add(rd);
            }

            Label lblte = (Label)this.FindName("time_elapsed");
            var watch = Stopwatch.StartNew();
            Console.WriteLine(estValide(g, 0));
            watch.Stop();
            var elapsedMs = watch.Elapsed;
            Console.WriteLine(elapsedMs);

            lblte.Foreground = Brushes.Red;
            lblte.Content = elapsedMs;
            for (int x = 0; x < size; x++)
            {
                Case[] tmpCase = g.Cases[x];

                for (int y = 0; y < size; y++)
                {
                    // Add the first text cell to the Grid

                    TextBlock txt1 = new TextBlock();
                    if (tmpCase[y].Valeur == '.')
                    {
                        txt1.Background = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        txt1.Background = new SolidColorBrush(Color.FromRgb(26, 163, 203));
                    }
                    txt1.Text = tmpCase[y].Valeur.ToString();

                    txt1.Width = 20;
                    txt1.Height = 20;
                    txt1.TextAlignment = TextAlignment.Center;

                    Grid.SetColumn(txt1, y);
                    Grid.SetRow(txt1, x);
                    tmpDG.Children.Add(txt1);
                }

            }
        }
        public bool estValide(Grille jeux, int position)
        {

            Case[][] grille = jeux.Cases;
            char[] enumValue = jeux.Values.ToCharArray();
            int size = enumValue.Length;
            // Si on est à la 82e case (on sort du tableau)
            if (position == size * size)
                return true;

            // On récupère les coordonnées de la case
            int i = position / size, j = position % size;
            int resTemoin = 0;
            int resCurrent = 0;
            for (int h = 0; h < size; h++)
            {
                resTemoin += Convert.ToInt32(enumValue[h]);
                resCurrent += Convert.ToInt32(grille[i][j].Valeur);
            }
            // Si la case n'est pas vide, on passe à la suivante (appel récursif)
            if (grille[i][j].Valeur != '.')
                return estValide(jeux, position + 1);
            if (resCurrent == resTemoin)
            {
                return estValide(jeux, position + 1);
            }

            // A implémenter : backtracking
            // énumération des valeurs possibles
            for (int k = 0; k < enumValue.Length; k++)
            {

                if (absentSurLigne(enumValue[k], grille, i) && absentSurColonne(enumValue[k], grille, j) && absentSurBloc(enumValue[k], grille, i, j))
                {
                    // On enregistre k dans la grille
                    grille[i][j].Valeur = enumValue[k];

                    // On appelle récursivement la fonction estValide(), pour voir si ce choix est bon par la suite
                    if (estValide(jeux, position + 1))
                        return true;  // Si le choix est bon, plus la peine de continuer, on renvoie true :)
                }
            }
            // Tous les chiffres ont été testés, aucun n'est bon, on réinitialise la case
            grille[i][j].Valeur = '.';
            // Puis on retourne false :(
            return false;
        }
        public bool absentSurLigne(char k, Case[][] grille, int i)
        {
            int size = grille[0].Length;
            for (int j = 0; j < size; j++)
                if (grille[i][j].Valeur == k)
                    return false;
            return true;
        }

        public bool absentSurColonne(char k, Case[][] grille, int j)
        {
            int size = grille[0].Length;
            for (int i = 0; i < size; i++)
                if (grille[i][j].Valeur == k)
                    return false;
            return true;
        }

        public bool absentSurBloc(char k, Case[][] grille, int i, int j)
        {
            int sqrt = (int)Math.Sqrt(grille[0].Length);
            int _i = i - (i % sqrt), _j = j - (j % sqrt);
            for (i = _i; i < _i + sqrt; i++)
                for (j = _j; j < _j + sqrt; j++)
                    if (grille[i][j].Valeur == k)
                        return false;
            return true;
        }
    }
}
