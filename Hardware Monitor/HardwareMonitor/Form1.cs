using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Collections;
using System.Timers;

namespace HardwareMonitor
{
    public partial class Form1 : Form
    {
        HardwareMonitor hardwareMonitor = null;
        SerialPort port = null;
        NM_Monitor nmMonitor;
        NM_Adapter[] arrAdapters;

        public Form1()
        {
            InitializeComponent();
            hardwareMonitor = new HardwareMonitor();
            port = new SerialPort();

            nmMonitor = new NM_Monitor();
            arrAdapters = nmMonitor.arrAdapters;


            try
            {
                notifyIcon1.Visible = false;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.RtsEnable = true;
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    comboBox1.Items.Add(port);
                }

                comboBox1.SelectedIndex = ports.Length - 1;
                port.BaudRate = 9600;

                comboBox2.SelectedIndex = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            hardwareMonitor.initHardware();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.PortName = comboBox1.Text;
                    port.Open();
                    timer1.Interval = Convert.ToInt32(comboBox2.Text);
                    timer1.Enabled = true;
                    toolStripStatusLabel1.Text = "Sending data...";
                    label2.Text = "Connected";
                    button2.Enabled = true;
                    button1.Enabled = false;
                    nmMonitor.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                port.Write("~");
                port.Close();
                button2.Enabled = false;
                button1.Enabled = true;
                nmMonitor.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            label2.Text = "Disconnected";
            timer1.Enabled = false;
            toolStripStatusLabel1.Text = "Connect to Arduino...";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Status();
        }


        private void Status()
        {
            hardwareMonitor.readData();
            try
            {
                port.Write(hardwareMonitor.cpuLoad + "!" + hardwareMonitor.gpuLoad + "@" + hardwareMonitor.cpuTemp + "#" + hardwareMonitor.gpuTemp + "$"+ arrAdapters[0].UploadSpeedMBps+"%"+ arrAdapters[0].DownloadSpeedMBps+"^");
                          

                label2.Text = "CPU Load:     " + hardwareMonitor.cpuLoad + " %" +
                             "\nCPU Temp:    " + hardwareMonitor.cpuTemp + " C" +
                             "\nCPU Power:   " + hardwareMonitor.cpuPower + " W" +

                             "\n\nGPU Load:     " + hardwareMonitor.gpuLoad + " %" +
                             "\nGPU Temp:    " + hardwareMonitor.gpuTemp + " C" +
                             "\nGPU Power:   " + hardwareMonitor.gpuPower + " W" +

                             "\n\nUpload:        " + arrAdapters[0].UploadSpeedMBps +
                             "\nDownload:    " + arrAdapters[0].DownloadSpeedMBps;


            }
            catch (Exception ex)
            {
                timer1.Stop();
                MessageBox.Show(ex.Message);
                toolStripStatusLabel1.Text = "Arduino's not responding...";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hardwareMonitor.openMonitor();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    public class HardwareMonitor
    {
        public string cpuLoad = "0.0";
        public string cpuTemp = "0.0";
        public string cpuPower = "0.0";

        public string gpuLoad = "0.0";
        public string gpuTemp = "0.0";
        public string gpuPower = "0.0";


        private Computer computer;

        public void initHardware()
        {
            computer = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };
        }

        public void openMonitor()
        {
            computer.Open();
        }

        public void closeMonitor()
        {
            computer.Close();
        }

        public void readData()
        {

            computer.Accept(new UpdateVisitor());
            foreach (IHardware hardware in computer.Hardware)
            {

                if (hardware.HardwareType == HardwareType.CPU)
                {
                    hardware.Update();
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                        {
                            cpuLoad = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }

                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            cpuTemp = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }

                        if (sensor.SensorType == SensorType.Power)
                        {
                            cpuPower = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    hardware.Update();

                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        {
                            gpuLoad = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }

                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            gpuTemp = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }

                        if (sensor.SensorType == SensorType.Power)
                        {
                            gpuPower = Math.Round(sensor.Value.GetValueOrDefault(), 1).ToString();
                        }
                    }
                }


            }
        }
    }

    public class NM_Adapter
    {

        internal NM_Adapter(string strName)
        {
            strAdapterName = strName;
        }

        private long lngDownloadSpeed;
        private long lngUploadSpeed;
        private long lngDownloadValue;
        private long lngUploadValue;
        private long lngOldDownloadValue;
        private long lngOldUploadValue;

