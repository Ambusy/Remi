using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Remi
{
    [Activity(Label = "Scores", ScreenOrientation = ScreenOrientation.Landscape)]
    public class Scores : Activity
    {
        int[] pn = new int[4];// names
        int[] ps = new int[4];// puncte
        int[] pa = new int[4];// atout
        int[] pi = new int[4];// inchide
        int[] pt = new int[4];// total
        string dbName;
        int nPlayer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_scores);
            GetIds();
            Button randButton = FindViewById<Button>(Resource.Id.buttonRand);
            randButton.Click += OnRandButtonClicked;
            Button corButton = FindViewById<Button>(Resource.Id.buttonCor);
            corButton.Click += OnCorButtonClicked;
            Button listButton = FindViewById<Button>(Resource.Id.buttonList);
            listButton.Click += OnListButtonClicked;
            for (int i = 0; i < 4; i++)
            {
                Button atout = FindViewById<Button>(pa[i]);
                atout.Click += onClickPlayeratout;
                Button inchide = FindViewById<Button>(pi[i]);
                inchide.Click += onClickPlayerinchide;
            }
            fillScreen("C");
        }
        void fillScreen(string src)
        {
            GetIds();
            using (var connection = new SQLiteConnection(dbName))
            {          
                IEnumerable<Player> pl = connection.Query<Player>("select * from player order by id");
                List<Player> players = pl.ToList<Player>();
                int actPlayer = getActPlayer(pl);
                int iPlayer = 0;
                foreach (Player plr in pl)
                {
                    TextView joc = FindViewById<TextView>(pn[iPlayer]);
                    EditText score = FindViewById<EditText>(ps[iPlayer]);
                    Button atout = FindViewById<Button>(pa[iPlayer]);
                    Button inchis = FindViewById<Button>(pi[iPlayer]);
                    TextView total = FindViewById<TextView>(pt[iPlayer]);
                    joc.Text = plr.Name;                    
                    setActPlayer(iPlayer, plr);
                    if (src == "I") // correction procedure
                    {
                        score.Text = plr.Score.ToString();
                        if (plr.Atout)
                            atout.Text = "X";
                        else
                            atout.Text = "-"; 
                        if (plr.Inchis)
                            inchis.Text = "X";
                        else
                            inchis.Text = "-";
                        if (iPlayer == 0)
                        {
                            Switch jocDublu = FindViewById<Switch>(Resource.Id.jocDublu);
                            jocDublu.Checked = plr.Dublu;
                        }

                    }
                    else // normal procedure
                    {
                        score.Text = "";
                        atout.Text = "-";
                        inchis.Text = "-";
                        if (iPlayer == 0)
                        {
                            Switch jocDublu = FindViewById<Switch>(Resource.Id.jocDublu);
                            jocDublu.Checked = false;
                        }

                    }
                    total.Text = plr.Total.ToString();
                    iPlayer += 1;
                }
                for (int i = iPlayer; i<4; i++) // if < 4 players
                {
                    TextView joc = FindViewById<TextView>(pn[i]);
                    EditText score = FindViewById<EditText>(ps[i]);
                    Button atout = FindViewById<Button>(pa[i]);
                    Button inchis = FindViewById<Button>(pi[i]);
                    TextView total = FindViewById<TextView>(pt[i]);
                    joc.Visibility = ViewStates.Gone;
                    score.Visibility = ViewStates.Gone;
                    atout.Visibility = ViewStates.Gone;
                    inchis.Visibility = ViewStates.Gone;
                    total.Visibility = ViewStates.Gone;
                }
            }
        }
        void OnListButtonClicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ScoreListActivity));
            StartActivity(intent);
        }
        void OnCorButtonClicked(object sender, EventArgs e)
        {
            Button corButton = FindViewById<Button>(Resource.Id.buttonCor);
            corButton.Visibility = ViewStates.Invisible;
            GetIds();
            using (var connection = new SQLiteConnection(dbName))
            {
                Scorelem scorelemSv;
                IEnumerable<Scorelem> sl = connection.Query<Scorelem>("select * from scorelem order by id desc limit 1");
                List<Scorelem> scoreelems = sl.ToList<Scorelem>();
                scorelemSv = scoreelems[0]; // last scores;
                int id = scoreelems[0].Id;
                connection.Delete<Scorelem>(id);
                int plInchide = 0;
                if (Int32.TryParse(scorelemSv.Inchide, out plInchide))
                {
                }
                IEnumerable<Player> pl = connection.Query<Player>("select * from player order by id");
                List<Player> players = pl.ToList<Player>();
                int actPlayer = getActPlayer(pl) + 1;               
                if (actPlayer == 1) // previous player becomes active
                    actPlayer = 4;
                else
                    actPlayer -= 1;               
                int iPlayer = 0;
                foreach (Player player in pl) // restore totals for each player
                {
                    int j = player.Score;
                    if (player.Atout)
                        j -= 50;
                    if (player.Dublu)
                        j = j / 2;
                    if (plInchide == iPlayer + 1)
                        j -= 100;
                    player.Total -= player.Score;
                    player.Score = j;
                    player.Active = (actPlayer - 1 == iPlayer);
                    connection.Update(player);
                    iPlayer++;
                }
            }
            fillScreen("I");
        }
        void OnRandButtonClicked(object sender, EventArgs e)
        {
            GetIds();
            using (var connection = new SQLiteConnection(dbName))
            {
                Scorelem scorelem;
                scorelem = new Scorelem
                {
                    Id = 0
                };
                Switch jocDublu = FindViewById<Switch>(Resource.Id.jocDublu);            
                IEnumerable<Player> pl = connection.Query<Player>("select * from player order by id");
                List<Player> players = pl.ToList<Player>();
                int actPlayer = getActPlayer(pl);
                if (actPlayer == nPlayer - 1)
                    actPlayer = 0;
                else
                    actPlayer += 1;
                int iPlayer = 0;
                bool notNull = false;
                foreach (Player player in pl)
                {
                    player.Active = (actPlayer == iPlayer);
                    setActPlayer(iPlayer,  player);                   
                    EditText score = FindViewById<EditText>(ps[iPlayer]);
                    Button atout = FindViewById<Button>(pa[iPlayer]);
                    Button inchide = FindViewById<Button>(pi[iPlayer]);
                    player.Atout = (atout.Text == "X");
                    player.Dublu = jocDublu.Checked;
                    player.Inchis = (inchide.Text == "X");
                    int j = 0;
                    if (Int32.TryParse(score.Text, out j))
                    {
                    }
                    if (inchide.Text == "X")
                        j += 100;
                    if (jocDublu.Checked)
                        j += j;
                    if (atout.Text == "X")
                        j += 50;
                    player.Score = j;                   
                    player.Total += j;
                    if (j != 0) notNull = true;
                    connection.Update(player);
                    if (iPlayer == 0) scorelem.Score1 = player.Score.ToString();
                    if (iPlayer == 1) scorelem.Score2 = player.Score.ToString();
                    if (iPlayer == 2) scorelem.Score3 = player.Score.ToString();
                    if (iPlayer == 3) scorelem.Score4 = player.Score.ToString();
                    iPlayer++;
                    if (inchide.Text == "X")
                        scorelem.Inchide = iPlayer.ToString();                
               }
                if (notNull) { 
                    Button corButton = FindViewById<Button>(Resource.Id.buttonCor);
                    corButton.Visibility = ViewStates.Visible;
                    connection.Insert(scorelem);
                }
            }
            fillScreen("N");
         
        }
        public void onClickPlayeratout(object sender, EventArgs e)
        {
            TextView t = (TextView)sender;
            t.Text = (t.Text != "X") ? "X" : "-";
        }
        public void onClickPlayerinchide(object sender, EventArgs e)
        {
            TextView t = (TextView)sender;
            t.Text = (t.Text != "X") ? "X" : "-";
        }
        int getActPlayer(IEnumerable<Player> pl)
        {
            int actPlayer = 0;
            nPlayer = 0;
            foreach (Player player in pl)
            {
                if (player.Active)
                {
                    actPlayer = nPlayer;
                }
                nPlayer++;
            }
            return actPlayer;
        }
        void setActPlayer(int iPlayer,Player player)
        {
            TextView joc = FindViewById<TextView>(pn[iPlayer]);
            if (player.Active)
            {
                joc.SetBackgroundColor(Android.Graphics.Color.Yellow);
            }
            else
            {
                joc.SetBackgroundColor(Android.Graphics.Color.White);
            }
        }
        public void GetIds()
        {
            string folder;
            pn[0] = Resource.Id.player1name; pn[1] = Resource.Id.player2name; pn[2] = Resource.Id.player3name; pn[3] = Resource.Id.player4name;
            ps[0] = Resource.Id.player1score; ps[1] = Resource.Id.player2score; ps[2] = Resource.Id.player3score; ps[3] = Resource.Id.player4score;
            pa[0] = Resource.Id.player1atout; pa[1] = Resource.Id.player2atout; pa[2] = Resource.Id.player3atout; pa[3] = Resource.Id.player4atout;
            pi[0] = Resource.Id.player1inchis; pi[1] = Resource.Id.player2inchis; pi[2] = Resource.Id.player3inchis; pi[3] = Resource.Id.player4inchis;
            pt[0] = Resource.Id.player1total; pt[1] = Resource.Id.player2total; pt[2] = Resource.Id.player3total; pt[3] = Resource.Id.player4total;
            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            dbName = System.IO.Path.Combine(folder, "players.db");
        }
        public int GetPlrId(int id, int[] ids)
        {
            for (int i = 0; i < 4; i++)                
            {
                if (ids[i] == id) return i=1;
            }
            return 0;
        }
        public Player GetPlayer(int id)
        {
            int dbId = GetPlrId(id, ps);
            Player retPlr=null;
            using (var connection = new SQLiteConnection(dbName))
            {
                string[] pars = new string[1];
                pars[0] = dbId.ToString();
                IEnumerable<Player> pl = connection.Query<Player>("select * from player where Id = @id",pars);
                List<Player> players = pl.ToList<Player>();            
                foreach (Player plr in pl)
                {
                    retPlr = plr;
                }
            }
            return retPlr;
        }
    }
}