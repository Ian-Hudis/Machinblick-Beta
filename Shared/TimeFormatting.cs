namespace Maschinblick.Shared
{
    public class TimeFormatting
    {
        // default values for start and end date
        public string StartDate = DateTime.Today.AddDays(-1).AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"); //= DataSave.Savedata.StartDate;
        public string EndDate = DateTime.Today.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"); //= DataSave.Savedata.EndDate;

        public void StartEarlier()  //minus sign
        {
            try
            {
                DateTime DateStarts = DateTime.Parse(StartDate);
                DateStarts = DateStarts.AddDays(-1);
                StartDate = DateStarts.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                Console.WriteLine("Failed to Parse The Start date (StartEarlier function)");
            }

        }

        public void StartLater() // plus sign
        {
            try
            {
                DateTime DateStarts = DateTime.Parse(StartDate);
                DateStarts = DateStarts.AddDays(1);
                StartDate = DateStarts.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                Console.WriteLine("Failed to Parse The Start date (StartLater function)");
            }

        }

        public void EndEarlier() //minus sign
        {
            try
            {
                DateTime DateEnds = DateTime.Parse(EndDate);
                DateEnds = DateEnds.AddDays(-1);
                EndDate = DateEnds.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                Console.WriteLine("Failed to Parse The End date (EndEarlier function)");
            }

        }

        public void EndLater() // plus sign
        {
            try
            {
                DateTime DateEnds = DateTime.Parse(EndDate);
                DateEnds = DateEnds.AddDays(1);
                EndDate = DateEnds.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                Console.WriteLine("Failed to Parse The End date (EndLater function)");
            }

        }

        public void SetToNow() // set to now button
        {
            EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }


    }
}
