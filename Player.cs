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
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }
        public bool Atout { get; set; }
        public bool Active { get; set; }
        public bool Inchis { get; set; }
        public bool Dublu { get; set; }
    }
}