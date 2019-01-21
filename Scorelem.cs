using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Remi
{
    public class Scorelem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Score1 { get; set; }
        public string Score2 { get; set; }
        public string Score3 { get; set; }
        public string Score4 { get; set; }
        public string Inchide { get; set; } // nr of player
    }
}