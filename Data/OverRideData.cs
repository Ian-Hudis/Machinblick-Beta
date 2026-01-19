using static Maschinblick.SQL_Client;


namespace Maschinblick.Data
{
    public class OverRideData
    {

        public class Item
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }

        public struct OverRideMaterial
        {
            // head 1
            public List<Item>? Rapid;
            public List<Item>? Feed;
            public List<Item>? Spindle;
            // head 2
            public List<Item>? Rapid2;
            public List<Item>? Feed2;
            public List<Item>? Spindle2;
            // gantry
            public List<Item>? Gantry;
        }

        public OverRideMaterial MakeLineGraph(OverRideMaterial overrides, ServiceData line)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            try
            {
                // head 1
                for (int i = 0; i<line.H1Rap.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H1Rap[i].Stime,
                        Value = line.H1Rap[i].Value
                    };
                    overrides.Rapid.Add(item);
                }
                for (int i = 0; i<line.H1Feed.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H1Feed[i].Stime,
                        Value = line.H1Feed[i].Value
                    };
                    overrides.Feed.Add(item);
                }
                for (int i = 0; i<line.H1Spind.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H1Spind[i].Stime,
                        Value = line.H1Spind[i].Value
                    };
                    overrides.Spindle.Add(item);
                }
                // head 2
                for (int i = 0; i<line.H2Rap.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H2Rap[i].Stime,
                        Value = line.H2Rap[i].Value
                    };
                    overrides.Rapid2.Add(item);
                }
                for (int i = 0; i<line.H2Feed.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H2Feed[i].Stime,
                        Value = line.H2Feed[i].Value
                    };
                    overrides.Feed2.Add(item);
                }
                for (int i = 0; i<line.H2Spind.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.H2Spind[i].Stime,
                        Value = line.H2Spind[i].Value
                    };
                    overrides.Spindle2.Add(item);
                }
                // gantry
                for (int i = 0; i<line.Gantry.Length-1; i++)
                {
                    Item item = new()
                    {
                        Date = line.Gantry[i].Stime,
                        Value = line.Gantry[i].Value
                    };
                    overrides.Gantry.Add(item);
                }
            }
            catch 
            {
                Console.WriteLine("Failed OverRideData line 89");
            }
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            return overrides;
        } 

    }
}
