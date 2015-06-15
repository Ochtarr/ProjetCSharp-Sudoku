using SudokuApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivan.Projet.Sudoku.Configuration
{
    public class Grille
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Values { get; set; }
        public Case[][] Cases { get; set; }

        // L'idée est de récupérer l'ensemble des lignes sous la forme d'un char[]
        //public Char[] getLines(Case[] cases)
    
    }
}
