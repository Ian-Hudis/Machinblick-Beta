using Microsoft.VisualBasic;
using Radzen;
using System.Data;
using System.Net.Http.Headers;

namespace Maschinblick.Data
{
    //! Data for tables
    /*
    public class DataExtraction
    {
        public long ID { get; set; }
        public string? Plant { get; set; }
        public string? WorkCenter { get; set; }
        public string? Machine { get; set; }
        public string? TimeMark { get; set; }
        public string? Material { get; set; }
        public string? ProductionOrder { get; set; }
        public string? Operator { get; set; }
        public string? ProdSupervisor { get; set; }
        public string? Event { get; set; }
        public int Value { get; set; }
        public string? ServerTime { get; set; }

        // added later
        public string? Shift { get; set; }
    }
    */
    // old fashion table from V0.1
    /*
   public class DeviceData
   {

       public Task<DataExtraction[]> GetData(int row, SQL_Client.SQL_Reader[] data)
       {
           return Task.FromResult(
               Enumerable.Range(1, 1).Select(index => new DataExtraction
               {
                   ID = data[row].ID,
                   Plant = data[row].Plant,
                   WorkCenter = data[row].WorkCenter,
                   Machine = data[row].Machine,
                   TimeMark = data[row].Mtime.ToString(),
                   Material = data[row].Material,
                   ProductionOrder = data[row].ProductionOrder,
                   Operator = data[row].Operator,
                   ProdSupervisor = data[row].ProdSupervisor,
                   Event = data[row].Event,
                   Value = data[row].Value,
                   ServerTime = data[row].Stime.ToString(),
                   Shift = data[row].shift
               }).ToArray());
       }


   }
   */

    //! Data for tables
    public class Datalog_column
	{
		public long ID { get; set; }
		public string? ServerTime { get; set; }
		public string? Event { get; set; }
		public int Value { get; set; }
		public string? ProdOrder { get; set; }
		public string? Material { get; set; }
		public string? Shift { get; set; }
		public string? Op { get; set; }
		public string? Sup { get; set; }

        // new columns from sql
        public string? SAP_Cycletime { get; set; }
        public string? SAP_Loadtime { get; set; }
        public string? Actual_Cycletime { get; set; }
        public string? Actual_Loadtime { get; set; }
        public string? SAP_SerialNumber { get; set; }
        public string? Serial_Internal { get; set; }
        public bool? Result { get; set; }
        public string? Wshift { get; set; }
        public string? MTConnectState { get; set; }
        public int MT_PC1 { get; set; }
        public int MT_PC2 { get; set; }
        public short MT_H1Rapid { get; set; }
        public short MT_H1Feed { get; set; }
        public short MT_H1Spindle { get; set; }
        public short MT_H2Rapid { get; set; }
        public short MT_H2Feed { get; set; }
        public short MT_H2Spindle { get; set; }
        public short MT_Gantry { get; set; }

        public string? KioskOverride { get; set; }

        public int SAP_Quanitity { get; set; }
        public string? DAY_OF_Week { get; set; }
        public string? Loader_Type { get; set; }
        public string? Kiosk_OP { get; set; }
        public string? Kiosk_ConfirmationNumber { get; set; }
        public string? Kiosk_Setuptime { get; set; }
        public string? SAP_SetupTime { get; set; }
        public string? SAP_Machine_Cycletime { get; set; }
        public int SAP_Base_Quantity { get; set; }
        public string? Kiosk_State { get; set; }

        // generic info
        public string? Plant { get; set; }
		public string? WorkCenter { get; set; }
	}

    // new datalog table for V0.2+
	public class Datalog_Service
	{
        public List<Datalog_column> Datalog_columns = new();

