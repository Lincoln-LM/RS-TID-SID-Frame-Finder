using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RNGRecovertest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Recover(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            uint add;
            uint k;
            uint mult;
            byte[] low = new byte[0x10000];
            bool[] flags = new bool[0x10000];
            
            k = 0xC64E6D00; // Mult << 8
            mult = 0x41c64e6d; // pokerng constant
            add = 0x6073; // pokerng constant
            uint count = 0;
            foreach (byte element in low) {
                low[count] = 0;
                count++;
                    }
            count = 0;
            foreach (bool element in flags)
            {
                flags[count] = true;
                count++;
            }
            for (short i = 0; i < 256; i++)
            {
                uint right = (uint)(mult * i + add);
                ushort val = (ushort)(right >> 16);
                flags[val] = true;
                low[val--] = (byte)(i);
                flags[val] = true;
                low[val] = (byte)(i);
            }
            uint tid, pid, PIDhigh, PIDlow;
            try
            {
                tid = uint.Parse(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("Error: TID entered incorrectly.");
                return;
            }
            try
            {
                pid = uint.Parse(textBox2.Text, System.Globalization.NumberStyles.HexNumber);
                PIDhigh = (pid >> 16);
                PIDlow = (pid & 0xFFFF);
            }
            catch
            {
                MessageBox.Show("Error: PID entered incorrectly.");
                return;
            }
            uint psv = (PIDlow ^ PIDhigh) / 8;
            uint prv = (PIDlow ^ PIDhigh) - 8 * psv;
            List<uint> sids = new List<uint>();
            List<uint> sids2 = new List<uint>();
            for (uint testsid = 0; testsid < 0xFFFF; testsid++)
            {
                if ((testsid ^ tid)/8 == psv)
                {
                    sids.Add(testsid);
                }
            }
            List<uint> origin = new List<uint>();
            foreach (uint sid in sids)
            {
                string TSIDlow = sid.ToString("X4");
                string TSIDhigh = tid.ToString("X4");
                string TSID = TSIDhigh + TSIDlow;
                
                uint tspid = uint.Parse(TSID, System.Globalization.NumberStyles.HexNumber);
                uint first = tspid << 16;
                uint second = (uint)(tspid & 0xFFFF0000);
                uint search = (uint)(second - first * mult);
                for (short i = 0; i < 256; i++, search -= k)
                {
                    if (flags[search >> 16])
                    {
                        uint test = first | (uint)(i << 8) | low[search >> 16];
                        if (((test * mult + add) & 0xffff0000) == second)
                        {
                            LCRNG rng = new LCRNG((int)test);
                            int seed = rng.nextUInt();
                            origin.Add((uint)seed);
                            sids2.Add(sid);
                        }
                    }
                }
            }
            uint frame;
            int index = 0;
            foreach (int s in origin)
            {
                frame = 1;
                LCRNG rng = new LCRNG(s);
                while ((uint)rng.seed > (uint)0xFFFF)
                {
                    rng.nextUInt();
                    frame++;
                }
                uint temptsv = (sids2[index] ^ tid)/8;
                uint temptrv = sids2[index] ^ tid - 8 * temptsv;
                string shiny = "Star";
                if (temptrv == prv)
                {
                    shiny = "Square";
                }
                int ugh = sids2.Count;
                var start = new DateTime(2000, 1, 1, 1, 0, 0);
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                var lastdate = new DateTime(start.Year, 12, 31);
                lastdate = DateTime.SpecifyKind(lastdate, DateTimeKind.Utc);
                int minDay = 0;
                int maxDay = 0;
                List<DateTime> times = new List<DateTime>();
                for (short x = 2001; x < 2000; x++)
                {
                    var temp = new DateTime(x, 1, 1);
                    temp = DateTime.SpecifyKind(temp, DateTimeKind.Utc);
                    minDay += (int)(lastdate - start).TotalDays;
                    maxDay += (int)(lastdate - start).TotalDays;
                }
                for (byte month = 1; month < 13; month++)
                {
                    var temp = new DateTime(2000, month, 1);
                    maxDay += DateTime.DaysInMonth(2000, month);
                    for (int day = minDay; day < maxDay; day++)
                    {
                        for (int hour = 0; hour < 24; hour++)
                        {
                            for (int minute = 0; minute < 60; minute++)
                            {
                                int v = 1440 * day + 960 * (hour / 10) + 60 * (hour % 10) + 16 * (minute / 10) + (minute % 10) + 0x5A0;
                                v = (v >> 16) ^ (v & 0xFFFF);
                                if (v == rng.seed)
                                {
                                    var finalTime = start.AddDays(day).AddSeconds(((hour-1) * 60 * 60) + (minute * 60));
                                    finalTime = DateTime.SpecifyKind(finalTime, DateTimeKind.Utc);
                                    times.Add(finalTime);
                                    

                                }
                            }
                        }
                    }
                    minDay += DateTime.DaysInMonth(2000, month);
                }
                times.Sort((a, b) => a.Month.CompareTo(b.Month));
                var dt = DateTime.SpecifyKind(times[0], DateTimeKind.Utc);
                string timestring = dt.ToString("ddd MMM d HH:mm:ss yyyy");

                dataGridView1.Rows.Add(rng.seed.ToString("X4"), frame.ToString(),tid,sids2[index++],shiny,timestring);

            }
        }
    }
}
