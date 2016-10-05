using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TCPChatRoomProjectServerSide
{
    public class DataLogger
    {
        private DateTime timeOfLog;
        private string logFile;

        public DataLogger()
        {
            logFile = "dataLog.txt";
        }

        public string GetMessageWithDateAndTime(string message)
        {
            timeOfLog = DateTime.Now;
            string newMessage = timeOfLog.ToString() + " : " + message;
            return newMessage;
        }

        public void AddToLog(string message)
        {
            try
            {
                StreamWriter writer;
                writer = new StreamWriter(logFile, true);
                writer.WriteLine(GetMessageWithDateAndTime(message));
                writer.Close();
            }
            catch
            {

                Console.WriteLine("An error occurred while trying to write to {0}", logFile);
            }
        }

        public string GetLog()
        {
            try
            {
                StreamReader reader;
                reader = new StreamReader(logFile);
                string log = reader.ReadToEnd();
                reader.Close();
                return log;
            }
            catch
            {
                string errorMessage = "An error ocurred while trying to read from " + logFile;
                return errorMessage;
            }
        }
    }
}
