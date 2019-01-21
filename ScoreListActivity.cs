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
    [Activity(Label = "ScoreListActivity", ScreenOrientation = ScreenOrientation.Landscape)]
    public class ScoreListActivity : ListActivity
    {
        List<Scorelem> scores;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string dbName;
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            dbName = System.IO.Path.Combine(folder, "players.db");
            using (var connection = new SQLiteConnection(dbName))
            {
                IEnumerable<Scorelem> scl = connection.Query<Scorelem>("select * from scorelem order by id");
                scores = scl.ToList<Scorelem>();                
            }
            ListAdapter = new ScoreListAdapter(this, scores);
        }        
    }
}
