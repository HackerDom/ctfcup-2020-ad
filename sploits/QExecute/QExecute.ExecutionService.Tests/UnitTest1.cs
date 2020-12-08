using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using QueenOfHearts.ExecutionService.Configuration;
using QueenOfHearts.ExecutionService.OpLog;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Commons.Time;
using Vostok.Logging.Console;

namespace QExecuteTests
{
    public class Tests
    {
        private static readonly Random random = new();

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void WriteTest()
        {
            var settingsProvider = new SettingsProvider();
            File.Delete(settingsProvider.ApplicationSettings.OpLogFileName);
            Console.WriteLine(Directory.GetCurrentDirectory());

            var manager = new OpLogManager(settingsProvider, new ConsoleLog());
            var newGuid = Guid.NewGuid();
            var addExecutorOperation = new AddExecutorOperation("test", newGuid.ToString());


            var start = DateTime.UtcNow;

            var count = 0;
            while (DateTime.UtcNow - start < 10.Seconds())
            {
                manager.Write(addExecutorOperation);
                count++;
            }

            Console.WriteLine(count / 10);
        }

        [Test]
        public void Test_OpLogAttack()
        {
            var settingsProvider = new SettingsProvider();
            File.Delete(settingsProvider.ApplicationSettings.OpLogFileName);
            Console.WriteLine(Directory.GetCurrentDirectory());

            var manager = new OpLogManager(settingsProvider, new ConsoleLog());
            var newGuid = Guid.NewGuid();
            var addExecutorOperation = new AddExecutorOperation("test", newGuid.ToString());

            var newGuid2 = Guid.NewGuid();
            var addExecutorOperation2 = new AddExecutorOperation("test2", newGuid.ToString());

            var badAdmin = "admin";
            var victimCommand = "command";
            var payload = BuildPayloadString(1, badAdmin, victimCommand);

            manager.Write(addExecutorOperation);
            try
            {
                manager.Write(new AddCommandOperation(payload, null));
            }
            catch (Exception e)
            {
            }

            manager.Write(addExecutorOperation2);

            var op = manager.Read().ToArray();
            op.Length.Should().Be(3);

            var op1 = op[0] as AddExecutorOperation;
            var op2 = op[1] as AddCommandAdminOperation;
            var op3 = op[2] as AddExecutorOperation;

            op1.ApiKey.Should().Be(addExecutorOperation.ApiKey);
            op1.ExecutorId.Should().Be(addExecutorOperation.ExecutorId);

            op2.ApiKey.Should().Be(badAdmin);
            op2.CommandName.Should().Be(victimCommand);

            op3.ApiKey.Should().Be(addExecutorOperation2.ApiKey);
            op3.ExecutorId.Should().Be(addExecutorOperation2.ExecutorId);
        }

        private static string BuildPayloadString(int opcode, string victimCommand, string badAdmin)
        {
            var first = Encoding.UTF32.GetBytes(badAdmin);
            var second = Encoding.UTF32.GetBytes(victimCommand);
            var buffer = new List<byte>();

            buffer.AddRange(GetIntBytes(OpLogManager.Marker));
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
            }

            sequenceEqual.Should().BeTrue();
            return result;
        }

        private static void GetMaxInvariantValue()
        {
            var max = 0;
            for (var i = 0; i < int.MaxValue; i++)
            {
                var bytes = GetIntBytes(max);
                var str = Encoding.UTF32.GetString(bytes) + "a";
                if (bytes.SequenceEqual(Encoding.UTF32.GetBytes(str).Take(4)))
                    max = i;
                else
                    break;
            }

            Console.WriteLine(max);
        }

        private static void PrintBytes(byte[] buffer)
        {
            Console.WriteLine(GetString(buffer));
        }

        private static string GetString(byte[] buffer)
        {
            return string.Join(" ", buffer.Select(b => b.ToString()));
        }


        private static string GetRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GetIntString(int i)
        {
            return new(GetIntBytes(i).Select(b => (char) b).ToArray());
        }

        private static void PrintInt(int i)
        {
            Console.WriteLine(string.Join(" ", GetIntBytes(i)));
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