using Microsoft.Extensions.Logging;
using Radzen.Blazor.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.PortableExecutable;
using static Maschinblick.SQL_Client;

namespace Maschinblick.Data
{
    public class BarData
    {
        // color pallet for the bar graph
        public readonly static List<string> ColorPallet = new()
        {
            "#20ff10", // green for running
            "#1a50ff", // Blue for manual
            "#eeff10", // yellow for idle
            "#fe1919", // red for down
            "grey" // grey for default
        };

        //slap some color on that baby
        private static string AssignColor(string color)
        {
            string StateColor;
            // find the color
            switch (color)
            {
                case string a when a.Contains("RUNNING"):
                    StateColor = ColorPallet[0]; // should be green
                    break;
                case string a when a.Contains("MANUAL"):
                    StateColor = ColorPallet[1]; //shoulde be blue
                    break;
                case string a when a.Contains("IDLE"):
                    StateColor = ColorPallet[2]; //shoulde be yellow
                    break;
                case string a when a.Contains("DOWN"):
                    StateColor = ColorPallet[3]; //shoulde be red
                    break;
                default:
                    StateColor = ColorPallet[4]; // default grey
                    break;
            }
            return StateColor;
        }

        // find the time given a shift
        private static DateTime TimeReference(string center, string shift, DateTime Day, bool start) // start is false when the shift is ending
        {
            DateTime timey;
            switch (center)
            {
                case "mach":
                    switch (shift)
                    {
                        case "1st":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 5, 0, 0); // starts at 5:00 am
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 15, 30, 0); // ends at 3:30 pm
                            }
                            break;
                        case "Overnight":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 2, 0, 0); // starts at 2:00 am
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 5, 0, 0); // ends at 5:00 am
                            }
                            break;
                        case "3rd":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 5, 0, 0); // starts at 5:00 am
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 17, 30, 0); // ends at 5:30 pm
                            }
                            break;
                        case "Overnight2":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 17, 30, 0); // starts at 5:30 pm            
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 5, 0, 0); // ends at 5:00 am    
                            }
                            break;

                        default: // 2nd shift
                            if (start)
                            {
                               timey = new DateTime(Day.Year, Day.Month, Day.Day, 15, 30, 0); // starts at 3:30 pm            
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 2, 0, 0); // ends at 2:00 am   
                            }
                            break;
                    }
                    break;

                default:  // line
                    switch (shift)
                    {
                        case "1st":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 6, 0, 0); // starts at 6:00 am
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 14, 30, 0); // ends at 2:30 pm
                            }
                            break;
                        case "2nd":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 14, 30, 0); // starts at 2:30 pm
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 1, 0, 0); // ends at 2 am
                            }
                            break;
                        case "3rd":
                            if (start)
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 6, 0, 0); // starts at 6:00 am
                            }
                            else
                            {
                                timey = new DateTime(Day.Year, Day.Month, Day.Day, 16, 30, 0); // ends at 4:30 pm
                            }
                            break;
                        default: // overnight
                            if (Day.DayOfWeek == DayOfWeek.Saturday || Day.DayOfWeek == DayOfWeek.Sunday) // the weekend for 3rd shift
                            {
                                if (start)
                                {
                                    timey = new DateTime(Day.Year, Day.Month, Day.Day, 16, 30, 0); // starts at 4:30 pm            
                                }
                                else
                                {
                                    timey = new DateTime(Day.Year, Day.Month, Day.Day, 6, 0, 0); // ends at 6:00 am    
                                }
                            }
                            else // the weekay after 2nd shift
                            {
                                if (start)
                                {
                                    timey = new DateTime(Day.Year, Day.Month, Day.Day, 1, 0, 0); // starts at 1:00 am            
                                }
                                else
                                {
                                    timey = new DateTime(Day.Year, Day.Month, Day.Day, 6, 0, 0); // ends at 6:00 am   
                                }
                            }
                            break;
                    }
                    break;
            }

            return timey;
        }

        // find the shift given a time
        private static string ShiftFinder(DateTime time, string Series)
        {
            string shift;


            switch (Series)
            {
                case "mach":

                    if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 5, 0, 0)) > 0 &&
                    DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 15, 30, 0)) < 0 &&
                    time.DayOfWeek != DayOfWeek.Friday && time.DayOfWeek != DayOfWeek.Saturday && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        shift = "1st";
                    }
                    else if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 15, 30, 0)) > 0 ||
                        DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 2, 0, 0)) < 0 &&
                        (time.DayOfWeek != DayOfWeek.Friday || time.Hour < 2) && time.DayOfWeek != DayOfWeek.Saturday && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        shift = "2nd";
                    }
                    else if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 5, 0, 0)) > 0 &&
                             DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 15, 30, 0)) < 0)
                    {
                        shift = "3rd";
                    }
                    else if (time.DayOfWeek != DayOfWeek.Friday && time.DayOfWeek != DayOfWeek.Saturday && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        shift = "Overnight";
                    }
                    else
                    {
                        shift = "Overnight2";
                    }

                    break;

                default: //line

                    if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 6, 0, 0)) > 0 &&
                    DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 14, 30, 0)) < 0 &&
                    time.DayOfWeek != DayOfWeek.Friday && time.DayOfWeek != DayOfWeek.Saturday && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        shift = "1st";
                    }
                    else if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 14, 30, 0)) > 0 ||
                    DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 1, 0, 0)) < 0 &&
                    (time.DayOfWeek != DayOfWeek.Saturday || time.Hour < 1) && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        shift = "2nd";
                    }
                    else if (DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 6, 0, 0)) > 0 &&
                    DateTime.Compare(time, new DateTime(time.Year, time.Month, time.Day, 14, 30, 0)) < 0)
                    {
                        shift = "3rd";
                    }
                    else if (time.DayOfWeek != DayOfWeek.Friday && time.DayOfWeek != DayOfWeek.Saturday && time.DayOfWeek != DayOfWeek.Sunday)
                    {
                            shift = "Overnight";
                    }
                    else
                    {
                            shift = "Overnight2";
                    }
                    break;
            }
            
            return shift;
        }

        // make the bar label
        private static string Labeler(TimeSpan StateDuration, DateTime starttime, DateTime endtime)
        {
            string Label;
            if (StateDuration < new TimeSpan(0, 20, 0) && StateDuration > new TimeSpan(0, -20, 0)) 
            {
                Label = " ";
            }
            else
            {
                Label = starttime.ToString("h:mm tt") + " -> " + endtime.ToString("h:mm tt"); // this is the label in the bar graph
            }
            return Label;
        }


        // the structure for the bars
        public class BarStructure
        {
            public class ChartData // object for the sizing
            {
                public string? X { get; set; }
                public decimal Tvalue { get; set; }
                public string? Text { get; set; }
            }

            public List<ChartData>[] Details = new List<ChartData>[2]; // array for the sizing 
            public string[] Color = new string[2]; // array for the color fill

            public int barcount;

        }

        // declaration for the structure of bars
        public class Barset
        { 
            public BarStructure Bar1 = new();
            public BarStructure Bar2 = new();
            public BarStructure Bar3 = new();
            public BarStructure BarOV = new();
            public BarStructure BarOV2 = new();
        }

        // for building the bar state
        private static BarStructure AddBarState(BarStructure bardata, string row, TimeSpan Event, string color, string label)
        {

            Array.Resize(ref bardata.Details, bardata.barcount + 1);
            Array.Resize(ref bardata.Color, bardata.barcount + 1);

            decimal T1 = (decimal)Event.TotalSeconds / 3600;
            T1 = Math.Round(T1, 2);
            //double timelength = decimal.ToDouble(T1);

            bardata.Details[bardata.barcount] = new List<BarStructure.ChartData>
            {
                new BarStructure.ChartData{X = row, Tvalue = T1, Text = label},
            };
            // Console.WriteLine(bardata.Details.Length +"   " + bardata.Color.Length);

            bardata.Color[bardata.barcount] = color;

            bardata.barcount++;

            return bardata;
        }

        // makes one bar in the graph each time this function is used
        
        private static Barset MakeSet(Barset bar, DateTime Time1, DateTime Time2, string Event, string shiftoccurance, string center)
        {
            TimeSpan StateDuration = Time2 - Time1; // find the the duration a machine is in a state                    
            string StateColor = AssignColor(Event); // find the color for the bar
            string Label = Labeler(StateDuration, Time1, Time2); // make the label
            
            string datelabel = Time1.ToString("MM/dd");
            
            if (shiftoccurance != "2nd" && shiftoccurance != "Overnight2")
            {
                datelabel = Time1.ToString("MM/dd"); 
            }
            else
            {
                if(shiftoccurance == "2nd")
                {
                    if(center == "mach") // machining
                    {
                        datelabel = Time1.AddHours(-2).ToString("MM/dd"); //2nd shift ends at 2:00 am
                    }
                    else // the line
                    {
                        datelabel = Time1.AddHours(-1).ToString("MM/dd"); //2nd shift ends at 1:00 am
                    }
                }
                else // Overnight2
                {
                    if(center == "mach")
                    {
                        datelabel = Time1.AddHours(-5).ToString("MM/dd"); // 3rd shift starts at 5:00am
                    }
                    else
                    {
                       datelabel = Time1.AddHours(-6).ToString("MM/dd"); // 3rd shift starts at 6:00am
                    }

                }

            }
           
            // put bar into the right shift
            switch (shiftoccurance)
            {
                case "1st":
                    bar.Bar1 = AddBarState(bar.Bar1, datelabel, StateDuration, StateColor, Label);
                    break;
                case "2nd":
                    bar.Bar2 = AddBarState(bar.Bar2, datelabel, StateDuration, StateColor, Label);
                    break;
                case "3rd":
                    bar.Bar3 = AddBarState(bar.Bar3, datelabel, StateDuration, StateColor, Label);
                    break;
                case "Overnight2":
                    bar.BarOV2 = AddBarState(bar.BarOV2, datelabel, StateDuration, StateColor, Label);
                    break;
                case "Overnight":
                    bar.BarOV = AddBarState(bar.BarOV, datelabel, StateDuration, StateColor, Label);
                    break;
            }
            return bar;
        }
        

        // get the bars to go into shifts correctly -> 2023
        
        // 8/9/2024 -> "correctly" my ass

        private static Barset CompletetheBar(Barset bar, string Event, DateTime StartTime, string startshift, DateTime EndTime, string endshift, string center)
        {
            TimeSpan logduration= EndTime - StartTime;

            Console.WriteLine(logduration.ToString());

            // these are the logs that take place within a shift
            if (startshift == "1st" && endshift=="1st" && logduration.TotalHours < 16)
            {
                bar = MakeSet(bar, StartTime, EndTime, Event, "1st", center);
            }
            else if (startshift =="2nd" && endshift == "2nd" &&  logduration.TotalHours < 16)
            {
                bar = MakeSet(bar, StartTime, EndTime, Event, "2nd", center);
            }
            else if (startshift =="Overnight" && endshift == "Overnight" && logduration.TotalHours < 6)
            {
                bar = MakeSet(bar, StartTime, EndTime, Event, "Overnight", center);
            }
            else if (startshift =="3rd" && endshift == "3rd" && logduration.TotalHours < 16)
            {
                bar = MakeSet(bar, StartTime, EndTime, Event, "3rd", center);
            }
            else if (startshift =="Overnight2" && endshift == "Overnight2" && logduration.TotalHours < 12)
            {
                bar = MakeSet(bar, StartTime, EndTime, Event, "Overnight2", center);
            }
            else
            {
                Console.WriteLine("Next Shift");
            }
            //else if(start)

            return bar;
        }
        
        // makes the bar graph
        private static Barset BarUpdate(SQL_Reader[] data, string center, string EndTime, string StartTime)
        {
            Barset bar = new(); // the bar graphs output

            DateTime starttime = DateTime.Parse(StartTime);
            DateTime endtime = DateTime.Parse(EndTime);

            string Startshift = ShiftFinder(starttime, center);
            string endshift = ShiftFinder(endtime, center);

            Console.WriteLine("\n\n\n");


            // the first bar
            CompletetheBar(bar, data[0].MT_state, starttime, Startshift, data[1].Stime, data[1].shift, center);
           // Console.WriteLine(0 +" " + data[0].ID + " " +Startshift +" "+ starttime +" " + data[0].MT_state); // other rows

            // the bars in between
            for (int row = 1; row < data.Length-2; row++)
            {
                CompletetheBar(bar, data[row].MT_state, data[row].Stime, data[row].shift, data[row+1].Stime, data[row+1].shift, center);
               //  Console.WriteLine(row +" " + data[row].ID + " " +data[row].shift +" "+ data[row].Stime +" " + data[row].MT_state); // other rows
            }
            // the last bar
            CompletetheBar(bar, data[data.Length-2].MT_state, data[data.Length-2].Stime, data[data.Length-2].shift, endtime, endshift, center);
          //  Console.WriteLine(data.Length-2 +" " + data[data.Length-2].ID + " " + data[data.Length-2].shift +" "+ data[data.Length-2].Stime +" " + data[data.Length-2].MT_state); // other rows
           // Console.WriteLine("END:" + endshift+ " " +endtime + " " + data[data.Length-1].MT_state);

            return bar;
        }

        // function in razor pages to make the graphs
        public Barset Barprocess(string center, SQL_Reader[] bardata, string EndTime, string StartTime)
        {

            Barset bs = new(); // creates a new instance of the barsets

            try
            {
                bs = BarUpdate(bardata, center, EndTime, StartTime); 
            }
            catch
            {
                Console.WriteLine("line 359 in Bardata: null values in " + bs.ToString());
            }
            return bs;
        }

    }
}
