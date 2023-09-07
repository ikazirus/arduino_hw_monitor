import time
from gui import MainWindow
from monitor_performance import net_usage
from serial.tools import list_ports

interval = 0
port_list = list(list_ports.comports())

def main():
    while True:
        net_usage()
        #display_usage(psutil.cpu_percent()*10,psutil.virtual_memory().percent,30)
        time.sleep(interval)
    
if __name__ == "__main__":
    # main()
    # print(psutil.net_io_counters(pernic=True))
    # for port in port_list:
    #     print (f"{port.name} - {port.serial_number}")
    
    main_window = MainWindow()
    main_window.port_list = port_list
    main_window.open_window()
    