        // function for updating the values on the datagrid
        private async Task UpdateList(SQL_Client.SQL_Reader[] tablecontent)
        {
            Datalog_columns.Clear();

            for (int rowcount=1; rowcount<tablecontent.Length-1; rowcount++)
            {
                Datalog_column row = new()
                {
                    ID = tablecontent[rowcount].ID,
                    ServerTime = tablecontent[rowcount].Stime.ToString(),
                    Event = tablecontent[rowcount].Event,
                    Value = tablecontent[rowcount].Value,
                    ProdOrder = tablecontent[rowcount].ProductionOrder,
                    Material = tablecontent[rowcount].Material,
                    Shift = tablecontent[rowcount].shift,
                    Op = tablecontent[rowcount].Operator,
                    Sup = tablecontent[rowcount].ProdSupervisor,

                    SAP_Cycletime = tablecontent[rowcount].SAP_Cycletime.ToString(),
                    SAP_Loadtime = tablecontent[rowcount].SAP_Loadtime.ToString(),
                    Actual_Cycletime = tablecontent[rowcount].Actual_Cycletime.ToString(),
                    Actual_Loadtime = tablecontent[rowcount].Actual_Loadtime.ToString(),
                    SAP_SerialNumber = tablecontent[rowcount].SAP_SerialNumber.ToString(),
                    Serial_Internal = tablecontent[rowcount].Serial_Internal.ToString(),
                    Result = tablecontent[rowcount].Result,
                    Wshift = tablecontent[rowcount].WShift,

                    MTConnectState = tablecontent[rowcount].MT_state,
                    MT_PC1 = tablecontent[rowcount].MT_Partcount1,
                    MT_PC2 = tablecontent[rowcount].MT_partcount2,

                    MT_H1Rapid = tablecontent[rowcount].MT_H1Rapid,
                    MT_H1Feed = tablecontent[rowcount].MT_H1Feed,
                    MT_H1Spindle = tablecontent[rowcount].MT_H1Spindle,
                    MT_H2Rapid = tablecontent[rowcount].MT_H2Rapid,
                    MT_H2Feed = tablecontent[rowcount].MT_H2Feed,
                    MT_H2Spindle = tablecontent[rowcount].MT_H2Spindle,
                    MT_Gantry = tablecontent[rowcount].MT_Gantry,

                    KioskOverride = tablecontent[rowcount].Kiosk_Override.ToString(),
                    SAP_Quanitity = tablecontent[rowcount].SAP_Quantity,
                    DAY_OF_Week = tablecontent[rowcount].Day_Of_Week,
                    Loader_Type = tablecontent[rowcount].Loader_Type,
                    Kiosk_OP = tablecontent[rowcount].KioskOp,
                    Kiosk_ConfirmationNumber = tablecontent[rowcount].KioskCN,
                    Kiosk_Setuptime = tablecontent[rowcount].Kiosk_SetupTime.ToString(),
                    SAP_SetupTime = tablecontent[rowcount].SAP_SetupTime.ToString(),
                    SAP_Machine_Cycletime = tablecontent[rowcount].SAP_Machine_Cycletime.ToString(),
                    SAP_Base_Quantity = tablecontent[rowcount].SAP_Base_Quantity,
                    Kiosk_State = tablecontent[rowcount].Kiosk_State,

                    Plant = tablecontent[rowcount].Plant,
                    WorkCenter = tablecontent[rowcount].WorkCenter
                };
                Datalog_columns.Add(row);
            }

            await Task.FromResult(Datalog_columns);
        }

        public async Task<List<Datalog_column>> ProductList(SQL_Client.SQL_Reader[] tablecontent)
		{
            await UpdateList(tablecontent);
            return await Task.FromResult(Datalog_columns);
		}

