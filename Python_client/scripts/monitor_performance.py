import time
import psutil
import serial


text_to_serial = "";

def send_data_via_serial(text):
    ser = serial.Serial("COM7", 9600)
   
    # Send character 'S' to start the program
    ser.write(bytearray(text_to_serial,'ascii'))

def net_usage(inf = "Ethernet 3"):   #change the inf variable according to the interface
    net_stat = psutil.net_io_counters(pernic=True, nowrap=True)[inf]
    net_in_1 = net_stat.bytes_recv
    net_out_1 = net_stat.bytes_sent
    time.sleep(1)
    net_stat = psutil.net_io_counters(pernic=True, nowrap=True)[inf]
    net_in_2 = net_stat.bytes_recv
    net_out_2 = net_stat.bytes_sent

    net_in = round((net_in_2 - net_in_1) / 1024 / 1024*8, 2)
    net_out = round((net_out_2 - net_out_1) / 1024 / 1024*8, 2)

    print(f"Current net-usage:\nIN: {net_in} mbps, OUT: {net_out} mbps")

def send_to_display():
    print("")
    

def display_usage(cpu_usage,mem_usage,bars=50):
    cpu_percentage = (cpu_usage/100.0)
    cpu_bar = '█'*int(cpu_percentage*bars)+'-'*(bars-int(cpu_percentage*bars))
    
    mem_percentage = (mem_usage/100.0)
    mem_bar = '█'*int(mem_percentage*bars)+'-'*(bars-int(mem_percentage*bars))

    print(f"\rCPU Usage: |{cpu_bar}| {cpu_usage:.2f}% ",end=" ")
    print(f"MEM Usage: |{mem_bar}| {mem_usage:.2f}% ",end="\r")
    

        