using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApplication.Models
{
    public class Case
    {
        public char Valeur { get; set; }
        public int Nbre_hypotheses { get; set; }
        public char[] Hypotheses { get; set; }

    }
}
