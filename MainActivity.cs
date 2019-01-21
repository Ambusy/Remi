using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Util;
using System.Linq;
using SQLite;
using System;
using System.Collections.Generic;

namespace Remi
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            Button startButton = FindViewById<Button>(Resource.Id.buttonStart);
            startButton.Click += OnStartButtonClicked;
            Button contButton = FindViewById<Button>(Resource.Id.buttonCont);
            contButton.Click += OnContButtonClicked;
            
        }
        void OnContButtonClicked(object sender, EventArgs e)
        {
            int[] jc = new int[4];
            jc[0] = Resource.Id.Jocatorul1; jc[1] = Resource.Id.Jocatorul2; jc[2] = Resource.Id.Jocatorul3; jc[3] = Resource.Id.Jocatorul4;
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dbname = System.IO.Path.Combine(folder, "players.db");
            using (var connection = new SQLiteConnection(dbname))
            {                
                for (int i = 0; i < 4; i++)
                {
                    EditText joc = FindViewById<EditText>(jc[i]);
                    joc.Text = "";                  
                }
                IEnumerable<Player> pl = connection.Query<Player>("select * from player order by id");
                List<Player> players = pl.ToList<Player>();
                int iPlayer = 0;
                foreach (Player player in pl)
                {
                    EditText joc = FindViewById<EditText>(jc[iPlayer]);
                    joc.Text = player.Name;
                    iPlayer++;
                }

            }
            Intent intent = new Intent(this, typeof(Scores));
            StartActivity(intent);
        }
        void OnStartButtonClicked(object sender, EventArgs e)
        {
            int[] jc = new int[4];
            jc[0] = Resource.Id.Jocatorul1; jc[1] = Resource.Id.Jocatorul2; jc[2] = Resource.Id.Jocatorul3; jc[3] = Resource.Id.Jocatorul4;
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dbname = System.IO.Path.Combine(folder, "players.db");            
            System.IO.File.Delete(dbname);
            using (var connection = new SQLiteConnection(dbname))
            {
                Player player;
                Scorelem scorelem;
                try
                {
                    connection.CreateTable<Player>();
                    connection.CreateTable<Scorelem>();
                }
                catch (SQLiteException ex)
                {
                    Log.Info("SQLiteEx", ex.Message);
                    return;
                }
                int nPlayers = 0;
                for (int i = 0; i < 4; i++)
                {
                    EditText joc = FindViewById<EditText>(jc[i]);
                    if (joc.Text != "")
                        nPlayers++;
                }
                scorelem = new Scorelem
                {
                    Id = 0
                };
                for (int i = 0; i < nPlayers; i++){               
                    EditText joc = FindViewById<EditText>(jc[i]);
                    player = new Player
                    {
                        Name = joc.Text,
                        Id=i+1,
                        Score = 0,
                        Atout=false,
                        Active=false,
                        Inchis=false
                    };
                    if (i == 0) player.Active = true;
                    if (i == 0) scorelem.Score1 = joc.Text;
                    if (i == 1) scorelem.Score2 = joc.Text;
                    if (i == 2) scorelem.Score3 = joc.Text;
                    if (i == 3) scorelem.Score4 = joc.Text;
                    connection.Insert(player);
                }
                connection.Insert(scorelem);
            }
            Intent intent = new Intent(this, typeof(Scores));
            StartActivity(intent);
        }
    }
}