        // datatable coloring
        public void CellRender(DataGridCellRenderEventArgs<Datalog_column> col)
        {
           // if(Theme == "Normal")
           // {
                if (col.Column.Property == "ID" ||col.Column.Property == "ServerTime" || col.Column.Property =="Event"|| col.Column.Property =="Value"
                || col.Column.Property == "MTConnectState" || col.Column.Property == "MT_PC1" || col.Column.Property == "MT_PC2"|| col.Column.Property == "SAP_Quanitity")
                {
                    switch (col.Data.MTConnectState)
                    {
                        case string a when a.Contains("RUNNING"):
                            col.Attributes.Add("style", $"font-weight: {("400")};" +
                                $"background-image: linear-gradient(180deg, #E0ffE5 0%, #78ff85 100%);");
                            //                     $"background-color: {(col.Data.MachineMode == "RUNNING" ? "#77ff77" : (col.Data.MachineMode == "MANUAL" ? "#00EEFF" : (col.Data.MachineMode == "DOWN" ? "#FF4040" : "#ffff00")))};");
                            break;
                        case string a when a.Contains("IDLE"):
                            col.Attributes.Add("style", $"font-weight: {("400")};" +
                            $"background-image: linear-gradient(180deg, #fff9E0 0%, #ffff10 100%);");
                            //                     $"background-color: {(col.Data.MachineMode == "RUNNING" ? "#77ff77" : (col.Data.MachineMode == "MANUAL" ? "#00EEFF" : (col.Data.MachineMode == "DOWN" ? "#FF4040" : "#ffff00")))};");
                            break;
                        case string a when a.Contains("DOWN"):
                            col.Attributes.Add("style", $"font-weight: {("400")};" +
                            $"background-image: linear-gradient(180deg, #ffE0E0 0%, #FF4242 100%);");
                            break;
                        case string a when a.Contains("MANUAL"):
                            col.Attributes.Add("style", $"font-weight: {("400")};" +
                            $"background-image: linear-gradient(180deg, #E0E4ff 0%, #10EEFF 100%);");
                            break;
                    }
                }
                else if (col.Column.Property == "Kiosk_State")
                {
                    switch (col.Data.Kiosk_State)
                    {
                        case string a when a.Contains("StartUp"): // green
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                                $"background-image: linear-gradient(180deg, #E0ffE5 10%, #78Ef85 100%);");
                            break;
                        case string a when a.Contains("SchedMaint"): // red
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #ffE0E0 10%, #EF4242 100%);");
                            break;
                        case string a when a.Contains("UnschedMaint"): // red
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #ffE0E0 10%, #EF4242 100%);");
                            break;
                        case string a when a.Contains("NoJob"): //yellow
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #fff9E0 10%, #efef10 100%);");
                            break;
                        case string a when a.Contains("TimeOut"): //yellow
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #fff9E0 10%, #efef10 100%);");
                            break;
                        case string a when a.Contains("Setup"): // blue
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0E4ff 10%, #10EEEF 100%);");
                            break;
                        case string a when a.Contains("Inspect"): // blue
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0E4ff 10%, #10EEEF 100%);");
                            break;
                        case string a when a.Contains("startup"): // green
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                                $"background-image: linear-gradient(180deg, #E0ffE5 10%, #78Ef85 100%);");
                            break;
                        case string a when a.Contains("run"): // green
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0ffE5 10%, #78Ef85 100%);");
                            break;
                        case string a when a.Contains("ready"): // green
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0ffE5 10%, #78Ef85 100%);");
                            break;
                        case string a when a.Contains("schedmaint"): // red
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #ffE0E0 10%, #EF4242 100%);");
                            break;
                        case string a when a.Contains("unschedmaint"): // red
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #ffE0E0 10%, #EF4242 100%);");
                            break;
                        case string a when a.Contains("nojob"): //yellow
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #fff9E0 10%, #efef10 100%);");
                            break;
                        case string a when a.Contains("timeout"): //yellow
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #fff9E0 10%, #efef10 100%);");
                            break;
                        case string a when a.Contains("setup"): // blue
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0E4ff 10%, #10EEEF 100%);");
                            break;
                        case string a when a.Contains("inspect"): // blue
                            col.Attributes.Add("style", $"font-weight: {("300")};" +
                            $"background-image: linear-gradient(180deg, #E0E4ff 10%, #10EEEF 100%);");
                            break;
                }
                }
                // override coloring
                switch (col.Column.Property)
                {
                    case "MT_H1Rapid":
                        if (col.Data.MT_H1Rapid > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_H1Feed":
                        if (col.Data.MT_H1Feed > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_H1Spindle":
                        if (col.Data.MT_H1Spindle > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_H2Rapid":
                        if (col.Data.MT_H2Rapid > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_H2Feed":
                        if (col.Data.MT_H2Feed > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_H2Spindle":
                        if (col.Data.MT_H2Spindle > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "MT_Gantry":
                        if (col.Data.MT_Gantry > 99)
                        {
                            col.Attributes.Add("style", $"background-color: #77ff77");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #ff2525");
                        }
                        break;
                    case "KioskOverride":
                        if (col.Data.KioskOverride != "True")
                        {
                            col.Attributes.Add("style", $"background-color: #7777ff");
                        }
                        else
                        {
                            col.Attributes.Add("style", $"background-color: #FF4040");
                        }
                        break;
                }
           // }


        }

        #pragma warning disable IDE0060 // Remove unused parameter
        public void RowRender(RowRenderEventArgs<Datalog_column> args)  // controls the font
        {
           // Density = Density.Compact;
            //  args.Attributes.Add("style", $"font-weight: {("bold")};");
        }
        #pragma warning restore IDE0060 // Remove unused parameter

        public void HeaderFooterCellRender(DataGridCellRenderEventArgs<Datalog_column> args)
        {
            if (args.Column.Property == "MachineID")
            {
                args.Attributes.Add("colspan", 2);
            }
        }

    }

}
