using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NModbus;
using NModbus.IO;
using System.Net.Sockets;
using NModbus.Utility;
using NModbus.Device;
using System.Threading;

namespace MVVMBase.MVVM.Model
{
    public class DataReader
    {
        ModbusFactory factory;
        Task readingTask;
        bool StopReadingSetted;

        CancellationTokenSource tokenSource2; 
        CancellationToken ct;


        public DataReader()
        {
            factory = new();            
        }

        byte slaveID = 16;
        ushort startAddress = 0;
        ushort numOfPoints = 6;

        public async void StartReading()
        {
            tokenSource2 = new();
            ct = tokenSource2.Token;

            readingTask = Task.Run(() =>
            {
                using TcpClient tcpClient = new();
                tcpClient.Connect("192.168.10.254", 4001);

                using TcpClientAdapter adapter = new(tcpClient);

                using ModbusSerialMaster master = (ModbusSerialMaster)factory.CreateRtuMaster(adapter);

                while (true)
                {
                    ushort[] holding_register = master.ReadHoldingRegisters(slaveID, startAddress, numOfPoints);

                    OutputLog.That(ModbusUtility.GetSingle(holding_register[4], holding_register[5]).ToString());

                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(1000);
                }

            }, ct);
            
            try
            {
                await readingTask;
            }
            catch (Exception ex)
            {
                OutputLog.That("Чтение прервано!");
            }
            finally
            {
                tokenSource2.Dispose();
            }
        }

        public void StopReading() 
        {
            tokenSource2.Cancel();
        }
    }
}
