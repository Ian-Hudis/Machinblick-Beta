using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace Maschinblick
{
    public class SQL_Client
    {
        //! Read From the sql server
        public struct SQL_Reader
        {
            public long ID;
            public string Plant;
            public string WorkCenter;
            public string Machine;
            public DateTime Mtime; // machine time
            public string Material;
            public string ProductionOrder;
            public string Operator;
            public string ProdSupervisor;
            public string Event;
            public int Value;
            public DateTime Stime; // server  time

            // added later
            public string shift; // 1 is 1st shift, 2 is second shift, 3 is 3rd shift, and 0 is overnight

            // new columns
            public TimeSpan SAP_Cycletime;
            public TimeSpan SAP_Loadtime;
            public TimeSpan Actual_Cycletime;
            public TimeSpan Actual_Loadtime;
            public int SAP_SerialNumber;
            public int Serial_Internal;
            public bool Result;
            public string WShift;
            public string MT_state;
            public int MT_Partcount1;
            public int MT_partcount2;
            public short MT_H1Rapid;
            public short MT_H1Feed;
            public short MT_H1Spindle;
            public short MT_H2Rapid;
            public short MT_H2Feed;
            public short MT_H2Spindle;
            public short MT_Gantry;
            public bool Kiosk_Override;
            public int SAP_Quantity;
            public string Day_Of_Week;
            public string Loader_Type;
            public string KioskOp;
            public string KioskCN;
            public TimeSpan Kiosk_SetupTime;
            public TimeSpan SAP_SetupTime;
            public TimeSpan SAP_Machine_Cycletime;
            public int SAP_Base_Quantity;
            public string Kiosk_State;
        }

        public class ServiceData
        {
            public SQL_Reader[] GenData = new SQL_Reader[2]; // raw data 
            //MTConnect logs
            public SQL_Reader[] StateData = new SQL_Reader[2]; // event data for changes in WorkCenter state
            public SQL_Reader[] PartData = new SQL_Reader[2]; // event data for part count
            public SQL_Reader[] H1Rap = new SQL_Reader[2]; // rapid for head 1
            public SQL_Reader[] H1Feed = new SQL_Reader[2]; // Feed for head 1
            public SQL_Reader[] H1Spind = new SQL_Reader[2]; // Spindle for head 1
            public SQL_Reader[] H2Rap = new SQL_Reader[2]; // rapid for head 1
            public SQL_Reader[] H2Feed = new SQL_Reader[2]; // Feed for head 1
            public SQL_Reader[] H2Spind = new SQL_Reader[2]; // Spindle for head 1
            public SQL_Reader[] Gantry = new SQL_Reader[2]; // gantry data
            //kiosk logs
            public SQL_Reader[] KioskData = new SQL_Reader[2]; // kiosk logs and program mods
        }

        // do an sql query
        public ServiceData MachineAnalyze(int machine, string Machinebrand,string workcenter, string StartDate, string EndDate)  // do a thing
        {
            ServiceData service = new();
            try
            {
                service.GenData = Import_From_SQL(service.GenData, Machinebrand, machine, StartDate, EndDate, workcenter); // grab the raw data

                service = SortTree(service); // parse the raw data

            }
            catch
            {
               // Console.WriteLine(machine + " Failed to construct;");
            }
            return service;  
        }

        // grab the sql data
        private static SQL_Reader[] Import_From_SQL(SQL_Reader[] QueryData, string MachineBrand, int machine, string StartDate, string EndDate, string SchedType)
        {
            //declare the shifts
            TimeSpan Startshift1;
            TimeSpan Startshift2;
            TimeSpan StartshiftOvernight;
            TimeSpan Startshift3 = new(5, 0, 0);
            TimeSpan EndShift3 = new(17, 30, 0);
            switch (SchedType)
            {
                case "mach":
                    Startshift1 = new(5, 0, 0); // 5:00 am  
                    Startshift2 = new(15, 30, 0); // 3:30 pm
                    StartshiftOvernight = new(2, 0, 0); // 2:00 am
                    //weekend
                    Startshift3 = new(5, 0, 0);
                    EndShift3 = new(17, 30, 0);
                    break;
                case "line":
                    Startshift1 = new(6, 0, 0); // 6:00 am  
                    Startshift2 = new(14, 30, 0); // 2:30 pm
                    StartshiftOvernight = new(1, 0, 0); // 1:00 am
                    //weekend
                    Startshift3 = new(6, 0, 0);
                    EndShift3 = new(16, 30, 0);
                    break;
                default: // assume its from machining
                    Startshift1 = new(5, 0, 0); // 5:00 am  
                    Startshift2 = new(15, 30, 0); // 3:30 pm
                    StartshiftOvernight = new(2, 0, 0); // 2:00 am
                    //weekend
                    Startshift3 = new(5, 0, 0);
                    EndShift3 = new(17, 30, 0);
                    break;
            }

            SQL_Reader Data = new(); // make the output data
            int row = 1; // for making the array

            var cs = @"Server=DB-USMN-001\IN01;Database=ProductionMonitor; User ID=s4automation;Password=s4automation";
            using var con = new SqlConnection(cs);
            con.Open();

            string sql = "SELECT [ID], [Plant], [WorkCenter], [Machine], [ControllerTime], [Material], [ProductionOrder], [Operator], [ProdSupervisor], [Event], [Value], [ServerTime]," +
                " [SAP_Cycletime], [SAP_Loadtime], [Actual_Cycletime], [Actual_Loadtime], [SAP_SerialNumber], [Serial_Internal], [Result], [WShift]," +
                " [MT_state], [MT_pc1], [MT_pc2], [MT_H1Rapid], [MT_H1Feed], [MT_H1Spindle], [MT_H2Rapid], [MT_H2Feed], [MT_H2Spindle], [MT_Gantry], [Kiosk_Override]," +
                " [SAP_Quantity], [DAY_OF_WEEK], [Loader_Type], [Kiosk_Op], [Kiosk_CN], [Kiosk_Setuptime], [SAP_Setuptime], [SAP_Machine_Cycletime], [SAP_Base_Quantity], [Kiosk_State]" +
                " FROM [ProductionMonitor].[dbo].[" + MachineBrand + machine + "_ProdMonitor] where ServerTime" +
                " between '" + StartDate + "' and '" + EndDate + "';";
            using var cmd = new SqlCommand(sql, con);
            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Data.ID = rdr.GetInt64(0);
                Data.Plant = rdr.GetString(1);
                Data.WorkCenter = rdr.GetString(2);
                Data.Machine = rdr.GetString(3);
                Data.Mtime = rdr.GetDateTime(4);
                Data.Material = rdr.GetString(5);
                Data.ProductionOrder = rdr.GetString(6);
                Data.Operator = rdr.GetString(7);
                Data.ProdSupervisor = rdr.GetString(8);
                Data.Event = rdr.GetString(9);
                Data.Value = rdr.GetInt32(10);
                Data.Stime = rdr.GetDateTime(11);

                // new columns
                Data.SAP_Cycletime = TimeSpan.FromSeconds(rdr.GetInt64(12));
                Data.SAP_Loadtime = TimeSpan.FromSeconds(rdr.GetInt64(13));
                Data.Actual_Cycletime = TimeSpan.FromSeconds(rdr.GetInt64(14));
                Data.Actual_Loadtime = TimeSpan.FromSeconds(rdr.GetInt64(15));
                
                Data.SAP_SerialNumber = rdr.GetInt32(16);
                Data.Serial_Internal = rdr.GetInt32(17);
                Data.Result = rdr.GetBoolean(18);
                Data.WShift = rdr.GetString(19);
                Data.MT_state = rdr.GetString(20);
                Data.MT_Partcount1 = rdr.GetInt32(21);
                Data.MT_partcount2 = rdr.GetInt32(22);
                Data.MT_H1Rapid = rdr.GetInt16(23);
                Data.MT_H1Feed = rdr.GetInt16(24);
                Data.MT_H1Spindle = rdr.GetInt16(25);
                Data.MT_H2Rapid = rdr.GetInt16(26);
                Data.MT_H2Feed = rdr.GetInt16(27);
                Data.MT_H2Spindle = rdr.GetInt16(28);
                Data.MT_Gantry = rdr.GetInt16(29);
                Data.Kiosk_Override = rdr.GetBoolean(30);

                Data.SAP_Quantity = rdr.GetInt32(31);
                Data.Day_Of_Week = rdr.GetString(32);
                Data.Loader_Type = rdr.GetString(33);
                Data.KioskOp = rdr.GetString(34);
                Data.KioskCN = rdr.GetString(35);
                Data.Kiosk_SetupTime = TimeSpan.FromSeconds(rdr.GetInt64(36)); // actual set time
                Data.SAP_SetupTime = TimeSpan.FromSeconds(rdr.GetInt64(37)); //ideal setup time
                Data.SAP_Machine_Cycletime = TimeSpan.FromSeconds(rdr.GetInt64(38));
                Data.SAP_Base_Quantity = rdr.GetInt32(39);
                Data.Kiosk_State = rdr.GetString(40);
                //! find the shift for the log
                // we are going to make 5:00 am the start point 
                Data.shift = FindShift(Data.Stime, Startshift1, Startshift2, StartshiftOvernight, Startshift3, EndShift3, SchedType);
                    

                QueryData[row] = Data;
                row++;
                Array.Resize(ref QueryData, row + 1);
            }

            rdr.Close();

            // new section V0.1
            long PrevStateFinder = 1; 
            long FirstLog;
            bool StateFound = false; // will turn true when an event gives a state 
            row = 0;
            
            if (QueryData.Length>2)
            {
                do // find the first state log before the query data
                {
                    FirstLog = QueryData[1].ID - PrevStateFinder;

                    // Console.WriteLine(FirstLog);
                    string sql2 = "SELECT [ID], [Plant], [WorkCenter], [Machine], [ControllerTime], [Material], [ProductionOrder], [Operator], [ProdSupervisor], [Event], [Value], [ServerTime]," +
                        " [SAP_Cycletime], [SAP_Loadtime], [Actual_Cycletime], [Actual_Loadtime], [SAP_SerialNumber], [Serial_Internal], [Result], [WShift]," +
                        " [MT_state], [MT_pc1], [MT_pc2], [MT_H1Rapid], [MT_H1Feed], [MT_H1Spindle], [MT_H2Rapid], [MT_H2Feed], [MT_H2Spindle], [MT_Gantry], [Kiosk_Override]," +
                        " [SAP_Quantity], [DAY_OF_WEEK], [Loader_Type], [Kiosk_Op], [Kiosk_CN], [Kiosk_Setuptime], [SAP_Setuptime], [SAP_Machine_Cycletime], [SAP_Base_Quantity], [Kiosk_State]" +
                        " FROM [ProductionMonitor].[dbo].[" + MachineBrand + machine + "_ProdMonitor] where ID="+ FirstLog.ToString() +";";
                    using var cmd2 = new SqlCommand(sql2, con);
                    using SqlDataReader rdr2 = cmd2.ExecuteReader();

                    while (rdr2.Read())
                    {
                        //original columns
                        Data.ID = rdr2.GetInt64(0);
                        Data.Plant = rdr2.GetString(1);
                        Data.WorkCenter = rdr2.GetString(2);
                        Data.Machine = rdr2.GetString(3);
                        Data.Mtime = rdr2.GetDateTime(4);
                        Data.Material = rdr2.GetString(5);
                        Data.ProductionOrder = rdr2.GetString(6);
                        Data.Operator = rdr2.GetString(7);
                        Data.ProdSupervisor = rdr2.GetString(8);
                        Data.Event = rdr2.GetString(9);
                        Data.Value = rdr2.GetInt32(10);
                        Data.Stime = rdr2.GetDateTime(11);
                        // new columns

                        // new columns
                        Data.SAP_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(12));
                        Data.SAP_Loadtime = TimeSpan.FromSeconds(rdr2.GetInt64(13));
                        Data.Actual_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(14));
                        Data.Actual_Loadtime = TimeSpan.FromSeconds(rdr2.GetInt64(15));

                        Data.SAP_SerialNumber = rdr2.GetInt32(16);
                        Data.Serial_Internal = rdr2.GetInt32(17);
                        Data.Result = rdr2.GetBoolean(18);
                        Data.WShift = rdr2.GetString(19);
                        Data.MT_state = rdr2.GetString(20);
                        Data.MT_Partcount1 = rdr2.GetInt32(21);
                        Data.MT_partcount2 = rdr2.GetInt32(22);
                        Data.MT_H1Rapid = rdr2.GetInt16(23);
                        Data.MT_H1Feed = rdr2.GetInt16(24);
                        Data.MT_H1Spindle = rdr2.GetInt16(25);
                        Data.MT_H2Rapid = rdr2.GetInt16(26);
                        Data.MT_H2Feed = rdr2.GetInt16(27);
                        Data.MT_H2Spindle = rdr2.GetInt16(28);
                        Data.MT_Gantry = rdr2.GetInt16(29);
                        Data.Kiosk_Override = rdr2.GetBoolean(30);

                        Data.SAP_Quantity = rdr2.GetInt32(31);
                        Data.Day_Of_Week = rdr2.GetString(32);
                        Data.Loader_Type = rdr2.GetString(33);
                        Data.KioskOp = rdr2.GetString(34);
                        Data.KioskCN = rdr2.GetString(35);
                        Data.Kiosk_SetupTime = TimeSpan.FromSeconds(rdr2.GetInt64(36)); // actual set time
                        Data.SAP_SetupTime = TimeSpan.FromSeconds(rdr2.GetInt64(37)); //ideal setup time
                        Data.SAP_Machine_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(38));
                        Data.SAP_Base_Quantity = rdr2.GetInt32(39);
                        Data.Kiosk_State = rdr2.GetString(40);
                        //! find the shift for the log
                        // we are going to make 5:00 am the start point 
                        Data.shift = FindShift(Data.Stime, Startshift1, Startshift2, StartshiftOvernight, Startshift3, EndShift3, SchedType);

                        QueryData[0] = Data;
                        //row=1;
                    }
                    rdr2.Close();
                    if (QueryData[0].Event.Contains("RUNNING") || QueryData[0].Event.Contains("MANUAL") || QueryData[0].Event.Contains("IDLE") || QueryData[0].Event.Contains("DOWN"))
                    {
                        StateFound=true;
                        //break;
                    }
                    PrevStateFinder++;

                }
                while (StateFound != true);
            }
            else  // find the last known state with no state logs 
            {
                DateTime Start = DateTime.Parse(StartDate);
                string searchrange = Start.AddHours(-3).ToString();

                string sql2 = "SELECT [ID], [Plant], [WorkCenter], [Machine], [ControllerTime], [Material], [ProductionOrder], [Operator], [ProdSupervisor], [Event], [Value], [ServerTime]," +
                    " [SAP_Cycletime], [SAP_Loadtime], [Actual_Cycletime], [Actual_Loadtime], [SAP_SerialNumber], [Serial_Internal], [Result], [WShift]," +
                    " [MT_state], [MT_pc1], [MT_pc2], [MT_H1Rapid], [MT_H1Feed], [MT_H1Spindle], [MT_H2Rapid], [MT_H2Feed], [MT_H2Spindle], [MT_Gantry], [Kiosk_Override]," +
                    " [SAP_Quantity], [DAY_OF_WEEK], [Loader_Type], [Kiosk_Op], [Kiosk_CN], [Kiosk_Setuptime], [SAP_Setuptime], [SAP_Machine_Cycletime], [SAP_Base_Quantity], [Kiosk_State]" +
                    " FROM [ProductionMonitor].[dbo].[" + MachineBrand + machine + "_ProdMonitor] where ServerTime" +
                    " between '" + searchrange + "' and '" + StartDate + "';";
                using var cmd2 = new SqlCommand(sql2, con);
                using SqlDataReader rdr2 = cmd2.ExecuteReader();

                while (rdr2.Read())
                {
                    Data.ID = rdr2.GetInt64(0);
                    Data.Plant = rdr2.GetString(1);
                    Data.WorkCenter = rdr2.GetString(2);
                    Data.Machine = rdr2.GetString(3);
                    Data.Mtime = rdr2.GetDateTime(4);
                    Data.Material = rdr2.GetString(5);
                    Data.ProductionOrder = rdr2.GetString(6);
                    Data.Operator = rdr2.GetString(7);
                    Data.ProdSupervisor = rdr2.GetString(8);
                    Data.Event = rdr2.GetString(9);
                    Data.Value = rdr2.GetInt32(10);
                    Data.Stime = rdr2.GetDateTime(11);
                    // new columns
                    Data.SAP_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(12));
                    Data.SAP_Loadtime = TimeSpan.FromSeconds(rdr2.GetInt64(13));
                    Data.Actual_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(14));
                    Data.Actual_Loadtime = TimeSpan.FromSeconds(rdr2.GetInt64(15));

                    Data.SAP_SerialNumber = rdr2.GetInt32(16);
                    Data.Serial_Internal = rdr2.GetInt32(17);
                    Data.Result = rdr2.GetBoolean(18);
                    Data.WShift = rdr2.GetString(19);
                    Data.MT_state = rdr2.GetString(20);
                    Data.MT_Partcount1 = rdr2.GetInt32(21);
                    Data.MT_partcount2 = rdr2.GetInt32(22);
                    Data.MT_H1Rapid = rdr2.GetInt16(23);
                    Data.MT_H1Feed = rdr2.GetInt16(24);
                    Data.MT_H1Spindle = rdr2.GetInt16(25);
                    Data.MT_H2Rapid = rdr2.GetInt16(26);
                    Data.MT_H2Feed = rdr2.GetInt16(27);
                    Data.MT_H2Spindle = rdr2.GetInt16(28);
                    Data.MT_Gantry = rdr2.GetInt16(29);
                    Data.Kiosk_Override = rdr2.GetBoolean(30);

                    Data.SAP_Quantity = rdr2.GetInt32(31);
                    Data.Day_Of_Week = rdr2.GetString(32);
                    Data.Loader_Type = rdr2.GetString(33);
                    Data.KioskOp = rdr2.GetString(34);
                    Data.KioskCN = rdr2.GetString(35);
                    Data.Kiosk_SetupTime = TimeSpan.FromSeconds(rdr2.GetInt64(36)); // actual set time
                    Data.SAP_SetupTime = TimeSpan.FromSeconds(rdr2.GetInt64(37)); //ideal setup time
                    Data.SAP_Machine_Cycletime = TimeSpan.FromSeconds(rdr2.GetInt64(38));
                    Data.SAP_Base_Quantity = rdr2.GetInt32(39);
                    Data.Kiosk_State = rdr2.GetString(40);

                    //! find the shift for the log
                    // we are going to make 5:00 am the start point 
                    Data.shift = FindShift(Data.Stime, Startshift1, Startshift2, StartshiftOvernight, Startshift3, EndShift3, SchedType);

                    QueryData[0] = Data;
                    //row=1;
                }
                rdr2.Close();
                if (QueryData[0].Event.Contains("RUNNING") || QueryData[0].Event.Contains("MANUAL") || QueryData[0].Event.Contains("IDLE") || QueryData[0].Event.Contains("DOWN"))
                {
                    StateFound=true;
                    //break;
                }
                PrevStateFinder++;
            }
            
            return QueryData;
        }
        //! Read From the sql server

        // shift grabber
        private static string FindShift(DateTime Stime, TimeSpan Startshift1, TimeSpan Startshift2, TimeSpan StartshiftOvernight, TimeSpan Startshift3, TimeSpan EndShift3, string Center)
        {
            string shift;

            switch (Center)
            {
                case "mach":
                    // weekend   -> we are going to make 5:00 am the start point 
                    if ((Stime.AddHours(-5).DayOfWeek == DayOfWeek.Friday) || Stime.DayOfWeek == DayOfWeek.Saturday || Stime.AddHours(-5).DayOfWeek == DayOfWeek.Sunday
                        || Stime.DayOfWeek == DayOfWeek.Sunday || Stime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (Stime.TimeOfDay >= Startshift3 && Stime.TimeOfDay < EndShift3)
                        {
                            shift = "3rd";
                        }
                        else
                        {
                            shift = "Overnight2";
                        }
                    }
                    // week
                    else
                    {
                        if (Stime.TimeOfDay >= Startshift1 && Stime.TimeOfDay < Startshift2)             // 1st shift
                        {
                            shift = "1st";
                        }
                        else if (Stime.TimeOfDay >= StartshiftOvernight && Stime.TimeOfDay < Startshift1) // overnight
                        {
                            shift = "Overnight";
                        }
                        else                                                                                       // 2nd shift
                        {
                            shift = "2nd";
                        }
                    }
                    break;
                default: // line
                         // weekend   -> we are going to make 6:00 am the start point 
                    if ((Stime.AddHours(-6).DayOfWeek == DayOfWeek.Saturday) || Stime.AddHours(-6).DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (Stime.TimeOfDay >= Startshift3 && Stime.TimeOfDay < EndShift3)
                        {
                            shift = "3rd";
                        }
                        else
                        {
                            shift = "Overnight2";
                        }
                    }
                    // week
                    else
                    {
                        if (Stime.TimeOfDay >= Startshift1 && Stime.TimeOfDay < Startshift2)             // 1st shift
                        {
                            shift = "1st";
                        }
                        else if (Stime.TimeOfDay >= StartshiftOvernight && Stime.TimeOfDay < Startshift1) // overnight
                        {
                            shift = "Overnight";
                        }
                        else                                                                                       // 2nd shift
                        {
                            shift = "2nd";
                        }
                    }
                    break;

            }


            return shift;
        }

        // organize the sql table into lists
        private static ServiceData SortTree(ServiceData Data)
        {
            Array.Clear(Data.StateData);
            Array.Clear(Data.PartData);
            Array.Clear(Data.H1Rap);
            Array.Clear(Data.H2Rap);
            Array.Clear(Data.H1Feed);
            Array.Clear(Data.H2Feed);
            Array.Clear(Data.H1Spind);
            Array.Clear(Data.H2Spind);
            Array.Clear(Data.Gantry);

            int EventRow = 0, PartRow = 0;
            int H1RapRow = 0, H2RapRow = 0, H1FeedRow = 0, H2FeedRow = 0, H1SpinRow = 0, H2SpinRow = 0;
            int GantryRow = 0;
            int KioskRow = 0;


            for (int rawRow = 0; rawRow < Data.GenData.Length - 1; rawRow++)
            {
                switch (Data.GenData[rawRow].Event)
                {
                  // PartCount Changes 
                    case string a when a.Contains("Part"):
                        Data.PartData[PartRow] = Data.GenData[rawRow];
                        PartRow++;
                        Array.Resize(ref Data.PartData, PartRow + 1); // make the array bigger
                        break;
                    // PartCount Changes adapted 
                    case string a when a.Contains("PART"):
                        Data.PartData[PartRow] = Data.GenData[rawRow];
                        PartRow++;
                        Array.Resize(ref Data.PartData, PartRow + 1); // make the array bigger
                        break;
                    // overrides 
                    case string a when a.Contains("H1_Rap"):
                        Data.H1Rap[H1RapRow] = Data.GenData[rawRow];
                        H1RapRow++;
                        Array.Resize(ref Data.H1Rap, H1RapRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H1_RAP"):
                        Data.H1Rap[H1RapRow] = Data.GenData[rawRow];
                        H1RapRow++;
                        Array.Resize(ref Data.H1Rap, H1RapRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_Rap"):
                        Data.H2Rap[H2RapRow] = Data.GenData[rawRow];
                        H2RapRow++;
                        Array.Resize(ref Data.H2Rap, H2RapRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_RAP"):
                        Data.H2Rap[H2RapRow] = Data.GenData[rawRow];
                        H2RapRow++;
                        Array.Resize(ref Data.H2Rap, H2RapRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H1_Feed"):
                        Data.H1Feed[H1FeedRow] = Data.GenData[rawRow];
                        H1FeedRow++;
                        Array.Resize(ref Data.H1Feed, H1FeedRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H1_FEED"):
                        Data.H1Feed[H1FeedRow] = Data.GenData[rawRow];
                        H1FeedRow++;
                        Array.Resize(ref Data.H1Feed, H1FeedRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_Feed"):
                        Data.H2Feed[H2FeedRow] = Data.GenData[rawRow];
                        H2FeedRow++;
                        Array.Resize(ref Data.H2Feed, H2FeedRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_FEED"):
                        Data.H2Feed[H2FeedRow] = Data.GenData[rawRow];
                        H2FeedRow++;
                        Array.Resize(ref Data.H2Feed, H2FeedRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H1_Spin"):
                        Data.H1Spind[H1SpinRow] = Data.GenData[rawRow];
                        H1SpinRow++;
                        Array.Resize(ref Data.H1Spind, H1SpinRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H1_SPIN"):
                        Data.H1Spind[H1SpinRow] = Data.GenData[rawRow];
                        H1SpinRow++;
                        Array.Resize(ref Data.H1Spind, H1SpinRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_Spin"):
                        Data.H2Spind[H2SpinRow] = Data.GenData[rawRow];
                        H2SpinRow++;
                        Array.Resize(ref Data.H2Spind, H2SpinRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("H2_SPIN"):
                        Data.H2Spind[H2SpinRow] = Data.GenData[rawRow];
                        H2SpinRow++;
                        Array.Resize(ref Data.H2Spind, H2SpinRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("Gantry"):
                        Data.Gantry[GantryRow] = Data.GenData[rawRow];
                        GantryRow++;
                        Array.Resize(ref Data.Gantry, GantryRow + 1); // make the array bigger
                        break;
                    case string a when a.Contains("GANTRY"):
                        Data.Gantry[GantryRow] = Data.GenData[rawRow];
                        GantryRow++;
                        Array.Resize(ref Data.Gantry, GantryRow + 1); // make the array bigger
                        break;
                    default: 
                        if (Data.GenData[rawRow].Event.Contains("RUNNING") || Data.GenData[rawRow].Event.Contains("IDLE") ||
                        Data.GenData[rawRow].Event.Contains("MANUAL") || Data.GenData[rawRow].Event.Contains("DOWN") || Data.GenData[rawRow].Event.Contains("BeginShift")/*|| Data.GenData[rawRow].Event.Contains("LOADING")*/) // machine state data
                        {
                            Data.StateData[EventRow] = Data.GenData[rawRow]; // log the data as an event
                            EventRow++;
                            Array.Resize(ref Data.StateData, EventRow + 1); // make the array bigger
                        }
                        else // kiosk or other program data
                        {
                            Data.KioskData[KioskRow] = Data.GenData[rawRow];
                            KioskRow++;
                            Array.Resize(ref Data.KioskData, KioskRow + 1); // make the array bigger
                        }
                        break;
                }

            }
            return Data;
        }

    }
}
