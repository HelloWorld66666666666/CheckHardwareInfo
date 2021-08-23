using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CheckHardwareInfo
{
    public class BaseClass
    {
        protected ManagementObjectSearcher searcher = new ManagementObjectSearcher();
        protected SelectQuery query;
        protected void SetSelectQuery(string ClassName, StringCollection stringCollection)
        {
            query = new SelectQuery(ClassName, "");
            query.SelectedProperties = stringCollection;

            searcher.Query = query;
        }

        public virtual void GetInfo() { }

        protected void SetPropertyStringList<T>(T t, string device)
        {
            StringCollection stringList = new StringCollection();
            foreach (var item in t.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty))
            {
                stringList.Add(item.Name);
            }

            query = new SelectQuery(device, "");
            query.SelectedProperties = stringList;

            searcher.Query = query;

            foreach (var item in searcher.Get())
            {
                foreach (var property in t.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty))
                {
                    property.SetValue(t, item[property.Name].ToString());
                }
            }

            searcher.Dispose();
            searcher = null;
            query = null;
        }

        protected virtual void SetPropertyList<T>(string device, List<T> list) where T : new()
        {
            if(list == null)
            {
                list = new List<T>();
            }

            StringCollection stringList = new StringCollection();
            foreach (var item in typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty))
            {
                stringList.Add(item.Name);
            }

            query = new SelectQuery(device, "");
            query.SelectedProperties = stringList;

            searcher.Query = query;

            foreach(var item in searcher.Get())
            {
                T t = new T();
                foreach (var property in t.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty))
                {
                    if(item[property.Name] == null)
                        property.SetValue(t, null);
                    else
                        property.SetValue(t, item[property.Name].ToString());
                }

                list.Add(t);
            }

            searcher.Dispose();
            searcher = null;
            query = null;
        }
    }

    public class Computer : BaseClass
    { 
        string Name { get; set; }
        string Workgroup { get; set; }
        string Uptime { get; set; }

        public Computer()
        {
            GetInfo();
        }

        public override void GetInfo()
        {
            Name = Environment.MachineName;

            try
            {
                Workgroup = Domain.GetComputerDomain().Name;
            }
            catch
            {
                Workgroup = "Workgroup";
            }

            TimeSpan span = DateTime.Now - DateTime.Now.AddMilliseconds(-Environment.TickCount);
            Uptime = string.Format("{0}小时{1}分{2}秒", span.Hours.ToString(), span.Minutes.ToString(), span.Seconds.ToString());
        }
    }

    public class OperatingSystem : BaseClass
    {
        string Name { get; set; }
        string Version { get; set; }
        string Architecture { get; set; }
        string DisplayLanguage { get; set; }

        public OperatingSystem()
        {
            GetInfo();
        }

        public override void GetInfo()
        {
            ComputerInfo info = new ComputerInfo();
            Name = info.OSFullName;
            Version = info.OSVersion;

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
            {
                Architecture = "AMD64";
            }
            else
            {
                Architecture = "AMD32";
            }

            DisplayLanguage = info.InstalledUICulture.DisplayName;
        }
    }

    public class Motherboard : BaseClass
    {
        string Manufacturer { get; set; }
        string Version { get; set; }
        string Product { get; set; }
        string SerialNumber { get; set; }

        public Motherboard()
        {
            GetInfo();
        }

        public override void GetInfo()
        {
            SetPropertyStringList<Motherboard>(this, "Win32_BaseBoard");
        }
    }

    public class Processor : BaseClass
    {
        string Name { get; set; }
        string Manufacturer { get; set; }
        string maxClockSpeed;
        string MaxClockSpeed
        {
            get
            {
                return maxClockSpeed;
            }
            set
            {
                double speed = Convert.ToDouble(value) / 100;
                maxClockSpeed = (Math.Ceiling(speed) / 10).ToString() + "GHz";
            }
        }
        string ThreadCount { get; set; }
        string NumberOfCores { get; set; }
        string cacheSize;
        string L3CacheSize
        {
            get
            {
                return cacheSize;
            }
            set
            {
                double size = Convert.ToDouble(value) / 1024;
                cacheSize = Math.Ceiling(size).ToString() + "M";
            }
        }
        string l2cachesize;
        string L2CacheSize
        {
            get
            {
                return l2cachesize;
            }
            set
            {
                double speed = Convert.ToDouble(value) / 1024;
                l2cachesize = Math.Ceiling(speed).ToString() + "M";
            }
        }

        public Processor()
        {
            GetInfo();   
        }

        public override void GetInfo()
        {
            SetPropertyStringList<Processor>(this, "Win32_Processor");
        }
    }

    public class BIOS : BaseClass
    { 
        string Manufacturer { get; set; }
        string Version { get; set; }
        string ReleaseDate { get; set; }
        string SMBIOSBIOSVersion { get; set; }

        public BIOS()
        {
            GetInfo();
        }

        public override void GetInfo()
        {
            SetPropertyStringList<BIOS>(this, "Win32_BIOS");
        }
    }

    public class Memory
    {
        string Manufacturer { get; set; }
        string capacity;
        string Capacity 
        { 
            get
            {
                return capacity;
            }
            set
            {
                ulong c = Convert.ToUInt64(value);
                c = c / 1024 / 1024 / 1024;

                capacity = c.ToString() + "G";
            } 
        }

        string speed;
        string Speed
        { 
            get
            {
                return speed;
            }
            set
            {
                speed = value + "MHz";
            }
        }

        string PartNumber { get; set; }

        string MemoryType;
        string SMBIOSMemoryType
        {
            get
            {
                return MemoryType;
            }
            set
            {
                switch(Convert.ToInt32(value))
                {
                    case 26:
                        {
                            MemoryType = "DDR4";
                            break;
                        }
                    default:
                        MemoryType = "Unknown";
                        break;
                }
            }
        }
    }

    public class Memorys : BaseClass
    {
        List<Memory> list { get; set; }

        public Memorys()
        {
            list = new List<Memory>();
            GetInfo();
        }

        public override void GetInfo()
        {
            SetPropertyList<Memory>("Win32_PhysicalMemory", list);
        }
    }

    public class LogicalDrive
    {
        string FileSystem { get; set; }

        string freeSpace;
        string FreeSpace
        {
            get
            {
                return freeSpace;
            }
            set
            {
                ulong free = Convert.ToUInt64(value) / 1024 / 1024 / 1024;
                freeSpace = free.ToString() + "G";
            }
        }

        string size;
        string Size
        {
            get
            {
                return size;
            }
            set
            {
                ulong s = Convert.ToUInt64(value) / 1024 / 1024 / 1024;
                size = s.ToString() + "G";
            }
        }

        string VolumeName { get; set; }
        string Name { get; set; }
    }

    public class LogicalDrives : BaseClass
    {
        List<LogicalDrive> list { get; set; }

        public LogicalDrives()
        {
            list = new List<LogicalDrive>();
            GetInfo();
        }

        public override void GetInfo()
        {
            SetPropertyList<LogicalDrive>("Win32_LogicalDisk", list);
        }
    }

    public class NetworkConnection
    {
        string Name { get; set; }
        string AdapterName { get; set; }
        string InterfaceType { get; set; }
        string ConnectionSpeed { get; set; }
        string macAddress;
        string MACAddress
        {
            get
            {
                return macAddress;
            }
            set
            {
                macAddress = value;
                for(int i = 2; i < macAddress.Length; i += 3)
                {
                    macAddress = macAddress.Insert(i, "-");
                }
            }
        }
        string DHCPEnabled { get; set; }
        string AllIPAddress { get; set; }

        private NetworkConnection()
        {

        }

        public static NetworkConnection GetInstance(NetworkInterface network)
        {
            NetworkConnection connection = new NetworkConnection();
            connection.Name = network.Name;
            connection.AdapterName = network.Description;
            connection.InterfaceType = network.NetworkInterfaceType.ToString();
            connection.ConnectionSpeed = (network.Speed / 1000 / 1000 / 1000).ToString() + "Gbps";
            connection.MACAddress = network.GetPhysicalAddress().ToString();
            if(network.GetIPProperties().DhcpServerAddresses.Count != 0)
            {
                connection.DHCPEnabled = "True";
                foreach(var dhcpIP in network.GetIPProperties().DhcpServerAddresses)
                {
                    connection.AllIPAddress += dhcpIP.ToString() + "\n";
                }
            }
            else
            {
                connection.DHCPEnabled = "False";
                foreach(var unicastIP in network.GetIPProperties().UnicastAddresses)
                {
                    connection.AllIPAddress += unicastIP.Address.ToString() + "\n";
                }

                foreach (var castIP in network.GetIPProperties().MulticastAddresses)
                {
                    connection.AllIPAddress += castIP.Address.ToString() + "\n";
                }
            }
            if(!string.IsNullOrEmpty(connection.AllIPAddress))
                connection.AllIPAddress = connection.AllIPAddress.Remove(connection.AllIPAddress.Length - 1, 1);
            return connection;
        }
    }

    public class NetworkConnections : BaseClass
    {
        List<NetworkConnection> list { set; get; }

        public NetworkConnections()
        {
            list = new List<NetworkConnection>();
            GetInfo();
        }

        public override void GetInfo()
        {
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                list.Add(NetworkConnection.GetInstance(item));
            }
        }
    }

    public class GraphicCard
    { 
        string Name { get; set; }
        string adapterRam;
        string AdapterRAM 
        { 
            get
            {
                return adapterRam;
            }
            set
            {
                long ram = Convert.ToInt64(value);
                ram = ram / 1024 / 1024 / 1024;

                adapterRam = ram.ToString() + "G";
            }
        }
        string AdapterDACType { get; set; }
        string DriverVersion { get; set; }
    }

    public class GraphicCards : BaseClass
    {
        List<GraphicCard> list { get; set; }
        public GraphicCards()
        {
            list = new List<GraphicCard>();
            GetInfo();
        }

        public override void GetInfo()
        {
            SetPropertyList<GraphicCard>("Win32_VideoController", list);
        }
    }

    public class SystemInfo
    {
        Computer computer { get; set; } = null;
        OperatingSystem operatingSystem { get; set; } = null;
        Motherboard motherboard { get; set; } = null;
        Processor processor { get; set; } = null;
        BIOS bios { get; set; } = null;
        GraphicCards graphicCards { get; set; } = null;
        Memorys memorys { get; set; } = null;
        LogicalDrives logicalDrivers { get; set; } = null;
        NetworkConnections networkConnections { get; set; } = null;

        public void CheckInfo(string info, GroupBox groupBox)
        {
            if (info == typeof(NetworkConnections).Name)
            {
                networkConnections = new NetworkConnections();
                SetInfoToGroupBox(networkConnections, groupBox);
            }
            else if(info == typeof(LogicalDrives).Name)
            {
                logicalDrivers = new LogicalDrives();
                SetInfoToGroupBox(logicalDrivers, groupBox);
            }
            else if(info == typeof(Memorys).Name)
            {
                memorys = new Memorys();
                SetInfoToGroupBox(memorys, groupBox);
            }
            else if(info == typeof(BIOS).Name)
            {
                bios = new BIOS();
                SetInfoToGroupBox(bios, groupBox);
            }
            else if(info == typeof(Processor).Name)
            {
                processor = new Processor();
                SetInfoToGroupBox(processor, groupBox);
            }
            else if(info == typeof(Motherboard).Name)
            {
                motherboard = new Motherboard();
                SetInfoToGroupBox(motherboard, groupBox);
            }
            else if(info == typeof(OperatingSystem).Name)
            {
                operatingSystem = new OperatingSystem();
                SetInfoToGroupBox(operatingSystem, groupBox);
            }
            else if(info == typeof(Computer).Name)
            {
                computer = new Computer();
                SetInfoToGroupBox(computer, groupBox);
            }
            else if(info == typeof(GraphicCards).Name)
            {
                graphicCards = new GraphicCards();
                SetInfoToGroupBox(graphicCards, groupBox);
            }
            else
            {

            }
        }

        private void SetInfoToGroupBox(BaseClass baseClass, GroupBox groupBox)
        {
            string className = baseClass.GetType().Name;
            groupBox.Header = className;
            groupBox.Margin = new System.Windows.Thickness(10, 5, 10, 5);
            groupBox.Background = new SolidColorBrush(Color.FromRgb(42, 56, 101));

            PropertyInfo[] infos = baseClass.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
            if (infos.Length == 1)
            {
                StackPanel stackPanel = new StackPanel();
                groupBox.Content = stackPanel;

                IEnumerable<object> baseLists = infos[0].GetValue(baseClass) as IEnumerable<object>;
                foreach(var item in baseLists)
                {
                    GroupBox box = new GroupBox();
                    box.Header = item.GetType().Name;
                    PropertyInfo[] listInfos = item.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
                    box.Content = GetGroupBoxContent(item, listInfos);
                    box.Margin = new System.Windows.Thickness(5 ,5, 5, 5);

                    stackPanel.Children.Add(box);
                }
            }
            else
            {
                groupBox.Content = GetGroupBoxContent(baseClass, infos);
            }
        }

        private object GetGroupBoxContent(object baseClass, PropertyInfo[] infos)
        {
            DataGrid dataGrid = GetTempDataGrid();
            ObservableCollection<Tuple<string, string>> collecions = new System.Collections.ObjectModel.ObservableCollection<Tuple<string, string>>();
            foreach (var item in infos)
            {
                Tuple<string, string> tuple;
                if (item.GetValue(baseClass) == null)
                {
                    tuple = new Tuple<string, string>(item.Name, "null");
                }
                else
                {
                    tuple = new Tuple<string, string>(item.Name, item.GetValue(baseClass).ToString()); ;
                }
                collecions.Add(tuple);
            }
            dataGrid.ItemsSource = collecions;

            Border border = GetTemplateBorder();
            border.Child = dataGrid;

            return border;
        }

        private DataGrid GetTempDataGrid()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Style = new MyDataGridStyle();
            dataGrid.ColumnHeaderStyle = new MyDataGridHeaderStyle();
            dataGrid.RowStyle = new MyDataGridRowStyle();
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "Type";
            column1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column1.Binding = new Binding("Item1");
            column1.Foreground = new SolidColorBrush(Color.FromRgb(255, 199, 56));
            column1.ElementStyle = new MyDataGridTextColumnElementStyle();
            dataGrid.Columns.Add(column1);
            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Value";
            column2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column2.Binding = new Binding("Item2");
            column2.Foreground = new SolidColorBrush(Color.FromRgb(255, 199, 56));
            column2.ElementStyle = new MyDataGridTextColumnElementStyle();
            dataGrid.Columns.Add(column2);

            return dataGrid;
        }

        private Border GetTemplateBorder()
        {
            Border border = new Border();
            border.Style = new MyBorderStyle();

            return border;
        }

        public SystemInfo()
        {
        }
    }
}
