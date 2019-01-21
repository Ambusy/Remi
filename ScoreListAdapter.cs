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

namespace Remi
{
    [Activity(Label = "ScoreListAdapter")]
    public class ScoreListAdapter : BaseAdapter<string>
    {
        List<Scorelem> items;
        Activity context;
        public ScoreListAdapter(Activity context, List<Scorelem> items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override string this[int position]
        {
            get { return items[position].Score1 + " " + items[position].Score2 + " " + items[position].Score3 + " " + items[position].Score4;}
        }
        public override int Count
        {
            get { return items.Count; }//items.Length
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            int iPlayer;
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.list_item, null);
            view.FindViewById<TextView>(Resource.Id.lScore1).Text = items[position].Score1;
            view.FindViewById<TextView>(Resource.Id.lScore2).Text = items[position].Score2;
            view.FindViewById<TextView>(Resource.Id.lScore3).Text = items[position].Score3;
            view.FindViewById<TextView>(Resource.Id.lScore4).Text = items[position].Score4;
            if (Int32.TryParse(items[position].Inchide, out iPlayer))
            {
            }
            setPlayer(iPlayer, 1, view.FindViewById<TextView>(Resource.Id.lScore1));
            setPlayer(iPlayer, 2, view.FindViewById<TextView>(Resource.Id.lScore2));
            setPlayer(iPlayer, 3, view.FindViewById<TextView>(Resource.Id.lScore3));
            setPlayer(iPlayer, 4, view.FindViewById<TextView>(Resource.Id.lScore4));
            return view;
        }
        void setPlayer(int iPlayer, int nr,  TextView score)
        {
            if (iPlayer==nr)
            {
                score.SetBackgroundColor(Android.Graphics.Color.LightGreen);
            }
            else
            {
                score.SetBackgroundColor(Android.Graphics.Color.White);
            }
        }

    }
}