        internal string strAdapterName;
        internal PerformanceCounter pcDownloadCounter;
        internal PerformanceCounter pcUploadCounter;

        internal void Initialize()
        {
            lngOldDownloadValue = pcDownloadCounter.NextSample().RawValue;
            lngOldUploadValue = pcUploadCounter.NextSample().RawValue;
        }

        internal void Update()
        {
            lngDownloadValue = pcDownloadCounter.NextSample().RawValue;
            lngUploadValue = pcUploadCounter.NextSample().RawValue;

            lngDownloadSpeed = lngDownloadValue - lngOldDownloadValue;
            lngUploadSpeed = lngUploadValue - lngOldUploadValue;

            lngOldDownloadValue = lngDownloadValue;
            lngOldUploadValue = lngUploadValue;
        }

        public override string ToString()
        {
            return this.strAdapterName;
        }

        public string AdapterName
        {
            get
            {
                return strAdapterName;
            }
        }

        public long DownloadSpeed
        {
            get
            {
                return lngDownloadSpeed;
            }
        }

        public long UploadSpeed
        {
            get
            {
                return lngUploadSpeed;
            }
        }

        public double DownloadSpeedKbps
        {
            get
            {
                return Math.Round(lngDownloadSpeed / 1024.0, 2);
            }
        }

        public double UploadSpeedKbps
        {
            get
            {
                return Math.Round(lngUploadSpeed / 1024.0, 2);
            }
        }

        public string DownloadSpeedMBps
        {
            get
            {
                double speed = lngDownloadSpeed*8 / 1024.0;
                String unit = " kbps";
                if (speed > 1024.0)
                {
                    speed = speed / 1024.0;
                    unit = " mbps";
                }
                return Math.Round(speed, 2) + unit;
            }
        }

        public String UploadSpeedMBps
        {
            get
            {
                double speed = lngUploadSpeed*8 / 1024.0;
                String unit = " kbps";
                if (speed > 1024.0)
                {
                    speed = speed / 1024.0;
                    unit = " mbps";
                }
                return Math.Round(speed, 2)+unit;
            }
        }
    }

    public class NM_Monitor
    {
        private System.Timers.Timer tmrTime;
        private ArrayList alAdapters;
        private ArrayList alAdaptersMonitored;

        public NM_Monitor()
        {
            alAdapters = new ArrayList();
            alAdaptersMonitored = new ArrayList();

            LoopAdapters();

            tmrTime = new System.Timers.Timer(1000);
            tmrTime.Elapsed += new ElapsedEventHandler(tmrTime_Elapsed);
        }

        private void LoopAdapters()
        {

            PerformanceCounterCategory pcNetworkInterface = new PerformanceCounterCategory("Network Interface");

            foreach (string tmpName in pcNetworkInterface.GetInstanceNames())
            {

                if (tmpName == "MS TCP Loopback interface")
                    continue;

                NM_Adapter netAdapter = new NM_Adapter(tmpName);

                netAdapter.pcDownloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", tmpName);
                netAdapter.pcUploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", tmpName);

                alAdapters.Add(netAdapter);
            }
        }

        private void tmrTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (NM_Adapter tmpAdapter in alAdaptersMonitored)
                tmpAdapter.Update();
        }

        public NM_Adapter[] arrAdapters
        {
            get
            {
                return (NM_Adapter[])alAdapters.ToArray(typeof(NM_Adapter));
            }
        }

        public void Start()
        {
            if (this.alAdapters.Count > 0)
            {
                foreach (NM_Adapter currAdapter in alAdapters)

                    if (!alAdaptersMonitored.Contains(currAdapter))
                    {
                        alAdaptersMonitored.Add(currAdapter);
                        currAdapter.Initialize();
                    }

                tmrTime.Enabled = true;
            }
        }

        public void Start(NM_Adapter nmAdapter)
        {
            if (!alAdaptersMonitored.Contains(nmAdapter))
            {
                alAdaptersMonitored.Add(nmAdapter);
                nmAdapter.Initialize();
            }

            tmrTime.Enabled = true;
        }

        public void Stop()
        {
            alAdaptersMonitored.Clear();
            tmrTime.Enabled = false;
        }

        public void Stop(NM_Adapter currAdapter)
        {
            if (alAdaptersMonitored.Contains(currAdapter))
                alAdaptersMonitored.Remove(currAdapter);

            if (alAdaptersMonitored.Count == 0)
                tmrTime.Enabled = false;
        }
    }
}
