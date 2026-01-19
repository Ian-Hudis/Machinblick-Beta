using Syncfusion.Blazor.PivotView.Internal;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using static Maschinblick.Data.BarData;

namespace Maschinblick.Data
{
    public class RuntimeDiagnoses
    {
        public class RuntimeData
        {
            // the Data that actually goes into the pie charts
            public ShiftRuntime Shift1;
            public ShiftRuntime Shift2;
            public ShiftRuntime Shift3; //the weekend shift
            public ShiftRuntime Overnight;
            public ShiftRuntime Overnight2; //the weekend shift

            public struct ShiftRuntime  // will be in minutes
            {
                public decimal TimeRunning;
                public decimal TimeIdle;
                public decimal TimeManual;
                public decimal TimeDown;
                public decimal future;

                public decimal Denominator;
            }

        }

        public static RuntimeData MakePieCharts(Barset Bardata)
        {
            RuntimeData Piedata = new();
            Piedata.Shift1 = Calc(Piedata.Shift1, Bardata.Bar1);
            Piedata.Shift2 = Calc(Piedata.Shift2, Bardata.Bar2);
            Piedata.Shift3 = Calc(Piedata.Shift3, Bardata.Bar3);   
            Piedata.Overnight = Calc(Piedata.Overnight, Bardata.BarOV);
            Piedata.Overnight2 = Calc(Piedata.Overnight2, Bardata.BarOV2);
            return Piedata;
        }

        private static RuntimeData.ShiftRuntime Calc(RuntimeData.ShiftRuntime shift, BarStructure Input)
        {
            string running = ColorPallet[0];
            string manual = ColorPallet[1];
            string idle = ColorPallet[2];
            string down = ColorPallet[3];
            string future = ColorPallet[4];

            for (int i = 0; i < Input.barcount; i++)
            {
                if (Input.Color[i] == running)
                {
                    shift.TimeRunning += Input.Details[i].Select(ChartData => ChartData.Tvalue).First();
                }
                else if (Input.Color[i] == manual)
                {
                    shift.TimeManual += Input.Details[i].Select(ChartData => ChartData.Tvalue).First();
                }
                else if (Input.Color[i] == idle)
                {
                    shift.TimeIdle += Input.Details[i].Select(ChartData => ChartData.Tvalue).First();
                }
                else if (Input.Color[i] == down)
                {
                    shift.TimeDown += Input.Details[i].Select(ChartData => ChartData.Tvalue).First();
                }
                else // grey
                {
                    shift.future += Input.Details[i].Select(ChartData => ChartData.Tvalue).First();
                }

                shift.Denominator += Input.Details[i].Select(ChartData => ChartData.Tvalue).First(); // calculate the denominator

            }

            return shift;
        }
    }

    //! Data for Runtime

    public class RuntimeExtraction
    {
        public decimal Shift1Running { get; set; }
        public decimal Shift2Running { get; set; }
        public decimal Shift3Running { get; set; }
        public decimal ShiftOVNRunning { get; set; }

        public decimal Shift1Manual { get; set; }
        public decimal Shift2Manual { get; set; }
        public decimal Shift3Manual { get; set; }
        public decimal ShiftOVNManual { get; set; }

        public decimal Shift1Idle { get; set; }
        public decimal Shift2Idle { get; set; }
        public decimal Shift3Idle { get; set; }
        public decimal ShiftOVNIdle { get; set; }

        public decimal Shift1Down { get; set; }
        public decimal Shift2Down { get; set; }
        public decimal Shift3Down { get; set; }
        public decimal ShiftOVNDown { get; set; }
    }

    public class RuntimeDataDisplay
    {
        public Task<RuntimeExtraction[]> GetData(RuntimeDiagnoses.RuntimeData data)
        {
            return Task.FromResult(Enumerable.Range(1, 4).Select(index => new RuntimeExtraction
            {
                Shift1Running = data.Shift1.TimeRunning,
                Shift2Running = data.Shift2.TimeRunning,
                Shift3Running = data.Shift3.TimeRunning,
                ShiftOVNRunning = data.Overnight.TimeRunning,

                Shift1Manual = data.Shift1.TimeManual,
                Shift2Manual= data.Shift2.TimeManual,
                Shift3Manual= data.Shift3.TimeManual,
                ShiftOVNManual= data.Overnight.TimeManual,

                Shift1Idle = data.Shift1.TimeIdle,
                Shift2Idle = data.Shift2.TimeIdle,
                Shift3Idle = data.Shift3.TimeIdle,
                ShiftOVNIdle = data.Overnight.TimeIdle,

                Shift1Down = data.Shift1.TimeDown,
                Shift2Down = data.Shift2.TimeDown,
                Shift3Down = data.Shift3.TimeDown,
                ShiftOVNDown = data.Overnight.TimeDown
            }).ToArray());
        }

        public class DataItem
        {
            public string? State { get; set; }
            public double TimeAmount { get; set; }
        }


        public static DataItem[] PieMaker(decimal Running, decimal Manual, decimal Idle, decimal Down)
        {
            DataItem[] dataitem = new DataItem[]
                {
                new DataItem
                {
                    State = "Running",
                    TimeAmount = 0
                },
                new DataItem
                {
                    State = "Manual",
                    TimeAmount = 0
                },
                new DataItem
                {
                    State = "Idle",
                    TimeAmount = 0
                },
                new DataItem
                {
                    State = "Down",
                    TimeAmount = 0
                }
                };


            decimal denominator = Running + Manual + Idle + Down;

            if (denominator > decimal.Zero)
            {
                dataitem = new DataItem[]
                {
                new DataItem
                {
                    State = "Running",
                    TimeAmount = decimal.ToDouble(Running/denominator)
                },
                new DataItem
                {
                    State = "Manual",
                    TimeAmount = decimal.ToDouble(Manual/denominator)
                },
                new DataItem
                {
                    State = "Idle",
                    TimeAmount = decimal.ToDouble(Idle/denominator)
                },
                new DataItem
                {
                    State = "Down",
                    TimeAmount = decimal.ToDouble(Down/denominator)
                }
                };
            }
            return dataitem;
        }


    }

    //! Data for Runtime
}
